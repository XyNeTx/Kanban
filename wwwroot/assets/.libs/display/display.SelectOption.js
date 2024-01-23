

$(document).ready(function () {

    //renderSelectBox = function () {

        $("select").each(function () {
            if (this.id != '') {
                var _value = $("#" + this.id).attr('value');
                if (_value != undefined) $("#" + this.id).val(_value);

                //console.log(this.id + ' => ' + _value);
            }
        });

   // }
   //renderSelectBox();
});