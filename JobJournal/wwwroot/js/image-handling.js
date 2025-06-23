document.addEventListener('DOMContentLoaded', function () {

    const pasteArea = document.querySelector('.image-paste-area');
    const imagePreview = document.getElementById('imagePreview');
    const pasteAreaText = document.getElementById('pasteAreaText');
    const clearImageButton = document.getElementById('clearImageButton');
    const imageError = document.getElementById('imageError'); 



    const imageDataHidden = document.getElementById('imageDataHidden');
    const imageFileNameHidden = document.getElementById('imageFileNameHidden');
    const imageContentTypeHidden = document.getElementById('imageContentTypeHidden');



    const imageUrlInput = document.getElementById('imageUrlInput');
    const uploadImageButton = document.getElementById('uploadImageButton');
    const imageUploadInput = document.getElementById('imageUploadInput'); 
    const pasteImageButton = document.getElementById('pasteImageButton');


    function showMessage(message, isError = false) {
        imageError.textContent = message;
        imageError.style.display = isError ? 'block' : 'none';
        if (!isError) {
            console.log(message); 
        }
    }


    function displayImage(src, fileName, contentType) {
        imagePreview.src = src;
        imagePreview.style.display = 'block';
        pasteAreaText.style.display = 'none';
        clearImageButton.style.display = 'inline-block';

        imageDataHidden.value = src;
        imageFileNameHidden.value = fileName;
        imageContentTypeHidden.value = contentType;
        showMessage(''); 

        imageUrlInput.value = '';
        imageUploadInput.value = '';
    }

    function clearImage() {
        imagePreview.src = '';
        imagePreview.style.display = 'none';
        pasteAreaText.style.display = 'block';
        clearImageButton.style.display = 'none';

        imageDataHidden.value = '';
        imageFileNameHidden.value = '';
        imageContentTypeHidden.value = '';

        imageUrlInput.value = '';
        imageUploadInput.value = ''; 
        showMessage(''); 
    }


    pasteArea.addEventListener('paste', function (e) {
        e.preventDefault();
        const items = (e.clipboardData || e.originalEvent.clipboardData).items;
        let imageFound = false;

        for (let i = 0; i < items.length; i++) {
            if (items[i].type.indexOf('image') !== -1) {
                const blob = items[i].getAsFile();
                if (blob) {
                    const reader = new FileReader();
                    reader.onload = function (event) {
                        displayImage(event.target.result, blob.name || 'pasted_image.png', blob.type || 'image/png');
                    };
                    reader.readAsDataURL(blob);
                    imageFound = true;
                    break;
                }
            }
        }
        if (!imageFound) {
            showMessage('No image data found in clipboard. Please copy an actual image (e.g., from a screenshot tool) and try pasting with Ctrl+V.', true);
        }
    });


    pasteArea.addEventListener('click', function () {
        imageUploadInput.click(); 
    });



    uploadImageButton.addEventListener('click', () => {
        imageUploadInput.click();
    });


    imageUploadInput.addEventListener('change', function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                displayImage(e.target.result, file.name, file.type);
            };
            reader.readAsDataURL(file);
        } else {
            clearImage();
        }
    });


    pasteImageButton.addEventListener('click', async () => {
        try {
            const clipboardItems = await navigator.clipboard.read();
            let imageFoundInClick = false;

            for (const clipboardItem of clipboardItems) {
                for (const type of clipboardItem.types) {
                    if (type.startsWith('image/')) {
                        const blob = await clipboardItem.getType(type);
                        if (blob) { 
                            const reader = new FileReader();
                            reader.onload = function (e) {
                                displayImage(e.target.result, blob.name || 'pasted_clipboard_image.png', blob.type || 'image/png');
                            };
                            reader.readAsDataURL(blob);
                            imageFoundInClick = true;
                            break;
                        }
                    }
                }
            }

            if (!imageFoundInClick) {
                showMessage('No image data found in clipboard. Please copy an image and try again. (May require browser permission)', true);
            }
        } catch (err) {
            console.error('Failed to read clipboard contents:', err);
            if (err.name === 'NotAllowedError' || err.name === 'SecurityError') {
                showMessage('Permission to access clipboard denied or blocked by browser. Please ensure you allow clipboard access.', true);
            } else if (err.name === 'AbortError') {
                showMessage('Clipboard read was aborted.', true);
            } else {
                showMessage('Error accessing clipboard: ' + err.message, true);
            }
        }
    });

    
    imageUrlInput.addEventListener('change', function () {
        const url = this.value.trim();
        if (url !== '') {
            try {
                new URL(url); 
                
                fetch(url)
                    .then(response => response.blob())
                    .then(blob => {
                        const reader = new FileReader();
                        reader.onload = function (e) {
                            displayImage(e.target.result, url.substring(url.lastIndexOf('/') + 1) || 'url_image.png', blob.type || 'application/octet-stream');
                        };
                        reader.readAsDataURL(blob);
                    })
                    .catch(error => {
                        console.error('Error fetching image from URL:', error);
                        showMessage('Failed to load image from URL. Please ensure it\'s a valid image URL and consider CORS policies.', true);
                        clearImage();
                    });

            } catch (e) {
                showMessage('Please enter a valid image URL.', true);
                clearImage();
            }
        } else {
            if (imagePreview.src && (imagePreview.src.startsWith('http://') || imagePreview.src.startsWith('https://'))) {
                clearImage();
            }
        }
    });

    clearImageButton.addEventListener('click', clearImage);

    clearImage();
});