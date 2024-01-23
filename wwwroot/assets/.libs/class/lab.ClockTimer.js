class labClock {
    constructor() {
        this.Clock;
    }

    Start(pConfig = null, fncCounting = null) {
        let _interval = 1000;

        if (pConfig != null) {
            _interval = (typeof (pConfig) == 'number' ? pConfig : _interval);
            _interval = (pConfig.start != undefined ? pConfig.start : _interval);
        }

        this.Clock = setInterval(function () {
            if (pConfig != null) {
                if (typeof (pConfig) == 'function') pConfig();
                if (pConfig.counting != undefined) if (typeof (pConfig.counting) == 'function') pConfig.counting();
            }
            if (fncCounting != null) {
                if (fncCounting != undefined) if (typeof (fncCounting) == 'function') fncCounting();
            }
        }, _interval);
    };

    Stop(pConfig = null) {
        if (pConfig != null) {
            if (typeof (pConfig.finish) == 'function') {
                pConfig.finish();
            };
        }
        clearInterval(this.Clock);
    };

}
const xClock= new labClock();




const _xTimer_Property = {
    "start": 0,
    "stop": 0,
    "current": 0,
    "percent": 0
};

class labTimer {
    constructor() {
        this.Clock = new labClock();
    }


    Stoper(pConfig = null) {
        _xTimer_Property.start = pConfig.start;
        _xTimer_Property.stop = pConfig.stop;

        var Timer = setInterval(function () {
            _xTimer_Property.current = _xTimer_Property.current + pConfig.start;
            _xTimer_Property.percent = parseFloat((_xTimer_Property.current / _xTimer_Property.stop)*100).toFixed(2);
            
            if (typeof (pConfig.counting) == 'function') {
                pConfig.counting(_xTimer_Property);
            };
        }, pConfig.start);

        setTimeout(function () {
            if (typeof (pConfig.finish) == 'function') {
                pConfig.counting(_xTimer_Property);
                pConfig.finish();
            };
            clearInterval(Timer);
        }, pConfig.stop);
    };
    stoper(pConfig = null) {
        this.Stoper(pConfig);
    }
    Timer(pConfig = null) {
        this.Stoper(pConfig);
    }
    timer(pConfig = null) {
        this.Stoper(pConfig);
    }
}
const xTimer = new labTimer();
