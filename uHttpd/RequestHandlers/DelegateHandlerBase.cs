using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace uHttpd.RequestHandlers
{
    public abstract class DelegateHandlerBase : IRequestHandler
    {
        protected abstract object ExecuteHandler(System.Net.HttpListenerRequest request,
            System.Net.HttpListenerResponse response);

        public async Task<bool> HandleRequest(HttpListenerRequest request,
            System.Net.HttpListenerResponse response)
        {
            var matches = UrlMatches(request.Url.AbsolutePath);
            if (!matches)
                return false;
            var result = ExecuteHandler(request, response);
            if (result == null)
            {
                response.StatusCode = 204;
            }
            else if (result is string)
            {                
                if (String.IsNullOrEmpty(response.ContentType))
                    response.ContentType = "text/plain";
                using (var writer = new StreamWriter(response.OutputStream))
                    await writer.WriteAsync(result as string);
            }
            else if (result is Stream)
            {                
                await (result as Stream).CopyToAsync(response.OutputStream);
            }
            else
                throw new InvalidOperationException();
            return true;
        }

        protected abstract bool UrlMatches(string url);

        public virtual int Priority
        {
            get { return 500; }
        }
    }
}
