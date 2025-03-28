$(document).ready(function () {
    var _i18n = '@ViewData["UserLanguage"].ToString()';

    var data = [
        {
            "Detail": "TMT Forecast",
            "PCS": "196",
            "KB": "",
            "T1": "98",
            "T2": "",
            "T3": "98"
        },
        {
            "Detail": "HMMT Prod.Forecast",
            "PCS": "211",
            "KB": "",
            "T1": "71",
            "T2": "70",
            "T3": "70"
        },
        {
            "Detail": "HMMT Order Plan",
            "PCS": "211",
            "KB": "",
            "T1": "71",
            "T2": "70",
            "T3": "70"
        },
        {
            "Detail": "Production Volumn",
            "PCS": "201",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Detail": "MRP (Actual Production)",
            "PCS": "186",
            "KB": "",
            "T1": "62",
            "T2": "62",
            "T3": "62"
        },
        {
            "Detail": "Diff Last MRP vs PC",
            "PCS": "0",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Detail": "Abnormal Part",
            "PCS": "0",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Detail": "Total",
            "PCS": "201",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Detail": "Remain From Last Trip",
            "PCS": "4",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Detail": "Order Base",
            "PCS": "197",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Detail": "Total Order",
            "PCS": "200",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Detail": "KB Cut(-)/Add(+)",
            "PCS": "",
            "KB": "0",
            "T1": "0",
            "T2": "0",
            "T3": "0"
        },
        {
            "Detail": "Actual Order",
            "PCS": "200",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Detail": "Urgent (Pcs.)",
            "PCS": "0",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Detail": "Adjust Order/Trip",
            "PCS": "",
            "KB": "",
            "T1": "",
            "T2": "",
            "T3": ""
        },
        {
            "Detail": "Receive Plan",
            "PCS": "160",
            "KB": "8",
            "T1": "3",
            "T2": "3",
            "T3": "2"
        },
        {
            "Detail": "BL (Plan)",
            "PCS": "220",
            "KB": "",
            "T1": "218",
            "T2": "216",
            "T3": "194"
        },
        {
            "Detail": "Receive Actual",
            "PCS": "160",
            "KB": "8",
            "T1": "2",
            "T2": "3",
            "T3": "3"
        },
        {
            "Detail": "BL (Actual)",
            "PCS": "220",
            "KB": "",
            "T1": "198",
            "T2": "196",
            "T3": "194"
        }
    ];

    var _rdoMRPMore20 = `
                        <div class="input-group">
                            <div class="form-check">
                                <label>
                                    <input class="form-check-input" type="radio" id="rdoMRPMRPMore20" name="rdoMRP" value="MRPMore20">
                                    <span>MRP+20%</span>
                                </label>
                            </div>   
                        </div>
    `;

    var tblMaster = xDataTable.Initial({
        name: 'tblMaster',
        //data: data,
        running: -1,
        freezeleft: 1,
        toolbar: false,
        dom: '<"clear">',
        scrollbar: 2200,
        ordering: false,
        columnTitle: {
            "EN": [_rdoMRPMore20, 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3'],
            "TH": [_rdoMRPMore20, 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3'],
            "JP": [_rdoMRPMore20, 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3', 'Pcs', 'KB', 'T1', 'T2', 'T3'],
        },
        column:
            [
                { "data": "Detail" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" },
                { "data": "PCS" }
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

    var srtAction = 'preview'
    var _arrSupplier, _arrPartNo, _arrPartNoTo;

    var _dtPartControl, _dtDeliveryDate, _dtDate, _dtPeriod;
    var _dtHeader, _dtDetail, _dtVolume, _dtAdjustOrderTrip, _dtActualReceive;
    var _charStartDate, _charEndDate, _intAmountShow;
    var _dateDelivery, _intDeliveryTrip;

    xAjax.onClick('btnPreview', async function () {

        if ($('#itmSupplier').val() == null) {
            MsgBox("Please Select Supplier Before Process Data", MsgBoxStyle.Critical, "DATA ERROR");
            return;
        }
        if ($('#itmKanban').val() > $('#itmKanbanTo').val()) {
            MsgBox("Please Select Kanban No. Again Before Process Data", MsgBoxStyle.Critical, "DATA ERROR");
            return;
        }
        if ($('#itmStoreCode').val() > $('#itmStoreCodeTo').val()) {
            MsgBox("Please Select Store Code Again Before Process Data", MsgBoxStyle.Critical, "DATA ERROR");
            return;
        }
        if ($('#itmPartNo').val() > $('#itmPartNoTo').val()) {
            MsgBox("Please Select Part No. Again Before Process Data", MsgBoxStyle.Critical, "DATA ERROR");
            return;
        }

        _arrSupplier = $('#itmSupplier').val().split('-');

        srtAction = 'preview';
        Find_StartEnd_Date();      // '@Remark : Find Start/End Date
        Get_ALL_Data();     //'@Remark : Get Data fill in DT



        //console.log($('#itmPartNo').val());

        _arrPartNo = ($('#itmPartNo').val() != null ? $('#itmPartNo').val().split('-') : '');
        _arrPartNoTo = ($('#itmPartNoTo').val() != null ? $('#itmPartNoTo').val().split('-') : '');


        if (_dtHeader.rows != null) {
            _dtHeader.rows.forEach(async function (item) {
                //console.log(item);




            });
        }


    });





    ////'1. Find Start/End Date
    Find_StartEnd_Date = async function () {
        try {

            if (srtAction == 'preview') {
                var _dt = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[CKD_Inhouse].[sp_NumberOfDayToPreview]",
                        "@Plant": _PLANT_,
                        "@Supplier_Code": _arrSupplier[0],
                        "@Supplier_Plant": _arrSupplier[1],
                        "@Store_Code": '1A',
                        "@Date": ReplaceAll(_PROCESSDATE_, '-', '')
                        //"@Date": '20230918'
                    },
                });
                //console.log(_dt);
                if (_dt.rows != null) _charStartDate = _dt.rows[0].Start_Date;
                if (_dt.rows != null) _charEndDate = _dt.rows[0].End_Date;
                if (_dt.rows != null) _intAmountShow = _dt.rows[0].Display_Date;
            } else if (srtAction == 'preview') {
                var _dt = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[CKD_Inhouse].[sp_NumberOfDayToSearch]",
                        "@Plant": _PLANT_,
                        "@Supplier_Code": _arrSupplier[0],
                        "@Supplier_Plant": _arrSupplier[1],
                        "@Date": xDate.Now('yyyyMMdd'),
                        "@Shift": (_SHIFT_ == 1 ? 'D' : 'N'),
                        "@UserName": _DISPLAY_
                    },
                });
                //console.log(_dt);
                if (_dt.rows != null) _charStartDate = _dt.rows[0].Start_Date;
                if (_dt.rows != null) _charEndDate = _dt.rows[0].End_Date;
                if (_dt.rows != null) _intAmountShow = _dt.rows[0].Display_Date;
            }


            _dtDeliveryDate = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[CKD_Inhouse].[sp_getDeliveryDateTrip]",
                    "@Plant": _PLANT_,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@ProcessDate": xDate.Now('yyyyMMdd'),
                    "@ProcessShift": (_SHIFT_ == 1 ? 'D' : 'N')
                },
            });
            //console.log(_dtDeliveryDate);
            if (_dtDeliveryDate.rows != null) _dateDelivery = _dtDeliveryDate.rows[0].F_Delivery_Date;
            if (_dtDeliveryDate.rows != null) _intDeliveryTrip = _dtDeliveryDate.rows[0].F_Delivery_Trip;


            _dtDate = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[dbo].[sp_getCycleTime]",
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@StartDate": _charStartDate,
                    "@EndDate": _charEndDate
                },
            });
            //'Check Start_Date and End_Date for Support ECI Case
            if (_dtDate.rows != null) {
                if (_charStartDate != _dtDate.rows[0].F_Date) _charStartDate = _dtDate.rows[0].F_Date;
                if (_charEndDate != _dtDate.rows[_dtDate.rows.length - 1].F_Date) _charEndDate = _dtDate.rows[_dtDate.rows.length - 1].F_Date;
                _intAmountShow = _dtDate.rows.length;
            }



            for (var i = 0; i < _intAmountShow; i++) {
                _dtPeriod = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[dbo].[sp_findPeriod]",
                        "@Plant": _PLANT_,
                        "@Supplier_Code": _arrSupplier[0],
                        "@Supplier_Plant": _arrSupplier[1],
                        "@dateNow": _dtDate.rows[0].F_Date,
                        "@UserName": _DISPLAY_
                    },
                });
            }

        } catch (error) {
            MsgBox("Error Sub : Find_StartEnd_Date() " + error.Message)
        }

    }


    ////'2. Get Data
    Get_ALL_Data = async function () {
        try {
            _dtPartControl = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[CKD_Inhouse].[sp_DT_PartControl]",
                    "@StartDate": _charStartDate,
                    "@EndDate": _charEndDate,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@Part_No_FROM": _arrPartNo[0],
                    "@Part_No_TO": _arrPartNo[1],
                    "@Ruibetsu_FROM": _arrPartNoTo[0],
                    "@Ruibetsu_TO": _arrPartNoTo[1],
                    "@Kanban_No_FROM": $('#itmKanban').val(),
                    "@Kanban_No_TO": $('#itmKanbanTo').val(),
                    "@Store_Code_FROM": $('#itmStoreCode').val(),
                    "@Store_Code_TO": $('#itmStoreCodeTo').val(),
                    "@OrderType": ''
                },
            });
            if (_dtPartControl.rows == null) {
                MsgBox("No Data PartControl", MsgBoxStyle.Critical, "DATA ERROR")
                return false;
            } else {
                //If sender.Text = "Re-Calculate BL" OrElse sender.Text = "Re-Calculate" Then
                //get_startDate(sender)
                //End If
            }


            //'@Remark : Get Data to DT_Header
            _dtHeader = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[CKD_Inhouse].[sp_DT_Header]",
                    "@StartDate": _charStartDate,
                    "@EndDate": _charEndDate,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@Part_No_FROM": _arrPartNo[0],
                    "@Part_No_TO": _arrPartNo[1],
                    "@Ruibetsu_FROM": _arrPartNoTo[0],
                    "@Ruibetsu_TO": _arrPartNoTo[1],
                    "@Kanban_No_FROM": $('#itmKanban').val(),
                    "@Kanban_No_TO": $('#itmKanbanTo').val(),
                    "@Store_Code_FROM": $('#itmStoreCode').val(),
                    "@Store_Code_TO": $('#itmStoreCodeTo').val(),
                    "@OrderType": ''
                },
            });
            if (_dtHeader.rows == null) {
                MsgBox("No Data", MsgBoxStyle.Critical, "DATA ERROR")
                return false;
            }


            //'@Remark : Get Data to DT_Detail
            _dtDetail = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[CKD_Inhouse].[sp_DT_Detail]",
                    "@StartDate": _charStartDate,
                    "@EndDate": _charEndDate,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@Part_No_FROM": _arrPartNo[0],
                    "@Part_No_TO": _arrPartNo[1],
                    "@Ruibetsu_FROM": _arrPartNoTo[0],
                    "@Ruibetsu_TO": _arrPartNoTo[1],
                    "@Kanban_No_FROM": $('#itmKanban').val(),
                    "@Kanban_No_TO": $('#itmKanbanTo').val(),
                    "@Store_Code_FROM": $('#itmStoreCode').val(),
                    "@Store_Code_TO": $('#itmStoreCodeTo').val(),
                    "@OrderType": ''
                },
            });
            if (_dtDetail.rows == null) {
                MsgBox("No Data", MsgBoxStyle.Critical, "DATA ERROR")
                return false;
            }


            //'@Remark : Get Data to DT_Volume
            _dtVolume = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[CKD_Inhouse].[sp_DT_Volume]",
                    "@StartDate": _charStartDate,
                    "@EndDate": _charEndDate,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@Part_No_FROM": _arrPartNo[0],
                    "@Part_No_TO": _arrPartNo[1],
                    "@Ruibetsu_FROM": _arrPartNoTo[0],
                    "@Ruibetsu_TO": _arrPartNoTo[1],
                    "@Kanban_No_FROM": $('#itmKanban').val(),
                    "@Kanban_No_TO": $('#itmKanbanTo').val(),
                    "@Store_Code_FROM": $('#itmStoreCode').val(),
                    "@Store_Code_TO": $('#itmStoreCodeTo').val(),
                    "@OrderType": ''
                },
            });
            if (_dtVolume.rows == null) {
                MsgBox("No Data", MsgBoxStyle.Critical, "DATA ERROR")
                return false;
            }


            //'@Remark : Get Data to DT_AdjustOrder_Trip
            _dtAdjustOrderTrip = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[dbo].[sp_DT_AdjustOder_Trip]",
                    "@StartDate": _charStartDate,
                    "@EndDate": _charEndDate,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@Part_No_FROM": _arrPartNo[0],
                    "@Part_No_TO": _arrPartNo[1],
                    "@Ruibetsu_FROM": _arrPartNoTo[0],
                    "@Ruibetsu_TO": _arrPartNoTo[1],
                    "@Kanban_No_FROM": $('#itmKanban').val(),
                    "@Kanban_No_TO": $('#itmKanbanTo').val(),
                    "@Store_Code_FROM": $('#itmStoreCode').val(),
                    "@Store_Code_TO": $('#itmStoreCodeTo').val(),
                    "@OrderType": ''
                },
            });
            if (_dtAdjustOrderTrip.rows == null) {
                MsgBox("No Data", MsgBoxStyle.Critical, "DATA ERROR")
                return false;
            }


            //'@Remark : Get Data to DT_Actual_Receive : [hmmt-ppm].[PPMDB].[dbo].[T_Receive_Local] rl
            _dtActualReceive = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[dbo].[sp_DT_Actual_Receive]",
                    "@StartDate": _charStartDate,
                    "@EndDate": _charEndDate,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@Part_No_FROM": _arrPartNo[0],
                    "@Part_No_TO": _arrPartNo[1],
                    "@Ruibetsu_FROM": _arrPartNoTo[0],
                    "@Ruibetsu_TO": _arrPartNoTo[1],
                    "@Store_Code_FROM": $('#itmStoreCode').val(),
                    "@Store_Code_TO": $('#itmStoreCodeTo').val()
                },
            });
            if (_dtActualReceive.rows == null) {
                MsgBox("No Data", MsgBoxStyle.Critical, "DATA ERROR")
                return false;
            }


            //'@Remark : Create Datagrid
            //If DataGridView1.ColumnCount > 0 OrElse Me.DataGridView1.ColumnHeadersHeight <> 23 Then
            //Me.DataGridView1.ColumnHeadersHeight = 23 'ลดความยาวเพื่อเตรียมสร้างใหม่
            //DataGridView1.Columns.Clear()
            //End If
            //CreateDataGrid(sender)




            //'@Remark : Display Detail
            Detail_Data(intRun)
            Display_Data(intRun)





        } catch (error) {
            MsgBox("Error Method : Get_ALL_Data() " + error.Message)
        }
    }


    ////'4. Fill Detail
    Detail_Data = async function (pIntRow = 1) {
        try {
            $('#Label36').val(xDate.Now('dd/MM/yyyy'));
            $('#Label35').val(xDate.Format(_dateDelivery, 'dd/MM/yyyy'));


            //'@Remark : Forecast Max
            var _dtForecastMax = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[dbo].[sp_getForecastMax]",
                    "@StartDate": _PLANT_,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@Part_No": _dtPartControl.rows[pIntRow].F_Part_No,
                    "@Ruibetsu": _dtPartControl.rows[pIntRow].F_Ruibetsu,
                    "@Kanban_No": _dtPartControl.rows[pIntRow].F_Kanban_No,
                    "@Store_Code": _dtPartControl.rows[pIntRow].F_Store_Code,
                    "@Date": _dateDelivery

                },
            });
            var _intFCMax = 0;
            if (_dtForecastMax.rows == null) {
                MsgBox("ไม่พบค่า Forecast Max", MsgBoxStyle.Information)
            } else {
                _intFCMax = (_dtForecastMax.rows[0].ForecastMax >= 0 ? _dtForecastMax.rows[0].ForecastMax : 0);
                $('#txtForecastMax').val(_intFCMax)
            }


            var _strCycle = _dtDate.rows[0].F_Cycle;
            var _intCycleB = intCycleB.substring(3, 2);
            //'@Remark : Cycle
            //DR_GetData = DT_Date.Select("F_Date = '" & Format(dateDelivery, "yyyyMMdd") & "' ")
            //If DR_GetData.Length > 0 Then
            //    strCycle = DR_GetData(0).Item("F_Cycle").ToString.Trim
            //    intCycleB = Mid(strCycle, 3, 2)
            //End If


            if (_dtHeader.rows != null) {
                _dtHeader.rows.forEach(async function (item) {

                    ////'@Remark : Qty/Pack
                    var _intQtyPack = 0;
                    if (item.F_Process_Date == _dateDelivery
                        && item.F_Store_Code == _dtPartControl.rows[_intRow].F_Store_Code
                        && item.F_Kanban_No == _dtPartControl.rows[_intRow].F_Kanban_No
                        && item.F_Part_No == _dt_dtPartControl.rows[_intRow].F_Part_No
                        && item.F_Ruibetsu == _dtPartControl.rows[_intRow].F_Ruibetsu
                    ) {
                        _intQtyPack = (item.F_Qty_Box > 0 ? item.F_Qty_Box : 1);
                    }




                    ////'@Remark : Supplier, Cycle Time, Store Code, Part No, Kanban No, Qty/Pack
                    if (item.F_Store_Code == _dtPartControl.rows[_intRow].F_Store_Code
                        && item.F_Kanban_No == _dtPartControl.rows[_intRow].F_Kanban_No
                        && item.F_Part_No == _dt_dtPartControl.rows[_intRow].F_Part_No
                        && item.F_Ruibetsu == _dtPartControl.rows[_intRow].F_Ruibetsu
                    ) {
                        $('#txtSupplier').val(item.F_Supplier_Code + '-' + item.F_Supplier_Plant);
                        $('#txtCycleTime').val(_strCycle + '-' + _strCycle + '-' + _strCycle);
                        $('#txtStoreCode').val(item.F_Store_Code);
                        $('#txtPartNo').val(item.F_Part_No + '-' + item.F_Ruibetsu);
                        $('#txtKanbanNo').val(item.F_Kanban_No);
                        $('#txtQtyPack').val(_intQtyPack);
                    } else {
                        MsgBox("ไม่พบข้อมูลรายละเอียด Part นี้", MsgBoxStyle.Information);
                        $('#txtSupplier').val("XXXX-X");
                        $('#txtCycleTime').val("00-00-00");
                        $('#txtStoreCode').val("");
                        $('#txtPartNo').val("0000000000-00");
                        $('#txtKanbanNo').val("0000");
                        $('#txtQtyPack').val("0");
                    }


                    ////'@Remark : AvgTrip, Current Use ต้องใช้วันที่ในการค้นหา
                    if (item.F_Process_Date == _dateDelivery
                        && item.F_Store_Code == _dtPartControl.rows[_intRow].F_Store_Code
                        && item.F_Kanban_No == _dtPartControl.rows[_intRow].F_Kanban_No
                        && item.F_Part_No == _dt_dtPartControl.rows[_intRow].F_Part_No
                        && item.F_Ruibetsu == _dtPartControl.rows[_intRow].F_Ruibetsu
                    ) {
                        $('#txtTotalLimit').val(Math.ceil((_intFCMax / _intCycleB) / _intQtyPack) * _intQtyPack);
                        $('#txtUseDay').val(item.F_TMT_FO);
                    } else {
                        $('#txtTotalLimit').val("0");
                        $('#txtUseDay').val("0");
                    }


                });

            }

            ////'@Remark : Part Name, Supplier Name
            var _dtPPM = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR321_SUP]",
                    "@F_plant": _dtPartControl.rows[_intRow].F_Supplier_Plant,
                    "@F_Part_no": _dtPartControl.rows[_intRow].F_Part_No,
                    "@F_Ruibetsu": _dtPartControl.rows[_intRow].F_Ruibetsu,
                    "@F_Store_cd": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@F_supplier_cd": _dtPartControl.rows[_intRow].F_Supplier_Code,
                    "@F_Sebango": _dtPartControl.rows[_intRow].F_Kanban_No
                },
            });
            if (_dtPPM.rows != null) {
                $('#txtPartName').val(_dtPPM.rows[0].F_Part_nm);
                $('#txtSupplierName').val(_dtPPM.rows[0].F_short_name);
                $('#txtLine').val(_dtPPM.rows[0].F_Address);
            } else {
                $('#txtPartName').val('');
                $('#txtSupplierName').val('');
                $('#txtLine').val('');
            }



            ////'@Remark : Max Area
            var _dtArea = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR321_AREA]",
                    "@F_plant": _PLANT_,
                    "@F_supplier_cd": _dtPartControl.rows[_intRow].F_Supplier_Code,
                    "@F_Supplier_Plant": _dtPartControl.rows[_intRow].F_Supplier_Plant,
                    "@F_Part_no": _dtPartControl.rows[_intRow].F_Part_No,
                    "@F_Ruibetsu": _dtPartControl.rows[_intRow].F_Ruibetsu,
                    "@F_Store_cd": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@F_Kanban_No": _dtPartControl.rows[_intRow].F_Kanban_No
                },
            });
            if (_dtArea.rows != null) {
                $('#txtMaxArea').val(_dtArea.rows[0].F_Max_Area);
            } else {
                $('#txtMaxArea').val('0');
            }



            ////'@Remark : STD+B
            var _dtSTDB = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[dbo].[sp_getSTD_B]",
                    "@Plant": _PLANT_,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@Part_No": _dtPartControl.rows[_intRow].F_Part_No,
                    "@Ruibetsu": _dtPartControl.rows[_intRow].F_Ruibetsu,
                    "@Kanban_No": _dtPartControl.rows[_intRow].F_Kanban_No,
                    "@Store_Code": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@Date": _dateDelivery
                },
            });
            if (_dtSTDB.rows != null) {
                $('#txtStdB').val(Math.Round(_dtSTDB.rows[0].STD_B));
                $('#txtSafetyStock').val(_dtSTDB.rows[0].Safety_Stock);
            } else {
                MsgBox("ไม่พบค่า Safety Stock", MsgBoxStyle.Information);
                $('#txtStdB').val('0');
                $('#txtSafetyStock').val('0');
            }



            ////'@Remark : STD
            var _dtSTD = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[dbo].[sp_getSTDStock]",
                    "@Plant": _PLANT_,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@Part_No": _dtPartControl.rows[_intRow].F_Part_No,
                    "@Ruibetsu": _dtPartControl.rows[_intRow].F_Ruibetsu,
                    "@Kanban_No": _dtPartControl.rows[_intRow].F_Kanban_No,
                    "@Store_Code": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@Date": _dateDelivery
                },
            });
            if (_dtSTD.rows != null) {
                $('#txtStdStock').val(Math.Round(_dtSTD.rows[0].STDStock));
            } else {
                MsgBox("ไม่พบค่า Standard Stock", MsgBoxStyle.Information)
                $('#txtStdStock').val('0');
            }



            ////'@Remark : Min STD
            var _dtMINSTD = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[dbo].[sp_getMinStock]",
                    "@Plant": _PLANT_,
                    "@Supplier_Code": _arrSupplier[0],
                    "@Supplier_Plant": _arrSupplier[1],
                    "@Part_No": _dtPartControl.rows[_intRow].F_Part_No,
                    "@Ruibetsu": _dtPartControl.rows[_intRow].F_Ruibetsu,
                    "@Kanban_No": _dtPartControl.rows[_intRow].F_Kanban_No,
                    "@Store_Code": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@Date": _dateDelivery
                },
            });
            if (_dtMINSTD.rows != null) {
                $('#txtMinStock').val(Math.Round(_dtMINSTD.rows[0].Min_Stock));
            } else {
                MsgBox("ไม่พบค่า Minimum Stock", MsgBoxStyle.Information)
                $('#txtMinStock').val('0');
            }



            ////'@Remark : State MRP
            var _dtStateMRP = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR321_MRP]",
                    "@F_Supplier_Code": _arrSupplier[0],
                    "@F_Supplier_Plant": _arrSupplier[1],
                    "@F_Part_No": _dtPartControl.rows[_intRow].F_Part_No,
                    "@F_Ruibetsu": _dtPartControl.rows[_intRow].F_Ruibetsu,
                    "@F_Store_Code": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@F_Kanban_No": _dtPartControl.rows[_intRow].F_Kanban_No,
                    "@F_Process_Date": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@F_Process_Shift": _dateDelivery
                },
            });
            $('#rdoMRPMRPLess20').removeAttr('checked');
            $('#rdoMRPMRPLess20').removeAttr('readonly');
            $('#rdoMRPMRPLess20_label').attr('style', 'color:black');

            $('#rdoMRPMRPMore20').removeAttr('checked');
            $('#rdoMRPMRPMore20').removeAttr('readonly');
            $('#rdoMRPMRPMore20_label').attr('style', 'color:black');

            if (_dtStateMRP.rows != null
                && $.isNumeric(_dtStateMRP.rows[0].F_MRP)
                && $.isNumeric(_dtStateMRP.rows[0].F_TMT_FO)) {
                if ((_dtStateMRP.rows[0].F_TMT_FO * 0.8) > _dtStateMRP.rows[0].F_MRP) {
                    $('#rdoMRPMRPLess20').attr('checked', 'checked');
                    $('#rdoMRPMRPLess20').attr('readonly', true);
                    $('#rdoMRPMRPLess20_label').attr('style', 'color:red');
                } else if ((_dtStateMRP.rows[0].F_TMT_FO * 0.8) > _dtStateMRP.rows[0].F_MRP) {
                    $('#rdoMRPMRPMore20').attr('checked', 'checked');
                    $('#rdoMRPMRPMore20').attr('readonly', true);
                    $('#rdoMRPMRPMore20_label').attr('style', 'color:red');
                }
            }



            ////'@Remark : Maintenance Kanban
            var _dtKanban = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR321_Kanban]",
                    "@F_Supplier_Code": _arrSupplier[0],
                    "@F_Supplier_Plant": _arrSupplier[1],
                    "@F_Part_No": _dtPartControl.rows[_intRow].F_Part_No,
                    "@F_Ruibetsu": _dtPartControl.rows[_intRow].F_Ruibetsu,
                    "@F_Store_Code": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@F_Kanban_No": _dtPartControl.rows[_intRow].F_Kanban_No,
                    "@F_Process_Date": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@F_Process_Shift": _dateDelivery
                },
            });
            $('#rdoStatusSTOP').removeAttr('checked');
            $('#rdoStatusSTOP').removeAttr('readonly');
            $('#rdoStatusADD').removeAttr('checked');
            $('#rdoStatusADD').removeAttr('readonly');
            $('#rdoStatusCUT').removeAttr('checked');
            $('#rdoStatusCUT').removeAttr('readonly');
            if (_dtKanban.rows != null) {
                if (_dtKanban.rows[0].F_NON_STOP == '0') {
                    $('#rdoStatusSTOP').attr('checked', 'checked');
                    $('#rdoStatusSTOP').attr('readonly', true);
                }
                if (_dtKanban.rows[0].F_KB_ADD > 0) {
                    $('#rdoStatusADD').attr('checked', 'checked');
                    $('#rdoStatusADD').attr('readonly', true);
                }
                if (_dtKanban.rows[0].F_KB_CUT < 0) {
                    $('#rdoStatusCUT').attr('checked', 'checked');
                    $('#rdoStatusCUT').attr('readonly', true);
                }
            }



            ////'@Remark : Kanban Order
            var _dtOrder = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR321_Order]",
                    "@F_Process_Date": _PLANT_,
                    "@F_Supplier_Code": _arrSupplier[0],
                    "@F_Supplier_Plant": _arrSupplier[1],
                    "@F_Part_No": _dtPartControl.rows[_intRow].F_Part_No,
                    "@F_Ruibetsu": _dtPartControl.rows[_intRow].F_Ruibetsu,
                    "@F_Kanban_No": _dtPartControl.rows[_intRow].F_Kanban_No,
                    "@F_Store_Code": _dtPartControl.rows[_intRow].F_Store_Code
                },
            });
            $('#chkKanbanOrder').removeAttr('checked');
            $('#chkKanbanOrder').removeAttr('readonly');
            if (_dtOrder.rows != null) {
                $('#chkKanbanOrder').attr('checked', 'checked');
                $('#chkKanbanOrder').attr('readonly', true);
            }






            ////'@Remark : Change CycleTime
            Set_Cycle_Time()




            ////'@Remark : Slice Order
            var _dtSlice = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR321_Slice]",
                    "@F_Plant": _PLANT_,
                    "@F_Supplier_CD": _arrSupplier[0],
                    "@F_Supplier_Plant": _arrSupplier[1],
                    "@F_Store_CD": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@F_Delivery_Date": _dateDelivery,
                    "@F_Delivery_Trip": _intDeliveryTrip,
                    "@F_Part_No": _dtPartControl.rows[_intRow].F_Part_No,
                    "@F_Ruibetsu": _dtPartControl.rows[_intRow].F_Ruibetsu
                },
            });
            $('#chkSliceOrder').removeAttr('checked');
            $('#chkSliceOrder').removeAttr('readonly');
            if (_dtSlice.rows != null) {
                $('#chkSliceOrder').attr('checked', 'checked');
                $('#chkSliceOrder').attr('readonly', true);
            }



            ////'@Remark : Receive Slice Order
            var _dtReceiveSlice = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR321_ReceiveSlice]",
                    "@F_Plant": _PLANT_,
                    "@F_Supplier_CD": _arrSupplier[0],
                    "@F_Supplier_Plant": _arrSupplier[1],
                    "@F_Store_CD": _dtPartControl.rows[_intRow].F_Store_Code,
                    "@F_Slide_Date": _dateDelivery,
                    "@F_Slide_Trip": _intDeliveryTrip,
                    "@F_Part_No": _dtPartControl.rows[_intRow].F_Part_No,
                    "@F_Ruibetsu": _dtPartControl.rows[_intRow].F_Ruibetsu
                },
            });
            $('#chkReceiveSlice').removeAttr('checked');
            $('#chkReceiveSlice').removeAttr('readonly');
            if (_dtReceiveSlice.rows != null) {
                $('#chkReceiveSlice').attr('checked', 'checked');
                $('#chkReceiveSlice').attr('readonly', true);
            }




























        } catch (error) {

        }
    }


    Display_Data = async function (pIntRow = 1) {
        try {
            var _ExpireDate;
            var _intDayShift, _intNightShift;

            //'@Remark : เก็บจำนวน Column ทั้งหมดของ Datagrid
            var CountDGHeader = 0;
            var CountDGDetail = 0, CountDGFC = 0;
            var CountDGVolume = 0;
            $('#txtNumber').val(pIntRow + '/' + _dtPartControl.rows.length);


            if (_dtHeader.rows != null) {
                _dtHeader.rows.forEach(async function (item) {

                    ////'@Remark : Check Expire Date of Part No.
                    if (item.F_Supplier_Code == _dtPartControl.rows[pIntRow].F_Supplier_Code
                        && item.F_Supplier_Plant == _dtPartControl.rows[pIntRow].F_Supplier_Plant
                        && item.F_Store_Code == _dtPartControl.rows[pIntRow].F_Store_Code
                        && item.F_Kanban_No == _dtPartControl.rows[pIntRow].F_Kanban_No
                        && item.F_Part_No == _dt_dtPartControl.rows[pIntRow].F_Part_No
                        && item.F_Ruibetsu == _dtPartControl.rows[pIntRow].F_Ruibetsu
                    ) {
                        _ExpireDate = item.F_Process_Date;
                    }


                });
            }

            var _dgHeader = [];

            for (var i=0; i < _intAmountShow; i++) {
                //'@Remark : Support ECI : แสดงข้อมูลถึงวันสุดท้ายที่มี Forecast
                if (_dtDate.rows[i].F_Date <= _ExpireDate) {

                    _intCycleB = _dtDate.rows[i].F_Cycle.substring(3, 2);

                    _dtPeriod.rows.forEach(async function (item) {
                        if (item.Date_Now == _dtDate.rows[i].F_Date
                            && item.Row_Num == '1'
                        ) {
                            _intDayShift = item.F_Period;
                        }
                    });
                    _dtPeriod.rows.forEach(async function (item) {
                        if (item.Date_Now == _dtDate.rows[i].F_Date
                            && item.Row_Num == '2'
                        ) {
                            _intNightShift = item.F_Period;
                        }
                    });

                    var _intSumActual = 0;
                    _dtActualReceive.rows.forEach(async function (item) {
                        if (item.F_Receive_Date == _dtPartControl.rows[pIntRow].F_Date
                            && item.F_Store_CD == _dtPartControl.rows[pIntRow].F_Store_Code
                            && item.F_Part_No == _dtPartControl.rows[pIntRow].F_Part_No
                            && item.F_Ruibetsu == _dtPartControl.rows[pIntRow].F_Ruibetsu
                        ) {
                            _intSumActual += item.F_Receive_Qty;
                        }
                    });


                    ////'@Remark : Header
                    for (var iShift=0; iShift <= 1; iShifti++) {
                        _dtHeader.rows.forEach(async function (item) {

                            ////'@Remark : Check Expire Date of Part No.
                            if (item.F_Process_Date == _dtDate.rows[pIntRow].F_Date
                                && item.F_Process_Shift == (iShift = 0 ? "D" : "N")
                                && item.F_Store_Code == _dtPartControl.rows[pIntRow].F_Store_Code
                                && item.F_Kanban_No == _dtPartControl.rows[pIntRow].F_Kanban_No
                                && item.F_Part_No == _dt_dtPartControl.rows[pIntRow].F_Part_No
                                && item.F_Ruibetsu == _dtPartControl.rows[pIntRow].F_Ruibetsu
                            ) {
                                if (item != null && CountDGHeader < 100) {
                                    _dgHeader[CountDGHeader][0] = item.F_TMT_FO;
                                    _dgHeader[CountDGHeader][1] = item.F_MRP;
                                    _dgHeader[CountDGHeader][2] = item.F_AbNormal_Part;
                                    _dgHeader[CountDGHeader][3] = item.F_TMT_FO;
                                }


                                
                            }


                        });

                    }






                }










            }






        } catch (error) {

        }
    }



    ////'7. Check ว่าปุ่ม "Set Cycle Time" จะเปิด หรือปิด
    Set_Cycle_Time = async function () {
        try {
            ////' Enable Set Cycle Time
            var _dtCycleTime = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR321_CycleTime]",
                    "@F_Supplier_Code": _arrSupplier[0],
                    "@F_Supplier_Plant": _arrSupplier[1],
                    "@F_Start_Date": xDate.Now('yyyyMMdd'),
                    "@F_End_Date": xDate.Now('yyyyMMdd')
                },
            });
            $('#chkChangeCycleTime').removeAttr('checked');
            $('#chkChangeCycleTime').removeAttr('readonly');
            if (_dtCycleTime.rows != null) {
                $('#chkChangeCycleTime').attr('checked', 'checked');
                $('#chkChangeCycleTime').attr('readonly', true);
            }
        } catch (error) {

        }
    }






















    xAjax.onClick('btnReport', async function () {
        $('#rdoStatusSTOP').attr('checked', 'checked');
        $('#rdoStatusSTOP').attr('readonly', true);
    });


    xAjax.onClick('itmSupplier', async function () { setCriteria(1); });
    xAjax.onClick('itmKanban', async function () { setCriteria(2); });
    xAjax.onClick('itmKanbanTo', async function () { setCriteria(2); });
    xAjax.onClick('itmStoreCode', async function () { setCriteria(3); });
    xAjax.onClick('itmStoreCodeTo', async function () { setCriteria(3); });
    xAjax.onClick('itmPartNo', async function () { setCriteria(4); });
    xAjax.onClick('itmPartNoTo', async function () { setCriteria(4); });


    setCriteria = async function (pLevel = 0) {
        var _supplier = ($('#itmSupplier').val() != null ? ", @Supplier_Code='" + $('#itmSupplier').val().substring(0, 4) + "' " : "")
            + ($('#itmSupplier').val() != null ? ", @Supplier_Plant='" + $('#itmSupplier').val().substring(5) + "' " : "");
        var _store = ($('#itmStoreCode').val() != null ? ", @Store_Code_FROM='" + $('#itmStoreCode').val() + "' " : "")
            + ($('#itmStoreCodeTo').val() != null ? ", @Store_Code_TO='" + $('#itmStoreCodeTo').val() + "' " : "");


        //sp_NormalOrder_getSupplier
        var _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": "[dbo].[sp_NormalOrder_getSupplier] '1', @Supplier_Code='9999' "
            },
        });
        if (_dt.rows != null && $('#itmSupplier').val() == null) xDropDownList.bind('#itmSupplier', _dt.rows, 'F_Supplier', 'F_Supplier');


        //sp_NormalOrder_getKanban
        _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": "[dbo].[sp_NormalOrder_getKanban] '1', @OrderType='Daily' " + _supplier
            },
        });
        if (_dt.rows != null && (pLevel < 2 || $('#itmKanban').val() == null)) xDropDownList.bind('#itmKanban', _dt.rows, 'F_Kanban_No', 'F_Kanban_No');
        if (_dt.rows != null && (pLevel < 2 || $('#itmKanbanTo').val() == null)) xDropDownList.bind('#itmKanbanTo', _dt.rows, 'F_Kanban_No', 'F_Kanban_No');


        //sp_NormalOrder_getStoreCode
        _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": "[dbo].[sp_NormalOrder_getStoreCode] '1', @OrderType='Daily' " + _supplier
            },
        });
        if (_dt.rows != null && (pLevel < 3 || $('#itmStoreCode').val() == null)) xDropDownList.bind('#itmStoreCode', _dt.rows, 'F_Store_CD', 'F_Store_CD');
        if (_dt.rows != null && (pLevel < 3 || $('#itmStoreCodeTo').val() == null)) xDropDownList.bind('#itmStoreCodeTo', _dt.rows, 'F_Store_CD', 'F_Store_CD');


        //sp_NormalOrder_getPartNo
        _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": "[dbo].[sp_NormalOrder_getPartNo] '1', @OrderType='Daily' " + _supplier + _store
            },
        });
        if (_dt.rows != null && (pLevel < 4 || $('#itmPartNo').val() == null)) xDropDownList.bind('#itmPartNo', _dt.rows, 'F_Part_No', 'F_Part_No');
        if (_dt.rows != null && (pLevel < 4 || $('#itmPartNoTo').val() == null)) xDropDownList.bind('#itmPartNoTo', _dt.rows, 'F_Part_No', 'F_Part_No');




    }


    initial = async function () {

        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR321]"
            },
        });

        if (_dt.rows != null) xDataTable.loading('#tblMaster', _dt.rows);

        var _pWidth = $('#tblMaster_wrapper').parent().width();
        $('#tblMaster_wrapper').width(_pWidth);
        $('.dataTables_scrollHeadInner .display').width($('#tblMaster_wrapper').width(_pWidth));

        setCriteria(0);



        var _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": "[exec].[spMSParameter] '"
                    + ajexHeader.UserCode + "' "
                    + ", 'CI_CKD' "
                    + ", @ErrorMessage='' "
            },
        });
        //console.log(_dt.rows[0].F_Value2);
        //console.log(_dt.rows[0].F_Value3);
        var _nNextProcess = xDate.Format(_PROCESSDATE_, 'yyyyMMdd') + (_SHIFT_ == '1' ? 'D' : 'N');
        //console.log(_nNextProcess);
        if (_dt.rows != null) {
            if (_dt.rows[0].F_Value2 == '0' || _dt.rows[0].F_Value2 == '3' || _dt.rows[0].F_Value3 != _nNextProcess) {
                //$('#btnProcess').attr('readonly', true);
                //$('#btnPreview').attr('readonly', true);
                //$('#btnReCalculate').attr('readonly', true);
            } else {
                $('#btnProcess').removeAttr('readonly');
                $('#btnPreview').removeAttr('readonly');
                $('#btnReCalculate').removeAttr('readonly');
                //'If shiftNow = "D" AndAlso srtAction = "Process" _ IT เปิดให้ ReCal ได้ทั้งกลางวัน กลางคืนละ 
                //    If srtAction = "Process" _
                //    AndAlso Txt_Supplier.Text.Trim <> "" AndAlso Txt_PartNo.Text.Trim <> "" _
                //    AndAlso Txt_Kanban.Text.Trim <> "" AndAlso Txt_StoreCode.Text.Trim <> "" Then
                //Btn_ReCalculate.Enabled = True
                //Else
                //Btn_ReCalculate.Enabled = False
                //    End If
            }

        }


        xSplash.hide();
    }
    initial();


    $('.form-floating label').attr('style', 'font-size:10px;height: 32px;');
    $('.form-floating input').attr('style', 'font-size:10px;height: 32px;');


});