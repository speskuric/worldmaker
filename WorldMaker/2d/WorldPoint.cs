using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldMaker
{
    public class WorldPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public WorldPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
