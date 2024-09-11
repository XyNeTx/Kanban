$(document).ready(async function () {

    await $("#tableImport").DataTable({
        "processing": true,
        "serverSide": false,
        width: '50%',
        paging: false,
        sorting: false,
        searching: false,
        autoWidth: false,
        scrollX: false,
        scrollY: "300px", // "300px"
        scrollCollapse: true,
        ordering: false,
        columns: [
            {
                data: "No", title: "No", render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { title: "Type_Import", data: "F_TYPE",  },
            { title: "Records", data: "F_REcords",  },
        ],
    });

    await $("#tableConfrim").DataTable({
        "processing": true,
        "serverSide": false,
        width: '50%',
        paging: false,
        sorting: false,
        searching: false,
        autoWidth: false,
        scrollX: false,
        scrollY: "300px", // "300px"
        scrollCollapse: true,
        ordering: false,
        columns: [
            {
                data: "No", title: "No",width:"10%", render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },

            { title: "Type_Import", data: "F_TYPE", width:"20%" },
            { title: "Records", data: "F_REcords", width: "10%" },
        ],
    });

    let date = moment(_xLib.GetCookie("loginDate").substring(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY");
    let shift = _xLib.GetCookie("loginDate").substring(10, 11);

    await _xLib.AJAX_Get("/api/KBNIM010/Onload", { Date: date, Shift: shift },
        async function (success) {
            if (success.status == "200") {
                await ListData(date, shift);
            }
        },
        function (error) {
            xSwal.error(error.responseJSON.title, error.responseJSON.message);
            console.log(error);
        }
    );
    

});

async function ListData(date, shift) {

    await _xLib.AJAX_Get("/api/KBNIM010/ListData", { Date: date, Shift: shift },
        async function (success) {
            if (success.status == "200") {
                data = JSON.parse(success.data[0]);
                data2 = JSON.parse(success.data[1]);
                //console.log(data);
                //console.log(data2);

                $("#tableImport").DataTable().rows.add(data).draw();
                $("#tableConfrim").DataTable().rows.add(data2).draw();

                $("#tableImport").DataTable().columns.adjust().draw();
                $("#tableConfrim").DataTable().columns.adjust().draw();

                await $("table thead tr th").css("text-align", "center");
                await $("table tbody tr td").css("text-align", "center");

                await xSplash.hide();
            }
        },
        async function (error) {
            await xSplash.hide();
            xSwal.error(error.responseJSON.title, error.responseJSON.error);
            console.log(error);
        }
    );
}

$("#btnConfirm").click(async function () {

    let date = moment(_xLib.GetCookie("loginDate").substring(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY");
    let shift = _xLib.GetCookie("loginDate").substring(10, 11);

    let isConfirm = await xSwal.confirm("Are you sure?", "Do you want Confirm All Import Data?");

    if (isConfirm) {
        _xLib.AJAX_Post("/api/KBNIM010/Confirm", (JSON.stringify({ date: date, shift: shift })),
            function(success) {
                if (success.status == "200") {
                    xSwal.success("Success", success.message);
                }
            },
            function(error) {
                xSwal.error(error.responseJSON.title, error.responseJSON.message);
                console.log(error);
            }
        )
    }
});