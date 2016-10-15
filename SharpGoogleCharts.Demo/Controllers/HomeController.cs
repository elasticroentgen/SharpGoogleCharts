using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SharpGoogleCharts.Demo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("ExampleMvc");
        }
        public IActionResult ExampleMvc()
        {
            ChartOptions opts = new ChartOptions();
            opts.ChartType = PlotChartType.Line;
            opts.Resolution = PlotResolution.None;
            opts.Showgrid = true;
            opts.XAxis.Datatype = AxisDataType.Numeric;
            opts.YAxis.Datatype = AxisDataType.Numeric;

            GoogleChart chart = new GoogleChart();
            chart.ChartDefinition = opts;
            chart.CurveData.Add("My Curve 1", new List<PlotPoint>
            {
                new PlotPoint(0,20),
                new PlotPoint(1,30),
                new PlotPoint(2,40),
                new PlotPoint(3,35),
                new PlotPoint(4,15),
                new PlotPoint(5,25)
            });

            chart.CurveData.Add("My Curve 2", new List<PlotPoint>
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



        public IActionResult Error()
        {
            return View();
        }
    }
}
