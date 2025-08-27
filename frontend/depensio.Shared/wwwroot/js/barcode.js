window.renderBarcode = window.renderBarcode || {};

window.renderBarcode = function (ean13, svgId) {
    const svgElem = document.getElementById(svgId);
    if (!svgElem) {
        console.warn("SVG non trouvé :", svgId);
        return;
    }

    JsBarcode(svgElem, ean13, {
        format: "ean13",
        displayValue: true,   // affiche les chiffres
        fontSize: 12,
        textMargin: 2,
        width: 2,
        height: 80
    });
}


window.printBarcodes = function () {
    window.print();
}
