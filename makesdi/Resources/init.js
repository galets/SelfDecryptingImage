function show(control) {
    control.style.display = 'block';
}

function hide(control) {
    control.style.display = 'none';
}

function setUrlText(keyText) {
    var href = window.location.href + "#" + keyText;
    url1.href = href;
    url1.innerText = href;
}

function decrypt(keyText) {
    try {
        show(text);

        var dataBytes = base64js.toByteArray(data);
        var keyBytes = base64js.toByteArray(keyText);
        var ivBytes = base64js.toByteArray(iv);

        var decryptedBytes = slowAES.decrypt(dataBytes, slowAES.modeOfOperation.CBC, keyBytes, ivBytes);
        var decryptesBase64 = base64js.fromByteArray(decryptedBytes);
        
        hide(text);
        hide(keyprompt);
        image.src="data:image/" + imageType + ";base64," + decryptesBase64;
        show(image);
    }
    catch(e) {
        text.innerText = "This image cannot be decrypted. Please make sure you have the correct URL";
    }
}

if (!window.location.hash || window.location.hash == "" || window.location.hash == "#") {
    show(keyprompt);
} else {
    var key = window.location.hash.substring(1);
    show(text);
    window.onload = function() {
        setTimeout(function() {
            decrypt(key);
        }, 100);
    }
}