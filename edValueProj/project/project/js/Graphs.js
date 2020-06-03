
var options = {
    responsive: true,
    maintainAspectRatio: true,
    legend: {
        labels: {
            // This more specific font property overrides the global property
            fontColor: 'black',
            fontSize: 20,
            fontFamily: "Varela Round,Arial,sans-serif"
        }
    },
    scales: {
        yAxes: [{
            ticks: {
                fontColor: 'black',
                fontSize: 18,
                fontFamily: "Varela Round,Arial,sans-serif"
               
            }
        }],
        xAxes: [{
            ticks: {
                fontColor: 'black',
                fontSize: 18,
                fontFamily: "Varela Round,Arial,sans-serif"
            }
        }]

    }

};

var gradesAvgChart;
var IntPercentageChart;
var intlMultiBar;

var intDictionary = {
    "logically": "לוגית",
    "movemental": "תנועתית",
    "musically": "מוזיקלית",
    "personal": "אישית",
    "spatial": "מרחבית",
    "tongue": "לשונית",
    "לא ביצעו": "לא ביצעו"
};



function GradesAvgChart() {
    var IntData = [];
    var medianArr = [];
    var IntLabels = [];
    var obj = [];
    var medianValue = 0.0;

    var ctx = document.getElementById('GradeChart').getContext('2d');

    for (x in sortedGraphData) {
        if (!obj[sortedGraphData[x].IntelligenceName] && sortedGraphData[x].IntelligenceName != "לא ביצעו") {
            obj[sortedGraphData[x].IntelligenceName] = [0, 0];

        }
        if (obj[sortedGraphData[x].IntelligenceName] != "לא ביצעו" && sortedGraphData[x].Grade != 0) {
            obj[sortedGraphData[x].IntelligenceName][0] += parseInt(sortedGraphData[x].Grade);
            obj[sortedGraphData[x].IntelligenceName][1]++;
            medianArr.push(sortedGraphData[x].Grade);
        }

    }
    for (x in obj) {
        IntLabels.push(intDictionary[x]);
        IntData.push(obj[x][0] / obj[x][1]);
    }
    console.log(IntLabels);
    console.log(IntData);
    medianValue = median(medianArr);
 

    gradesAvgChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: IntLabels,
            datasets: [
                {
                    label: 'ממוצע ציונים לפי אינטלגנציות',
                    data: IntData,
                    backgroundColor: ['rgba(0, 123, 255, 0.78)', 'rgba(102, 16, 242, 0.78)', 'rgba(32, 201, 151, 0.78)', 'rgba(220, 53, 69, 0.78)', 'rgba(40, 167, 69, 0.78)', 'rgba(23, 162, 184, 0.78)', 'rgba(253, 126, 20, 0.78)'],
                    borderColor: ['#007bff', '#6610f2', '#20c997', '#dc3545', '#28a745', '#17a2b8', '#fd7e14'],
                    borderWidth: 3
                }
            ]
        },
        options: {
            legend: {
                labels: {
                    // This more specific font property overrides the global property
                    fontColor: 'black',
                    fontSize: 20,
                    fontFamily:"Varela Round,Arial,sans-serif"
                }
            },
            animation: {
                duration: 100
            },
            scales: {
                yAxes: [{
                    display: true,
                    ticks: {
                        beginAtZero: true,   // minimum value will be 0.
                        suggestedMax: 100,
                        fontColor: 'black',
                        fontSize: 18,
                        fontFamily: "FbSpacer"
                    }
                }],
                xAxes: [{
                    ticks: {
                        fontColor: 'black',
                        fontSize: 18,
                        fontFamily: "Varela Round,Arial,sans-serif"
                    }
                }]
            },
            annotation: {
                annotations: [{
                    type: 'line',
                    mode: 'horizontal',
                    scaleID: 'y-axis-0',
                    value: medianValue,
                    borderColor: 'red',
                    borderWidth: 4,
                    label: {
                        enabled: true,
                        content: 'Median'
                    }
                }]
            }
        },

        animation: {
            animateScale: true,
            animateRotate: true
        }
    });
    gradesAvgChart.update();
}
function median(arrForMedian) {
    // median of [3, 5, 4, 4, 1, 1, 2, 3] = 3
    var median = 0, arrLen = arrForMedian.length;
    arrForMedian.sort();

    if (arrLen % 2 === 0) {  // is even
        // average of two middle numbers
        median = (parseInt(arrForMedian[arrLen / 2 - 1]) + parseInt(arrForMedian[arrLen / 2])) / 2;
    }
    else { // is odd
        // middle number only
        median = arrForMedian[(arrLen - 1) / 2];
    }

    return median;
}
function PercentageChart() {

    var ctx = document.getElementById('PercentageChart').getContext('2d');
    var IntData = [];
    var IntLabels = [];
    var obj = [];
    var sum = sortedGraphData.length;
    for (x in sortedGraphData) {
        if (!obj[sortedGraphData[x].IntelligenceName]) {
            obj[sortedGraphData[x].IntelligenceName] = [0];

        }
        obj[sortedGraphData[x].IntelligenceName]++;



    }
    for (x in obj) {
        IntLabels.push(intDictionary[x]);
        IntData.push(((obj[x] / sum) * 100).toFixed(2));
    }

    IntPercentageChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: IntLabels,
            datasets: [
                {
                    label: 'אחוז תלמידים לפי אינטליגנציות',
                    data: IntData,
                    backgroundColor: ['rgba(0, 123, 255, 0.78)', 'rgba(102, 16, 242, 0.78)', 'rgba(32, 201, 151, 0.78)', 'rgba(220, 53, 69, 0.78)', 'rgba(40, 167, 69, 0.78)', 'rgba(23, 162, 184, 0.78)', 'rgba(253, 126, 20, 0.78)'],
                    borderColor: ['#007bff', '#6610f2', '#20c997', '#dc3545', '#28a745', '#17a2b8', '#fd7e14'],
                    borderWidth: 3
                  
                }
            ]
        },
        options: {
            responsive: true,
            legend: {
                labels: {
                    // This more specific font property overrides the global property
                    fontColor: 'black',
                    fontSize: 20,
                    fontFamily: "Varela Round,Arial,sans-serif"
                }
            }
        },
        
        animation: {
            animateScale: true,
            animateRotate: true
        }
    });
    IntPercentageChart.update();
}


