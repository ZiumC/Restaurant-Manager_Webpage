setTimeout(function () {
    $('#toast-popup').fadeOut(1500, function () {
        $(this).remove();
    });
}, 7000);



function getSelectedItem(e) {
    let number = e.id.split("=")[1];

    let element = document.getElementById("types-options=" + number);
    let value = element.value;

    if (value == 0) {
        document.getElementById("table-row-" + number).classList.add("table-danger");
        document.getElementById("error-assign").innerHTML = "Please select an option.";
    }

    let form = document.getElementById("assign-emp-rest=" + number);
    let idTypeField = document.createElement("input");
    idTypeField.setAttribute("type", "hidden");
    idTypeField.setAttribute("value", value);
    idTypeField.setAttribute("name", "idType");
    form.appendChild(idTypeField)

   document.getElementById("assign-emp-rest=" + number).submit();
}

const dialog = document.querySelector("dialog");
let isCanceled = false;
const noButton = document.getElementById("n-btn");
const yesButton = document.getElementById("y-btn");
function showModal(e) {
    let formToSubmitId = ""
    let message = "Do you want to";
    let buttonName = e.id.split("=")[0];
    let buttonId = e.id.split("=")[1];


    if (buttonName == "rm-from-rest") {
        message = message + " remove employee from restaurant?"
        formToSubmitId = "rm-emp-rest=" + buttonId;
    } else if (id = "rm-emp-data") {
        message = message + " fire employee? This action can't be undone!"
        formToSubmitId = "fire-emp-rest=" + buttonId;
    } else {

    }

    document.getElementById("message-dialog").innerHTML = message;

    dialog.showModal();
    yesButton.addEventListener("click", () => {
        document.getElementById(formToSubmitId).submit();
    });

    noButton.addEventListener("click", () => {
        dialog.close();
    });
}