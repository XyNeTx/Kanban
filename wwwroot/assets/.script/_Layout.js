$(document).ready(function (e, context) {


    var browserInfo = {
        appName: navigator.appName,
        appVersion: navigator.appVersion,
        userAgent: navigator.userAgent,
        platform: navigator.platform
    };

    var _debug = getCookie('debug');
    if ((_debug == undefined) || (_debug == '')) setCookie('debug', '0');
    if (_debug != 1) $('#_SYSTEMHISTORY_').attr('style', 'visibility:hidden;display:none;');
    if (_debug != 1) $('#_SYSTEMHISTORYMENU_').attr('style', 'visibility:hidden;display:none;');
    

    $('#_REFERCODE_').parent().after('<i class="modal-reference mr-auto">Plant :&nbsp;<i id="_MODALPLANT_">' + _PLANT_ + '</i></i>');
    $('#_MODALPLANT_').parent().after('<i class="modal-reference mr-auto"><i id="_MODALUSER_">' + _UID_ +' : '+ _DISPLAY_ + '</i></i>');
    $('#_MODALUSER_').parent().after('<i class="modal-reference">At :&nbsp;<i id="_MODALDATETIME_"></i></i>');
    var _MODALDATETIME_ = setInterval(function () {
        $('#_NAVDATETIME_').html('Today : ' + xDate.Now());
        $('#_MODALDATETIME_').html(xDate.Now());
    }, 1000);

    if (!_PERMISSION_.toolbar) {
        $('.btn-toolbar').attr('style','visibility:hidden;display:none;');
    }



    $("#ButtonLayoutExport_Copy").click(function () {
        $('#tblMaster').DataTable().buttons(0).trigger();
    });

    $('#ButtonLayoutExport_CSV').on("mousedown", function () {
        xSwal.questionPack(_i18n_._Layout.export.swal_csv,
            function () {
                xSplash.show('EXPORT TO CSV');
                xClock.Start(1000, function () {
                    _EXPORTING_ = true;
                    xDataTableExport.Exporting(1);
                })
            })
    });

    $('#ButtonLayoutExport_XLS').on("mousedown", function () {
        xSwal.questionPack(_i18n_._Layout.export.swal_xls,
            function () {
                xSplash.show('EXPORT TO XLS');
                xClock.Start(1000, function () {
                    _EXPORTING_ = true;
                    xDataTableExport.Exporting(2);
                })
            })
    });

    $('#ButtonLayoutExport_PDF').on("mousedown", function () {
        xSwal.questionPack(_i18n_._Layout.export.swal_pdf,
            function () {
                xSplash.show('EXPORT TO PDF');
                xClock.Start(1000, function () {
                    _EXPORTING_ = true;
                    xDataTableExport.Exporting(3);
                })
            })
    });

    $('#ButtonLayoutExport_Print').click(function () {
        $('#tblMaster').DataTable().buttons(4).trigger();
    });





    $('#fileImage').on("change", function () {

        var file = $(this)[0].files[0];
        var formData = new FormData();
        formData.append("file", file);
        formData.append("userid", $('#frmModalUserProfile #_ID').val());
        formData.append("usercode", $('#frmModalUserProfile #Code').val());

        $.ajax({
            url: _HOSTNAME_ + '/Layout/ProfileImage',
            type: 'POST',
            enctype: 'multipart/form-data',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                //console.log(response);
                $('#profile-avatar').attr('src', _HOSTNAME_ + '/assets/img/avatars/' + response.data);
                $('#profile-image').attr('src', _HOSTNAME_ + '/assets/img/avatars/' + response.data);

                setCookie('Avatar', response.data);
            }
        });

    });


    onSaveProfile = function () {

        Swal.fire({
            title: i18nLayout._Layout.modal.profile.swal.title,
            text: i18nLayout._Layout.modal.profile.swal.text,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: i18nLayout._Layout.modal.profile.swal.cancel,
            confirmButtonText: i18nLayout._Layout.modal.profile.swal.save
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: 'PATCH',
                    headers: ajexHeader,
                    url: (_PAGE_ != 'Index' ? '../' : '') + 'Layout/Profile',
                    data: $("#frmModalUserProfile").serialize(),
                    dataType: "json",
                    success: function (result) {
                        //console.log(result);
                        $('#ModalUserProfile').modal('hide');

                        if (result.response == 'OK') {
                            //result.data[0];
                            if (result.data[0].Code + 0 > 0) {
                                document.location.replace((_PAGE_ != 'Index' ? '../' : '') + 'Login/Logout');
                                //} else {
                                //document.location.replace('@Url.Action("",ViewData["System"].ToString())');
                                //document.location.replace((_PAGE_ != 'Index' ? '../' : '') + 'Home/Logout');
                            }
                        }


                    },
                    error: function () {
                        console.log('error handling here');
                    }
                });
            }
        })


    }

    $('#load-wrapper').attr("style", "visibility:hidden;");
    $('#table-wrapper').attr("style", "visibility:hidden;");




    //onChangePassword = function () {
    //    $.ajax({
    //        type: 'POST',
    //        headers: ajexHeader,
    //        url: '@Url.Action("ForgotPassword","Authen")',
    //        data: $("#frmModalUserPassword").serialize(),
    //        dataType: "json",
    //        success: function (result) {
    //            if (result.response == "OK") {
    //                Swal.fire({
    //                    title: i18nLayout._Layout.modal.password.swal.title,
    //                    text: i18nLayout._Layout.modal.password.swal.text,
    //                    icon: 'info',
    //                    showCancelButton: false,
    //                    confirmButtonColor: '#3085d6',
    //                    confirmButtonText: i18nLayout._Layout.modal.password.swal.ok,
    //                }).then((result) => {
    //                    $('#ModalUserPassword').modal('hide');
    //                });
    //            } else {
    //                console.log(result);
    //            }
    //        },
    //        error: function () {
    //            console.log('error handling here');
    //        }
    //    });
    //}


    //onLogout = function () {
    //    console.log('onLogout');
    //    //setCookie('',)
    //    //var _logout = getCookie('logout');
    //    console.log(_logout);
    //}

    //onInitial = function () {
    //    try {

    //        var _uid = '@ViewData["UserCode"]';
    //        var tree = $('#menutree').tree({
    //            primaryKey: 'id',
    //            dataSource: _HOSTNAME_ + '/assets/template/Menu/' + _uid + '.json',
    //            icons: {
    //                expand: '<i class="gj-icon chevron-right" style="color:white;"></i>',
    //                collapse: '<i class="gj-icon chevron-down" style="color:white;"></i>'
    //            },
    //            //initialized: function (e) {
    //            //    console.log('initialized is fired.');
    //            //},
    //            //dataBinding: function (e) {
    //            //    console.log('dataBinding is fired.');
    //            //    console.log(e);
    //            //},
    //            select: function (e, node, id) {
    //                var _node = tree.getNodeById(id);
    //                var data = tree.getDataById(id);
    //                var _m = getCookie('_Menu');

    //                if (data.url != undefined) {
    //                    setCookie('_Menu', id, 9999);
    //                    if (data.id != _m) document.location.replace(data.url.replace('~', _HOSTNAME_));
    //                } else {

    //                    tree.unselectAll();

    //                    //setCookie('_Select', id, 9999);


    //                    var _status = $('[data-id=' + id + '] div span[data-role=expander]').attr('data-mode');
    //                    if (_status == 'open' || _status == undefined) {
    //                        $($('[data-id=' + id + '] div span[data-role=expander]')[0]).attr('data-mode', 'close');
    //                        $($('[data-id=' + id + '] ul')[0]).attr('style', 'display:none;');
    //                        tree.collapse(node);
    //                    } else {
    //                        $($('[data-id=' + id + '] div span[data-role=expander]')[0]).attr('data-mode', 'open');
    //                        $($('[data-id=' + id + '] ul')[0]).attr('style', 'display:block;');
    //                        tree.expand(node);
    //                    }

    //                    //var _n = tree.getNodeById(id);

    //                    //tree.unselect(id);
    //                    //tree.unselectAll();

    //                }


    //            },
    //            unselect: function (e, node, id) {
    //                //var _node = tree.getNodeById(id);
    //                //var _s = getCookie('_Select');
    //            }
    //        });
    //        tree.on('dataBound', function () {
    //            var _p = getCookie('_Parent');
    //            var _m = getCookie('_Menu');
    //            const myArray = _p.split(",");
    //            $.each(myArray, function (k, v) {

    //                if (v != '') tree.expand(tree.getNodeById(v));
    //            });
    //            setCookie('_Parent', _p, 9999);


    //            if (window.location.pathname.indexOf('Home') < 0) {
    //                tree.select(tree.getNodeById(_m));

    //                var s = $('li [data-id="' + _m + '"] div span')[2];
    //                $(s).attr('style', 'color:red; font-style:oblique;');
    //                //console.log($(s)[0].innerHTML);
    //            } else {
    //                setCookie('_Menu', '', 9999);
    //            }
    //        });
    //        tree.on('expand', function (e, node, id) {
    //            var _p = getCookie('_Parent');
    //            if (_p.indexOf(',' + id) < 0) _p += ',' + id;
    //            setCookie('_Parent', _p, 9999);
    //        });

    //        tree.on('collapse', function (e, node, id) {
    //            var _p = getCookie('_Parent');
    //            _p = _p.toString().replaceAll(',' + id, '');
    //            setCookie('_Parent', _p, 9999);
    //        });

    //        tree.on('nodeDataBound', function (e, node, id, record) {
    //            if (!record.parent) {
    //                var _node = $('li [data-id="' + record.id + '"] div span')[2];
    //                //node.css('background-color', 'red');
    //                _node.innerText = "-" + _node.innerText;
    //            }
    //        });

    //        //display = function (id) {
    //        //    var s = $('li [data-id="' + id + '"] div span')[1];
    //        //    //console.log(id);

    //        //    if ($(s).attr('data-mode') == 'open') {
    //        //        //console.log('collapse');
    //        //        tree.collapse(tree.getNodeById(id));
    //        //    } else {
    //        //        //console.log('expand');
    //        //        tree.expand(tree.getNodeById(id));
    //        //    }

    //        //    tree.unselectAll();
    //        //    //console.log('>>>' + $(s).attr('data-mode'));
    //        //}

    //    } catch (error) {
    //        console.log(error.message);

    //    }

    //}
    ////onInitial();




    onInitial = function () {

        //var currentUrl = window.location.href;
        //console.log("Current URL: " + currentUrl);

        //var currentPath = window.location.pathname;
        //console.log("Current Path: " + currentPath);

        //var protocol = window.location.protocol;
        //var host = window.location.host;
        //console.log("Protocol: " + protocol);
        //console.log("Host: " + host);


        // Error as #demo is the `div` element
        //$('#demo').dataTable()

        // Selector too broad.
        // Error as `.display` is applied to both the div and the table
        //$('.display').dataTable();

        //var _tblHistorySQLLog = $('#tblHistorySQLLog').prop('class');
        //var _tblHistorySQLLog = $('#tblHistorySQLLog').prop('class');
        //var _tblHistorySQLLog = $('#tblHistorySQLLog').prop('class');
        //console.log(_tblHistorySQLLog);


        if ($('#tblHistorySQLLog').prop('class') != '' && $('#tblHistoryAction').prop('class') != '' && $('#tblHistoryLogin').prop('class') != '') {

            var tblHistorySQLLog = xDataTable.Initial({
                name: 'tblHistorySQLLog',
                //checking: 0,
                dom: '<"clear">',
                columnTitle: {
                    "EN": ['Controller Name', 'Action Name', 'SQL'],
                    "TH": ['Controller Name', 'Action Name', 'SQL'],
                    "JP": ['Controller Name', 'Action Name', 'SQL'],
                },
                column: [
                    { "data": "ControllerName" },
                    { "data": "ActionName" },
                    { "data": "SQL" }
                ],
                addnew: false,
                rowclick: (row) => {
                    xAjax.ToClipboard(row.SQL);
                }
            });


            var tblHistoryAction = xDataTable.Initial({
                name: 'tblHistoryAction',
                //checking: 0,
                //dom: '<"clear">',
                columnTitle: {
                    "EN": ['Action At', 'User Code', 'Controller Name', 'Action Name', 'Result', 'SQL'],
                    "TH": ['Action At', 'User Code', 'Controller Name', 'Action Name', 'Result', 'SQL'],
                    "JP": ['Action At', 'User Code', 'Controller Name', 'Action Name', 'Result', 'SQL'],
                },
                column: [
                    { "data": "ActionAt" },
                    { "data": "UserCode" },
                    { "data": "ControllerName" },
                    { "data": "ActionName" },
                    { "data": "Result" },
                    { "data": "SQL" }
                ],
                order: [[0, 'desc']],
                addnew: false,
                rowclick: (row) => {
                    xAjax.ToClipboard(row.SQL);
                }
            });


            var tblHistoryLogin = xDataTable.Initial({
                name: 'tblHistoryLogin',
                //checking: 0,
                //dom: '<"clear">',
                columnTitle: {
                    "EN": ['Date', 'Start', 'Finish', 'Process Time', 'User Code', 'Token'],
                    "TH": ['Date', 'Start', 'Finish', 'Process Time', 'User Code', 'Token'],
                    "JP": ['Date', 'Start', 'Finish', 'Process Time', 'User Code', 'Token'],
                },
                column: [
                    { "data": "LoginDate" },
                    { "data": "StartAt" },
                    { "data": "FinishAt" },
                    { "data": "ProcessTime" },
                    { "data": "Code" },
                    { "data": "Token" }
                ],
                order: [[0, 'desc']],
                addnew: false,
                rowclick: (row) => {
                }
            });
        }

    }
    onInitial();





    var _ModalHistory = document.getElementById('ModalHistory');
    _ModalHistory.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget; // Button that triggered the modal
        var url = '/History/' + button.getAttribute('data-bs-remote'); // URL from data-bs-remote attribute
        var modalBodyEl = _ModalHistory.querySelector('.modal-body');

        // Load the content from the URL into the modal body using AJAX or fetch
        fetch(url)
            .then(response => response.text())
            .then(data => {
                modalBodyEl.innerHTML = data;

                xHistory.onLoad();

                xSplash.hide();
            });
    });



    var _ModalHistorySQL = document.getElementById('ModalSystemHistory');
    _ModalHistorySQL.addEventListener('show.bs.modal', function (event) {

        xAjax.Post({
            url: 'HistoryLogin/getHistory',
            data: {
                'System': 'KB3',
                'UserCode': _UID_,
                'Controller': _CONTROLLER_,
                'Action': _PAGE_,
                'Date': xDate.Date()
            },
            then: function (result) {
                //console.log(result);
                //console.log(result);
                $('#tblHistorySQLLog').dataTable().fnClearTable();
                $('#tblHistoryAction').dataTable().fnClearTable();
                $('#tblHistoryLogin').dataTable().fnClearTable();
                if (result.data.sql.length > 0) {
                    $('#tblHistorySQLLog').dataTable().fnAddData(result.data.sql);
                    $('#tblHistoryAction').dataTable().fnAddData(result.data.action);
                    $('#tblHistoryLogin').dataTable().fnAddData(result.data.login);
                }
            }
        });

        //console.log('SQL');

    });







});

