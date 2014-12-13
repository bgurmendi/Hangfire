// This file is part of Hangfire.
// Copyright � 2013-2014 Sergey Odinokov.
// 
// Hangfire is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// Hangfire is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with Hangfire. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire.Annotations;
using Hangfire.Common;
using Hangfire.Storage;

namespace Hangfire.States
{
    internal class StateChangeProcess : IStateChangeProcess
    {
        private readonly IStorageConnection _connection;
        private readonly StateHandlerCollection _handlers;

        private readonly Func<Job, IEnumerable<JobFilter>> _getFiltersThunk
            = JobFilterProviders.Providers.GetFilters;

        public StateChangeProcess([NotNull] IStorageConnection connection, [NotNull] StateHandlerCollection handlers)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (handlers == null) throw new ArgumentNullException("handlers");

            _connection = connection;
            _handlers = handlers;
        }

        internal StateChangeProcess(
            IStorageConnection connection, 
            StateHandlerCollection handlers, 
            IEnumerable<object> filters)
            : this(connection, handlers)
        {
            if (filters == null) throw new ArgumentNullException("filters");

            _getFiltersThunk = md => filters.Select(f => new JobFilter(f, JobFilterScope.Type, null));
        }

        public bool ChangeState(IStateMachine stateMachine, StateContext context, IState toState, string oldStateName)
        {
            try
            {
                var filterInfo = GetFilters(context.Job);
                var electStateContext = new ElectStateContext(context, _connection, stateMachine, toState, oldStateName);
                
                foreach (var filter in filterInfo.ElectStateFilters)
                {
                    filter.OnStateElection(electStateContext);
                }

                var applyStateContext = new ApplyStateContext(
                    context, 
                    electStateContext.CandidateState, 
                    oldStateName,
                    electStateContext.TraversedStates);

                ApplyState(applyStateContext, filterInfo.ApplyStateFilters);

                // State transition has been succeeded.
                return true;
            }
            catch (Exception ex)
            {
                var failedState = new FailedState(ex)
                {
                    Reason = "An exception occurred during the transition of job's state"
                };

                var applyStateContext = new ApplyStateContext(context, failedState, oldStateName, Enumerable.Empty<IState>());

                // We should not use any state changed filters, because
                // some of the could cause an exception.
                ApplyState(applyStateContext, Enumerable.Empty<IApplyStateFilter>());

                // State transition has been failed due to exception.
                return false;
            }
        }

        private void ApplyState(ApplyStateContext context, IEnumerable<IApplyStateFilter> filters)
        {
            using (var transaction = _connection.CreateWriteTransaction())
            {
                foreach (var state in context.TraversedStates)
                {
                    transaction.AddJobState(context.JobId, state);
                }

                foreach (var handler in _handlers.GetHandlers(context.OldStateName))
                {
                    handler.Unapply(context, transaction);
                }

                foreach (var filter in filters)
                {
                    filter.OnStateUnapplied(context, transaction);
                }

                transaction.SetJobState(context.JobId, context.NewState);

                foreach (var handler in _handlers.GetHandlers(context.NewState.Name))
                {
                    handler.Apply(context, transaction);
                }

                foreach (var filter in filters)
                {
                    filter.OnStateApplied(context, transaction);
                }

                if (context.NewState.IsFinal)
                {
                    transaction.ExpireJob(context.JobId, context.JobExpirationTimeout);
                }
                else
                {
                    transaction.PersistJob(context.JobId);
                }

                transaction.Commit();
            }
        }

        private JobFilterInfo GetFilters(Job job)
        {
            return new JobFilterInfo(_getFiltersThunk(job));
        }
    }
}