using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using uHttpd.Exceptions;
using uHttpd.RequestHandlers;

namespace uHttpd
{
    public class uHttpServer
    {
        private readonly HttpListener _httpListener;       
        public IList<IRequestHandler>  Handlers = new List<IRequestHandler>();
        public IList<IHttpExceptionHandler> ExceptionHandlers = new List<IHttpExceptionHandler>();

        private int _totalAccepts;
        public uHttpServer(int acceptPerCore = 4)
        {
            _totalAccepts = acceptPerCore * Environment.ProcessorCount;
            _httpListener = new HttpListener();            
        }

        /// <summary>
        /// Adds file & directory listing handler for specified root path.
        /// This is simple convenience method that registers both handlers & default exception handler.
        /// </summary>
        /// <param name="rootDir"></param>
        /// <param name="enableDirectoryListing"></param>
        public void InitDefaults(string rootDir, bool enableDirectoryListing = true)
        {
            if (!Directory.Exists(rootDir))
                throw new ArgumentException(String.Format("Specified directory {0} does not exist", rootDir));
            Handlers.Add(new FileSystemHandler(rootDir, true));
            if (enableDirectoryListing)
                Handlers.Add(new DirectoryListerHandler(rootDir));
            ExceptionHandlers.Add(new DefaultHttpExceptionHandler());
        }
        
        public void Start(params string[] listenAt)
        {
            foreach (var prefix in listenAt)
            {
                _httpListener.Prefixes.Add(prefix);
            }            
            
            new Thread(() =>
            {
                _httpListener.Start();

                var sem = new Semaphore(_totalAccepts, _totalAccepts);
                while (true)
                {
                    sem.WaitOne();
                    _httpListener.GetContextAsync().ContinueWith(async t =>
                    {
                        string errMessage;

                        try
                        {
                            sem.Release();
                            var context = await t;
                            await HandleRequest(context);
                            return;
                        }
                        catch (Exception ex)
                        {
                            errMessage = ex.ToString();
                        }

                        await Console.Error.WriteLineAsync(errMessage);
                    });
                }
            })
            {
                IsBackground = true
            }.Start();
        }

        protected async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                context.Response.AppendHeader(@"X-Powered-By", @"Migajek's uHttpServer");
                foreach (var requestHandler in Handlers.OrderByDescending(x => x.Priority))
                {
                    var handled = await requestHandler.HandleRequest(context.Request, context.Response);
                    if (handled)
                        break;
                }
            }
            catch (HttpException httpException)
            {
                var handled = false;
                foreach (var httpExceptionHandler in ExceptionHandlers.OrderByDescending(x => x.Priority))
                {
                    handled = httpExceptionHandler.HandleException(httpException, context.Request, context.Response);
                    if (handled)
                        break;
                }
                if (!handled)
                    throw;
            }
            finally
            {
                context.Response.Close();
            }
        }
    }
}
