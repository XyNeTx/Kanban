class MasterTemplate {
    constructor(pConfig = '') {
        this.Controller = pConfig.Controller;
        this.TableName = pConfig.Table;
        this.ColumnTitle = pConfig.ColumnTitle;
        this.ColumnValue = pConfig.ColumnValue;
        this.columnDefs = pConfig.columnDefs;
        this.order = pConfig.order;
        this.orderNo = pConfig.orderNo;
        this.DOM = pConfig.dom;

        this.ModalName = pConfig.Modal;
        this.FormName = pConfig.Form;
        this.PostData = pConfig.PostData;
        this.tblMaster;
        this.onEditCallback = null;
        this.row = null;

    }



    prepare = function () {
        xSplash.show();
        xSplash.text('PREPARING');

        var _addnew = false;
        if (_PERMISSION_.new) {
            _addnew = (config) => {
                this.edit();
            };
            if (typeof (pCallback) === 'function' && this.edit() == undefined) _addnew = false;
        }

        //console.log('xxx'+this.edit());

        this.tblMaster = xDataTable.Initial({
            //this.tblMaster = xDataTable.InitialOld({
            name: this.TableName,
            checking: 0,
            running: 0,
            columnTitle: this.ColumnTitle,
            column: this.ColumnValue,
            columnDefs: this.columnDefs,
            order: this.order,
            orderNo: this.orderNo,
            //orderNo: this.orderNo,
            //dom: '<"clear">',
            addnew: _addnew,
            rowclick: (row) => {
                if (row != undefined) {
                    this.edit(row);
                    xHistory.Data = row;
                }
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

        xSplash.show();
        xSplash.text('DATA INITIALIZING');

        var _PostData = this._reCallPostData();
        //console.log(_PostData);

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: ajexHeader,
            url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/initial',
            data: ajaxPostData(_PostData),
            success: function (result) {
                //console.log(result);

                if (result.response == "OK") {
                    if (typeof (pCallback) === 'function') pCallback(result);

                } else {
                    //console.log(result);
                    if (result.status == 401) xAjax.redirect('../Login/Logout');
                    xSwal.error('Error', 'Master.Initial: ' + result.message);
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
        xSplash.text('DATA LOADING');

        var _PostData = this._reCallPostData();

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: ajexHeader,
            url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/search',
            data: ajaxPostData(_PostData),
            success: function (result) {
                //console.log(result);

                if (result.response == "OK") {
                    //console.log(result);

                    $('#' + _TableName).dataTable().fnClearTable();
                    if (result.data.length > 0) {
                        $('#' + _TableName).dataTable().fnAddData(result.data);

                        //$('#' + _TableName).DataTable().page.len(-1).draw();
                    }

                    xSplash.hide();
                    if (typeof (pCallback) === 'function') pCallback(result);

                } else {
                    //console.log(result);
                    if (result.status == 401) xAjax.redirect('../Login/Logout');
                    xSwal.error('Error', 'Master.Search: ' + result.message);
                    xSplash.hide();
                }

            },
            error: function (result) {
                console.error(_Controller + '.Search: ' + result.responseText);
                xSplash.hide();
            }
        });

    }


    editCallback = function () {
        if ($.isFunction(window.onEdit)) onEdit();
    }



    edit = function (row = null) {

        if ($('#' + this.ModalName).length == 0) return false;

        xSplash.show();
        xSplash.text('DATA LOADING');

        const _ModalName = this.ModalName;
        const _FormName = this.FormName;

        //console.log('DATA LOADING');
        //console.log(row);
        ajexHeader.Records = JSON.stringify(row);
        //console.log(ajexHeader.Records);

        //#### Clear display item ####
        //for disable datetime picker component
        $('#' + _FormName + ' .gj-icon').each(function () {
            if ($(this).parent().prev()[0].hasAttribute('noedit')) $(this).parent().removeAttr('readonly');
            $(this).unbind('click');
        });

        $('#' + _ModalName + 'Label span').text('[NEW]');
        //$('#' + _FormName + ' #btnDelete').attr('disabled', 'disabled');
        $('#' + _FormName + ' input').each(function () {

            if (this.id != '') {

                if ($('#' + this.id).attr('type') == 'color') {
                    //console.log($('#' + this.id).attr('type'));
                    //console.log($('#' + this.id).val());
                } else if ($('#' + this.id).attr('type') == 'checkbox') {
                    $('#' + this.id).prop('checked', false);
                    if (row[this.id] == 1 || row[this.id] == true || row[this.id] == 'true') $("#" + this.id).prop('checked', true);

                    //console.log(this.id+'>>>'+row[this.id]);

                } else {

                    //console.log(this.id + ' >>> ' + $('#' + this.id).attr('default'));
                    this.value = ($('#' + this.id).attr('default') != undefined ? $('#' + this.id).attr('default') : '');
                    //this.value = ($('#' + this.id).attr('default') != undefined ? $('#' + this.id).val() : '');

                    if (document.getElementById(this.id).hasAttribute('noedit')) $('#' + _FormName + ' #' + this.id).removeAttr('readonly');
                }

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
            //$('#' + _FormName + ' #btnDelete').removeAttr('disabled');
            $('#_REFERCODE_').html(row['_ID']);

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




        if (typeof (this.onEditCallback) === 'function') this.onEditCallback(row);


        $('#' + _FormName).removeClass('was-validated');
        $('#' + _ModalName).modal('toggle');




        this.editCallback();

        xSplash.hide();
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

        console.log($('#' + _FormName).serialize());

        xSwal.questionPack(
            i18nLayout.modal.button.swal_save,
            function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('SAVING');

                    $.ajax({
                        type: ($('#' + _FormName + ' #_Action').val() != '' ? $('#' + _FormName + ' #_Action').val() : 'POST'),
                        dataType: "json",
                        headers: ajexHeader,
                        url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/save',
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
                    xSplash.text('DELETING');

                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        headers: ajexHeader,
                        url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/delete',
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
    deleteall = async function (pCallback = '') {
        xSwal.questionPack(
            i18nLayout.modal.button.swal_deleteall,
            async function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('DELETING');

                    var _data = xDataTable.Selected('#tblMaster');
                    for (var i = 0; i < _data.length; i++) {

                        ajexHeader.Records = JSON.stringify(_data[i]);
                        
                        await xAjax.PostAsync({
                            method:'POST',
                            url: _PAGE_ + '/delete',
                            data: null,
                        });
                        
                    }
                    if (typeof (pCallback) === 'function') pCallback();

                    xSplash.hide();
                }

            });

    };


    _reCallPostData = function () {

        var _PostData = ``;
        $.each(this.PostData, function (key, v) {
            v.value = CheckObject(v.value);
            _PostData += (_PostData == `` ? `` : `, `) + `"` + v.name + `" : "` + ($(v.value).val() != undefined ? $(v.value).val() : ReplaceAll(v.value, '#', '')) + `"`;
        });

        return `{` + _PostData + `}`;
    }

}


//const xKBNMS001 = new KBNMS001();
//xKBNMS001.prepare();




xItem.render();



