let localizationStrings = { };
export function initializeLocalizationData(strings) {
    // Store localized strings.
    localizationStrings = strings;

};

export function openPDFDocument(url) {
    try {
        // Validate the URL.
        if (!url || typeof url !== "string") {
            return;
        }

        // Open a new tab.
        const newTab = window.open();
        // We could move the alert to the blazor component and display a better looking toast as well.
        if (!newTab || newTab.closed || typeof newTab.closed === "undefined") {
            alert(localizationStrings.enablePopupMessage);
            return;
        }

            // Sanitize and set up the iframe.
            // Ensure the URL is safe and properly encoded.
            const sanitizedUrl = encodeURI(url);
            newTab.document.title = "PDFDocView";
            newTab.document.write(`
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Document Viewer</title>
                <style>
                    body, html {
                        margin: 0;
                        padding: 0;
                        height: 100%;
                        overflow: hidden;
                    }
                    iframe {
                        width: 100%;
                        height: 100%;
                        border: none;
                    }
                </style>
            </head>
            <body>
                <iframe src="${sanitizedUrl}" title="Document Viewer"></iframe>
            </body>
            </html>
        `);
        
    } catch (error) {
        console.error("An error occurred while opening the URL in a new tab.");
    }
}
