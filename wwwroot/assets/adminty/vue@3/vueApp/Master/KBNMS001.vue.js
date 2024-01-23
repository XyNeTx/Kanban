const { createApp, onBeforeMount, onMounted, ref } = Vue

createApp({
    setup() {

        const frmCondition = ref({
            "F_Plant": "3"
        })


        onBeforeMount(() => {
            //console.info('onBeforeMount');

        })

        onMounted(() => {
            console.info('onMounted');

            const xKBNMS001 = new MasterTemplate({
                Controller: _PAGE_,
                Table: 'tblMaster',
                ColumnTitle: {
                    "EN": ['Flag', 'Plant', 'Order Type', 'Effective Date', 'End Date'],
                    "TH": ['Flag', 'Plant', 'Order Type', 'Effective Date', 'End Date'],
                },
                ColumnValue: [
                    { "data": "F_Plant" },
                    { "data": "F_Plant" },
                    { "data": "F_OrderType" },
                    { "data": "F_Effect_Date" },
                    { "data": "F_End_Date" }
                ],
                Modal: 'modalMaster',
                Form: 'frmMaster',
                PostData: [
                    { name: 'F_Plant', value: '#frmCondition #F_Plant' }
                ],
            });


            xKBNMS001.prepare();

            xKBNMS001.initial(function (result) {
                xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
                xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

                frmCondition.value.F_Plant = 3

                console.log(frmCondition.value.F_Plant);

                xKBNMS001.search();
            });

            onSave = function () {
                xKBNMS001.save(function () {
                    xKBNMS001.search();
                });
            }

            onDelete = function () {
                xKBNMS001.delete(function () {
                    xKBNMS001.search();
                });
            }

            onDeleteAll = function () {
                xKBNMS001.deleteall(function () {
                    xKBNMS001.search();
                });
            }

            onPrint = function () {
                console.log('onPrint');
            }

            onExecute = function () {
                console.log('onExecute');
                var table = $('#tblMaster').DataTable().buttons(0).trigger();
            }

            xAjax.onChange('#frmCondition #F_Plant', function () {
                $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

                xKBNMS001.search();
            });

            var table = $('#example').DataTable();

            table.column(0).visible(false);






        })


        return {
            frmCondition,
        }
    }
}).mount('#vueApp')