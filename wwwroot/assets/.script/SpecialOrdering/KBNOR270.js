$(document).ready(async () => {

    //$("#inpExDeliveryFrom").initDatepicker();
    //$("#inpExDeliveryTo").initDatepicker();

    await ExGetPO();
    //await ExGetSupplier();
    await ExGetPDS();

    $(".selectpicker").selectpicker("refresh");
    xSplash.hide();
});


$("#chkboxExCustOrder").change(async () => {
    if ($("#chkboxExCustOrder").prop("checked")) {
        $("#inpExIssuedYM").prop("disabled", false);
        $("#inpExPONoFrom").prop("disabled", false);
        $("#inpExPONoTo").prop("disabled", false);
    } else {
        $("#inpExIssuedYM").prop("disabled", true);
        $("#inpExPONoFrom").prop("disabled", true);
        $("#inpExPONoTo").prop("disabled", true);
    }
    $(".selectpicker").selectpicker("refresh");
});

$("#chkboxExPDSNo").change(async () => {
    if ($("#chkboxExPDSNo").prop("checked")) {
        $("#inpExPDSNoFrom").prop("disabled", false);
        $("#inpExPDSNoTo").prop("disabled", false);
        await ExGetPDS();
    } else {
        $("#inpExPDSNoFrom").prop("disabled", true);
        $("#inpExPDSNoTo").prop("disabled", true);
    }


    $(".selectpicker").selectpicker("refresh");

});

$("#inpExPONoFrom").change(async () => {
    await $("#inpExPONoTo").val($("#inpExPONoFrom").val());
    await ExGetPDS();
});
$('#inpExPONoTo').change(async () => {
    await ExGetPDS();
});
$("#inpExPDSNoFrom").change(async () => {
    await $("#inpExPDSNoTo").selectpicker("val", $("#inpExPDSNoFrom").val());
});

$("#inpExIssuedYM").change(async () => {
    await ExGetPO();
});

//$("#btnExExport").click(async () => {
//    await ExExportData();
//});

$("#btnPDS").click(async () => {
    await Preview();
});
$("#btnPart").click(async () => {

    await PreviewKB();
});

//ExGetSupplier = async () => {

//    _xLib.AJAX_Get("/api/KBNOR280/GetSupplier", null,
//        async (success) => {
//            $("#inpExSupplierFrom").empty();
//            $("#inpExSupplierTo").empty();
//            $("#inpExSupplierFrom").append("<option value='' hidden>-- Select Supplier --</option>");
//            $("#inpExSupplierTo").append("<option value='' hidden>-- Select Supplier --</option>");
//            success.data.forEach((e) => {
//                $("#inpExSupplierFrom").append(`<option value='${e.f_Supplier_Code}'>${e.f_Supplier_Code}</option>`);
//                $("#inpExSupplierTo").append(`<option value='${e.f_Supplier_Code}'>${e.f_Supplier_Code}</option>`);
//            });
//            $(".selectpicker").selectpicker("refresh");
//            //console.log(success);
//        }
//    );
//}

ExGetPO = async () => {
    let GetQuery = {
        IssuedYM: $("#inpExIssuedYM").val().replace("-", ""),
    }

    _xLib.AJAX_Get("/api/KBNOR280/GetPO", GetQuery,
        async (success) => {
            //console.log(success);
            $("#inpExPONoFrom").empty();
            $("#inpExPONoTo").empty();
            $("#inpExPONoFrom").append("<option value='' hidden>-- Select PO --</option>");
            $("#inpExPONoTo").append("<option value='' hidden>-- Select PO --</option>");
            success.data.forEach((e) => {
                $("#inpExPONoFrom").append(`<option value='${e.f_PO_Customer}'>${e.f_PO_Customer}</option>`);
                $("#inpExPONoTo").append(`<option value='${e.f_PO_Customer}'>${e.f_PO_Customer}</option>`);
            });
            $(".selectpicker").selectpicker("refresh");
        }
    );
};

