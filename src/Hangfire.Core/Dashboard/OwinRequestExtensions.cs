using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.Text;
using Microsoft.Owin.Helpers;
using System.Reflection;
using System.Web;
using System.Collections.Generic;

namespace Hangfire.Dashboard
{
    internal static class OwinRequestExtensions
    {
        private const string FormCollectionKey = "Microsoft.Owin.Form#collection";

        /// <summary>
        /// Hack to prevent "Unable to cast object of type 'Microsoft.Owin.FormCollection' 
        /// to type 'Microsoft.Owin.IFormCollection'" exception, when internalized version
        /// does not match the current project's one.
        /// </summary>
        public static async Task<IFormCollection> ReadFormSafeAsync(this IOwinContext context)
        {          

            var inputStream = context.Request.Body;
            var t = inputStream.GetType();
            //var disableBuffering = t.GetMethod("DisableBuffering",BindingFlags.NonPublic | BindingFlags.Instance);
            //disableBuffering.Invoke(context.Request.Body, null);

            //var f_stream = t.GetField("_stream",BindingFlags.NonPublic | BindingFlags.Instance);
            //var _stream = (Stream) f_stream.GetValue(inputStream);
            var f_request = t.GetField("_request",BindingFlags.NonPublic | BindingFlags.Instance);
            var _request = (HttpRequestBase) f_request.GetValue(inputStream);


            var dic = new Dictionary<string, string[]>();
            foreach (var k in _request.Form.AllKeys)
            {                
                var v = _request.Form.Get(k);
                var values_array = v.Split(',');
                dic.Add(k, values_array);
            }
            var form = new FormCollection(dic);
            return form;


        } 

        /*
        private static async Task<IFormCollection> ReadFormAsync(IOwinContext context)
        {
            var form = context.Get<IFormCollection>("Microsoft.Owin.Form#collection");
            if (form == null)
            {
                string text;
                // Don't close, it prevents re-winding.


                //var stream2 = (Microsoft.Owin.Host.SystemWeb.CallStreams.InputStream) context.Request.Body;

                //llamar: DisableBuffering();

                var inputStream = context.Request.Body;
                var t = inputStream.GetType();
                //var disableBuffering = t.GetMethod("DisableBuffering",BindingFlags.NonPublic | BindingFlags.Instance);
                //disableBuffering.Invoke(context.Request.Body, null);

                var f_stream = t.GetField("_stream",BindingFlags.NonPublic | BindingFlags.Instance);
                var _stream = (Stream) f_stream.GetValue(inputStream);

                var f_request = t.GetField("_request",BindingFlags.NonPublic | BindingFlags.Instance);
                var _request = (HttpRequestBase) f_request.GetValue(inputStream);




                // http://www.symbolsource.org/MyGet/Metadata/aspnetwebstacknightlyrelease/Project/Microsoft.Owin.Host.SystemWeb/3.0.0-beta2-30512-033-rel/Release/.NETFramework,Version=v4.5/Microsoft.Owin.Host.SystemWeb/Microsoft.Owin.Host.SystemWeb/Microsoft.Owin.Host.SystemWeb/CallStreams/InputStream.cs
                // https://github.com/mono/mono/blob/b7a308f660de8174b64697a422abfc7315d07b8c/mcs/class/System.Web/System.Web/HttpRequestWrapper.cs





                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4 * 1024, leaveOpen: true))
                {
                    text = await reader.ReadToEndAsync();
                }
                form = WebHelpers.ParseForm(text);
                context.Set("Microsoft.Owin.Form#collection", form);
            }

            return form;
        }
        */
    }
}
