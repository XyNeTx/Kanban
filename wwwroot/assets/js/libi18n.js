class labi18n {
    constructor() {
    }

    initial(pi18n = '',) {
        //console.log(pi18n);

        xi18n._checkObject(pi18n);

        $.each(pi18n, function (key, value) {

            $('#' + key + ' h4').each(function () {
                $(this).html(pi18n[key]['h4']);
            });

            $('#' + key + ' label').each(function () {
                var _item = $(this).prop('for');
                $(this).html(pi18n[key]['label'][_item]);
            });


            $('#' + key + ' button').each(function () {
                var _id = $(this).attr('id');

                if (pi18n[key]['tab'] != undefined) {
                    if (_id != undefined) $(this).html(pi18n[key]['tab'][_id]);
                } else if (pi18n[key]['button'] != undefined) {
                    if (_id != undefined) $(this).html(pi18n[key]['button'][_id]);
                }
                
            });
        });


    }

    _checkObject(pObject, pTag = '') {
        $.each(pObject, function (key, value) {

            if (typeof (value) == 'object') {
                //console.log('key=>' + key + ' value=>' + value);
                xi18n._checkObject(value, key);

            } else if (typeof (value) == 'string') {
                if (key == 'h1') $('#frmCriteria ' + key).html(value);
                if (key == 'h2') $('#frmCriteria ' + key).html(value);
                if (key == 'h3') $('#frmCriteria ' + key).html(value);
                if (key == 'h4') $('#frmCriteria ' + key).html(value);
                if (key == 'h5') $('#frmCriteria ' + key).html(value);
                if (pTag == 'label') $('#frmCriteria ' + key).html(value);
                //console.log('Tag=>' + pTag + '; frmCriteria ' + key + ' => ' + value);

            }
        });
    }
}
const xi18n = new labi18n();


//console.log(i18nLayout);
var _urlLayout = assets + 'i18n/' + i18n.Language + '/Layout.json';
//console.log(assets);
$.ajax({
    url: _urlLayout,
    type: 'HEAD',
    success: function () {
        $.getJSON(_urlLayout, function (data) {
            i18nLayout = data;
        });
    },
    error: function () {
        console.error('Cannot load file: ' + _urlLayout);
        //console.clear();
        //console.log('Clear with cannot load file: ' + _urlLayout);
    }
});


if (i18n.Action != undefined) {
    //console.log(i18n.Action);
    //var _url = assets + 'i18n/' + i18n.Language + '/' + (system == '' ? '' : system + '/') + i18n.Action + '.json';
    var _url = assets + 'i18n/' + i18n.Language + '/' + (system == '' ? '' : system + '/') + i18n.Controller + '/' + i18n.Action + '.json';
    $.ajax({
        url: _url,
        type: 'HEAD',
        //beforeSend: function (xhr) {
        //    xhr.overrideMimeType('application/json');
        //},
        success: function () {
            // File exists, proceed with $.getJSON()
            $.getJSON(_url, function (data) {
                i18n = data;
            });
        },
        error: function () {
            console.error('Cannot load file: ' + _url);
            //console.clear();
            //console.log('Clear with cannot load file: ' + _url);
        }
    });


}
//$.getJSON(assets + 'i18n/' + i18n.Language + '/' + (system == '' ? '' : system + '/') + i18n.Action + '.json', function (data) {
//    i18n = data;
//}).fail(function () {
//    //console.log(assets + 'i18n/' + i18n.Language + '/' + (system == '' ? '' : system + '/') + i18n.Action + '.json');
////    console.error('Can not load file i18n [i18n].');
//});

//$.getJSON(assets +'i18n/' + i18n + '.json', function (data) {
//    i18n = data;
//    //console.log(data);
//}).fail(function () {

//    swalShow('Can not load file i18n [@ViewData["UserLanguage"]].');
//    //console.log('Can not load file i18n [@ViewData["UserLanguage"]].');
//});


function swal_i18n(_obj, _swal) {

    var _string = '';
    var _object = eval('i18n.' + _obj);

    if (_obj.indexOf('.swal_save') >= 0) {
        console.log(_obj);

        if (typeof _object === 'object') _string = eval('i18n.' + _obj + '.' + _swal);
        if (_string != '') return _string;
        if (_swal.indexOf('title') >= 0) return i18n.modal.button.swal_save.title;
        if (_swal.indexOf('text') >= 0) return i18n.modal.button.swal_save.text;
        if (_swal.indexOf('cancel') >= 0) return i18n.modal.button.swal_save.cancel;
        if (_swal.indexOf('save') >= 0) return i18n.modal.button.swal_save.save;
        if (_swal.indexOf('delete') >= 0) return i18n.modal.button.swal_save.delete;
        return '';
    }


    if (_obj.indexOf('swal_delete') >= 0) {
        console.log(_obj);

        if (typeof _object === 'object') _string = eval('i18n.' + _obj + '.' + _swal);
        if (_string != '') return _string;
        if (_swal.indexOf('title') >= 0) return i18n.modal.button.swal_delete.title;
        if (_swal.indexOf('text') >= 0) return i18n.modal.button.swal_delete.text;
        if (_swal.indexOf('cancel') >= 0) return i18n.modal.button.swal_delete.cancel;
        if (_swal.indexOf('save') >= 0) return i18n.modal.button.swal_delete.save;
        if (_swal.indexOf('delete') >= 0) return i18n.modal.button.swal_delete.delete;
        return '';
    }

}




function swal_i18n_save(_object) {
    console.log(_object);

    var _string = eval('i18n.' + _object);

    if (_string == undefined) {

        if (_object.indexOf('.title') > 0) return i18n.modal.button.swal_save.title;
        if (_object.indexOf('.text') > 0) return i18n.modal.button.swal_save.text;
        if (_object.indexOf('.cancel') > 0) return i18n.modal.button.swal_save.cancel;
        if (_object.indexOf('.save') > 0) return i18n.modal.button.swal_save.save;
        if (_object.indexOf('.delete') > 0) return i18n.modal.button.swal_save.delete;
        return '';
    }
    //console.log(_string);
    return _string;
}

function swal_i18n_delete(_object) {
    console.log(_object);

    var _string = eval('i18n.' + _object);

    if (_string == undefined) {

        if (_object.indexOf('.title') > 0) return i18n.modal.button.swal_delete.title;
        if (_object.indexOf('.text') > 0) return i18n.modal.button.swal_delete.text;
        if (_object.indexOf('.cancel') > 0) return i18n.modal.button.swal_delete.cancel;
        if (_object.indexOf('.save') > 0) return i18n.modal.button.swal_delete.save;
        if (_object.indexOf('.delete') > 0) return i18n.modal.button.swal_delete.delete;
        return '';
    }
    //console.log(_string);
    return _string;
}




