$(document).ready(function () {

    function hideBlank() {
        $('#SupFromBlank').hide();
        $('#SupToBlank').hide();
    }
    function initial() {
        return xAjax.Post({
            url: 'KBNRT160/Initial',
            then: function (result) {
                $.each(result.data, function (i, v) {
                    $("#F_SupFrom").append($("<option>", { value: v.Sup_CD, text: v.Sup_CD }, "</option>"));
                    $("#F_SupTo").append($("<option>", { value: v.Sup_CD, text: v.Sup_CD }, "</option>"));
                });
            },
            error: function (result) {
                console.error("Initial Error");
            },
        });
    }
    function deletedOldTemp() {
        return xAjax.Post({
            url: 'KBNRT220/DeleteTemp',
            then: function () {
                xSplash.hide();
            },
            error: function () {
                console.error("DeleteTemp Error");
                xSplash.hide();
            },
        });
    }
    async function onPageload() {
        await initial();
        await deletedOldTemp();
        await hideBlank();
    }

    onPageload();
});