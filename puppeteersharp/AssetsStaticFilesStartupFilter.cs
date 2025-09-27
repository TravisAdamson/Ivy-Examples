using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace PuppeteerSharpDemo
{
    public sealed class AssetsStaticFilesStartupFilter : IStartupFilter
    {
        private readonly string _requestPath;
        private readonly string _physicalPath;

        public AssetsStaticFilesStartupFilter(string requestPath, string physicalPath)
        {
            _requestPath = requestPath;   // e.g. "/assets"
            _physicalPath = physicalPath;  // e.g. Path.Combine(AppContext.BaseDirectory, "Assets")
            Directory.CreateDirectory(_physicalPath); // Ensure folder exists

        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(_physicalPath),
                    RequestPath = _requestPath,

                    // Optional: add `?download=1` to force browser download
                    OnPrepareResponse = ctx =>
                    {
                        if (ctx.Context.Request.Query.ContainsKey("download"))
                        {
                            var fileName = System.IO.Path.GetFileName(ctx.File.PhysicalPath);
                            ctx.Context.Response.Headers.Append(
                                "Content-Disposition", $"attachment; filename=\"{fileName}\"");
                        }
                    }
                });

                next(app);
            };
        }
    }
}
