class libChartJS {
    constructor() {
        //console.log("load");



    }



    //### Line Chart
    Line = function (pConfig = null) {

        let _title = {
            text: '',
            display: true,
            align: 'start',
            font: { size: 16, family: 'tahoma', weight: 'bold', style: null },
        };
        if (pConfig.title != undefined) {
            _title = {
                text: (pConfig.title.text == undefined ? _title.text : pConfig.title.text),
                display: true,
                align: (pConfig.title.position == undefined ? _title.align : pConfig.title.position),
                font: (pConfig.title.font == undefined ? _title.font : pConfig.title.font),
            };
        }


        let _legend = {
            position: 'bottom'
        }
        if (pConfig.legend != undefined) {
            _legend = {
                position: (pConfig.legend.position == undefined ? _legend.position : pConfig.legend.position)
            }
        }

        let _scales = {
            y: {
                min: null,
                max: null
            }
        }
        if (pConfig.y != undefined) {
            _scales = {
                y: {
                    min: pConfig.y.min,
                    max: pConfig.y.max
                }
            }
        }

        let _dataset = [];
        for (var i = 0; i < pConfig.x.data.length; i++) {
            let _ds =
            {
                label: pConfig.x.data[i].label,
                data: pConfig.x.data[i].data,
                borderWidth: 1,
            }
            _dataset.push(_ds);
        }
        //console.log(_dataset);



        //console.log(_legend);

        const Utils = ChartUtils.init();

        const plugin = {
            id: 'customCanvasBackgroundColor',
            beforeDraw: (chart, args, options) => {
                const { ctx } = chart;
                ctx.save();
                ctx.globalCompositeOperation = 'destination-over';
                ctx.fillStyle = options.color || '#99ffff';
                ctx.fillRect(0, 0, chart.width, chart.height);
                ctx.restore();
            }
        };


        var _ChartJS;
        _ChartJS = new Chart($('#' + pConfig.name), {
            type: 'line',
            data: {
                labels: pConfig.x.label,
                datasets: _dataset
            },
            options: {
                responsive: true,
                interaction: {
                    intersect: false,
                    mode: 'index'
                },
                plugins: {
                    title: _title,
                    legend: _legend,
                    tooltip: {
                        position: 'nearest'
                    },
                    customCanvasBackgroundColor: {
                        color: 'white',
                    }
                },
                scales: _scales,
                animation: {
                    onComplete: function (e) {
                        ((pConfig.then != undefined && typeof (pConfig.then) === 'function') ? pConfig.then() : null);
                    }
                },
            },
            plugins: [plugin],
        });

        return _ChartJS;
    }




    //### Bar Chart
    Bar = function (pConfig = null) {

        let _title = {
            text: '',
            display: true,
            align: 'start',
            font: { size: 16, family: 'tahoma', weight: 'bold', style: null },
        };
        if (pConfig.title != undefined) {
            _title = {
                text: (pConfig.title.text == undefined ? _title.text : pConfig.title.text),
                display: true,
                align: (pConfig.title.position == undefined ? _title.align : pConfig.title.position),
                font: (pConfig.title.font == undefined ? _title.font : pConfig.title.font),
            };
        }

        let _legend = {
            position: 'bottom'
        }
        if (pConfig.legend != undefined) {
            _legend = {
                position: (pConfig.legend.position == undefined ? _legend.position : pConfig.legend.position)
            }
        }

        let _scales = {
            y: {
                min: null,
                max: null,
            }
        }
        if (pConfig.y != undefined) {
            _scales = {
                y: {
                    min: pConfig.y.min,
                    max: pConfig.y.max
                }
            }
        }

        let _dataset = [];
        for (var i = 0; i < pConfig.x.data.length; i++) {
            let _ds =
            {
                label: pConfig.x.data[i].label,
                data: pConfig.x.data[i].data,
                borderWidth: 1,
            }
            _dataset.push(_ds);
        }


        //console.log(_legend);

        const Utils = ChartUtils.init();

        const plugin = {
            id: 'customCanvasBackgroundColor',
            beforeDraw: (chart, args, options) => {
                const { ctx } = chart;
                ctx.save();
                ctx.globalCompositeOperation = 'destination-over';
                ctx.fillStyle = options.color || '#99ffff';
                ctx.fillRect(0, 0, chart.width, chart.height);
                ctx.restore();
            }
        };

        var _ChartJS;
        _ChartJS = new Chart($('#' + pConfig.name), {
            type: 'bar',
            data: {
                labels: pConfig.x.label,
                datasets: _dataset
            },
            options: {
                responsive: true,
                interaction: {
                    intersect: false,
                    mode: 'index'
                },
                plugins: {
                    title: _title,
                    legend: _legend,
                    tooltip: {
                        position: 'nearest'
                    },
                    customCanvasBackgroundColor: {
                        color: 'white',
                    }
                },
                scales: _scales,
                animation: {
                    onComplete: function () {
                        ((pConfig.then != undefined && typeof (pConfig.then) === 'function') ? pConfig.then() : null);

                    }
                },
            },
            plugins: [plugin],
        });

        return _ChartJS;
    }




