namespace SharpGoogleCharts
{
    public class PlotPoint
    {
        public PlotPoint(object v1, double v2)
        {
            X = v1;
            Y = v2;
        }

        public object X { get; set; }
        public double Y { get; set; }
        public string PrettyY { get; set; }

        public override string ToString()
        {
            return $"{nameof(PlotPoint)} | X:{X} => Y:{Y} [{PrettyY}]";
        }
    }
}