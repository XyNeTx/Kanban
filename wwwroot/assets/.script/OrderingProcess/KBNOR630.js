$(document).ready(function () {


    initial = async function () {

        var tblMaster = xDataTable.Initial({
            name: 'tblMaster',
            dom: '<"clear">',
            running: 0,
            orderNo: 0,
            columnTitle: {
                "EN": ['Part No', 'Kanban No.', 'Qty/Pack', 'Qty(PCS)'],
                "TH": ['Part No', 'Kanban No.', 'Qty/Pack', 'Qty(PCS)'],
                "JP": ['Part No', 'Kanban No.', 'Qty/Pack', 'Qty(PCS)'],
            },
            column: [
                { "data": "F_Part_No" },
                { "data": "F_Kanban_No" },
                { "data": "F_Box_Qty" },
                { "data": "F_Unit_Amount" }
            ],
            addnew: false,
            rowclick: (row) => {
            },
            then: function (config) {
                //xSplash.hide();
            }
        });

        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR630]",
                "pUserCode": ajexHeader.UserCode,
                "F_Plant": ajexHeader.Plant,
                "F_ORDERTYPE": "U"
            },
        });

        if (_dt.rows != null) xDropDownList.bind('#ddlPDSNo', _dt.rows, 'F_OrderNo', 'F_OrderNo');

        //console.log(ddlPDSNo);
        $("#ddlPDSNo").selectpicker();
        xSplash.hide();
    }
    initial();



    xAjax.onClick('btnExit', function () {
        ////ddlPDSNo.title = '1A24022701023';
        //ddlPDSNo.value = '1A24022701023';
        ////ddlPDSNo.value = '1D24022801C02';
        ////ddlPDSNo.value = '1A24021701001';

        xAjax.redirect('KBNOR600');
    });


    xAjax.onClick('btnSearch', async function () {
        try {
            btnDelete.readonly = false;

            if (ddlPDSNo.value == null || ddlPDSNo.value == undefined) {
                MsgBox(`Please input PDS No. Before click Search`, MsgBoxStyle.Critical);
                return false;
            }

            //### 'Check ไม่สามารถ Cancel PDS นี้ได้ เพราะมีบาง Part No. ที่รับ Part ไปแล้ว
            var _dt = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR630_S1]",
                    "F_OrderNo": ddlPDSNo.value
                },
            });
            if (_dt.rows != null) {
                MsgBox(`ไม่สามารถ Delete PDS นี้ได้ เพราะมีบาง Part No. ที่รับ Part ไปแล้ว!!!`, MsgBoxStyle.Critical);
                btnDelete.readonly = true;
                return false;
            }


            //### 'Check PDS=Cancel Last Time
            var _dt = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR630_S2]",
                    "F_OrderNo": ddlPDSNo.value,
                    "F_Plant": ajexHeader.Plant,
                    "F_ORDERTYPE": "U"
                },
            });
            if (_dt.rows != null) MsgBox(`ไม่สามารถ Delete PDS นี้ได้ เพราะ PDS นี้ถูก Delete ไปแล้ว!!!`, MsgBoxStyle.Critical);




            var _dtChk = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR630_S3]",
                    "F_OrderNo": ddlPDSNo.value,
                    "F_Plant": ajexHeader.Plant,
                    "F_ORDERTYPE": "U"
                },
            });
            console.log(_dtChk.rows[0].F_Delivery_Date);
            if (_dtChk.rows != null) {
                txtSupplierCode.value = _dtChk.rows[0].F_Supplier_Code + '-' + _dtChk.rows[0].F_Supplier_Plant;
                txtSupplierName.value = _dtChk.rows[0].F_name;
                txtDeliveryDate.value = _dtChk.rows[0].F_Delivery_Date;
                txtDeliveryTrip.value = _dtChk.rows[0].F_Delivery_Trip;
                txtStoreCode.value = _dtChk.rows[0].F_Delivery_Dock;
                txtStatus.value = (_dtChk.rows[0].F_Status == 'N' ? 'NORMAL - Can Delete' : (_dtChk.rows[0].F_Status == 'D' ? 'DELETED' : ''));

                xDataTable.bind('#tblMaster', _dtChk.rows);

                btnDelete.readonly = false;
                if (_dtChk.rows[0].F_Status == 'D') btnDelete.readonly = true;
            }
            if (_dtChk.rows == null) MsgBox(`Not Found data...`, MsgBoxStyle.Information);

            xSplash.hide();


        } catch (error) {       // Code to handle the error
            xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });

            MsgBox("ERROR : Can not search data.", MsgBoxStyle.Critical);

        }
    });


    xAjax.onClick('btnDelete', async function () {
        try {
            MsgBox(`Do you want Delete URGENT PDS Data?`, MsgBoxStyle.OkCancel, async function () {

                xItem.progress({ id: 'prgProcess', current: 5, label: 'Processing Please wait : {{##.##}} %' });

                //'TB_REC_HEADER  SET F_Status ='D'
                console.log(txtDeliveryDate.value)
                await xAjax.Execute({
                    data: {
                        "Module": "[exec].[spKBNOR630_CANCEL]",
                        "@pUserCode": ajexHeader.UserCode,
                        "@F_Plant": ajexHeader.Plant,
                        "@F_OrderNo": ddlPDSNo.value,
                        "@F_ORDERTYPE": 'U',
                        "@F_Delivery_Date": ReplaceAll(txtDeliveryDate.value,'-',''),
                        "@F_Supplier_Code": txtSupplierCode.value.substring(0, 4),
                        "@F_Supplier_Plant": txtSupplierCode.value.substring(5),
                        "@F_Store_CD": txtStoreCode.value
                    },
                });
                xItem.progress({ id: 'prgProcess', current: 100, label: 'Process Complete : {{##.##}} %' });

                txtStatus.value = 'DELETED';
                btnDelete.readonly = true;


                MsgBox("Delete Data Complete", MsgBoxStyle.Information);

            });


        } catch (error) {       // Code to handle the error
            xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });

            await xAjax.Execute({
                data: {
                    "Module": "[exec].[spKBNOR630_EXCEPTION]",
                    "@OrderType": "N",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            MsgBox("ERROR : Can not delete data.", MsgBoxStyle.Critical);

        }
    });

    if (ajexHeader.UserCode == "20234111") {
        $("#btnDeleteReq").parent().removeClass("d-none");
    }


})

