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

    //initTheme = function () {
    //    setCookie('UI_ExpandIcon', _LAYOUT_.ExpandIcon);
    //    setCookie('UI_Header', _LAYOUT_.UI_Header);
    //    setCookie('UI_HeaderBrand', _LAYOUT_.UI_HeaderBrand);
    //    setCookie('UI_IconColor', _LAYOUT_.UI_IconColor);
    //    setCookie('UI_Layout', _LAYOUT_.UI_Layout);
    //    setCookie('UI_LinkColor', _LAYOUT_.UI_LinkColor);
    //    setCookie('UI_MenuColor', _LAYOUT_.UI_MenuColor);
    //    setCookie('UI_MenuIcon', _LAYOUT_.UI_MenuIcon);
    //    setCookie('UI_SideBar', _LAYOUT_.UI_SideBar);
    //}

    ////##### Set style selector language #####
    styleSelector = function (_i18n_) {
        setCookie('UI_ExpandIcon', _LAYOUT_.ExpandIcon);
        setCookie('UI_Header', _LAYOUT_.Header);
        setCookie('UI_HeaderBrand', _LAYOUT_.HeaderBrand);
        setCookie('UI_IconColor', _LAYOUT_.IconColor);
        setCookie('UI_Layout', _LAYOUT_.Layout);
        setCookie('UI_LinkColor', _LAYOUT_.LinkColor);
        setCookie('UI_MenuColor', _LAYOUT_.MenuColor);
        setCookie('UI_MenuIcon', _LAYOUT_.MenuIcon);
        setCookie('UI_SideBar', _LAYOUT_.SideBar);


        if (_i18n_.styleSelector == null) return false;

        $.each(_i18n_.styleSelector, function (e, v) {
            $('#styleSelector_' + e).html(v);
            $('#styleSelector_' + e).val(v);
        })

        $.each(_i18n_.styleSelector.ExpandIconOption, function (e, v) {
            $('#vertical-dropdown-icon option[value=' + e + ']').text(v);
            if (e == getCookie('UI_ExpandIcon')) $('#vertical-dropdown-icon option[value=' + e + ']').attr('selected', 'selected');
        })

        $.each(_i18n_.styleSelector.MenuIconOption, function (e, v) {
            $('#vertical-subitem-icon option[value=' + e + ']').text(v);
            if (e == getCookie('UI_MenuIcon')) $('#vertical-subitem-icon option[value=' + e + ']').attr('selected', 'selected');
        })

        $.each(_i18n_.styleSelector.SideBarOption, function (e, v) {
            $('#vertical-menu-effect option[value=' + e + ']').text(v);
            if (e == getCookie('UI_SideBar')) $('#vertical-menu-effect option[value=' + e + ']').attr('selected', 'selected');
        })

        $('[name="radio"][value="st6"]').attr('checked', 'true');
        if (getCookie('UI_IconColor') == 'st5') $('[name="radio"][value="st5"]').attr('checked', 'true');
    }
    styleSelector(_i18n_);

    $("[class=pcoded-navbar][navbar-theme=theme1]").attr('navbar-theme', (getCookie('UI_Layout') == '' ? 'themelight1' : getCookie('UI_Layout')));
    $('[class="navbar-logo"]').attr('logo-theme', (getCookie('UI_HeaderBrand') == '' ? 'theme4' : getCookie('UI_HeaderBrand')));
    $('[class="navbar header-navbar pcoded-header iscollapsed"][header-theme=theme1]').attr('header-theme', (getCookie('UI_Header') == '' ? 'theme4' : getCookie('UI_Header')));
    $('[class="pcoded-navbar"]').attr('active-item-theme', (getCookie('UI_LinkColor') == '' ? 'theme1' : getCookie('UI_LinkColor')));
    $('[class="pcoded-navigatio-lavel"]').attr('menu-title-theme', (getCookie('UI_MenuColor') == '' ? 'theme3' : getCookie('UI_MenuColor')));
    $('#pcoded').attr('nav-type', (getCookie('UI_IconColor') == '' ? 'st5' : getCookie('UI_IconColor')));
    $('[class="pcoded-hasmenu"]').attr('dropdown-icon', (getCookie('UI_ExpandIcon') == '' ? 'style2' : getCookie('UI_ExpandIcon')));
    $('[class="pcoded-hasmenu"]').attr('subitem-icon', (getCookie('UI_MenuIcon') == '' ? 'style4' : getCookie('UI_MenuIcon')));
    $('#pcoded').attr('vertical-effect', (getCookie('UI_SideBar') == '' ? 'shrink' : getCookie('UI_SideBar')));

    $('[class="navbar-theme"]').on('click', async function () {
        //_LAYOUT_.Theme = $(this).attr('navbar-theme');;
        changeLayOut('Layout', $(this).attr('navbar-theme'));
    })
    $('[class="logo-theme"]').on('click', async function () {
        //_LAYOUT_.HeaderBrand = $(this).attr('logo-theme');
        changeLayOut('HeaderBrand', $(this).attr('logo-theme'));
    })
    $('[class="header-theme"]').on('click', async function () {
        //_LAYOUT_.Header = $(this).attr('header-theme');
        changeLayOut('Header', $(this).attr('header-theme'));
    })
    $('[class="active-item-theme small"]').on('click', async function () {
        //_LAYOUT_.LinkColor = $(this).attr('active-item-theme');
        changeLayOut('LinkColor', $(this).attr('active-item-theme'));
    })
    $('[class="leftheader-theme small"]').on('click', async function () {
        //_LAYOUT_.MenuColor = $(this).attr('lheader-theme');
        changeLayOut('MenuColor', $(this).attr('lheader-theme'));
    })
    $('#vertical-dropdown-icon').on('click', async function () {
        //_LAYOUT_.ExpandIcon = $(this).val();
        changeLayOut('ExpandIcon', $(this).val());
    })
    $('#vertical-subitem-icon').on('click', async function () {
        //_LAYOUT_.MenuIcon = $(this).val();
        //setCookie('UI_SideBar', $(this).val());
        changeLayOut('MenuIcon', $(this).val());
    })
    $('#vertical-menu-effect').on('click', async function () {
        changeLayOut('SideBar', $(this).val());
    })
    $('[name="radio"][value="st6"]').on('click', async function () {
        changeLayOut('IconColor', $(this).val());
    })
    $('[name="radio"][value="st5"]').on('click', async function () {
        changeLayOut('IconColor', $(this).val());
    })


    changeLayOut = async function (pObject = '', pValue = 'theme1') {
        setCookie('UI_' + pObject, pValue);
        if (pObject == 'Header') setCookie('UI_HeaderBrand', pValue);
        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[erp].[themeLayout]",
                "@UID": _UID_,
                "@Layout": (pObject == 'Layout' ? pValue : ''),
                "@HeaderBrand": (pObject == 'Header' ? pValue : (pObject == 'HeaderBrand' ? pValue : '')),
                "@Header": (pObject == 'Header' ? pValue : ''),
                "@LinkColor": (pObject == 'LinkColor' ? pValue : ''),
                "@MenuColor": (pObject == 'MenuColor' ? pValue : ''),
                "@IconColor": (pObject == 'IconColor' ? pValue : ''),
                "@ExpandIcon": (pObject == 'ExpandIcon' ? pValue : ''),
                "@MenuIcon": (pObject == 'MenuIcon' ? pValue : ''),
                "@SideBar": (pObject == 'SideBar' ? pValue : '')
            },
        });
        //console.log(_dt.rows[0]);
    }




    
    ////##### Set layout language #####
    displayLayout = function (_i18n_) {
        if (_i18n_._Layout == null) return false;
        $.each(_i18n_._Layout, function (e, v) {
            $('#_Layout_' + e).html(v);
            $('#_Layout_' + e).val(v);
        })
    }
    displayLayout(_i18n_);
});

