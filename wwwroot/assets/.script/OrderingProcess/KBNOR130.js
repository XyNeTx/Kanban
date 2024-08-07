$(document).ready(function () {


    const KBNOR130 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Type Import', 'Supplier Code', 'Short Name', 'Delivery Date', 'Delivery Trip', 'Order No'],
            "TH": ['Type Import', 'Supplier Code', 'Short Name', 'Delivery Date', 'Delivery Trip', 'Order No'],
            "JP": ['Type Import', 'Supplier Code', 'Short Name', 'Delivery Date', 'Delivery Trip', 'Order No'],
        },

        ColumnValue: [
            { "data": "F_Type_Import" },
            { "data": "F_Supplier" },
            { "data": "F_Short_name" },
            { "data": "F_Delivery_Date" },
            { "data": "F_Delivery_Trip" },
            { "data": "F_OrderNo" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });



    KBNOR130.prepare();


    KBNOR130.initial(function (result) {
        xSplash.hide();
        GetCheckSum();
        //console.log(ajexHeader);
    });



    let pdsno = '';
    let currentdate = replaceall(_PROCESSDATE_, '-', '');
    let issueddate = new Date(currentdate.substring(0, 4), currentdate.substring(4, 6) - 1, currentdate.substring(6));
    let facflag = (ajexHeader.Plant == 1 ? '9Y' : (ajexHeader.Plant == 3 ? '7Y' : ''));

    let value = [];
    let data = [];
    let barcode = '';

    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR100');
    });

    xAjax.onClick('btnGenerate', async function () {

        MsgBox("Do you want Issued PDS Normal Data?", MsgBoxStyle.OkCancel, async function () {

            xItem.progress({ id: 'prgProcess', current: 5, label: 'Start Process Normal : {{##.##}} %' });
  
            var _dtChk = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR130_CALCULATE]",
                    "@ProcessDate": "20240813",
                    "@ProcessShift": "D",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            xItem.progress({ id: 'prgProcess', current: 40, label: 'Geneate NORMAL PDS : {{##.##}} %' });

            var _dt = await xAjax.xExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR130_ShowResult]",
                    "OrderType": "N",
                    "Plant": ajexHeader.Plant,
                    "UserCode": ajexHeader.UserCode
                },
            });

            console.log(_dt);

            if (_dt.rows != null) xDataTable.bind('#tblMaster', _dt.rows);
            if (_dt.rows == null) MsgBox("ไม่พบข้อมูล PDS Normal Order", MsgBoxStyle.Information, "Generate Normal Data");

            $("#table-wrapper").css("visibility", "hidden");
            xItem.progress({ id: 'prgProcess', current: 100, label: 'Generate PDS Completed : {{##.##}} %' });

        })
    });

   







    //KBNOR130_02 = function () {
    //    xItem.progress({ id: 'prgProcess', current: 20, label: 'Delete TB_PDS_HEADER : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecuteJSON',
    //        data: {
    //            "Module": "[exec].[spKBNOR130_02]",
    //            "OrderType": "N",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('deletePDSHeader');
    //            if (result.response == 'OK') {
    //                KBNOR130_03();
    //            }

    //        }
    //    })
    //}


    //KBNOR130_03 = function () {
    //    xItem.progress({ id: 'prgProcess', current: 30, label: 'Insert TB_PDS_HEADER and TB_PDS_DETAIL : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'KBNOR130/KBNOR130_03',
    //        data: {
    //            "OrderType": "N",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('deletePDSHeader');
    //            if (result.response == 'OK') {
    //                KBNOR130_06();
    //            }

    //        }
    //    })
    //}

    //KBNOR130_06 = function () {
    //    xItem.progress({ id: 'prgProcess', current: 60, label: 'Process Special KPO : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'KBNOR130/KBNOR130_06',
    //        data: {
    //            "OrderType": "N",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('deletePDSHeader');
    //            if (result.response == 'OK') {
    //                updateRemark();
    //            }

    //        }
    //    })
    //}




    //xAjax.onClick('btnReport', async function () {
    //    //updateRemark();
    //    console.clear();

    //    //console.log('Start A');
    //    var _result = await xAjax.ExecuteJSON({
    //        data: {
    //            "Module": "[exec].[spTest]",
    //            "Plant": 3,
    //            "len": 1000,
    //        },
    //    });
    //    for (var i = 0; i < _result.data.length; i++) {
    //        _dtB = await xAjax.ExecuteJSON({
    //            data: {
    //                "Module": "[exec].[spTest]",
    //                "Plant": 3,
    //                "len": 5,
    //            },
    //        });

    //        //console.log('Sub Process [' + (i) + '] ');

    //        xItem.progress({ id: 'prgProcess', current: i, max: _result.data.length, label: 'Process A : {{##.##}} %' });
    //    }
    //    //console.log('End A');


    //    //console.log('Start B');
    //    _result = await xAjax.ExecuteJSON({
    //        data: {
    //            "Module": "[exec].[spTest]",
    //            "Plant": 3,
    //            "len": 1000,
    //        },
    //    });
    //    for (var i = 0; i < _result.data.length; i++) {
    //        xItem.progress({ id: 'prgProcess', current: i, max: _result.data.length, label: 'Process B : {{##.##}} %' });
    //        //console.log('item B [' + i + '] : ' + _result.data[i].F_PDS_No);
    //    }
    //    //console.log('End B');
    //});





    ////xAjax.onClick('btnReport', async function () {
    ////    //updateRemark();

    ////    console.log('Start A');
    ////    var _result = await getData(50000);
    ////    for (var i = 0; i < _result.data.length; i++) {
    ////        console.log('item A [' + i + '] : ' + _result.data[i].F_PDS_No);
    ////    }
    ////    console.log('End A');


    ////    console.log('Start B');
    ////    _result = await getData(10000);
    ////    for (var i = 0; i < _result.data.length; i++) {
    ////        console.log('item B [' + i + '] : ' + _result.data[i].F_PDS_No);
    ////    }
    ////    console.log('End B');
    ////});

    //PostAsync= function(pConfig = null) {
    //    if (pConfig != null) {
    //        //console.log(pConfig.data);

    //        let _url = (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + pConfig.url;
    //        let _postData = (pConfig.data != undefined ? ajaxPostData(pConfig.data) : null);

    //        //console.log(_postData);

    //        return $.ajax({
    //            type: "POST",
    //            contentType: "application/json; charset=utf-8",
    //            dataType: "json",
    //            headers: ajexHeader,
    //            url: '/EXEC/ExecuteJSON',
    //            data: _postData,
    //            success: function (result) {

    //                //console.log(result);

    //                if (result.response == 'OK') {
    //                    console.log('Process Complete');
    //                }

    //            }
    //        });

    //        //return $.ajax({
    //        //    type: "POST",
    //        //    contentType: "application/json; charset=utf-8",
    //        //    dataType: "json",
    //        //    headers: ajexHeader,
    //        //    url: _url,
    //        //    data: _postData,
    //        //    success: function (result) {
    //        //        console.log('PostAsync Complete');

    //        //    },
    //        //    error: function (result) {
    //        //        console.error('Ajax.Post: ' + result.responseText);
    //        //        xSplash.hide();
    //        //    }
    //        //});

    //    }
    //}


    //getData = function (p) {
    //    var data = {
    //        "Module": "[exec].[spTest]",
    //        "Plant": 3,
    //        "len": p,
    //    };


    //    let _postData = (data != undefined ? ajaxPostData(data) : null);
    //    return $.ajax({
    //        type: "POST",
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        headers: ajexHeader,
    //        url: '/EXEC/ExecuteJSON',
    //        data: _postData,
    //        success: function (result) {

    //            //console.log(result);

    //            if (result.response == 'OK') {
    //                console.log('Process Complete');
    //            }

    //        }
    //    });

    //}
})

