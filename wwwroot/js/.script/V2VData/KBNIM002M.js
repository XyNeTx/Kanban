"use strict";
$(document).ready(function () {
    const KBNIM002M = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Order No.', 'Order Issued', 'Part No.', 'Store Code', 'ADV. Deli Date', 'Qty.', 'Remark', 'Country', 'Order Type'],
            "TH": ['Order No.', 'Order Issued', 'Part No.', 'Store Code', 'ADV. Deli Date', 'Qty.', 'Remark', 'Country', 'Order Type'],
            "JP": ['Order No.', 'Order Issued', 'Part No.', 'Store Code', 'ADV. Deli Date', 'Qty.', 'Remark', 'Country', 'Order Type'],
        },
        ColumnValue: [
            { "data": "F_PDS_No" },
            { "data": "F_PDS_Issued_Date" },
            { "data": "F_Part_No" },
            { "data": "F_Store_Order" },
            { "data": "F_Delivery_Date" },
            { "data": "F_Qty" },
            { "data": "F_Remark" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });
    xSplash.hide();
    KBNIM002M.prepare();
    KBNIM002M.initial(function (result) {
        KBNIM002M.search();
    });
    //onSave = function () {
    //    KBNIM002M.save(function () {
    //        KBNIM002M.search();
    //    });
    //}
    //onDelete = function () {
    //    KBNIM002M.delete(function () {
    //        KBNIM002M.search();
    //    });
    //}
    //onDeleteAll = function () {
    //    KBNIM002M.deleteall(function () {
    //        KBNIM002M.search();
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
    //    KBNIM002M.search();
    //});
});
