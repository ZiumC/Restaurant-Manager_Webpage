setTimeout(function () {
    $('#toast-popup').fadeOut(1500, function () {
        $(this).remove();
    });
}, 7000);