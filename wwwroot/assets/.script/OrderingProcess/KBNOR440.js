$(document).ready(function () {


    const xKBNOR440 = new MasterTemplate({
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
        //processing :false,
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });



    xKBNOR440.prepare();


    xKBNOR440.initial(function (result) {
        xSplash.hide();
        GetCheckSum();
        //console.log(ajexHeader);
    });



    let pdsno = '';
    //let currentdate = replaceall(_PROCESSDATE_, '-', '');
    //let issueddate = new Date(currentdate.substring(0, 4), currentdate.substring(4, 6) - 1, currentdate.substring(6));
    let facflag = (ajexHeader.Plant == 1 ? '9Y' : (ajexHeader.Plant == 3 ? '7Y' : ''));

    let value = [];
    let data = [];
    let barcode = '';

    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR400');
    });

    xAjax.onClick('btnGenerate', async function () {

        MsgBox("Do you want Issued PDS Urgent Data?", MsgBoxStyle.OkCancel, async function () {

            xItem.progress({ id: 'prgProcess', current: 5, label: 'Start Process Urgent : {{##.##}} %' });
            //Start Process Special  '(9Y Delivery_Trip = 1 only)

            var _dtChk = await xAjax.xExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_CALCULATE]",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            xItem.progress({ id: 'prgProcess', current: 25, label: 'Delete TB_PDS_DETAIL : {{##.##}} %' });
/*            MsgBox("Process GEN PDS Data for Urgent Order Completed.", MsgBoxStyle.Information, "Process Complete");*/

            var _dt = await xAjax.xExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_ShowResult]",
                    "OrderType": "U",
                    "Plant": ajexHeader.Plant,
                    "UserCode": ajexHeader.UserCode
                },
            });

            console.log(_dt);

            if (_dt.rows != null) xDataTable.bind('#tblMaster', _dt.rows);
            if (_dt.rows == null) MsgBox("ไม่พบข้อมูล PDS Urgent Order", MsgBoxStyle.Information, "Interface Urgent Data");

            $("#table-wrapper").css("visibility", "hidden");


            xItem.progress({ id: 'prgProcess', current: 100, label: 'Generate PDS Completed : {{##.##}} %' });
            xSwal.success('Success', 'Generate PDS Completed');
        })
    });

    $("#btnReport").click(function () {
        var UserName = $(".pcoded-navigatio-lavel").text();
        console.log(UserName);
        _xLib.OpenReport('/KBNOR440', `UserCode=${ajexHeader.UserCode}&Plant=${ajexHeader.Plant}&UserName=${UserName}`);
    });

    GetCheckSum = function() {
        for (let i = 0; i < 10; i++) {
            value[i] = i;
            data[i] = i.toString();
        }
        for (let J = 10; J <= 35; J++) {
            value[J] = J;
            data[J] = String.fromCharCode(55 + J);
        }
        value[36] = 36; data[36] = "-";
        value[37] = 37; data[37] = ".";
        value[38] = 38; data[38] = " ";
        value[39] = 39; data[39] = "$";
        value[40] = 40; data[40] = "/";
        value[41] = 41; data[41] = "+";
        value[42] = 42; data[42] = "%";
    }

    CheckSum = function (pds_ = '') {
        let nNo = 0;
        for (let i = 0; i < pds_.length; i++) {
            let nCh = pds_.charAt(i);
            let nValue = 0;
            if (nCh <= "9") {
                nValue = value(nCh);
            } else {
                nValue = value(nCh.charCodeAt(0)) - 55;
            }
            nNo = nNo + nValue;
        }
        nNo = nNo % 43;
        return pds_ + data[nNo];
    }

    GetDate = async function (delivery_) {
        let Get_Date = "";
        let _Store_CD = '1A'; // Assuming this is a constant
        let Sql;

        var _dtChk = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR440_GetDate]",
                "@F_Store_CD": _Store_CD,
                "@F_YM": delivery_.substring(0, 6)
            },
        });

        if (_dtChk.rows != null) {
            let _MM = parseInt(sDelivery.substring(6, 8));
            if (_MM > 1) {
                for (let i = _MM - 1; i >= 1; i--) {
                    if (parseInt(_dtChk.rows[0][i * 2]) + parseInt(_dtChk.rows[0][i * 2 + 1]) === 2) {
                        return delivery_.substring(0, 6) + ('0' + i).slice(-2);
                        break;
                    }
                }
            }
        }


        let sYMLast = new Date(delivery_.substring(4, 6) + "/" + delivery_.substring(6, 8) + "/" + delivery_.substring(0, 4));
        sYMLast.setMonth(sYMLast.getMonth() - 1);
        let sYMLastStr = sYMLast.getFullYear().toString() + ('0' + (sYMLast.getMonth() + 1)).slice(-2);

        _dtChk = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR440_GetDate]",
                "@F_Store_CD": _Store_CD,
                "@F_YM": delivery_.substring(0, 6)
            },
        });

        if (_dtChk.rows != null) {
            for (let i = 31; i >= 1; i--) {
                if (parseInt(_dtChk.Get_Date1[0][i * 2]) + parseInt(_dtChk.Get_Date1[0][i * 2 + 1]) === 2) {
                    return sYMLastStr + ('0' + i).slice(-2);
                    break;
                }
            }
        }


        return "";
    }
})

