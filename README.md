# SharpGoogleCharts

.Net Library to easily create JSON outputs for use with Google Charts

Demo at http://sharpgooglecharts.keil-connect.com/Home/ExampleMvc

## Quick Start

- Install using NuGet
- Build your plot in C#

 ```csharp
public IActionResult Index()
{
    ChartOptions opts = new ChartOptions();
    opts.ChartType = PlotChartType.Line;
    opts.Resolution = PlotResolution.None;
    opts.Showgrid = true;
    opts.XAxis.Datatype = AxisDataType.Numeric;
    opts.YAxis.Datatype = AxisDataType.Numeric;
    
    GoogleChart chart = new GoogleChart();
    chart.ChartDefinition = opts;
    chart.CurveData.Add("My Curve 1", new List
    {
        new PlotPoint(0,20),
        new PlotPoint(1,30),
        new PlotPoint(2,40),
        new PlotPoint(3,35),
        new PlotPoint(4,15),
        new PlotPoint(5,25)
    });
    chart.CurveData.Add("My Curve 2", new List
    {
        new PlotPoint(0,12),
        new PlotPoint(1,13),
        new PlotPoint(2,18),
        new PlotPoint(3,22),
        new PlotPoint(4,42),
        new PlotPoint(5,23)
    });
    return View(chart);
}
 ```

 - Bootstrap the Chart in your Razor View

 ```
<div class="chart" id="linePlotDemo"></div>
<script type="text/javascript">
    google.charts.load('current', { 'packages': ['corechart'] });
    google.charts.setOnLoadCallback(drawChart);
    function drawChart() {
        var demoLinePlot = new google.visualization.ChartWrapper(@Model.GetGoogleChartWrapper("linePlotDemo"));
        demoLinePlot.draw();
    }
</script>
 ```