"use strict";
$(document).ready(function () {
    const xKBNIM002TR = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Part No.', 'Customer Order', 'Supplier Code', 'Kanban No.', 'Store Code', 'Part Name', 'Lot Size', 'Qty'],
            "TH": ['Part No.', 'Customer Order', 'Supplier Code', 'Kanban No.', 'Store Code', 'Part Name', 'Lot Size', 'Qty'],
            "JP": ['Part No.', 'Customer Order', 'Supplier Code', 'Kanban No.', 'Store Code', 'Part Name', 'Lot Size', 'Qty'],
        },
        ColumnValue: [
            { "data": "F_Plant" },
            { "data": "F_OrderType" },
            { "data": "F_Effect_Date" },
            { "data": "F_End_Date" },
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
    xKBNIM002TR.prepare();
    xKBNIM002TR.initial(function (result) {
        $('#frmMaster #btnExecute').attr('title', 'Transfer Data');
        xKBNIM002TR.search();
    });
    //onSave = function () {
    //    xKBNIM002TR.save(function () {
    //        xKBNIM002TR.search();
    //    });
    //}
    //onDelete = function () {
    //    xKBNIM002TR.delete(function () {
    //        xKBNIM002TR.search();
    //    });
    //}
    //onDeleteAll = function () {
    //    xKBNIM002TR.deleteall(function () {
    //        xKBNIM002TR.search();
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
    //    xKBNIM002TR.search();
    //});
});
