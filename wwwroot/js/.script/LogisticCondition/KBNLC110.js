"use strict";
$(document).ready(function () {
    const xKBNMS001 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Flag', 'Plant', 'Order Type', 'Effective Date', 'End Date'],
            "TH": ['Flag', 'Plant', 'Order Type', 'Effective Date', 'End Date'],
            "JP": ['Flag', 'Plant', 'Order Type', 'Effective Date', 'End Date'],
        },
        ColumnValue: [
            { "data": "F_Plant" },
            { "data": "F_Plant" },
            { "data": "F_OrderType" },
            { "data": "F_Effect_Date" },
            { "data": "F_End_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });
    xKBNMS001.prepare();
    xKBNMS001.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xKBNMS001.search();
    });
    onSave = function () {
        xKBNMS001.save(function () {
            xKBNMS001.search();
        });
    };
    onDelete = function () {
        xKBNMS001.delete(function () {
            xKBNMS001.search();
        });
    };
    onDeleteAll = function () {
        xKBNMS001.deleteall(function () {
            xKBNMS001.search();
        });
    };
    onPrint = function () {
        xDataTableExport.setConfigPDF({
            title: 'OLD TYPE SERVICE CHECK LIST'
        });
    };
    onExecute = function () {
        console.log('onExecute');
    };
    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());
        xKBNMS001.search();
    });
});
