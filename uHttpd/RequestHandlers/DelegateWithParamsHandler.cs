using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace uHttpd.RequestHandlers
{
    public class DelegateWithParamsHandler: DelegateRegexHandler
    {
        private Func<HttpListenerRequest, Dictionary<string, string>, object> _paramsHandler;

        private static Regex converter = new Regex(@"\{(?<name>[\w\d]+)\:(?<pattern>[^\}]+)\}");
        /// <summary>
        /// Converts {Name:pattern} to regex syntax
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>       
        private static string ConvertToRegex(string pattern)
        {
            return converter.Replace(pattern, "(?<${name}>${pattern})");
        }

        public DelegateWithParamsHandler(string pattern, Func<HttpListenerRequest, Dictionary<string, string>, object> handler)
            : base(ConvertToRegex(pattern), null)
        {
            this._paramsHandler = handler;
            this._handler = Handler;
        }

        private object Handler(HttpListenerRequest httpListenerRequest, Match match)
        {
            var data = _urlRegex.GetGroupNames().Where(g => g != "0").ToDictionary(@group => @group, @group => match.Groups[@group].Value);
            return this._paramsHandler(httpListenerRequest, data);
        }        
    }
}
