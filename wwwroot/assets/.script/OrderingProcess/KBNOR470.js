$(document).ready(async function () {


    await $("#tableMain").DataTable({
        "processing": true,
        "serverSide": false,
        scrollY: '350px',
        scrollCollapse: true,
        paging: false,
        searching: false,
        info: false,
        ordering: false,
        width: "80%",
        columns: [
            {
                title: "No", width: "10%", render: function (data, type, row, meta) {
                    return '<input type="checkbox" class="chkBoxDT" name="inputChkBox" value=' + meta.row + ' checked>';
                },
            },
            { title: "PDS No.", data: "F_OrderNo",width: "20%" },
            { title: "Supplier", data: "F_SUpplier_Code", width: "15%" },
            { title: "Delivery Date", data: "F_Delivery_Date", width: "15%" },
            { title: "Part No.", data: "F_PART_NO", width: "20%" },
            //{ title: "Remark", data: "", width: "20%" }
        ]
    });

    await $("table thead tr th").addClass("text-center");
    await $("table body tr td").addClass("text-center");

    await _xLib.AJAX_Get("/api/KBNOR470/List_Data", '',
        function (success) {
            if (success.status == 200) {
                success = _xLib.JSONparseAndTrim(success);
                console.log(success)
                $("#tableMain").DataTable().clear().draw();
                $("#tableMain").DataTable().rows.add(success.data).draw();
            }
        },
        function (error) {
            console.log(error);
            xSwal.error("Error", "Data Not Found");
        }
    );

    xSplash.hide();
})

$("#btnUnlock").click(function () {
    var listObj = [];

    $(".chkBoxDT:checked").each(function () {
        var data = $("#tableMain").DataTable().row($(this).val()).data();
        data.F_SUpplier_Code = data.F_SUpplier_Code.split("-")[0];
        data.F_PART_NO = data.F_PART_NO.split("-")[0];
        data.F_Delivery_Date = data.F_Delivery_Date;
        listObj.push(data);
    });
    //console.log(moment().format("YYYY-MM-DD"))
    //console.log(listObj);

    _xLib.AJAX_Post("/api/KBNOR470/Unlock", JSON.stringify(listObj),
        async function (success) {
            if(success.status == 200)
            {
                xSwal.success("Success", "Unlock Data Complete")

                await _xLib.AJAX_Get("/api/KBNOR470/List_Data", '',
                    function (success) {
                        if (success.status == 200) {
                            success = _xLib.JSONparseAndTrim(success);
                            console.log(success)
                            $("#tableMain").DataTable().clear().draw();
                            $("#tableMain").DataTable().rows.add(success.data).draw();
                        }
                    },
                    function (error) {
                        console.log(error);
                    }
                );
            }
        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );
});

