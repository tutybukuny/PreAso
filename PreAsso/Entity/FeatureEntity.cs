using System.Collections.Generic;

namespace PreAsso.Entity
{
    /**
     * author: thienthn
     * created date: 27/03/2018
     */
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

        /// <summary>
        /// calculating feature's appearance ratio over total data
        /// </summary>
        /// <param name="total">total rows of data</param>
        public void PercentCalculating(int total)
        {
            Percent = Count / (double) total * 100;
        }
    }
}