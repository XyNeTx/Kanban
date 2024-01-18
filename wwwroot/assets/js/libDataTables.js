class libDataTable {
    constructor() {
        //console.log("load");
    }


    Initial = function (pConfig = null) {
        //console.log(pConfig);
        var _DELETE_ = false, _NO_ = false;
        var _column = '', _order = [[0, 'asc']];

        for (var c = 0; c < pConfig.column.length; c++) {
            var _strJSON = JSON.stringify(pConfig.column[c]);
            var _columnTitle = null, _columnWidth = null;

            if (i18n.Language == 'EN') _columnTitle = pConfig.columnTitle.EN[c];
            if (i18n.Language == 'TH') _columnTitle = (pConfig.columnTitle.TH != undefined ? pConfig.columnTitle.TH[c] : pConfig.columnTitle.EN[c]);
            if (i18n.Language == 'JP') _columnTitle = (pConfig.columnTitle.JP != undefined ? pConfig.columnTitle.JP[c] : pConfig.columnTitle.EN[c]);

            if (_strJSON.indexOf('_ID') >= 0) {
                _strJSON = _strJSON.replace('}', ', "visible": false, "searchable": false}');
                _order = [[1, 'asc']];
            }
            if (_strJSON.indexOf('RunningNo') >= 0) {
                _strJSON = _strJSON.replace('}', ', "width":"5px", "orderable":false, "searchable": false}');
                _order = [[1, 'asc']];
            }
            if (_strJSON.indexOf('_DELETE_') >= 0) {
                _strJSON = _strJSON.replace('}', ', "width":"5px", "orderable":false, "searchable": false}');
                _order = [[2, 'asc']];
            }

            //if (_columnTitle == '_NO_') {
            //    _NO_ = true;
            //    _columnTitle = `No.`;
            //}
            //if (_columnTitle == '_DELETE_') {
            //    _DELETE_ = true;
            //    _columnTitle = `<input type='checkbox' id='` + pConfig.name + `_DELETEALL' name='` + pConfig.name + `_DELETEALL' onchange='onCheck_DELETEALL(` + pConfig.name + `)' >`;
            //}
            _strJSON = _strJSON.replace('}', ', "title":"' + _columnTitle + '"}');
            //console.log(_columnTitle);

            _column += (_column == '' ? _strJSON : ',' + _strJSON);
            //console.log(_strJSON);
        }
        //console.log(_column);
        _column = JSON.parse('[' + _column + ']');
        //console.log(_order);
        //console.log(JSON.parse(_column));

        _order = (pConfig.order != undefined ? pConfig.order : _order);

        var _pageLength = (pConfig.pageLength != undefined ? pConfig.pageLength : 10);
        var _lengthMenu = (pConfig.optionLength != undefined ? pConfig.optionLength : [
            [10, 25, 50, -1],
            [10, 25, 50, 'All'],
        ]);
        _lengthMenu = (_pageLength == 5 ?
            [
                [5, 10, 25, 50, -1],
                [5, 10, 25, 50, 'All'],
            ]
            : [
                [10, 25, 50, -1],
                [10, 25, 50, 'All'],
            ]
        );


        var _showZero = (pConfig.showZero != undefined ? pConfig.showZero : true);

        var _columnDefs = (pConfig.columnDefs != undefined ? pConfig.columnDefs : null);

        //console.log(pConfig.toolbar);
        var _dom = (pConfig.toolbar != undefined ? (pConfig.toolbar != false ? pConfig.toolbar : 't') : '<"top"lBf>t<"bottom"ip><"clear">')


        var _freezeleft = (pConfig.freezeleft != undefined ? pConfig.freezeleft : 0);
        var _freezeright = (pConfig.freezeright != undefined ? pConfig.freezeright : 0);

        if (pConfig.ordering == false) {
            Object.assign(DataTable.defaults, {
                ordering: false
            });
        }
        if (pConfig.searching == false) {
            Object.assign(DataTable.defaults, {
                searching: false
            });
        }

        //console.log(_column);
        if (pConfig.running >= 0) {
            if (_column[0].data == 'RunningNo') _column.splice(0, 1);
            _order = (pConfig.order != undefined ? pConfig.order : [[pConfig.running + 1, 'asc']]);
            _column.unshift({
                "data": "RunningNo",
                "title": "No.",
                "orderable": false,
                "searchable": false,
                'className': 'dt-body-center col10px'
            });
        }

        if (pConfig.columnDelete) {
            pConfig.running += 1;
            _order = (pConfig.order != undefined ? pConfig.order : [[pConfig.running + 1, 'asc']]);
            _column.unshift({
                "data": "RunningNo",
                "title": "<span style='vertical-align:middle;text-align:center;' ><center><input type='checkbox' id='" + pConfig.name + "_DELETEALL' name='" + pConfig.name + "_DELETEALL' onchange='onCheck_DELETEALL(" + pConfig.name + ")' ></center></span>",
                "width": "5px;",
                "orderable": false,
                "searchable": false
            });

            let _cDef = [
                {
                    'targets': 0,
                    'searchable': false,
                    'orderable': false,
                    'className': 'dt-body-center col5px',
                    'render': function (data, type, full, meta) {
                        //console.log(full.RunningNo);
                        return '<input type="checkbox" id="' + pConfig.name + '_DELETE_" name="' + pConfig.name + '_DELETE_" value="a' + full.RunningNo + '">';
                    }
                }
            ];
            if (_columnDefs != null) _cDef.push(_columnDefs);
            _columnDefs = _cDef;
        }

        //console.log(_column);
        //console.log(_columnDefs);
        ////_column[0].data = '_DELETE_';

        var _dataTable = $('#' + pConfig.name).DataTable({
            "scrollX": true,
            "destroy": true,
            "autoWidth": false,
            "deferRender": true,
            //"stateSave": true,
            "lengthMenu": _lengthMenu,
            "pageLength": _pageLength,
            "language": {
                url: assets + 'vendor/datatables/i18n/' + (i18n.Language == '' ? 'EN' : i18n.Language) + '.json'
            },
            "data": pConfig.data,
            "order": _order,
            "columns": _column,
            "dom": 'Bfrtip',
            "buttons": ['copy', 'csv', 'excel', 'pdf', 'print'],
            "columnDefs": _columnDefs,
            "fixedColumns": {
                left: _freezeleft,
                right: _freezeright
            },
            "drawCallback": function (settings) {

                //### Display [NEW] button
                $('#datatable_button_new').attr('id', 'datatable_' + pConfig.name + '_button_new');

                if (pConfig.addnew == undefined || pConfig.addnew != false) {
                    $('#datatable_' + pConfig.name + '_button_new').removeAttr('style');

                    $('#datatable_' + pConfig.name + '_button_new').on('click', function () {
                        if (pConfig.then != undefined && typeof (pConfig.then) == 'function') {
                            pConfig.addnew(settings);
                        }
                    });
                    //console.log(pConfig.addnew);
                    //} else {
                    //    $('#datatable_' + pConfig.name + '_button_new').attr('style','visibility:hidden;display:none;');
                    //    //pConfig.addnew != false
                    //    console.log('Hidden');
                }


                //### Display running number in column => pConfig.running
                if (pConfig.running != undefined) {
                    let i = 0;
                    _dataTable.cells(null, pConfig.running, { search: 'applied', order: 'applied' }).every(
                        function (cell) {
                            i += 1;
                            this.data(i);
                            //console.log(this.data());
                        });
                }

                //### Callback after draw
                if (pConfig.then != undefined && typeof (pConfig.then) == 'function') {
                    pConfig.then(settings);
                    _dataTable.columns.adjust();
                }


                //console.log($('.dt-body-right currency').html());
                var elCurrency = document.querySelectorAll('[id="' + pConfig.name + '"] tbody .maskcurrency');
                elCurrency.forEach(e => {
                    e.className = ' dt-body-right maskcurrency';
                    e.textContent = MaskCurrency(e.textContent, 2, _showZero);
                });
                elCurrency = document.querySelectorAll('[id="' + pConfig.name + '"] tbody .dt-body-right.currency');
                elCurrency.forEach(e => {
                    e.textContent = MaskCurrency(e.textContent, 2, _showZero);
                });

                var elNumber = document.querySelectorAll('[id="' + pConfig.name + '"] tbody .masknumber');
                elNumber.forEach(e => {
                    e.className = ' dt-body-right masknumber';
                    e.textContent = MaskNumber(e.textContent, _showZero);
                });
                elNumber = document.querySelectorAll('[id="' + pConfig.name + '"] tbody .dt-body-right.number');
                elNumber.forEach(e => {
                    e.textContent = MaskNumber(e.textContent, _showZero);
                });

                var elDate = document.querySelectorAll('[id="' + pConfig.name + '"] tbody .maskdate');
                elDate.forEach(e => {
                    e.className = ' dt-body-center maskdate';
                    e.textContent = MaskDate(e.textContent);
                });
                elDate = document.querySelectorAll('[id="' + pConfig.name + '"] tbody .maskdate-left');
                elDate.forEach(e => {
                    e.className = ' dt-body-left maskdate';
                    e.textContent = MaskDate(e.textContent);
                });
                elDate = document.querySelectorAll('[id="' + pConfig.name + '"] tbody .maskdate-right');
                elDate.forEach(e => {
                    e.className = ' dt-body-right maskdate';
                    e.textContent = MaskDate(e.textContent);
                });


                //xSplash.hide();


                var elRowIcon = document.querySelectorAll('[row-click="true"] tbody tr');
                elRowIcon.forEach(e => {
                    e.style = 'cursor: pointer;' + e.style;
                });

            }
        });


        ////### Callback when click the table row
        $('#' + pConfig.name + ' tbody').on('click', 'tr', function (event) {

            if ($(event.target).is("select") || $(event.target).is("input")) {
                return;
            }

            var _Table = $('#' + pConfig.name).DataTable();
            let data = _Table.row(this).data();
            if (pConfig.rowclick != undefined && typeof (pConfig.rowclick) == 'function') {
                pConfig.rowclick(data);
            }
        });

        return _dataTable;
    }

    RunningColumn = function (pTable, pColumn = 0) {

        //$('#datatable_button_new').attr('style');

        pTable.on('order.dt search.dt', function () {
            let i = 1;


            pTable.cells(null, pColumn, { search: 'applied', order: 'applied' }).every(function (cell) {
                this.data(i++);
                //console.log(cell);
            });
        }).draw();
    }


    ClickRow = function (pTable, pCallback = null) {

        let _tableName = pTable.context[0].nTable.id;

        $('#' + _tableName + ' tbody').on('click', 'tr', function () {
            let data = pTable.row(this).data();
            pCallback(data);
        });
    }

    Summary = function (pTable, pColumn = 0, pMask = 'Currency') {
        let _v = $('#' + pTable).DataTable().column(pColumn).data().sum();

        //if (pMask.toLocaleLowerCase() == 'currency') return MaskCurrency(_v);
        //if (pMask.toLocaleLowerCase() == 'number') return MaskNumber(_v);
        return _v;
    }

}

onCheck_DELETEALL = function (pTable) {
    let _tid = $(pTable)[0].id;
    let allPages = $('#' + _tid).DataTable().cells().nodes();

    if ($('input[id=' + _tid + '_DELETEALL]').prop('checked')) {
        $(allPages).find('input[type="checkbox"]').prop('checked', true);
    } else {
        $(allPages).find('input[type="checkbox"]').prop('checked', false);
    }
}


var xDataTable = new libDataTable();






