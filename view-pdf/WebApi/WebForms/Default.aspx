<%@ Page Title="SupportApi Demo Service" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" ValidateRequest="false" Inherits="GcPdfViewerSupportApiDemo._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<h1>
    SupportApi Demo Service (WebForms/Web API 2) v<%=GcPdfViewerSupportApiDemo.Startup.GetVersionString(typeof(GrapeCity.Documents.Pdf.ViewerSupportApi.Controllers.GcPdfViewerController))%> (DsPdf v<%=GcPdfViewerSupportApiDemo.Startup.GetVersionString(typeof(GrapeCity.Documents.Pdf.GcPdfDocument))%>)
</h1>
<p>
    Support API is a server-side ASP.​NET library that ships as C# source code with DsPdf,
    and allows to easily set up a server that uses GcPdf to provide PDF modification features to DsPdfViewer.
</p>
<p>
    When connected to Support API, DsPdfViewer can be used as a PDF editor to save filled PDF forms,
    remove sensitive content, edit annotations and forms, and more.
</p>
<p>
    <a target="_blank" href="/api/pdf-viewer/version?token=support-api-demo-net-core-token">Click here to check the SupportApi/DsPdf version.</a>
</p>
<p>
    <a href="https://www.grapecity.com/documents-api-pdfviewer/demos/product-bundles/assets/WebApi.zip">Download the SupportApiService project (.zip).</a>
</p>
<p>
    <h3>Viewer configuration example</h3>
    <pre>
        function loadPdfViewer(selector) {
            //DsPdfViewer.LicenseKey = 'your_license_key';
            var viewer = new DsPdfViewer(selector, {
                supportApi: {
                    apiUrl: '<%= HttpContext.Current.Request.Url.Scheme %>://<%= HttpContext.Current.Request.Url.Authority %>/api/pdf-viewer',
                    webSocketUrl: '<%= HttpContext.Current.Request.Url.Scheme %>://<%= HttpContext.Current.Request.Url.Authority %>/signalr',
                    token: 'support-api-demo'
                }
            });
            viewer.addDefaultPanels();
            viewer.addAnnotationEditorPanel();
            viewer.addFormEditorPanel();
        }
    </pre>
</p>
</asp:Content>