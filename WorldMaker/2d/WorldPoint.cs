using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldMaker
{
    public class WorldPoint
    {
        public LineGraph Parent { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public int getIndex()
        {
            return Parent.IndexOf(this);
        }

        public WorldPoint(double x, double y) : this(x, y, null) { }

        public WorldPoint(double x, double y, LineGraph parent)
        {
            X = x;
            Y = y;
            Parent = parent;
        }
    }
}
