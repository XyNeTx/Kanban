$(document).ready(async function () {

    $("#txtUserName").prop('readonly', true);
    $("#txtProcessDate").prop('disabled', true);
    $("#ddlShift").prop('disabled', true);

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

    $(".date-picker").datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
        todayHighlight: true,
        autoclose: true,
        showRightIcon: false
    });

    $(".date-picker").parent().find("button").prop("disabled", true);

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

    await (_xLib.GetCookie("isDev") == null || _xLib.GetCookie("isDev") == "")
        ? _xLib.SetCookie("isDev", 0) : null;

    initial = async function () {
        xSplash.show();

        initTheme();


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
                //if (getCookie('debug') == '') setCookie('debug', 0);
                //if (getCookie('debug') == '1') console.log(result);

                //if (getCookie('debug') == '1') $('#txtUserName').val('DEVELOPER');
                //if (getCookie('debug') == '1') $('#txtUserName').removeAttr('readonly');
                if (_xLib.GetCookie("isDev") == '1') $('h4').append('<h5 class="mt-3">DEVELOPER MODE</h5>');
                //console.log(result);
                $("#txtProcessDate").prop('disabled', true);
                $("#ddlShift").prop('disabled', true);

                if (result.userDetail.departmentCode.startsWith("44")
                    && !result.userDetail.departmentCode.includes("_"))
                {
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
                $('#txtIPAddress').val(result.addObj.f_IPAddress);
                _xLib.SetCookie('plantCode', result.userDetail.locationCode);
            },
            error: async function (error) {
                //console.log(error);
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

    let iSpy = 0;
    $('#imgHINOLogo').on('click',function (e) {
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

    xSplash.hide();

});

$("#btnSubmit").click(function () {
    $("#formAuthentication").one('submit', function (e) {
        e.preventDefault();
        var processDate = moment($('#txtProcessDate').val(),"DD/MM/YYYY").format("YYYY-MM-DD");
        var shift = $("#ddlShift").val() == 1 ? "D" : "N";
        document.cookie = `loginDate=${processDate}${shift}`;
        _xLib.SetCookie('plantCode', $('#ddlFactory').val());
        $("#ddlShift").prop('disabled', false);
        $("#txtProcessDate").prop('disabled', false);
        $(this).submit();
    });

});


<script type="text/javascript">
        //ImageUserFolder  foldername
    var rm_id = '';
    var rm_ty = '';
    var AttendanceCode = "";


    function oneclick_folder(data) {

        $('.f-active').removeClass('f-active');
    $(data).parent("a").addClass('f-active');

    if (typeof ($(data).attr("idimage")) != "undefined") {

        rm_ty = 'image';
    rm_id = $(data).attr("idimage");
                //console.log($(data).attr("idimage"));

            } else {

        rm_ty = 'folder';
    rm_id = $(data).attr("idfolder");
                //console.log($(data).attr("idfolder"));

            }

        }

    $('#img_Close').click(function () {
        $("[uk-navbar]").hide();

        });


    $('#img_delete').click(function () {

        //alert("รออัพเดท");
        //return false;
        $.ajax({
            method: "POST",
            url: "GETData.aspx",
            async: false,
            cache: false,
            timeout: 30000,
            data: { Controller: "DeleteFileAndFolder", Type: rm_ty, Id: rm_id, NameFile: $(".f-active").find("img").attr("alt"), NameFolder: $(".f-active").find("span").text() }
        })
            .success(function (res) {

                if (res == "Success") {
                    Swal.fire({
                        allowOutsideClick: false,
                        icon: 'success',
                        title: 'Success...',
                        text: 'ลบสำเร็จ!',
                        footer: '<a href="#" style="color:red;">    </a>'
                    })

                }


            });

        });



    function swapcre_img_folder(id) {

        $('[div_box="0"]').show();
    $('[div_box="1"]').hide();
    $('[div_box="2"]').hide();
    $('[div_box="' + id + '"]').show();
    if (id == 2) {
        $("#folder_list").hide();
                //$('.arrow-up2').removeClass("arrow-up2").addClass("arrow-up");
                //$('#div_word').text('เลือกตำแหน่งที่จะสร้าง');
            } else {
        $('.arrow-up').removeClass("arrow-up").addClass("arrow-up2");
    $('#div_word').text('เลือกโฟลเดอร์');
    $("#folder_list").show();
            }
        }

    $('#MyPhoto1').click(function () {

        $("#lv_2").hide();
    $('li[li_fol="1"]').remove();
    $('li[li_img="1"]').remove();


    listfile_and_folder_main();
        });


    $('#back').click(function () {
        $("#lv_2").hide();
    $('li[li_fol="1"]').remove();
    $('li[li_img="1"]').remove();

    listfile_and_folder_main();
        });

    function twoclick_folder(id) {


        $.ajax({
            method: "POST",
            url: "GETData.aspx",
            async: false,
            cache: false,
            timeout: 30000,
            data: { Controller: "ImageUserFileByFolder", FolderId: id }
        })
            .success(function (res) {

                $('li[li_fol="1"]').remove();
                $('li[li_img="1"]').remove();


                var AllFile = JSON.parse(res);


                $.each(AllFile['Table1'], function (key, value) {

                    //str_File += '<option value="' + value.Id + '">' + value.FolderName + '</option>';


                    var boxfile = '   <li li_img="1" class="contact h-card" data-contact="' + value.ID + '" draggable="true" tabindex="0" style=" height:60px;"> ';

                    //<li li_img='1' class="contact h-card" data-contact="{{:i}}" draggable="true" tabindex="0" style=" height:130px;">

                    boxfile += ' <a>';

                    boxfile += '<img style="height: 50px;width:80px;"  idImage=' + value.ID + ' onclick="oneclick_folder(this)"  src=https://hmmtweb01.hinothailand.com/FTPFILE/User/' + value.cb + '/' + value.FolderName + '/' + value.FileName + ".jpg" + ' alt="' + value.FileName + '" class="u-photo" id="data_contact' + value.ID + '"';

                    boxfile += '/>';
                    boxfile += '</a>';
                    //boxfile += '<span style="font-size:10px;color: black;" >' + value.FileName + '</span>';
                    boxfile += '</li>';

                    //boxfile += ' <img idfolder="' + value.Id + '" style="width:50%;" src="images/icon-fd.png" alt="" onClick="oneclick_folder(this)"  ondblclick="twoclick_folder(this)" />';
                    //    boxfolder += '<span style="font-size:10px" >' + value.FolderName + '</span></a></li>';

                    $("#list_folder").append(boxfile);

                    $("#lv_2").text(value.FolderName);
                    $("#lv_2").show();

                });

                //console.log(res);


                // บาสทำต่อ

            });

            //var id = $(data).attr('idfolder');
            //changedir_img(id);
            //$('#img_selete_type').val(id);
        }


    $('#bt_folder_create').click(function () {
            if ($('#name_folder').val().length == 0) {
        alert('กรุณากรอกชื่อFolder');
    return 0;
            }


    //return false;


    var name = $('#name_folder').val();
    //var check_name = $('#name_folder').val();
    //var op = $('#img_selete_type').children('option');



    $.ajax({
        method: "POST",
    url: "GETData.aspx",
    async: false,
    cache: false,
    timeout: 30000,
    data: {Controller: "ImageUserFolder", FolderName: name }
            })
    .success(function (res) {

                    if (res == 'double') {

        alert("ชื่อโฟลเดอร์นี้มีอยุ่แล้ว");

    return false;
                    } else {

        //$("#lv_2").text(name);
        //$("#lv_2").show();
        $("#name_folder").val("");
                        //$("#img_selete_type").append("<option value='" + res + "'> " + name + "</option>")

    alert("สร้างสำเร็จ Folder : " + name);

    var boxfolder = ' <li li_fol="1"><a>';
        boxfolder += ' <img idfolder="' + res + '" style="width:50%;" src="images/icon-fd.png" alt="' + name + '" onClick="oneclick_folder(this)" ondblclick="twoclick_folder("' + res + '")" />';
        boxfolder += '<span style="font-size:10px" >' + name + '</span></a></li>';

    $("#list_folder").append(boxfolder);

    // บาสทำต่อ

    return false;
                    }

                });



        });

    $('#sendimg').on('click', function () {

            if ($('#images').val() == '') {
        alert('กรุณาเลือกรูป');
    return 0;
            }

    var imagefileUser = $('#folder_list :selected').val();
    var FilenameUser = $('#images').val();

    var data = new FormData();

    var files = $("#images").get(0).files;

    var TWOMB = 5242880;

    if (TWOMB < files[0].size) {
        Swal.fire({
            allowOutsideClick: false,
            icon: 'error',
            title: 'Oops...',
            text: 'ขนาดไฟล์ต้องไม่เกิน 5 MB!',
            footer: '<a href="#" style="color:red;">    </a>'
        })

                return false;
            }

    var UserId = "20234111";
    var NameFolderUpload = $("#folder_list option:selected").text();

    if (UserId == "") {

                return false;
            }

    if (NameFolderUpload == "My Photo") {
        NameFolderUpload = "MyFolder";
            }

    var N_date = new Date().getTime();


    $.ajax({
        method: "POST",
    url: "GETData.aspx",
    async: false,
    cache: false,
    timeout: 30000,
    data: {Controller: "ImageUserFile", FileName: N_date, FolderName: NameFolderUpload }
            })
    .success(function (res) {



                    // Add the uploaded image content to the form data collection
                    if (files.length > 0) {
        data.append("UploadedImage", files[0]);
    data.append("Path", UserId + "\\" + NameFolderUpload + "\\" + N_date + ".jpg");
                    }

    // Make Ajax request with the contentType = false, and procesDate = false
    var ajaxRequest = $.ajax({
        type: "POST",
    url: "https://hmmtweb01.hinothailand.com/FTPFILE/api/fileupload/UploadFileByID",
    contentType: false,
    processData: false,
    data: data
                    });

    ajaxRequest.done(function (responseData, textStatus) {
                        if (textStatus == 'success') {
                            if (responseData != null) {
                                if (responseData.Key) {
                                    //NEW_FileName = responseData.Value;
                                    //console.log(responseData.Value);



                                    var boxNewFile = "";

    boxNewFile += ' <a>';

        boxNewFile += '<img style="height: 50px;width:80px;" idImage=' + N_date + ' onclick="oneclick_folder(this)" src=https://hmmtweb01.hinothailand.com/FTPFILE/User/' + UserId + '/' + NameFolderUpload + '/' + N_date + ".jpg" + ' alt="' + N_date + '" class="u-photo" id="data_contact' + N_date + '"';

            boxNewFile += '/>';
        boxNewFile += '</a>';
                                    //boxNewFile += '<span style="font-size:10px;color: black;" >' + N_date + '</span>';
    boxNewFile += '</li>';

$("#list_folder").append(boxNewFile);



                                    //alert(responseData.Value);
                                    //บาสทำต่อ
                                    //console.log(responseData);
                                    //console.log(textStatus);
                                    //console.log($("#fileUpload").val());


                                } else {
    alert(responseData.Value);
}
                            }
                        } else {
    alert(responseData.Value);
}
                    });

                });





        });

/// -------------------------------------------------------------   upload           ------------------------------------------------------------
var filename = "";
var fileid = "";


function validateSpecialcharFile(text) {
    var re = /^[-_ กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรฤลฦวศษสหฬอฮฯะัาำิีึืฺุูเแโใไๅๆ็่้๊๋์a-zA-Z0-9!@#$%\^&*)(.]*$/

    return re.test(String(text).toLowerCase());
}

function validateSpecialchar(text) {
    var re = /^[-_/: กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรฤลฦวศษสหฬอฮฯะัาำิีึืฺุูเแโใไๅๆ็่้๊๋์a-zA-Z0-9!@#$%\^&*)(.]*$/

    return re.test(String(text).toLowerCase());
}


function validatefile() {

    var TWOMB = 41943040;
    var filesize = 0;
    var Totalfilesize = 0;

    var lenfile = $("#ContentPlaceHolder1_FileUpload2")[0].files.length;


    for (var i = 0; i < lenfile; i++) {

        filesize = $("#ContentPlaceHolder1_FileUpload2")[0].files[i].size;

        if (typeof filesize != 'undefined' || filesize != null) {
            Totalfilesize = parseInt(Totalfilesize) + parseInt(filesize);
            // Do stuff
            if (parseInt(Totalfilesize) > parseInt(TWOMB)) {

                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'ไฟล์ทั้งหมด : มีขนาดเกิน 40 MB  กรุณาอัพโหลดไฟล์ใหม่!',
                    footer: '<a href="#" style="color:red;">    </a>'
                })


                $("#ContentPlaceHolder1_FileUpload2").val("");
                return;
            }


        }


    }




}

function Addtask() {


    //alert("  ปิดปรับปรุงระบบ การขออนุมัติเอกสารชั่วคราว!  ");
    //return false;


    var CheckFileID = $('#ContentPlaceHolder1_ChangeFile').select().val();
    var checkInchange = $("#PurchaseSelect").select().val();
    var checkSuppliername = $("#Spn1Select").select().val();

    if (CheckFileID == 1923 || CheckFileID == 1921) {

        if (checkSuppliername == 0) {
            Swal.fire({
                //allowOutsideClick: false,
                icon: 'error',
                title: 'Oops...',
                text: 'ต้องเลือก Supplier Name!',
                footer: '<a href="#" style="color:red;">    </a>'
            });

            return false;
        }

    }


    if (CheckFileID == 2057 || CheckFileID == 2064 || CheckFileID == 1942 || CheckFileID == 1933 || CheckFileID == 2226) {

        if (checkInchange == 0) {
            Swal.fire({
                //allowOutsideClick: false,
                icon: 'error',
                title: 'Oops...',
                text: 'ต้องเลือก รูปแบบการสั่งซื้อ !',
                footer: '<a href="#" style="color:red;">    </a>'
            });

            return false;
        }

        if ($("#mon-hi").prop("checked") == false && $("#mon-md").prop("checked") == false && $("#mon-lw").prop("checked") == false) {


            Swal.fire({
                //allowOutsideClick: false,
                icon: 'error',
                title: 'Oops...',
                text: 'กรุณาเลือกช่วงของยอดเงินการสั่งซื้อ!',
                footer: '<a href="#" style="color:red;">    </a>'
            });

            return false;
        }


    }


    if ($("#OutDept1").select().val() == null) {

        Swal.fire({
            allowOutsideClick: false,
            icon: 'error',
            title: 'Oops...',
            text: 'สายอนุมัตินอกฝ่ายที่ 1  :เลือกฝ่ายและผู้ตรวจสอบ!',
            footer: '<a href="#" style="color:red;">    </a>'
        });

        console.log("OKCase");

        return false;

    }

    if ($('#OutRoot').select().val() != 0) {

        for (var i = 1; i <= 5; i++) {

            var TextRootCheck = $('#HeadOutRoot' + i).find('input').eq(0).val();


            if ((($("#OutDeptUser" + i).select().val() != '0' && TextRootCheck == '') && $('#OutDept' + i).select().val() != '0')) {
                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'ต้องเลือกผู้ตรวจสอบเอกสาร และ ต้องมีสายอนุมัตินอกฝ่ายที่ ' + i + ' อย่างน้อย 1 คน!',
                    footer: '<a href="#" style="color:red;">    </a>'
                });

                return false;
            } else if ((($("#OutDeptUser" + i).select().val() != '0' && (TextRootCheck != '')) && $('#OutDept' + i).select().val() == '0')) {
                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'ต้องเลือกผู้ตรวจสอบเอกสาร และ ต้องมีสายอนุมัตินอกฝ่ายที่ ' + i + ' อย่างน้อย 1 คน!',
                    footer: '<a href="#" style="color:red;">    </a>'
                });
                return false;
            } else if (($('#OutDept' + i).select().val() == '0' && (TextRootCheck != '')) && $('#OutDept' + i).select().val() == '0') {
                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'ต้องเลือกผู้ตรวจสอบเอกสาร และ ต้องมีสายอนุมัตินอกฝ่ายที่ ' + i + ' อย่างน้อย 1 คน!',
                    footer: '<a href="#" style="color:red;">    </a>'
                });
                return false;
            } else if ((($("#OutDeptUser" + i).select().val() != '0' && TextRootCheck == '') && $('#OutDept' + i).select().val() == '0')) {
                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'ต้องเลือกผู้ตรวจสอบเอกสาร และ ต้องมีสายอนุมัตินอกฝ่ายที่ ' + i + ' อย่างน้อย 1 คน!',
                    footer: '<a href="#" style="color:red;">    </a>'
                });
                return false;
            }

            TextRootCheck = '';


        }


    }


    horizontalCustomStyle();
    //return false;
    //$(".elastic-pupop-sl").hide();

    //return false;

    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        //async: false,
        //cache: false,
        //timeout: 30000,
        data: { Controller: "CheckSaveForm" }
    })
        .success(function (con) {

            var checkSave = con.split("_");

            //AttendanceCode = "";
            //AttendanceCode = checkSave[1];


            //ตรวจสอบอีกที

            //if (checkSave[1] == "YES") {

            //    alert(" รหัสการลานี้ ต้องมีเอกสารแนบ! ");

            //    return false;
            //}



            if (checkSave[0] == "Success") {

                $("#BtnAddtask").attr("disable", "true")
                var FormID = "";
                var Filename_Tmp = "";
                var Templateid_Tmp = "";

                // length file
                //var max = $("#ContentPlaceHolder1_FileUpload2")[0].files[0].size


                //alert($("#ContentPlaceHolder1_FileUpload2")[0].files[0].size);
                //return false;

                var str = $('#HeadInRoot1 :input').val();

                var arr = str.split(",");


                var uname = $(".txt-user").text();
                var checkName = '';

                $.each(arr, function (i, j) {


                    if (j.trim() == uname.trim()) {

                        checkName = "Error"

                    }
                });

                if (checkName == "Error") {
                    $(".modal-mask").hide();
                    Swal.fire({
                        allowOutsideClick: false,
                        icon: 'error',
                        title: 'Oops...',
                        text: 'ไม่สามารถสร้างรายการได้!  มีชื่อคุณในรายการอนุมัติ!',
                        footer: '<a href="#" style="color:red;">    </a>'
                    })


                    return false;
                }

                if ($('#HeadInRoot1 :input').val().length == 0) {
                    $(".modal-mask").hide();
                    Swal.fire({
                        allowOutsideClick: false,
                        icon: 'error',
                        title: 'Oops...',
                        text: 'ไม่สามารถสร้างรายการได้!  ต้องมีรายชื่อผู้อนุมัติในฝ่ายของท่าน!',
                        footer: '<a href="#" style="color:red;">    </a>'
                    })

                    return false;


                }



                if ($('#HeadInRoot1 :input').val() == "" && $('#HeadOutRoot1 :input').val() == "") {
                    $(".modal-mask").hide();
                    Swal.fire({
                        allowOutsideClick: false,
                        icon: 'error',
                        title: 'Oops...',
                        text: 'ไม่สามารถสร้างรายการได้!  ไม่มีรายชื่อผู้อนุมัติ!',
                        footer: '<a href="#" style="color:red;">    </a>'
                    })

                    return false;


                }


                for (var i = 1; i <= 5; i++) {

                    var TextRootCheck = $('#HeadOutRoot' + i + ' :input').val();
                    var TextSplit = TextRootCheck.split(",");
                    var lnTextSplit = TextSplit.length;

                    for (var k = 0; k < lnTextSplit; k++) {

                        if (uname.trim() == TextSplit[k].trim()) {
                            $(".modal-mask").hide();
                            Swal.fire({
                                allowOutsideClick: false,
                                icon: 'error',
                                title: 'Oops...',
                                text: 'ไม่สามารถสร้างรายการได้!  มีชื่อคุณในรายการอนุมัติ!',
                                footer: '<a href="#" style="color:red;">    </a>'
                            });


                            return false;
                        }
                        //console.log(TextSplit[k]);

                    }
                    //console.log(TextRootCheck);
                }



                $.ajax({
                    method: "POST",
                    url: "GETData.aspx",
                    async: false,
                    cache: false,
                    timeout: 30000,
                    data: { Controller: "CheckContent" }
                })
                    .success(function (data) {


                        if (data == 'HinoWorkflow') {
                            $(".modal-mask").hide();

                            Swal.fire({
                                allowOutsideClick: false,
                                icon: 'error',
                                title: 'Oops...',
                                text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                                footer: '<a href="#" style="color:red;">    </a>'
                            }).then(function (isConfirm) {
                                if (isConfirm.isConfirmed == true) {
                                    location.href = "login.aspx";
                                }
                            });

                            return false;
                        }

                        var returnedData = JSON.parse(data);

                        if (typeof returnedData['Table1'][0] === "undefined") {
                            $(".modal-mask").hide();
                            Swal.fire({
                                allowOutsideClick: false,
                                icon: 'error',
                                title: 'Oops...',
                                text: 'กรุณาบันทึกฟอร์มของคุณก่อน โดยคลิกที่  บันทึกเอกสาร!',
                                footer: '<a href="#" style="color:red;">    </a>'
                            });




                            return false;
                        } else {

                            $.each(returnedData['Table1'], function (i, j) {
                                //console.log(j);

                                FormID = j.FormID;
                                Filename_Tmp = j.Filename;
                                Templateid_Tmp = j.Templateid;



                                if ($('#Title').val().length == 0) {
                                    $(".modal-mask").hide();
                                    Swal.fire({
                                        allowOutsideClick: false,
                                        icon: 'error',
                                        title: 'Oops...',
                                        text: 'กรุถณากรอกหัวข้อ!',
                                        footer: '<a href="#" style="color:red;">    </a>'
                                    });


                                    return false;
                                }


                                if (validateSpecialchar($('#Title').val()) == false) {
                                    $('#Title').focus();
                                    $('#Title').val("");
                                    $(".modal-mask").hide();

                                    Swal.fire({
                                        allowOutsideClick: false,
                                        icon: 'error',
                                        title: 'Oops...',
                                        text: 'Title ห้ามใช้อักขระพิเศษ กรุณาแก้ไขข้อมูล!',
                                        footer: '<a href="#" style="color:red;">    </a>'
                                    });


                                    return false;
                                }




                                var Title = $("#Title").val();
                                var Detail = "";
                                var fileuploadid = $("#ContentPlaceHolder1_ChangeFile").select().val();
                                var fileupload = Filename_Tmp;
                                var TableName = ""
                                var Note = $('#Headnote :input').val();

                                var NextApproved = str[0];
                                var Piority = "";


                                if ($('#ContentPlaceHolder1_FileUpload2').prop("files").length > 0) {
                                    var files = $("#ContentPlaceHolder1_FileUpload2")[0].files;

                                    for (var i = 0; i < files.length; i++) {
                                        //console.log(files[i].name)

                                        if (validateSpecialcharFile(files[i].name) == false) {
                                            $('#ContentPlaceHolder1_FileUpload2').focus();
                                            $(".modal-mask").hide();
                                            Swal.fire({
                                                allowOutsideClick: false,
                                                icon: 'error',
                                                title: 'Oops...',
                                                text: 'ชื่อไฟล์แนบ ห้ามใช้อักขระพิเศษ ห้ามเว้นวรรค ห้ามมีจุด กรุณาแก้ไขข้อมูล!',
                                                footer: '<a href="#" style="color:red;">    </a>'
                                            });

                                            return false;
                                        }


                                    }
                                }


                                if ($('#pri-hi').prop('checked') == true) {
                                    Piority = "h";
                                } else if ($('#pri-md').prop('checked') == true) {
                                    Piority = "m";
                                } else if ($('#pri-lw').prop('checked') == true) {
                                    Piority = "l";
                                } else {
                                    $(".modal-mask").hide();
                                    Swal.fire({
                                        allowOutsideClick: false,
                                        icon: 'error',
                                        title: 'Oops...',
                                        text: 'Priority Error! แจ้ง Admin!',
                                        footer: '<a href="#" style="color:red;">    </a>'
                                    });
                                }


                                if (CheckFileID == 2057 || CheckFileID == 2064 || CheckFileID == 1942 || CheckFileID == 1933 || CheckFileID == 2226) {

                                    if ($("#mon-md").prop("checked") == true || $("#mon-lw").prop("checked") == true) {

                                        Piority = "a";
                                    }


                                }

                                $.ajax({
                                    method: "POST",
                                    url: "GETData.aspx",
                                    async: false,
                                    cache: false,
                                    timeout: 30000,
                                    data: { Controller: "SaveRequest", Title: Title, Detail: Detail, Docid: fileuploadid, FileName: fileupload, Piority: Piority, Note: Note, NextApproved: NextApproved }

                                })
                                    .success(function (ID) {


                                        if (ID == 'HinoWorkflow') {
                                            $(".modal-mask").hide();
                                            Swal.fire({
                                                allowOutsideClick: false,
                                                icon: 'error',
                                                title: 'Oops...',
                                                text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                                                footer: '<a href="#" style="color:red;">    </a>'
                                            }).then(function (isConfirm) {
                                                if (isConfirm.isConfirmed == true) {
                                                    location.href = "login.aspx";
                                                }
                                            });

                                            return false;
                                        }

                                        if (ID == 'false') {

                                            $(".modal-mask").hide();
                                            Swal.fire({
                                                allowOutsideClick: false,
                                                icon: 'error',
                                                title: 'Oops...',
                                                text: 'ไม่สามารถบันทึกทึกข้อมูล Server ขัดข้อง!',
                                                footer: '<a href="#" style="color:red;">    </a>'
                                            });
                                            return false;
                                        }

                                        if (ID == 'Error') {
                                            $(".modal-mask").hide();
                                            Swal.fire({
                                                allowOutsideClick: false,
                                                icon: 'error',
                                                title: 'Oops...',
                                                text: 'ไม่สามารถบันทึกได้ ติดต่อเจ้าหน้าที่!',
                                                footer: '<a href="#" style="color:red;">    </a>'
                                            });
                                            return false;
                                        }

                                        var DeptInName = $("#InDept1").val();
                                        var Name_InrootApproved1 = $('#HeadInRoot1 :input').val();
                                        var Totalpeople = Name_InrootApproved1.split(",");



                                        if (Name_InrootApproved1 == "") {
                                            //  มี root  นอกสายงาน

                                            for (var i = 1; i <= 5; i++) {

                                                var ID_OutDept = $("#OutDept" + i).select().val();
                                                var Name_OutDept = $("#OutDept" + i + " option:selected").text();
                                                var ID_OutDeptUser = $("#OutDeptUser" + i).select().val();
                                                var Name_OutDeptUser = $("#OutDeptUser" + i + " option:selected").text();
                                                var NamTagRoot = $('#HeadOutRoot' + i + ' :input').val();
                                                var Totalpeople = NamTagRoot.split(",");

                                                //console.log("check" + i + ID_OutDept);
                                                //console.log("check" + i + ID_OutDeptUser);

                                                if (ID_OutDept != 0 && ID_OutDeptUser != 0) {


                                                    //console.log("check1");

                                                    $.ajax({
                                                        method: "POST",
                                                        url: "GETData.aspx",
                                                        async: false,
                                                        cache: false,
                                                        timeout: 30000,
                                                        data: {
                                                            Controller: "SaveRequestDetail", RequestId: ID, approvedline: i + 1,
                                                            approvedname: NamTagRoot, totalpeople: Totalpeople.length,
                                                            mumberofpeople: "0", department: Name_OutDept,
                                                            EmpRoot: ID_OutDeptUser,
                                                            departmentcode: ID_OutDept, EmpRootName: Name_OutDeptUser
                                                        }

                                                    })
                                                        .success(function (data) {


                                                            if (data == 'HinoWorkflow') {
                                                                $(".modal-mask").hide();
                                                                Swal.fire({
                                                                    allowOutsideClick: false,
                                                                    icon: 'error',
                                                                    title: 'Oops...',
                                                                    text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                                                                    footer: '<a href="#" style="color:red;">    </a>'
                                                                }).then(function (isConfirm) {
                                                                    if (isConfirm.isConfirmed == true) {
                                                                        location.href = "login.aspx";
                                                                    }
                                                                });


                                                                return false;
                                                            }




                                                        });


                                                    // code
                                                    //$("#RequestId").val(ID);
                                                    //$("#form1").submit();

                                                }


                                                if (i == 5) {
                                                    var sumbitflax = 0;
                                                    $(".modal-mask").hide();
                                                    Swal.fire({
                                                        allowOutsideClick: false,
                                                        icon: 'success',
                                                        title: 'Success...',
                                                        text: 'บันทึกข้อมูลเรียบร้อย!',
                                                        footer: '<a href="#" style="color:red;">    </a>'
                                                    }).then(function (isConfirm) {
                                                        if (isConfirm.isConfirmed == true) {
                                                            $("#ContentPlaceHolder1_RequestId").val(ID);
                                                            //$("#ContentPlaceHolder1_TableName").val(TableName[0]);
                                                            $("#ContentPlaceHolder1_FileName").val(fileupload);

                                                            if (sumbitflax == 0) {
                                                                sumbitflax = 1;

                                                                $.ajax({
                                                                    method: "POST",
                                                                    url: "GETData.aspx",
                                                                    async: false,
                                                                    cache: false,
                                                                    timeout: 30000,
                                                                    data: {
                                                                        Controller: "UpdateContent", RequestId: ID, approvedline: i + 1,

                                                                    }

                                                                })
                                                                    .success(function (data) {
                                                                        if (data == 'HinoWorkflow') {

                                                                            Swal.fire({
                                                                                allowOutsideClick: false,
                                                                                icon: 'error',
                                                                                title: 'Oops...',
                                                                                text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                                                                                footer: '<a href="#" style="color:red;">    </a>'
                                                                            }).then(function (isConfirm) {
                                                                                if (isConfirm.isConfirmed == true) {
                                                                                    location.href = "login.aspx";
                                                                                }
                                                                            });

                                                                            return false;
                                                                        }
                                                                        $('#form1').submit();
                                                                    });

                                                            }

                                                            return false;
                                                        }
                                                    });


                                                }

                                            }



                                            return false;



                                        } else {

                                            //  ไม่ มี root นอกสายงาน 

                                            $.ajax({
                                                method: "POST",
                                                url: "GETData.aspx",
                                                async: false,
                                                cache: false,
                                                timeout: 30000,
                                                data: {
                                                    Controller: "SaveRequestDetail", RequestId: ID, approvedline: "1",
                                                    approvedname: Name_InrootApproved1, totalpeople: Totalpeople.length,
                                                    mumberofpeople: "0", department: DeptInName,
                                                    EmpRoot: "",
                                                    departmentcode: "", EmpRootName: ""
                                                }

                                            })
                                                .success(function (result) {

                                                    if (result == 'HinoWorkflow') {
                                                        $(".modal-mask").hide();
                                                        Swal.fire({
                                                            allowOutsideClick: false,
                                                            icon: 'error',
                                                            title: 'Oops...',
                                                            text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                                                            footer: '<a href="#" style="color:red;">    </a>'
                                                        }).then(function (isConfirm) {
                                                            if (isConfirm.isConfirmed == true) {
                                                                location.href = "login.aspx";
                                                            }
                                                        });

                                                        return false;
                                                    }

                                                    if (result == 'false') {
                                                        $(".modal-mask").hide();
                                                        Swal.fire({
                                                            allowOutsideClick: false,
                                                            icon: 'error',
                                                            title: 'Oops...',
                                                            text: 'ไม่สามารถบันทึกได้การเชื่อมต่อ Server ขัดข้อง',
                                                            footer: '<a href="#" style="color:red;">    </a>'
                                                        });


                                                        return false;
                                                    }

                                                    if (result == 'Error') {
                                                        $(".modal-mask").hide();

                                                        Swal.fire({
                                                            allowOutsideClick: false,
                                                            icon: 'error',
                                                            title: 'Oops...',
                                                            text: 'ไม่สามารถบันทึกได้ ติดต่อเจ้าหน้าที่',
                                                            footer: '<a href="#" style="color:red;">    </a>'
                                                        });

                                                        return false;
                                                    }

                                                    for (var i = 1; i <= 5; i++) {

                                                        var ID_OutDept = $("#OutDept" + i).select().val();
                                                        var Name_OutDept = $("#OutDept" + i + " option:selected").text();
                                                        var ID_OutDeptUser = $("#OutDeptUser" + i).select().val();
                                                        var Name_OutDeptUser = $("#OutDeptUser" + i + " option:selected").text();
                                                        var NamTagRoot = $('#HeadOutRoot' + i + ' :input').val();
                                                        var Totalpeople = NamTagRoot.split(",");


                                                        if (ID_OutDept != 0 && ID_OutDeptUser != 0) {


                                                            $.ajax({
                                                                method: "POST",
                                                                url: "GETData.aspx",
                                                                async: false,
                                                                cache: false,
                                                                timeout: 30000,
                                                                data: {
                                                                    Controller: "SaveRequestDetail", RequestId: ID, approvedline: i + 1,
                                                                    approvedname: NamTagRoot, totalpeople: Totalpeople.length,
                                                                    mumberofpeople: "0", department: Name_OutDept,
                                                                    EmpRoot: ID_OutDeptUser,
                                                                    departmentcode: ID_OutDept, EmpRootName: Name_OutDeptUser
                                                                }

                                                            })
                                                                .success(function (data) {


                                                                    if (data == 'HinoWorkflow') {
                                                                        $(".modal-mask").hide();
                                                                        Swal.fire({
                                                                            allowOutsideClick: false,
                                                                            icon: 'error',
                                                                            title: 'Oops...',
                                                                            text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                                                                            footer: '<a href="#" style="color:red;">    </a>'
                                                                        }).then(function (isConfirm) {
                                                                            if (isConfirm.isConfirmed == true) {
                                                                                location.href = "login.aspx";
                                                                            }
                                                                        });
                                                                        return false;
                                                                    }



                                                                });


                                                            // code
                                                            //$("#RequestId").val(ID);
                                                            //$("#form1").submit();

                                                        }

                                                        if (i == 5) {
                                                            var sumbitflax = 0;
                                                            $(".modal-mask").hide();
                                                            Swal.fire({
                                                                allowOutsideClick: false,
                                                                icon: 'success',
                                                                title: 'Success...',
                                                                text: 'บันทึกข้อมูลเรียบร้อย!',
                                                                footer: '<a href="#" style="color:red;">    </a>'
                                                            }).then(function (isConfirm) {
                                                                if (isConfirm.isConfirmed == true) {
                                                                    // do something
                                                                    $("#ContentPlaceHolder1_RequestId").val(ID);
                                                                    //$("#ContentPlaceHolder1_TableName").val(TableName[0]);
                                                                    $("#ContentPlaceHolder1_FileName").val(fileupload);


                                                                    if (sumbitflax == 0) {

                                                                        sumbitflax = 1;
                                                                        $.ajax({
                                                                            method: "POST",
                                                                            url: "GETData.aspx",
                                                                            async: false,
                                                                            cache: false,
                                                                            timeout: 30000,
                                                                            data: {
                                                                                Controller: "UpdateContent", RequestId: ID, approvedline: i + 1,

                                                                            }

                                                                        })
                                                                            .success(function (data) {


                                                                                if (data == 'HinoWorkflow') {
                                                                                    $(".modal-mask").hide();
                                                                                    Swal.fire({
                                                                                        allowOutsideClick: false,
                                                                                        icon: 'error',
                                                                                        title: 'Oops...',
                                                                                        text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                                                                                        footer: '<a href="#" style="color:red;">    </a>'
                                                                                    }).then(function (isConfirm) {
                                                                                        if (isConfirm.isConfirmed == true) {
                                                                                            location.href = "login.aspx";
                                                                                        }
                                                                                    });

                                                                                    return false;
                                                                                }


                                                                                $('#form1').submit();


                                                                            });

                                                                    }



                                                                    return false;
                                                                }
                                                            });


                                                        }
                                                    }





                                                });



                                        }


                                        ////////////////////////////////////////////////////////////////////////////////////////////////





                                    });







                            });

                        }
                    });


                //------------------------------------------------------

            } else {
                $(".modal-mask").hide();
                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: ' กรุณาบันทึกเอกสารให้เรียบร้อยก่อน!',
                    footer: '<a href="#" style="color:red;">    </a>'
                })

                return false;


            }


        });



    return false;
}

