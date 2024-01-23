

$(document).ready(function () {

    $('input[type="checkbox"]').each(function (index, obj) {

        if ($('#' + obj.id).prop('type') == 'checkbox') {

            if (obj.id != '') {
                var _value = $("#" + obj.id).attr('value');
                if (_value != undefined) $("#" + obj.id).val(_value);
                if (_value == 1 || _value == true) $("#" + obj.id).attr('checked', 'true');

                //console.log(obj.id + ' => ' + _value);
            }
        }

    });


    $('input[type="checkbox"]').on('click', function (e) {

        //console.log($("#" + this.id).prop('checked'));
        var _value = $("#" + this.id).prop('checked');
        $("#" + this.id).val((_value ? '1' : '0'));
        //console.log(this.id);
        //if ((pFunction != null) && (typeof (pFunction) == 'function')) {
        //    pFunction(e);
        //}
    });


});