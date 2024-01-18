class KBNMS001 {
    constructor(pConfig = '') {
        this.Controller = pConfig.Controller;
        this.PostData = pConfig.PostData;
        this.tblMaster;
        this.row = null;
    }

    prepare = function () {
        xSplash.show();
        xSplash.text('Preparing...');

        this.tblMaster = xDataTable.Initial({
            name: 'tblMaster',
            running: 0,
            columnTitle: {
                "EN": ['No.', 'Flag', 'Plant', 'Order Type', 'Effective Date', 'End Date'],
                "TH": ['No.', 'Flag', 'Plant', 'Order Type', 'Effective Date', 'End Date'],
            },
            column:
                [
                    { "data": "RunningNo" },
                    { "data": "F_Plant" },
                    { "data": "F_Plant" },
                    { "data": "F_OrderType" },
                    { "data": "F_Effect_Date" },
                    { "data": "F_End_Date" }
                ],
            addnew: (config) => {
                this.edit();
            },
            rowclick: (row) => {
                this.edit(row);
                xHistory.Data = row;
            },

            //addnew: function (config) {
            //    this.edit();
            //}.bind(this),
            //rowclick: function (row) {
            //    this.edit(row);
            //    xHistory.Data = row;
            //}.bind(this),
            then: function (config) {
                xSplash.hide();
            }
        });
    }


    initial = function (pCallback = '') {
        xSplash.show();
        xSplash.text('Data initializing...');
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: ajexHeader,
            url: (NameSpace != '' ? '/' + NameSpace : '') + '/' + this.Controller + '/initial',
            success: function (result) {

                if (result.response == "OK") {
                    if (typeof (pCallback) === 'function') pCallback(result);  

                } else {
                    xSwal.error('Error', result.message);
                }

                xSplash.hide();
            },
            error: function () {
                console.error('KBNMS001.Initial: ' + result.responseText);
            }
        });

    }


    search = function (pCallback = '') {
        xSplash.show();
        xSplash.text('Data loading...');

        var _PostData = this._reCallPostData();        

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: ajexHeader,
            url: (NameSpace != '' ? '/' + NameSpace : '') + '/' + this.Controller + '/search',
            data: ajaxPostData(_PostData),
            success: function (result) {
                //console.log(result);

                if (result.response == "OK") {

                    $('#tblMaster').dataTable().fnClearTable();
                    if (result.data.length > 0) {
                        $('#tblMaster').dataTable().fnAddData(result.data);
                    }
                    if (typeof (pCallback) === 'function') pCallback(result);

                }
                xSplash.hide();

            },
            error: function () {
                console.error('KBNMS001.Save: ' + result.responseText);
            }
        });

    }





    edit = function (row = null) {

        if (row == null) {
            $('#modalMasterLabel span').text('[NEW]');
            $('#frmMaster #btnDelete').attr('disabled', 'disabled');
            $('#frmMaster input').each(function () {
                this.value = '';
            });

            $("#frmMaster select").each(function () {
                if (this.id != '') {
                    var _value = $("#frmMaster #" + this.id).attr('value');
                    $("#frmMaster #" + this.id).val(_value);
                }
            });

            $('#frmMaster #_Action').val('POST');

        } else {
            $('#modalMasterLabel span').text('[' + row.Code + ']');
            $('#frmMaster #btnDelete').removeAttr('disabled');
            $('#_refcode').html(row['F_Plant']);

            $.each(row, function (k, v) {
                $('#frmMaster #' + k).val(xAjax.trim(v));
            });

            $('#frmMaster #_Action').val('PATCH');
        }


        $('#frmMaster').removeClass('was-validated');
        $('#modalMaster').modal('toggle');
    }





    save = function (pCallback = '') {

        const _frmModal = document.getElementById("frmMaster");
        if (!_frmModal.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
            _frmModal.classList.add('was-validated');
            return false;
        }

        xSwal.questionPack(
            i18nLayout.modal.button.swal_save,
            function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('Saving...');

                    $.ajax({
                        type: ($('#frmMaster #_Action').val() != '' ? $('#frmMaster #_Action').val() : 'POST'),
                        dataType: "json",
                        headers: ajexHeader,
                        url: (NameSpace != '' ? '/' + NameSpace : '') + '/KBNMS001/save',
                        data: $("#frmMaster").serialize(),
                        success: function (result) {

                            $('#modalMaster').modal('hide');
                            xSplash.hide();
                            if (typeof (pCallback) === 'function') pCallback(result);
                        },
                        error: function () {
                            console.error('KBNMS001.Save: ' + result.responseText);
                        }
                    });

                }
            });

    }




    delete = function (pCallback = '') {

        xSwal.questionPack(
            i18nLayout.modal.button.swal_delete,
            function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('Deleting...');

                    $.ajax({
                        type: "DELETE",
                        dataType: "json",
                        headers: ajexHeader,
                        url: (NameSpace != '' ? '/' + NameSpace : '') + '/KBNMS001/delete',
                        data: $("#frmMaster").serialize(),
                        success: function (result) {

                            $('#modalMaster').modal('hide');
                            xSplash.hide();
                            if (typeof (pCallback) === 'function') pCallback(result);
                        },
                        error: function () {
                            console.error('KBNMS001.Delete: ' + result.responseText);
                        }
                    });
                }

            });
    }
























    _reCallPostData = function () {

        var _PostData = `{`;
        $.each(this.PostData, function (key, v) {
            _PostData += `"` + v.name + `" : "` + $(`#`+v.value).val() + `"`;
        });
        _PostData += `}`;

        return _PostData;
    }

}


//const xKBNMS001 = new KBNMS001();
//xKBNMS001.prepare();