function GETTagInput(Tagid) {

    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        async: false,
        cache: false,
        timeout: 30000,
        data: { Controller: "Taginput" }

    })

        .success(function (data) {

            if (data == 'HinoWorkflow') {

                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                    footer: '<a href="#" style="color:red;">    </a>'
                }).then(function (isConfirm) {
                    if (isConfirm.isConfirmed == true) {
                        location.href = "login.aspx";
                    }
                });

                return false;
            }

            var result = data.split(",");
            var tags_array = [];

            $.each(result, function (key, value) {
                tags_array[key] = value
            });

            $(Tagid).tagsInput({

                'autocomplete': {
                    source: tags_array
                }

            });




        });
}

function listfile_and_folder_main() {


    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        async: false,
        cache: false,
        timeout: 30000,
        data: { Controller: "ListUserFolder" }

    })

        .success(function (data) {

            //console.log(data);

            var AllFolder = jQuery.parseJSON(data);
            var str_Folder = "";

            $.each(AllFolder['Table1'], function (key, value) {

                str_Folder += '<option value="' + value.Id + '">' + value.FolderName + '</option>';


                var boxfolder = ' <li li_fol="1" ><a>';
                boxfolder += ' <img idfolder="' + value.Id + '" style="width:50%;" src="images/icon-fd.png" alt=""  onClick="oneclick_folder(this)"   ondblclick="twoclick_folder(' + value.Id + ')" />';
                boxfolder += '<span style="font-size:10px" >' + value.FolderName + '</span></a></li>';

                $("#list_folder").append(boxfolder);


            });

            $("#img_selete_type").append(str_Folder)
            $("#folder_list").append(str_Folder)


            $.ajax({
                method: "POST",
                url: "GETData.aspx",
                async: false,
                cache: false,
                timeout: 30000,
                data: { Controller: "ListUserFile" }

            })

                .success(function (DFile) {


                    var AllFile = jQuery.parseJSON(DFile);
                    var str_File = "";

                    //console.log(AllFile);

                    $.each(AllFile['Table1'], function (key, value) {

                        //str_File += '<option value="' + value.Id + '">' + value.FolderName + '</option>';


                        var boxfile = '   <li li_img="1" class="contact h-card" data-contact="' + value.Id + '" draggable="true" tabindex="0" style=" height:60px;"> ';

                        //<li li_img='1' class="contact h-card" data-contact="{{:i}}" draggable="true" tabindex="0" style=" height:130px;">

                        boxfile += ' <a>';

                        if (value.FolderName == '') {
                            boxfile += '<img style="height: 50px;width:80px;" idImage=' + value.Id + ' onClick="oneclick_folder(this)" src=https://hmmtweb01.hinothailand.com/FTPFILE/User/' + value.cb + '/MyFolder/' + value.FileName + ".jpg" + ' alt="' + value.FileName + '" class="u-photo" id="data_contact' + value.Id + '"';
                        }
                        else {
                            boxfile += '<img style="height: 50px;width:80px;" idImage=' + value.Id + ' onClick="oneclick_folder(this)" src=https://hmmtweb01.hinothailand.com/FTPFILE/User/' + value.cb + '/' + value.FolderName + '/' + value.FileName + ".jpg" + ' alt="' + value.FileName + '" class="u-photo" id="data_contact' + value.Id + '"';

                        }

                        boxfile += '/>';
                        boxfile += '</a>';
                        //boxfile += '<span style="font-size:10px;color: black;" >' + value.FileName + '</span>';
                        boxfile += '</li>';

                        //boxfile += ' <img idfolder="' + value.Id + '" style="width:50%;" src="images/icon-fd.png" alt="" onClick="oneclick_folder(this)"  ondblclick="twoclick_folder(this)" />';
                        //    boxfolder += '<span style="font-size:10px" >' + value.FolderName + '</span></a></li>';

                        $("#list_folder").append(boxfile);


                    });


                });

        });


}

