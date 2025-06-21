
        document.addEventListener('DOMContentLoaded', function () {
            var toastElList = [].slice.call(document.querySelectorAll('.toast'));
            var toastList = toastElList.map(function (toastEl) {
                return new bootstrap.Toast(toastEl);
            });
            toastList.forEach(toast => toast.show());

            const pasteArea = document.querySelector('.image-paste-area');
            const imageUrlInput = document.getElementById('imageUrlInput');
            const uploadImageButton = document.getElementById('uploadImageButton');
            const imageUploadInput = document.getElementById('imageUploadInput');
            const pasteImageButton = document.getElementById('pasteImageButton');
            const imagePreview = document.getElementById('imagePreview');
            const hiddenInput = document.getElementById('imageHidden');
            const clearButton = document.getElementById('clearImageButton');
            const pasteAreaText = document.getElementById('pasteAreaText');

            function displayImage(src) {
                imagePreview.src = src;
                imagePreview.style.display = 'block';
                hiddenInput.value = src;
                clearButton.style.display = 'block';
                pasteAreaText.style.display = 'none';
                pasteArea.style.border = '1px solid #dee2e6';
            }

            function clearImage() {
                imagePreview.src = '#';
                imagePreview.style.display = 'none';
                hiddenInput.value = '';
                clearButton.style.display = 'none';
                pasteAreaText.style.display = 'block';
                imageUrlInput.value = '';
                imageUploadInput.value = '';
                pasteArea.style.border = '2px dashed #ccc';
            }

            pasteArea.addEventListener('paste', function(event) {
                event.preventDefault();

                const items = event.clipboardData.items;
                let imageFound = false;

                for (let i = 0; i < items.length; i++) {
                    if (items[i].type.startsWith('image/')) {
                        const file = items[i].getAsFile();
                        if (file) {
                            const reader = new FileReader();
                            reader.onload = function(e) {
                                displayImage(e.target.result);
                            };
                            reader.readAsDataURL(file);
                            imageFound = true;
                            return;
                        }
                    }
                }

                if (!imageFound) {
                    alert('No image data found in clipboard. Please copy an actual image (e.g., from a screenshot tool) and try pasting with Ctrl+V.');
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
                                const reader = new FileReader();
                                reader.onload = function(e) {
                                    displayImage(e.target.result);
                                };
                                reader.readAsDataURL(blob);
                                imageFoundInClick = true;
                                return;
                            }
                        }
                    }

                    if (!imageFoundInClick) {
                        alert('No image data found in clipboard. Please copy an image and try again. (May require browser permission)');
                    }
                } catch (err) {
                    console.error('Failed to read clipboard contents:', err);
                    if (err.name === 'NotAllowedError' || err.name === 'SecurityError') {
                        alert('Permission to access clipboard denied or blocked by browser. Please ensure you allow clipboard access, or use Ctrl+V to paste directly into the box below.');
                    } else if (err.name === 'AbortError') {
                        alert('Clipboard read was aborted.');
                    } else {
                        alert('Error accessing clipboard: ' + err.message);
                    }
                }
            });

            uploadImageButton.addEventListener('click', () => {
                imageUploadInput.click();
            });

            imageUploadInput.addEventListener('change', function() {
                if (this.files.length > 0) {
                    const file = this.files[0];
                    const reader = new FileReader();
                    reader.onload = (e) => {
                        displayImage(e.target.result);
                    };
                    reader.readAsDataURL(file);
                }
            });

            imageUrlInput.addEventListener('change', function() {
                const url = this.value.trim();
                if (url !== '') {
                    try {
                        new URL(url);
                        displayImage(url);
                    } catch (e) {
                        alert('Please enter a valid image URL.');
                        clearImage();
                    }
                } else {
                    if (!hiddenInput.value || hiddenInput.value.startsWith('http') || hiddenInput.value.startsWith('https')) {
                         clearImage();
                    }
                }
            });

            clearButton.addEventListener('click', clearImage);
            
            pasteArea.addEventListener('click', function() {
                imageUploadInput.click();
            });

            if (hiddenInput.value && hiddenInput.value !== '') {
                displayImage(hiddenInput.value);
            }
        });
