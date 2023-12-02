let barXvalues = ["Total salary", "Total bonus"];
let barYvalues = [0, 0];
let barColors = ["#2B3F63", "#2C632B"];

let doughnutComplainsXvalues = ["New", "Pending", "Accepted", "Rejected"];
let doughnutComplainsYvalues = [0, 0, 0, 0]
let doughnutColors = ["#35A2BD", "#CFB43C", "#62B361", "#BE4648"];

let doughnutEmployeesXvalues = ["Owner", "Chef", "Chef helper", "Waiter"];
let doughnutEmployeesYvalues = [0, 0, 0, 0]
let doughnutEmployeesColors = ["#833AB0", "#C2B02D", "#C78095", "#3055AB"];

let pieReservationsXvalues = ["New", "Confirmed", "Cancelled", "Rated"];
let pieReservationsYvalues = [0, 0, 0, 0]
let pieReservationsColors = ["#C36AB2", "#5EAD4C", "#D8464C", "#4585CC"];

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