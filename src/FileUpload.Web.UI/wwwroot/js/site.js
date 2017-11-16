// Write your JavaScript code.

function UploadFile(file, onCompleted, onError, onProgress) {
    var formData = new FormData();
    formData.append("file", file, file.name);

    var currentRequest = new XMLHttpRequest();
    currentRequest.onreadystatechange = function (e) {
        var request = e.target;

        if (request.readyState == XMLHttpRequest.DONE) {
            if (request.status == 200) {
                var responseText = currentRequest.responseText;
                onCompleted(responseText);
            }
            else if (onError != null) {
                onError(currentRequest.status, currentRequest.statusText);
            }
        }
    };

    if (onError != null) {
        currentRequest.onerror = function (e) {
            onError(500, e.message)
        };
    }

    if (onProgress != null) {
        currentRequest.upload.onprogress = function (e) {
            onProgress(e.loaded, e.total);
        };
    }

    currentRequest.open("POST", Url);
    currentRequest.send(formData);
}

function IsDraggableSupported() {
    var div = document.createElement('div');
    return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div));
}