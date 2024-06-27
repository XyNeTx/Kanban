$(document).ready(function () {

    const tblMaster = xDataTable.Initial({
        name: 'tblMaster',
        scrollbar: "300px",
        dom:'<"top"f>t<"clear">',
        columnTitle: {
            "EN": ['Supplier Code'],
            "TH": ['Supplier Code'],
            "JP": ['Supplier Code'],
        },
        column: [
            { "data": "F_Supplier" }
        ],
        addnew: false,
        then: function (config) {
        },
        rowclick: function (data) {
            //console.log(data);
            //xSplash.show();

            xAjax.Post({
                url: 'KBNMS009/search',
                data: {
                    "F_Plant": $('#frmCondition #F_Plant').val(),
                    "SupplierCode": data.F_Supplier,
                    "UID": _UID_
                },
                then: function (result) {
                    //console.log(result);
                    xDataTable.Bind('tblDetail', result.data);

                    //xSplash.hide();
                }
            })        
        }
    });

    const tblDetail = xDataTable.Initial({
        name: 'tblDetail',
        scrollbar: "300px",
        dom: '<"top"f>t<"clear">',
        //checking: 0,
        //running: 0,
        columnTitle: {
            "EN": ['Supplier Code', 'Supplier Plant', 'Kanban No', 'Dock', 'Number'],
            "TH": ['Supplier Code', 'Supplier Plant', 'Kanban No', 'Dock', 'Number'],
            "JP": ['Supplier Code', 'Supplier Plant', 'Kanban No', 'Dock', 'Number'],
        },
        column: [
            { "data": "F_Supplier_Code" },
            { "data": "F_Supplier_Plant" },
            { "data": "F_Kanban_No" },
            { "data": "F_Supply_Code" },
            { "data": "F_Number" }
        ],
        //order: 0,
        addnew: false, 
        then: function (config) {
        },
        rowclick: function (data, r) {
            //console.log(row);
            xSwal.input('Please enter number'
                , function (result) {
                    data.F_Number = (result.value == '' ? '0' : result.value);

                    xDataTable.Cell('tblDetail', r, 4, data.F_Number);
                }
                , (data.F_Number == '0' ? '' : data.F_Number)
            );

        }
    });


    initial = function () {
        xAjax.Post({
            url: 'KBNMS009/initial',
            callback: function (result) {
                //console.log(result);
                xDropDownList.Bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

                xDataTable.Bind('tblMaster', result.data.PPM_T_Construction);

                xSplash.hide();
            }
        })        
    }
    initial();



    xAjax.onClick('#btnClearData', async function () {
        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNMS009_ClearData]",
                "pPlant": ajexHeader.Plant,
                "F_Update_By": ajexHeader.UserCode
            },
        });

        xDataTable.Bind('tblDetail', []);
        xSplash.hide();
    })



    xAjax.onClick('#btnSave', async function () {
        try {


            if (tblDetail.property.rows == 0) {
                MsgBox("Please Select Supplier Code Before Process Data", MsgBoxStyle.Critical, "DATA ERROR");
                return false;
            }

            for (var i = 0; i < tblDetail.property.rows; i++) {

                var _dt = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNMS009_SAVE]",
                        "pPlant": ajexHeader.Plant,
                        "F_Update_By": ajexHeader.UserCode,
                        "F_Supplier_Code": tblDetail.property.data[i].F_Supplier_Code,
                        "F_Supplier_Plant": tblDetail.property.data[i].F_Supplier_Plant,
                        "F_Store_Code": tblDetail.property.data[i].F_Store_Code,
                        "F_Kanban_No": tblDetail.property.data[i].F_Kanban_No,
                        "F_Supply_Code": tblDetail.property.data[i].F_Supply_Code,
                        "F_Number": tblDetail.property.data[i].F_Number
                    },
                });

                //console.log(_dt);
            }

            Process_Data();

        } catch (error) {
            // Code to handle the error
            MsgBox("Error Sub : Btn_Save_Click.", MsgBoxStyle.Critical, "Error");
        }
    })



    Process_Data = async function () {
        try {

            var _DT1 = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNMS009_P1]",
                    "pPlant": ajexHeader.Plant,
                    "F_Update_By": ajexHeader.UserCode
                },
            });

            if (_DT1.rows != null) {

                for (var i = 0; i < _DT1.rows.length; i++) {
                    intNum1 = _DT1.rows[i].F_Number;

                    var _DT2 = await xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNMS009_P2]",
                            "pPlant": ajexHeader.Plant,
                            "F_Update_By": ajexHeader.UserCode,
                            "F_Supplier_Code": tblDetail.property.data[i].F_Supplier_Code,
                            "F_Supplier_Plant": tblDetail.property.data[i].F_Supplier_Plant,
                            "F_Store_Code": tblDetail.property.data[i].F_Store_Code,
                            "F_Kanban_No": tblDetail.property.data[i].F_Kanban_No,
                            "F_Supply_Code": tblDetail.property.data[i].F_Supply_Code,
                            "F_Part_No": tblDetail.property.data[i].F_Part_No,
                            "F_Ruibetsu": tblDetail.property.data[i].F_Ruibetsu
                        },
                    });


                    if (_DT2.rows != null) {
                        intNum2 = _DT2.rows[0].F_Number;


                        if (intNum1 > intNum2) {

                        }


                    }


                }














            }






            MsgBox("Save Data..Process Complete", MsgBoxStyle.Information);
        } catch (error) {
            // Code to handle the error
            MsgBox("Error Sub : Btn_Save_Click.", MsgBoxStyle.Critical, "Error");
        }
    }






})

