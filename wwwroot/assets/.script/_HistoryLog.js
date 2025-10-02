$(document).ready(function () {
    $("div.gj-datepicker.gj-datepicker-bootstrap.gj-unselectable.mb-3").removeClass("mb-3");
    $("div.gj-datepicker.gj-datepicker-bootstrap.gj-unselectable.input-group.mb-3").removeClass("mb-3");
    //console.log($("div.gj-datepicker.gj-datepicker-bootstrap.gj-unselectable.mb-3"));

});


//$(document).ready(function (e, context) {
       


//    onInitialHistoryLog = function () {

//        //var currentUrl = window.location.href;
//        //console.log("Current URL: " + currentUrl);

//        //var currentPath = window.location.pathname;
//        //console.log("Current Path: " + currentPath);

//        //var protocol = window.location.protocol;
//        //var host = window.location.host;
//        //console.log("Protocol: " + protocol);
//        //console.log("Host: " + host);


//        // Error as #demo is the `div` element
//        //$('#demo').dataTable()

//        // Selector too broad.
//        // Error as `.display` is applied to both the div and the table
//        //$('.display').dataTable();

//        //var _tblHistorySQLLog = $('#tblHistorySQLLog').prop('class');
//        //var _tblHistorySQLLog = $('#tblHistorySQLLog').prop('class');
//        //var _tblHistorySQLLog = $('#tblHistorySQLLog').prop('class');
//        //console.log(_tblHistorySQLLog);


//        if ($('#tblHistorySQLLog').prop('class') != '' && $('#tblHistoryAction').prop('class') != '' && $('#tblHistoryLogin').prop('class') != '') {

//            var tblHistorySQLLog = xDataTable.Initial({
//                name: 'tblHistorySQLLog',
//                //checking: 0,
//                dom: '<"clear">',
//                columnTitle: {
//                    "EN": ['Controller Name', 'Action Name', 'SQL'],
//                    "TH": ['Controller Name', 'Action Name', 'SQL'],
//                    "JP": ['Controller Name', 'Action Name', 'SQL'],
//                },
//                column: [
//                    { "data": "ControllerName" },
//                    { "data": "ActionName" },
//                    { "data": "SQL" }
//                ],
//                addnew: false,
//                rowclick: (data) => {
//                    xAjax.ToClipboard(data.SQL);
//                }
//            });


//            var tblHistoryAction = xDataTable.Initial({
//                name: 'tblHistoryAction',
//                //checking: 0,
//                //dom: '<"clear">',
//                columnTitle: {
//                    "EN": ['Action At', 'User', 'Device Name', 'IP Address', 'Action Name', 'Result', 'SQL'],
//                    "TH": ['Action At', 'User', 'Device Name', 'IP Address', 'Action Name', 'Result', 'SQL'],
//                    "JP": ['Action At', 'User', 'Device Name', 'IP Address', 'Action Name', 'Result', 'SQL'],
//                },
//                column: [
//                    { "data": "ActionAt" },
//                    { "data": "UserName" },
//                    { "data": "DeviceName" },
//                    { "data": "IPAddress" },
//                    //{ "data": "ControllerName" },
//                    { "data": "ActionName" },
//                    { "data": "Result" },
//                    { "data": "SQLDisplay" }
//                ],
//                order: [[0, 'desc']],
//                addnew: false,
//                rowclick: (data) => {
//                    xAjax.ToClipboard(data.SQL);
//                }
//            });


//            var tblHistoryFailed = xDataTable.Initial({
//                name: 'tblHistoryFailed',
//                columnTitle: {
//                    "EN": ['Action At', 'User', 'Device Name', 'IP Address', 'Action Name', 'Message'],
//                    "TH": ['Action At', 'User', 'Device Name', 'IP Address', 'Action Name', 'Message'],
//                    "JP": ['Action At', 'User', 'Device Name', 'IP Address', 'Action Name', 'Message'],
//                },
//                column: [
//                    { "data": "ActionAt" },
//                    { "data": "UserName" },
//                    { "data": "DeviceName" },
//                    { "data": "IPAddress" },
//                    //{ "data": "ControllerName" },
//                    { "data": "ActionName" },
//                    //{ "data": "Result" },
//                    { "data": "MessageDisplay" }
//                ],
//                order: [[0, 'desc']],
//                addnew: false,
//                rowclick: (data, row, col) => {
//                    //console.log(col);
//                    xAjax.ToClipboard(data.SQL);
//                    if (col == 5) xAjax.ToClipboard(data.Message);
//                }
//            });


//            var tblHistoryLogin = xDataTable.Initial({
//                name: 'tblHistoryLogin',
//                //checking: 0,
//                //dom: '<"clear">',
//                columnTitle: {
//                    "EN": ['Code', 'Name', 'Date', 'Start', 'Finish', 'Process Time', 'IP Address', 'Device Name', 'Token'],
//                    "TH": ['Code', 'Name', 'Date', 'Start', 'Finish', 'Process Time', 'IP Address', 'Device Name', 'Token'],
//                    "JP": ['Code', 'Name', 'Date', 'Start', 'Finish', 'Process Time', 'IP Address', 'Device Name', 'Token'],
//                },
//                column: [
//                    { "data": "Code" },
//                    { "data": "UserName" },
//                    { "data": "LoginDate" },
//                    { "data": "StartAt" },
//                    { "data": "FinishAt" },
//                    { "data": "ProcessTime" },
//                    { "data": "IPAddress" },
//                    { "data": "DeviceName" },
//                    { "data": "Token" }
//                ],
//                order: [[3, 'desc']],
//                addnew: false,
//                rowclick: (data) => {
//                }
//            });
//        }

