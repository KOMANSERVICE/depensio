window.qrScanner = window.qrScanner || {};

window.qrScanner = {
    start: async function (dotnetHelper) {
        const codeReader = new ZXing.BrowserMultiFormatReader();

        const videoInputDevices = await codeReader.listVideoInputDevices();
        if (videoInputDevices.length === 0) {
            alert("Aucune caméra trouvée !");
            return;
        }

        const selectedDeviceId = videoInputDevices[0].deviceId;
        const previewElem = document.getElementById("qr-video");

        codeReader.decodeFromVideoDevice(
            selectedDeviceId,
            previewElem,
            (result, err) => {
                if (result) {
                    // ? Récupération du code scanné (ex : EAN-13 produit)
                    dotnetHelper.invokeMethodAsync("OnQrCodeScanned", result.text);
                    codeReader.reset();
                }
            }
        );

        //const codeReader = new ZXing.BrowserQRCodeReader();
        //const videoElement = document.getElementById('qr-video');
        //console.log("ah ah ah ");
        //try {
        //    console.log("2 ah ah ah ");
        //    const result = await codeReader.decodeOnceFromVideoDevice(undefined, videoElement);

        //    console.log("3 ah ah ah ");
        //    dotnetHelper.invokeMethodAsync("OnQrCodeScanned", result.text);

        //    console.log("3ah ah ah ");
        //} catch (err) {
        //    console.error("test des test");
        //    console.error(err);
        //    dotnetHelper.invokeMethodAsync("OnQrCodeScanned", "");
        //}
    },
    stop: function () {
        const videoElement = document.getElementById('qr-video');
        if (videoElement && videoElement.srcObject) {
            let tracks = videoElement.srcObject.getTracks();
            tracks.forEach(track => track.stop());
            videoElement.srcObject = null;
        }
    }
};
