$(document).ready(async function () {
    await initial();
});

function hideBlank() {
    $('#FlagFromBlank').hide();
    $('#FlagToBlank').hide();
}
function initial() {
    return xAjax.Post({
        url: 'KBNRT250/F_System_Flag',
        then: function (result) {
            $.each(result.data, async function (i, v) {
                $("#F_FlagFrom").append($("<option>", { value: v, text: v }, "</option>"));
                $("#F_FlagTo").append($("<option>", { value: v, text: v }, "</option>"));
                await hideBlank();
                await xSplash.hide();
            });
        },
        error: function (result) {
            console.error("Initial Error");
        },
    });
}