ExGetPDS = async () => {
    let GetQuery = {
        POFrom: $("#inpExPONoFrom").val(),
        POTo: $("#inpExPONoTo").val(),
    }

    _xLib.AJAX_Get("/api/KBNOR280/GetPDS", GetQuery,
        async (success) => {
            //console.log(success);
            $("#inpExPDSNoFrom").empty();
            $("#inpExPDSNoTo").empty();
            $("#inpExPDSNoFrom").append("<option value='' hidden>-- Select PDS --</option>");
            $("#inpExPDSNoTo").append("<option value='' hidden>-- Select PDS --</option>");
            success.data.forEach((e) => {
                $("#inpExPDSNoFrom").append(`<option value='${e.f_PDS_No}'>${e.f_PDS_No}</option>`);
                $("#inpExPDSNoTo").append(`<option value='${e.f_PDS_No}'>${e.f_PDS_No}</option>`);
            });
            $(".selectpicker").selectpicker("refresh");
        }
    );
}

Preview = async (e) => {
    let data = [];

    if ($("#chkboxExCustOrder").prop("checked") || !$("#chkboxExPDSNo").prop("checked")) {

        $("#inpExPDSNoFrom option").each((i, e) => {
            if ($(e).val() != "") {
                let obj = {
                    F_OrderNO: $(e).val()
                }
                data.push(obj);
            }
        });

        if (data.length == 0) {
            xSwal.error("Please Select PO No.");
            return;
        }

    }

    console.log(data);

    _xLib.AJAX_Post(`/api/KBNOR270/Preview`, data,
        async (success) => {
            console.log(success);
            await xSwal.success(success.response, success.message);
            let link = `http://hmmta-tpcap/E-Report/Report.aspx?Register=REC&PDSNoFrom=${data[0].F_OrderNO}&PDSNoTo=${data[data.length-1].F_OrderNO}&DateFrom=2024-07-01&DateTo=2999-12-31`;

            await window.open(link, "_blank");
        },
        async (error) => {
            xSwal.xError(error);
        }
    );
}

