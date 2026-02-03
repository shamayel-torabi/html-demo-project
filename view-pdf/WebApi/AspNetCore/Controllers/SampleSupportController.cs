// Ignore Spelling: Pdf Api Gc

using System;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Controllers;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Utils;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Models;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Resources.Stamps;
using System.Text;
using GrapeCity.Documents.Pdf.Annotations;
using System.Drawing;
using System.Linq;

namespace GcPdfViewerSupportApiDemo.Controllers
{
    [Route("api/pdf-viewer")]
    [ApiController]
    public class SampleSupportController : GcPdfViewerController
    {
        static SampleSupportController()
        {
#if DEBUG
            // VS's F5 sets the current working directory to project dir rather than bin dir,
            // we set it to bin dir so that we can fetch files from the Resources/ sub-dir:
            var exePath = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
            var directory = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(directory);
#endif
            Settings.AvailableUsers.AddRange(new string[] { "James", "Susan Smith" });
        }

        /// <summary>
        /// As an example, override one of the base Support API methods.
        /// </summary>
        /// <returns></returns>
        public override string Ping(string docId)
        {
            return base.Ping(docId);
        }


        public override IStampImagesStorage StampImagesStorage
        {
            get
            {
                var token = GetQueryValue("token");
                if (!string.IsNullOrEmpty(token) && token.Contains("custom-stamps-sample"))
                {
                    string projectRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string imagesPath = Path.Combine(projectRootPath, "wwwroot/assets/example-stamps");
                    return new FileSystemStampImagesStorage(imagesPath);
                }
                return base.StampImagesStorage;
            }
        }

        public override void OnDocumentModified(GcPdfDocumentLoader documentLoader)
        {
            base.OnDocumentModified(documentLoader);
            CleanupSampleCloudStorage();

            if (documentLoader.Info.documentOptions.userData is JObject userData &&
                userData.ToObject<Dictionary<string, string>>() is { } userDataObj &&
                userDataObj.TryGetValue("sampleName", out string sampleName) &&
                userDataObj.TryGetValue("docName", out string docName))
            {
                switch (sampleName)
                {
                    case "SaveChangesSample":
                        HandleSaveChangesSample(documentLoader, docName);
                        break;

                        // Future cases for other samples can be added here.
                }
            }
        }

        private void HandleSaveChangesSample(GcPdfDocumentLoader documentLoader, string docName)
        {
            var document = documentLoader.Document;
            ApplyFinalChanges(document);
            FlattenAnnotations(document);
            SaveDocumentToCloud(documentLoader.ClientId, document, docName);
        }

        [Route("SubmitFormSample")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SubmitFormSample()
        {
            if (!Request.HasFormContentType)
            {
                return Content("No submitted data");
            }
            else
            {
                var form = Request.Form;
                var e = form.Keys.GetEnumerator();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Submitted data:");
                while (e.MoveNext())
                {
                    if (form.TryGetValue(e.Current, out var val))
                    {
                        sb.AppendLine($" {e.Current}: {val};");
                    }
                }
                return Content(sb.ToString());
            }
        }
    

        #region ** ViewerSaveChanges sample:

        /// <summary>
        /// This method is called from the client side.
        /// </summary>
        /// <param name="docName"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Route("GetPdfFromCloud")]
        [ApiExplorerSettings(IgnoreApi = true)]
        async public Task<IActionResult> GetPdfFromCloud(string docName, string clientId)
        {
            var fileBytes = await GetDocumentFromCloud(docName, clientId);
            if (fileBytes == null)
                throw new Exception($"Sample document '{docName}' not found.");
            return new FileContentResult(fileBytes, "application/pdf")
            {
                FileDownloadName = docName
            };
        }

        /// <summary>
        /// Save the PDF document to a Cloud Service.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="document"></param>
        /// <param name="docName"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        public void SaveDocumentToCloud(string clientId, GcPdfDocument document, string docName)
        {
            // ************************************************************************************
            // Write your code to save a PDF document to a specific Cloud Service here.
            // ************************************************************************************

            // Here's the sample stub code:
            SaveToMemory(clientId, document, docName);
            //SaveToDisk(document, docName);

        }


        /// <summary>
        /// Download a PDF document from a Cloud Service.
        /// </summary>
        /// <param name="docName"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        async public static Task<byte[]> GetDocumentFromCloud(string docName, string clientId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // ************************************************************************************
            // Write your code to get a PDF document from a specific Cloud Service here.
            // ************************************************************************************

            // Here's the sample stub code:
            return LoadFromMemory(clientId, docName);
            //return LoadFromDisk(docName);
        }

