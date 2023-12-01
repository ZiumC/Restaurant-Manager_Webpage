let barXvalues = ["Total salary", "Total bonus"];
let barYvalues = [0, 0];
let barColors = ["#2B3F63", "#2C632B"];

let doughnutXvalues = ["New", "Pending", "Accepted", "Rejected"];
let doughnutYvalues = [0, 0, 0, 0]
let doughnutColors = ["#2A8095", "#8F842F", "#3A2A80", "#8F3D2F"];

Chart.plugins.register({
    afterDraw: function (chart) {
        if (chart.data.datasets[0].data.every(item => item === 0)) {
            let ctx = chart.chart.ctx;
            let width = chart.chart.width;
            let height = chart.chart.height;

            chart.clear();
            ctx.save();
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText('No data to display', width / 2, height / 2);
            ctx.restore();
        }
    }
});