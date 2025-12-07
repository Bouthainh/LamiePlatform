
const intelligenceColors = {
    "الذكاء اللغوي": "#FF3C4B",
    "الذكاء المنطقي": "#FF96AA",
    "الذكاء البصري": "#A03CC8",
    "الذكاء الجسدي": "#32C8C3",
    "الذكاء الموسيقي": "#FFB90F",
    "الذكاء الاجتماعي": "#FF622E",
    "الذكاء الذاتي": "#A4C83C",
    "الذكاء الطبيعي": "#6B9DC0"
};

function initDashboardCharts(barLabels, barData, radarLabels, radarData) {

    // Generate colors based on labels
    const barColors = barLabels.map(label => intelligenceColors[label] || "#CCCCCC");
    const radarColors = radarLabels.map(label => intelligenceColors[label] || "#CCCCCC");

    //Bar Chart
    new Chart(document.getElementById('barChart'), {
        type: 'bar',
        data: {
            labels: barLabels,
            datasets: [{
                label: "",
                data: barData,
                backgroundColor: barColors
            }]
        },
        options: {
            plugins: {
                legend: { display: false }
            }
        }
    });

    //Radar Chart
    new Chart(document.getElementById('radarChart'), {
        type: 'radar',
        data: {
            labels: radarLabels,
            datasets: [{
                label: "",
                data: radarData,
                backgroundColor: radarColors.map(c => c + "70"),
                borderColor: radarColors,
                pointBackgroundColor: radarColors,
                fill: true
            }]

        },
        options: {
            plugins: {
                legend: { display: false }
            }
        }

    });
}
