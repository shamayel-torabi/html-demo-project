using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GcPdfViewerSupportApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    /*
                    webBuilder.UseStartup<Startup>().UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = 268435456; //(256 MB) 
                        options.Limits.MaxRequestBufferSize = 10485760; //(10 MB)
                    });
                    */
                    webBuilder.UseStartup<Startup>();
                });
    }
}
