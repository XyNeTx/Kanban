const { createApp, onBeforeMount, onMounted, ref } = Vue

createApp({
    setup() {

        const frmCondition = ref({
            "F_Plant": "3",
        })

        let menus = ref('')
        const message = ref('Sign In')     


        onBeforeMount(() => {
            //console.info('onBeforeMount');

        })

        onMounted(() => {
            //console.info('onMounted');

            const xKBNMS002 = new MasterTemplate({
                Controller: _PAGE_,
                Table: 'tblMaster',
                ColumnTitle: {
                    "EN": ['No.', 'Plant', 'Parent Part', 'Part Name', 'Effective Date', 'End Date'],
                    "TH": ['No.', 'Plant', 'Parent Part', 'Part Name', 'Effective Date', 'End Date'],
                },
                ColumnValue: [
                    { "data": "RunningNo" },
                    { "data": "F_Plant" },
                    { "data": "F_Parent_Part" },
                    { "data": "F_Part_Name" },
                    { "data": "F_Start_Date" },
                    { "data": "F_End_Date" }
                ],
                Modal: 'modalMaster',
                Form: 'frmMaster',
                PostData: [
                    { name: 'F_Plant', value: '#frmCondition #F_Plant' }
                ],
            });

            xKBNMS002.prepare();

            xKBNMS002.initial(function (result) {
                xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
                xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

                xKBNMS002.search();
            });

            onSave = function () {
                xKBNMS002.save(function () {
                    xKBNMS002.search();
                });
            }

            onDelete = function () {
                xKBNMS002.delete(function () {
                    xKBNMS002.search();
                });
            }

            onDeleteAll = function () {
                xKBNMS002.deleteall(function () {
                    xKBNMS002.search();
                });
            }

            onPrint = function () { }

            onExecute = function () { }

            xAjax.onChange('#frmCondition #F_Plant', function () {
                $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

                xKBNMS002.search();
            });





        })


        return {
            frmCondition,
            menus,
            message
        }
    }
}).mount('#vueApp')