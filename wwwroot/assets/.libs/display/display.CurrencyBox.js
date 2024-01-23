
$(document).ready(function () {

    $('[mask="currency"]').on("keydown", function (event) {
        if (!(event.key === 'Backspace' ||
            event.key === 'Delete' ||
            event.key === 'Tab' ||
            event.key === 'Shift' ||
            event.key === 'Ctrl' ||
            event.key === 'Alt' ||
            event.key === 'Home' ||
            event.key === 'End' ||
            event.keyCode === 37 ||
            event.keyCode === 39 ||
            event.key === '+' ||
            event.key === '-' ||
            event.key === '*' ||
            event.key === '/' ||
            event.key === ',' ||
            event.key === '.'
        ) && isNaN(Number(event.key))) {
            event.preventDefault();
        }
    });

    $('[mask="currency"]').on("keyup", function (event) {
        if (event.key === 'Enter') {
            $(this).val(eval($(this).val()));
        }
    });

    $('[mask="currency"]').on("blur", function () {
        $(this).val(MaskCurrency($(this).val(), 2, true, true));
    });




    $('[mask="number"]').on("keydown", function (event) {
        if (!(event.key === 'Backspace' ||
            event.key === 'Delete' ||
            event.key === 'Tab' ||
            event.key === 'Shift' ||
            event.key === 'Ctrl' ||
            event.key === 'Alt' ||
            event.key === 'Home' ||
            event.key === 'End' ||
            event.keyCode === 37 ||
            event.keyCode === 39 ||
            event.key === '+' ||
            event.key === '-' ||
            event.key === '*' ||
            event.key === '/' ||
            event.key === ',' ||
            event.key === '.'
        ) && isNaN(Number(event.key))) {
            event.preventDefault();
        }
    });

    $('[mask="number"]').on("keyup", function (event) {
        if (event.key === 'Enter') {
            $(this).val(eval($(this).val()));
        }
    });

    $('[mask="number"]').on("blur", function () {
        $(this).val(MaskNumber($(this).val()))
    });
});