$(document).ready(function () {
    //$("#Step2").hide();

    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        //async: false,
        //cache: false,
        //timeout: 30000,
        data: { Controller: "GetAutoComplateTitle" }
    })
        .success(function (data) {
            var res = JSON.parse(data);

            var availableTags = [];
            $.each(res['Table1'], function (key, value) {
                availableTags.push(value.title);
            });
            $("#Title").autocomplete({
                source: availableTags
            });


        });





    $("#Setiframe").append(' <iframe  deliframe src="RequestTemplate.aspx?cid=yes&id=" style="height: 1323px;width: 100%;"></iframe>');
    GETTagInput(".form-tags");



    $("#ContentPlaceHolder1_ChangeFile option[value='1668']").remove();
    $("#ContentPlaceHolder1_ChangeFile option[value='1755']").remove();
    $("#ContentPlaceHolder1_ChangeFile option[value='1752']").remove();
    $("#ContentPlaceHolder1_ChangeFile option[value='1754']").remove();
    $("#ContentPlaceHolder1_ChangeFile option[value='1758']").remove();

    $("#ContentPlaceHolder1_ChangeFile option[value='1629']").remove();
    $("#ContentPlaceHolder1_ChangeFile option[value='1800']").remove();



    // Root by Login

    //console.log("THITIMA.PRAKOBWIT,ROJCHANA.SRIVISES,");



    //bt_folder_create

    listfile_and_folder_main();

    if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1668") {
        //$("#Title").val("ขออนุมัติการลา 2021");
        //ChangeFileTemp()
        //$("#R2").hide();
        //$("#R3").hide();
        //$("#R4").hide();
        //$("#R5").hide();

    }

    else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1755" || $('#ContentPlaceHolder1_ChangeFile').select().val() == "1754" || $('#ContentPlaceHolder1_ChangeFile').select().val() == "1752" || $('#ContentPlaceHolder1_ChangeFile').select().val() == "1758") {


        //$("#Title").val("ขออนุมัติการลา Supplier 2021");
        //ChangeFileTemp()
        //$("#R2").hide();
        //$("#R3").hide();
        //$("#R4").hide();
        //$("#R5").hide();

    }

    else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1800") {
        $("#Title").val("เปลี่ยนแปลงประวัติ ");
        ChangeFileTemp()
        $("#R2").hide();
        $("#R3").hide();
        $("#R4").hide();
        $("#R5").hide();
    }


    else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1737") {
        $("#Title").val("เบิกเงินสงเคราะห์บุตร");
        ChangeFileTemp()
        $("#R1").hide();
        $("#R2").hide();
        $("#R3").hide();
        $("#R4").hide();
        $("#R5").hide();
    }


    else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1649") {
        ChangeFileTemp()
    }


    else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1767") {
        ChangeFileTemp()
    }


    else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1765") {
        ChangeFileTemp()
    }
    else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1717") {
        ChangeFileTemp()
    }
    else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1942") {
        ChangeFileTemp()
    } else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1933") {
        ChangeFileTemp()
    }

    else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "2057" || $('#ContentPlaceHolder1_ChangeFile').select().val() == "2064" || $('#ContentPlaceHolder1_ChangeFile').select().val() == "2226") {

        ChangeFileTemp();
    } else if ($('#ContentPlaceHolder1_ChangeFile').select().val() == "1923" || $('#ContentPlaceHolder1_ChangeFile').select().val() == "1921") {

        ChangeFileTemp();
    }


    $("#OutrootApproved1_tagsinput").attr("style", "width: auto; min-height: auto; height: auto;margin-top: 35px;")
    $("#OutrootApproved2_tagsinput").attr("style", "width: auto; min-height: auto; height: auto;margin-top: 35px;")
    $("#OutrootApproved3_tagsinput").attr("style", "width: auto; min-height: auto; height: auto;margin-top: 35px;")
    $("#OutrootApproved4_tagsinput").attr("style", "width: auto; min-height: auto; height: auto;margin-top: 35px;")
    $("#OutrootApproved5_tagsinput").attr("style", "width: auto; min-height: auto; height: auto;margin-top: 35px;")

});

