setTimeout(function () {
    $('#toast-popup').fadeOut(1500, function () {
        $(this).remove();
    });
}, 7000);


let isCanceled = false;
const dialog = document.querySelector("dialog");
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
        dialog.close();
        document.getElementById(formToSubmitId).submit();
    });

    noButton.addEventListener("click", () => {
        dialog.close();
    });
}