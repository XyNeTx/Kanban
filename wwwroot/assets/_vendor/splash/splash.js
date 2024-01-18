class xSplashScreen {
    constructor(height, width) {
        this.height = height;
        this.width = width;
        $('#xSplash-wrapper').attr("style", "visibility:hidden;");
        //console.log("load");
    }


    show = function () {
        $('#xSplash-wrapper').attr("style", "visibility:visible;");
        //console.log("show");

    }

    hide = function () {
        this.text('FINISH');
        $('#xSplash-wrapper').attr("style", "visibility:hidden;");
        //console.log("hide");
        //clearInterval(myInterval);

        $('input[data-inputmask="number"]').each(function () {
            $(this).val(MaskNumber($(this).val()));
        })
        $('input[data-inputmask="currency"]').each(function () {
            $(this).val(MaskCurrency($(this).val()));
        })
    }

    text = function (text = "") {
        //console.log(text.length);
        if (text.length <= 7) $('#xSplash-text').html('<h5>' + text + '</h5>');
        if (text.length > 7) $('#xSplash-text').html('<span style="font-size: 14px;">' + text + '</span>');
        if (text.length > 10) $('#xSplash-text').html('<span style="font-size: 12px;">' + text + '</span>');
        if (text.length > 15) $('#xSplash-text').html('<span style="font-size: 10px;">' + text + '</span>');

        //console.log($('#xSplash-text').html());
    }
}

var xSplash = new xSplashScreen();