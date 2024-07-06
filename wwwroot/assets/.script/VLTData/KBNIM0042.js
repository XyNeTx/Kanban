$(document).ready(async function () {

    await _xLib.AJAX_Get("/api/KBNIM0042/GetCustomerCode", '',
        function (success)
        {
            if(success.status == "200")
            {
                $("#F_Customer").empty();
                $("#F_Customer").append('<option value="" hidden></option>');
                success.data = JSON.parse(success.data);
                success.data = _xLib.TrimArrayJSON(success.data);
                success.data.forEach(function (item)
                {
                    $("#F_Customer").append('<option value="' + item.F_Customer_Cd + '">' + item.F_Customer_Cd + '</option>');
                });
            }
        }
        );

    xSplash.hide();
});

$("#F_Customer").change(function () {
    _xLib.AJAX_Get("/api/KBNIM0042/GetStartJigIn", { F_Customer_Cd: $("#F_Customer").val() },
        function (success)
        {
            if(success.status == "200")
            {
                for (var each in success.data)
                {
                    success.data[each] = JSON.parse(success.data[each]);
                    success.data[each] = _xLib.TrimArrayJSON(success.data[each]);
                }
                //console.log(success.data);
                var table = $("#tableManage tbody tr td");
                var tableCKD = $("#tableCKD tbody tr td");
                //console.log(tableCKD);
                //console.log(table[0]);
                //console.log(success.data["frame"][0].F_Seq_nO);
                $(table[0]).text(success.data["frame"][0].F_Seq_nO);
                $(table[1]).text(success.data["dedion"][0].F_Seq_nO);
                $(table[2]).text(success.data["tg"][0].F_Seq_nO);
                $(table[3]).text(success.data["rr"][0].F_Seq_nO);

                $(tableCKD[0]).text(success.data["frameCKD"][0].F_Seq_nO);
                $(tableCKD[1]).text(success.data["dedionCKD"][0].F_Seq_nO);
                $(tableCKD[2]).text(success.data["tgCKD"][0].F_Seq_nO);
                $(tableCKD[3]).text(success.data["rrCKD"][0].F_Seq_nO);
            }
        }
    );
});

$("#tableManage tbody tr td").on("change", "input", function () {
    var value = $(this).val();
    var inputIndex = $(this).closest("td").index();
    var table = $("#tableManage tbody tr td");
    var tableCKD = $("#tableCKD tbody tr td");
    var _StartJigFrame = $(table[0]).text();
    var _StartJigDedion = $(table[1]).text();
    var _StartJigTG = $(table[2]).text();
    var _StartJigRR = $(table[3]).text();

    if (!_StartJigFrame && inputIndex == 1) return;
    else if (!_StartJigDedion && inputIndex == 2) return;
    else if (!_StartJigTG && inputIndex == 3) return;
    else if (!_StartJigRR && inputIndex == 4) return;

    var F_Part_Type = $("#tableManage thead").find("th").eq(inputIndex).text();

    _xLib.AJAX_Get("/api/KBNIM0042/GetEndJigIn", { F_Customer_Cd: $("#F_Customer").val(), F_Part_Type: F_Part_Type, F_Remain_Unit: value },
        function (success) {
            if (success.status == "200") {
                for (var each in success.data) {
                    success.data[each] = JSON.parse(success.data[each]);
                    success.data[each] = _xLib.TrimArrayJSON(success.data[each]);
                }

                var _index = inputIndex + 7;
                var _indexCKD = inputIndex + 3;
                console.log(success.data);
                $(table[_index]).text(success.data["endSeq"][0].F_Seq_nO);
                $(tableCKD[_indexCKD]).text(success.data["endSeqCKD"][0].F_Seq_nO);
            }
        }
    );
});

$("#buttonConfirm").click(function () {
    var table = $("#tableManage tbody tr td");
    var tableCKD = $("#tableCKD tbody tr td");
    var _StartJigFrame = $(table[0]).text();
    var _StartJigDedion = $(table[1]).text();
    var _StartJigTG = $(table[2]).text();
    var _StartJigRR = $(table[3]).text();
    var _EndJigFrame = $(table[8]).text();
    var _EndJigDedion = $(table[9]).text();
    var _EndJigTG = $(table[10]).text();
    var _EndJigRR = $(table[11]).text();

    var _StartJigFrameCKD = $(tableCKD[0]).text();
    var _StartJigDedionCKD = $(tableCKD[1]).text();
    var _StartJigTGCKD = $(tableCKD[2]).text();
    var _StartJigRRCKD = $(tableCKD[3]).text();
    var _EndJigFrameCKD = $(tableCKD[4]).text();
    var _EndJigDedionCKD = $(tableCKD[5]).text();
    var _EndJigTGCKD = $(tableCKD[6]).text();
    var _EndJigRRCKD = $(tableCKD[7]).text();

    var _F_Customer = $("#F_Customer").val();

    var data = {
        F_Customer: _F_Customer,
        F_StartJigFrame: _StartJigFrame,
        F_StartJigDedion: _StartJigDedion,
        F_StartJigTG: _StartJigTG,
        F_StartJigRR: _StartJigRR,
        F_EndJigFrame: _EndJigFrame,
        F_EndJigDedion: _EndJigDedion,
        F_EndJigTG: _EndJigTG,
        F_EndJigRR: _EndJigRR,
        F_StartJigFrameCKD: _StartJigFrameCKD,
        F_StartJigDedionCKD: _StartJigDedionCKD,
        F_StartJigTGCKD: _StartJigTGCKD,
    }
});