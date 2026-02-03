using System.Web.Http;

namespace GcPdfViewerSupportApiDemo.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/pdf-viewer/{action}/{id}/{id2}/{id3}",
                defaults: new { controller = "SampleSupport", id = RouteParameter.Optional, id2 = RouteParameter.Optional, id3 = RouteParameter.Optional }
            );
        }
    }
}