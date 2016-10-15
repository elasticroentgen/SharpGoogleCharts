using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SharpGoogleCharts
{
    public class GoogleChart
    {
        public ChartOptions ChartDefinition { get; set; }
        public Dictionary<string, List<PlotPoint>> CurveData { get; set; }

        public GoogleChart()
        {
            CurveData = new Dictionary<string, List<PlotPoint>>();
        }



        private string ConvertDateTimeToJs(DateTime dt)
        {
            return $"Date({dt.Year},{dt.Month - 1},{dt.Day},{dt.Hour},{dt.Minute},0,0)";
        }


        public Dictionary<string, object> GetGoogleDataTable()
        {
            Dictionary<string, object> resultDict = new Dictionary<string, object>();


            //Create data rows
            List<List<object>> curvesArray = new List<List<object>>();

            foreach (List<PlotPoint> curvePoints in CurveData.Values)
            {
                foreach (PlotPoint pp in curvePoints)
                {
                    string xvalue;

                    if (pp.X is DateTime)
                        xvalue = ConvertDateTimeToJs((DateTime)pp.X);
                    else
                        xvalue = pp.X.ToString();


                    if (!curvesArray.Exists(x => x[0].ToString() == xvalue))
                    {
                        //X-Value don't exists yet so create it
                        curvesArray.Add(new List<object> { xvalue });
                    }
                    curvesArray.First(x => x[0].ToString() == xvalue).Add(pp);
                }
            }


            List<Dictionary<string, object>> dataRows = new List<Dictionary<string, object>>();

            int nbrDataCurves = 0;
            if (curvesArray.Count > 0)
                nbrDataCurves = curvesArray.Max(x => x.Count) - 1;

            //create rows
            foreach (List<object> row in curvesArray)
            {
                var rowValues = new List<Dictionary<string, object>>();
                foreach (object cellValue in row)
                {
                    PlotPoint cellPoint = cellValue as PlotPoint;
                    if (cellPoint != null)
                    {
                        rowValues.Add(new Dictionary<string, object>
                        {
                            {"v", cellPoint.Y},
                            {"f", cellPoint.PrettyY}
                        });
                    }
                    else
                    {
                        string prettyValue = cellValue.ToString();
                        rowValues.Add(new Dictionary<string, object>
                        {
                            {"v", cellValue},
                            {"f", prettyValue}
                        });
                    }
                }
                dataRows.Add(new Dictionary<string, object> { { "c", rowValues } });
            }
            resultDict["rows"] = dataRows;


            //create columns

            List<Dictionary<string, string>> cols = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    {"id", "xaxis"},
                    {"label", ChartDefinition.XAxis.Label},
                    {"pattern", ChartDefinition.XAxis.Dataformat},
                    {"type", ChartDefinition.XAxis.Datatype.ToGoogleCharts()}
                }
            };

            int i = 0;

            foreach (var c in CurveData)
            {
                //Dont add more columns than we have data
                if (i >= nbrDataCurves)
                    break;

                var col = new Dictionary<string, string>
                {
                    {"id", "yaxis_" + i++},
                    {"label", c.Key},
                    {"pattern", ""},
                    {"type", "number"}
                };
                cols.Add(col);
            }

            resultDict["cols"] = cols;
            return resultDict;
        }

        public Dictionary<string, object> GetGoogleOptions()
        {
            Dictionary<string, object> optionsDict = new Dictionary<string, object>
            {
                ["hAxis"] = new Dictionary<string, object>
                {
                    {"title", ChartDefinition.XAxis.Label}
                },
                ["vAxis"] = new Dictionary<string, object>
                {
                    {"title", ChartDefinition.YAxis.Label}
                }
            };

            if (!ChartDefinition.Showgrid)
            {
                ((Dictionary<string, object>)optionsDict["hAxis"])["gridlines"] = new Dictionary<string, object>
                {
                    {"color", "transparent"}
                };

                ((Dictionary<string, object>)optionsDict["vAxis"])["gridlines"] = new Dictionary<string, object>
                {
                    {"color", "transparent"}
                };
            }

            //Legend handling
            if (ChartDefinition.Showlegend)
            {
                if (ChartDefinition.ChartType == PlotChartType.Donut || ChartDefinition.ChartType == PlotChartType.Pie)
                {
                    optionsDict["legend"] = new Dictionary<string, object>
                    {
                        {"position", "right"}
                    };

                    optionsDict["chartArea"] = new Dictionary<string, object>
                    {
                        {"height", "100%"}
                    };
                }
                else
                {
                    optionsDict["legend"] = new Dictionary<string, object>
                    {
                        {"position", "top"},
                        {"maxLines", 3}
                    };
                }
            }
            else //hide legend
            {
                optionsDict["legend"] = new Dictionary<string, object>
                {
                    {"position", "none"}
                };
            }

            switch (ChartDefinition.ChartType)
            {
                case PlotChartType.BarStacked:
                    optionsDict["isStacked"] = "true";
                    optionsDict["bar"] = new Dictionary<string, string>
                    {
                        {"groupWidth", "75%"}
                    };
                    break;
                case PlotChartType.Donut:
                    optionsDict["pieHole"] = 0.4;
                    break;
                case PlotChartType.Table:
                    optionsDict["width"] = "100%";
                    optionsDict["sortAscending"] = false;
                    optionsDict["sortColumn"] = 1;
                    break;
                case PlotChartType.GeoMap:
                    string color = ChartDefinition.Curves[0].Color;
                    if (!string.IsNullOrWhiteSpace(color) && color != "auto")
                    {
                        optionsDict["colorAxis"] = new Dictionary<string, object>
                        {
                            {"colors", new List<string> { "#eeeeee", ChartDefinition.Curves[0].Color}}
                        };
                    }
                    break;
            }

            if (ChartDefinition.ChartType == PlotChartType.Pie || ChartDefinition.ChartType == PlotChartType.Donut)
            {
                var sliceOptions = new Dictionary<string, object>();

                for (int i = 0; i < ChartDefinition.Curves.Count; i++)
                {
                    var curve = ChartDefinition.Curves[i];
                    if (curve.Color == "auto" || string.IsNullOrWhiteSpace(curve.Color))
                        continue;

                    sliceOptions.Add(i.ToString(), new Dictionary<string, object>
                    {
                        { "color", curve.Color }
                    });
                }

                optionsDict["slices"] = sliceOptions;
            }


            Dictionary<int, object> series = new Dictionary<int, object>();
            //create curve options
            for (int i = 0; i < ChartDefinition.Curves.Count; i++)
            {
                var curve = ChartDefinition.Curves[i];
                Dictionary<string, object> curveOptions = new Dictionary<string, object>();

                switch (curve.Linemarker)
                {
                    case PlotLineMarker.Diamond:
                        curveOptions.Add("pointShape", "diamond");
                        curveOptions.Add("pointSize", 10);
                        break;
                    case PlotLineMarker.Dot:
                        curveOptions.Add("pointShape", "circle");
                        curveOptions.Add("pointSize", 10);
                        break;
                    case PlotLineMarker.Square:
                        curveOptions.Add("pointShape", "square");
                        curveOptions.Add("pointSize", 10);
                        break;
                    case PlotLineMarker.Triangle:
                        curveOptions.Add("pointShape", "triangle");
                        curveOptions.Add("pointSize", 10);
                        break;
                }

                if (!string.IsNullOrWhiteSpace(curve.Color) && curve.Color != "auto")
                {
                    curveOptions.Add("color", curve.Color);

                }


                series.Add(i, curveOptions);
            }

            optionsDict["series"] = series;
            return optionsDict;
        }

        public string GetGoogleFunctionName()
        {
            switch (ChartDefinition.ChartType)
            {
                case PlotChartType.Bar:
                    return "ColumnChart";
                case PlotChartType.GeoMap:
                    return "GeoChart";
                case PlotChartType.BarStacked:
                    return "ColumnChart";
                case PlotChartType.Donut:
                    return "PieChart";
                case PlotChartType.Line:
                    return "LineChart";
                case PlotChartType.Pie:
                    return "PieChart";
                case PlotChartType.Table:
                    return "Table";
            }
            return "";
        }

        public List<GoogleChartFormatter> GetGoogleFormatters()
        {
            List<GoogleChartFormatter> formatters = new List<GoogleChartFormatter>();

            switch (ChartDefinition.Resolution)
            {
                case PlotResolution.Day:
                case PlotResolution.Week:
                case PlotResolution.Month:
                    GoogleChartFormatter ccfa = new GoogleChartFormatter { Name = "DateFormat", ColumnIdx = 0 };
                    ccfa.Options.Add("pattern", "MMM dd, yyyy");
                    formatters.Add(ccfa);
                    break;

                case PlotResolution.Hour:
                    GoogleChartFormatter ccfb = new GoogleChartFormatter { Name = "DateFormat", ColumnIdx = 0 };
                    ccfb.Options.Add("pattern", "MMM dd, HH:mm");
                    formatters.Add(ccfb);
                    break;
            }

            if (ChartDefinition.ChartType == PlotChartType.Table)
            {
                int i = 0;
                foreach (var curve in ChartDefinition.Curves)
                {
                    i++;
                    if (!curve.DrawTablebars) continue;

                    GoogleChartFormatter ccf = new GoogleChartFormatter { Name = "BarFormat", ColumnIdx = i };
                    ccf.Options.Add("width", 120);
                    ccf.Options.Add("min", 0);

                    if (!string.IsNullOrWhiteSpace(curve.Color))
                        ccf.Options.Add("colorPositive", curve.Color);

                    formatters.Add(ccf);

                }
            }
            return formatters;
        }

        public string GetGoogleFormattersJson()
        {
            return JsonConvert.SerializeObject(GetGoogleFormatters());
        }

        public string GetGoogleOptionsJson()
        {
            return JsonConvert.SerializeObject(GetGoogleOptions());
        }

        public string GetGoogleDataTableJson()
        {
            return JsonConvert.SerializeObject(GetGoogleDataTable());
        }
    }
}