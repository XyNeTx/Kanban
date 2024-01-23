

$(document).ready(function () {

    renderSelectBox = function () {

        $("select").each(function () {
            if (this.id != '') {
                var _value = $("#" + this.id).attr('value');
                $("#" + this.id).val(_value);

                //console.log(_value);
            }
        });

    }
    renderSelectBox();

});