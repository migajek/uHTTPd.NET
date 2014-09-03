using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uHttpd.Exceptions;
using uHttpd.Utils;

namespace uHttpd.RequestHandlers
{
    public class FileSystemHandler : FileSystemHandlerBase
    {        
        private readonly bool _handleErrors;

        public FileSystemHandler(string rootDir, bool handleErrors = true): base(rootDir)
        {            
            _handleErrors = handleErrors;
        }
        public override async Task<bool> HandleRequest(System.Net.HttpListenerRequest request, System.Net.HttpListenerResponse response)
        {
            var fn = GetPathFromRequest(request);
            if (!File.Exists(fn))
            {
                if (_handleErrors)
                    throw new HttpException(404);
                return false;
            }
            using (var fs = File.OpenRead(fn))
            {
                response.ContentType = MimeHelper.GetMimeType(Path.GetExtension(fn));
                response.ContentLength64 = fs.Length;
                await fs.CopyToAsync(response.OutputStream);
                return true;
            }
        }

        public override int Priority { get { return 100; }}
    }
}