PreviewKB = async (e) => {
    let PDSNoFrom = $("#inpExPDSNoFrom").val();
    let PDSNoTo = $("#inpExPDSNoTo").val();

    if(PDSNoFrom == "" || PDSNoTo == ""){
        xSwal.error("Error!!!","Please select PDS No.");
        return;
    }

    _xLib.AJAX_Get(`/api/KBNOR270/PreviewKB`, null,
        async (success) => {
            console.log(success);
            await xSwal.success(success.response, success.message);

            //await window.open(link, "_blank");
        }
    );

    var _dtKanban = await xAjax.ExecuteJSON({
        data: {
            "Module": "[exec].[spKBNOR700_KANBAN]",
            "@pUserCode": ajexHeader.UserCode,
            "@pPlant": ajexHeader.Plant,
            "@pDeliveryDate": xDate.Date('yyyyMMdd', 'MM=-3'),
            "@F_orderType": "S",
        },
    });

    if (_dtKanban.rows == null) MsgBox("Order data not found.", MsgBoxStyle.Information, "Information");
    if (_dtKanban.rows != null) {

        xSplash.show();

        var _pds = '', _OrderType = '', _Plant = '', _DeliveryDate = '';
        //var _dt = _dtKanban;
        for (var i = 0; i < _dtKanban.rows.length; i++) {

            var _BoxQty = _dtKanban.rows[i].F_Box_Qty;
            var _UnitAmount = _dtKanban.rows[i].F_Unit_Amount;

            var _page_total = Math.floor(_UnitAmount / _BoxQty);
            var _ceil = Math.ceil(_UnitAmount / _BoxQty);
            var _amt = _UnitAmount - (_page_total * _BoxQty);

            var _dt = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR700_KANBANA4]",
                    "@pUserCode": ajexHeader.UserCode,
                    "@F_OrderNo": Trim(_dtKanban.rows[i].F_OrderNO),
                    "@F_PartNO": Trim(_dtKanban.rows[i].F_PartNO),
                    "@F_Kanban_No": Trim(_dtKanban.rows[i].F_Kanban_No),
                    "@pMax": _ceil,
                    "@pBoxQty": (_amt > 0 ? _amt : '')
                },
            });
            //console.log(i);
            var _pcent = (i / _dtKanban.rows.length) * 100;
            xItem.progress({ id: 'prgProcess', current: _pcent, label: 'Processing, Please wait : ' + i + '/' + _dtKanban.rows.length + ' ({{##.##}}) %' });

        }

        for (var i = 0; i < _dt.rows.length; i++) {
            _OrderType = Trim(_dt.rows[i].F_Order_Type);
            _OrderType = (_OrderType == 'N' ? 'NORMAL' : (_OrderType == 'S' ? 'SPECIAL' : (_OrderType == 'U' ? 'URGENT' : '')));

            _Plant = Trim(_dt.rows[i].F_Plant);
            _Plant = (_Plant == '1' ? 'SAMRONG PLANT' : (_Plant == '2' ? 'BANGPEE PLANT' : (_Plant == 'B' ? 'SAMRONG WAREHOUSE' : 'BANGPRAKONG PLANT')));

            _DeliveryDate = Trim(_dt.rows[i].F_Delivery_Date);
            _DeliveryDate = _DeliveryDate.substring(6, 8) + '/' + _DeliveryDate.substring(4, 6) + '/' + _DeliveryDate.substring(0, 4);

            _pds = _pds + '00' + Trim(_dt.rows[i].F_PartNO) + '0|';
            _pds = _pds + Trim(_dt.rows[i].F_Supplier_INT) + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Supplier_CD) + '|';
            _pds = _pds + _DeliveryDate + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Delivery_Trip) + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Delivery_Time) + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Delivery_Dock) + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Dock_Code) + '|';
            _pds = _pds + _Plant + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Kanban_No) + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Part_name) + '|';
            _pds = _pds + Trim(_dt.rows[i].F_OrderNO) + '|';
            _pds = _pds + _OrderType + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Box_Qty) + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Page) + '/' + Trim(_dt.rows[i].F_Page_Total) + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Unit_Amount) + '|';
            _pds = _pds + Trim(_dt.rows[i].F_Remark) + '|';

            _pds = (_pds != '' ? _pds + ',@@,' : _pds + '');
            //console.log(i);
        }
        xItem.progress({ id: 'prgProcess', current: 100, label: 'Processing, Please wait : {{##.##}} %' });


        await xAjax.Post({
            url: 'KBNOR700/PDS_GENQRCODE',
            data: {
                'PDSNO': _pds
            },
            then: function (result) {
                //console.log(result);
                if (_xLib.GetCookie('isDev') == "1") {
                    return _xLib.OpenReport("/KBNOR700KANBAN_X2", `pUserCode=${ajexHeader.UserCode}` +
                        `&OrderNo=${PDSNoFrom}&OrderNoTo=${PDSNoTo}&DeliveryDate=${moment().add(-3, 'months').format('YYYYMMDD')}
                        &DeliveryDateTo=${moment().add(3, 'months').format('YYYYMMDD')}`);
                }
                else {

                    window.open(
                        _REPORTINGSERVER_ + '%2fKB3%2f' + pRPTNAME + '&rs:Command=Render'
                        + '&pUserCode=' + ajexHeader.UserCode
                        + '&OrderNo=' + PDSNoFrom
                        + '&OrderNoTo=' + PDSNoTo
                        + '&DeliveryDate=' + moment().add(-3, 'months').format('YYYYMMDD')//itmDelivery.value
                        + '&DeliveryDateTo=' + moment().add(3, 'months').format('YYYYMMDD')//itmDeliveryTo.value
                        , '_blank'
                    );
                }
            }
        });

        xSplash.hide();

    }
}