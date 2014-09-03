using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace uHttpd.Exceptions
{
    public interface IHttpExceptionHandler
    {
        int Priority { get;  }
        bool HandleException(HttpException exception, HttpListenerRequest request, HttpListenerResponse response);
    }
}
