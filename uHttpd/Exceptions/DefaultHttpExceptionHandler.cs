using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uHttpd.Exceptions
{
    public class DefaultHttpExceptionHandler: IHttpExceptionHandler
    {
        public bool ShowStackTrace { get; set; }
        public bool HandleException(HttpException exception, System.Net.HttpListenerRequest request, System.Net.HttpListenerResponse response)
        {
            response.StatusCode = exception.StatusCode;
            response.ContentType = "text/html";
            using (var writer = new StreamWriter(response.OutputStream))
            {
                writer.Write(String.Format("HTTP {0}<br>", response.StatusCode));
                if (ShowStackTrace)
                {
                    writer.Write(exception.StackTrace.Replace("\n", "<br>"));
                }
            }
            return true;
        }

        public int Priority
        {
            get { return 100; }
        }
    }
}
