$(document).ready(function () {

    // Initialize the DataTable with the dynamically created columns
    _xDataTable.InitialDataTable("#tableMain", {
        processing: false,
        serverSide: false,
        width: '100%',
        paging: false,
        sorting: false,
        searching: false,
        scrollX: true,
        scrollY: "200px",
        scrollCollapse: false,
        select: false,
        columns:
        [
                {
                    title: "Flag",
                    render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox">`;
                    }
                },
                {
                    title: "Group Name",data: "f_CycleB"
                },
                {
                    title: "Supplier Code",data: "f_CycleB"
                },
                {
                    title: "Kanban No", data: "f_CycleB"
                },
                {
                    title: "Store Code", data: "f_CycleB"
                },
                {
                    title: "Part No.", data: "f_CycleB"
                },
                {
                    title: "Q'ty", data: "f_CycleB"
                },
                {
                    title: "Start Date", data: "f_CycleB"
                },
                {
                    title: "End Date", data: "f_CycleB"
                },
        ],
        order: [[0, "asc"]]
    });

    $("#F_Start_Date").initDatepicker();
    $("#F_End_Date").initDatepicker();

    List_Data();

    xSplash.hide();
})

async function List_Data() {
    let getObj = await $("#formMain").formToJSON();

    _xLib.AJAX_Get("/api/KBNMS016/List_Data", getObj,
        function (success) {
            console.log(success);
        },
    );

}