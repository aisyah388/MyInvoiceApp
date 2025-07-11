window.renderInvoiceChart = (containerId, chartData) => {
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
            name: 'Invoices',
            data: chartData.totals
        }]
    });
};

window.statusChart = function (chartData) {
    Highcharts.chart('statusChartContainer', {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Invoices by Status'
        },
        xAxis: {
            categories: chartData.statusNames,
            crosshair: true
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Invoice Count'
            }
        },
        series: [{
            name: 'Invoices',
            data: chartData.invoiceCounts
        }]
    });
};