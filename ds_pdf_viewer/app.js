window.onload = function(){
    //DsPdfViewer.LicenseKey = "***key***";
    let viewer = new DsPdfViewer("#viewer", {});
    viewer.addDefaultPanels();
    viewer.open("/document-solutions/javascript-pdf-viewer/demos/product-bundles/assets/pdf/newsletter.pdf");
}