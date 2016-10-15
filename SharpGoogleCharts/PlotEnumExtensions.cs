namespace SharpGoogleCharts
{
    public static class PlotEnumExtensions
    {
        public static string ToGoogleCharts(this PlotChartType pct)
        {
            switch (pct)
            {
                case PlotChartType.Pie:
                    return "pie";
                case PlotChartType.Donut:
                    return "donut";
                case PlotChartType.Line:
                    return "line";
                case PlotChartType.Bar:
                    return "bar";
                case PlotChartType.BarStacked:
                    return "stacked";
                case PlotChartType.GeoMap:
                    return "geomap";
                case PlotChartType.Table:
                    return "table";
                default:
                    return string.Empty;
            }
        }



        public static string ToGoogleCharts(this AxisDataType adt)
        {
            switch (adt)
            {
                case AxisDataType.Numeric:
                    return "number";
                case AxisDataType.Text:
                    return "string";
                case AxisDataType.DateTime:
                    return "datetime";
                default:
                    return string.Empty;
            }
        }


        public static string ToGoogleCharts(this PlotLineMarker plm)
        {
            switch (plm)
            {
                case PlotLineMarker.None:
                    return "none";
                case PlotLineMarker.Dot:
                    return "circle";
                case PlotLineMarker.Triangle:
                    return "triangle";
                case PlotLineMarker.Square:
                    return "square";
                case PlotLineMarker.Diamond:
                    return "diamond";
                default:
                    return string.Empty;
            }
        }
    }
}