        #region ** PDF Processing Helpers

        /// <summary>
        /// Applies final modifications before saving the document.
        /// </summary>
        private static void ApplyFinalChanges(GcPdfDocument document)
        {
            document.Metadata.ModifyDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Flattens annotations, converting them into static PDF content.
        /// </summary>
        private static void FlattenAnnotations(GcPdfDocument document)
        {
            var annotations = document.Pages.SelectMany(page => page.Annotations).ToList();
            ConvertAnnotationsToContent(annotations);
        }

        /// <summary>
        /// Converts annotations into page content and removes them.
        /// </summary>
        private static void ConvertAnnotationsToContent(List<AnnotationBase> annotations)
        {
            foreach (var group in annotations.GroupBy(a => a.Page))
            {
                var page = group.Key;
                if (page == null) continue;

                var graphics = page.Graphics;
                var size = page.GetRenderSize(graphics.Resolution, graphics.Resolution);
                var destinationRectangle = new RectangleF(0, 0, size.Width, size.Height);

                page.DrawAnnotations(graphics, destinationRectangle, group.ToList());
                foreach (var annotation in group)
                {
                    page.Annotations.Remove(annotation);
                }
            }
        }

        #endregion

        #region ** disk storage example

        private static byte[] LoadFromDisk(string docName)
        {
            string path = $"Documents/{docName}";
            return System.IO.File.ReadAllBytes(path);
        }

        private void SaveToDisk(GcPdfDocument document, string docName)
        {
            if (!Directory.Exists("Documents"))
                Directory.CreateDirectory("Documents");
            string path = $"Documents/{docName}";
            document.Save(path);
        }

        #endregion

        #region ** in-memory storage example

        public static Dictionary<string, KeyValuePair<DateTime, byte[]>> DocumentsInCloud { get; private set; } = new Dictionary<string, KeyValuePair<DateTime, byte[]>>();

        private static byte[] LoadFromMemory(string clientId, string docName)
        {
            var key = $"{docName}_{clientId}";
            byte[] bytes = null;
            lock (DocumentsInCloud)
            {
                bytes = DocumentsInCloud.ContainsKey(key) ? DocumentsInCloud[key].Value : null;
            }
            CleanupSampleCloudStorage();
            return bytes;
        }

        private void SaveToMemory(string clientId, GcPdfDocument document, string docName)
        {
            var key = $"{docName}_{clientId}";
            MemoryStream ms = new MemoryStream();
            document.Save(ms);
            lock (DocumentsInCloud)
            {
                if (DocumentsInCloud.ContainsKey(key))
                    DocumentsInCloud.Remove(key);
                DocumentsInCloud.Add(key, new KeyValuePair<DateTime, byte[]>(DateTime.Now, ms.ToArray()));
            }
            ms.Dispose();
        }

        static void CleanupSampleCloudStorage()
        {
            lock (DocumentsInCloud)
            {
                foreach (var k in DocumentsInCloud.Keys)
                {
                    if ((DateTime.Now - DocumentsInCloud[k].Key) > new TimeSpan(0, 10, 0) /* 10 min */)
                    {
                        DocumentsInCloud.Remove(k);
                    }
                }
            }
        }

        #endregion


        #endregion
    }
}
