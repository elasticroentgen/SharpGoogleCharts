namespace GoogleSharpPlots
{
    public class PlotPoint
    {
        public object X { get; set; }
        public double Y { get; set; }
        public string PrettyY { get; set; }

        public override string ToString()
        {
            return $"{nameof(PlotPoint)} | X:{X} => Y:{Y} [{PrettyY}]";
        }
    }
}
