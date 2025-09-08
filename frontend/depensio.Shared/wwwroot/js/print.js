window.printBlock = window.printBlock || {};
window.printBlock = function (divId) {
    var content = document.getElementById(divId).innerHTML;
    var mywindow = window.open('', '_blank');

    mywindow.document.write(`
        <html>
        <head>
            <title>Impression</title>
            <script src="https://cdn.tailwindcss.com"></script>
            <style>
                /* Format A4 */
                @page {
                    size: A4;
                    margin: 20mm;
                }

                body {
                    font-family: Arial, sans-serif;
                    font-size: 12pt;
                }

                /* Numérotation des pages en bas à droite */
                @page {
                    @bottom-right {
                        content: "Page " counter(page) " / " counter(pages);
                        font-size: 10pt;
                    }
                }

                /* Alternative plus compatible pour numéros de page */
                .page-number:after {
                    content: counter(page);
                }

                /* Évite de couper un bloc entre deux pages */
                .avoid-break {
                    page-break-inside: avoid;
                    break-inside: avoid;
                }

                /* Forcer un saut de page après un élément si nécessaire */
                .page-break {
                    page-break-after: always;
                    break-after: page;
                }
            </style>
        </head>
        <body>
            ${content}

            
        </body>
        </html>
    `);
    mywindow.document.close();
    mywindow.onload = function () {
        mywindow.focus();
        mywindow.print();
        mywindow.close();
    };
    
}
