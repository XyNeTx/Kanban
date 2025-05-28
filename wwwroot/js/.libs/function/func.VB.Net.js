"use strict";
var MsgBoxStyle = {
    "Critical": "MsgBoxStyle.Critical",
    "Exclamation": "MsgBoxStyle.Exclamation",
    "Information": "MsgBoxStyle.Information",
    "Question": "MsgBoxStyle.Question",
    "OkCancel": "MsgBoxStyle.Question",
    "OkOnly": "MsgBoxStyle.Information"
};
var MsgBoxResult = {
    "Abort": "MsgBoxResult.Abort",
    "Cancel": "MsgBoxResult.Cancel",
    "Ignore": "MsgBoxResult.Ignore",
    "No": "MsgBoxResult.No",
    "Ok": "MsgBoxResult.Ok",
    "Retry": "MsgBoxResult.Retry",
    "Yes": "MsgBoxResult.Yes"
};
function MsgBox(Prompt = null, MsgBoxStyle = MsgBoxStyle.Information, Title = null, Cancel = null) {
    if (MsgBoxStyle == 'MsgBoxStyle.Critical') {
        if (typeof (Title) === 'function')
            xSwal.error('ERROR', Prompt, Title);
        if (typeof (Title) !== 'function')
            xSwal.error((Title == '' || Title == null ? 'ERROR' : Title), Prompt);
    }
    if (MsgBoxStyle == 'MsgBoxStyle.Exclamation')
        xSwal.warning(Title, Prompt);
    if (MsgBoxStyle == 'MsgBoxStyle.Information')
        xSwal.info((Title == '' || Title == null ? 'Information' : Title), Prompt);
    if (MsgBoxStyle == 'MsgBoxStyle.Question') {
        if (typeof (Title) == 'function') {
            xSwal.question((Title == '' ? '' : Title), Prompt, function (result) {
                Title(result);
                return MsgBoxResult.Ok;
            }, function (result) {
                if (typeof (Cancel) == 'function')
                    Cancel(result);
                return MsgBoxResult.Cancel;
            });
        }
    }
}
//class labVB {
//    constructor() {
//    }
//    Format = function (pDate, pFormat='yyyyMMdd') {
//        return(pFormat)
//    }
//}
//xVB = new labVB();
