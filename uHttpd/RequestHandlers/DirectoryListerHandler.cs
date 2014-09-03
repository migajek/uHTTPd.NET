using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uHttpd.RequestHandlers
{
    internal struct Entry
    {
        public enum EntryType
        {
            Directory,
            File
        };

        public string Name { get; set; }
        public EntryType Type { get; set; }
    }

    public class DirectoryListerHandler: FileSystemHandlerBase
    {        
        public DirectoryListerHandler(string rootDir): base(rootDir)
        {            
        }

        public override async Task<bool> HandleRequest(System.Net.HttpListenerRequest request,
            System.Net.HttpListenerResponse response)
        {
            var fn = GetPathFromRequest(request);
            if (!Directory.Exists(fn)) 
                return false;
            var isRoot = String.IsNullOrEmpty(request.RawUrl) || request.RawUrl == "/";
            using (var writer = new StreamWriter(response.OutputStream))
            {
                await writer.WriteAsync("<html><head></head><body>\n");
                await writer.WriteAsync(String.Format("<h1>Directory {0}</h1>\n\n<ul>\n", request.RawUrl));
                var dinfo = new DirectoryInfo(fn);
                var files =
                    dinfo.EnumerateDirectories()
                        .OrderBy(x => x.Name)
                        .Select(x => new Entry()
                        {
                            Name = x.Name,
                            Type = Entry.EntryType.Directory
                        })
                        .Union(dinfo.EnumerateFiles()
                            .OrderBy(x => x.Name)
                            .Select(x => new Entry()
                            {
                                Name = x.Name,
                                Type = Entry.EntryType.File
                            }));
                if (!isRoot)
                {
                    await writer.WriteAsync("<li><a href=\"./..\">[...]</a></li>\n");
                }
                foreach (var file in files)
                {
                    await writer.WriteAsync(String.Format("<li><a href=\"{0}{1}\">{1}</a></li>\n", isRoot ? "" : request.RawUrl+"/", file.Name));
                }
                await writer.WriteAsync("</ul>");

                await writer.WriteAsync("</html>");

            }
            return false;
        }

        public override int Priority
        {
            get { return 200; }
        }
    }
}
