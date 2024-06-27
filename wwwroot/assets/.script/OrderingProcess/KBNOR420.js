$(document).ready(function () {



    const xKBNOR420 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Customer PO', 'Part No', 'Supplier', 'Short Name', 'Store Code', 'Kanban No.', 'Delivery Date', 'Delivery Trip', 'Qty', 'Qty KB', 'Import Type'],
            "TH": ['Customer PO', 'Part No', 'Supplier', 'Short Name', 'Store Code', 'Kanban No.', 'Delivery Date', 'Delivery Trip', 'Qty', 'Qty KB', 'Import Type'],
            "JP": ['Customer PO', 'Part No', 'Supplier', 'Short Name', 'Store Code', 'Kanban No.', 'Delivery Date', 'Delivery Trip', 'Qty', 'Qty KB', 'Import Type'],
        },

        ColumnValue: [
            { "data": "F_PDS_No" },
            { "data": "F_Part_No" },
            { "data": "F_Supplier_CD" },
            { "data": "F_Short_name" },
            { "data": "F_Store_CD" },
            { "data": "F_Kanban_No" },
            { "data": "F_Delivery_Date" },
            { "data": "F_Round" },
            { "data": "F_Qty" },
            { "data": "F_QTY_KB" },
            { "data": "F_OrderType" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });



    xKBNOR420.prepare();




    xKBNOR420.initial(function (result) {
        xSplash.hide();
        //xKBNOR420.search();
    });


    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR400');
    });


    xAjax.onClick('btnCalculate', async function () {
        try {

            MsgBox("Do you want Calculate Order for Urgent?", MsgBoxStyle.OkCancel, async function () {

                xItem.progress({ id: 'prgProcess', current: 5, label: 'Start Calculate Order of Urgent : {{##.##}} %' });
                //await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL01]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                await xAjax.xExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR420_CALCULATE]",                     
                        "@pPlant": ajexHeader.Plant,
                        "@pUserCode": ajexHeader.UserCode
                    },
                });
                xItem.progress({ id: 'prgProcess', current: 10, label: 'Delete TB_Delivery Urgent Data : {{##.##}} %' });

                //let _remark = '';

                //var _dtChk = await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL02]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //if (_dtChk.rows != null) {
                //    for (var i = 0; i < _dtChk.rows.length; i++) {

                //        var _delivery = GetDate(Trim(_dtChk.rows[i][0].ToString()));

                //        var _dt = await xAjax.ExecuteJSON({
                //            data: {
                //                "Module": "[exec].[spKBNOR420_CAL02_S]",
                //                "@pOrderType": "U",
                //                "@pPlant": ajexHeader.Plant,
                //                "@pUserCode": ajexHeader.UserCode,
                //                "@F_Delivery_Date": _dt.rows[i][0].ToString(),
                //                "@F_Delivery_Trip": _dt.rows[i][0].ToString(),
                //                "@F_Supplier_Cd": _dt.rows[i][0].ToString(),
                //                "@F_Supplier_Plant": _dt.rows[i][0].ToString(),
                //                "@F_Store_Cd": _dt.rows[i][0].ToString()
                //            },
                //        });

                //        if (_dt.rows != null) {
                //            for (var j = 0; j < _dt.rows.length; j++) {
                //                _remark = _remark + _dt.rows[j].F_Remark + ',';
                //            }
                //            _remark.substring(0, remark.length - 1);
                //        }

                //        //===Update Volume
                //        await xAjax.ExecuteJSON({
                //            data: {
                //                "Module": "[exec].[spKBNOR420_CAL02_U]",
                //                "@pOrderType": "U",
                //                "@pPlant": ajexHeader.Plant,
                //                "@pUserCode": ajexHeader.UserCode,
                //                "@F_Delivery_Date": _dt.rows[i][0].ToString(),
                //                "@F_Delivery_Trip": _dt.rows[i][0].ToString(),
                //                "@F_Supplier_Cd": _dt.rows[i][0].ToString(),
                //                "@F_Supplier_Plant": _dt.rows[i][0].ToString(),
                //                "@F_Store_Cd": _dt.rows[i][0].ToString(),
                //                "@pRemark": _remark
                //            },
                //        });



                //    }
                //    xItem.progress({ id: 'prgProcess', current: 20, label: 'Update Remark for KPO ONLY : {{##.##}} %' });

                //}



                ////'' Calculate in case KPO Data: Service part 
                //await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL03]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 25, label: 'Calculate in case KPO Data [Service part] : {{##.##}} %' });






                //await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL04]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 30, label: 'Calculate in case KPO Data [Service part-Tacoma Only] : {{##.##}} %' });





                //await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL05]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 40, label: 'Generate in case Remain of KPO Data [Urgent] : {{##.##}} %' });






                //await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL06]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 50, label: 'Incase V2V and Urgent Order : {{##.##}} %' });


                //await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL07]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 55, label: 'In case Direct Supply : {{##.##}} %' });




                //await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL08]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 60, label: 'Update F_Delivery_Time from TB_MS_DeliveryTime : {{##.##}} %' });





                //await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL09]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 65, label: 'Update Cycle Time : {{##.##}} %' });




                //var _dtChk = await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL10]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 70, label: 'Update F_Dock_Code from TB_Import_Delivery : {{##.##}} %' });

                //if (_dtChk.rows != null) {
                //    for (var i = 0; i < _dtChk.rows.length; i++) {
                //        var _dtPOM = await xAjax.Execute({
                //            data: {
                //                "Module": "[exec].[spKBNOR420_CAL10_S]",
                //                "@OrderType": "U",
                //                "@Plant": ajexHeader.Plant,
                //                "@UserCode": ajexHeader.UserCode,
                //                "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                //                "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                //                "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                //                "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip
                //            },
                //        });

                //        if (_dtPOM.rows != null) {
                //            await xAjax.Execute({
                //                data: {
                //                    "Module": "[exec].[spKBNOR420_CAL10_U]",
                //                    "@OrderType": "U",
                //                    "@Plant": ajexHeader.Plant,
                //                    "@UserCode": ajexHeader.UserCode,
                //                    "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                //                    "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                //                    "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                //                    "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                //                    "@F_Dock_Cd": _dtPOM.rows[0].F_Dock_Cd
                //                },
                //            });
                //        }
                //    }
                //}
                //xItem.progress({ id: 'prgProcess', current: 75, label: 'UPDATE TB_Delivery.F_Dock_Code Special : {{##.##}} %' });


                //let _Detail = "", _Key = "";

                //var _dtChk = await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL11]",
                //        "@pOrderType": "U",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //if (_dtChk.rows != null) {
                //    for (var i = 0; i < _dtChk.rows.length; i++) {


                //        if (i > 0 && _Key != _dtChk.rows[i].F_SUpplier_CD
                //            + _dtChk.rows[i].F_SUpplier_Plant
                //            + _dtChk.rows[i].F_Store_Cd
                //            + _dtChk.rows[i].F_Delivery_Date
                //            + _dtChk.rows[i].F_Delivery_Trip
                //        ) {
                //            _Detail = _Detail.substring(0, _Detail.length - 1);

                //            await xAjax.ExecuteJSON({
                //                data: {
                //                    "Module": "[exec].[spKBNOR420_CAL11_U]",
                //                    "@OrderType": "U",
                //                    "@Plant": ajexHeader.Plant,
                //                    "@UserCode": ajexHeader.UserCode,
                //                    "@F_Remark": _Detail,
                //                    "@pKey": _Key
                //                },
                //            });
                //            _Detail = '';
                //        }

                //        _Detail += _dtChk.rows[i].F_Remark + ",";
                //        _Key = _dtChk.rows[i].F_SUpplier_CD
                //            + _dtChk.rows[i].F_SUpplier_Plant
                //            + _dtChk.rows[i].F_Store_Cd
                //            + _dtChk.rows[i].F_Delivery_Date
                //            + _dtChk.rows[i].F_Delivery_Trip;
                //    }


                //    _Detail = _Detail.substring(0, _Detail.length - 1);
                //    await xAjax.ExecuteJSON({
                //        data: {
                //            "Module": "[exec].[spKBNOR420_CAL11_U]",
                //            "@OrderType": "U",
                //            "@Plant": ajexHeader.Plant,
                //            "@UserCode": ajexHeader.UserCode,
                //            "@F_Remark": _Detail,
                //            "@pKey": _Key
                //        },
                //    });
                //}
                //xItem.progress({ id: 'prgProcess', current: 80, label: 'Summary Remark for Case : {{##.##}} %' });




                ////''Clear Delivery IN case Qty = 0
                //await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL12]",
                //        "@OrderType": "U",
                //        "@Plant": ajexHeader.Plant,
                //        "@UserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 90, label: 'Clear Delivery : {{##.##}} %' });





                //var _dt = await xAjax.ExecuteJSON({
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL13]",
                //        "@pOrderType": "SRV",
                //        "@pPlant": ajexHeader.Plant,
                //        "@pUserCode": ajexHeader.UserCode
                //    },
                //});
                //if (_dt.rows != null) xDataTable.bind('#tblMaster', _dt.rows);
                ////console.log(_dt);

                xItem.progress({ id: 'prgProcess', current: 100, label: 'Process Complete... : {{##.##}} %' });

                MsgBox("Process Calculate Data for Urgent Order Completed.", MsgBoxStyle.Information, "Process Complete");


                //xItem.progress({ id: 'prgProcess', current: 0, label: 'Start Process Special : {{##.##}} %' });
                ////Start Process Special  '(9Y Delivery_Trip = 1 only)
                //xAjax.Post({
                //    url: 'EXEC/eExecute',
                //    data: {
                //        "Module": "[exec].[spKBNOR420_CAL01]",
                //        "OrderType": "U",
                //        "Plant": ajexHeader.Plant,
                //        "UserCode": ajexHeader.UserCode
                //    },
                //    then: function (result) {
                //        //console.log(result);
                //        xItem.progress({ id: 'prgProcess', current: 10, label: 'Delete TB_Delivery Urgent Data : {{##.##}} %' });
                //        if (result.response == 'OK') {
                //            checkRemark();
                //        }

                //    }
                //})
            })

        } catch (error) {
            // Code to handle the error
            await xAjax.Execute({
                data: {
                    "Module": "[exec].[spKBNOR420_EXCEPTION]",
                    "@OrderType": "U",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            MsgBox("ERROR Interface Data from Import Data.", MsgBoxStyle.Critical, "Interface Urgent Data Error");

            xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });


        }
    });

    //checkRemark = function () {
    //    xAjax.Post({
    //        url: 'EXEC/eExecuteJSON',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL02]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('checkRemark');
    //            if (result.response == 'OK') {
    //                updateRemark();
    //            }

    //        }
    //    })
    //}

    //updateRemark = function () {
    //    xItem.progress({ id: 'prgProcess', current: 20, label: 'Update Remark for KPO ONLY : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'KBNOR420/calculateData',
    //        data: {
    //            "OrderType": "U"
    //        },
    //        then: function (result) {
    //            //console.log('updateRemark');
    //            if (result.response == 'OK') {
    //                calServicePart()
    //            }

    //        }
    //    })
    //}

    //calServicePart = function () {
    //    xItem.progress({ id: 'prgProcess', current: 25, label: 'Calculate in case KPO Data [Service part] : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecute',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL03]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('calServicePart');
    //            if (result.response == 'OK') {
    //                calServicePartTacoma();
    //            }

    //        }
    //    })
    //}

    //calServicePartTacoma = function () {
    //    xItem.progress({ id: 'prgProcess', current: 30, label: 'Calculate in case KPO Data [Service part-Tacoma Only] : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecute',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL04]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('calServicePartTacoma');
    //            if (result.response == 'OK') {
    //                genRemain();
    //            }

    //        }
    //    })
    //}

    //genRemain = function () {
    //    xItem.progress({ id: 'prgProcess', current: 40, label: 'Generate in case Remain of KPO Data [Urgent] : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecute',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL05]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('genRemain');
    //            if (result.response == 'OK') {
    //                incaseV2V();
    //            }

    //        }
    //    })
    //}

    //incaseV2V = function () {
    //    xItem.progress({ id: 'prgProcess', current: 50, label: 'Incase V2V and Urgent Order : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecute',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL06]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('incaseV2V');
    //            if (result.response == 'OK') {
    //                incaseDirectSupply();
    //            }

    //        }
    //    })
    //}

    //incaseDirectSupply = function () {
    //    xItem.progress({ id: 'prgProcess', current: 55, label: 'In case Direct Supply : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecute',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL07]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('incaseDirectSupply');
    //            if (result.response == 'OK') {
    //                updateDeliveryTime();
    //            }

    //        }
    //    })
    //}

    //updateDeliveryTime = function () {
    //    xItem.progress({ id: 'prgProcess', current: 60, label: 'Update F_Delivery_Time from TB_MS_DeliveryTime : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecute',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL08]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('updateDeliveryTime');
    //            if (result.response == 'OK') {
    //                updateCycleTime();
    //            }

    //        }
    //    })
    //}

    //updateCycleTime = function () {
    //    xItem.progress({ id: 'prgProcess', current: 65, label: 'Update Cycle Time : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecute',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL09]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('updateCycleTime');
    //            if (result.response == 'OK') {
    //                updateDockCode();
    //            }

    //        }
    //    })
    //}

    //updateDockCode = function () {
    //    xItem.progress({ id: 'prgProcess', current: 70, label: 'Update F_Dock_Code from TB_Import_Delivery : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'KBNOR420/updateDockCode',
    //        data: {
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('updateDockCode');
    //            if (result.response == 'OK') {
    //                summaryRemark();
    //            }

    //        }
    //    })
    //}

    //summaryRemark = function () {
    //    xItem.progress({ id: 'prgProcess', current: 80, label: 'Summary Remark for Case : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'KBNOR420/summaryRemark',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL09]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('summaryRemark');
    //            if (result.response == 'OK') {
    //                clearDelivery();
    //            }

    //        }
    //    })
    //}

    //clearDelivery = function () {
    //    xItem.progress({ id: 'prgProcess', current: 90, label: 'Clear Delivery : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecute',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL12]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('clearDelivery');
    //            if (result.response == 'OK') {
    //                showReport();
    //            }

    //        }
    //    })
    //}

    //showReport = function () {
    //    xItem.progress({ id: 'prgProcess', current: 100, label: 'Process Complete : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecuteJSON',
    //        data: {
    //            "Module": "[exec].[spKBNOR420_CAL12]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('showReport');
    //            console.log(result);
    //            if (result.response == 'OK') {

    //            }

    //        }
    //    })
    //}



})

