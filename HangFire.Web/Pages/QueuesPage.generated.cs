﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HangFire.Web.Pages
{
    
    #line 2 "..\..\Pages\QueuesPage.cshtml"
    using System;
    
    #line default
    #line hidden
    using System.Collections.Generic;
    
    #line 3 "..\..\Pages\QueuesPage.cshtml"
    using System.Linq;
    
    #line default
    #line hidden
    using System.Text;
    
    #line 4 "..\..\Pages\QueuesPage.cshtml"
    using Pages;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class QueuesPage : RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");






            
            #line 6 "..\..\Pages\QueuesPage.cshtml"
  
    Layout = new LayoutPage { Title = "Queues" };


            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 10 "..\..\Pages\QueuesPage.cshtml"
 foreach (var queue in JobStorage.Queues())
{

            
            #line default
            #line hidden
WriteLiteral("    <div class=\"panel panel-default\">\r\n        <div class=\"panel-heading\">\r\n     " +
"       <h3 class=\"panel-title\">\r\n                ");


            
            #line 15 "..\..\Pages\QueuesPage.cshtml"
           Write(queue.QueueName);

            
            #line default
            #line hidden
WriteLiteral("    </h3>\r\n        </div>\r\n        <div class=\"panel-body\">\r\n            \r\n\r\n    " +
"        <dl class=\"dl-horizontal\">\r\n                <dt>Queue length:</dt>\r\n    " +
"            <dd>");


            
            #line 22 "..\..\Pages\QueuesPage.cshtml"
               Write(queue.Length);

            
            #line default
            #line hidden
WriteLiteral("</dd>\r\n                <dt>Servers:</dt>\r\n                <dd>");


            
            #line 24 "..\..\Pages\QueuesPage.cshtml"
               Write(String.Join(", ", queue.Servers));

            
            #line default
            #line hidden
WriteLiteral(@"</dd>
            </dl>

            <table class=""table"">
                <thead>
                    <tr>
                        <th>Type</th>
                        <th>Args</th>
                        <th>Enqueued At</th>
                    </tr>
                </thead>
                <tbody>
");


            
            #line 36 "..\..\Pages\QueuesPage.cshtml"
                     foreach (var job in queue.FirstJobs)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <tr>\r\n                            <td>");


            
            #line 39 "..\..\Pages\QueuesPage.cshtml"
                           Write(HtmlHelper.JobType(job.Value.Type));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                            <td><code>\r\n                                  " +
"  ");


            
            #line 41 "..\..\Pages\QueuesPage.cshtml"
                               Write(HtmlHelper.FormatProperties(job.Value.Args));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </code></td>\r\n                            <td>");


            
            #line 43 "..\..\Pages\QueuesPage.cshtml"
                           Write(job.Value.EnqueuedAt);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                        </tr>\r\n");


            
            #line 45 "..\..\Pages\QueuesPage.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral(@"                </tbody>
            </table>
            
            <a href=""#"">Enqueued jobs</a>
            
        </div>
        <div class=""panel-footer"">
            <div class=""btn-toolbar pull-right"">
                <button class=""btn btn-default btn-sm"">
                    <span class=""glyphicon glyphicon-trash""></span>
                    Clear
                </button>
                <button class=""btn btn-danger btn-sm"">
                    <span class=""glyphicon glyphicon-remove""></span>
                </button>
            </div>

            <div class=""clearfix""></div>
        </div>
    </div>
");


            
            #line 66 "..\..\Pages\QueuesPage.cshtml"
}

            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591