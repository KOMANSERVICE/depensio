window.printBlock = window.printBlock || {};
window.printBlock = function (divId, options = {}) {

    const content = document.getElementById(divId).innerHTML;
    const newTab = window.open('', '_blank');

    // Valeurs par défaut
    const defaultOptions = {
        title: "Impression",
        pageSize: null, // "A4" | "Letter" | "80mm 40mm"
        margin: "15mm",
        extraCss: "",   // CSS additionnel
        tailwind: true, // Charger Tailwind ou pas
        autoClose: true // Fermer l'onglet après impression
    };

    const settings = { ...defaultOptions, ...options };

    // Génération du CSS @page
    let pageCss = "";
    if (settings.pageSize) {
        pageCss = `@page { size: ${settings.pageSize}; margin: ${settings.margin}; }`;
    }

    newTab.document.write(`
        <html>
        <head>
            <title>${settings.title}</title>
            ${settings.tailwind
            ? "<link href='https://cdn.tailwindcss.com' rel='stylesheet'>"
            : ""}
            <style>
                ${pageCss}
                .avoid-break { page-break-inside: avoid; break-inside: avoid; }
                ${settings.extraCss}
            </style>
        </head>
        <body>
            ${content}
        </body>
        </html>
    `);

    newTab.document.close();

    newTab.onload = function () {
        newTab.focus();
        newTab.print();
        if (settings.autoClose) {
            newTab.close();
        }
    };    
}
