document.addEventListener('DOMContentLoaded', function () {
    const noteTriggers = document.querySelectorAll('.note-trigger');
    let currentPopover = null;
    let showTimeout;
    let hideTimeout;

    noteTriggers.forEach(trigger => {
        const noteContent = trigger.dataset.noteContent;

        if (noteContent && noteContent.trim() !== '') {
            let popover = document.createElement('div');
            popover.classList.add('note-popover');
            popover.innerHTML = `<p>${noteContent}</p>`;
            document.body.appendChild(popover);

            popover.style.display = 'none';

            trigger.addEventListener('mouseenter', () => {
                clearTimeout(hideTimeout);
                showTimeout = setTimeout(() => {
                    const triggerRect = trigger.getBoundingClientRect();

                    popover.style.left = `${triggerRect.right + window.scrollX - (popover.offsetWidth * 0.2)}px`;
                    popover.style.top = `${triggerRect.top + window.scrollY - popover.offsetHeight - 60}px`;

                    popover.style.display = 'block';
                    currentPopover = popover;

                    popover.addEventListener('mouseenter', () => {
                        clearTimeout(hideTimeout);
                    });
                    popover.addEventListener('mouseleave', () => {
                        clearTimeout(showTimeout);
                        hideTimeout = setTimeout(() => {
                            popover.style.display = 'none';
                            currentPopover = null;
                        }, 0);
                    });

                }, 300);
            });

            trigger.addEventListener('mouseleave', () => {
                clearTimeout(showTimeout);
                hideTimeout = setTimeout(() => {
                    if (currentPopover && currentPopover === popover) {
                        currentPopover.style.display = 'none';
                        currentPopover = null;
                    }
                }, 0);
            });
        }
    });

    document.addEventListener('click', (event) => {
        if (currentPopover && !event.target.closest('.note-trigger') && !event.target.closest('.note-popover')) {
            currentPopover.style.display = 'none';
            currentPopover = null;
        }
    });
});