using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace uHttpd
{
    public interface IRequestHandler
    {
        Task<bool> HandleRequest(HttpListenerRequest request, HttpListenerResponse response);
        int Priority { get; }
    }
}
