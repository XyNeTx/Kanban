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
                    title: "Plant", data: "F_PlantCode"
                },
                {
                    title: "Dock Code", data: "F_Dock_Code"
                },
                {
                    title: "Start Date", data: "F_Start_Date", render: function (row, meta) {
                        return moment(row, "YYYYMMDD").format("DD/MM/YYYY");
                    }
                },
                {
                    title: "End Date", data: "F_End_Date", render: function (row, meta) {
                        return moment(row, "YYYYMMDD").format("DD/MM/YYYY");
                    }
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    xSplash.hide();
})

