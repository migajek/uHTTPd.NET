using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace uHttpd.RequestHandlers
{
    public class DelegateRegexHandler: DelegateHandlerBase
    {
        protected Regex _urlRegex;
        protected Func<HttpListenerRequest, Match, object> _handler;
        public DelegateRegexHandler(Regex urlRegex, Func<HttpListenerRequest, Match, object> handler)
        {
            _urlRegex = urlRegex;
            _handler = handler;
        }

        public DelegateRegexHandler(string pattern, Func<HttpListenerRequest, Match, object> handler): this(new Regex(pattern), handler)
        {}
        

        protected override object ExecuteHandler(System.Net.HttpListenerRequest request, System.Net.HttpListenerResponse response)
        {
            return _handler(request, _urlRegex.Match(request.Url.AbsolutePath));
        }

        protected override bool UrlMatches(string url)
        {
            return _urlRegex.IsMatch(url);
        }
    }
}
