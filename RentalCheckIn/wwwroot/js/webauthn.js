(() => {
    // Namespace to avoid polluting the global scope
    window.webauthnInterop = {
        /**
         * Registers a new credential with the provided PublicKeyCredentialCreationOptions.
         * @param {Object} options - The PublicKeyCredentialCreationOptions received from the server.
         * @returns {Promise<Object>} - A promise that resolves to the serialized credential object.
         */
        registerCredential: async function (options) {
            if (!window.PublicKeyCredential) {
                throw new Error("Web Authentication API is not supported in this browser.");
            }

            try {
                const credential = await navigator.credentials.create({ publicKey: options });
                return credentialToJson(credential);
            } catch (err) {
                console.error("Error during credential registration:", err);
                throw new Error("Registration failed. Please try again.");
            }
        },

        /**
         * Authenticates a user using an existing credential with the provided PublicKeyCredentialRequestOptions.
         * @param {Object} options - The PublicKeyCredentialRequestOptions received from the server.
         * @returns {Promise<Object>} - A promise that resolves to the serialized assertion object.
         */
        authenticateCredential: async function (options) {
            if (!window.PublicKeyCredential) {
                throw new Error("Web Authentication API is not supported in this browser.");
            }

            try {
                const assertion = await navigator.credentials.get({ publicKey: options });
                return assertionToJson(assertion);
            } catch (err) {
                console.error("Error during credential authentication:", err);
                throw new Error("Authentication failed. Please try again.");
            }
        }
    };

    /**
     * Converts a PublicKeyCredential object to a JSON-friendly format.
     * @param {PublicKeyCredential} credential - The credential object to serialize.
     * @returns {Object} - The serialized credential object.
     */
    function credentialToJson(credential) {
        return {
            id: credential.id,
            rawId: arrayBufferToBase64Url(credential.rawId),
            type: credential.type,
            response: {
                clientDataJSON: arrayBufferToBase64Url(credential.response.clientDataJSON),
                attestationObject: arrayBufferToBase64Url(credential.response.attestationObject)
            },
            // Optionally include extensions if needed
            // extensions: credential.getClientExtensionResults()
        };
    }

    /**
     * Converts a PublicKeyCredentialAssertion object to a JSON-friendly format.
     * @param {PublicKeyCredential} assertion - The assertion object to serialize.
     * @returns {Object} - The serialized assertion object.
     */
    function assertionToJson(assertion) {
        return {
            id: assertion.id,
            rawId: arrayBufferToBase64Url(assertion.rawId),
            type: assertion.type,
            response: {
                clientDataJSON: arrayBufferToBase64Url(assertion.response.clientDataJSON),
                authenticatorData: arrayBufferToBase64Url(assertion.response.authenticatorData),
                signature: arrayBufferToBase64Url(assertion.response.signature),
                userHandle: assertion.response.userHandle ? arrayBufferToBase64Url(assertion.response.userHandle) : null
            },
            // Optionally include extensions if needed
            // extensions: assertion.getClientExtensionResults()
        };
    }

    /**
     * Converts an ArrayBuffer to a Base64URL-encoded string.
     * @param {ArrayBuffer} buffer - The ArrayBuffer to convert.
     * @returns {string} - The Base64URL-encoded string.
     */
    function arrayBufferToBase64Url(buffer) {
        const bytes = new Uint8Array(buffer);
        let binary = '';
        bytes.forEach((b) => binary += String.fromCharCode(b));
        const base64 = window.btoa(binary);
        return base64ToBase64Url(base64);
    }

    /**
     * Converts a standard Base64 string to a Base64URL-encoded string.
     * @param {string} base64 - The standard Base64 string.
     * @returns {string} - The Base64URL-encoded string.
     */
    function base64ToBase64Url(base64) {
        return base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    }

    /**
     * Converts a Base64URL-encoded string back to an ArrayBuffer.
     * Useful if you need to deserialize data on the client-side.
     * @param {string} base64url - The Base64URL-encoded string.
     * @returns {ArrayBuffer} - The resulting ArrayBuffer.
     */
    function base64UrlToArrayBuffer(base64url) {
        const base64 = base64url.replace(/-/g, '+').replace(/_/g, '/');
        // Pad with '=' to make the length a multiple of 4
        const padded = base64.padEnd(base64.length + (4 - base64.length % 4) % 4, '=');
        const binary = window.atob(padded);
        const bytes = new Uint8Array(binary.length);
        for (let i = 0; i < binary.length; i++) {
            bytes[i] = binary.charCodeAt(i);
        }
        return bytes.buffer;
    }

})();
