window.qrScanner = window.qrScanner || {};

window.qrScanner = {
    start: async function (dotnetHelper) {
        const codeReader = new ZXing.BrowserQRCodeReader();
        const videoElement = document.getElementById('qr-video');

        try {
            const result = await codeReader.decodeOnceFromVideoDevice(undefined, videoElement);
            dotnetHelper.invokeMethodAsync("OnQrCodeScanned", result.text);
        } catch (err) {
            console.error(err);
            dotnetHelper.invokeMethodAsync("OnQrCodeScanned", "");
        }
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
