
$(document).ready(function () {

    $('.input-group.LOV .input-group-text').on('click', function (e) {
        if ($(this).attr('disabled') != 'disabled') {

            _onLOVLoad(this);
        }
    });

    $('.input-group.LOV input').on('blur', function (e) {
        //console.log($(this));
        if ($(this).attr('disabled') != 'disabled') {
            _onLOVBlur(this);
        }
    });

    $('.input-group.LOV input').on('keypress', function (e) {
        if (e.which === 13) {
            event.preventDefault();
            _onLOVBlur(this);
        }
    });

    var _object;
    var _value;
    _onLOVLoad = function (pObject = null) {
        xSplash.show();
        xSplash.text('Loading data');



        //_object = $('.input-group.LOV .input-group-text .fas.fa-list').attr('controller').split('.');
        //_value = $('.input-group.LOV .input-group-text .fas.fa-list').attr('value').split(',');
        _object = $($(pObject)[0].innerHTML).attr('controller').split('.');
        _value = $($(pObject)[0].innerHTML).attr('value').split(',');
        var _controller = _object[0];
        var _method = _object[1];

        if ($.fn.DataTable.isDataTable('#_tblLOV')) {
            $('#_tblLOV').DataTable().destroy();
            $('#_tblLOV')[0].outerHTML = '<table id="_tblLOV" class="display" style="width:100%"></table>'
        }


        var _columnTitle = Array('No.');
        var _column = Array({ "data": "RunningNo" });
        for (var i = 0; i < _value.length; i++) {
            var _description = _value[i].split(':')[1];
            _columnTitle[i + 1] = _description;
            _column[i + 1] = { "data": _description };
        }

        console.log('/' + _controller + '/' + _method);
        xSplash.hide();


        $.ajax(
            {
                type: "POST",
                headers: ajexHeader,
                url: 'https://localhost:7267/LOVKB3/PartList',
                //url: '../' + _controller + '/' + _method,
                success: function (result) {
                    console.log(result);
                    //xSplash.hide();
                    //if (result.response == "OK") {

                    //    if (result.data.length > 0) {

                    //        var _tblLOV = xDataTable.Initial({
                    //            name: '_tblLOV',
                    //            pageLength: 10,
                    //            running: 0,
                    //            data: result.data,
                    //            toolbar: '<"top"f>t<"bottom"ip><"clear">',
                    //            columnTitle: {
                    //                "EN": _columnTitle,
                    //                "TH": _columnTitle,
                    //            },
                    //            column: _column,
                    //            addnew: false,
                    //            rowclick: function (row) {
                    //                _onLOVClick(row, _value);
                    //            },
                    //            then: function (config) {
                    //                //console.log('finish');
                    //                xSplash.hide();
                    //            }
                    //        });


                            $('#ModalLOV').modal('toggle');

                    //    }
                    //}
                    xSplash.hide();

                },
                finally: function () {

                    xSplash.hide();
                }
            }
        );
    }


    _onLOVClick = function (row, value) {
        //var _return = $('.input-group.LOV .input-group-text .fas.fa-list').attr('value').split(',');
        var _return = value;

        console.log(value);

        for (var i = 0; i < _return.length; i++) {
            var _ret = _return[i].split(':');
            var _value = $('#' + _ret[0]);
            var _description = _ret[1];

            _value.val(row[_description]);
        }

        $('#ModalLOV').modal('hide');
    }



    _onLOVBlur = function (pThis = null) {
        var _LOV = $($($($(pThis).parent())[0].innerHTML)[2].innerHTML).first();
        var _item = $($($(pThis).parent())[0].innerHTML);

        var _object = _LOV.attr('controller').split('.');
        var _value = _LOV.attr('value').split(',');


        if ($(pThis).val() == "") {
            for (var i = 0; i < _value.length; i++) {
                var _ret = _value[i].split(':');
                $('#' + _ret[0]).val('');
            }
            return false;
        }

        var _controller = _object[0];
        var _method = _object[1];

        $.ajax(
            {
                type: "POST",
                headers: ajexHeader,
                url: '/' + _controller + '/' + _method,
                data: { text: $(pThis).val() },
                success: function (result) {
                    //console.log(result);

                    if (result.response == "OK") {

                        if (result.data.length > 0) {

                            for (var i = 0; i < _value.length; i++) {
                                var _ret = _value[i].split(':');
                                var _obj = $('#' + _ret[0]);
                                var _field = _ret[1];

                                _obj.val('');
                                if (result.data[0][_field] != '') _obj.val(result.data[0][_field]);
                            }
                        }
                    }

                }
            }
        );

    };

});