function GETTagInput(Tagid) {

    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        async: false,
        cache: false,
        timeout: 30000,
        data: { Controller: "Taginput" }

    })

        .success(function (data) {

            if (data == 'HinoWorkflow') {

                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                    footer: '<a href="#" style="color:red;">    </a>'
                }).then(function (isConfirm) {
                    if (isConfirm.isConfirmed == true) {
                        location.href = "login.aspx";
                    }
                });
                return false;
            }

            var result = data.split(",");
            var tags_array = [];

            $.each(result, function (key, value) {
                tags_array[key] = value
            });

            $(Tagid).tagsInput({

                'autocomplete': {
                    source: tags_array
                }

            });
            //$(".tag").addClass("tag-bas");
            //$(".tag-remove").remove();
        });

}
function SelectSectonListEmp(id, subid, Headid, Tagid) {

    $(Headid).children().remove()
    $(Headid).append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="' + Tagid + '" />')
    GETTagInput('#' + Tagid);

    var val = $(id).select().val();
    $(subid).empty();
    $(subid).append('<option value="0" > --- select ---</option>  ');
    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        async: false,
        cache: false,
        timeout: 30000,
        data: { Controller: "SelectDeptByID", Dept: val }
    })
        .success(function (data) {

            if (data == 'HinoWorkflow') {

                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                    footer: '<a href="#" style="color:red;">    </a>'
                }).then(function (isConfirm) {
                    if (isConfirm.isConfirmed == true) {
                        location.href = "login.aspx";
                    }
                });
                return false;
            }

            if (data != 'false') {
                $(subid).append(data);
            }
        });
}
function SelectEmpListRoot(id, Headid, Tagid) {

    var UserID = $(id).val();

    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        async: false,
        cache: false,
        timeout: 30000,
        data: { Controller: "SelectRoottByID", UserID: UserID }
    })
        .success(function (data) {


            if (data == 'HinoWorkflow') {
                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                    footer: '<a href="#" style="color:red;">    </a>'
                }).then(function (isConfirm) {
                    if (isConfirm.isConfirmed == true) {
                        location.href = "login.aspx";
                    }
                });
                return false;
            }
            if (data != 'false') {

                $(Headid).children().remove()
                $(Headid).append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="' + Tagid + '" />')
                $("#" + Tagid).val(data);
                GETTagInput('#' + Tagid);
                //console.log(data);
                var FileID = $('#ContentPlaceHolder1_ChangeFile').select().val();
                if (FileID == "1942" || FileID == "1933" || FileID == "2057" || FileID == "2064" || FileID == "2226") {

                    CalRoutePsOther();

                }



            }
        });
}


