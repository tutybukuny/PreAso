using System.Collections.Generic;

namespace PreAsso.Entity
{
    public class FeatureEntity
    {
        public FeatureEntity()
        {
            ValueList = new List<string>();
        }

        public string Name { get; set; }
        public int Count { get; set; }
        public double Percent { get; set; }
        public List<string> ValueList { get; set; }

        public void PercentCalculating(int total)
        {
            Percent = Count / (double) total * 100;
        }
    }
}