$(document).ready(async function () {
    xSplash.hide();
    await GetMonthly();
    await KanBanNo();
    await StoreCode("ConditionSupplier");
    await StoreCode("ConditionDetail");
    await PartNo();


    const dayColumns = Array.from({ length: 31 }, (_, i) => {
        const day = i + 1;
        return {
            data: `f_Amount_MD${day}`,
            title: `Day${day}`,
            createdCell: function (td, cellData, rowData) {
                const isLocked = parseInt(rowData.f_Lock || 0) >= day;

                if (isLocked) {
                    td.style.backgroundColor = 'coral';
                } else {
                    td.classList.add('cell-editable');
                }
            }
        };
    });
    // ✅ 2. สร้าง column definitions
    const columns = [
        { data: 'f_Supplier_Code', title: 'Supplier Cd.' },
        { data: 'f_Short_Name', title: 'Short Name' },
        { data: 'f_Part_No', title: 'Part No.', className: "pe-3" },
        { data: 'f_Part_Name', title: 'Name', className: "pe-3" },
        { data: 'f_Store_cd', title: 'StoreCd' },
        { data: 'f_Sebango', title: 'KB' },
        { data: 'f_Delivery_qty', title: 'Box/Qty' },
        { data: 'f_cycle_supply', title: 'Cycle' },
        ...dayColumns,
        { data: 'f_Amount_M', title: 'M', className: "cell-editable" },
        { data: 'f_Amount_M1', title: 'M1', className: "cell-editable" },
        { data: 'f_Amount_M2', title: 'M2', className: "cell-editable" },
        { data: 'f_Amount_M3', title: 'M3', className: "cell-editable" },

    ];

    let table = $('#myTable').DataTable({
                    //data: dataSet,
                    columns: columns,
                    scrollCollapse: true,
                    scrollY: '300px',
                    scrollX: true,
                    pageLength: 50,
                    fixedColumns: {
                        leftColumns: 8 // จำนวนคอลัมน์ที่คุณต้องการล็อคทางซ้าย
                    },
                });

    $(document).find("input[class='datepicker form-control']").each(function () {
        $(this).initDatepicker();
    });


    $(document).find("input[class='monthPicker form-control']").each(function () {
        $(this).datepicker({

            format: 'mm/yyyy',
            showOtherMonths: false,
            changeMonth: true,
            changeYear: true,
            // ซ่อนปฏิทินวันที่
            width: "40%",
            icons: {
                rightIcon: false
            }
        }).on('show', function (e) {
            // ป้องกันการเลือกวันที่
            $('.gj-picker div[data-role="calendar"]').hide();
        });

    });

    $("input[name='importType']").on("change", function () {
        var selectedValue = $(this).val();
        console.log("เลือก:", selectedValue);

        $("#detail_supplier").val("").change();
        $("#detail_supplierName").val("");
        $("#detail_PartName").val("");
        $("#detail_Kanban").val("").change();
        $("#detail_StoreCode").val("").change();
        $("#detail_PartNo").val("").change();

        $("#supplier_supplierCode").val("").change();
        $("#supplier_storeCode").val("").change();
        $("#supplier_supplierName").val("");

        if (selectedValue === "ConditionDetail") {
            $("#supplier_supplierCode").prop("disabled", true);
            $("#supplier_storeCode").prop("disabled", true);
            $("#supplier_storeCode,#supplier_supplierCode").selectpicker("refresh");

            $("#detail_supplier").prop("disabled", false);
            $("#detail_supplierName").val("");
            $("#detail_Kanban").prop("disabled", false);
            $("#detail_StoreCode").prop("disabled", false);
            $("#detail_PartNo").prop("disabled", false);

            $("#detail_supplier, #detail_Kanban, #detail_StoreCode ,#detail_PartNo").selectpicker("refresh");
        }
        else {
            $("#detail_supplier").prop("disabled", true);
            $("#detail_Kanban").prop("disabled", true);
            $("#detail_StoreCode").prop("disabled", true);
            $("#detail_PartNo").prop("disabled", true);
            $("#detail_supplier, #detail_Kanban, #detail_StoreCode ,#detail_PartNo").selectpicker("refresh");

            $("#supplier_supplierCode").prop("disabled", false);
            $("#supplier_storeCode").prop("disabled", false);
            $("#supplier_storeCode,#supplier_supplierCode").selectpicker("refresh");

        }
    });

    $('.input-group-text.event').on('click', function () {
        $(this).closest('.input-group').find('input.datepicker').focus();
        $(this).closest('.input-group').find('input.monthPicker').focus();
        //$(this).closest('.input-group').find('input.datepicker').focus();
    });

    $('.input-group-text.eventMonth').on('click', function () {
        $(this).closest('.input-group').find('input.monthPicker').focus();
    });


    async function GetMonthly() {
        _xLib.AJAX_Get("/api/KBNIM012M/FormLoad", "",
            async function (success) {
                console.log(success);
                $("#ImportMonth").val(success.import.prodYM);
                $("#ImportVersion").val(success.import.version);
                $("#ImportRevision").val(success.import.revision);

                $("#ConditionMonth").val(success.condition.prodYM);
                $("#ConditionVersion").val(success.condition.version);
                $("#ConditionRevision").val(success.condition.revision);

            },
            function (error) {
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );

        _xLib.AJAX_Get("/api/KBNIM012M/SupplierCode", "",
            async function (success) {
                console.log(success);
                success.data.forEach(item => {

                    $('#detail_supplier').append(new Option(item, item));
                    $('#supplier_supplierCode').append(new Option(item, item));

                });
                $("#detail_supplier, #supplier_supplierCode").resetSelectPicker();
            },
            function (error) {
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );
    }

    async function KanBanNo() {

        let obj = {
            supplierCode: $("#detail_supplier").val()
        };

        _xLib.AJAX_Get("/api/KBNIM012M/KanBanNo", obj,
            async function (success) {
                console.log(success);
                $('#detail_Kanban').empty();
                success.data.forEach(item => {

                    $('#detail_Kanban').append(new Option(item, item));

                });
                $("#detail_Kanban").resetSelectPicker();
            },
            function (error) {
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );
    }

    async function SupplierName(condition) {
        let obj = {
            supplierCode: condition === "ConditionDetail" ? $("#detail_supplier").val() : $("#supplier_supplierCode").val()
        };

        $("#detail_supplierName").val("");
        $("#supplier_supplierName").val("");

        _xLib.AJAX_Get("/api/KBNIM012M/SupplierName", obj,
            async function (success) {
                console.log(success);

                if (condition === "ConditionDetail")
                {
                    $("#detail_supplierName").val(success.data);
                }
                else
                {
                    $("#supplier_supplierName").val(success.data);
                }
            },
            function (error) {
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );
    }

    async function StoreCode(condition) {
        let obj = {
            supplierCode: condition === "ConditionDetail" ? $("#detail_supplier").val() : $("#supplier_supplierCode").val(),
            kanbanNo: condition === "ConditionDetail" ? $('#detail_Kanban').val() : ""
        };

        $('#detail_StoreCode').empty();
        $('#supplier_storeCode').empty();

        _xLib.AJAX_Get("/api/KBNIM012M/StoreCode", obj,
            async function (success) {
                console.log(success);

                if (condition === "ConditionDetail") {

                    success.data.forEach(item => { $('#detail_StoreCode').append(new Option(item, item)); });
                   
                }
                else {
                    success.data.forEach(item => { $('#supplier_storeCode').append(new Option(item, item)); });
                }

                $("#detail_StoreCode, #supplier_storeCode").resetSelectPicker();
            },
            function (error) {
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );
    }

    async function PartNo() {
        let obj = {
            supplierCode: $("#detail_supplier").val(),
            kanbanNo: $('#detail_Kanban').val(),
            storeCode: $('#detail_StoreCode').val() 
        };

        $('#detail_PartNo').empty();
        $("#detail_PartName").val("");

        _xLib.AJAX_Get("/api/KBNIM012M/PartNoList", obj,
            async function (success) {
                console.log(success);

                success.data.forEach(item => { $('#detail_PartNo').append(new Option(item, item)); });
                $("#detail_PartNo").resetSelectPicker();
            },
            function (error) {
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );
    }

    async function PartName() {

        if ($("#detail_PartNo").val() === "") return;
        $("#detail_PartName").val("");
        let obj = {
            partNo: $("#detail_PartNo").val(),
        };

        _xLib.AJAX_Get("/api/KBNIM012M/PartName", obj,
            async function (success) {
                console.log(success);

                $("#detail_PartName").val(success.data);
            },
            function (error) {
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );
    }

    window.dataSet = [];

    async function Search(condition) {

        if ($.fn.DataTable.isDataTable('#myTable')) {
            $('#myTable').DataTable().destroy();
            $('#myTable').empty(); // ล้างหัวตารางเดิม
        }
        console.log(condition)
        condition = $("input[name='importType']:checked").val();
        xSplash.show("LoadData...");
        let obj = {

            ProdYM: moment($("#ConditionMonth").val(), "MM/YYYY").format("YYYYMM"),
            Version: $("#ConditionVersion").val(),
            Revision: $("#ConditionRevision").val(),
            Condition: $("input[name='importType']:checked").val(),
            SupplierCode: (condition === "ConditionDetail") ? $("#detail_supplier").val() : $("#supplier_supplierCode").val(),
            StoreCode: (condition === "ConditionDetail") ? $("#detail_StoreCode").val() : $("#supplier_storeCode").val(),
            KanbanNo: $('#detail_Kanban').val(),
            PartNo: $("#detail_PartNo").val(),
            Txt_ProdYM: moment($("#ImportMonth").val(), "MM/YYYY").format("YYYYMM"),
            Txt_Rev: $("#ImportRevision").val(),
            Txt_Ver: $("#ImportVersion").val(),
            Txt_Date: $("#ImportDate").val(),
        };

        _xLib.AJAX_Post("/api/KBNIM012M/Search", obj,
            async function (success) {
                console.log(success);
                window.dataSet = success.data;

                table = $('#myTable').DataTable({
                    data: dataSet,
                    columns: columns,
                    scrollCollapse: true,
                    scrollY: '300px',
                    scrollX: true,
                    //ordering: false,
                    pageLength: 50,
                    //searching: false,
                    fixedColumns: {
                        leftColumns: 8 // จำนวนคอลัมน์ที่คุณต้องการล็อคทางซ้าย
                    },
                });
                console.log(dataSet)
                xSplash.hide();
            },
            function (error) {
                xSplash.hide();
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );
    }

    async function Transfer() {
        
        let condition = $("input[name='importType']:checked").val();
        xSplash.show("LoadData...");
        let obj = {
            ProdYM: moment($("#ConditionMonth").val(), "MM/YYYY").format("YYYYMM"),
            Version: $("#ConditionVersion").val(),
            Revision: $("#ConditionRevision").val(),
            Condition: $("input[name='importType']:checked").val(),
            SupplierCode: (condition === "ConditionDetail") ? $("#detail_supplier").val() : $("#supplier_supplierCode").val(),
            StoreCode: (condition === "ConditionDetail") ? $("#detail_StoreCode").val() : $("#supplier_storeCode").val(),
            KanbanNo: $('#detail_Kanban').val(),
            PartNo: $("#detail_PartNo").val(),
            Txt_ProdYM: moment($("#ImportMonth").val(), "MM/YYYY").format("YYYYMM"),
            Txt_Rev: $("#ImportRevision").val(),
            Txt_Ver: $("#ImportVersion").val(),
            Txt_Date: $("#ImportDate").val(),
            Txt_DateF: moment($("#txtTransferDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            Txt_DateT: moment($("#txtTransferDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD")
        };

        _xLib.AJAX_Post("/api/KBNIM012M/Transfer", obj,
            async function (success) {
                console.log(success);
                xSplash.hide();
                Swal.fire("Saved!", success.data, "success");
            },
            function (error) {
                xSplash.hide();
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );
    }

    async function Report() {

        let condition = $("input[name='importType']:checked").val();
        xSplash.show("LoadData...");
        let obj = {
            ProdYM: moment($("#ConditionMonth").val(), "MM/YYYY").format("YYYYMM"),
            Version: $("#ConditionVersion").val(),
            Revision: $("#ConditionRevision").val(),
            Condition: $("input[name='importType']:checked").val(),
            SupplierCode: (condition === "ConditionDetail") ? $("#detail_supplier").val() : $("#supplier_supplierCode").val(),
            StoreCode: (condition === "ConditionDetail") ? $("#detail_StoreCode").val() : $("#supplier_storeCode").val(),
            KanbanNo: $('#detail_Kanban').val(),
            PartNo: $("#detail_PartNo").val(),
            Txt_ProdYM: moment($("#ImportMonth").val(), "MM/YYYY").format("YYYYMM"),
            Txt_Rev: $("#ImportRevision").val(),
            Txt_Ver: $("#ImportVersion").val(),
            Txt_Date: $("#ImportDate").val(),
            Txt_DateF: moment($("#txtTransferDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            Txt_DateT: moment($("#txtTransferDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD")
        };

        _xLib.AJAX_Post("/api/KBNIM012M/Report", obj,
            async function (success) {
                console.log(success);
                xSplash.hide();
                Swal.fire("Report!", success.data, "success");

                let userName = _xLib.GetUserName();
                let reportUrl = " http://hmmt-app03/Reports/Pages/ReportViewer.aspx?%2fKB3%2fKBNIM012M&rs:Command=Render";
                window.open(reportUrl + '&UserName=' + userName , '_blank');
            },
            function (error) {
                xSplash.hide();
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );
    }

    $('#detail_supplier').change(async function () {

        await SupplierName("ConditionDetail");
        await StoreCode("ConditionDetail");
        await KanBanNo();
        await PartNo();

    });

    $('#supplier_supplierCode').change(async function () {

        await SupplierName("ConditionSupplier");
        await StoreCode("ConditionSupplier");

    });

    $("#detail_Kanban").change(async function () {
        await StoreCode("ConditionDetail");
        await PartNo();
    });

    $("#detail_PartNo").change(async function () {
        await PartName();
    });

    //------Transfer---------

    $("#btnTransfer").click(async function(){
        await Transfer();
        await Search("ConditionDetail");

    });

    //------SEARCH DATA-------
    $("#btnSearch").click(async function () {
        await Search("ConditionDetail");
    });
    //-------REPORT-------
    $("#btnReport").click(async function () {
        await Report();
    });

    //-------SAVE DATA---------
    $("#btnSave").click(async function () {
        Swal.fire({
            title: "Do you want to save the changes?",
            showDenyButton: true,
            confirmButtonText: "Save",
            denyButtonText: `Don't save`
        }).then((result) => {

            if (result.isConfirmed) {

                xSplash.show("Process Data..");

                _xLib.AJAX_Post("/api/KBNIM012M/SaveForeCast", dataSet,
                    async function (success) {
                        console.log(success);

                        xSplash.hide();
                        Swal.fire("Saved!", success.data, "success");
                    },
                    function (error) {
                        xSplash.hide();
                        xSwal.error(error.responseJSON.response, error.responseJSON.message);
                    }
                );


            }
        });
    });

    $("#btnClear").click(async function () {


        await $("#TypeForImportSupplier").prop('checked', true).trigger('change');
        await $("#TypeForImportCondition").prop('checked', true).trigger('change');

        if ($.fn.DataTable.isDataTable('#myTable')) {
            $('#myTable').DataTable().destroy();
            $('#myTable').empty(); // ล้างหัวตารางเดิม
        }

        table = $('#myTable').DataTable({
            columns: columns,
            scrollCollapse: true,
            scrollY: '300px',
            scrollX: true,
            pageLength: 50,
            fixedColumns: {
                leftColumns: 8 // จำนวนคอลัมน์ที่คุณต้องการล็อคทางซ้าย
            },
        });
    });

   
    // dblclick ที่ bind หลังจาก DataTable ถูกสร้าง
    $(document).on('dblclick', '#myTable td.cell-editable', function () {
        console.log("dbClick");
        const cell = table.cell(this);
        const colIndex = cell.index().column;
        const rowIndex = cell.index().row;

        if (!$(this).hasClass('editing')) {
            const currentValue = cell.data();
            $(this).addClass('editing').html(`<input type="number" value="${currentValue}" style="width: 100%">`);
            $(this).find('input').focus();
        }
    });

    $(document).on('blur', '#myTable td.editing input', function () {

        const td = $(this).closest('td');
        let newValue = $(this).val();
        const cell = table.cell(td);
        const colIndex = cell.index().column;
        const rowIndex = cell.index().row;
        const colName = columns[colIndex].data;

        // อัปเดต dataset
        newValue = parseInt(newValue)
        dataSet[rowIndex][colName] = parseInt(newValue);

        td.removeClass('editing');
        cell.data(newValue).draw();

        console.log('Updated:', dataSet[rowIndex]);
    });

    $("#btnFileImport").click(async function () {

        xSplash.show("Process Import Data!");
        let url = "";
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        else if (window.location.hostname.includes("app07")) {
            url = "/kanban" + url;
        }
        
        var formData = new FormData();
        formData.append('File', $('#fileImport')[0].files[0]);
        formData.append('ProdYM', moment($("#ConditionMonth").val(), "MM/YYYY").format("YYYYMM"));
        formData.append('Version', $("#ConditionVersion").val());
        formData.append('Revision', $("#ConditionRevision").val());
        formData.append('Condition', "Condition");

        let title = $(document).find("title").text();
        title = title.split(" - ")[0];
        title = title.split(" : ")[1];
        ajexHeader.title = title;

        await $.ajax({
            url: url + "/api/KBNIM012M/ImportData",
            type: 'POST',
            headers: ajexHeader,
            data: formData,
            processData: false,
            contentType: false,
            success: async function (respone) {
                let result = respone.data;
                console.log(result);

                Swal.fire("Success!", result, "success");

            },
            error: async function (xhr, error) {
               
                xSplash.hide();
                xSwal.error(xhr.responseJSON.response, xhr.responseJSON.message);
                if (xhr.responseJSON.message == "Please Check Data Again") {

                    let userName = _xLib.GetUserName();
                    let reportUrl = " http://hmmt-app03/Reports/Pages/ReportViewer.aspx?%2fKB3%2fKBNIM012_ERR&rs:Command=Render";
                    window.open(reportUrl + '&UserName=' + userName, '_blank');
                }
            }
        });


    });

});