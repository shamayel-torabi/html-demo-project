using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Controllers;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Models;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using GrapeCity.Documents.Pdf;
using System.Reflection;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Connection;
using Microsoft.AspNetCore.Http;

namespace GcPdfViewerSupportApiDemo
{
    public class Startup
    {

        static Startup()
        {
#if DEBUG
            // VS's F5 sets the current working directory to project dir rather than bin dir.
            var exePath = new System.Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath;
            var directory = System.IO.Path.GetDirectoryName(exePath);
            System.IO.Directory.SetCurrentDirectory(directory);
#endif
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var culture = configuration.GetValue("Culture", "");
            if (!string.IsNullOrEmpty(culture))
                GrapeCity.Documents.Pdf.ViewerSupportApi.Localization.Localizer.Culture = System.Globalization.CultureInfo.GetCultureInfo(culture);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("WebCorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => RequestVerifier.IsOriginAllowed(origin))
                .AllowCredentials();
            }));
            services.AddRazorPages();
            services.AddMvc((opts) => { opts.EnableEndpointRouting = false; });
            services.AddRouting();
            GrapeCity.Documents.Pdf.ViewerSupportApi.Connection.GcPdfViewerHub.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("WebCorsPolicy");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GcPdfViewerHub>("/signalr", opts =>
                {
                    opts.TransportMaxBufferSize = 268435456L; opts.ApplicationMaxBufferSize = 268435456L;
                });
            });
            
            GcPdfViewerController.Settings.VerifyToken += VerifyAuthToken;
            GcPdfViewerController.Settings.Sign += OnSign;
            GcPdfViewerController.Settings.ResolveFont += OnResolveFont;
            GcPdfViewerController.Settings.DocumentInitialized += OnDocumentInitialized;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseAuthorization();
            app.UseMvcWithDefaultRoute();
        }

        private void OnDocumentInitialized(object sender, DocumentInitializedEventArgs e)
        {     
            /*

            var doc = e.Document;
            
            // Create a FontCollection instance:
            FontCollection fc = new FontCollection();

            // Add system fonts to collection:
            fc.AppendFonts(FontCollection.SystemFonts, false);

            // Resolve the "FantaisieArtistique" font using the GcPdfDocument's FontCollection property.
            // Get the font file using RegisterFont method:
            string projectRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fontPath = Path.Combine(projectRootPath, "assets/FantaisieArtistique.ttf");
            // Font license: https://www.1001fonts.com/fantaisieartistique-font.html#license
            // (The Font is Free for commercial use)
            fc.RegisterFont(fontPath);

            // Allow the SupportApi to find the specified fonts in the font collection:
            doc.FontCollection = fc;

            */
            
        }

        private void OnResolveFont(object sender, ResolveFontEventArgs e)
        {
            /*

            // Utilize the ResolvedFont event argument property to resolve the "BudmoJiggler-Regular" font.

            string projectRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (e.FontName == "BudmoJiggler-Regular")
            {
                string fontPath = Path.Combine(projectRootPath, "assets/budmo_jiggler.ttf");
                // Font license: https://www.1001fonts.com/budmo-font.html#license
                // (The Font is Free for commercial use)
                e.ResolvedFont = Font.FromFile(fontPath);
            }

            */
        }

        private void VerifyAuthToken(object sender, VerifyTokenEventArgs e)
        {
            if(!RequestVerifier.VerifySecurityToken(e.ControllerContext.HttpContext.Request, e.Token))
            {
                e.Reject = true;
            }
        }


        private void OnSign(object sender, SignEventArgs e)
        {
            // Example: Implement PDF document signing

            var signatureProperties = e.SignatureProperties;

            string projectRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string certificatePath = Path.Combine(projectRootPath, "assets/GcPdfTest.pfx");
            byte[] certificateBytes = File.ReadAllBytes(certificatePath);
            if (certificateBytes == null)
            {
                e.Reject = true;
                return;
            }
            X509Certificate2 certificate = new X509Certificate2(
                certificateBytes, "qq", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            signatureProperties.SignatureBuilder = new GrapeCity.Documents.Pdf.Pkcs7SignatureBuilder()
            {
                CertificateChain = new X509Certificate2[] { certificate },
                HashAlgorithm = GrapeCity.Documents.Pdf.Security.OID.HashAlgorithms.SHA512,
                Format = GrapeCity.Documents.Pdf.Pkcs7SignatureBuilder.SignatureFormat.adbe_pkcs7_detached
            };

            string fontPath = Path.Combine(projectRootPath, "assets/arialuni.ttf");
            signatureProperties.SignatureAppearance = new SignatureAppearance
            {
                TextFormat = new TextFormat
                {
                    Font = Font.FromFile(fontPath),
                    FontSize = 5.5f
                }
            };


        }


    }
}