//    }
//    onInitialHistoryLog();



//    var _ModalHistory = document.getElementById('ModalHistory');
//    _ModalHistory.addEventListener('show.bs.modal', function (event) {
//        var button = event.relatedTarget; // Button that triggered the modal
//        var url = '/History/' + button.getAttribute('data-bs-remote'); // URL from data-bs-remote attribute
//        var modalBodyEl = _ModalHistory.querySelector('.modal-body');

//        // Load the content from the URL into the modal body using AJAX or fetch
//        fetch(url)
//            .then(response => response.text())
//            .then(data => {
//                modalBodyEl.innerHTML = data;

//                xHistory.onLoad();

//                xSplash.hide();
//            });
//    });



//    var _ModalHistorySystem = document.getElementById('ModalHistorySystem');
//    _ModalHistorySystem.addEventListener('show.bs.modal', function (event) {

//        $('#tblHistorySQLLog').dataTable().fnClearTable();
//        $('#tblHistoryAction').dataTable().fnClearTable();
//        $('#tblHistoryFailed').dataTable().fnClearTable();

//        xAjax.Post({
//            url: 'HistoryLogin/getHistory',
//            data: {
//                'System': 'KB3',
//                'UserCode': _UID_,
//                'Controller': _CONTROLLER_,
//                'Action': _PAGE_,
//                'Date': $('#frmHistorySystemDate').val(),
//                'DateTo': $('#frmHistorySystemDateTo').val(),
//                'FailedDate': $('#frmHistoryFailedDate').val(),
//                'FailedDateTo': $('#frmHistoryFailedDateTo').val(),
//                'Exclude': $('#frmHistorySystemExclude').val()
//            },
//            then: function (result) {
//                console.log(result);
//                if (result.data.action.length > 0) $('#tblHistoryAction').dataTable().fnAddData(result.data.action);
//                if (result.data.failed.length > 0) $('#tblHistoryFailed').dataTable().fnAddData(result.data.failed);
//                if (result.data.sql.length > 0) $('#tblHistorySQLLog').dataTable().fnAddData(result.data.sql);

//            }
//        });

//        //console.log('SQL');

//    });

//    onCHangeHistorySystem = function () {

//        xAjax.Post({
//            url: 'HistoryLogin/getHistory',
//            data: {
//                'System': 'KB3',
//                'UserCode': _UID_,
//                'Controller': _CONTROLLER_,
//                'Action': _PAGE_,
//                'Date': $('#frmHistorySystemDate').val(),
//                'DateTo': $('#frmHistorySystemDateTo').val(),
//                'FailedDate': $('#frmHistoryFailedDate').val(),
//                'FailedDateTo': $('#frmHistoryFailedDateTo').val(),
//                'Exclude': $('#frmHistorySystemExclude').val()
//            },
//            then: function (result) {
//                //console.log(result);
//                $('#tblHistoryAction').dataTable().fnClearTable();
//                if (result.data.action.length > 0) $('#tblHistoryAction').dataTable().fnAddData(result.data.action);

//                $('#tblHistoryFailed').dataTable().fnClearTable();
//                if (result.data.failed.length > 0) $('#tblHistoryFailed').dataTable().fnAddData(result.data.failed);

//                $('#tblHistorySQLLog').dataTable().fnClearTable();
//                if (result.data.login.length > 0) $('#tblHistorySQLLog').dataTable().fnAddData(result.data.sql);
//            }
//        });
//    }
//    xAjax.onChange('#frmHistorySystemDate', function () {
//        onCHangeHistorySystem();
//    });
//    xAjax.onChange('#frmHistorySystemDateTo', function () {
//        onCHangeHistorySystem();
//    });
//    xAjax.onChange('#frmHistorySystemExclude', function () {
//        onCHangeHistorySystem();
//    });
//    xAjax.onChange('#frmHistoryFailedDate', function () {
//        onCHangeHistorySystem();
//    });
//    xAjax.onChange('#frmHistoryFailedDateTo', function () {
//        onCHangeHistorySystem();
//    });



//    var _ModalHistoryLogin = document.getElementById('ModalHistoryLogin');
//    _ModalHistoryLogin.addEventListener('show.bs.modal', function (event) {

//        $('#tblHistoryLogin').dataTable().fnClearTable();

//        xAjax.Post({
//            url: 'HistoryLogin/getHistory',
//            data: {
//                'System': 'KB3',
//                'UserCode': _UID_,
//                'Controller': _CONTROLLER_,
//                'Action': _PAGE_,
//                'Date': $('#frmHistoryLoginToDay').val()
//            },
//            then: function (result) {
//                //console.log(result);
//                if (result.data.login.length > 0) $('#tblHistoryLogin').dataTable().fnAddData(result.data.login);
//            }
//        });

//        //console.log('SQL');

//    });
//    xAjax.onChange('#frmHistoryLoginToDay', function () {
//        xAjax.Post({
//            url: 'HistoryLogin/getHistory',
//            data: {
//                'System': 'KB3',
//                'UserCode': _UID_,
//                'Controller': _CONTROLLER_,
//                'Action': _PAGE_,
//                'Date': $('#frmHistoryLoginToDay').val()
//            },
//            then: function (result) {
//                //console.log(result);
//                $('#tblHistoryLogin').dataTable().fnClearTable();
//                if (result.data.login.length > 0) {
//                    $('#tblHistoryLogin').dataTable().fnAddData(result.data.login);
//                }
//            }
//        });
//    });










//});