    //### Stacked Bar Chart
    StackBar = function (pConfig = null) {

        let _title = {
            text: '',
            display: true,
            align: 'start',
            font: { size: 16, family: 'tahoma', weight: 'bold', style: null },
        };
        if (pConfig.title != undefined) {
            _title = {
                text: (pConfig.title.text == undefined ? _title.text : pConfig.title.text),
                display: true,
                align: (pConfig.title.position == undefined ? _title.align : pConfig.title.position),
                font: (pConfig.title.font == undefined ? _title.font : pConfig.title.font),
            };
        }


        let _legend = {
            position: 'bottom'
        }
        if (pConfig.legend != undefined) {
            _legend = {
                position: (pConfig.legend.position == undefined ? _legend.position : pConfig.legend.position)
            }
        }

        let _scales = {
            x: {
                stacked: true,
            },
            y: {
                min: null,
                max: null,
                stacked: true
            }
        }
        if (pConfig.y != undefined) {
            _scales = {
                y: {
                    min: pConfig.y.min,
                    max: pConfig.y.max
                }
            }
        }

        let _dataset = [];
        for (var i = 0; i < pConfig.x.data.length; i++) {
            let _ds =
            {
                label: pConfig.x.data[i].label,
                data: pConfig.x.data[i].data,
                borderWidth: 1,
            }
            _dataset.push(_ds);
        }


        //console.log(_legend);

        const Utils = ChartUtils.init();

        const plugin = {
            id: 'customCanvasBackgroundColor',
            beforeDraw: (chart, args, options) => {
                const { ctx } = chart;
                ctx.save();
                ctx.globalCompositeOperation = 'destination-over';
                ctx.fillStyle = options.color || '#99ffff';
                ctx.fillRect(0, 0, chart.width, chart.height);
                ctx.restore();
            }
        };

        var _ChartJS;
        _ChartJS = new Chart($('#' + pConfig.name), {
            type: 'bar',
            data: {
                labels: pConfig.x.label,
                datasets: _dataset
            },
            options: {
                responsive: true,
                interaction: {
                    intersect: false,
                    mode: 'index'
                },
                plugins: {
                    title: _title,
                    legend: _legend,
                    tooltip: {
                        position: 'nearest'
                    },
                    customCanvasBackgroundColor: {
                        color: 'white',
                    }
                },
                scales: _scales,
                animation: {
                    onComplete: function () {
                        ((pConfig.then != undefined && typeof (pConfig.then) === 'function') ? pConfig.then() : null);

                    }
                },
            },
            plugins: [plugin],
        });

        return _ChartJS;

    }




