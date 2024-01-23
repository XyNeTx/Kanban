const { createApp, onBeforeMount, onMounted, ref } = Vue

createApp({
    setup() {

        onBeforeMount(() => {
            console.info('Layout onBeforeMount');

            //fetch(_HOSTNAME_ + '/assets/template/Menu/' + _UID_ + '.json')
            //    .then((response) => response.json())
            //    .then((json) => {
            //        console.log(json)

            //        //menus.value = json
            //        menus.value = `
            //                        <li class="pcoded-hasmenu pcoded-trigger" dropdown-icon="style1" subitem-icon="style1">
            //                            <a href="javascript:void(0)">
            //                                <span class="pcoded-micon"><i class="feather icon-home"></i></span>
            //                                <span class="pcoded-mtext">Dashboard</span>
            //                            </a>
            //                            <ul class="pcoded-submenu">
            //                                <li class>
            //                                    <a href="../index.html">
            //                                        <span class="pcoded-mtext">Default</span>
            //                                    </a>
            //                                </li>
            //                                <li class>
            //                                    <a href="dashboard-crm.html">
            //                                        <span class="pcoded-mtext">CRM</span>
            //                                    </a>
            //                                </li>
            //                                <li class="active">
            //                                    <a href="dashboard-analytics.html">
            //                                        <span class="pcoded-mtext">Analytics</span>
            //                                        <span class="pcoded-badge label label-info ">NEW</span>
            //                                    </a>
            //                                </li>
            //                            </ul>
            //                        </li>
            //                        `

            //        //$('.pcoded-item.pcoded-left-item').append(menus.value);

            //    })
            //    .catch(error => {
            //        console.warn('Layout.LoadMenu| ' + error);
            //    });

        })

        onMounted(() => {
            console.info('Layout onMounted');


            //console.log($('li.active').text());

            //fetch(_HOSTNAME_ + '/assets/template/Menu/' + _UID_ + '.json')
            //    .then((response) => response.json())
            //    .then((json) => {
            //        console.log(json)

            //        //menus.value = json
            //        menus.value = `
            //                        <li class="pcoded-hasmenu active pcoded-trigger">
            //                            <a href="javascript:void(0)">
            //                                <span class="pcoded-micon"><i class="feather icon-home"></i></span>
            //                                <span class="pcoded-mtext">Dashboard</span>
            //                            </a>
            //                            <ul class="pcoded-submenu">
            //                                <li class>
            //                                    <a href="../index.html">
            //                                        <span class="pcoded-mtext">Default</span>
            //                                    </a>
            //                                </li>
            //                                <li class>
            //                                    <a href="dashboard-crm.html">
            //                                        <span class="pcoded-mtext">CRM</span>
            //                                    </a>
            //                                </li>
            //                                <li class="active">
            //                                    <a href="dashboard-analytics.html">
            //                                        <span class="pcoded-mtext">Analytics</span>
            //                                        <span class="pcoded-badge label label-info ">NEW</span>
            //                                    </a>
            //                                </li>
            //                            </ul>
            //                        </li>
            //                        `

            //        $('.pcoded-item.pcoded-left-item').append(menus.value);

            //    })
            //    .catch(error => {
            //        console.warn('Layout.LoadMenu| ' + error);
            //    });


            //console.log('onMounted')
            //xSplash.hide()
        })


        return {
        }
    }
}).mount('#vueApp')