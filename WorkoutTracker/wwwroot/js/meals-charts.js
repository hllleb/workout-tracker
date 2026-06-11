/**
 * Renders daily nutrition charts on the meals index page.
 * @param {Array<{date: string, calories: number, proteinG: number, carbsG: number, fatG: number}>} data
 */
function initMealsCharts(data) {
    if (!data || data.length === 0 || typeof Chart === 'undefined') {
        return;
    }

    const labels = data.map((entry) => entry.date);

    const caloriesCanvas = document.getElementById('caloriesChart');
    if (caloriesCanvas) {
        new Chart(caloriesCanvas, {
            type: 'bar',
            data: {
                labels,
                datasets: [{
                    label: 'Calories',
                    data: data.map((entry) => entry.calories),
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    title: { display: true, text: 'Daily calories' }
                },
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });
    }

    const macrosCanvas = document.getElementById('macrosChart');
    if (macrosCanvas) {
        new Chart(macrosCanvas, {
            type: 'bar',
            data: {
                labels,
                datasets: [
                    {
                        label: 'Protein (g)',
                        data: data.map((entry) => entry.proteinG),
                        backgroundColor: 'rgba(255, 99, 132, 0.6)'
                    },
                    {
                        label: 'Carbs (g)',
                        data: data.map((entry) => entry.carbsG),
                        backgroundColor: 'rgba(75, 192, 192, 0.6)'
                    },
                    {
                        label: 'Fat (g)',
                        data: data.map((entry) => entry.fatG),
                        backgroundColor: 'rgba(255, 206, 86, 0.6)'
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    title: { display: true, text: 'Daily macros (g)' }
                },
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });
    }
}
