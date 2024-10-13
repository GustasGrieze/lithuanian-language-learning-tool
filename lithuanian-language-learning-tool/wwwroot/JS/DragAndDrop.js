window.dragAndDrop = function dragAndDrop(className)
{
    const position = { x: 0, y: 0 }

    interact(className).draggable({
        listeners: {
            start(event) {
                console.log(event.type, event.target)
            },
            move(event) {
                position.x += event.dx
                position.y += event.dy

                event.target.style.transform =
                    `translate(${position.x}px, ${position.y}px)`
            },
        }, modifiers: [
            interact.modifiers.restrictRect({
                restriction: '.main',
                endOnly: true
            })]
    })
};

// wwwroot/js/dropzone.js

window.dropzoneHandler = {
    initDraggable: function (draggableId, dropzoneId) {
        interact(draggableId).draggable({
            inertia: true,
            modifiers: [
                interact.modifiers.restrictRect({
                    restriction: 'parent',
                    endOnly: true
                })
            ],
            autoScroll: true,
            onmove: function (event) {
                const target = event.target;
                const x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx;
                const y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;

                target.style.transform = `translate(${x}px, ${y}px)`;
                target.setAttribute('data-x', x);
                target.setAttribute('data-y', y);
            }
        });

        interact(dropzoneId).dropzone({
            accept: '.draggable',
            overlap: 0.75,  // Change this value as per your requirement
            ondrop: function (event) {
                alert(event.relatedTarget.id + ' was dropped into ' + event.target.id);
                event.target.appendChild(event.relatedTarget);
            },
            ondropactivate: function (event) {
                event.target.classList.add('drop-active');
            },
            ondropdeactivate: function (event) {
                event.target.classList.remove('drop-active');
            },
            checker: function (
                dragEvent,
                event,
                dropped,
                dropzone,
                dropElement,
                draggable,
                draggableElement
            ) {
                // Additional checks (e.g., allow only one element per drop zone)
                return dropped && !dropElement.hasChildNodes();
            }
        });
    }
};



window.initializeDragAndDrop = function (dotNetHelper) {
    // Initialize draggable punctuation marks
    document.querySelectorAll('.draggable').forEach(function (draggableElement) {
        draggableElement.addEventListener('dragstart', function (event) {
            var mark = draggableElement.getAttribute('data-mark');
            event.dataTransfer.setData('text/plain', mark);
        });
    });

    // Use event delegation for the sentence
    var sentenceElement = document.querySelector('.sentence');
    if (sentenceElement) {
        sentenceElement.addEventListener('dragover', function (event) {
            event.preventDefault(); // Allow drop
            var target = event.target;
            if (target && target.classList.contains('letter')) {
                // Adjust styles to create a gap
                target.style.marginLeft = '10px'; // Adjust the value as needed
            }
        });

        sentenceElement.addEventListener('dragleave', function (event) {
            var target = event.target;
            if (target && target.classList.contains('letter')) {
                // Reset styles
                target.style.marginLeft = '0px';
            }
        });

        sentenceElement.addEventListener('drop', function (event) {
            event.preventDefault();
            var target = event.target;
            if (target && target.classList.contains('letter')) {
                var punctuation = event.dataTransfer.getData('text/plain');
                var id = target.id; // e.g., "letter-5"
                var index = parseInt(id.replace('letter-', ''));
                // Call Blazor method to update the sentence
                dotNetHelper.invokeMethodAsync('InsertPunctuation', index, punctuation);
                // Reset styles
                target.style.marginLeft = '0px';
            }
        });
    }
};

