// Write your JavaScript code.

function UploadFile(file, url, onCompleted, onError, onProgress) {
    var formData = new FormData();
    formData.append("file", file, file.customName || file.name);

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
            onError(500, e.message);
        };
    }

    if (onProgress != null) {
        currentRequest.upload.onprogress = function (e) {
            onProgress(e.loaded, e.total);
        };
    }

    currentRequest.open("POST", url);
    currentRequest.send(formData);
}

function IsDraggableSupported() {
    var div = document.createElement('div');
    return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div));
}

function IsPasteSupport() {
    return 'onpaste' in document;
}


function Initialize() {

    var Form;
    var FileInput;
    var SelectButton;
    var Status;
    var CurrentFiles = [];
    var CurrentFileIndex = -1;

    var Browser;
    var BrowserContent;

    function ReloadBrowser() {
        if (Browser !== null) {
            var url = Browser.dataset['reloadUrl'];
            fetch(url)
                .then(function (response) {
                    response.text()
                        .then(function (body) {
                            var temp = document.createElement("div");
                            temp.innerHTML = body;

                            var newBrowserContent = temp.querySelector(".browser table tbody");
                            BrowserContent.innerHTML = newBrowserContent.innerHTML;
                        });
                });
        }
    }

    function RenderFiles() {
        var content = "";
        for (var i = 0; i < CurrentFiles.length; i++) {
            var file = CurrentFiles[i];
            var fileName = file.customName || file.name;
            var currentLocation = Form.dataset['downloadUrl'];

            var fileNameHtml = "";
            if (currentLocation != '') {
                var fileUrl = currentLocation + (currentLocation[currentLocation.length - 1] == '/' ? '' : '/') + fileName;
                fileNameHtml = "<a href='" + fileUrl + "' target='_blank'>" + fileName + "</a>";
            } else {
                fileNameHtml = fileName;
            }

            content += ""
                + "<div data-file-index='" + i + "' class='file'>"
                    + "<div class='file-name'>"
                        + fileNameHtml
                    + "</div>"
                    + "<div class='file-state'>Waiting</div>"
                    + "<div class='clear'></div>"
                    + "<div class='file-progress' style='width: 0%;'></div>"
                + "</div>";
        }

        Status.innerHTML = content;
    }

    function UploadStep() {
        CurrentFileIndex++;
        if (CurrentFiles.length > CurrentFileIndex) {
            var progress = null;
            var state = null;
            var container = Status.querySelector("[data-file-index='" + CurrentFileIndex + "']");
            if (container != null) {
                container.classList.add("file-current");
                progress = container.querySelector(".file-progress");
                state = container.querySelector(".file-state");
            }

            UploadFile(
                CurrentFiles[CurrentFileIndex],
                Form.action,
                function (e) {
                    if (container != null) {
                        container.classList.remove("file-current");
                        container.classList.add("file-done");
                        state.innerHTML = "Done";
                    }
                    UploadStep();
                },
                function (code, message) {
                    if (container != null) {
                        container.classList.remove("file-current");
                        container.classList.add("file-error");
                        state.innerHTML = "Failed";
                    }
                    UploadStep();
                },
                function (loaded, total) {
                    var percent = 100 / total * loaded;
                    progress.style.width = percent + "%";
                    state.innerHTML = Math.floor(percent) + "%";
                }
            );
        }
        else {
            SelectButton.removeAttribute("disabled");
            Form.reset();
            ReloadBrowser();
        }
    }


    Container = document.getElementById("upload-container");
    Form = document.getElementById("form");
    FileInput = document.getElementById("files");
    FileInput.addEventListener("change", function (e) {
        SelectButton.setAttribute("disabled", "disabled");

        CurrentFiles = FileInput.files;
        CurrentFileIndex = -1;
        RenderFiles();
        UploadStep();
    });

    SelectButton = document.getElementById("picker");
    SelectButton.addEventListener("click", function (e) {
        FileInput.click();
        e.preventDefault();
    });

    Container.addEventListener('drag', function (e) {
        e.preventDefault();
    });
    Container.addEventListener('dragstart', function (e) {
        e.preventDefault();
    });
    Container.addEventListener('dragend', function (e) {
        e.preventDefault();
    });
    Container.addEventListener('dragover', function (e) {
        e.preventDefault();
    });
    Container.addEventListener('dragenter', function (e) {
        e.preventDefault();
    });
    Container.addEventListener('dragleave', function (e) {
        e.preventDefault();
    });
    Container.addEventListener('drop', function (e) {
        CurrentFiles = e.dataTransfer.files;
        CurrentFileIndex = -1;
        RenderFiles();
        UploadStep();

        e.preventDefault();
    });

    Status = document.querySelector(".status");

    if (IsDraggableSupported()) {
        document.body.classList.add("dragdrop-supported");
    }

    if (IsPasteSupport()) {
        document.addEventListener("paste", function (e) {
            CurrentFiles = e.clipboardData.files;
            CurrentFileIndex = -1;

            for (var i = 0; i < CurrentFiles.length; i++) {
                var file = CurrentFiles[i];
                var name = file.name.split(".");
                if (name[0] == 'image') {
                    var defaultName = 'file_' + (+new Date);
                    var userName = prompt("Name the file (without extension):", defaultName);
                    if (userName == null || userName.trim() == '') {
                        userName = defaultName;
                    }

                    name[0] = userName;
                    file.customName = name.join(".");
                }
            }

            RenderFiles();
            UploadStep();

            e.preventDefault();
        });

        document.body.classList.add('clipboard-supported');
    }

    Container.style.display = 'block';


    Browser = document.querySelector(".browser");
    if (Browser !== null) {
        BrowserContent = Browser.querySelector("table tbody");

        var reloadButton = Browser.querySelector(".reload-button");
        if (reloadButton !== null) {
            reloadButton.addEventListener('click', ReloadBrowser, false);
        }
    }
}