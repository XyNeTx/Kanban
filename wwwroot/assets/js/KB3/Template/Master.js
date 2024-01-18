class MasterTemplate {
    constructor(pConfig = '') {
        this.Controller = pConfig.Controller;
        this.TableName = pConfig.Table;
        this.ColumnTitle = pConfig.ColumnTitle;
        this.ColumnValue = pConfig.ColumnValue;

        this.ModalName = pConfig.Modal;
        this.FormName = pConfig.Form;
        this.PostData = pConfig.PostData;
        this.tblMaster;
        this.onEditCallback = null;
        this.row = null;
    }



    prepare = function () {
        xSplash.show();
        xSplash.text('Preparing...');

        var _addnew = false;
        if (_PERMISSION_.new) {
            _addnew = (config) => {
                this.edit();
            };
        }

        this.tblMaster = xDataTable.Initial({
            name: this.TableName,
            running: 0,
            columnTitle: this.ColumnTitle,
            column: this.ColumnValue,
            columnDelete: true,
            addnew: _addnew,
            rowclick: (row) => {
                this.edit(row);
                xHistory.Data = row;
            },
            //columnDefs: [
            //    {
            //        'targets': 3,
            //        'searchable': false,
            //        'orderable': false,
            //    }
            //],
            //addnew: function (config) {
            //    this.edit();
            //}.bind(this),
            //rowclick: function (row) {
            //    this.edit(row);
            //    xHistory.Data = row;
            //}.bind(this),
            then: function (config) {
                //xSplash.hide();
            }
        });
    }




    initial = function (pCallback = '') {
        const _Controller = this.Controller;

        //console.log(_HOSTNAME_);

        xSplash.show();
        xSplash.text('Data initializing...');
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: ajexHeader,
            url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/initial',
            success: function (result) {
                if (result.response == "OK") {
                    if (typeof (pCallback) === 'function') pCallback(result);

                } else {
                    xSwal.error('Error', result.message);
                    xSplash.hide();
                }

            },
            error: function (result) {
                console.error(_Controller + '.Initial: ' + result.responseText);
                xSplash.hide();
            }
        });

    }


    search = function (pCallback = '') {
        const _Controller = this.Controller;
        const _TableName = this.TableName;

        xSplash.show();
        xSplash.text('Data loading...');

        var _PostData = this._reCallPostData();

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: ajexHeader,
            url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/search',
            data: ajaxPostData(_PostData),
            success: function (result) {

                if (result.response == "OK") {
                    //console.log(result);

                    $('#' + _TableName).dataTable().fnClearTable();
                    if (result.data.length > 0) {
                        $('#' + _TableName).dataTable().fnAddData(result.data);
                    }
                    xSplash.hide();
                    if (typeof (pCallback) === 'function') pCallback(result);

                }

            },
            error: function (result) {
                console.error(_Controller + '.Search: ' + result.responseText);
                xSplash.hide();
            }
        });

    }





    edit = function (row = null) {
        const _ModalName = this.ModalName;
        const _FormName = this.FormName;

        //console.log(row);

        //#### Clear display item ####
        //for disable datetime picker component
        $('#' + _FormName + ' .gj-icon').each(function () {
            if ($(this).parent().prev()[0].hasAttribute('noedit')) $(this).parent().removeAttr('readonly');
            $(this).unbind('click');
        });

        $('#' + _ModalName + 'Label span').text('[NEW]');
        $('#' + _FormName + ' #btnDelete').attr('disabled', 'disabled');
        $('#' + _FormName + ' input').each(function () {

            if ($('#' + this.id).attr('type') == 'color') {
                //console.log($('#' + this.id).attr('type'));
                //console.log($('#' + this.id).val());
            } else {

                //console.log(this.id + ' >>> ' + $('#' + this.id).attr('default'));
                //this.value = ($('#' + this.id).attr('default') != undefined ? $('#' + this.id).attr('default') : '');
                this.value = ($('#' + this.id).attr('default') != undefined ? $('#' + this.id).val() : '');

                if (document.getElementById(this.id).hasAttribute('noedit')) $('#' + _FormName + ' #' + this.id).removeAttr('readonly');
            }
        });

        $('#' + _FormName + ' select').each(function () {
            if (this.id != '') {
                var _value = $('#' + _FormName + ' #' + this.id).attr('value');
                $('#' + _FormName + ' #' + this.id).val(_value);

                if ($('#' + _FormName + ' #' + this.id)[0].hasAttribute('noedit')) $('#' + _FormName + ' #' + this.id).removeAttr('readonly');
            }
        });




        if (row == null) {

            $('#' + _FormName + ' #_Action').val('POST');

        } else {
            $('#' + _ModalName + 'Label span').text('[' + 'EDIT' + ']');
            $('#' + _FormName + ' #btnDelete').removeAttr('disabled');
            $('#_refcode').html(row['F_Plant']);

            $.each(row, function (k, v) {
                $('#' + _FormName + ' #' + k).val(xAjax.trim(v));

                if ($('#' + _FormName + ' #' + k)[0] != undefined)
                    if ($('#' + _FormName + ' #' + k)[0].hasAttribute('noedit')) $('#' + _FormName + ' #' + k).attr('readonly', 'readonly');

            });

            //for disable datetime picker component
            $('#' + _FormName + ' .gj-icon').each(function () {
                if ($(this).parent().prev()[0].hasAttribute('noedit')) $(this).parent().attr('readonly', 'readonly');
            });


            $('#' + _FormName + ' #_Action').val('PATCH');
        }

        $('#' + _FormName + ' label').attr('readonly', 'readonly');


        $('#' + _FormName + ' label').each(function () {
            var _id = $(this).attr('for');
            $(this).insertBefore('#' + _FormName + ' #' + _id);
        });




        if (typeof (this.onEditCallback) === 'function') this.onEditCallback();


        $('#' + _FormName).removeClass('was-validated');
        $('#' + _ModalName).modal('toggle');
    }





    save = function (pCallback = '') {
        const _Controller = this.Controller;
        const _ModalName = this.ModalName;
        const _FormName = this.FormName;

        const _frmModal = document.getElementById(_FormName);
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
                        type: ($('#' + _FormName + ' #_Action').val() != '' ? $('#' + _FormName + ' #_Action').val() : 'POST'),
                        dataType: "json",
                        headers: ajexHeader,
                        url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller +'/save',
                        data: $('#' + _FormName).serialize(),
                        success: function (result) {

                            $('#' + _ModalName).modal('hide');
                            xSplash.hide();
                            if (typeof (pCallback) === 'function') pCallback(result);
                        },
                        error: function (result) {
                            console.error(_Controller + '.Save: ' + result.responseText);
                            xSplash.hide();
                        }
                    });

                }
            });

    }




    delete = function (pCallback = '') {
        const _Controller = this.Controller;
        const _ModalName = this.ModalName;
        const _FormName = this.FormName;

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
                        url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller +'/delete',
                        data: $('#' + _FormName).serialize(),
                        success: function (result) {

                            $('#' + _ModalName).modal('hide');
                            xSplash.hide();
                            if (typeof (pCallback) === 'function') pCallback(result);
                        },
                        error: function (result) {
                            console.error(_Controller + '.Delete: ' + result.responseText);
                            xSplash.hide();
                        }
                    });
                }

            });
    }



    //$('#btnToolbarDelete').on('click', function() {
    deleteall = function (pCallback = '') {

        var _delData = [];
        var allPages = $('#tblMaster').DataTable().cells().nodes();
        $(allPages).find('input[type="checkbox"]').each(function () {
            if ($(this).prop('checked')) {
                var _x = $(this).parent().parent().find('.dt-body-center.col10px')[0].innerHTML;
                _delData.push($('#tblMaster').DataTable().rows(_x - 1).data()[0]);
            }
        });
        if (_delData.length <= 0) {
            xSwal.Info(i18nLayout.modal.button.swal_delete_info.title, i18nLayout.modal.button.swal_delete_info.text);
            return;
        }


        xSwal.questionPack(
            i18nLayout.modal.button.swal_delete,
            function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('Deleting...');

                    console.log(_delData);

                    xSplash.hide();

                    //$.ajax({
                    //    type: "DELETE",
                    //    dataType: "json",
                    //    headers: ajexHeader,
                    //    url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/delete',
                    //    data: $('#' + _FormName).serialize(),
                    //    success: function (result) {

                    //        $('#' + _ModalName).modal('hide');
                    //        xSplash.hide();
                    //        if (typeof (pCallback) === 'function') pCallback(result);
                    //    },
                    //    error: function (result) {
                    //        console.error(_Controller + '.Delete: ' + result.responseText);
                    //        xSplash.hide();
                    //    }
                    //});
                }

            });

    };

    _reCallPostData = function () {

        var _PostData = ``;
        $.each(this.PostData, function (key, v) {
            v.value = CheckObject(v.value);
            _PostData += (_PostData == `` ? `` : `, `) + `"` + v.name + `" : "` + $(v.value).val() + `"`;
        });

        return `{` + _PostData + `}`;
    }

}


//const xKBNMS001 = new KBNMS001();
//xKBNMS001.prepare();


xItem.render();