$("#btnDeleteReq").click(async function () {

    let listObj = [
        { F_PDS_No: "3B25022601U07", F_Delivery_Date: moment("26/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022601U08", F_Delivery_Date: moment("26/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022602U02", F_Delivery_Date: moment("26/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022602U03", F_Delivery_Date: moment("26/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022603U01", F_Delivery_Date: moment("26/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022603U02", F_Delivery_Date: moment("26/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022604U01", F_Delivery_Date: moment("26/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022604U02", F_Delivery_Date: moment("26/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022605U01", F_Delivery_Date: moment("26/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022605U02", F_Delivery_Date: moment("26/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },

        { F_PDS_No: "3B25022701U08", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022701U12", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022702U02", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022702U03", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022703U02", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022703U03", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022704U01", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022704U02", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022705U01", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022705U02", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },

        { F_PDS_No: "3B25022706U01", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022706U02", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022707U01", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022707U02", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022708U01", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022708U02", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022709U01", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022709U02", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022710U01", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022710U02", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022711U01", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022711U02", F_Delivery_Date: moment("27/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },

        { F_PDS_No: "3B25022801U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022801U06", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022802U01", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022802U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022803U01", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022803U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022804U01", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022804U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022805U01", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022805U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022806U01", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022806U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022807U01", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022807U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022808U01", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022808U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022809U01", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022809U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022810U01", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022810U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" },
        { F_PDS_No: "3B25022811U01", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "1027-C" },
        { F_PDS_No: "3B25022811U02", F_Delivery_Date: moment("28/02/2025", "DD/MM/YYYY").format("DD/MM/YYYY"), F_Store_Code: "3B", F_Supplier_Code: "5369-C" }
    ];


    console.log(listObj);
    MsgBox(`Do you want Delete URGENT PDS Data?`, MsgBoxStyle.OkCancel, async function () {
        for (let i = 0; i < listObj.length; i++) {
            try {

                await xAjax.Execute({
                    data: {
                        "Module": "[exec].[spKBNOR630_CANCEL]",
                        "@pUserCode": ajexHeader.UserCode,
                        "@F_Plant": ajexHeader.Plant,
                        "@F_OrderNo": listObj[i].F_PDS_No,
                        "@F_ORDERTYPE": 'U',
                        "@F_Delivery_Date": listObj[i].F_Delivery_Date,
                        "@F_Supplier_Code": listObj[i].F_Supplier_Code.substring(0, 4),
                        "@F_Supplier_Plant": listObj[i].F_Supplier_Code.substring(5),
                        "@F_Store_CD": listObj[i].F_Store_Code,
                    },
                });

                MsgBox("Delete Data Complete", MsgBoxStyle.Information);


            } catch (error) {       // Code to handle the error
                xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });

                await xAjax.Execute({
                    data: {
                        "Module": "[exec].[spKBNOR630_EXCEPTION]",
                        "@OrderType": "N",
                        "@Plant": ajexHeader.Plant,
                        "@UserCode": ajexHeader.UserCode
                    },
                });
                MsgBox("ERROR : Can not delete data.", MsgBoxStyle.Critical);

            }
        }
    });
})