// น้อกุ้ง
function CalRoutePsOther() {

    //console.log(id);

    var EmpCode = $('#OutDeptUser1').select().val();
    var position = "";

    if (EmpCode == "0") {

        Swal.fire({
            //allowOutsideClick: false,
            icon: 'error',
            title: 'Oops...',
            text: 'ต้องเลือก รูปแบบการสั่งซื้อ!',
            footer: '<a href="#" style="color:red;">    </a>'
        });

        $("#mon-hi").prop("checked", false);
        $("#mon-md").prop("checked", false);
        $("#mon-lw").prop("checked", false);

        return false;

    }

    if ($("#mon-hi").prop("checked") == true || $("#mon-md").prop("checked") == true || $("#mon-lw").prop("checked") == true) {

        if ($("#mon-hi").prop("checked") == true) {

            $("#HeadInRoot1").children().remove()
            $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
            $("#InrootApproved1").val(OldRoot);
            GETTagInput('#InrootApproved1');
            //return false;
            position = "";

        } else if ($("#mon-md").prop("checked") == true) {

            position = "22";

        } else if ($("#mon-lw").prop("checked") == true) {
            position = "61";

        }

        // inroot
        if (position != "") {
            $.ajax({
                method: "POST",
                url: "GETData.aspx",
                //async: false,
                //cache: false,
                //timeout: 30000,
                data: { Controller: "GetPSVPUP", EmpCode: EmpCode, position: position }
            })
                .success(function (data) {
                    var res = JSON.parse(data);

                    var NewRootVp = "";

                    if (position == "22") {
                        //NewRootVp = OldRoot + ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;

                        try {

                            NewRootVp = OldRoot + ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;

                        }
                        catch (err) {

                            NewRootVp = OldRoot + ',NOPPADOL.VUTIDECHKAMJORN';

                        }



                        $("#HeadInRoot1").children().remove();

                        $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
                        $("#InrootApproved1").val(NewRootVp);
                        GETTagInput('#InrootApproved1');
                    } else if (position == "61") {
                        NewRootVp = OldRoot + ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;
                        NewRootVp = NewRootVp + ',' + res["Table1"][1].Name + "." + res["Table1"][1].Surname;
                        $("#HeadInRoot1").children().remove();

                        $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
                        $("#InrootApproved1").val(NewRootVp);
                        GETTagInput('#InrootApproved1');
                    }




                    console.log(res);


                });
        }


        // inroot

        // outroot
        $.ajax({
            method: "POST",
            url: "GETData.aspx",
            //async: false,
            //cache: false,
            //timeout: 30000,
            data: { Controller: "PSGetPSVPUP", EmpCode: EmpCode, position: position }
        })
            .success(function (data) {
                var res = JSON.parse(data);

                var PSNewRootVp = "";


                if (position == "22") {
                    //PSNewRootVp = ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;


                    try {

                        PSNewRootVp = PSNewRootVp + ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;

                    }
                    catch (err) {

                        PSNewRootVp = PSNewRootVp + ',NOPPADOL.VUTIDECHKAMJORN';

                    }




                } else if (position == "61") {
                    PSNewRootVp = ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;
                    PSNewRootVp = PSNewRootVp + ',' + res["Table1"][1].Name + "." + res["Table1"][1].Surname;
                }

                ////////

                var PurchaseAdmin = $('#OutDeptUser1').select().val();

                var PurchaseFileID = $('#ContentPlaceHolder1_ChangeFile').select().val();
                if (PurchaseFileID == "2057" || PurchaseFileID == "2064" || PurchaseFileID == "1942" || PurchaseFileID == "1933" || PurchaseFileID == "2226") {

                    var Headid = "#HeadOutRoot1";
                    var Tagid = "OutrootApproved1";
                    //var PurchaseAdmin = ["20041635", "20122721", "20051684"];

                    $.ajax({
                        method: "POST",
                        url: "GETData.aspx",
                        //async: false,
                        //cache: false,
                        //timeout: 30000,
                        data: { Controller: "SelectRoottByID", UserID: PurchaseAdmin }
                    })
                        .success(function (data) {


                            if (data == 'HinoWorkflow') {
                                Swal.fire({
                                    allowOutsideClick: false,
                                    icon: 'error',
                                    title: 'Oops...',
                                    text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                                    footer: '<a href="#" style="color:red;">    </a>'
                                }).then(function (isConfirm) {
                                    if (isConfirm.isConfirmed == true) {
                                        location.href = "login.aspx";
                                    }
                                });
                                return false;
                            }
                            if (data != 'false') {

                                $("#OutDeptUser1").select().val(PurchaseAdmin)
                                $(Headid).children().remove();
                                $(Headid).append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="' + Tagid + '" />')

                                //if (PurchaseAdmin == '11540196' || PurchaseAdmin == '20112591' || PurchaseAdmin == '20153446') {
                                //	data = 'TEERASAK.NUTIPRAPUN,' + data;
                                //}

                                $("#" + Tagid).val(data + PSNewRootVp);
                                GETTagInput("#" + Tagid);
                                //console.log(data);

                            }
                        });


                    //basxxx

                }




                console.log(res);


            });
    }




}


