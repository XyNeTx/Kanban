"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
$(document).ready(function () {
    initial = function () {
        $('#itmRedLine').val(`Order < Average/Trip  -  (Average/Trip * 0.3) = RED 
BL < ForecastMax * SafetyStock * 0.6 = RED`);
        $('#itmBlueLine').val(`Order > Average/Trip  +  (Average/Trip * 0.3) = BLUE 
BL > ForecastMax * SafetyStock * 1.4 = BLUE`);
        xAjax.Post({
            url: 'KBNMS005/initial',
            //data: '',
            then: function (result) {
                console.log(result);
                xDropDownList.bind('#frmCondition #itmSupplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');
            }
        });
        xSplash.hide();
    };
    initial();
    xAjax.onChange('#frmCondition #itmSupplier', function () {
        return __awaiter(this, void 0, void 0, function* () {
            //console.log('xXx');
            var dt = yield xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNMS005_KANBAN]",
                    "@pPlant": ajexHeader.Plant,
                    "@pSupplier": $('#itmSupplier').val(),
                    "@pSupplierTo": $('#itmSupplier').val()
                },
            });
            console.log(dt);
        });
    });
    //const KBNMS005 = new MasterTemplate({
    //    Controller: _PAGE_,
    //    Table: 'tblMaster',
    //    ColumnTitle: {
    //        "EN": ['No.', 'Supplier_Code', 'Supplier_Plant', 'Part_No', 'Ruibetsu', 'Kanban_No'],
    //        "TH": ['No.', 'Plant', 'F_Parent_Part', 'F_Part_Name', 'Effective Date', 'End Date'],
    //    },
    //    ColumnValue: [
    //        { "data": "Supplier_Code" },
    //        { "data": "Supplier_Code" },
    //        { "data": "Supplier_Plant" },
    //        { "data": "Part_No" },
    //        { "data": "Ruibetsu" },
    //        { "data": "Kanban_No" }
    //    ],
    //    Modal: 'modalMaster',
    //    Form: 'frmMaster',
    //    PostData: [
    //        { name: 'F_Plant', value: _PLANT_ },
    //        { name: 'F_Supplier', value: '#frmCondition #F_Supplier' },
    //        { name: 'F_Start_Date', value: '#frmCondition #F_Start_Date' },
    //        { name: 'F_End_Date', value: '#frmCondition #F_End_Date' }
    //    ],
    //});
    //KBNMS005.prepare();
    //KBNMS005.initial(function (result) {
    //    //console.log(result);
    //    //xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
    //    xDropDownList.bind('#frmCondition #F_Supplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');
    //    //xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
    //    KBNMS005.search();
    //});
    //onSave = function () {
    //    KBNMS005.save(function () {
    //        KBNMS005.search();
    //    });
    //}
    //onDelete = function () {
    //    KBNMS005.delete(function () {
    //        KBNMS005.search();
    //    });
    //}
    //onDeleteAll = function () {
    //    KBNMS005.deleteall(function () {
    //        KBNMS005.search();
    //    });
    //}
    //onPrint = function () { }
    //onExecute = function () { }
    ////xAjax.onChange('#frmCondition #F_Supplier', function () {
    ////    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());
    ////    KBNMS005.search();
    ////});
});
