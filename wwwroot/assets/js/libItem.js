class labItem {
    render = function () {

        $('item').each(function () {
            let _type = $(this).attr('type');
            let _id = $(this).attr('id');
            let _label = $(this).attr('label');
            let _value = $(this).attr('value');
            let _noedit = $(this).attr('noedit');
            let _format = $(this).attr('format');
            let _default = $(this).attr('default');
            let _readonly = $(this).attr('readonly');
            let _required = $(this).attr('required');
            let _datacontroller = $(this).attr('data-controller');
            let _datavalue = $(this).attr('data-value');
            let _datadescription = $(this).attr('data-description');
            let _classvalue = $(this).attr('class-value');
            let _form = $(this).parents('form:first').attr('id');
            
            //console.log(_noedit);

            let _item = '';
            if (_type == 'text') {
                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <input type="text" class="form-control `+ _classvalue +`" id="`+ _id + `" name="` + _id + `" `
                                + (_readonly != undefined ? ' readonly' : '')
                                + (_noedit != undefined ? ' noedit' : '')
                                + (_required != undefined ? ' required' : '')
                                + `/>
                        </div>
                        `;
            } else if (_type == 'number') {
                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <input type="text" class="form-control text-end `+ _classvalue + `" id="` + _id + `" name="` + _id + `" `
                    + (_value != undefined ? ' value="' + MaskNumber(_value) + '"' : '')
                    + (_readonly != undefined ? ' readonly' : '')
                    + (_noedit != undefined ? ' noedit' : '')
                    + (_required != undefined ? ' required' : '')
                    + `/>
                        </div>
                        `;
            } else if (_type == 'currency') {
                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <input type="text" class="form-control text-end `+ _classvalue + `" id="` + _id + `" name="` + _id + `" `
                    + (_value != undefined ? ' value="' + MaskCurrency(_value) + '"' : '')
                    + (_readonly != undefined ? ' readonly' : '')
                    + (_noedit != undefined ? ' noedit' : '')
                    + (_required != undefined ? ' required' : '')
                    + `/>
                        </div>
                        `;
            } else if (_type == 'date') {
                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `" noedit>` + _label + `</label>
                            <input type="text" class="form-control col-xl-8" id="`+ _id + `" name="` + _id + `" format="` + _format + `" default="` + _default + `" `
                                + (_readonly != undefined ? ' readonly' : '')
                                + (_noedit != undefined ? ' noedit' : '')
                                + (_required != undefined ? ' required' : '')
                                + ` />
                        </div>
                        `;
            } else if (_type == 'time') {
                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `" noedit>` + _label + `</label>
                            <input type="text" class="form-control col-xl-8" id="`+ _id + `" name="` + _id + `" format="` + _format + `" default="` + _default + `" `
                                + (_readonly != undefined ? ' readonly' : '')
                                + (_noedit != undefined ? ' noedit' : '')
                                + (_required != undefined ? ' required' : '')
                                + ` />
                        </div>
                        `;
            } else if (_type == 'color') {
                _item = `
                
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <input type="text" class="form-control `+ _classvalue +`" id="` + _id + `" name="` + _id + `"`
                                + (_readonly != undefined ? ' readonly' : '')
                                + (_noedit != undefined ? ' noedit' : '')
                                + (_required != undefined ? ' required' : '')
                                + ` />
                            <input type="color" class="form-control col-md-2" id="` + _id + `_Button" name="` + _id + `_Button">
                        </div>
                        `;
            } else if (_type == 'select') {
                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <select class="form-select `+ _classvalue +`" id="`+ _id + `" name="` + _id + `" value="` + _value + `" `
                                + (_readonly != undefined ? ' readonly' : '')
                                + (_noedit != undefined ? ' noedit' : '')
                                + (_required != undefined ? ' required' : '')
                                + ` >
                            </select>
                        </div>
                        `;
            } else if (_type == 'list') {
                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <div class="input-group LOV">
                                <input type="text" class="form-control `+ _classvalue +`" id="`+ _id + `" name="` + _id + `" `
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
                _item = `
                        <div class="input-group">
                            <label class="input-group-text" for="`+ _id + `">` + _label + `</label>
                            <div class="input-group LOV">
                                <input type="text" class="form-control `+ _classvalue +`" id="`+ _id + `" name="` + _id + `" `
                                    + (_readonly != undefined ? ' readonly' : '')
                                    + (_noedit != undefined ? ' noedit' : '')
                                    + (_required != undefined ? ' required' : '')
                                    + ` />
                                <div class="input-group-text">
                                    <i class="fas fa-list" controller="`+ _datacontroller + `" value="` + _datavalue + `"></i>
                                </div>
                                <input type="text" class="form-control" id="`+ _datadescription + `" name="` + _datadescription + `" disabled>
                            </div>
                        </div>
                        `;
            }
            $('#' + _form + ' #' + _id).parent().append(_item);
            $(this).remove();

        });


    }
}


const xItem = new labItem();