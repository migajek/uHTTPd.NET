using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uHttpd;
using uHttpd.Exceptions;
using uHttpd.RequestHandlers;

namespace uHttpdApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = args.FirstOrDefault() ?? AppDomain.CurrentDomain.BaseDirectory;
            var http = new uHttpServer();
            http.InitDefaults(dir);
            http.Handlers.Add(new DelegateWithParamsHandler(@"^/hello,{name:\w+}$",
                delegate(HttpListenerRequest request,HttpListenerResponse response, Dictionary<string, string> dictionary)
                {
                    response.ContentType = "text/html";                    
                    return String.Format("Hi there, <b>{0}</b>", dictionary["name"]);
                }));
            http.ExceptionHandlers.OfType<DefaultHttpExceptionHandler>().Single().ShowStackTrace = true;
            http.Start("http://localhost:8090/");
            Console.WriteLine("µHTTPd Listening / dir {0}... ", dir);
            Console.ReadLine();
        }
    }
}
