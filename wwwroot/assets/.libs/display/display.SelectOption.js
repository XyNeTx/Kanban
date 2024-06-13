

$(document).ready(function () {

    $('select').each(function () {
        if (this.id != '') {
            var _value = $("#" + this.id).attr('value');
            if (_value != undefined) $("#" + this.id).val(_value);

            //console.log(this.id + ' => ' + _value);
        }
    });


    $('select').change(function () {
        window[this.id].value = $(this).val();
        window[this.id].title = $('#' + this.id + ' option:selected').text();
    });

});