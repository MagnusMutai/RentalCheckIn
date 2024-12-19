window.fido2 = {
    async register(options) {
        try {
            // Decode challenge using Base64URL
            options.challenge = base64urlToUint8Array(options.challenge);

            // Decode user.id using Base64URL
            options.user.id = base64urlToUint8Array(options.user.id);

            // If there are excludeCredentials, decode their IDs as well
            if (options.excludeCredentials) {
                options.excludeCredentials = options.excludeCredentials.map(cred => ({
                    ...cred,
                    id: base64urlToUint8Array(cred.id)
                }));
            }

            // Call the WebAuthn API
            const credential = await navigator.credentials.create({ publicKey: options });

            // Convert binary fields to Base64URL strings for server compatibility
            return {
                id: credential.id,
                rawId: arrayBufferToBase64url(credential.rawId),
                type: credential.type,
                response: {
                    attestationObject: arrayBufferToBase64url(credential.response.attestationObject),
                    clientDataJSON: arrayBufferToBase64url(credential.response.clientDataJSON)
                }
            };
        } catch (error) {
            console.error("WebAuthn registration failed:", error);
            // Don't throw in production
            // throw error;
        }
    },
    async authenticate(options) {
        try {
            // Decode challenge using Base64URL
            options.challenge = base64urlToUint8Array(options.challenge);

            // Decode allowCredentials IDs using Base64URL
            if (options.allowCredentials) {
                options.allowCredentials = options.allowCredentials.map(cred => ({
                    ...cred,
                    id: base64urlToUint8Array(cred.id)
                }));
            }

            // Call the WebAuthn API
            const assertion = await navigator.credentials.get({ publicKey: options });

            // Convert binary fields to Base64URL strings for server compatibility
            return {
                id: assertion.id,
                rawId: arrayBufferToBase64url(assertion.rawId),
                response: {
                    authenticatorData: arrayBufferToBase64url(assertion.response.authenticatorData),
                    clientDataJSON: arrayBufferToBase64url(assertion.response.clientDataJSON),
                    signature: arrayBufferToBase64url(assertion.response.signature),
                    userHandle: assertion.response.userHandle
                        ? arrayBufferToBase64url(assertion.response.userHandle)
                        : null
                },
                type: assertion.type
            };
        } catch (error) {
            console.error("WebAuthn authentication failed:", error);
            // Don't throw in production
            // throw error;
        }
    }
};

function base64urlToUint8Array(baseurl) {
    // Replace URL-specific characters
    var padding = '='.repeat((4 - (baseurl.length % 4)) % 4);
    var base64 = baseurl.replace(/-/g, '+').replace(/_/g, '/') + padding;
    var binary = atob(base64);
    var bytes = new Uint8Array(binary.length);
    for (var i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes;
}

// Helper function to convert ArrayBuffer to Base64URL
function arrayBufferToBase64url(buffer) {
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    var base64 = btoa(binary);
    return base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
}
