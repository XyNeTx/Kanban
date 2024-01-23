$(document).ready(function () {

    const xKBNIM012 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Supplier Code', 'Short Name', 'Part No.', 'Name', 'Store Code', 'KB', 'Box/Qty.', 'Cycle'
                , 'Day1', 'Day2', 'Day3', 'Day4', 'Day5', 'Day6', 'Day7', 'Day8', 'Day9', 'Day10'
                , 'Day11', 'Day12', 'Day13', 'Day14', 'Day15', 'Day16', 'Day17', 'Day18', 'Day19', 'Day20'
                , 'Day21', 'Day22', 'Day23', 'Day24', 'Day25', 'Day26', 'Day27', 'Day28', 'Day29', 'Day30'
            ],
            "TH": ['Supplier Code', 'Short Name', 'Part No.', 'Name', 'Store Code', 'KB', 'Box/Qty.', 'Cycle'
                , 'Day1', 'Day2', 'Day3', 'Day4', 'Day5', 'Day6', 'Day7', 'Day8', 'Day9', 'Day10'
                , 'Day11', 'Day12', 'Day13', 'Day14', 'Day15', 'Day16', 'Day17', 'Day18', 'Day19', 'Day20'
                , 'Day21', 'Day22', 'Day23', 'Day24', 'Day25', 'Day26', 'Day27', 'Day28', 'Day29', 'Day30'
            ],
            "JP": ['Supplier Code', 'Short Name', 'Part No.', 'Name', 'Store Code', 'KB', 'Box/Qty.', 'Cycle'
                , 'Day1', 'Day2', 'Day3', 'Day4', 'Day5', 'Day6', 'Day7', 'Day8', 'Day9', 'Day10'
                , 'Day11', 'Day12', 'Day13', 'Day14', 'Day15', 'Day16', 'Day17', 'Day18', 'Day19', 'Day20'
                , 'Day21', 'Day22', 'Day23', 'Day24', 'Day25', 'Day26', 'Day27', 'Day28', 'Day29', 'Day30'
            ],
        },
        ColumnValue: [
            { "data": "F_Plant" },
            { "data": "F_Plant" },
            { "data": "F_OrderType" },
            { "data": "F_Effect_Date" },
            { "data": "F_Plant", className: "text-center" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" },
            { "data": "F_Plant", className: "text-right" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });



    xSplash.hide();

    xKBNIM012.prepare();

    xKBNIM012.initial(function (result) {

        if ($('#TypeForImportConditionDetail').prop('checked')) $('#fldConditionSupplier').prop('disabled', 'disabled');
        if ($('#TypeForImportConditionSupplier').prop('checked')) $('#fldConditionDetail').prop('disabled', 'disabled');



        xAjax.onCheck('#TypeForImportConditionDetail', function () {
            console.log('TypeForImportConditionDetail');
            $('#fldConditionSupplier').prop('disabled', 'disabled');
            $('#fldConditionDetail').prop('disabled', false);
        });
        xAjax.onCheck('#TypeForImportConditionSupplier', function () {
            console.log('TypeForImportConditionSupplier');
            $('#fldConditionSupplier').prop('disabled', false);
            $('#fldConditionDetail').prop('disabled', 'disabled');
        });

        xKBNIM012.search();
    });

    //onSave = function () {
    //    KBNIM003M.save(function () {
    //        KBNIM003M.search();
    //    });
    //}

    //onDelete = function () {
    //    KBNIM003M.delete(function () {
    //        KBNIM003M.search();
    //    });
    //}

    //onDeleteAll = function () {
    //    KBNIM003M.deleteall(function () {
    //        KBNIM003M.search();
    //    });
    //}

    //onPrint = function () {
    //    xDataTableExport.setConfigPDF({
    //        title: 'OLD TYPE SERVICE CHECK LIST'
    //    });
    //}

    //onExecute = function () {
    //    console.log('onExecute');
    //}



});