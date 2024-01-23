$(document).ready(function () {

    const xKBNMS028 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Dock Code', 'LP1', 'Remark1', 'LP2', 'Remark2', 'LP3', 'Remark3', 'LP4', 'Remark4', 'LP5', 'Remark5', 'LP6', 'Remark6', 'LP7', 'Remark7', 'LP8', 'Remark8', 'LP9', 'Remark9', 'LP10', 'Remark10', 'LP11', 'Remark11', 'LP12', 'Remark12', 'LP13', 'Remark13', 'LP14', 'Remark14', 'LP15', 'Remark15', 'LP16', 'Remark16'],
            "TH": ['Dock Code', 'LP1', 'Remark1', 'LP2', 'Remark2', 'LP3', 'Remark3', 'LP4', 'Remark4', 'LP5', 'Remark5', 'LP6', 'Remark6', 'LP7', 'Remark7', 'LP8', 'Remark8', 'LP9', 'Remark9', 'LP10', 'Remark10', 'LP11', 'Remark11', 'LP12', 'Remark12', 'LP13', 'Remark13', 'LP14', 'Remark14', 'LP15', 'Remark15', 'LP16', 'Remark16'],
            "JP": ['Dock Code', 'LP1', 'Remark1', 'LP2', 'Remark2', 'LP3', 'Remark3', 'LP4', 'Remark4', 'LP5', 'Remark5', 'LP6', 'Remark6', 'LP7', 'Remark7', 'LP8', 'Remark8', 'LP9', 'Remark9', 'LP10', 'Remark10', 'LP11', 'Remark11', 'LP12', 'Remark12', 'LP13', 'Remark13', 'LP14', 'Remark14', 'LP15', 'Remark15', 'LP16', 'Remark16'],
        },
        ColumnValue: [
            { "data": "F_Dock_Cd" },
            { "data": "F_short_Logistic1" },
            { "data": "F_Remark1" },
            { "data": "F_short_Logistic2" },
            { "data": "F_Remark2" },
            { "data": "F_short_Logistic3" },
            { "data": "F_Remark3" },
            { "data": "F_short_Logistic4" },
            { "data": "F_Remark4" },
            { "data": "F_short_Logistic5" },
            { "data": "F_Remark5" },
            { "data": "F_short_Logistic6" },
            { "data": "F_Remark6" },
            { "data": "F_short_Logistic7" },
            { "data": "F_Remark7" },
            { "data": "F_short_Logistic8" },
            { "data": "F_Remark8" },
            { "data": "F_short_Logistic9" },
            { "data": "F_Remark9" },
            { "data": "F_short_Logistic10" },
            { "data": "F_Remark10" },
            { "data": "F_short_Logistic11" },
            { "data": "F_Remark11" },
            { "data": "F_short_Logistic12" },
            { "data": "F_Remark12" },
            { "data": "F_short_Logistic13" },
            { "data": "F_Remark13" },
            { "data": "F_short_Logistic14" },
            { "data": "F_Remark14" },
            { "data": "F_short_Logistic15" },
            { "data": "F_Remark15" },
            { "data": "F_short_Logistic16" },
            { "data": "F_Remark16" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS028.prepare();

    xKBNMS028.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS028.search();
    });

    onSave = function () {
        xKBNMS028.save(function () {
            xKBNMS028.search();
        });
    }

    onDelete = function () {
        xKBNMS028.delete(function () {
            xKBNMS028.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS028.deleteall(function () {
            xKBNMS028.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS028.search();
    });





})