    //### Stacked Bar & Line Chart
    StackBarLine = function (pConfig = null) {

        //### For initial Utilities
        const Utils = ChartUtils.init();

        //console.log(Utils);

        const plugin =
        {
            id: 'customCanvasBackgroundColor',
            beforeDraw: (chart, args, options) => {
                const { ctx } = chart;
                ctx.save();
                ctx.globalCompositeOperation = 'destination-over';
                ctx.fillStyle = options.color || '#99ffff';
                ctx.fillRect(0, 0, chart.width, chart.height);
                ctx.restore();
            }
        };

        let _title = {
            text: '',
            display: true,
            align: 'start',
            font: { size: 16, family: 'tahoma', weight: 'bold', style: null },
        };
        if (pConfig.title != undefined) {
            _title = {
                text: (pConfig.title.text == undefined ? _title.text : pConfig.title.text),
                display: true,
                align: (pConfig.title.position == undefined ? _title.align : pConfig.title.position),
                font: (pConfig.title.font == undefined ? _title.font : pConfig.title.font),
            };
        }

        let _legend = {

            position: 'bottom'
        }
        if (pConfig.legend != undefined) {
            _legend = {
                //labels: {
                //    usePointStyle: true
                //},
                position: (pConfig.legend.position == undefined ? _legend.position : pConfig.legend.position)
            }
        }

        let _scales = {
            x: {
                stacked: true,
            },
            y: {
                min: null,
                max: null,
                stacked: true
            }
        }
        if (pConfig.y != undefined) {
            _scales = {
                x: {
                    stacked: true,
                },
                y: {
                    min: pConfig.y.min,
                    max: pConfig.y.max,
                    stacked: true
                }
            }
        }

        let _dataset = [];
        for (var i = 0; i < pConfig.x.data.length; i++) {
            //console.log(pConfig.x.data[i].type);
            let _ds =
            {
                label: pConfig.x.data[i].label,
                data: pConfig.x.data[i].data,
                borderWidth: 1,
                type: (pConfig.x.data[i].type != undefined ? pConfig.x.data[i].type : (i == pConfig.x.line ? 'line' : 'bar')),
            }
            _dataset.push(_ds);
        }
        //console.log(_dataset);
        var _ChartJS;
        _ChartJS = new Chart($('#' + pConfig.name), {
            type: 'bar',
            data: {
                labels: pConfig.x.label,
                datasets: _dataset
            },
            options: {
                responsive: true,
                interaction: {
                    intersect: false,
                    mode: 'index'
                },
                plugins: {
                    title: _title,
                    legend: _legend,
                    tooltip: {
                        position: 'nearest'
                    },
                    customCanvasBackgroundColor: {
                        color: 'white',
                    },

                },
                scales: _scales,
                animation: {
                    onComplete: function () {

                        if (_ChartJS != undefined) {
                            if (pConfig.x.line != undefined) {
                                _ChartJS.data.datasets[pConfig.x.line].borderWidth = 1;
                                _ChartJS.data.datasets[pConfig.x.line].borderColor = Utils.CHART_COLORS.line;
                                _ChartJS.data.datasets[pConfig.x.line].backgroundColor = Utils.CHART_COLORS.line;
                                //_ChartJS.data.datasets[pConfig.x.line].pointStyle = 'line';
                                _ChartJS.update();
                            } else {
                                for (var b = 0; b < _ChartJS.data.datasets.length; b++) {
                                    //console.log(_ChartJS.data.datasets[b]);
                                    if (_ChartJS.data.datasets[b].type == 'line') {
                                        _ChartJS.data.datasets[b].borderWidth = 1;
                                        _ChartJS.data.datasets[b].borderColor = Utils.CHART_COLORS.line;
                                        _ChartJS.data.datasets[b].backgroundColor = Utils.CHART_COLORS.line;
                                        //_ChartJS.data.datasets[b].pointStyle = 'line';
                                        _ChartJS.update();
                                    }

                                }
                            }

                            ((pConfig.then != undefined && typeof (pConfig.then) === 'function') ? pConfig.then() : null);
                        }

                    }
                },
            },
            plugins: [plugin],
        });

        return _ChartJS;



    }



}
const xChartJS = new libChartJS();
