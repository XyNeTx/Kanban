$(document).ready(async () => {
    ShowCalendar();
    xSplash.hide();
});

$("#mthDeliYM").change(() => {
    $("#tableMain").find("tbody").remove();
    ShowCalendar();
});

let _command = "inquiry";

ShowCalendar = async () => {

    let YM = $("#mthDeliYM").val();

    let Table = $("#tableMain");
    Table.append("<tbody></tbody>");

    _xLib.AJAX_Get("/api/KBNOR220_2/GetCalendar", { YM: YM.replaceAll("-", "") },
        async (success) => {
            console.log(success);
            let day = 1;
            const startDate = new Date(YM + "-01");
            const endDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, 0);
            //endDate.setMonth(endDate.getMonth() + 1);
            //endDate.setDate(endDate.getDate() - 1);
            //console.log(startDate);
            //console.log(endDate);

            for (let mockDay = 1; mockDay <= 42; mockDay++) {
                let readonly = "readonly";

                if (mockDay % 7 == 1) {
                    Table.find("tbody").append("<tr>");
                }

                success.data["f_workCd_D" + day] == "1" ? readonly = "" : readonly = "readonly";

                if (_command == "inquiry") {
                    readonly = "readonly";
                }

                //console.log("mockDay", mockDay);
                //console.log("startDate.getDay()", startDate.getDay());
                //console.log("endDate.getDate() + startDate.getDay()", endDate.getDate() + startDate.getDay());

                if (mockDay > startDate.getDay() && mockDay <= endDate.getDate() + startDate.getDay()) {

                    Table.find("tbody").find("tr:last").append(
                        `<td class='text-center'>
                        <span>${day}</span></br>
                        <span class='bg-warning' id=span></span></br>
                        <div class='row justify-content-center'>
                            <input type='number' class='form-control w-75' min='0' max='9999'
                            id='QTY_${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}' 
                            name='QTY_${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}' ${readonly} />
                        </div>        
                    </td >`
                    );
                    day++;
                } else {
                    Table.find("tbody").find("tr:last").append("<td></td>");
                }

                if (mockDay % 7 == 0) {
                    Table.find("tbody").append("</tr>");
                }
            }

            $("#tableMain").find("tbody tr:last").find("td").each(function () {
                if($(this).text() == ""){
                    $(this).remove();
                }
            });
        },
        function (error) {
            console.log(error);
        }
    );
}