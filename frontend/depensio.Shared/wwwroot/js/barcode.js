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


window.togglePanel = function () {
    const togglepanel = document.querySelectorAll('.dep-toggle-panel');

    if (togglepanel) {
        // Ajout des événements pour les en-têtes de produit
        togglepanel.forEach(header => {
            header.addEventListener('click', (e) => {
                console.log("Toggle panel clicked");
                const product = header.getAttribute('data-product');
                const content = document.querySelector(`.product-content[data-product-item="${product}"]`);
                const icon = header.querySelector('#svg-icon');

                content.classList.toggle('hidden');
                icon.classList.toggle('rotate-180');
            });
        });

    }
}

