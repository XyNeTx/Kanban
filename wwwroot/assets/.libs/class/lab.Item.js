class labItem {
    render = function () {

        $('item').each(function () {
            let _type = $(this).attr('type');
            let _id = $(this).attr('id');
            let _label = ($(this).attr('label') != undefined ? $(this).attr('label') : '');
            let _subfix = $(this).attr('subfix');
            let _value = $(this).attr('value');
            let _noedit = $(this).attr('noedit');
            let _format = $(this).attr('format');
            let _default = $(this).attr('default');
            let _readonly = $(this).attr('readonly');
            let _required = $(this).attr('required');
            let _datacontroller = $(this).attr('data-controller');
            let _datavalue = $(this).attr('data-value');
            let _datadescription = $(this).attr('data-description');
            let _classvalue = ($(this).attr('class-value') != undefined ? $(this).attr('class-value') : '');
            _classvalue = (_classvalue == '' ? ($(this).attr('class') != undefined ? $(this).attr('class') : '') : _classvalue);
            let _style = ($(this).attr('style') != undefined ? $(this).attr('style') : '');
            let _title = ($(this).attr('title') != undefined ? $(this).attr('title') : '');
            let _form = $(this).parents('form:first').attr('id');

            //for Progress bar
            let _label_align = ($(this).attr('label-align') != undefined ? $(this).attr('label-align') : 'c');
            let _min = ($(this).attr('min') != undefined ? $(this).attr('min') : '0');
            let _max = ($(this).attr('max') != undefined ? $(this).attr('max') : '100');
            let _current = ($(this).attr('current') != undefined ? $(this).attr('current') : '0');

            //for Radio button
            let _checked = ($(this).attr('checked') != undefined ? 'checked' : '');

            //for File
            let _filter = ($(this).attr('filter') != undefined ? $(this).attr('filter') : '*.*');

            //for textarea
            let _rows = ($(this).attr('rows') != undefined ? $(this).attr('rows') : '4');

            //for checkbox
            let _effect = ($(this).attr('effect') != undefined ? $(this).attr('effect') : 'fade-in-');

            //for DataTable
            let _legend = ($(this).attr('legend') != undefined ? $(this).attr('legend') : ($(this).attr('label') != undefined ? $(this).attr('label') : ''));


            //console.info(_label);

            let _item = '';
            if (_type == 'text') {
                window[_id] = new TextBox(_id);

                if (_label == '') {
                    _item = `
                        <div class="input-group">
                            <input type="text" class="form-control `+ _classvalue + `" id="` + _id + `" name="` + _id + `" `
                        + (_value != undefined ? ' value="' + _value + '"' : '')
                        + (_style != undefined ? ' style="' + _style + '"' : '')
                        + (_readonly != undefined ? ' readonly' : '')
                        + (_noedit != undefined ? ' noedit' : '')
                        + (_required != undefined ? ' required' : '')
                        + `/>`
                        + (_subfix != undefined ? ' <label class="input-group-text" noedit>' + _subfix + '</label>' : '') + ` 
                        </div>
                        `;
                } else {
                    _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <input type="text" class="form-control `+ _classvalue + `" id="` + _id + `" name="` + _id + `" `
                        + (_value != undefined ? ' value="' + _value + '"' : '')
                        + (_style != undefined ? ' style="' + _style + '"' : '')
                        + (_readonly != undefined ? ' readonly' : '')
                        + (_noedit != undefined ? ' noedit' : '')
                        + (_required != undefined ? ' required' : '')
                        + `/>`
                        + (_subfix != undefined ? ' <label class="input-group-text" noedit>' + _subfix + '</label>' : '') + ` 
                        </div>
                        `;
                }

            } if (_type == 'textfloating') {
                window[_id] = new TextBox(_id);

                if (_label == '') {
                    _item = `
                        <div class="form-floating">
                            <input type="text" class="form-control`+ _classvalue + `" id="` + _id + `" name="` + _id + `" `
                        + (_value != undefined ? ' value="' + _value + '"' : '')
                        + (_style != undefined ? ' style="' + _style + '"' : '')
                        + (_readonly != undefined ? ' readonly' : '')
                        + (_noedit != undefined ? ' noedit' : '')
                        + (_required != undefined ? ' required' : '')
                        + `/>
                        </div>
                        `;
                } else {
                    _item = `
                        <div class="form-floating">
                            <input type="text" class="form-control`+ _classvalue + `" id="` + _id + `" name="` + _id + `" `
                        + (_value != undefined ? ' value="' + _value + '"' : '')
                        + (_style != undefined ? ' style="' + _style + '"' : '')
                        + (_readonly != undefined ? ' readonly' : '')
                        + (_noedit != undefined ? ' noedit' : '')
                        + (_required != undefined ? ' required' : '')
                        + `/>
                            <label for="`+ _id + `">` + _label + `</label>
                        </div>
                        `;
                }

            } else if (_type == 'textarea') {
                window[_id] = new TextBox(_id);

                _item = `
                            `+ (_label != '' ? `<label class="custom-input-group-textarea" for="` + _id + `">` + _label + `</label>` : ``) + `
                            <textarea class="custom-form-control `+ _classvalue + `" id="` + _id + `" name="` + _id + `"  rows="` + _rows + `" `
                        + (_style != undefined ? ' style="' + _style + '"' : 'style="width:100%"')
                        + (_readonly != undefined ? ' readonly' : '')
                        + `>`
                        + (_value != undefined ? _value : '')
                        + `</textarea>
                        `;

            } else if (_type == 'number') {
                window[_id] = new TextBox(_id);

                if (_label == '') {
                    _item = `
                        <div class="input-group">
                            <input type="text" class="form-control text-end `+ _classvalue + `" id="` + _id + `" name="` + _id + `" mask="number" `
                        + (_value != undefined ? ' value="' + MaskNumber(_value) + '"' : '')
                        + (_style != undefined ? ' style="' + _style + '"' : '')
                        + (_readonly != undefined ? ' readonly' : '')
                        + (_noedit != undefined ? ' noedit' : '')
                        + (_required != undefined ? ' required' : '')
                        + `/>`
                        + (_subfix != undefined ? ' <label class="input-group-text" noedit>' + _subfix + '</label>' : '') + ` 
                        </div>
                        `;
                } else {
                    _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <input type="text" class="form-control text-end `+ _classvalue + `" id="` + _id + `" name="` + _id + `" mask="number" `
                        + (_value != undefined ? ' value="' + MaskNumber(_value) + '"' : '')
                        + (_style != undefined ? ' style="' + _style + '"' : '')
                        + (_readonly != undefined ? ' readonly' : '')
                        + (_noedit != undefined ? ' noedit' : '')
                        + (_required != undefined ? ' required' : '')
                        + `/>`
                        + (_subfix != undefined ? ' <label class="input-group-text" noedit>' + _subfix + '</label>' : '') + ` 
                        </div>
                        `;
                }
            } else if (_type == 'currency') {
                window[_id] = new TextBox(_id);


                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <input type="text" class="form-control text-end `+ _classvalue + `" id="` + _id + `" name="` + _id + `" mask="currency" `
                    + (_value != undefined ? ' value="' + MaskCurrency(_value, 2) + '"' : '')
                    + (_style != undefined ? ' style="' + _style + '"' : '')
                    + (_readonly != undefined ? ' readonly' : '')
                    + (_noedit != undefined ? ' noedit' : '')
                    + (_required != undefined ? ' required' : '')
                    + `/>`
                    + (_subfix != undefined ? ' <label class="input-group-text" noedit>' + _subfix + '</label>' : '') + ` 
                        </div>
                        `;
            } else if (_type == 'date') {
                window[_id] = new TextBox(_id);

                _item = `
                        <div class="input-group" style="height:28px;">
                            <label class="input-group-text" for="`+ _id + `" noedit>` + _label + `</label>
                            <input type="text" class="form-control col-xl-8" id="`+ _id + `" name="` + _id + `" format="` + _format + `" `
                    + (_default != undefined ? ' default="' + _default + '"' : 'default="now"')
                    + (_value != undefined ? ' value="' + _value + '"' : '')
                    + (_style != undefined ? ' style="' + _style + '"' : '')
                    + (_readonly != undefined ? ' readonly' : '')
                    + (_noedit != undefined ? ' noedit' : '')
                    + (_required != undefined ? ' required' : '')
                    + `/>`
                    + (_subfix != undefined ? ' <label class="input-group-text" noedit>' + _subfix + '</label>' : '') + ` 
                        </div>
                        `;
            } else if (_type == 'time') {
                window[_id] = new TextBox(_id);

                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `" noedit>` + _label + `</label>
                            <input type="text" class="form-control col-xl-8" id="`+ _id + `" name="` + _id + `" format="` + _format + `" `
                    + (_default != undefined ? ' default="' + _default + '"' : 'default="now"')
                    + (_value != undefined ? ' value="' + _value + '"' : '')
                    + (_style != undefined ? ' style="' + _style + '"' : '')
                    + (_readonly != undefined ? ' readonly' : '')
                    + (_noedit != undefined ? ' noedit' : '')
                    + (_required != undefined ? ' required' : '')
                    + `/>`
                    + (_subfix != undefined ? ' <label class="input-group-text" noedit>' + _subfix + '</label>' : '') + ` 
                        </div>
                        `;
            } else if (_type == 'month') {
                window[_id] = new TextBox(_id);

                _item = `
                        <div class="input-group" style="height:28px;">
                            <label class="input-group-text" for="`+ _id + `" noedit>` + _label + `</label>
                            <input type="month" class="form-control col-xl-8" id="`+ _id + `" name="` + _id + `" `
                    + (_default != undefined ? ' value="' + (_default == 'now' ? xDate.Date('yyyy-mm') : _default) + '"' : 'default="now"')
                    + (_value != undefined ? ' value="' + _value + '"' : '')
                    + (_style != undefined ? ' style="' + _style + '"' : '')
                    + `/>`
                    + (_subfix != undefined ? ' <label class="input-group-text" noedit>' + _subfix + '</label>' : '') + ` 
                        </div>
                        `;
            } else if (_type == 'color') {
                window[_id] = new TextBox(_id);

                _item = `
                
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <input type="text" class="form-control `+ _classvalue + `" id="` + _id + `" name="` + _id + `"`
                    + (_style != undefined ? ' style="' + _style + '"' : '')
                    + (_readonly != undefined ? ' readonly' : '')
                    + (_noedit != undefined ? ' noedit' : '')
                    + (_required != undefined ? ' required' : '')
                    + ` />
                            <input type="color" class="form-control col-md-2" id="` + _id + `_Button" name="` + _id + `_Button" />
                    `
                    + (_subfix != undefined ? ' <label class="input-group-text" noedit>' + _subfix + '</label>' : '') + ` 
                        </div >
                        `;
            } else if (_type == 'select') {
                window[_id] = new Select(_id);
                
                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <select class="form-select `+ _classvalue + `" id="` + _id + `" name="` + _id + `" value="` + _value + `" `
                    + (_style != undefined ? ' style="' + _style + '"' : '')
                    + (_readonly != undefined ? ' readonly' : '')
                    + (_noedit != undefined ? ' noedit' : '')
                    + (_required != undefined ? ' required' : '')
                    + ` >
                            </select>
                        </div>
                        `;
            } else if (_type == 'list') {
                window[_id] = new Select(_id);

                _item = `
                        <div class="input-group mb-0">
                            <div class="input-group LOV">
                                <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                                <input type="text" class="form-control `+ _classvalue + `" id="` + _id + `" name="` + _id + `" `
                    + (_style != undefined ? ' style="' + _style + '"' : '')
                    + (_readonly != undefined ? ' readonly' : '')
                    + (_noedit != undefined ? ' noedit' : '')
                    + (_required != undefined ? ' required' : '')
                    + `  />
                                <div class="input-group-text">
                                    <i class="fas fa-list" controller="`+ _datacontroller + `" value="` + _datavalue + `"></i>
                                </div>
                            </div>
                        </div>
                        `;
            } else if (_type == 'lov') {
                window[_id] = new TextBox(_id);

                _item = `
                        <div class="input-group mb-0">
                            <div class="input-group LOV">
                                <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                                <input type="text" class="form-control `+ _classvalue + `" id="` + _id + `" name="` + _id + `" `
                    + (_style != undefined ? ' style="' + _style + '"' : '')
                    + (_readonly != undefined ? ' readonly' : '')
                    + (_noedit != undefined ? ' noedit' : '')
                    + (_required != undefined ? ' required' : '')
                    + ` />
                                <div class="input-group-text">
                                    <i class="fas fa-list" controller="`+ _datacontroller + `" value="` + _datavalue + `"></i>
                                </div>
                                <input type="text" class="form-control" id="`+ _datadescription + `" name="` + _datadescription + `" disabled />
                            </div>
                        </div>
                        `;
            } else if (_type == 'file') {
                window[_id] = new TextBox(_id);

                _item = `
                        <div class="input-group mb-3">`
                    + (_label != undefined && _label != '' ? `<label class="input-group-text" for="` + _id + `">` + _label + `</label>` : ``)
                    + ` <input type="file" class="form-control ` + _classvalue + `" id="` + _id + `" name="` + _id + `" accept="` + _filter + `" />
                            <button type="button" id="` + _id + `_button_" name="` + _id + `_button_" class="btn btn-outline-success border-left-0" style="width:42px;height:29px;border-color:#d1d3e2;color:#858796;cursor:pointer;" title="Upload">
                                <i class="fas fa-upload"></i>
                            </button>
                        </div>
                        `;

            //######### BUTTON #########
            } else if (_type == 'button') {
                window[_id] = new Button(_id);

                _item = `
                        <button type="button" class="btn `+ (_classvalue == '' ? 'btn-success' : _classvalue) + `" id="` + _id + `" name="` + _id + `" title="` + _title + `" ` 
                    + (_style != undefined ? ' style="' + _style + '"' : '')
                    + `>`+ _label + `
                        </button>
                        `;

            } else if (_type == 'progress') {
                window[_id] = new TextBox(_id);

                _label = ReplaceAll(_label, '{{##.##}}', '0.00');

                _item = `
                        <div class="progress">
                            <div class="progress-bar progress-bar-striped progress-bar-animated `+ _classvalue + ` " id="` + _id + `" role="progressbar" style="width: ` + _current + `%" aria-valuenow="` + _current + `" aria-valuemin="` + _min + `" aria-valuemax="` + _max + `" label="` + _label + `"></div>
                        </div>
                        <div id="` + _id + `_label_" style="text-align:` + (_label_align == 'c' ? 'center' : (_label_align == 'l' ? 'left' : (_label_align == 'r' ? 'right' : _label_align))) + `;">` + _label + `</div>
                        `;
            } else if (_type == 'submit') {
                _item = `
                        <button type="submit" class="btn btn-success `+ _classvalue + `" id="` + _id + `" name="` + _id + `" title="` + _title + `" `
                    + (_style != undefined ? ' style="' + _style + '"' : '')
                    + `>`+ _label + `
                        </button>
                        `;
            //} else if (_type == 'progress') {
            //    window[_id] = new TextBox(_id);

            //    _item = `
            //            <div class="progress">
            //                <div class="progress-bar progress-bar-striped progress-bar-animated `+ _classvalue + ` " id="` + _id + `" role="progressbar" style="width: ` + _current + `%" aria-valuenow="` + _current + `" aria-valuemin="` + _min + `" aria-valuemax="` + _max + `" label="` + _label + `"></div>
            //            </div>
            //            <div id="` + _id + `_label_" style="text-align:` + (_label_align == 'c' ? 'center' : (_label_align == 'l' ? 'left' : (_label_align == 'r' ? 'right' : _label_align))) + `;">` + _label + `</div>
            //            `;
            } else if (_type == 'radio') {
                window[_id + _value] = new Radio(_id + _value);
                //window[_id + _value].default = _id;
                window[_id + _value].type = _type;
                window[_id + _value].label = _label;
                window[_id + _value].value = _value;
                window[_id + _value].checked = (_checked == 'checked' ? true : false);

                _item = `
                        <div class="input-group">
                            <div class="form-check">
                                <label>
                                    <input class="form-check-input" type="radio" id="` + _id + _value + `" name="` + _id + `" value="` + _value + `" ` + _checked + ` />
                                    <span id="` + _id + _value + `_label">` + _label + `</span>
                                </label>
                            </div>   
                        </div>   
                        `;
            } else if (_type == 'check' || _type == 'checkbox') {
                window[_id] = new CheckBox(_id);
                //window[_id + _value].type = _type;
                //window[_id + _value].label = _label;
                //window[_id + _value].value = _value;
                //window[_id + _value].checked = (_checked == 'checked' ? true : false);

                //console.log(_id);

                //### <item type="check" id="ToolbarDelete" label="Delete" value="1" class="danger" effect="zoom"></item>
                _effect = 'checkbox-' + (_effect == 'zoom' ? 'zoom zoom-' : 'fade fade-in-') + (_classvalue == '' ? 'primary' : _classvalue);
                _item = `                
                        <div class="` + _effect + `">
                            <label>
                                <input type="checkbox" id="` + _id + `" name="` + _id + `" value="` + _value + `" ` + _checked +`>
                                <span class="cr">
                                    <i class="cr-icon icofont icofont-ui-check txt-default"></i>
                                </span>
                                <span id="` + _id + `_label">` + _label + `</span>
                            </label>
                        </div>
                        `;
            } else if (_type == 'table') {
                /*window[_id] = new table(_id);*/


                //console.log(_legend);
                if (_legend == '') {
                    _item = `
                        <div class="bg-white border rounded-3">
                            <section class="w-100 p-2">
                                <table id="` + _id + `" class="display ` + _classvalue + `" style="width:100%"></table>
                            </section>
                        </div>
                            `;
                } else {
                    _item = `
                        <fieldset class="fieldset">
                            <legend>` + _legend + `</legend>
                            <table id="` + _id + `" class="display ` + _classvalue + `" style="width:100%"></table>
                        </fieldset>
                        `;
                }
            }



            //console.log('Type : ' + _type+ ' >> Label : ' + _label);


            if (_type == 'radio') {

                if (_form != undefined) $(`#` + _form + ` #` + _id + `[value=` + _value + `]`).parent().append(_item);
                if (_form == undefined) $('#' + _id).parent().append(_item);

                if (_checked == 'checked') {
                    window[_id] = new Radio(_id);
                    window[_id].type = _type;
                    window[_id].label = _label;
                    window[_id].value = _value;
                    window[_id].checked = true;
                }

            } else if (_type == 'check' || _type == 'checkbox') {

                if (_form != undefined) $('#' + _form + ' #' + _id).parent().append(_item);
                if (_form == undefined) $('#' + _id).parent().append(_item);

                window[_id].type = _type;
                window[_id].label = _label;
                window[_id].value = _value;

            } else {

                if (_form != undefined) $('#' + _form + ' #' + _id).parent().append(_item);
                if (_form == undefined) $('#' + _id).parent().append(_item);

                window[_id].type = _type;
                window[_id].label = _label;
                window[_id].value = _value;
                window[_id].readonly = (_readonly == 'readonly' ? true : false);
            }


            $(this).remove();


        });






    }


    progress = function (pConfig = null) {
        //console.log(pConfig);

        let _id = CheckObject(pConfig.id);
        let _label = (pConfig.label != undefined ? pConfig.label : $(_id).attr('label'));
        let _min = (pConfig.min != undefined ? pConfig.min : $(_id).attr('aria-valuemin'));
        let _max = (pConfig.max != undefined ? pConfig.max : $(_id).attr('aria-valuemax'));
        let _current = (pConfig.current != undefined ? pConfig.current : '0');

        let _percent = (_current / _max) * 100;

        //console.log(_current);
        //console.log(_max);
        //console.log(_percent);


        $(_id).attr('style', 'width:' + _percent + '%');
        $(_id).attr('aria-valuenow', _current);

        if (_percent >= 100) {
            let _class = $(_id).attr('class');
            _class = ReplaceAll(_class, 'progress-bar-animated', '');
            _class = ReplaceAll(_class, 'progress-bar-striped', '');
            $(_id).attr('class', _class);
            //console.log(_class);
            _percent = 100;
        }

        if (_label != undefined) {
            _label = ReplaceAll(_label, '{{##.##}}', _percent.toFixed(2));
            _label = ReplaceAll(_label, '{{##}}', _percent.toFixed(0));
            //console.log(_label);
            $(_id + `_label_`).html(_label);
        }
    }

    reset = function (pConfig = null) {
        let _id = CheckObject(pConfig.id);
        let _label = $(_id).attr('label');

        $(_id).attr('style', 'width:0%');
        $(_id).attr('aria-valuenow', 0);

        _label = ReplaceAll(_label, '{{##.##}}', 0.00);
        _label = ReplaceAll(_label, '{{##}}', 0);
        //console.log(_label);
        $(_id + `_label_`).html(_label);
    }


}


const xItem = new labItem();