$(document).ready(function () {

    const KBNMS001 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Plant', 'Order Type', 'Effective Date', 'End Date'],
            "TH": ['Plant', 'Order Type', 'Effective Date', 'End Date'],
            "JP": ['Plant', 'Order Type', 'Effective Date', 'End Date'],
        },
        ColumnValue: [
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


    KBNMS001.prepare();

    KBNMS001.initial(function (result) {
        $('#btnExecute').removeAttr('disabled');
        KBNMS001.search();
    });

    onSave = function () {
        KBNMS001.save(function () {
            KBNMS001.search();
        });
    }

    onDelete = function () {
        KBNMS001.delete(function () {
            KBNMS001.search();
        });
    }

    onDeleteAll = function () {
        KBNMS001.deleteall(function () {
            KBNMS001.search();
        });
    }

    onPrint = function () {
        xDataTableExport.setConfigPDF({
            title: 'OLD TYPE SERVICE CHECK LIST'
        });
    }

    onExecute = function () {
        F_OrderType.readonly = (F_OrderType.readonly == true ? false : true);
        //F_OrderType.label = 'label';
        //console.log(F_Effect_Date.value);
        //F_Effect_Date.value = '2999-12-31';

        console.log(F_Effect_Date);
        console.log(F_Effect_Date.value);
        console.log(F_End_Date.value);
    }

    //xAjax.onChange('#frmCondition #F_Plant', function () {
    //    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

    //    KBNMS001.search();
    //});


});