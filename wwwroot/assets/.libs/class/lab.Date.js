class labDate {

    constructor(pFormat = 'yyyy-MM-dd HH:mm:ss') {
        this.Format = pFormat;
        this.Value = pFormat;

        const _now = new Date();
        const _year = _now.getFullYear();
        const _month = String(_now.getMonth() + 1).padStart(2, '0');
        const _date = String(_now.getDate()).padStart(2, '0');
        const _hours = String(_now.getHours()).padStart(2, '0');
        const _minutes = String(_now.getMinutes()).padStart(2, '0');
        const _seconds = String(_now.getSeconds()).padStart(2, '0');

        this.Value = this.Value.replace('yyyy', _year);
        this.Value = this.Value.replace('yy', _year);

        this.Value = this.Value.replace('MM', _month);

        this.Value = this.Value.replace('dd', _date);

        this.Value = this.Value.replace('HH', _hours);

        this.Value = this.Value.replace('mm', _minutes);

        this.Value = this.Value.replace('ss', _seconds);

    }


    Now(pFormat = 'yyyy-MM-dd HH:mm:ss') {
        const _now = new Date();
        const _year = _now.getFullYear();
        const _month = String(_now.getMonth() + 1).padStart(2, '0');
        const _date = String(_now.getDate()).padStart(2, '0');
        const _hours = String(_now.getHours()).padStart(2, '0');
        const _minutes = String(_now.getMinutes()).padStart(2, '0');
        const _seconds = String(_now.getSeconds()).padStart(2, '0');

        pFormat = pFormat.replace('yyyy', _year);
        pFormat = pFormat.replace('yy', _year);

        pFormat = pFormat.replace('MM', _month);

        pFormat = pFormat.replace('dd', _date);

        pFormat = pFormat.replace('HH', _hours);

        pFormat = pFormat.replace('mm', _minutes);

        pFormat = pFormat.replace('ss', _seconds);

        return pFormat;
    }
    now(pFormat = 'yyyy-MM-dd HH:mm:ss') {
        return this.Now(pFormat);
    }


    Date(pFormat = 'yyyy-MM-dd') {
        const _now = new Date();
        const _year = _now.getFullYear();
        const _month = String(_now.getMonth() + 1).padStart(2, '0');
        const _date = String(_now.getDate()).padStart(2, '0');

        pFormat = pFormat.replace('yyyy', _year);
        pFormat = pFormat.replace('yy', _year);

        pFormat = pFormat.replace('MM', _month);
        pFormat = pFormat.replace('mm', _month);

        pFormat = pFormat.replace('dd', _date);

        return pFormat;
    }
    date(pFormat = 'yyyy-MM-dd') {
        return this.Date(pFormat);
    }


    Time(pFormat = 'HH:mm:ss') {
        const _now = new Date();
        const _hours = String(_now.getHours()).padStart(2, '0');
        const _minutes = String(_now.getMinutes()).padStart(2, '0');
        const _seconds = String(_now.getSeconds()).padStart(2, '0');

        pFormat = pFormat.replace('HH', _hours);
        pFormat = pFormat.replace('hh', _hours);

        pFormat = pFormat.replace('mm', _minutes);

        pFormat = pFormat.replace('ss', _seconds);

        return pFormat;
    }
    time(pFormat = 'HH:mm:ss') {
        return this.Time(pFormat);
    }

}

const xDate = new labDate();

