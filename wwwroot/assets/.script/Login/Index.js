$(document).ready(function () {


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

    initial = function () {
        xSplash.show();

        initTheme();

        xAPI.initial();
        xAPI.getUser({
            //systemname: 'Hino Kanban F.3',
            then: function (result) {
                console.log(result);
                if (result.response == 'OK') {

                    if (getCookie('debug') == '') setCookie('debug', 0);
                    if (getCookie('debug') == '1') console.log(result);

                    $('#txtDomain').val(result.data.domain);
                    $('#txtDeviceName').val(result.data.devicename);
                    $('#txtFullDeviceName').val(result.data.fulldevicename);
                    $('#txtIPAddress').val(result.data.ipaddress);
                    $('#txtUserName').val(result.data.username);
                    $('#txtProcessDate').val(xDate.Date('yyyy-MM-dd'));
                    $('#ddlFactory').val(3);
                    $('#ddlShift').val((xDate.Time('hhmm') < 1930 ? "1" : "2"));


                    if (getCookie('debug') == '1') $('#txtUserName').val('DEVELOPER');
                    if (getCookie('debug') == '1') $('#txtUserName').removeAttr('readonly');

                    if (_DEV_) {
                        console.info('DEVELOPER ACTIVE');
                        //$('#txtUserName').removeAttr('readonly');
                        //if (result.data.username == '20223983') $('#txtUserName').val('DEVELOPER');
                    } else {
                        console.info('PRODUCTION ACTIVE');
                    }

                    xSplash.hide();

                }
            }
        })


    }
    initial();




    let iSpy = 0;
    $('#imgHINOLogo').on('click',function (e) {
        iSpy++;
        if (iSpy == 3) {
            let _debug = getCookie('debug');
            if (_debug == 0) {
                _debug = 1;
            } else {
                _debug = 0;
            }

            setCookie('debug', _debug);
            document.location.reload();
        }
    });


    $.ajax({
        url: "http:\\\\hmmt-app07/sso/api/SingleSignOn/getLogin",
        type: "GET",
        xhrFields: {
            withCredentials: true // Include credentials in the request
        },

        success: function (result) {
            console.log(result);
            console.log($('#txtProcessDate').val());
        }
    });


});

$("#formAuthentication").one('submit',function (e) {
    e.preventDefault();
    var processDate = $('#txtProcessDate').val();
    var shift = $("#ddlShift").val() == 1 ? "D" : "N";
    document.cookie = `loginDate=${processDate}${shift}`;
    $(this).submit();
});