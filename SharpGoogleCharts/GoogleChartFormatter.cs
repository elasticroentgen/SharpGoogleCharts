using System.Collections.Generic;

namespace SharpGoogleCharts
{
    public class GoogleChartFormatter
    {
        public string Name { get; set; }
        public int ColumnIdx { get; set; }
        public Dictionary<string, object> Options { get; set; }

        public GoogleChartFormatter()
        {
            ColumnIdx = 0;
            Options = new Dictionary<string, object>();
        }
    }
}