const { createApp, onMounted, ref } = Vue

createApp({
    setup() {

        const formAuthentication = ref({
            "txtUserName": "20223983",
            "txtProcessDate": "2023-09-08",
            "ddlFactory": "2",
            "ddlShift": "D",
            "txtDomain": "D",
            "txtDeviceName": "D",
            "txtFullDeviceName": "D",
            "txtIPAddress": "D",
            })

        const message = ref('Sign In')        


        const signIn = () => {
            $('#txtUserName').val('xXx');
        }


        onMounted(() => {
            //console.log(xDate.Date('yyyy-MM-dd'));
            //console.log(url);
            //_DEV_ = false;
            if (_DEV_) {
                console.info('Developer active');

                $('#txtUserName').removeAttr('readonly');
                //$('#txtUserName').val('DEVELOPER');
                //console.log(_DEVELOPER_);
                //console.log(formAuthentication.value);
                formAuthentication.value.txtDomain = _DEVELOPER_.domain
                formAuthentication.value.txtDeviceName = _DEVELOPER_.devicename
                formAuthentication.value.txtFullDeviceName = _DEVELOPER_.fulldevicename
                formAuthentication.value.txtIPAddress = _DEVELOPER_.ipaddress
                formAuthentication.value.txtUserName = _DEVELOPER_.username
                formAuthentication.value.txtProcessDate = xDate.Date('yyyy-MM-dd');
                formAuthentication.value.ddlFactory = '3'
                formAuthentication.value.ddlShift = (xDate.Time('hhmm') < 1930 ? "D" : "N")
                if (_DEVELOPER_.username=='20223983') formAuthentication.value.txtUserName = 'DEVELOPER';
                xSplash.hide();
            } else {
                console.info('Production active');
                fetch(url)
                    .then(response => {
                        //console.log(response);
                        if (!response.ok) {
                            throw new Error('Login.onInitial| NetworkError: Response was not ok.');
                        }
                        return response.text();
                    })
                    .then(response => {
                        if (response == '') {
                            console.warn("Login.onInitial| ProcessError : Cannot login with single sign-on.");
                            return;
                        }
                        if (response.startsWith('{')) {
                            var _json = JSON.parse(response);
                            formAuthentication.value.txtDomain = _json.domain
                            formAuthentication.value.txtDeviceName = _json.devicename
                            formAuthentication.value.txtFullDeviceName = _json.fulldevicename
                            formAuthentication.value.txtIPAddress = _json.ipaddress
                            formAuthentication.value.txtUserName = _json.username
                        }
                    })
                    .catch(error => {
                        console.warn('Login.onInitial| ' + error);
                    });

                xSplash.hide();
            }

        })


        return {
            formAuthentication
            //signIn,
            //message,
        }
    }



}).mount('#vueApp')
