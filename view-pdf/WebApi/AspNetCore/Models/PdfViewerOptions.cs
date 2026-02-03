using System;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.AcroForms;

namespace GcPdfViewerSupportApiDemo.Models
{
    public class PdfViewerOptions
    {
        [Flags]
        public enum Options
        {
            None = 0,
            AnnotationEditorPanel = 1,
            FormEditorPanel = 2,
            ActivateAnnotationEditor = 4,
            ActivateFormEditor = 8,
            SupportApi = 16,
            ReplyTool = 32,
            ExpandedReplyTool = 64,
            SharedDocumentsPanel = 128,
            AllPanels = AnnotationEditorPanel | FormEditorPanel,
        }

        public Options ViewerOptions { get; set; }
        public string[] ViewerTools { get; set; }
        public string[] AnnotationEditorTools { get; set; }
        public string[] FormEditorTools { get; set; }

        public PdfViewerOptions(Options options = Options.AllPanels, string[] viewerTools = null, string[] annotationEditorTools = null, string[] formEditorTools = null) : base()
        {
            ViewerOptions = options;
            ViewerTools = viewerTools;
            AnnotationEditorTools = annotationEditorTools;
            FormEditorTools = formEditorTools;
        }
    }
}
