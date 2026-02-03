let draggable = document.querySelectorAll("[draggable]");
let targets = document.querySelectorAll("[data-drop-target]");

for (let i = 0; i < draggable.length; i++) {
  draggable[i].addEventListener("dragstart", handleDragStart);
}

for (let i = 0; i < targets.length; i++) {
  targets[i].addEventListener("dragover", handleOverDrop);
  targets[i].addEventListener("drop", handleOverDrop);
  targets[i].addEventListener("dragenter", handleDragEnterLeave);
  targets[i].addEventListener("dragleave", handleDragEnterLeave);
}

function handleDragStart(e) {
  e.dataTransfer.setData("text", this.id);
}

function handleDragEnterLeave(e) {
  if (e.type == "dragenter") {
    //this.className = "drag-enter";
    this.classList.add("drag-enter");
  } else {
    //this.className = "";
    this.classList.remove("drag-enter");
  }
}

function handleOverDrop(e) {
  e.preventDefault();

  if (e.type != "drop") {
    return;
  }

  let draggedId = e.dataTransfer.getData("text");
  let draggedEl = document.getElementById(draggedId);
  if (draggedEl.parentNode == this) {
    return;
  }

  draggedEl.parentNode.removeChild(draggedEl);
  this.appendChild(draggedEl);
  this.classList.remove("drag-enter");
}