function ChangeFileTemp() {

    //if (FileID == 1668 || FileID == 1755 || FileID == 1752 || FileID == 1754 || FileID == 1758) {

    //    alert("ปิดระบบการลา!");
    //    location.href = "Request.aspx";
    //    return false;
    //}

    var FileID = $('#ContentPlaceHolder1_ChangeFile').select().val();
    $("[deliframe]").remove();

    if (FileID == "1737") { $("#Title").val("เบิกเงินสงเคราะห์บุตร"); }

    if (FileID == "2057" || FileID == "2064" || FileID == "1942" || FileID == "1933" || FileID == "2226") {
        $("#PurchaseType").show();
        $("#PurchaseTypeMoney").show();


    } else {
        $("#PurchaseType").hide();
        $("#PurchaseTypeMoney").hide();

    }

    if (FileID == "1921" || FileID == "1923") {
        GetSpn1Supplier();
        $("#Spn1Type").show();
    } else {
        $("#Spn1Type").hide();
    }


    // 2819
    if (FileID == 1668 || FileID == 1755 || FileID == 1752 || FileID == 1754 || FileID == 1758) {
        $("#Case2819").show();
        $('#pri-user').prop('checked', true)
    } else {
        $("#Case2819").hide();
    }
    //2819

    if (FileID == 0) {

        $("#Setiframe").append(' <iframe  deliframe src="RequestTemplate.aspx?cid=yes&id=" style="height: 1323px;width: 100%;"></iframe>');
        $("#Step2").hide();

    } else {
        //$("#Step2").show();

        $.ajax({
            method: "POST",
            url: "GETData.aspx",
            //async: false,
            //cache: false,
            //timeout: 30000,
            data: { Controller: "GetFileBYTemplate", Templateid: FileID }

        })
            .success(function (data) {


                if (data == 'HinoWorkflow') {
                    Swal.fire({
                        allowOutsideClick: false,
                        icon: 'error',
                        title: 'Oops...',
                        text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                        footer: '<a href="#" style="color:red;">    </a>'
                    }).then(function (isConfirm) {
                        if (isConfirm.isConfirmed == true) {
                            location.href = "login.aspx";
                        }
                    });
                    return false;
                }


                if (data == "3143" || data == "3139" || data == "3136" || data == "3140") {

                    $("#Setiframe").append(' <iframe  deliframe src="RequestTemplateLeaveSup.aspx?cid=yes&id=' + data + '" style="height: 1323px;width: 100%;"></iframe>');

                } else

                    if (data == "3014" || data == "3979" || data == "3978" || data == "3979" || data == "4034") {

                        $("#Setiframe").append(' <iframe  deliframe src="RequestTemplateFy2020.aspx?cid=yes&id=' + data + '" style="height: 1323px;width: 100%;"></iframe>');

                    } else if (data == "3267" || data == "3299") {

                        $("#Setiframe").append(' <iframe  deliframe src="Orderslip.aspx?cid=yes&id=' + data + '" style="height: 1323px;width: 100%;"></iframe>');

                    } else {

                        $("#Setiframe").append(' <iframe  deliframe src="RequestTemplate.aspx?cid=yes&id=' + data + '" style="height: 1323px;width: 100%;"></iframe>');

                    }

                $("#step2").show();


            });

        //$("[Box_UploadImage]").show();

        //if (FileID == "1340") {


        //}

    }



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //
    //   <iframe src="Createtemplate.aspx" style="height: 1323px;width: 100%;"></iframe>

    //$("#ExcelOT").hide();


    //if (FileID == 62) {
    //    $("#ExcelOT").show();
    //}

    for (var i = 1; i <= 5; i++) {
        $("#HeadOutRoot" + i).children().remove()
        $("#HeadOutRoot" + i).append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id=OutrootApproved' + i + ' />')
        //$("#OutrootApproved"+i).val("'" + value['TempRoot'] + "'");
        GETTagInput('#OutrootApproved' + i);
        $("#OutDept" + i).select().val("0");
        $("#OutDeptUser" + i).select().val("0");

    }

    if (FileID == 0) {
        return false;
    }

    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        //async: false,
        //cache: false,
        //timeout: 30000,
        data: { Controller: "GetDeptByFileID", FileID: FileID }

    })
        .success(function (data) {


            if (data == 'HinoWorkflow') {

                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                    footer: '<a href="#" style="color:red;">    </a>'
                }).then(function (isConfirm) {
                    if (isConfirm.isConfirmed == true) {
                        location.href = "login.aspx";
                    }
                });
                return false;
            }

            var v = jQuery.parseJSON(data);
            var i = 0;

            //console.log(v);

            $.each(v['Table1'], function (key, value) {

                if (value['DeptIn'] == "No") {


                    $("#HeadInRoot1").children().remove()
                    $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')

                    if (FileID == "1699") {

                        var locationCode = "1";
                        var rootGM = "THITIMA.PRAKOBWIT,ROJCHANA.SRIVISES,";
                        var rootGMTo = rootGM.split(',');
                        var SLocation = value['DeptInUser'].split(",");
                        if (locationCode == 1) {
                            $("#InrootApproved1").val(SLocation[0] + ',' + rootGMTo[2]);
                        } else if (locationCode == 2) {
                            $("#InrootApproved1").val(SLocation[1] + ',' + rootGMTo[2]);
                        } else if (locationCode == 3) {
                            $("#InrootApproved1").val(SLocation[2] + ',' + rootGMTo[2]);
                        } else {
                            $("#InrootApproved1").val(SLocation[0] + ',' + rootGMTo[2]);
                        }

                    } else {
                        $("#InrootApproved1").val(value['DeptInUser']);
                    }

                    GETTagInput('#InrootApproved1');


                    $("#Headnote").children().remove()
                    $("#Headnote").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="note" />')
                    //$("#note").val(value['DeptInUser']);
                    GETTagInput('#note');

                } else {

                    $.ajax({
                        method: "POST",
                        url: "GETData.aspx",
                        //async: false,
                        //cache: false,
                        //timeout: 30000,
                        data: { Controller: "GetRootLogin" }

                    })
                        .success(function (data) {


                            if (data == 'HinoWorkflow') {

                                Swal.fire({
                                    allowOutsideClick: false,
                                    icon: 'error',
                                    title: 'Oops...',
                                    text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                                    footer: '<a href="#" style="color:red;">    </a>'
                                }).then(function (isConfirm) {
                                    if (isConfirm.isConfirmed == true) {
                                        location.href = "login.aspx";
                                    }
                                });
                                return false;
                            }

                            $("#Title").val("");

                            if (FileID == "1245" || FileID == "1668" || FileID == "1755" || FileID == "1752" || FileID == "1754" || FileID == "1758") {

                                $("#HeadOutRoot1").show();

                                if (FileID == "1668" || FileID == "1755" || FileID == "1752" || FileID == "1754" || FileID == "1758") {
                                    $("#Title").val("ขออนุมัติการลา ");
                                    //$("#HeadOutRoot1").hide();
                                } else {

                                    $("#Title").val("ขอเบิกรองเท้าเซฟตี้");

                                    $("#OutDept1").attr('disabled', 'disabled');
                                    $("#OutDeptUser1").attr('disabled', 'disabled');
                                }

                                //ChangeFileTemp()
                                $("#R2").hide();
                                $("#R3").hide();
                                $("#R4").hide();
                                $("#R5").hide();

                                $("#HeadInRoot1").children().remove()
                                $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')


                                $("#InrootApproved1").val("THITIMA.PRAKOBWIT,");

                                GETTagInput('#InrootApproved1');


                            } else {


                                if (FileID == '1765') {
                                    // iso 45001

                                    var locationCode = "1";
                                    //data
                                    if (locationCode == '1') {
                                        data = 'PIYAPORNPHAT.KRAISINGSOMNUEK,' + data;
                                    } else if (locationCode == '2') {
                                        data = 'WUTTICHAI.SUPPAWUT,' + data;
                                    } else if (locationCode == '3') {
                                        data = 'SANCHUMAKORN.SA-NGIAM,' + data;
                                    }
                                }

                                if (FileID == '1767') {
                                    // iso 45001

                                    var locationCode = "1";
                                    //data
                                    if (locationCode == '1') {
                                        data = 'JIRAYUT.KHAPCHADA,' + data;
                                    } else if (locationCode == '2') {
                                        data = 'SUPIYA.INTAPAJ,' + data;
                                    } else if (locationCode == '3') {
                                        data = 'WARISA.WONGJEAMRAT,' + data;
                                    } else if (locationCode == '4') {
                                        data = 'JIRAPAT.PONINTAWONG,' + data;
                                    }
                                }



                                if (FileID == '2185') {

                                    data += 'KENJI.TAMURA,ATHIKOM.POTHIKANON,RYOJI.FUKUDA';
                                }


                                $("#Title").val("");
                                $("#HeadOutRoot1").show();

                                $("#HeadInRoot1").children().remove()
                                $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
                                $("#InrootApproved1").val(data);
                                GETTagInput('#InrootApproved1');

                            }


                            OldRoot = $('#HeadInRoot1 :input').val();
                        });

                }
                if (value['DeptOut'] != 0) {

                    i = (key + 1);

                    //console.log(" tagsinput -->" + i);


                    //console.log("'" + value['TempRoot'] + "'");

                    $("#OutDept" + i).val(value['DepartmentCode']);

                    getlistname(value['DepartmentCode'], '#OutDeptUser' + i, value['UserID']);

                    $("#HeadOutRoot" + i).children().remove()
                    $("#HeadOutRoot" + i).append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id=OutrootApproved' + i + ' />')
                    $("#OutrootApproved" + i).val(value['TempRoot']);
                    GETTagInput('#OutrootApproved' + i);

                    $("#R" + i).show();

                }
            });

            //console.log(i);

            //for (var j = i+1; j <= 5; j++) {
            //    $("#R" + j).hide();
            //}

            if (FileID == '1788' || FileID == '1804') {

                var locationCode = "1";
                //data
                if (locationCode == '1') {
                    //data = 'PIMPIMOL.RUNSOMRONG,' + data;
                    $("#OutDeptUser1").val("20133089");
                    SelectEmpListRoot('#OutDeptUser1', '#HeadOutRoot1', 'OutrootApproved1');
                } else if (locationCode == '2') {
                    $("#OutDeptUser1").val("19960817");
                    SelectEmpListRoot('#OutDeptUser1', '#HeadOutRoot1', 'OutrootApproved1');
                    //data = 'SUPIYA.INTAPAJ,' + data;
                } else if (locationCode == '3') {
                    $("#OutDeptUser1").val("20112592");
                    SelectEmpListRoot('#OutDeptUser1', '#HeadOutRoot1', 'OutrootApproved1');
                    //data = 'SIRIWAN.TOPRASERT,' + data;
                }



            }

        });



    if (FileID == "1261" || FileID == "1262") {
        $("#OutDeptUser1").prop("disabled", true);
        $("#OutDept1").prop("disabled", true);

    }



    if (FileID == "1680" || FileID == "1686") {

        $(".tag-remove").remove();
        $("#HeadInRoot1").children().eq(1).children().eq($("#HeadInRoot1").children().eq(1).children().length - 1).remove();

    } else if (FileID == "1687" || FileID == "1688") {

        $(".tag-remove").remove();
        $("#HeadInRoot1").children().eq(1).children().eq($("#HeadInRoot1").children().eq(1).children().length - 1).remove();

    }




    OldRoot = $('#HeadInRoot1 :input').val();
    $("[uk-navbar]").hide();


}



