$(document).ready(function () {

    var _i18n = '@ViewData["UserLanguage"].ToString()';
    //xi18n.initial(i18n['@ViewData["Action"].ToString()']);

    var data = [
        {
            "Sort": "A",
            "Detail": "TMT Forecast",
            "PCS": "196",
            "KB": "",
            "T1": "98",
            "T2": "",
            "T3": "98"
        },
        {
            "Sort": "B",
            "Detail": "HMMT Prod.Forecast",
            "PCS": "211",
            "KB": "",
            "T1": "71",
            "T2": "70",
            "T3": "70"
        },
        {
            "Sort": "C",
            "Detail": "HMMT Order Plan",
            "PCS": "211",
            "KB": "",
            "T1": "71",
            "T2": "70",
            "T3": "70"
        },
        {
            "Sort": "D",
            "Detail": "Production Volumn",
            "PCS": "201",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Sort": "E",
            "Detail": "MRP (Actual Production)",
            "PCS": "186",
            "KB": "",
            "T1": "62",
            "T2": "62",
            "T3": "62"
        },
        {
            "Sort": "F",
            "Detail": "Diff Last MRP vs PC",
            "PCS": "0",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Sort": "G",
            "Detail": "Abnormal Part",
            "PCS": "0",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Sort": "H",
            "Detail": "Total",
            "PCS": "201",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Sort": "I",
            "Detail": "Remain From Last Trip",
            "PCS": "4",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Sort": "J",
            "Detail": "Order Base",
            "PCS": "197",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Sort": "K",
            "Detail": "Total Order",
            "PCS": "200",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Sort": "L",
            "Detail": "KB Cut(-)/Add(+)",
            "PCS": "",
            "KB": "0",
            "T1": "0",
            "T2": "0",
            "T3": "0"
        },
        {
            "Sort": "M",
            "Detail": "Actual Order",
            "PCS": "200",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Sort": "N",
            "Detail": "Urgent (Pcs.)",
            "PCS": "0",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Sort": "O",
            "Detail": "Adjust Order/Trip",
            "PCS": "",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Sort": "P",
            "Detail": "Receive Plan",
            "PCS": "160",
            "KB": "8",
            "T1": "3",
            "T2": "3",
            "T3": "2"
        },
        {
            "Sort": "Q",
            "Detail": "BL (Plan)",
            "PCS": "220",
            "KB": "",
            "T1": "218",
            "T2": "216",
            "T3": "194"
        },
        {
            "Sort": "R",
            "Detail": "Receive Actual",
            "PCS": "160",
            "KB": "8",
            "T1": "2",
            "T2": "3",
            "T3": "3"
        },
        {
            "Sort": "S",
            "Detail": "BL (Actual)",
            "PCS": "220",
            "KB": "",
            "T1": "198",
            "T2": "196",
            "T3": "194"
        }
    ];


    var tblMaster = xDataTable.Initial({
        name: 'tblMaster',
        data: data,
        running: 0,
        freezeleft: 1,
        toolbar: false,
        ordering: false,
        columnTitle: {
            "EN": ['', 'MRP+20%', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3'],
            "TH": ['', 'MRP+20%', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3'],
        },
        column:
            [
                { "data": "Sort" },
                { "data": "Detail" },
                { "data": "PCS" },
                { "data": "KB" },
                { "data": "T1" },
                { "data": "T2" },
                { "data": "T3" },
                { "data": "PCS" },
                { "data": "KB" },
                { "data": "T1" },
                { "data": "T2" },
                { "data": "T3" },
                { "data": "PCS" },
                { "data": "KB" },
                { "data": "T1" },
                { "data": "T2" },
                { "data": "T3" },
                { "data": "PCS" },
                { "data": "KB" },
                { "data": "T1" },
                { "data": "T2" },
                { "data": "T3" },
                { "data": "PCS" },
                { "data": "KB" },
                { "data": "T1" },
                { "data": "T2" },
                { "data": "T3" },
                { "data": "PCS" },
                { "data": "KB" },
                { "data": "T1" },
                { "data": "T2" },
                { "data": "T3" },
                { "data": "PCS" },
                { "data": "KB" },
                { "data": "T1" },
                { "data": "T2" },
                { "data": "T3" },
                { "data": "PCS" },
                { "data": "KB" },
                { "data": "T1" },
                { "data": "T2" },
                { "data": "T3" },
                { "data": "PCS" },
                { "data": "KB" },
                { "data": "T1" },
                { "data": "T2" },
                { "data": "T3" },
                { "data": "PCS" },
                { "data": "KB" },
                { "data": "T1" },
                { "data": "T2" },
                { "data": "T3" }
            ],
        columnDefs: [
            { target: 0, visible: false, searchable: false }
        ],
        then: function (config) {
            //var _style = $('#tblMaster tbody tr').attr('style', 'font-size:10px;padding:2px;');
            //$('.dataTables_scrollHeadInner table thead tr th').attr('style', 'border-style:solid;border-color:red;color:red;bor');
            $('#tblMaster tbody tr td').attr('style', 'font-size:10px;padding:2px;');
            //console.log(_style);



            $('.thTop').each(function () {
                let _html = $(this)[0].innerHTML;
                let _color = (_html == '17-08-2023' ? 'background-color:orange;' : (_html > '17-08-2023' ? 'background-color:green;color:white;' : ''));
                _color = (_html.indexOf('MRP') >= 0 ? '' : _color);

                $(this).attr('style', 'text-align:center;' + _color);
            });

            $('th').each(function () {
                let _html = $(this)[0].innerHTML;
                let _style = $(this).attr('style');
                //console.log(_html);
                let _color = (_html == 'D' ? 'background-color:orange;color:white;' : (_html == 'N' ? 'background-color:violet;color:white;' : _style));
                //_color = (_html.indexOf('MRP') >= 0 ? '' : _color);

                $(this).attr('style', 'text-align:center;' + _color);
            });




            $('#tblMaster tbody tr td').each(function () {
                let _html = $(this)[0].innerHTML;
                //console.log(_html);

                if (_html == 'BL (Plan)') {
                    console.log(_html)
                }

                //let _style = $(this).attr('style');
                //console.log(_html);
                //let _color = (_html == 'D' ? 'background-color:orange;color:white;' : (_html == 'N' ? 'background-color:violet;color:white;' : _style));
                ////_color = (_html.indexOf('MRP') >= 0 ? '' : _color);

                //$(this).attr('style', 'text-align:center;' + _color);
            });




            xSplash.hide();
        }
    });

    $('.form-floating label').attr('style', 'font-size:10px;height: 32px;');
    $('.form-floating input').attr('style', 'font-size:10px;height: 32px;');






})

