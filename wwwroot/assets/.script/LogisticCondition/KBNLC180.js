$(document).ready(function () {

    initial = function () {
        xAjax.Post({
            url: 'KBNLC180/initial',
            then: function (result) {
                xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

                F_Period_onChange();
            }
        })

        xSplash.hide();
    }
    initial();

    F_Period_onChange = function () {
        xAjax.Post({
            url: 'KBNLC180/Period',
            data: {
                "Plant": _PLANT_,
                "Period": ReplaceAll($('#frmCondition #F_Period').val(), '-', ''),
            },
            then: function (result) {
                xDropDownList.bind('#frmCondition #F_Rev', result.data.TB_Import_Delivery, 'F_Rev', 'F_Rev');
            }
        })
    }

    xAjax.onChange('#F_Period', function () {
        F_Period_onChange();
    })

    xAjax.onChange('#F_Rev', function () {
        xAjax.Post({
            url: 'KBNLC180/Rev',
            data: {
                "Plant": _PLANT_,
                "Period": ReplaceAll($('#frmCondition #F_Period').val(), '-', ''),
                "Rev": $('#frmCondition #F_Rev').val(),
            },
            then: function (result) {
                console.log(result);
                xDropDownList.bind('#frmCondition #F_DockFrom', result.data.TB_Dock, 'F_Dock_CD', 'F_Dock_CD');
                xDropDownList.bind('#frmCondition #F_DockTo', result.data.TB_Dock, 'F_Dock_CD', 'F_Dock_CD');

                xDropDownList.bind('#frmCondition #F_RouteFrom', result.data.TB_Route, 'F_Truck_Card', 'F_Truck_Card');
                xDropDownList.bind('#frmCondition #F_RouteTo', result.data.TB_Route, 'F_Truck_Card', 'F_Truck_Card');

                xDropDownList.bind('#frmCondition #F_Route1', result.data.TB_Route, 'F_Truck_Card', 'F_Truck_Card');
                xDropDownList.bind('#frmCondition #F_Route2', result.data.TB_Route, 'F_Truck_Card', 'F_Truck_Card');
            }
        })
    })




});