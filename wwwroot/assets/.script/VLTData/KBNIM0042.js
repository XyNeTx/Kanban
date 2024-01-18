$(document).ready(function () {

    initial = function () {


        var tblFrame = xDataTable.Initial({
            name: 'tblFrame',
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Cut', 'Jig In Seq', 'Part Code'],
                "TH": ['Cut', 'Jig In Seq', 'Part Code'],
                "JP": ['Cut', 'Jig In Seq', 'Part Code'],
            },
            column: [
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_OrderType" }
            ],
            addnew: false,
            rowclick: (row) => {
            },
            then: function (config) {
                //xSplash.hide();
            }
        });

        var tblSidePanel = xDataTable.Initial({
            name: 'tblSidePanel',
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Cut', 'Jig In Seq', 'Part Code'],
                "TH": ['Cut', 'Jig In Seq', 'Part Code'],
                "JP": ['Cut', 'Jig In Seq', 'Part Code'],
            },
            column: [
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_OrderType" }
            ],
            addnew: false,
            rowclick: (row) => {
            },
            then: function (config) {
                //xSplash.hide();
            }
        });

        var tblTailGate = xDataTable.Initial({
            name: 'tblTailGate',
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Cut', 'Jig In Seq', 'Part Code'],
                "TH": ['Cut', 'Jig In Seq', 'Part Code'],
                "JP": ['Cut', 'Jig In Seq', 'Part Code'],
            },
            column: [
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_OrderType" }
            ],
            addnew: false,
            rowclick: (row) => {
            },
            then: function (config) {
                //xSplash.hide();
            }
        });

        var tblRRAxle = xDataTable.Initial({
            name: 'tblRRAxle',
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Cut', 'Jig In Seq', 'Part Code'],
                "TH": ['Cut', 'Jig In Seq', 'Part Code'],
                "JP": ['Cut', 'Jig In Seq', 'Part Code'],
            },
            column: [
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_OrderType" }
            ],
            addnew: false,
            rowclick: (row) => {
            },
            then: function (config) {
                //xSplash.hide();
            }
        });

        xSplash.hide();
    }
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
        })

        var file = $('#fileImport')[0].files[0];
        var formData = new FormData();
        formData.append("file", file);

        xAjax.PostFile({
            url: 'KBNIM003/Upload',
            data: formData,
            then: function (result) {
                uploadData = result.data;                
            }
        })
    })



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

                xTimer.Clock.Start(
                    {
                        "start": 100,
                        "counting": function () {
                            getProgress();

                        }
                    })

                var formData = new FormData();
                formData.append("fileName", uploadData.file);

                xAjax.PostFile({
                    url: 'KBNIM003/Import',
                    data: formData,
                    then: function (result) {

                        console.log(result);

                        xTimer.Clock.Stop({
                            "finish": function () {
                                getProgress();
                            }
                        })
                    }
                })
            }
        })

    })



    getProgress = function () {
        xAjax.Post({
            url: 'KBNIM003/checkProgress',
            data: null,
            then: function (result) {

                xItem.progress({
                    id: 'pgsStatus',
                    max: uploadData.count,
                    current: result.data[0].cnt
                })

            }
        })
    }

});