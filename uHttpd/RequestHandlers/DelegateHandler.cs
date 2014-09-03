using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace uHttpd.RequestHandlers
{
    
    public class DelegateHandler: DelegateHandlerBase
    {
        public enum UrlMatchMethodType
        {
            StartsWith,
            ExactMatch        
        }

        private readonly Func<HttpListenerRequest, object> _handler;
        private string _url;

        public UrlMatchMethodType UrlMatchMethod { get; set; }
        

        public DelegateHandler(string url, Func<HttpListenerRequest, object> handler)
        {
            _url = url;
            _handler = handler;
        }

        protected override bool UrlMatches(string url)
        {
            switch (UrlMatchMethod)
            {
                case UrlMatchMethodType.ExactMatch:
                    return url == _url;                
                case UrlMatchMethodType.StartsWith:
                    return url.StartsWith(_url);
            }
            return false;
        }

        
        protected override object ExecuteHandler(HttpListenerRequest request, HttpListenerResponse response)
        {
            return _handler(request);
        }
    }
}