//function DownloadPDF() {

//    var FileID = $('#ChangeFile').select().val();

//    if (FileID == 0) {
//        return false;
//    }

//    $.ajax({
//        method: "POST",
//        url: "GETData.aspx",
//        data: { Controller: "Getfile", FileID: FileID }

//    })
//     .success(function (data) {

//         if (data == 'false') {
//             new duDialog('Message', 'ไม่มีไฟล์ในระบบ กรุณาแจ้งผู้ดูแล!');

//             return false;
//         }

//         var result = data.split(",");

//         $.each(result, function (key, value) {

//             if (value != "") {
//                 var file_path = value;
//                 var a = document.createElement('A');
//                 a.href = file_path;
//                 a.download = file_path.substr(file_path.lastIndexOf('/') + 1);
//                 filename = file_path.substr(file_path.lastIndexOf('/') + 1);
//                 fileid = FileID;

//                 document.body.appendChild(a);
//                 a.click();
//                 document.body.removeChild(a);
//             }

//         });
//     });



//    return false;
//}





//function _download() {

//    if ($("#ContentPlaceHolder1_ddlfile option:selected").val() == 0) {
//        return false;
//    }

//    $.ajax({
//        method: "POST",
//        url: "ajax.aspx",
//        data: { type: "Newfile", file: $("#ContentPlaceHolder1_ddlfile option:selected").val(), UID: $("#UID").text() }

//    })
//      .success(function (data) {

//          $("[show]").show()

//          console.log(data)
//          var file_path = data;
//          var a = document.createElement('A');
//          a.href = file_path;
//          a.download = file_path.substr(file_path.lastIndexOf('/') + 1);
//          document.body.appendChild(a);
//          a.click();
//          document.body.removeChild(a);

//      });



//}



function getlistname(deptID, subid, UserID) {

    $(subid).empty();
    $(subid).append('<option value="0" > --- select ---</option>  ');

    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        async: false,
        cache: false,
        timeout: 30000,
        data: { Controller: "SelectDeptByID", Dept: deptID, }
    })
        .success(function (data) {

            if (data == 'HinoWorkflow') {
                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                    footer: '<a href="#" style="color:red;">    </a>'
                }).then(function (isConfirm) {
                    if (isConfirm.isConfirmed == true) {
                        location.href = "login.aspx";
                    }
                });
                return false;
            }
            if (data != 'false') {
                $(subid).append(data);
                $(subid).select().val(UserID);
            }
        });
}

var Inroot2819 = "";
function GetRootLine() {

    var PriUser = $('#pri-user').prop('checked');
    var PriOther = $('#pri-other').prop('checked');

    if (PriOther == true) {
        PriUser = 2;
        PriOther = 1;

        if (Inroot2819 == '') {
            Inroot2819 = $('#HeadInRoot1 :input').val();
        }

    } else {
        PriUser = 1;
        PriOther = 2;



    }
    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        //async: false,
        //cache: false,
        //timeout: 30000,
        data: { Controller: "GetRootLineF2819", PriUser: PriUser, PriOther: PriOther }

    })
        .success(function (data) {


            if (data == 'Error') {

                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'ทำ ขั้นตอนที่ 1 ให้เรียบร้อยก่อน!',
                    footer: '<a href="#" style="color:red;">    </a>'
                })

                $('#pri-user').prop('checked', true)
                if (Inroot2819 != '') {
                    $("#HeadInRoot1").children().remove()
                    $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
                    $("#InrootApproved1").val(Inroot2819);
                    GETTagInput("#InrootApproved1");

                }
            } else if (data == 'Yes') {
                $('#pri-user').prop('checked', true)
                if (Inroot2819 != '') {
                    $("#HeadInRoot1").children().remove()
                    $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
                    $("#InrootApproved1").val(Inroot2819);
                    GETTagInput("#InrootApproved1");

                }
            } else if (data == 'No') {
                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'เลือกลักษณะการลา ให้ถูกต้อง!',
                    footer: '<a href="#" style="color:red;">    </a>'
                })

                $('#pri-user').prop('checked', true)
                if (Inroot2819 != '') {
                    $("#HeadInRoot1").children().remove()
                    $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
                    $("#InrootApproved1").val(Inroot2819);
                    GETTagInput("#InrootApproved1");

                }

            } else if (data == 'No2') {
                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'เลือกลักษณะการลา ลาแทน เฉพาะสาย F!',
                    footer: '<a href="#" style="color:red;">    </a>'
                })

                $('#pri-user').prop('checked', true)
                if (Inroot2819 != '') {
                    $("#HeadInRoot1").children().remove()
                    $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
                    $("#InrootApproved1").val(Inroot2819);
                    GETTagInput("#InrootApproved1");

                }

            } else if (data == 'No1') {
                Swal.fire({
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Oops...',
                    text: 'เลือกลักษณะการลา ลาเอง ไม่ต้องเลือก ลาแทน !',
                    footer: '<a href="#" style="color:red;">    </a>'
                })


                $('#pri-user').prop('checked', true)
                if (Inroot2819 != '') {
                    $("#HeadInRoot1").children().remove()
                    $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
                    $("#InrootApproved1").val(Inroot2819);
                    GETTagInput("#InrootApproved1");

                }
            } else if (PriOther == true) {


                $("#HeadInRoot1").children().remove()
                $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')


                var FileID = $('#ContentPlaceHolder1_ChangeFile').select().val();

                if (FileID == "1755") {
                    $("#InrootApproved1").val(data + ",NUCHARIN.SAIMUEANG");
                } else if (FileID == "1752") {
                    $("#InrootApproved1").val(data + ",SUPARAT.SATBUBPA");
                } else if (FileID == "1754") {
                    $("#InrootApproved1").val(data + ",THANYAPHAT.THONGDEE");
                } else if (FileID == "1758") {
                    $("#InrootApproved1").val(data + ",RATTNAKORN.WORAPONG");
                } else {
                    $("#InrootApproved1").val(data);
                }


                GETTagInput('#InrootApproved1');

            }

            return true;
        });

    return false;
}