function ChartIntl() {

    var ctx = document.getElementById('intlChart').getContext('2d');
    var intPoints = {
        "logically": [0, 0, 0, 0],//a-סוכם ציונים של השאלון||b-סוכם ציונים של המערכת||c-סופר מספר שאלונים||d-סופר את מספר ציונים שונים מאפס
        "movemental": [0, 0, 0, 0],
        "musically": [0, 0, 0, 0],
        "personal": [0, 0, 0, 0],
        "spatial": [0, 0, 0, 0],
        "tongue": [0, 0, 0, 0]
     
    }
    var dataSpoints = [];
    var dataPoints = [];
    for (x in intSortedGraphData) {
        if (!intSortedGraphData[x].hasOwnProperty("Spoints")) {
            for (y in intDictionary) {
                if (intDictionary[y] != "לא ביצעו") {
                    if (intSortedGraphData[x][y + "Spoints"] > 0) {
                        intPoints[y][1] += parseInt(intSortedGraphData[x][y + "Spoints"]);
                        intPoints[y][3] += 1;
                    }
                    if (intSortedGraphData[x][y + "points"] > 0) {
                        intPoints[y][0] += parseInt(intSortedGraphData[x][y + "points"]);
                        intPoints[y][2] += 1;
                    }
                }
            }
        }       
    }

    for (x in intPoints) {
        dataPoints.push((intPoints[x][0] / intPoints[x][2]).toFixed(2))
        dataSpoints.push(((intPoints[x][1] / intPoints[x][3]) / 10).toFixed(2))
    }
    console.log(intPoints);
    intlMultiBar = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['לוגית', 'תנועתית', 'מוזיקלית', 'אישית', 'מרחבית', 'לשונית'],
            datasets: [
                {
                    label: 'דירוג שאלון',
                    data: dataPoints,
                    backgroundColor: 'rgba(200, 150, 132, 1)',
                    borderColor: 'rgba(200, 150, 132, 2)',
                    borderWidth: 1
                }, {
                    label: 'דירוג מערכת',
                    data: dataSpoints,
                    backgroundColor: 'rgba(12, 150, 132, 1)',
                    borderColor: 'rgba(12, 150, 132, 2)',
                    borderWidth: 1
                }
            ]
        },
        options: {
            legend: {
                labels: {
                    // This more specific font property overrides the global property
                    fontColor: 'black',
                    fontSize: 20,
                    fontFamily: "Varela Round,Arial,sans-serif"
                }
            },

            animation: {
                duration: 100
            },
            scales: {
                yAxes: [{
                    display: true,
                    ticks: {
                        beginAtZero: true,   // minimum value will be 0.
                        suggestedMax: 10,
                        fontColor: 'black',
                        fontSize: 18,
                        fontFamily: "FbSpacer"
                    }
                }],
                xAxes: [{
                    ticks: {
                        fontColor: 'black',
                        fontSize: 18,
                        fontFamily: "Varela Round,Arial,sans-serif"
                    }
                }]
            }
        },

        animation: {
            animateScale: true,
            animateRotate: true
        }
    });
    intlMultiBar.update();
}
      

function chartIntl(data) {

    var ctx = document.getElementById('').getContext('2d');

    multiBar = new Chart(ctx, {
        type: 'bar',
        data: {
            
        },
        options: options,

        animation: {
            animateScale: true,
            animateRotate: true
        }
    });
 
}



function UpdateTaskDoughnut() {
    var egish = 0;
    var loegish = 0;
    var ctx2 = document.getElementById('TaskChart').getContext('2d');
    for (x in sortedGraphData) {
        if (sortedGraphData[x].isperform == 1)
            egish++;
        else
            loegish++;
    }
    task = new Chart(ctx2, {
        type: 'doughnut',
        data: {
            labels: ['הוגש', 'לא הוגש'],
            datasets: [
                {

                    data: [egish, loegish],
                    backgroundColor: ['rgba(0, 255, 33, 0.78)','rgba(255, 0, 0, 0.78)'],
                    borderColor: ['#00ff21', '#ff0000'],
                    borderWidth: 3

                },

            ]
        },
        options: {
            responsive: true,
            legend: {
                position: 'top',
                labels: {
                    // This more specific font property overrides the global property
                    fontColor: 'black',
                    fontSize: 20,
                    fontFamily: "Varela Round,Arial,sans-serif"

                },
            },

            animation: {
                animateScale: true,
                animateRotate: true
            }
        }
    });
    task.update()
}