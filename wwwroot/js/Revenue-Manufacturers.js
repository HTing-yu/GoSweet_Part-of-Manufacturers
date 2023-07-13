window.onload = function () {
  
  //#region 宣告區
  //圖表參數
    const ctx = $('#chart');
    var ChartType = 'bar';
    var XaxisNameArray = [];
    var ChartLegend = '總額';
    var QuentityofXaxis = [];
    var color = ['rgba(195, 166, 160, 0.3)', 'rgba(161, 92, 56, 0.3)', 'rgba(255, 99, 132, 0.3)', 'rgba(255, 159, 64, 0.3)', 'rgba(255, 205, 86, 0.3)', 'rgba(75, 192, 192, 0.3)', 'rgba(54, 162, 235, 0.3)', 'rgba(153, 102, 255, 0.3)', 'rgba(201, 203, 207, 0.3)'];
    var datagroup = [{ label: ChartLegend, data: QuentityofXaxis, backgroundColor: color }];
    var ReportChart = drewChart();
  //日期參數
    var Start = "";
    var End = "";
    var StartDate = new Date (1997, 01, 14);
    var EndDate = new Date();

  //資料
    var DataList;
  //#endregion

  //#region 設定搜索終日為今天
    var NowDate = new Date()
    let Nowday = NowDate.getDate()
    let NowMonth = NowDate.getMonth() + 1
    let NowYear = NowDate.getFullYear()
    if (Nowday < 10) { Nowday = '0' + Nowday }
    if (NowMonth < 10) { NowMonth = '0' + NowMonth }
    NowDatestring = NowYear + "-" + NowMonth + "-" + Nowday
    //$("input[name='EndDate']").attr('value', NowDatestring)
    $("input[name='EndDate']").attr('max', NowDatestring)
    $("input[name='StartDate']").attr('max', NowDatestring)

    NowDate.setMonth(NowDate.getMonth() - 3)
    let LastThreeMonth = NowDate.getMonth() + 1
    if (LastThreeMonth < 10) { LastThreeMonth = "0" + LastThreeMonth }
    LastThreeMonthDate = NowYear + "-" + LastThreeMonth + "-" + Nowday
    //$("input[name='StartDate']").attr('value', LastThreeMonthDate)
    //$("input[name='StartDate']").attr('min', LastThreeMonthDate)
    $("input[name='EndDate']").attr('min', LastThreeMonthDate)
  //#endregion 注意日期格式
  
  //#region 搜索日期
  $("form button").on("click", function(){
        Start = $("input[name='StartDate']").val()
        StartDate = new Date(Start)
        End = $("input[name='EndDate']").val()
        EndDate = new Date(End)
        if (StartDate > EndDate) {
            alert('起始日期不可以大於結束日期')
            this.type = "button"
        } else {
            this.type = "submit"
            alert(`開始日期${Start}結束日期${End}`)
            console.log(StartDate)
            console.log(EndDate)
      }
        return StartDate, EndDate
   })
  //#endregion 回傳字串陣列(目前單一，想辦法傳兩個參數)

    async function redrew() {
        var Xaxis = $("[name='X-axis']:checked").val();
        var Yaxis = $("[name='Y-axis']:checked").val();
        await GetData("orderDate")
        drewChart();
    }
    redrew()
   




  //#region X軸圖表選擇按鈕
  $("[name='X-axis']").on('click',async function(){
        var Xaxis = $("[name='X-axis']:checked").val();
        var Yaxis = $("[name='Y-axis']:checked").val();
        
      await GetData(Xaxis, Yaxis)
      await alert(XaxisNameArray);
      drewChart();
  })
  //#endregion 回傳字串
  
  //#region Y軸圖表選擇按鈕
    $("[name='Y-axis']").on('click',async function () {
        var Xaxis = $("[name='X-axis']:checked").val();
        var Yaxis = $("[name='Y-axis']:checked").val();

        await GetData(Xaxis, Yaxis)
        await alert(QuentityofXaxis);
        drewChart();
  })
  //#endregion 回傳字串
  
  //#region 圖型選擇按鈕
  $("[name='Chart_type']").on('click', function(){
    ChartType = $("[name='Chart_type']:checked").val();
    alert(ChartType);
    
    // 圖表更新
    drewChart();
  })
  //#endregion 
  
  //#region 圖表重製方法
    function drewChart() {
        datagroup = [{ label: ChartLegend, data: QuentityofXaxis, backgroundColor: color }];
        if (ReportChart instanceof Chart){
            ReportChart.destroy()
        }
    var ChartSet={
      type:ChartType,
      data:{
        labels:XaxisNameArray,
        datasets:datagroup
      },
      options:{
        responsive: true,
        scales:{
          y:{
            beginAtZero: true
          }
        }
      }
    }
    ReportChart = new Chart(ctx, ChartSet)
    return ReportChart
  }
  //#endregion 回傳圖表但不知道要做啥
  
  //圖表大小重新繪製-不是必須
  window.addEventListener('resize', function(){
    ReportChart.resize();
  });




    //#region 從後端拿資料的方法
    async function GetData(Target, sum) {
        var DatafromJson = await $.ajax({
            url: "/Manufacturers/JsonData",
            success: function (item) {
                //全部
                $.each(item, function (Index, Data) {
                    $("#home tbody").append(
                        `<tr>
                    <th scope="row">${Index + 1}</th>
                    <td>${Data.orderDate}</td>
                    <td>${Data.name}</td>
                    <td>${Data.id}</td>
                    <td>${Data.categories}</td>
                    <td>${Data.quentity}</td>
                    <td>${Data.price}</td>
                    <td>${Data.total}</td>
                </tr>`
                    )
                })
                //收入
                $.each(item.filter(content => content.total >= 0), function (Index, Data) {
                    $("#profile tbody").append(
                        `<tr>
                    <th scope="row">${Index + 1}</th>
                    <td>${Data.orderDate}</td>
                    <td>${Data.name}</td>
                    <td>${Data.id}</td>
                    <td>${Data.categories}</td>
                    <td>${Data.quentity}</td>
                    <td>${Data.price}</td>
                    <td>${Data.total}</td>
                </tr>`
                    )
                })
                //支出
                $.each(item.filter(content => content.total < 0), function (Index, Data) {
                    $("#contact tbody").append(
                        `<tr>
                    <th scope="row">${Index + 1}</th>
                    <td>${Data.orderDate}</td>
                    <td>${Data.name}</td>
                    <td>${Data.id}</td>
                    <td>${Data.categories}</td>
                    <td>${Data.quentity}</td>
                    <td>${Data.price}</td>
                    <td>${Data.total}</td>
                </tr>`
                    )
                })
                groupbysomething(item, Target, sum)
            }
        })
    }
    //#endregion

    //#region 歸類及排序方法
    async function groupbysomething(Json, Target, sum) {
        var something = await Object.entries(Json.reduce(function (result, current) {
            result[current[Target]] = result[current[Target]] || [];
            result[current[Target]].push(current);
            return result;
        }, {})).map(([key, value]) => ({ name: key, children: value }));

        switch (sum) {
            case "price": {
                something = something.map(function (element) {
                    var totals = 0;
                    element.children.forEach(elements => {
                        totals = totals + elements.price;
                    });
                    return [element.name, totals]
                }).sort();
                break;
            }
            case "quentity": {
                something = something.map(function (element) {
                    var totals = 0;
                    element.children.forEach(elements => {
                        totals = totals + elements.quentity;
                    });
                    return [element.name, totals]
                }).sort();
                break;
            }
            case "total": {
                something = something.map(function (element) {
                    var totals = 0;
                    element.children.forEach(elements => {
                        totals = totals + elements.total;
                    });
                    return [element.name, totals]
                }).sort();
                break;
            }
            default: {
                something = something.map(element => [element.name, element.children.length]).sort();
            }

        }

        console.log(something)
        console.log(something.map(elements => elements[0]))
        XaxisNameArray = something.map(elements => elements[0])
        console.log(something.map(elements => elements[1]))
        QuentityofXaxis = something.map(elements => elements[1])
        return something
    }
//#endregion 測試
}


