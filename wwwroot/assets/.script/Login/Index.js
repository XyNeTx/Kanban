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

    initial = function () {
        xSplash.show();

        xAPI.initial();
        xAPI.getUser({
            //systemname: 'Hino Kanban F.3',
            then: function (result) {
                if (result.response == 'OK') {
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

                    if (_DEV_) {
                        console.info('Developer active');
                        $('#txtUserName').removeAttr('readonly');
                        if (result.data.username == '20223983') $('#txtUserName').val('DEVELOPER');
                    } else {
                        console.info('Production active');
                    }

                    xSplash.hide();

                }
            }
        })

        //xSSO.getSingleSignOn({
        //    then: function (result) {
        //        //console.log(result.data);

        //        $('#txtDomain').val(result.data.domain);
        //        $('#txtDeviceName').val(result.data.devicename);
        //        $('#txtFullDeviceName').val(result.data.fulldevicename);
        //        $('#txtIPAddress').val(result.data.ipaddress);
        //        $('#txtUserName').val(result.data.username);
        //        $('#txtProcessDate').val(xDate.Date('yyyy-MM-dd'));
        //        $('#ddlFactory').val(3);
        //        $('#ddlShift').val((xDate.Time('hhmm') < 1930 ? "D" : "N"));


        //        if (getCookie('debug') == '1') $('#txtUserName').val('DEVELOPER');

        //        if (_DEV_) {
        //            console.info('Developer active');
        //            $('#txtUserName').removeAttr('readonly');
        //            if (result.data.username == '20223983') $('#txtUserName').val('DEVELOPER');
        //        } else {
        //            console.info('Production active');
        //        }

        //        xSplash.hide();
        //    }
        //});





        //if (_DEV_) {
        //    console.info('Developer active');

        //    $('#txtUserName').removeAttr('readonly');
        //    $('#txtDomain').val(_DEVELOPER_.domain);
        //    $('#txtDeviceName').val(_DEVELOPER_.devicename);
        //    $('#txtFullDeviceName').val(_DEVELOPER_.fulldevicename);
        //    $('#txtIPAddress').val(_DEVELOPER_.ipaddress);
        //    $('#txtUserName').val(_DEVELOPER_.username);
        //    $('#txtProcessDate').val(xDate.Date('yyyy-MM-dd'));
        //    $('#ddlFactory').val(3);
        //    $('#ddlShift').val((xDate.Time('hhmm') < 1930 ? "D" : "N"));
        //    if (_DEVELOPER_.username == '20223983') $('#txtUserName').val('DEVELOPER');
        //    xSplash.hide();
        //} else {

        //    //console.log(url);
        //    console.info('Production active');
        //    fetch(url)
        //        //.then(response => {
        //        //    console.log(response);
        //        //    if (!response.ok) {
        //        //        throw new Error('Login.onInitial| NetworkError: Response was not ok.');
        //        //    }
        //        //    return response.text();
        //        //})
        //        .then(response => {
        //            if (response == '') {
        //                console.warn("Login.onInitial| ProcessError : Cannot login with single sign-on.");
        //                return;
        //            }
        //            if (response.startsWith('{')) {
        //                var _json = JSON.parse(response);
        //                $('#txtDomain').val(_json.domain);
        //                $('#txtDeviceName').val(_json.devicename);
        //                $('#txtFullDeviceName').val(_json.fulldevicename);
        //                $('#txtIPAddress').val(_json.ipaddress);
        //                $('#txtUserName').val(_json.username);


        //                console.log(getCookie('debug'));
        //                if (getCookie('debug') == '1') $('#txtUserName').val('DEVELOPER');
        //            }
        //        })
        //        .catch(error => {
        //            console.warn('Login.onInitial| ' + error);
        //        });

        //    xSplash.hide();
        //}





    }
    initial();


    //console.log(url);

    //fetch(url)
    //    .then(response => {
    //        console.log(response);
    //    })
    //    .then(text => {
    //        console.log(text);
    //        //if (text == '') {
    //        //    console.error("Login.onInitial| ProcessError : Cannot login with single sign-on.");
    //        //    return;
    //        //}
    //        //console.log(text);
    //        //if (text.startsWith('{')) {
    //        //    var _json = JSON.parse(text);
    //        //    $('#txtDomain').val(_json.domain);
    //        //    $('#txtUserName').val(_json.username);
    //        //    $('#txtDeviceName').val(_json.devicename);
    //        //    $('#txtFullDeviceName').val(_json.fulldevicename);
    //        //    $('#txtIPAddress').val(_json.ipaddress);
    //        //}
    //        //console.log(_json);

    //        //splash.hide();
    //    })
    //    .catch(error => {
    //        console.error('Login.InitialAD| ' + error);
    //        //splash.hide();
    //    });

    //$.ajax({
    //    type: "GET",
    //    url: 'http://hmmt-app07/api',
    //    success: function (result) {
    //        console.log(result);

    //    },
    //    error: function (result) {
    //        //console.error('Login.Index: ' + result);
    //        xSplash.hide();
    //    }
    //});



    //initial = function () {
    //    xAjax.Post({
    //        url: 'KBNIM003/initial',
    //        then: function (result) {
                
    //        }
    //    })

    //    xItem.reset({
    //        id: 'pgsStatus',
    //    })

    //    xSplash.hide();
    //}
    //initial();



});