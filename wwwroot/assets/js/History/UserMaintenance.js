class _History {
    constructor() {
        this.Data = null;
        this.Date = new labDate();
        this.Timer;
    }

    onLoad(row = null) {
        console.log(this.Data);

        $.each(this.Data, function (key, value) {
            $('#divHistoryModal #' + key).val(value);
            $('#divHistoryModal #' + key).attr('disabled', 'disabled');

            
        });
    }

    onClickbtnTest() {
        console.log(this.Data.UpdateAt);

    }
}
const xHistory = new _History();