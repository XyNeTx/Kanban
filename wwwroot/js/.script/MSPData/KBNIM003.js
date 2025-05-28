"use strict";
$(document).ready(function () {
    initial = function () {
        console.log(ajexHeader);
        xAjax.Post({
            url: 'KBNIM003/initial',
            headers: ajexHeader,
            then: function (result) {
                xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
            }
        });
        xItem.reset({
            id: 'pgsStatus',
        });
        xSplash.hide();
    };
    initial();
    var uploadData;
    xAjax.onChange('#fileImport', function () {
        let _file = $('#fileImport').val();
        if (_file == '') {
            xSwal.Info({ "message": 'Please check file before import!!' });
            return;
        }
        xItem.reset({
            id: 'pgsStatus',
        });
        var file = $('#fileImport')[0].files[0];
        var formData = new FormData();
        formData.append("file", file);
        xAjax.PostFile({
            url: 'KBNIM003/Upload',
            data: formData,
            then: function (result) {
                uploadData = result.data;
            }
        });
    });
    xAjax.onClick('#fileImport_button_', function () {
        let _file = $('#fileImport').val();
        if (_file == '') {
            xSwal.Info({ "message": 'Please check file before import!!' });
            return;
        }
        console.log('ready');
        xSwal.question({
            "message": 'Do you want import KANBAN and MSP?',
            "then": function () {
                xTimer.Clock.Start({
                    "start": 100,
                    "counting": function () {
                        //getProgress();
                    }
                });
                var formData = new FormData();
                formData.append("fileName", uploadData.file);
                xAjax.PostFile({
                    url: 'KBNIM003/Import',
                    data: formData,
                    then: function (result) {
                        if (result.response == "OK") {
                            xSwal.success("Success!!", "Import data Complete.");
                            console.log(result);
                        }
                        else {
                            xSwal.error(result.response, result.message);
                        }
                        //xTimer.Clock.Stop({
                        //    "finish": function () {
                        //        getProgress();
                        //    }
                        //})
                    }
                });
            }
        });
    });
    //getProgress = function () {
    //    xAjax.Post({
    //        url: 'KBNIM003/checkProgress',
    //        data: null,
    //        then: function (result) {
    //            xItem.progress({
    //                id: 'pgsStatus',
    //                max: uploadData.count,
    //                current: result.data[0].cnt
    //            })
    //        }
    //    })
    //}
});
