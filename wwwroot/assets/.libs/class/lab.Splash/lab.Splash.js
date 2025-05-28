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

    hide = function (pCallback = null) {
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
        //$('#splash-wrapper').fadeOut('slow', function () {
            $('#splash-wrapper').attr("style", "visibility:hidden;");

            $('#table-wrapper').attr("style", "visibility:hidden;");

            $('#load-wrapper').attr("style", "visibility:hidden;");
        //});
        $('#splash-wrapper').attr("style", "visibility:hidden;");

        if (pCallback != undefined && typeof (pCallback) == 'function') {
            pCallback();
        }

    }

    text = function (text = "") {
        //console.log(text.length);
        if (text.length <= 7) $('#splash-text').html('<h5>' + text + '</h5>');
        if (text.length > 7) $('#splash-text').html('<span style="font-size: 14px;">' + text + '</span>');
        if (text.length > 10) $('#splash-text').html('<span style="font-size: 12px;">' + text + '</span>');
        if (text.length > 15) $('#splash-text').html('<span style="font-size: 10px;">' + text + '</span>');

        //console.log($('#splash-text').html());
    }


    table(pTable) {

        $('#table-wrapper').attr("style", "visibility:visible;");

        $("#table-wrapper").parent().css({ position: 'relative' });
        $("#table-wrapper").css({
            top: $(pTable).offset().top,
            left: $(pTable).offset().left,
            right: 0,
            position: 'absolute'
        });


        $(pTable).on('draw.dt', function () {
            $('#table-wrapper').height($(pTable).height());
            $('#table-wrapper').width($(pTable).width());
        });

    }

    loading(pElement) {
        console.log('loading');

        $('#load-wrapper').attr("style", "visibility:visible;");

        $('#load-wrapper').height($(pElement).height());
        $('#load-wrapper').width($(pElement).width());

        console.log($('#tblMaster_wrapper .dataTables_scroll').height());
        //////$('#load-wrapper').position.left($('.pcoded-inner-content').position.left());

        $("#load-wrapper").parent().css({ position: 'relative' });
        $("#load-wrapper").css({
            top: $(pElement).offset().top,
            left: $(pElement).offset().left,
            right: 0,
            position: 'absolute'
        });

    }

}

var xSplash = new labSplashScreen();

//export { xSplash };