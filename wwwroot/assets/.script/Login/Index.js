$(document).ready(async function () {


    const formAuthentication = {
        "txtUserName": "20223983",
        "txtProcessDate": "2023-09-08",
        "ddlFactory": "2",
        "ddlShift": "D",
        "txtDomain": "D",
        "txtDeviceName": "D",
        "txtFullDeviceName": "D",
        "txtIPAddress": "D",
    }

    initTheme = function () {
        setCookie('UI_ExpandIcon','style2');
        setCookie('UI_Header','theme1');
        setCookie('UI_HeaderBrand', 'theme1');
        setCookie('UI_IconColor', 'st6');
        setCookie('UI_Layout', 'theme1');
        setCookie('UI_LinkColor', 'theme5');
        setCookie('UI_MenuColor', 'theme6');
        setCookie('UI_MenuIcon', 'style4');
        setCookie('UI_SideBar', 'shrink');
    }
    //console.log(_xLib.GetCookie("isDev"));
    await _xLib.GetCookie("isDev") == null || _xLib.GetCookie("isDev") == "" ? _xLib.SetCookie('isDev', 0) : _xLib.GetCookie('isDev');
    await _xLib.GetCookie("debug") == 1 ? _xLib.SetCookie("isDev", 1) : _xLib.SetCookie("isDev", 0);
    //console.log(_xLib.GetCookie("isDev"));
    initial = async function () {
        xSplash.show();

        initTheme();

        xAPI.initial();
        xAPI.getUser({
            //systemname: 'Hino Kanban F.3',
            then: function (result) {
                //console.log(result);
                if (result.response == 'OK') {

                    if (getCookie('debug') == '') setCookie('debug', 0);
                    if (getCookie('debug') == '1') console.log(result);

                    $('#txtDomain').val(result.data.domain);
                    $('#txtIPAddress').val(result.data.ipaddress);
                    //$('#txtProcessDate').val(xDate.Date('yyyy-MM-dd'));
                    //$('#ddlFactory').val(3);
                    //$('#ddlShift').val((xDate.Time('hhmm') < 1930 ? "1" : "2"));


                    if (getCookie('debug') == '1') $('#txtUserName').val('DEVELOPER');
                    if (getCookie('debug') == '1') $('#txtUserName').removeAttr('readonly');

                    if (_DEV_) {
                        console.info('DEVELOPER ACTIVE');
                        //$('#txtUserName').removeAttr('readonly');
                        //if (result.data.username == '20223983') $('#txtUserName').val('DEVELOPER');
                    } else {
                        console.info('PRODUCTION ACTIVE');
                    }

                }
            }
        })


        await $.ajax({
            url: "http:\\\\hmmt-app07/sso/api/SingleSignOn/getLogin",
            type: "GET",
            xhrFields: {
                withCredentials: true // Include credentials in the request
            },

            success: function (result) {
                //console.log(result);
                $('#txtUserName').val(result.userName);
                //$('#txtUserName').val("20234011");
                $('#txtDeviceName').val(result.computerName);
                $('#txtFullDeviceName').val(result.fullComputerName);
                $('#ddlFactory').val(result.userDetail.locationCode);
                _xLib.SetCookie('plantCode', result.userDetail.locationCode);
            },
            error: function (error) {
                //console.log(error);
                xSwal.error('Error', "Can't Get User to Login");
            }

        });

        await _xLib.AJAX_GetNoHeader(`/xapi/GetLoginDate`, '',
            function (success) {
                //console.log(success);
                if (success.status == "200") {
                    $("#txtProcessDate").val(success.data.date);
                    $("#ddlShift").val(success.data.shift);
                    $("#txtProcessDate").prop('readonly', true);
                    $("#ddlShift").prop('disabled', true);

                    if ($("#txtUserName").val() === "20234111" || _xLib.GetCookie("debug") === "1") {
                        $("#txtProcessDate").prop('readonly', false);
                        $("#ddlShift").prop('disabled', false);
                    }
                }
            },
            function (error) {
                //console.log(error);
                xSwal.error('Error', "Can't Get Login Date");
            }
        );

    }


    await initial();
    xSplash.hide();




    let iSpy = 0;
    $('#imgHINOLogo').on('click',function (e) {
        iSpy++;
        if (iSpy == 6) {
            let _debug = getCookie('debug');
            let _isDev = _xLib.SetCookie('isDev', 1);
            if (_debug == 0) {
                _debug = 1;
                _isDev = 1;
            } else {
                _debug = 0;
                _isDev = 0;
            }
            _xLib.SetCookie('isDev', _isDev);
            setCookie('debug', _debug);
            document.location.reload();
        }
    });

});

//$('.gj-datepicker').prop('disabled', true);

//$(".btn btn-outline-secondary border-left-0").click(function (e) {
//    if ($("#txtUserName") !== "20234111" || $("#txtUserName") !== "20062084") {
//        e.preventDefault();
//    }
//});

$("#btnSubmit").click(function () {
    $("#formAuthentication").one('submit', function (e) {
        e.preventDefault();
        var processDate = $('#txtProcessDate').val();
        var shift = $("#ddlShift").val() == 1 ? "D" : "N";
        document.cookie = `loginDate=${processDate}${shift}`;
        _xLib.SetCookie('plantCode', $('#ddlFactory').val());
        $("#ddlShift").prop('disabled', false);
        $(this).submit();
    });

});
