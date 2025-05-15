$(document).ready(function () {



    const KBNOR360 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['PDS No', 'Supplier', 'Delivery Date', 'Delivery Trip', 'Register', 'Picking'],
            "TH": ['PDS No', 'Supplier', 'Delivery Date', 'Delivery Trip', 'Register', 'Picking'],
            "JP": ['PDS No', 'Supplier', 'Delivery Date', 'Delivery Trip', 'Register', 'Picking'],
        },

        ColumnValue: [
            { "data": "F_OrderNo" },
            { "data": "SupplierCode" },
            { "data": "DeliveryDate" },
            { "data": "F_Delivery_Trip" },
            { "data": "Flg_Register" },
            { "data": "Flg_Picking" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        addnew: false,
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });



    KBNOR360.prepare();




    KBNOR360.initial(function (result) {
        KBNOR360.search();
        xSplash.hide();
    });


    xAjax.onClick('btnExit', async function () {
        //console.log(ReplaceAll(ajexHeader.ProcessDate, '-', ''));
        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR360_EXIT]",
                "@pUserCode": ajexHeader.UserCode,
                "@F_Issued_Date": ReplaceAll(ajexHeader.ProcessDate, '-', ''),
                "@F_Issued_Shift": (ajexHeader.Shift == 1 ? 'D' : 'N'),
            },
        });
        if (_dt.rows == null) xAjax.redirect('KBNOR300');
        if (_dt.rows != null)
            MsgBox("Generate Picking ไม่สำเร็จ ต้องการกด Generate Picking อีกครั้งหรือไม่?"
                , MsgBoxStyle.OkCancel
                , async function () {
                    console.log('Ok')
                }
                , async function () {
                    xAjax.redirect('KBNOR300');
                });


    });


    xAjax.onClick('btnClear', async function () {
        xAjax.redirect('KBNOR361');
    });


    xAjax.onClick('btnRegister', async function () {
        try {

            MsgBox("Do you want Registration Data for CKD Warehouse Order?", MsgBoxStyle.OkCancel, async function () {

                var _data = xDataTable.Selected('#tblMaster');

                if (_data.length == 0) return false;

                xItem.progress({ id: 'prgProcess', current: 0, label: 'Start Calculate Order of Urgent : {{##.##}} %' });
                for (var i = 0; i < _data.length; i++) {
                    var _p = ((i + 1) / _data.length) * 20;
                    //console.log(_p)
                    await xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR360_REGIS01]",
                            "@pUserCode": ajexHeader.UserCode,
                            "@F_Plant": ajexHeader.Plant,
                            "@F_OrderType": "N",
                            "@F_OrderNo": _data[i].F_OrderNo,
                            "@F_Supplier_Code": _data[i].F_Supplier_Code,
                            "@F_Supplier_Plant": _data[i].F_Supplier_Plant,
                            "@F_Delivery_Date": _data[i].F_Delivery_Date,
                            "@F_Delivery_Trip": _data[i].F_Delivery_Trip
                        },
                    });
                    xItem.progress({ id: 'prgProcess', current: _p, label: 'Insert TB_REC_HEADER & TB_REC_HEADER : {{##.##}} %' });


                    //''Update Flag_Lock Data & Update Flag Lock Header
                    await xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR360_REGIS02]",
                            "@pUserCode": ajexHeader.UserCode,
                            "@F_Plant": ajexHeader.Plant,
                            "@F_Supplier_Code": _data[i].F_Supplier_Code,
                            "@F_Supplier_Plant": _data[i].F_Supplier_Plant,
                            "@F_Delivery_Date": _data[i].F_Delivery_Date,
                            "@F_Delivery_Round": _data[i].F_Delivery_Trip,
                            "@F_Process_Date": ReplaceAll(ajexHeader.ProcessDate, '-', ''),
                            "@F_Process_SHift": (ajexHeader.Shift == 1 ? `'D'` : `'N','T'`),
                        },
                    });
                    xItem.progress({ id: 'prgProcess', current: _p + 5, label: 'Update Flag Lock Data & Header : {{##.##}} %' });
                }

                //''Update Case TYpe Version
                await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR360_REGIS03]",
                        "@pUserCode": ajexHeader.UserCode,
                        "@F_Plant": ajexHeader.Plant,
                        "@F_OrderType": "N",
                        "@F_Delivery_Date": ReplaceAll(ajexHeader.ProcessDate, '-', ''),
                        "@F_Process_SHift": (ajexHeader.Shift == 1 ? 'D' : 'N'),
                    },
                });
                xItem.progress({ id: 'prgProcess', current: 30, label: 'Update Case Type Version : {{##.##}} %' });


                //''=== Update Status Kanban
                //'' Update Kanban Cut & Update Kanban Add & Update Kanban Stop & Update Kanban Change Box
                await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR360_REGIS04]",
                        "@pUserCode": ajexHeader.UserCode,
                        "@F_Plant": ajexHeader.Plant,
                        "@F_OrderType": "N",
                        "@F_Process_Date": ReplaceAll(ajexHeader.ProcessDate, '-', ''),
                        "@F_Process_SHift": (ajexHeader.Shift == 1 ? 'D' : 'N'),
                    },
                });
                xItem.progress({ id: 'prgProcess', current: 40, label: 'Update Status Kanban(Add, Cut, Stop, Change Box) : {{##.##}} %' });



                //''Update Flag Lock for Case Not Calculate 
                await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR360_REGIS05]",
                        "@pUserCode": ajexHeader.UserCode,
                        "@F_Process_Date": ReplaceAll(ajexHeader.ProcessDate, '-', ''),
                        "@F_Process_SHift": (ajexHeader.Shift == 1 ? 'D' : 'N'),
                    },
                });
                xItem.progress({ id: 'prgProcess', current: 50, label: 'Update Flag Lock for Case Not Calculate : {{##.##}} %' });


                if (ajexHeader.Shift == 1) {
                    await xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR360_REGIS06]",
                            "@pUserCode": ajexHeader.UserCode,
                            "@F_Process_Date": ReplaceAll(ajexHeader.ProcessDate, '-', ''),
                            "@F_Process_SHift": (ajexHeader.Shift == 1 ? 'D' : 'N'),
                        },
                    });
                    xItem.progress({ id: 'prgProcess', current: 60, label: 'Update Flag Lock for Case Not Calculate (Day Shift) : {{##.##}} %' });

                }



                //'*****************5.DELETE DATA TO PDS Header, Detail*****************************
                for (var i = 0; i < _data.length; i++) {
                    var _p = ((i + 1) / _data.length) * 20;

                    await xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR360_REGIS_D]",
                            "@pUserCode": ajexHeader.UserCode,
                            "@F_Plant": ajexHeader.Plant,
                            "@F_OrderType": "N",
                            "@F_OrderNo": _data[i].F_OrderNo,
                            "@F_Supplier_Code": _data[i].F_Supplier_Code,
                            "@F_Supplier_Plant": _data[i].F_Supplier_Plant,
                            "@F_Delivery_Date": _data[i].F_Delivery_Date,
                            "@F_Delivery_Round": _data[i].F_Delivery_Trip,
                        },
                    });

                    xItem.progress({ id: 'prgProcess', current: _p + 80, label: 'Delete PDS Header & Detail Data : {{##.##}} %' });

                }


                var _dt = await xAjax.xExecuteJSON({
                    data: {
                        "Module": "[exec].[spMSParameter] '"
                            + ajexHeader.UserCode + "' "
                            + ", 'ST_CKD', @pValue2='5' "
                            + ", @ErrorMessage='' "
                    },
                });

                //console.log(_dt);
                xItem.progress({ id: 'prgProcess', current: 100, label: 'Register Complete : {{##.##}} %' });
                
            })

        } catch (error) {
            // Code to handle the error
            for (var i = 0; i < _data.length; i++) {

                await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR360_EXCEPTION]",
                        "@pUserCode": ajexHeader.UserCode,
                        "@F_Plant": ajexHeader.Plant,
                        "@F_OrderType": "N",
                        "@F_OrderNo": _data[i].F_OrderNo,
                        "@F_Supplier_Code": _data[i].F_Supplier_Code,
                        "@F_Supplier_Plant": _data[i].F_Supplier_Plant,
                        "@F_Delivery_Date": _data[i].F_Delivery_Date,
                        "@F_Delivery_Round": _data[i].F_Delivery_Trip,
                        "@F_Process_Date": ReplaceAll(ajexHeader.ProcessDate, '-', ''),
                        "@F_Process_SHift": (ajexHeader.Shift == 1 ? 'D' : 'N'),
                    },
                });

            }

            xItem.progress({ id: 'prgProcess', current: 0, label: 'Register NOT Complete : {{##.##}} %' });

            MsgBox("ERROR Register NOT Complete.", MsgBoxStyle.Critical, "Register Data Error");
        }
    });




    xAjax.onClick('btnGenerate', async function () {
        try {
            console.log('btnGenerate');
            MsgBox("Do you want Generate Picking?", MsgBoxStyle.OkCancel, async function () {

                //' Check PDS to Generate CKD Picking Only Normal Order



            });


        } catch (error) {
            // Code to handle the error
            for (var i = 0; i < _data.length; i++) {

                await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR360_EXCEPTION]",
                        "@pUserCode": ajexHeader.UserCode,
                        "@F_Plant": ajexHeader.Plant,
                        "@F_OrderType": "N",
                        "@F_OrderNo": _data[i].F_OrderNo,
                        "@F_Supplier_Code": _data[i].F_Supplier_Code,
                        "@F_Supplier_Plant": _data[i].F_Supplier_Plant,
                        "@F_Delivery_Date": _data[i].F_Delivery_Date,
                        "@F_Delivery_Round": _data[i].F_Delivery_Trip,
                        "@F_Process_Date": ReplaceAll(ajexHeader.ProcessDate, '-', ''),
                        "@F_Process_SHift": (ajexHeader.Shift == 1 ? 'D' : 'N'),
                    },
                });

            }

            xItem.progress({ id: 'prgProcess', current: 0, label: 'Register NOT Complete : {{##.##}} %' });

            MsgBox("ERROR Register NOT Complete.", MsgBoxStyle.Critical, "Register Data Error");
        }
    });


})

