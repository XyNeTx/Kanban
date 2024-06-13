$(document).ready(async function () {



    const xKBNOR310 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Import TYpe', 'Part No', 'Refer PDS/PO No.', 'Supplier', 'Delivery Date', 'Kanban No', 'Qty', 'Qty/Pack'],
            "TH": ['Import TYpe', 'Part No', 'Refer PDS/PO No.', 'Supplier', 'Delivery Date', 'Kanban No', 'Qty', 'Qty/Pack'],
            "JP": ['Import TYpe', 'Part No', 'Refer PDS/PO No.', 'Supplier', 'Delivery Date', 'Kanban No', 'Qty', 'Qty/Pack'],
        },

        ColumnValue: [
            { "data": "F_Type" },
            { "data": "F_Part_No" },
            { "data": "F_PDS_No" },
            { "data": "F_Supplier_CD" },
            { "data": "F_Delivery_Date" },
            { "data": "F_Kanban_No" },
            { "data": "F_Qty" },
            { "data": "F_Qty_Pack" },
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'ProcessDate', value: ajexHeader.ProcessDate },
            { name: 'ProcessShift', value: (ajexHeader.Shift == 1 ? 'D' : 'N') }
        ],
    });



    xKBNOR310.prepare();




    xKBNOR310.initial(function (result) {

        $('#btnInterface').attr('readonly', true);
        $('#btnReport').attr('readonly', true);
        $('#btnExit').attr('readonly', true);

        xSplash.hide(async function () {
            try {

                await xDataTable.LoadJSON('#tblMaster', {
                    "Module": "[exec].[spKBNOR310]",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode,
                    "@ProcessDate": ajexHeader.ProcessDate.trim().replaceAll('-', ''),
                    "@ProcessShift": (ajexHeader.Shift == 1 ? 'D' : 'N')
                });

                $('#btnInterface').removeAttr('readonly');
                $('#btnReport').removeAttr('readonly');
                $('#btnExit').removeAttr('readonly');

            } catch (error) {

                MsgBox("Error Sub : Migrated ProductionDataToTest", MsgBoxStyle.Critical, "Error")

            }
        });



    });


    xAjax.onClick('#btnExit', function () {
        xAjax.redirect('KBNOR300');
    });


    xAjax.onClick('#btnReport', async function () {

        //$('#table-wrapper').height($('#tblMaster_wrapper').height());
    });


    xAjax.onClick('#btnInterface', async function () {

        try {
            xItem.progress({ id: 'prgProcess', current: 0, label: 'Start Processing... : {{##.##}} %' });


            var _dt = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR310_S]",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode,
                    "@ProcessDate": ajexHeader.ProcessDate.trim().replaceAll('-', ''),
                    "@ProcessShift": (ajexHeader.Shift == 1 ? 'D' : 'N')
                },
            });

            //console.log(_dt.rows[0].Last_Order);

            if (_dt.rows != null) {
                if (_dt.rows[0].Last_Order < ajexHeader.ProcessDate.trim().replaceAll('-', '') + (ajexHeader.Shift == 1 ? 'D' : 'N')
                    && _dt.rows[0].Step_Order == 0) {

                    xItem.progress({ id: 'prgProcess', current: 40, label: 'Processing Please wait : {{##.##}} %' });

                    await MsgBox("Do you want Interface Data from Import Data?",
                        MsgBoxStyle.OkCancel,
                        async function () {
                            //console.log('Do you want Interface Data from Import Data?')


                            await xAjax.xExecute({
                                data: {
                                    "Module": "Exec CKD_Inhouse.SP_INF_CKDORDER '"
                                        + ajexHeader.ProcessDate.trim().replaceAll('-', '') + "','"
                                        + ajexHeader.Shift + "'"
                                },
                            });
                            xItem.progress({ id: 'prgProcess', current: 30, label: 'Processing Please wait : {{##.##}} %' });

                        });


                    xItem.progress({ id: 'prgProcess', current: 100, label: 'Process Complete... : {{##.##}} %' });

                    MsgBox("Process Complete..", MsgBoxStyle.Information, "Information")
                } else {
                    MsgBox("Already Interfaced Order", MsgBoxStyle.Exclamation, "Information")

                }


            }


        } catch (error) {

            MsgBox("Error Sub : Cmd_Interface_Click", MsgBoxStyle.Critical, "Error")

        }
    });






})

