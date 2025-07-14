window.invoiceChart = (containerId, chartData) => {
    Highcharts.chart(containerId, {
        chart: { type: 'column' },
        title: { text: 'Monthly Invoice Totals' },
        xAxis: {
            categories: chartData.labels,
            title: { text: 'Month' }
        },
        yAxis: {
            min: 0,
            title: { text: 'Total Amount (RM)' }
        },
        series: [{
            name: 'RM',
            data: chartData.totals
        }]
    });
};

window.statusChart = function (containerId, chartData) {
    Highcharts.chart(containerId, {
        chart: {
            type: 'pie'
        },
        title: {
            text: 'Invoices by Status'
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.y}</b>'
        },
        accessibility: {
            point: {
                valueSuffix: ''
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.y}'
                }
            }
        },
        series: [{
            name: 'Invoices',
            colorByPoint: true,
            data: chartData
        }]
    });
};

