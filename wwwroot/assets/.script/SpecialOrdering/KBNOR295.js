$(document).ready(async function () {

    $("#tableMain").DataTable({
        width: '100%',
        paging: false,
        scrollCollapse: true,
        "processing": false,
        "serverSide": false,
        scrollX: false,
        scrollY: '400px',
        searching: false,
        info: false,
        ordering: false,
        columns: [
            { title: "Username", data: "F_User_ID", width: "15%" },
            { title: "Name", data: "F_Name", width: "15%" },
            { title: "Surname", data: "F_Surname", width: "15%" },
            { title: "F_Email", data: "F_Email", width: "10%" },
            { title: "Path File", data: "F_Path_File", width: "35%" },
            {
                title: "", render: function (data, type, row) {
                    return '<label for="' + row.F_User_ID + '" class="btn btn-primary m-0">Browse...</label> <input type="file" class="form-control" accept = ".jpg, .jpeg, .png" id="' + row.F_User_ID + '" name="file" style="display:none">';
                }
            }
        ],
        order: [[1, 'asc']],
    });

    await LoadContactList();

    xSplash.hide();

});


function LoadContactList() {
    // Load Color Tag

    let obj = {
        F_User_ID: "...",
        F_Name: "",
        F_Surname: "",
        F_Email: "",
        F_Path_File: ""
    };

    _xLib.AJAX_Get("/api/KBNOR295/LoadContactList", "",
        function (success) {
            success = _xLib.JSONparseMixData(success);

            if (success.data.length > 0) {
                var table = $('#tableMain').DataTable();
                table.clear();
                table.rows.add(success.data);
                table.row.add(obj);
                table.draw();
            } else {
                // If there is no data, add a new row
                table.row.add(obj);
            }

            $("table thead tr th").addClass("text-center");
            $("table tbody tr td").addClass("text-center");

        },
        function (error) {
            console.log(error);
        }
    );

}

let _oriValue = "";
$(document).on('dblclick', '#tableMain tbody tr td', function () {

    if ($(".clsEdit").length > 0) return;

    _oriValue = $(this).text();

    let row = $(this).closest('tr');
    row.find('td').each(function () {
        let value = $(this).text();
        if($(this).index() >= 4) return;
        $(this).empty();
        $(this).append('<input type="text" class="clsEdit form-control" id="txtEdit" value="'+value+'" />');
    });


    //$(this).empty();
    //$(this).append('<input type="text" class="clsEdit form-control" id="txtEdit" value="' + _oriValue + '" />');

    $('#txtEdit').focus();
});

$(document).on('focusin', '#tableMain tbody tr td input[type="text"]', function () {
    _oriValue = $(this).val();
    console.log(_oriValue);
});

$(document).on('focusout  keypress', '#tableMain tbody tr td', function (e) {
    if ((e.type == 'keypress' && e.which == 13) || e.type == 'focusout') {

        let _newValue = $(this).find('input').val();

        if (_newValue == _oriValue || _newValue == "") {
            $(this).empty();
            $(this).text(_oriValue);
            return;
        };
        $(this).empty();
        $("#tableMain").DataTable().cell(this).data(_newValue).draw();

        let row = $(this).closest('tr');
        let rowIndex = $("#tableMain").DataTable().row(row).index();
        //console.log(rowIndex);
        let maxIndex = $("#tableMain").DataTable().rows().count() - 1;
        //console.log(maxIndex);

        if (rowIndex == maxIndex) {
            let obj = {
                F_User_ID: "...",
                F_Name: "",
                F_Surname: "",
                F_Email: "",
                F_Path_File: ""
            };
            $("#tableMain").DataTable().row.add(obj).draw();
            $("table tbody tr td").addClass("text-center");
        }
    }
    else {
        return;
    }

});

$(document).on('change', 'input[type="file"]', function () {
    console.log("Change File");
    let file = $(this)[0].files[0];

    let _data = new FormData();
    var blob = new Blob([file], { type: "image/jpeg" });
    _data.append("file", blob, file.name);

    let row = $(this).closest('tr');

    _xLib.AJAX_PostFile("/api/KBNOR295/UploadIMG", _data,
        function (success) {
            console.log(success);
            let rowData = $("#tableMain").DataTable().row(row).data();
            console.log(rowData);
            rowData.F_Path_File = success.data;
            $("#tableMain").DataTable().row(row).data(rowData).draw();
        },
        function (error) {
            console.log(error);
        }
    );



});

$("#btnConfirm").click(function () {
    Confirm();
});

function Confirm() {
    try {
        let listObj = $('#tableMain').DataTable().rows().data().toArray();
        //console.log(listObj);
        listObj = listObj.filter(function (el) {
            return (el.F_User_ID.trim() != "..." ||
                el.F_Surname.trim() != "" ||
                el.F_Path_File.trim() != "" ||
                el.F_Name.trim() != "" || el.F_Email.trim() != "");
        });

        //console.log(listObj);

        listObj = listObj.filter(function (el) {
            if (!el.F_Email.includes("@")) {
                throw "Email is invalid";
            }
            return el;
        });


        _xLib.AJAX_Post("/api/KBNOR295/Confirm", listObj,
            function (success) {
                xSwal.success(success.response, success.message);
                return LoadContactList();
            },
            function (error) {
                console.log(error);
            }
        );
    }
    catch (ex) {
        xSwal.error("Error", ex);
    }
}