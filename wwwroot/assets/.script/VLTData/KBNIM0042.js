$(document).ready(async function () {

    await _xLib.AJAX_Get("/api/KBNIM0042/GetCustomerCode", '',
        function (success)
        {
            if(success.status == "200")
            {
                $("#F_Customer").empty();
                $("#F_Customer").append('<option value="" hidden></option>');
                success.data = JSON.parse(success.data);
                success.data = _xLib.TrimArrayJSON(success.data);
                success.data.forEach(function (item)
                {
                    $("#F_Customer").append('<option value="' + item.F_Customer_Cd + '">' + item.F_Customer_Cd + '</option>');
                });
            }
        }
        );

    xSplash.hide();
});