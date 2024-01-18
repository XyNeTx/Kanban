$(document).ready(function () {

    initial = function () {
        xSplash.hide();
    }
    initial();







    xAjax.onClick('#txtFile_button_', function () {
        let _file = $('#txtFile').val();
        if (_file == '') {
            xSwal.Info({ "message": 'Please browse file name for Import Delivery Time Table!!!' });
            return;
        }

        xSwal.question({
            "message": 'Do you want Import Delivery Time Table?',
            "then": function () {
                xAjax.Post({
                    url: 'KBNLC150/Import',
                    data: {
                        "Plant": _PLANT_,
                        "Period": ReplaceAll($('#frmCondition #F_Production_Month').val(),'-',''),
                    },
                    then: function(result){
                        console.log(result);
                    }
                })
            }
        })

    })









    var _t = 0;
    var _x = xTimer.Stoper({
        start: 200,
        stop: 27800,
        counting: function (_c) {
            //console.log(_c);

            //xItem.progress({
            //    id: 'pgsStatus',
            //    //min: _c.start,
            //    //max: 1000,
            //    current: _c.current/100
            //})
        }
    })
});