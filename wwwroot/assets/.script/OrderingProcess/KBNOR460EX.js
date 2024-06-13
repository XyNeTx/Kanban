$(document).ready(function () {


    const xKBNOR460EX = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Plant', 'Parent Part', 'Ruibetsu', 'Effective Date', 'End Date'],
            "TH": ['Plant', 'Parent Part', 'Ruibetsu', 'Effective Date', 'End Date'],
            "JP": ['Plant', 'Parent Part', 'Ruibetsu', 'Effective Date', 'End Date'],
        },
        ColumnValue: [
            { "data": "F_Plant" },
            { "data": "F_Parent_Part" },
            { "data": "F_Part_Name" },
            { "data": "F_Effect_Date" },
            { "data": "F_End_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNOR460EX.prepare();

    xKBNOR460EX.initial(function (result) {
        //console.log(result);
        xDropDownList.bind('#frmCondition #itmPDSFrom', result.data.PDSNo, 'F_OrderNo', 'F_OrderNo');
        xDropDownList.bind('#frmCondition #itmPDSTo', result.data.PDSNo, 'F_OrderNo', 'F_OrderNo');
        xDropDownList.bind('#frmCondition #itmSupplierFrom', result.data.Supplier, 'Supplier_Code', 'Supplier_Code');
        xDropDownList.bind('#frmCondition #itmSupplierTo', result.data.Supplier, 'Supplier_Code', 'Supplier_Code');

        xAjax.onClick('#chkPDSNo', function () {
            if ($('#chkPDSNo').val() == 0) $('#fldPDSNo').prop('disabled', 'disabled');
            if ($('#chkPDSNo').val() == 1) $('#fldPDSNo').prop('disabled', false);
        });

        xAjax.onClick('#chkSupplierCode', function () {
            if ($('#chkSupplierCode').val() == 0) $('#fldSupplierCode').prop('disabled', 'disabled');
            if ($('#chkSupplierCode').val() == 1) $('#fldSupplierCode').prop('disabled', false);
        });

        xAjax.onClick('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });


        initial();

        xSplash.hide();
    });


    var _itmFileName = "";

    initial = async function () {
        xItem.progress({ id: 'prgProcess', current: 0, label: 'Status : {{##.##}} %' });

        _itmFileName = ajexHeader.UserCode + '_' + ReplaceAll(xDate.Date(), '-') + '_' + ReplaceAll(xDate.Time(), ':') + '.csv';

        $('#itmFileName').val(_itmFileName);
        //$('#itmFileName').removeAttr('readonly');
        $('#itmFileName').attr('readonly', true);

        xSplash.hide();
    }


    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR400');
    });


    xAjax.onClick('btnExportData', async function () {
        xItem.progress({ id: 'prgProcess', current: 0, label: 'Start process : {{##.##}} %' });

        MsgBox("Do you want to export order data?",
            MsgBoxStyle.OkCancel,
            async function () {


                xItem.progress({ id: 'prgProcess', current: 5, label: 'Loading data : {{##.##}} %' });
                var _dt = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR460EX]",
                        "@OrderType": "N",
                        "@Plant": ajexHeader.Plant,
                        "@UserCode": ajexHeader.UserCode,
                        "@itmPDSFrom": ($('#chkPDSNo').val() == 1 ? ($('#itmPDSFrom').val() != null ? $('#itmPDSFrom').val() : '') : ''),
                        "@itmPDSTo": ($('#chkPDSNo').val() == 1 ? ($('#itmPDSTo').val() != null ? $('#itmPDSTo').val() : '') : ''),
                        "@itmSupplierFrom": ($('#chkSupplierCode').val() == 1 ? ($('#itmSupplierFrom').val() != null ? $('#itmSupplierFrom').val() : '') : ''),
                        "@itmSupplierTo": ($('#chkSupplierCode').val() == 1 ? ($('#itmSupplierTo').val() != null ? $('#itmSupplierTo').val() : '') : ''),
                        "@itmDeliveryFrom": ($('#chkDeliveryDate').val() == 1 ? ($('#itmDeliveryFrom').val() != null ? $('#itmDeliveryFrom').val() : '') : ''),
                        "@itmDeliveryTo": ($('#chkDeliveryDate').val() == 1 ? ($('#itmDeliveryTo').val() != null ? $('#itmDeliveryTo').val() : '') : '')
                    },
                });
                xItem.progress({ id: 'prgProcess', current: 10, label: 'Loading data complete : {{##.##}} %' });

                var _header = `PDS No.,Supplier,Plant,Short Name,Name,IssueDate,DeliveryDate,Trip,Str,Flag,Cycle,No.,Part No.,`
                    + `Part Name,Q'Ty/Pack,Q'Ty Order Total,Price,%,P/O No.,Type Order,Remark1,Remark2,Remark3,Sebango,Location,Attn,KanbanID,Store No,`
                    + `DeliveryTime,Kanban Remark,Dockcode,Collect Date,Collect Time,Delivery By,Approval,Address,Type Version,PDS Backup,DeptCode,DAcctNo,Work Code,Sparetext1,Sparetext2,SpareNum1,SpareNum2
`;


                if (_dt.rows != null) {
                    for (var i = 0; i < _dt.rows.length; i++) {

                        var _percent = (((i + 1) / _dt.rows.length) * 80) + 10;
                        xItem.progress({ id: 'prgProcess', current: _percent, label: 'Exporting ' + (i + 1) + ' of ' + _dt.rows.length + ' records to file [' + $('#itmFileName').val() + '] : {{##.##}} %' });

                        _text = _dt.rows[i].F_OrderNo + `,` + `'`
                            + _dt.rows[i].F_Supplier_Code + `,`
                            + _dt.rows[i].F_Supplier_Plant + `,`
                            + _dt.rows[i].F_short_name + `,`
                            + _dt.rows[i].F_name + `,`
                            + _dt.rows[i].F_Issued_Date + `,`
                            + _dt.rows[i].F_Delivery_Date + `,`
                            + _dt.rows[i].F_Delivery_Trip + `,` + `,` + `,` + `'`
                            + _dt.rows[i].F_Delivery_Cycle + `,`
                            + _dt.rows[i].F_No + `,` + `'`
                            + _dt.rows[i].F_Part_No
                            + _dt.rows[i].F_Ruibetsu + `,`
                            + _dt.rows[i].F_Part_Name + `,`
                            + _dt.rows[i].F_Box_Qty + `,`
                            + _dt.rows[i].F_Unit_Amount + `,` + `0,`
                            + _dt.rows[i].F_Vat + `,`
                            + _dt.rows[i].F_PO_Customer + `,`
                            + _dt.rows[i].F_OrderType + `,`
                            + _dt.rows[i].F_Remark + `,,,,`
                            + _dt.rows[i].F_Plant + `,` + `,` + `'`
                            + _dt.rows[i].F_Kanban_No + `,`
                            + _dt.rows[i].F_Delivery_Dock + `,`
                            + `(` + _dt.rows[i].F_Delivery_Time + `),`
                            + _dt.rows[i].F_Inf_KB + `,`
                            + _dt.rows[i].F_Dock_Code + `,`
                            + _dt.rows[i].F_Collect_Date + `,`
                            + `(` + _dt.rows[i].F_Collect_Time + `),`
                            + _dt.rows[i].F_Transportor + `,,`
                            + _dt.rows[i].F_Address + `,T,P,0,0,,,,0,0`;

                        if (i == 0) _text = _header + _text;

                        var _result = await xAjax.PostAsync({
                            url: "KBNOR460EX/write",
                            data: {
                                "File": $('#itmFileName').val(),
                                "Text": _text
                            },
                        })

                    }

                    if (_result.response == 'OK') {
                        MsgBox("Export complete, Do you want to save file? (" + _itmFileName + ")",
                            MsgBoxStyle.OkCancel,
                            function () {
                                $('#aDownloadLink').attr('href', _STORAGESERVER_ + '/' + xDate.Date('yyyyMM') + `/` + _result.data);
                                document.getElementById('aDownloadLink').click();

                                initial();
                            }, function () {

                                initial();
                            });
                    }
                }

                xItem.progress({ id: 'prgProcess', current: 100, label: 'Export data completed : {{##.##}} %' });


            }, function () {
                initial();
            });
    });
})

