"use strict";
$(document).ready(function () {
    const KBNIM004M = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Order No.', 'Order Issued', 'Seq No.', 'Type Part', 'Part No.', 'Store Code', 'Qty.', 'Order'],
            "TH": ['Order No.', 'Order Issued', 'Seq No.', 'Type Part', 'Part No.', 'Store Code', 'Qty.', 'Order'],
            "JP": ['Order No.', 'Order Issued', 'Seq No.', 'Type Part', 'Part No.', 'Store Code', 'Qty.', 'Order'],
        },
        ColumnValue: [
            { "data": "F_Plant" },
            { "data": "F_Plant" },
            { "data": "F_OrderType" },
            { "data": "F_Effect_Date" },
            { "data": "F_End_Date" },
            { "data": "F_End_Date" },
            { "data": "F_End_Date" },
            { "data": "F_End_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });
    xSplash.hide();
    KBNIM004M.prepare();
    KBNIM004M.initial(function (result) {
        KBNIM004M.search();
    });
    //onSave = function () {
    //    KBNIM004M.save(function () {
    //        KBNIM004M.search();
    //    });
    //}
    //onDelete = function () {
    //    KBNIM004M.delete(function () {
    //        KBNIM004M.search();
    //    });
    //}
    //onDeleteAll = function () {
    //    KBNIM004M.deleteall(function () {
    //        KBNIM004M.search();
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
    //xAjax.onChange('#frmCondition #F_Plant', function () {
    //    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());
    //    KBNIM004M.search();
    //});
});
