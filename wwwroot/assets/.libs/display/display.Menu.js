

$(document).ready(function () {

    //##### Set menu to active/focus #####
    var _lv1 = $('li[data-focus=active]').parent().parent().attr('dropdown-icon');
    var _lv2 = $('li[data-focus=active]').parent().parent().parent().parent().attr('dropdown-icon');
    var _lv3 = $('li[data-focus=active]').parent().parent().parent().parent().parent().parent().attr('dropdown-icon');
    var _lv4 = $('li[data-focus=active]').parent().parent().parent().parent().parent().parent().parent().parent().attr('dropdown-icon');
    var _lv5 = $('li[data-focus=active]').parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().attr('dropdown-icon');

    if (_lv1 != undefined) $('li[data-focus=active]').parent().parent().attr('class', 'pcoded-hasmenu active pcoded-trigger');
    ////Parent level 2
    if (_lv2 != undefined) $('li[data-focus=active]').parent().parent().parent().parent().attr('class', 'pcoded-hasmenu pcoded-trigger');
    ////Parent level 3
    if (_lv3 != undefined) $('li[data-focus=active]').parent().parent().parent().parent().parent().parent().attr('class', 'pcoded-hasmenu pcoded-trigger');
    ////Parent level 4
    if (_lv4 != undefined) $('li[data-focus=active]').parent().parent().parent().parent().parent().parent().parent().parent().attr('class', 'pcoded-hasmenu pcoded-trigger');
    ////Parent level 5
    if (_lv5 != undefined) $('li[data-focus=active]').parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().attr('class', 'pcoded-hasmenu pcoded-trigger');


    //if (_CONTROLLER_ + _PAGE_ != 'HomeIndex')
    //    if (document.getElementById(_CONTROLLER_ + _PAGE_) != null) $('#mCSB_1_container').attr('style', 'position: relative; top: -' + document.getElementById(_CONTROLLER_ + _PAGE_).offsetTop + 'px; left: 0px; width: 100%;');


    //if (_MENUFOCUS_ != '') {
    //    $('#' + _CONTROLLER_ + _MENUFOCUS_).attr('data-focus', 'active');
    //    $('#' + _CONTROLLER_ + _MENUFOCUS_).attr('class', 'active');
    //    //console.log(_CONTROLLER_ + _MENUFOCUS_);
    //}

});



