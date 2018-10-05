// UploadFile: (1) Drag & drop single or multiple files, (2) Paste from clipboard and name .png file.

function UploadFile(postApiUrl, file, onCompleted, onError, onProgress) {
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
            onError(500, e.message)
        };
    }

    if (onProgress != null) {
        currentRequest.upload.onprogress = function (e) {
            onProgress(e.loaded, e.total);
        };
    }

    currentRequest.open("POST", postApiUrl);
    currentRequest.send(formData);
}

function IsDraggableSupported() {
    var div = document.createElement('div');
    return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div));
}

function IsPasteSupport() {
    return 'onpaste' in document;
}

// InitUploadZone("container1");
function InitUploadZone(containerName, fileContainerWithSeparator, uploadPostUrl) {
    var UploadPostUrl;
    var RootContainer;
    var FormContainer;
    var Form;
    var FileInput;
    var SelectButton;
    var Status;
    var CurrentFiles = [];
    var CurrentFileIndex = -1;

    function RenderFiles() {
        var content = "";
        for (var i = 0; i < CurrentFiles.length; i++) {
            var file = CurrentFiles[i];
            var fileName = file.customName || file.name;
            var fileUrl = fileContainerWithSeparator + fileName;//window.location.href + fileName;

            content += ""
                + "<div data-file-index='" + i + "' class='up-file'>"
                    + "<div class='up-file-name'>"
                        + "<a href='" + fileUrl + "' target='_blank'>" 
                            + fileName
                        + "</a>"
                    + "</div>"
                    + "<div class='up-file-state'>Waiting</div>"
                    + "<div class='clear'></div>"
                    + "<div class='up-file-progress'></div>"
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
                container.classList.add("up-file-current");
                progress = container.querySelector(".up-file-progress");
                state = container.querySelector(".up-file-state");
            }

            UploadFile(
                UploadPostUrl,
                CurrentFiles[CurrentFileIndex],
                function (e) {
                    if (container != null) {
                        container.classList.remove("up-file-current");
                        container.classList.add("up-file-done");
                        state.innerHTML = "Done";
                    }
                    UploadStep();
                },
                function (code, message) {
                    if (container != null) {
                        container.classList.remove("up-file-current");
                        container.classList.add("up-file-error");
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
        }
    }

    UploadPostUrl = uploadPostUrl;
    RootContainer = document.getElementById(containerName);
    FormContainer = RootContainer.querySelector(".up-form");
    Form = FormContainer.querySelector("form");//document.getElementById("form");
    FileInput = Form.querySelector("input[type='file']");//document.getElementById("files");
    FileInput.addEventListener("change", function (e) {
        SelectButton.setAttribute("disabled", "disabled");

        CurrentFiles = FileInput.files;
        CurrentFileIndex = -1;
        RenderFiles();
        UploadStep();
    });

    SelectButton = Form.querySelector("button");//document.getElementById(containerName + "_picker");//"picker"
    SelectButton.addEventListener("click", function (e) {
        FileInput.click();
        e.preventDefault();
    });

    RootContainer.addEventListener('drag', function (e) {
        e.preventDefault();
    });
    RootContainer.addEventListener('dragstart', function (e) {
        e.preventDefault();
    });
    RootContainer.addEventListener('dragend', function (e) {
        e.preventDefault();
    });
    RootContainer.addEventListener('dragover', function (e) {
        e.preventDefault();
    });
    RootContainer.addEventListener('dragenter', function (e) {
        e.preventDefault();
    });
    RootContainer.addEventListener('dragleave', function (e) {
        e.preventDefault();
    });
    RootContainer.addEventListener('drop', function (e) {
        CurrentFiles = e.dataTransfer.files;
        CurrentFileIndex = -1;
        RenderFiles();
        UploadStep();

        e.preventDefault();
    });

    Status = RootContainer.querySelector(".up-status");

    if (IsDraggableSupported()) {
        document.body.classList.add("up-draggable");
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
                    var userName = prompt("Name the file (without extension of .png):", defaultName);
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

        RootContainer.querySelector(".up-clipboard").style.display = 'block';
    }

    RootContainer.style.display = 'block';
}