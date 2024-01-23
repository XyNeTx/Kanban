
var _path = (window.location.port != '' ?
    window.location.protocol + '//' + window.location.hostname + ':' + window.location.port :
    window.location.protocol + '//' + window.location.hostname + '/' + namespace);

//console.log(_path);

function _onClickTheme(_theme) {
    //console.log(_theme);

    setCookie("theme", _theme, 90);
    _changeTheme();
}

function _onClickStyle(_style = 'default') {
    //console.log(_style);

    setCookie("theme_style", _style, 90);
    _changeTheme();
}
function _changeTheme() {
    theme = getCookie("theme");
    theme_style = getCookie("theme_style");

    $('#theme').attr('href', _path+'/assets/vendor/css/rtl/core' + (theme == 'light' ? '' : '-' + theme) + '.css');
    $('#theme_style').attr('href', _path+'/assets/vendor/css/rtl/theme-' + theme_style + (theme == 'light' ? '' : '-' + theme) + '.css');

    //console.log('theme : ' + theme + ', style : ' + theme_style + (theme == 'light' ? '' : '-' + theme));
}



var theme = getCookie("theme");
var theme_style = getCookie("theme_style");
//console.log('theme>>' + theme);

if (theme == '') {
    setCookie("theme", "light", 90);
    setCookie("theme_style", "default", 90);

    theme = getCookie("theme");
    theme_style = getCookie("theme_style");

}
_changeTheme();
//$('#theme').attr('href', '/assets/vendor/css/rtl/core' + theme + '.css');
//$('#theme_style').attr('href', '/assets/vendor/css/rtl/' + theme_style + theme+  '.css');
