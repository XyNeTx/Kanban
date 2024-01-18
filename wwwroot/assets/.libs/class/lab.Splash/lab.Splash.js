class labSplashScreen {
    constructor(height, width) {
        this.height = height;
        this.width = width;
        $('#splash-wrapper').attr("style", "visibility:hidden;");
        //console.log("load");
    }


    show = function (pData = null) {
        //$('.theme-loader').fadeIn('slow', function () {
        //    //console.log('xXx');
        //    //$(this).remove();
        //});

        if (pData != null) {
            if (jQuery.type(pData.callback) === 'function') {
                pData.callback();
                return false;
            }

            this.text(pData);
        }
        $('#splash-wrapper').attr("style", "visibility:visible;");


    }

    hide = function () {
        $('input[data-inputmask="number"]').each(function () {
            $(this).val(MaskNumber($(this).val()));
        })
        $('input[data-inputmask="currency"]').each(function () {
            $(this).val(MaskCurrency($(this).val()));
        })

        //$('.theme-loader').fadeOut('slow', function () {
        //    //console.log('xXx');
        //    //$(this).remove();
        //});

        this.text('COMPLETE');
        //$('#splash-wrapper').attr("style", "visibility:hidden;");
        $('#splash-wrapper').fadeOut('slow', function () {
            $('#splash-wrapper').attr("style", "visibility:hidden;");
            //console.log('xXx');
            //$(this).remove();
        });
    }

    text = function (text = "") {
        //console.log(text.length);
        if (text.length <= 7) $('#splash-text').html('<h5>' + text + '</h5>');
        if (text.length > 7) $('#splash-text').html('<span style="font-size: 14px;">' + text + '</span>');
        if (text.length > 10) $('#splash-text').html('<span style="font-size: 12px;">' + text + '</span>');
        if (text.length > 15) $('#splash-text').html('<span style="font-size: 10px;">' + text + '</span>');

        //console.log($('#splash-text').html());
    }
}

var xSplash = new labSplashScreen();