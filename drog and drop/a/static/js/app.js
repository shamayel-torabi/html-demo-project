// functions

function dragStart(e) {
  const element = e.target;
  draggedDOM = element;

  element.style.opacity = 0.4;
}

function dragEnter(e) {
  const dropZone = this;
  const dragedElement = dropZone.querySelector("#draggable-element");

  if (dragedElement) return false;

  dropZone.appendChild(draggedDOM);
}

function dragDroped(e) {
  // prevent to automatically open some files
  e.preventDefault();
  const dropZone = this;

  dropZone.appendChild(draggedDOM);
}

// DOM
window.draggedDOM = null;

const dropZone = document.getElementsByClassName("drop-zone");
document.getElementById("drog-zone").addEventListener("dragstart", dragStart);



Array.from(dropZone).forEach(function (element) {
  element.addEventListener("dragenter", dragEnter);

  element.addEventListener("dragover", function (event) {
    // prevent to let event drop trigger
    event.preventDefault();
  });

  element.addEventListener("drop", dragDroped);
  element.addEventListener("dragend", function (e) {
    draggedDOM.style.opacity = 1;
  });
});
