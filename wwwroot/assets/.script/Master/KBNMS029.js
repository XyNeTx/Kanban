$(document).ready(function () {

    _xDataTable.InitialDataTable("#tableMain",
        {
            "processing": false,
            "serverSide": false,
            width: '100%',
            paging: false,
            sorting: false,
            searching: false,
            scrollX: true,
            scrollY: "200px",
            scrollCollapse: false,
            "columns": [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox">`;
                    }
                },
                {
                    title: "Plant", data: "f_Plant"
                },
                {
                    title: "Dock Code", data: "f_Dock_Code"
                },
                {
                    title: "Start Date", data: "f_Start_Date"
                },
                {
                    title: "End Date", data: "f_End_Date"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    $("#F_Start_Date").initDatepicker();
    $("#F_End_Date").initDatepicker("31/12/2999");

    xSplash.hide();

})

