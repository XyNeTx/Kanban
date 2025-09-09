$(document).ready(async function () {
    xSplash.show();
    $("#txtUserName").prop('readonly', true);
    $("#txtProcessDate").prop('disabled', true);
    $("#ddlShift").prop('disabled', true);
    //requestHostName();
    // const formAuthentication = {
    //     "txtUserName": "20223983",
    //     "txtProcessDate": "2023-09-08",
    //     "ddlFactory": "2",
    //     "ddlShift": "D",
    //     "txtDomain": "D",
    //     "txtDeviceName": "D",
    //     "txtFullDeviceName": "D",
    //     "txtIPAddress": "D",
    // }

    $(".date-picker").datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
        todayHighlight: true,
        autoclose: true,
        showRightIcon: false
    });

    // $(".date-picker").parent().find("button").prop("disabled", true);

    // initTheme = function () {
    //     setCookie('UI_ExpandIcon', 'style2');
    //     setCookie('UI_Header', 'theme1');
    //     setCookie('UI_HeaderBrand', 'theme1');
    //     setCookie('UI_IconColor', 'st6');
    //     setCookie('UI_Layout', 'theme1');
    //     setCookie('UI_LinkColor', 'theme5');
    //     setCookie('UI_MenuColor', 'theme6');
    //     setCookie('UI_MenuIcon', 'style4');
    //     setCookie('UI_SideBar', 'shrink');
    // }

    await (_xLib.GetCookie("isDev") == null || _xLib.GetCookie("isDev") == "" || _xLib.GetCookie("isDev") == "null"|| _xLib.GetCookie("isDev") == undefined)
        ? _xLib.SetCookie("isDev", 0) : null;

    initial = async function () {
        // initTheme();

        //axios.get('http://hmmt-app07/sso_test/api/SingleSignOn/GetLogin', {
        //    params: {
        //        system_name: 'Hino Kanban System'
        //    },
        //    withCredentials: true // Include credentials in the request
        //})
        //    .then(response => {
        //        console.log("AXIOS : ");
        //        console.log(response.data);
        //    })
        //    .catch(error => {
        //        console.error('Error:', error);
        //    });

        await $.ajax({
            url: "http://hmmt-app07/sso_test/api/SingleSignOn/GetLogin",
            type: "GET",
            xhrFields: {
                withCredentials: true // Include credentials in the request
            },
            data: {
                system_name: 'Hino Kanban System'
            },

            success: function (result) {
                if (_xLib.GetCookie("isDev") == '1') $('h4').append('<h5 class="mt-3">DEVELOPER MODE</h5>');
                //console.log(result);
                $("#txtProcessDate").prop('disabled', true);
                $("#ddlShift").prop('disabled', true);

                if (result.userDetail.departmentCode.startsWith("44")
                    && !result.userDetail.departmentCode.includes("_")) {
                    console.assert(result.userDetail.departmentCode.startsWith("44"), "Department Code is not 44");
                    $("#txtUserName").prop('readonly', false);
                    $("#txtProcessDate").prop('disabled', false);
                    $("#ddlShift").prop('disabled', false);
                }

                $('#txtUserName').val(result.userName);
                //$('#txtUserName').val("20234011");
                $('#txtDeviceName').val(result.computerName);
                $('#txtFullDeviceName').val(result.fullComputerName);
                $('#ddlFactory').val(result.userDetail.locationCode);
                $('#txtDomain').val(result.domainName);
                $('#txtIPAddress').val(result.ipAddress);
                _xLib.SetCookie('plantCode', result.userDetail.locationCode);

            },
            error: async function (error) {
                console.log(error);
                await xSplash.hide();
                xSwal.error('Error', "Can't Get User to Login");
            }

        });

        await _xLib.AJAX_GetNoHeader(`/xapi/GetLoginDate`, '',
            function (success) {
                //console.log(success);
                if (success.status == "200") {
                    $("#txtProcessDate").val(moment(success.data.date).format("DD/MM/YYYY"));
                    $("#ddlShift").val(success.data.shift);
                }
            },
            function (error) {
                //console.log(error);
                xSplash.hide();
                xSwal.error('Error', "Can't Get Login Date");
            }
        );

    }


    await initial();
    xSplash.hide();

    let iSpy = 0;
    $('#imgHINOLogo').on('click', function (e) {
        iSpy++;
        if (iSpy == 6) {
            let _isDev = _xLib.GetCookie("isDev");
            if (_isDev == 0) {
                _isDev = 1;
            } else {
                _isDev = 0;
            }
            _xLib.SetCookie("isDev", _isDev);
            document.location.reload();
            xSplash.hide();
        }
    });

});

$("#btnSubmit").click(async function () {
    $("#formAuthentication").one('submit', async function (e) {
        e.preventDefault();
        var processDate = moment($('#txtProcessDate').val(), "DD/MM/YYYY").format("YYYY-MM-DD");
        var shift = $("#ddlShift").val() == 1 ? "D" : "N";
        _xLib.SetCookie('loginDate', processDate+shift);
        _xLib.SetCookie('isDev', _xLib.GetCookie("isDev"));
        _xLib.SetCookie('plantCode', $('#ddlFactory').val());

        sessionStorage.setItem("loginDate", processDate+shift);
        sessionStorage.setItem("shift", $("#ddlShift").val());
        sessionStorage.setItem("isDev", _xLib.GetCookie("isDev"));
        sessionStorage.setItem("plantCode", $('#ddlFactory').val());

        $("#ddlShift").prop('disabled', false);
        $("#txtProcessDate").prop('disabled', false);
        let isGen = await GenerateJWTToken();
        if (isGen) {
            $(this).submit();
        }
        else {
            window.location.reload();
        }
    });

});

async function GenerateJWTToken() {
    var obj = {
        UserCode: $('#txtUserName').val(),
        Factory: $('#ddlFactory').val(),
        Device: $('#txtDeviceName').val(),
        IpAddress: $('#txtIPAddress').val(),
    }
    return await _xLib.AJAX_PostNoHeader("/api/login/GenerateJWTToken", obj,
        async function (success) {
            console.log(success.token);
            localStorage.setItem("TOKEN", success.token);
            sessionStorage.setItem("UserCode", obj.UserCode);
            sessionStorage.setItem("Factory", obj.Factory);
            sessionStorage.setItem("Device", obj.Device);
            sessionStorage.setItem("IpAddress", obj.IpAddress);
            Headers = { 'Authorization': `Bearer ${success.token}` };

            return true;
        },
        async function (error) {
            xSwal.error("Login Failed");
            return false;
        }
    );
}