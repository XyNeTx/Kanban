$(document).ready(function () {

    ExGetPDS();

    xAjax.onClick('btnExit', async function () {
        xAjax.redirect('KBNOR300');
    });

    xAjax.onClick('#btnPrintPDS', async function () {
        xSplash.show("Generate PDS Data");
        let data = [];
        $("#itmPDS option").each((i, e) => {
            if ($(e).val() >= $("#itmPDS").val() && $(e).val() <= $("#itmPDSTo").val()) {
                let obj = {
                    F_OrderNO: $(e).val()//.replace(/_N|_U/g, "")
                }
                data.push(obj);
            }
        });

        //let obj = {
        //    F_OrderNO: $("#itmPDS").val(),
        //    F_OrderNO_To: $("#itmPDSTo").val()
        //}

        if ($("#itmPDS").val() == "" || $("#itmPDSTo").val() == "") {
            xSwal.error("Please Select PO No.");
            return;
        }

        console.log(data);

        _xLib.AJAX_Post(`/api/KBNOR370/Preview`, data,
            async (success) => {
                console.log(success);
                //let link = `http://hmmta-tpcap/E-Report/Report.aspx?Register=REC&PDSNoFrom=${data[0].F_OrderNO.replace(/_N|_U/g, "")}&PDSNoTo=${data[data.length - 1].F_OrderNO.replace(/_N|_U/g, "")}&DateFrom=2024-07-01&DateTo=2999-12-31`;

                //await window.open(link, "_blank");

                let pdsToGen = []
                success.data.forEach((e) => {
                    console.log(e);
                    let obj = {
                        F_OrderNO: e.trim()
                    }
                    pdsToGen.push(obj);
                })

                await _xLib.AJAX_Post(`/api/KBNOR370/PDS_GENBARCODE`, pdsToGen,
                    async (success) => {
                        xSplash.hide();
                        await xSwal.success(success.response, success.message);
                        //console.log(result);
                        if (_xLib.GetCookie('isDev') == "1") {
                            window.open(
                                _REPORTINGSERVER_ + '%2fKB3%2fKBNOR700PDS_X2&rs:Command=Render'
                                + '&pUserCode=' + ajexHeader.UserCode
                                + '&OrderNo=' + data[0].F_OrderNO.replace(/_N|_U/g, "")
                                + '&OrderNoTo=' + data[data.length - 1].F_OrderNO.replace(/_N|_U/g, "")
                                + '&DeliveryDate=' + ''
                                + '&DeliveryDateTo=' + ''
                                + '&F_Plant=' + ajexHeader.Plant
                                , '_blank'
                            );
                        }
                        else {
                            window.open(
                                _REPORTINGSERVER_ + '%2fKB3%2fKBNOR700PDS&rs:Command=Render'
                                + '&pUserCode=' + ajexHeader.UserCode
                                + '&OrderNo=' + data[0].F_OrderNO.replace(/_N|_U/g, "")
                                + '&OrderNoTo=' + data[data.length - 1].F_OrderNO.replace(/_N|_U/g, "")
                                + '&DeliveryDate=' + ''
                                + '&DeliveryDateTo=' + ''
                                + '&F_Plant=' + ajexHeader.Plant
                                , '_blank'
                            );
                        }
                    })

            },
            async (error) => {
                xSwal.xError(error);
            }
        );

    });



    xAjax.onClick('#btnPrintKanban', async function () {

        let PDSNoFrom = $("#itmPDS").val().trim().replace(/_N|_U/g, "")
        let PDSNoTo = $("#itmPDSTo").val().trim().replace(/_N|_U/g, "")

        if (PDSNoFrom == "" || PDSNoTo == "") {
            xSwal.error("Error!!!", "Please select PDS No.");
            return;
        }

        let isDeleted = await DeleteKBNOR_140_KB();
        console.log(isDeleted);
        if (isDeleted !== true) {
            return xSwal.error("Can't Delete Previous Data");
        }

        let _pdsAll = '';

        //for (let j = 0; j < 2; j++) {
        xSplash.show("Preparing Data");

        var _dtKanban = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR700_KANBAN]",
                "@pUserCode": ajexHeader.UserCode,
                "@pPlant": ajexHeader.Plant,
                "@pDeliveryDate": xDate.Date('yyyyMMdd', 'MM=-3'),
                "@F_orderType": null,
                //"@F_orderType": j == 0 ? "N" : "U",
                "@F_OrderNo": PDSNoFrom,
                "@F_OrderNoTo": PDSNoTo
            },
        });

        console.log(_dtKanban);

        if (_dtKanban.rows != null)
        {
            for (var i = 0; i < _dtKanban.rows.length; i++)
            {
                xSplash.show("Preparing Report");
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
                        "@F_Remark_KB": Trim(_dtKanban.rows[i].F_Remark_KB),
                        "@pMax": _ceil,
                        "@pBoxQty": (_amt > 0 ? _amt : '')
                    },
                });

                for (var k = 0; k < _dt.rows.length; k++)
                {
                    let row = _dt.rows[k];
                    let _OrderType = Trim(row.F_Order_Type);
                    _OrderType = (_OrderType == 'N' ? 'NORMAL' : (_OrderType == 'S' ? 'SPECIAL' : (_OrderType == 'U' ? 'URGENT' : '')));
                    let _Plant = Trim(row.F_Plant);
                    _Plant = (_Plant == '1' ? 'SAMRONG PLANT' : (_Plant == '2' ? 'BANGPEE PLANT' : (_Plant == 'B' ? 'SAMRONG WAREHOUSE' : 'BANGPRAKONG PLANT')));
                    let _DeliveryDate = Trim(row.F_Delivery_Date);
                    _DeliveryDate = _DeliveryDate.substring(6, 8) + '/' + _DeliveryDate.substring(4, 6) + '/' + _DeliveryDate.substring(0, 4);

                    let _pds = '';
                    _pds += '00' + Trim(row.F_PartNO) + '0|';
                    _pds += Trim(row.F_Supplier_INT) + '|';
                    _pds += Trim(row.F_Supplier_CD) + '|';
                    _pds += _DeliveryDate + '|';
                    _pds += Trim(row.F_Delivery_Trip) + '|';
                    _pds += Trim(row.F_Delivery_Time) + '|';
                    _pds += Trim(row.F_Delivery_Dock) + '|';
                    _pds += Trim(row.F_Dock_Code) + '|';
                    _pds += _Plant + '|';
                    _pds += Trim(row.F_Kanban_No) + '|';
                    _pds += Trim(row.F_Part_name) + '|';
                    _pds += Trim(row.F_OrderNO) + '|';
                    _pds += _OrderType + '|';
                    _pds += Trim(row.F_Box_Qty) + '|';
                    _pds += Trim(row.F_Page) + '/' + Trim(row.F_Page_Total) + '|';
                    _pds += Trim(row.F_Unit_Amount) + '|';
                    _pds += Trim(row.F_Remark) + '|';
                    _pdsAll += _pds + ',@@,';
                }
            }
        }
        //}

        xSplash.show("Generating QRCode");

        // ✅ Do this only once after all loops complete
        let isComplete = await xAjax.PostAsync({
            url: 'KBNOR700/PDS_GENQRCODE',
            data: {
                'PDSNO': _pdsAll
            },
            then: function (result) {
                xSplash.hide();
                return true;
            }
        });

        console.log(isComplete);

        if (isComplete) {
            xSplash.hide();
            let isA4 = $("#chkPrnitKanban").val() == "1";

            let baseParams = `pUserCode=${ajexHeader.UserCode}&OrderNo=${PDSNoFrom}&OrderNoTo=${PDSNoTo}` +
                `&DeliveryDate=${moment().add(-3, 'months').format('YYYYMMDD')}` +
                `&DeliveryDateTo=${moment().add(3, 'months').format('YYYYMMDD')}`;

            if (isA4) {
                return _xLib.OpenReport("/KBNOR700KANBANA4", baseParams);
            }
            else
            {
                return _xLib.OpenReport("/KBNOR700KANBAN", baseParams);
            }
        }


    });

});

async function ExGetPDS(){

    _xLib.AJAX_Get("/api/KBNOR370/GetPDS", null,
        async (success) => {
            $("#itmPDS").empty();
            $("#itmPDSTo").empty();
            $("#itmPDS").append("<option value='' hidden>-- Select PDS --</option>");
            $("#itmPDSTo").append("<option value='' hidden>-- Select PDS --</option>");
            success.data.forEach((e) => {
                $("#itmPDS").append(`<option value='${e.f_OrderNo}_${e.f_OrderType}'>${e.f_OrderNo}</option>`);
                $("#itmPDSTo").append(`<option value='${e.f_OrderNo}_${e.f_OrderType}'>${e.f_OrderNo}</option>`);
            });
            //$("#itmPDS").
            $(".selectpicker").selectpicker("refresh");
            xSplash.hide();
        }
    );

}



async function DeleteKBNOR_140_KB() {
    let isDel = false;
    await _xLib.AJAX_Post(`/xapi/DeleteKBNOR_140_KB`, '',
        function (success) {
            console.log(success);
            isDel = true;
        }
    );

    return isDel;
}