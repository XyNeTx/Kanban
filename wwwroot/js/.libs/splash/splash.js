"use strict";
class splashScreen {
    constructor(height, width) {
        this.show = function () {
            $('#splash-wrapper').attr("style", "visibility:visible;");
            //console.log("show");
        };
        this.hide = function () {
            this.text('COMPLETE');
            $('#splash-wrapper').attr("style", "visibility:hidden;");
            //console.log("hide");
            //clearInterval(myInterval);
            $('input[data-inputmask="number"]').each(function () {
                $(this).val(MaskNumber($(this).val()));
            });
            $('input[data-inputmask="currency"]').each(function () {
                $(this).val(MaskCurrency($(this).val()));
            });
        };
        this.text = function (text = "") {
            //console.log(text.length);
            if (text.length <= 7)
                $('#splash-text').html('<h5>' + text + '</h5>');
            if (text.length > 7)
                $('#splash-text').html('<span style="font-size: 14px;">' + text + '</span>');
            if (text.length > 10)
                $('#splash-text').html('<span style="font-size: 12px;">' + text + '</span>');
            if (text.length > 15)
                $('#splash-text').html('<span style="font-size: 10px;">' + text + '</span>');
            //console.log($('#splash-text').html());
        };
        this.height = height;
        this.width = width;
        $('#splash-wrapper').attr("style", "visibility:hidden;");
        //console.log("load");
    }
}
var splash = new splashScreen();
