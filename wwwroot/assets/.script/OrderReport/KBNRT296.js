$(document).ready(async function () {
    await OrderTypeChange();
    await xSplash.hide();
});

function OrderTypeChange() {
    var prodMonth = $("#F_DeliMonth").val().trim().replaceAll("-", "");
    var OrderType = $("#F_Order").val().trim();
    return xAjax.Post({
        url: 'KBNRT296/Onload',
        data: {
            prodMonth: prodMonth,
            OrderType: OrderType
        },
        then: function (result) {
            $("#F_SupFrom").empty();
            $("#F_SupTo").empty();
            $("#F_SupFrom").append($("<option id='supFromBlank'></option>"));
            $("#F_SupTo").append($("<option id='supToBlank'></option>"));
            console.log(result);
            $.each(result.data, function (i, v) {
                $("#F_SupFrom").append($("<option>", { value: v, text: v }, "</option>"));
                $("#F_SupTo").append($("<option>", { value: v, text: v }, "</option>"));
                $("#supFromBlank").hide();
                $("#supToBlank").hide();
            });
        },
        error: function (result) {
            console.error("Initial Error", result);
        },
    });
}

$("#F_Order").on("change", async function () {
    await OrderTypeChange();
});