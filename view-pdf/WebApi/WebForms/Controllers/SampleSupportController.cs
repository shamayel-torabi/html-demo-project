using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Controllers;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Utils;
using GrapeCity.Documents.Pdf.ViewerSupportApi.Resources.Stamps;
using Newtonsoft.Json.Linq;
using GrapeCity.Documents.Pdf;

namespace GcPdfViewerSupportApiDemo.Controllers
{

    public class SampleSupportController : GcPdfViewerController
    {

        static SampleSupportController()
        {
#if DEBUG
            // VS's F5 sets the current working directory to project dir rather than bin dir,
            // we set it to bin dir so that we can fetch files from the Resources/ sub-dir:
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                var exePath = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
                var directory = Path.GetDirectoryName(exePath);
                Directory.SetCurrentDirectory(directory);
            }
#endif
            Settings.AvailableUsers.AddRange(new string[] { "James", "Susan Smith" });
        }


        public override IStampImagesStorage StampImagesStorage
        {
            get
            {
                if (GetQueryValue("token") == "support-api-demo-net-core-token-2021-custom-stamps-sample")
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
            var userData = documentLoader.Info.documentOptions.userData as JObject;
            if (userData != null)
            {
                var userDataObj = userData.ToObject<Dictionary<string, string>>();
                if (userDataObj != null)
                {
                    if (userDataObj.ContainsKey("sampleName") && userDataObj["sampleName"] == "SaveChangesSample")
                    {
                        string docName = userDataObj["docName"];
                        SaveDocumentToCloud(documentLoader.ClientId, documentLoader.Document, docName);
                    }

                }
            }
        }


        #region ** ViewerSaveChanges sample:

        public static Dictionary<string, KeyValuePair<DateTime, byte[]>> DocumentsInCloud { get; private set; } = new Dictionary<string, KeyValuePair<DateTime, byte[]>>();

        [Route("GetPdfFromCloud")]
        [HttpGet()]
        public object GetPdfFromCloud(string docName, string clientId)
        {
            var fileBytes = GetDocumentFromCloud(docName, clientId);
            if (fileBytes == null)
                throw new Exception($"Sample document '{docName}' not found.");
            return _PrepareFileAttachmentAnswer(new MemoryStream(), docName);
        }

        public void SaveDocumentToCloud(string clientId, GcPdfDocument document, string docName)
        {
            var key = $"{docName}_{clientId}";
            MemoryStream ms = new MemoryStream();
            document.Save(ms);
            lock (DocumentsInCloud)
            {
                if (DocumentsInCloud.ContainsKey(key))
                {
                    DocumentsInCloud.Remove(key);
                }
                DocumentsInCloud.Add(key, new KeyValuePair<DateTime, byte[]>(DateTime.Now, ms.ToArray()));
            }
            ms.Dispose();
        }

        public static byte[] GetDocumentFromCloud(string docName, string clientId)
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


    }
}
