$(document).ready(async function () {

    await $("#tableMain").DataTable({
        width: '100%',
        paging: false,
        scrollCollapse: true,
        "processing": false,
        "serverSide": false,
        scrollX: false,
        scrollY: '300px',
        searching: true,
        select: false,
        info: true,
        ordering: false,
        autoWidth: true,  // Enable autoWidth
        columns: [
            {
                title: "<input type='checkbox' id='chkAll'>",
                render(data, type, row, meta) {
                    return `<input type="checkbox" class="chk" value="${row.menu_ID}">`;
                }
            },
            { title: "Menu ID", data: "menu_ID" },
            { title: "Remark", data: "remark" }
        ]
    });


    _xLib.AJAX_Get("/api/KBNMT110/GetUser", null,
        function (success)
        {
            //$("#tableMain").DataTable().clear().rows.add(success).draw();
            $("#inpUserCode").empty();
            $("#inpUserCode").append("<option value='' hidden></option>");
            success.data.forEach(function (item)
            {
                $("#inpUserCode").append(`<option value="${item.code.trim()}">${item.code.trim()}</option>`);
            });
            xSplash.hide();
        },
        function (error)
        {
            xSplash.hide();
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );


    await _xLib.AJAX_Get("/api/KBNMT110/GetMenu", "",
        function (success) {
            $("#tableMain").DataTable().clear().rows.add(success.data).draw();

            $("table tbody tr td").css("text-align", "center");
            $("table thead tr th").css("text-align", "center");
            xSplash.hide();
        },
        function (error) {
            xSplash.hide();
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

});

$("#inpUserCode").change(async function () {
    xSplash.show();
    $("#tableMain tbody tr td").find("input").each(function () {
        $(this).prop("checked", false);
    });
    $("#inpUserID").remove();
    $("#inpUserName").val("");

    _xLib.AJAX_Get("/api/KBNMT110/GetUserDetailAndAuth", { User_Code: $("#inpUserCode").val() },
        function (success) {
            console.log(success);
            $("#inpUserName").val(success.data[0].name + " " + success.data[0].surname);
            $("#inpUserName").append('<input type="hidden" id="inpUserID" value="' + success.data[0]._ID + '">')


            success.data.forEach(function (item) {

                $("#tableMain tbody tr").each(function () {
                    let menuID = $(this).find("td:eq(1)").text();
                    if (item.menu_ID == menuID) {
                        $(this).find("td:eq(0) input").prop("checked", true);
                    }
                });

            });
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

    xSplash.hide();

});

$(document).on("change" , "#chkAll",function () {
    console.log("chkAll");
    if ($(this).prop("checked")) {
        $(".chk").each(function () {
            $(this).prop("checked", true);
        });
    } else {
        $(".chk").each(function () {
            $(this).prop("checked", false);
        });
    }
});

$(document).on("click", "table tbody tr td" , function () {
    let chk = $(this).closest("tr").find("td:eq(0) input");
    if (chk.prop("checked")) {
        chk.prop("checked", false);
    } else {
        chk.prop("checked", true);
    }
});

$("#btnSave").click(async function () {

    let listObj = [];

    $("#tableMain :checkbox:checked").each(function () {
        let obj = $("#tableMain").DataTable().row($(this).closest("tr")).data();
        obj.user_ID = $("#inpUserID").val();
        listObj.push(obj);
    });

    _xLib.AJAX_Post("/api/KBNMT110/SetUserAuthorize", JSON.stringify(listObj),
    function (success) {
        xSwal.success("Success", success.message);
        $("#inpUserCode").trigger("change");
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );



});