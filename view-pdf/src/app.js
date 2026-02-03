
window.onload = function(){
    //DsPdfViewer.LicenseKey = "***key***";
    let viewer = new DsPdfViewer("#viewer", {});
    viewer.addDefaultPanels();
    viewer.open("assets/pdf/newsletter.pdf");
}
