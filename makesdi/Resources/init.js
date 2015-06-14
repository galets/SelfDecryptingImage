window.onload = function() {
    setTimeout(function() {
        try {
            var key = window.location.hash.substring(1);

            var dataBytes = base64js.toByteArray(data);
            var keyBytes = base64js.toByteArray(key);
            var ivBytes = base64js.toByteArray(iv);

            var decryptedBytes = slowAES.decrypt(dataBytes, slowAES.modeOfOperation.CBC, keyBytes, ivBytes);
            var decryptesBase64 = base64js.fromByteArray(decryptedBytes);
            
            text.style.display = 'none';
            image.src="data:image/" + imageType + ";base64," + decryptesBase64;
        }
        catch(e) {
            text.innerText = "This image cannot be decrypted. Please make sure you have the correct URL";
        }
    }, 100);
}