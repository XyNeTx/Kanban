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
            { title: "UserID", data: "UserID", width: "15%" },
            { title: "UserName", data: "UserName", width: "25%" },
            { title: "Telephone", data: "Telelphone", width: "15%" },
            { title: "Fax", data: "Fax", width: "10%" },
            { title: "Email", data: "Email", width: "35%" },
        ],
        order: [[1, 'asc']],
    });

    await LoadContactList();

    xSplash.hide();

});


function LoadContactList() {
    // Load Color Tag

    let obj = {
        UserID: "...",
        UserName: "",
        Telelphone: "",
        Fax: "",
        Email: ""
    };

    _xLib.AJAX_Get("/api/KBNOR294/LoadContactList", "",
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
            let obj = {
                UserID: "...",
                UserName: "",
                Telelphone: "",
                Fax: "",
                Email: ""
            };
            $("#tableMain").DataTable().row.add(obj).draw();
            $("table tbody tr td").addClass("text-center");
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
        $(this).empty();
        $(this).append('<input type="text" class="clsEdit form-control" id="txtEdit" value="' + value + '" />');
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

        if (_newValue == _oriValue) {
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
                UserID: "...",
                UserName: "",
                Telelphone: "",
                Fax: "",
                Email: ""
            };
            $("#tableMain").DataTable().row.add(obj).draw();
            $("table tbody tr td").addClass("text-center");
        }
    }
    else {
        return;
    }

});

$("#btnConfirm").click(function () {
    Confirm();
});

function Confirm() {

    let listObj = $('#tableMain').DataTable().rows().data().toArray();

    listObj = listObj.filter(x => x.UserID.trim() != "..."
        && x.UserName.trim() != "" && x.Telelphone.trim() != ""
        && x.Email.trim() != "");


    _xLib.AJAX_Post("/api/KBNOR294/Confirm", listObj,
        function (success) {
            xSwal.success(success.response, success.message);
            return LoadContactList();
        },
        function (error) {
            xSwal.xError(error);
            console.log(error);
        }
    );

}