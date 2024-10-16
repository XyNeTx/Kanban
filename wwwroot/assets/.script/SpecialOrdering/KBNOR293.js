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
            { title: "COLOR", data: "COLOR", width: "50%" },
            { title: "TypePart", data: "TypePart", width: "50%" },
        ],
        order: [[1, 'asc']],
    });

    await LoadColorTag();



    xSplash.hide();

});

function LoadColorTag() {
    // Load Color Tag


    let obj = {
        COLOR: "AddNew",
        TypePart: "",
    };

    _xLib.AJAX_Get("/api/KBNOR293/LoadColorTag","",
        function (success) {
            success = _xLib.JSONparseMixData(success);

            if (success.data.length > 0) {
                var table = $('#tableMain').DataTable();
                table.clear();
                table.rows.add(success.data);
                table.row.add(obj);
                table.draw();
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

    if($(".clsEdit").length > 0) return;

    _oriValue = $(this).text();

    $(this).empty();
    $(this).append('<input type="text" class="clsEdit form-control" id="txtEdit" value="' + _oriValue + '" />');

    $('#txtEdit').focus();
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
    }
    else {
        return;
    }

});

$("#btnConfirm").click(function () {
    SaveColorTag();
});

function SaveColorTag() {

    let listObj = $('#tableMain').DataTable().rows().data().toArray();

    listObj = listObj.filter(function (el) {
        return (el.COLOR != "AddNew" || el.TypePart != "");
    });

    _xLib.AJAX_Post("/api/KBNOR293/Confirm", listObj,
        function (success) {
            xSwal.success(success.response, success.message);
            return LoadColorTag();
        },
        function (error) {
            console.log(error);
        }
    );

}
