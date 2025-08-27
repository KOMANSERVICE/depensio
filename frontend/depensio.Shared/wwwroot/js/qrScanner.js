window.qrScanner = window.qrScanner || {};
let scanning = false;
// Charger le son
const beepSound = new Audio('_content/depensio.Shared/sounds/beep-08b.mp3'); // ? met ton chemin vers le fichier son

window.qrScanner = {
    start: async function (dotnetHelper) {
        if (scanning) {
            console.log("Déjà en train de scanner...");
            return;
        }
        scanning = true;
        const codeReader = new ZXing.BrowserMultiFormatReader();

        const videoInputDevices = await codeReader.listVideoInputDevices();
        if (videoInputDevices.length === 0) {
            alert("Aucune caméra trouvée !");
            return;
        }

        //const selectedDeviceId = videoInputDevices[0].deviceId;

        // ?? Choisir une caméra arrière si dispo
        let backCamera = videoInputDevices.find(device =>
            device.label.toLowerCase().includes('back')
        );

        if (!backCamera && videoInputDevices.length > 0) {
            // fallback ? prend la première caméra
            backCamera = videoInputDevices[0];
        }

        const previewElem = document.getElementById("qr-video");

        codeReader.decodeFromVideoDevice(
            backCamera.deviceId,
            previewElem,
            (result, err) => {
                if (result) {
                    beepSound.play().catch(e => console.warn("Erreur lecture son :", e));
                    // ? Récupération du code scanné (ex : EAN-13 produit)
                    scanning = false;
                    dotnetHelper.invokeMethodAsync("OnQrCodeScanned", result.text);
                    //codeReader.reset();
                }
            }
        );

      
    },
    stop: function () {
        const videoElement = document.getElementById('qr-video');
        if (videoElement && videoElement.srcObject) {
            scanning = false;
            let tracks = videoElement.srcObject.getTracks();
            tracks.forEach(track => track.stop());
            videoElement.srcObject = null;
        }
    }
};
