$(document).ready(function () {

    const tblMaster = xDataTable.Initial({
        name: 'tblMaster',
        scrollbar: "300px",
        dom: '<"top"f>t<"clear">',
        checking: 0,
        //running: 0,
        columnTitle: {
            "EN": ['Flag', 'Supplier Name', 'Supplier Code', 'Route', 'Cycle Time'],
            "TH": ['Flag', 'Supplier Name', 'Supplier Code', 'Route', 'Cycle Time'],
            "JP": ['Flag', 'Supplier Name', 'Supplier Code', 'Route', 'Cycle Time'],
        },
        column: [
            { "data": null },
            { "data": "F_short_Logistic" },
            { "data": "F_Supplier" },
            { "data": "F_Route" },
            { "data": "F_Cycle_Time" }
        ],
        //order: 0,
        addnew: false,
        then: function (config) {
        }
    });

    initial = function () {
        xAjax.Post({
            url: 'KBNLC190/initial',
            then: function (result) {
                xDropDownList.bind('#frmCondition #selPlant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

                datPeriod_onChange();
            }
        })

        xSplash.hide();
    }
    initial();

    datPeriod_onChange = function () {
        xAjax.Post({
            url: 'KBNLC190/Period',
            data: {
                "Plant": _PLANT_,
                "Period": ReplaceAll($('#frmCondition #datPeriod').val(), '-', ''),
            },
            then: function (result) {
                xDropDownList.bind('#frmCondition #selRev', result.data.TB_Import_Delivery, 'F_Rev', 'F_Rev');
            }
        })
    }

    xAjax.onChange('#datPeriod', function () {
        datPeriod_onChange();
    })


    xAjax.onClick('#btnCheckStatus', function () {
        console.log(xAjax.item('#rdoSupplier').val());

    });

    xAjax.onClick('#btnSearch', function () {
        let _selRev = $('#selRev').val();
        //console.log(_selRev);
        if (_selRev == '' || _selRev == null)
            return xSwal.Info({ "message": 'Please input Revision before click [Search]!!!' });

        xSwal.Question({
            message: 'Do you want change start date?',
            cancel: function () {
                xAjax.Post({
                    url: 'KBNLC190/Search',
                    data: {
                        "Plant": _PLANT_,
                        "Period": ReplaceAll($('#frmCondition #datPeriod').val(), '-', ''),
                        "Rev": $('#frmCondition #selRev').val(),
                        "Flag": xAjax.item('#rdoSupplier').val(),
                    },
                    then: function (result) {
                        xDataTable.bind('#tblMaster', result.data.TB_Import_Delivery);
                    }
                })
            }
        })

    })

    xAjax.onClick('#btnInterface', function () {
        if (xAjax.item('#rdoSupplier').val() == 9) return xSwal.Error({ message: 'Can not Interface Data, because Interface.'});

        xSwal.Question({
            message: 'Do you want Interface Cycle Time to Delivery?',
            then: function () {
                let _StartDate = ReplaceAll(xAjax.item('#datStartDate').val(), '-', '');

                if (_StartDate < xDate.Now('yyyyMMdd'))
                    return xSwal.Error({ message: 'Can not input Start Date < Current Date!!!' });

                xAjax.Post({
                    url: 'KBNLC190/GetDateStartForecast',
                    data: {
                        "StartDate": xDate.Now('yyyyMMdd')
                    },
                    then: function (result) {
                        console.log(_StartDate + '==' + result.data[0].StartForecastDate);

                        if (_StartDate < result.data[0].StartForecastDate)
                            return xSwal.Error({ message: 'Please select Start Date More than ' + result.data[0].ForecastDate + ' Because effect to Normal Ordering.' });

                        console.log(xDataTable.Selects('tblMaster'));

                    }
                })
            }
        })


    })


});