iOS();

function iOS() {


    var iDevices = [

        'iPhone',
        'iPod'

    ];

    if (!!navigator.platform) {

        while (iDevices.length) {

            if (navigator.platform === iDevices.pop()) {



                $('body').css('font-size', '8px');
                $('button').css('padding', '1px,1px');
                $('button').css('font-size', '5px');
                $('#uk-button_btn-menu').css('font-size', '20px');
                $(".data-stable>tbody>tr>td").css('padding', '0px');
                $("[uk-navbar]").hide();


            }

        }

    }


    return false;

}


//-------------------------------------------------  Spn1 -----------------------------------------------

function GetSpn1Supplier() {

    var optionSpn1 = "<option value='0'> ---- Select ---- </option>";
    $.ajax({
        method: "POST",
        url: "GETData.aspx",
        data: { Controller: "GetSpn1Supplier" }
    })
        .success(function (data) {
            var res = JSON.parse(data);
            $.each(res['Table1'], function (key, value) {
                optionSpn1 += "<option value='" + value.EmpCode + "'>" + value.SP + " </option>";
            });
            $("#Spn1Select").children().remove();
            $("#Spn1Select").append(optionSpn1);
        });


}

function SpnChangeSupplier() {

    var Spn1Select = $("#Spn1Select").select().val();

    var Spn1FileID = $('#ContentPlaceHolder1_ChangeFile').select().val();
    if (Spn1FileID == "1921" || Spn1FileID == "1923") {

        var Headid = "#HeadOutRoot1";
        var Tagid = "OutrootApproved1";
        //var PurchaseAdmin = ["20041635", "20122721", "20051684"];

        $.ajax({
            method: "POST",
            url: "GETData.aspx",
            async: false,
            cache: false,
            timeout: 30000,
            data: { Controller: "SelectRoottByID", UserID: Spn1Select }
        })
            .success(function (data) {


                if (data == 'HinoWorkflow') {
                    Swal.fire({
                        allowOutsideClick: false,
                        icon: 'error',
                        title: 'Oops...',
                        text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                        footer: '<a href="#" style="color:red;">    </a>'
                    }).then(function (isConfirm) {
                        if (isConfirm.isConfirmed == true) {
                            location.href = "login.aspx";
                        }
                    });
                    return false;
                }
                if (data != 'false') {

                    $("#OutDeptUser1").select().val(Spn1Select)
                    $(Headid).children().remove();
                    $(Headid).append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="' + Tagid + '" />')
                    $("#" + Tagid).val(data);
                    GETTagInput("#" + Tagid);
                    //console.log(data);

                }
            });


        //basxxx

    }



}
//-------------------------------------------------  Spn1 -----------------------------------------------

//-------------------------------------------------  Purchase -----------------------------------------------

var OldRoot = "";
function CalRoutePs() {

    //console.log(id);

    var EmpCode = $("#PurchaseSelect").select().val();
    var position = "";

    if (EmpCode == "0") {

        Swal.fire({
            //allowOutsideClick: false,
            icon: 'error',
            title: 'Oops...',
            text: 'ต้องเลือก รูปแบบการสั่งซื้อ!',
            footer: '<a href="#" style="color:red;">    </a>'
        });

        $("#mon-hi").prop("checked", false);
        $("#mon-md").prop("checked", false);
        $("#mon-lw").prop("checked", false);

        return false;

    }

    if ($("#mon-hi").prop("checked") == true || $("#mon-md").prop("checked") == true || $("#mon-lw").prop("checked") == true) {

        if ($("#mon-hi").prop("checked") == true) {

            $("#HeadInRoot1").children().remove()
            $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
            $("#InrootApproved1").val(OldRoot);
            GETTagInput('#InrootApproved1');
            //return false;
            position = "";

        } else if ($("#mon-md").prop("checked") == true) {

            position = "22";

        } else if ($("#mon-lw").prop("checked") == true) {
            position = "61";

        }

        // inroot
        if (position != "") {
            $.ajax({
                method: "POST",
                url: "GETData.aspx",
                //async: false,
                //cache: false,
                //timeout: 30000,
                data: { Controller: "GetPSVPUP", EmpCode: EmpCode, position: position }
            })
                .success(function (data) {
                    var res = JSON.parse(data);

                    var NewRootVp = "";

                    if (position == "22") {
                        //NewRootVp = OldRoot + ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;

                        try {

                            NewRootVp = OldRoot + ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;

                        }
                        catch (err) {

                            NewRootVp = OldRoot + ',NOPPADOL.VUTIDECHKAMJORN';

                        }


                        $("#HeadInRoot1").children().remove();

                        $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
                        $("#InrootApproved1").val(NewRootVp);
                        GETTagInput('#InrootApproved1');
                    } else if (position == "61") {
                        NewRootVp = OldRoot + ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;
                        NewRootVp = NewRootVp + ',' + res["Table1"][1].Name + "." + res["Table1"][1].Surname;
                        $("#HeadInRoot1").children().remove();

                        $("#HeadInRoot1").append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="InrootApproved1" />')
                        $("#InrootApproved1").val(NewRootVp);
                        GETTagInput('#InrootApproved1');
                    }




                    console.log(res);


                });
        }


        // inroot

        // outroot
        $.ajax({
            method: "POST",
            url: "GETData.aspx",
            //async: false,
            //cache: false,
            //timeout: 30000,
            data: { Controller: "PSGetPSVPUP", EmpCode: EmpCode, position: position }
        })
            .success(function (data) {
                var res = JSON.parse(data);

                var PSNewRootVp = "";


                if (position == "22") {
                    //PSNewRootVp = ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;


                    try {

                        PSNewRootVp = PSNewRootVp + ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;
                    }
                    catch (err) {

                        PSNewRootVp = PSNewRootVp + ',NOPPADOL.VUTIDECHKAMJORN';

                    }


                    //if (res["Table1"][0].Name == '') {


                    //}else{



                    //}


                } else if (position == "61") {
                    PSNewRootVp = ',' + res["Table1"][0].Name + "." + res["Table1"][0].Surname;
                    PSNewRootVp = PSNewRootVp + ',' + res["Table1"][1].Name + "." + res["Table1"][1].Surname;
                }

                ////////

                var PurchaseAdmin = $("#PurchaseSelect").select().val();

                var PurchaseFileID = $('#ContentPlaceHolder1_ChangeFile').select().val();
                if (PurchaseFileID == "2057" || PurchaseFileID == "2064" || PurchaseFileID == "1942" || PurchaseFileID == "1933" || PurchaseFileID == "2226") {

                    var Headid = "#HeadOutRoot1";
                    var Tagid = "OutrootApproved1";
                    //var PurchaseAdmin = ["20041635", "20122721", "20051684"];

                    $.ajax({
                        method: "POST",
                        url: "GETData.aspx",
                        //async: false,
                        //cache: false,
                        //timeout: 30000,
                        data: { Controller: "SelectRoottByID", UserID: PurchaseAdmin }
                    })
                        .success(function (data) {


                            if (data == 'HinoWorkflow') {
                                Swal.fire({
                                    allowOutsideClick: false,
                                    icon: 'error',
                                    title: 'Oops...',
                                    text: 'login หมดอายุ กรุณา login อีกครั้ง!',
                                    footer: '<a href="#" style="color:red;">    </a>'
                                }).then(function (isConfirm) {
                                    if (isConfirm.isConfirmed == true) {
                                        location.href = "login.aspx";
                                    }
                                });
                                return false;
                            }
                            if (data != 'false') {

                                $("#OutDeptUser1").select().val(PurchaseAdmin)
                                $(Headid).children().remove();
                                $(Headid).append('<input type="text" placeholder="" data-role="tagsinput" class="form-tags" style="width: 70%;" value="" id="' + Tagid + '" />')

                                //if (PurchaseAdmin == '11540196' || PurchaseAdmin == '20112591' || PurchaseAdmin == '20153446') {
                                //	data = 'TEERASAK.NUTIPRAPUN,' + data;
                                //}

                                $("#" + Tagid).val(data + PSNewRootVp);
                                GETTagInput("#" + Tagid);
                                //console.log(data);

                            }
                        });


                    //basxxx

                }




                console.log(res);


            });
    }




}


function ChkMoney(id) {
    CalRoutePs();
}

function PurchaseChangeJob() {
    CalRoutePs();
}

function loadingOut(loading) {
    //setTimeout(() => loading.out(), 15000);
    () => loading.out()

}


function horizontalCustomStyle() {

    var loading = new Loading({
        title: ' โปรดรอ...',
        direction: 'hor',
        discription: 'กำลังบันทึกข้อมูล...',
        defaultApply: true,
    });

    loadingOut(loading);
}



        //-------------------------------------------------  Purchase -----------------------------------------------

        //-------------------------------------------------  other -----------------------------------------------



        //-------------------------------------------------  other -----------------------------------------------






    </script >