document.addEventListener('DOMContentLoaded', function () {
    document
        .getElementById('registerMeAsEmployee')
        .addEventListener('change', empOptionsFields);

})

function empOptionsFields(e) {

    const pesel = document.getElementById("pesel");
    const hiredDate = document.getElementById("hiredDate");

    if (this.checked) {
        removeAttr(pesel, 'disabled');
        removeAttr(hiredDate, 'disabled');
        addAttr(pesel, 'required');
        addAttr(hiredDate, 'required');
    } else {
        removeAttr(pesel, 'required');
        removeAttr(hiredDate, 'required');
        addAttr(pesel, 'disabled');
        addAttr(hiredDate, 'disabled');
    }
}

function removeAttr(element, value) {
    element.removeAttribute(value)
}

function addAttr(element, value) {
    element.setAttribute(value, true);
}
