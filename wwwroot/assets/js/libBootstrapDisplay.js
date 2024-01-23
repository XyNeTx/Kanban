$(document).ready(function () {


    //### Adjust column of datatable in Modal
    $("[id*='Modal']").on('shown.bs.modal', function (e) {
        $('#' + e.currentTarget.id + ' table').each(function () {
            if (this.id != '') {

                if ($('#' + this.id + ' thead').length != 0) $('#' + this.id).DataTable().columns.adjust(function () {
                    console.log('after adjust');
                });
            }

            xSplash.hide();
        })

        //$('form').find('select').each(function () {
        //    if (this.id != '') {
        //        //console.log(this.id + '>>>' + this.value);
        //    }          

        //})

    });



    //### Adjust column of datatable in Tab-Panel
    $('[data-bs-toggle="pill"]').on('shown.bs.tab', function (e) {
        $('#' + ReplaceAll(e.currentTarget.id, '-tab', '') + ' table').each(function () {

            console.log(this.id);
            if (this.id != '') {
                if ($('#' + this.id + ' thead').length != 0) $('#' + this.id).DataTable().columns.adjust(function () {
                    console.log('after adjust');
                });
            }

            xSplash.hide();
        })

    });

});


//###### backdrop managment
$(document).on('show.bs.modal', '.modal', function (e) {
    const zIndex = 1040 + 10 * $('.modal:visible').length;
    $(this).css('z-index', zIndex);
    setTimeout(() => $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack'));

});