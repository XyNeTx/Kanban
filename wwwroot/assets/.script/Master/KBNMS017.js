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
                    title: "Part No.", data: "f_Supplier"
                },
                {
                    title: "Store Code.", data: "f_KanbanNo"
                },
                {
                    title: "Address1.", data: "f_PartNo"
                },
                {
                    title: "Ratio1", data: "f_PartName"
                },
                {
                    title: "Address2", data: "f_StoreCD"
                },
                {
                    title: "Ratio2", data: "f_Max_Trip"
                },
                {
                    title: "Address3", data: "f_StoreCD"
                },
                {
                    title: "Ratio3", data: "f_Max_Trip"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    GetDropDownList();

    xSplash.hide();
})

async function GetDropDownList() {
    let getObj = await $("#formMain").formToJSON();
    getObj.action = "new";

    _xLib.AJAX_Get("/api/KBNMS017/GetDropDownList", getObj,
        function (success) {
            console.log(success)
        }
    );

}

