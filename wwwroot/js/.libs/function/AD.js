"use strict";
//console.log(NameSpace);
const url = (_NAMESPACE_ != '' ? 'http://hmmta-tpcap/ad' : 'http://hmmta-tpcap/ad');
//const url = (_NAMESPACE_ != '' ? 'http://hmmt-app07/api/' : 'http://hmmt-app07/api/');
fncInitialAD = function () {
    ////splash.show();
    //if (window.location.search != '') {
    //    var _q = window.location.search;
    //    setCookie('_CN', _q.substring(4), 999);
    //}
    //var _CN = getCookie("_CN");
    //return;
    console.log(_DEVELOPER_);
    if (_DEV_) {
        $('#txtDomain').val(_DEVELOPER_.domain);
        $('#txtUserName').val(_DEVELOPER_.username);
        $('#txtDeviceName').val(_DEVELOPER_.devicename);
        $('#txtFullDeviceName').val(_DEVELOPER_.fulldevicename);
        $('#txtIPAddress').val(_DEVELOPER_.ipaddress);
        //splash.hide();
    }
    else {
        fetch(url)
            .then(response => {
            console.log(response);
            if (!response.ok) {
                throw new Error('Login.onInitial| NetworkError: Response was not ok.');
            }
            return response.text();
        })
            .then(text => {
            if (text == '') {
                console.error("Login.onInitial| ProcessError : Cannot login with single sign-on.");
                return;
            }
            console.log(text);
            if (text.startsWith('{')) {
                var _json = JSON.parse(text);
                $('#txtDomain').val(_json.domain);
                $('#txtUserName').val(_json.username);
                $('#txtDeviceName').val(_json.devicename);
                $('#txtFullDeviceName').val(_json.fulldevicename);
                $('#txtIPAddress').val(_json.ipaddress);
            }
            //console.log(_json);
            //splash.hide();
        })
            .catch(error => {
            console.error('Login.InitialAD| ' + error);
            //splash.hide();
        });
    }
};
