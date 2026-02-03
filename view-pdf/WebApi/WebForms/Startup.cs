// Ignore Spelling: Gc Api Pdf

using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Controllers;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Connection;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Models;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System;
using System.Web;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

[assembly: OwinStartup(typeof(GcPdfViewerSupportApiDemo.Startup))]
namespace GcPdfViewerSupportApiDemo
{
    public class Startup
    {

        const int MAX_MESSAGE_SIZE = 268435456/*256MB*/;

        static Startup()
        {
            Licensing.AddLicenseKeys();
        }
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            GcPdfViewerHub.Configure(app);
            GcPdfViewerController.Settings.VerifyToken += VerifyAuthToken;
            GcPdfViewerController.Settings.Sign += OnSign;
            GcPdfViewerController.Settings.ResolveFont += OnResolveFont;
            GcPdfViewerController.Settings.DocumentInitialized += OnDocumentInitialized;
        }

        private void OnDocumentInitialized(object sender, DocumentInitializedEventArgs e)
        {
			// Resolve the "FantaisieArtistique" font using the GcPdfDocument's FontCollection property.
			
            var doc = e.Document;
            // Create a FontCollection instance:
            FontCollection fc = new FontCollection();
            // Get the font file using RegisterFont method:
            string projectRootPath = HttpContext.Current.Server.MapPath("~");
            string fontPath = Path.Combine(projectRootPath, "assets/FantaisieArtistique.ttf");
            // Font license: https://www.1001fonts.com/fantaisieartistique-font.html#license
            // (The Font is Free for commercial use)
            fc.RegisterFont(fontPath);
            // Allow the SupportApi
            // to find the specified fonts in the font collection:
            doc.FontCollection = fc;

        }

        private void OnResolveFont(object sender, ResolveFontEventArgs e)
        {
			// Utilize the ResolvedFont event argument property to resolve the "BudmoJiggler-Regular" font.
			
            string projectRootPath = HttpContext.Current.Server.MapPath("~");
            if (e.FontName == "BudmoJiggler-Regular")
            {
                string fontPath = Path.Combine(projectRootPath, "assets/budmo_jiggler.ttf");
                // https://www.1001fonts.com/budmo-font.html#license
                // (The Font is Free for commercial use)
                e.ResolvedFont = Font.FromFile(fontPath);
            }
        }

        private void VerifyAuthToken(object sender, VerifyTokenEventArgs e)
        {
            // Here is an example of how you can validate the authentication token
            // provided during viewer initialization - new GcPdfViewer(selector, { supportApi: { token: "support-api-demo" }});.
            string token = e.Token;
            if (string.IsNullOrEmpty(token) || !token.StartsWith("support-api-demo"))
            {
                e.Reject = true;
            }
        }

        private void OnSign(object sender, SignEventArgs e)
        {
            // Example: Implement PDF document signing

            var signatureProperties = e.SignatureProperties;

            string projectRootPath = HttpContext.Current.Server.MapPath("~");
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

        public static string GetVersionString(Type t)
        {
            Version version = t.Assembly.GetName().Version;

            string versionString = version.Major + "." + version.Minor + "." + version.Build;

            if (version.Revision > 0)
            {
                versionString += "-" + version.Revision;
            }
            return versionString;
        }
    }
}
