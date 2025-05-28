"use strict";
class libAPI {
    constructor() {
        const _today = new Date();
        const yy = _today.getFullYear();
        const mm = String(_today.getMonth() + 1).padStart(2, '0');
        const dd = String(_today.getDate()).padStart(2, '0');
        const hh = String(_today.getHours()).padStart(2, '0');
        const ms = String(_today.getMinutes()).padStart(2, '0');
        const ss = String(_today.getSeconds()).padStart(2, '0');
        const ml = _today.getMilliseconds();
        this.token = yy + mm + dd + '_' + hh + ms + ss + ml;
        //this.url = 'http://hmmt-app07';
        this.url = 'http://hmmta-tpcap';
        this.systemname = document.URL;
        if (typeof _SYSTEMNAME_ !== 'undefined')
            this.systemname = _SYSTEMNAME_;
    }
    initial(pURL = null) {
        this.url = (pURL != null ? pURL : this.url);
        if (this.getCookie('debug') == '1')
            console.log(this.url + '/ad/?tid=' + this.token);
        $('<iframe>', {
            src: this.url + '/ad/?tid=' + this.token,
            id: 'iframeAPIObject',
            name: 'iframeAPIObject',
            frameborder: 0,
            scrolling: 'no',
            style: 'height:200px;width:300px;display:none',
            title: 'Add iframe for get single sign-on user',
            value: '',
            onload: function () {
                //this.style.display = 'block';
                //return this.token;
            }
        }).appendTo('body');
    }
    getUser(pConfig = null) {
        var ajexHeader = {
            "SystemName": (pConfig.systemname == undefined ? this.systemname : pConfig.systemname)
        };
        if (this.getCookie('debug') == '1')
            console.log(ajexHeader);
        var _token = (pConfig.token == undefined ? this.token : pConfig.tokens);
        var _url = (pConfig.url == undefined ? this.url + '/api/sso?tid=' + _token : pConfig.url);
        if (this.getCookie('debug') == '1')
            console.log(_url);
        const _Timer = setInterval(function () {
            $.ajax({
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: ajexHeader,
                url: _url,
                success: function (result) {
                    //console.log(result);
                    if (result.response == 'OK') {
                        clearInterval(_Timer);
                        //console.log(result);
                        ((pConfig.then != undefined && typeof (pConfig.then) === 'function') ? pConfig.then(result) : null);
                    }
                },
                error: function (result) {
                    console.error('Ajax.GET: ' + result.responseText);
                    // $('#txtUserName').val('20173621');
                    xSplash.hide();
                }
            });
        }, 1000);
    }
    getSingleSignOn(pConfig = null) {
        getUser(pConfig);
    }
    getSSO(pConfig = null) {
        getUser(pConfig);
    }
    getWindowUser(pConfig = null) {
        getUser(pConfig);
    }
    authen(pConfig = null) {
        getUser(pConfig);
    }
    getCookie(cname) {
        let name = cname + "=";
        let decodedCookie = decodeURIComponent(document.cookie);
        let ca = decodedCookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }
}
const xAPI = new libAPI();
