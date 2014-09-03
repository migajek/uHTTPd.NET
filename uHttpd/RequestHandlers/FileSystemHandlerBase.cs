using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace uHttpd.RequestHandlers
{
    public abstract class FileSystemHandlerBase: IRequestHandler
    {
        public string RootDir { get; set; }

        protected FileSystemHandlerBase(string rootDir)
        {
            RootDir = rootDir;
        }

        public string GetPathFromRequest(HttpListenerRequest request)
        {
            var url = request.Url.AbsolutePath;
            var pathPart = url.Replace('/', Path.DirectorySeparatorChar);
            var result = Path.GetFullPath(Path.Combine(RootDir, ".\\" + pathPart));
            return result;
        }
        public abstract Task<bool> HandleRequest(System.Net.HttpListenerRequest request,
            System.Net.HttpListenerResponse response);

        public abstract int Priority { get; }
    }
}
