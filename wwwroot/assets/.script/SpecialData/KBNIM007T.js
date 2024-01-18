$(document).ready(function () {

    const xKBNIM007T = new ActionTemplate({
        Controller: _PAGE_,
        Form: 'frmCondition'
    });

    xKBNIM007T.prepare(function () {

        var tblMaster = xDataTable.Initial({
            name: 'tblMaster',
            checking: 0,
            dom: '<"clear">',
            scrollX: true,
            columnTitle: {
                "EN": ['Order No.', 'Parent Part', 'Store Cd.', 'Supplier', 'Deli Date', 'Trip', 'Qty', 'Child Part', 'ST. Child', 'Child Name', 'Order Type'],
                "TH": ['Order No.', 'Parent Part', 'Store Cd.', 'Supplier', 'Deli Date', 'Trip', 'Qty', 'Child Part', 'ST. Child', 'Child Name', 'Order Type'],
                "JP": ['Order No.', 'Parent Part', 'Store Cd.', 'Supplier', 'Deli Date', 'Trip', 'Qty', 'Child Part', 'ST. Child', 'Child Name', 'Order Type'],
            },
            column: [
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_OrderType" }
            ],
            addnew: false,
            rowclick: (row) => {
            }
        });

    });


    xKBNIM007T.initial(function () {

        xAjax.onClick('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });

        xSplash.hide();
    })

    onExecute = function () {
        xKBNIM007T.execute(function () {
            console.log('onExecute');
        })
    }

    onDeleteAll = function () {
        xKBNIM007T.delete(function () {
            console.log('onDelete');
        })
    }



    _setDayInMonth = function () {
        let _sDate = xDate.Now('yyyy-MM-01');
        if ($('#txtOrderNo').val() != '') _sDate = $('#txtOrderNo').val() + '-01';

        let _date = new Date(_sDate);
        //console.log(_date);

        let _sd = new Date(_date.setMonth(_date.getMonth()));
        //console.log(_sd);

        let _e = new Date(_date.setMonth(_date.getMonth() + 1));
        //console.log(_e);

        let _ed = new Date(_e.getFullYear(), _e.getMonth(), 0);
        //console.log(_ed);

        let _holiday = [3, 4, 5, 10, 16, 17, 24, 27, 28, 29, 30, 31];

        //console.log(Number(xDate.Date('dd'))+2);

        let _n, _row6 = 0;
        for (var i = 0; i <= 41; i++) {
            $('#txtDay' + i + '').attr('class', 'col clear');
            $('#txtDay' + i + '').attr('style', 'visibility:visible;display:block;');
            $('#txtDay' + i + '').prop('disabled', 'disabled');

            if (i >= _sd.getDay() && i < (_sd.getDay() + _ed.getDate())) {
                _n = ((_sd.getDay() - i) * -1) + 1;

                $('#day' + i + '').text(_n);

                if (_n <= Number(xDate.Date('dd')) + 3) {
                    $('#txtDay' + i + '').attr('class', 'col workday');
                } else {
                    $('#txtDay' + i + '').prop('disabled', false);
                }

                if (_holiday.indexOf(_n) != -1) {
                    $('#txtDay' + i + '').attr('class', 'col holiday');
                    $('#txtDay' + i + '').prop('disabled', 'disabled');
                }
                //console.log('i>>' + i + ' n>>' + _n);
                if (i >= 35) _row6 = 1;
            } else {
                $('#day' + i + '').text('');
                $('#txtDay' + i + '').attr('style', 'visibility:hidden;display:none;');
            }
        }
        $('#row6').attr('style', 'visibility:visible;display:block;');
        if (_row6 == 0) $('#row6').attr('style', 'visibility:hidden;display:none;');
        //console.log(_row6);
    }
    _setDayInMonth();

    xAjax.onChange('#txtOrderNo', function () {
        _setDayInMonth();
    })

});

