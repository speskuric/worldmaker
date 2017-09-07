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

        public WorldPoint() : this(double.NaN, double.NaN) { }

        public WorldPoint(double x, double y) : this(x, y, null) { }

        public WorldPoint(double x, double y, LineGraph parent)
        {
            X = x;
            Y = y;
            Parent = parent;
        }

        public static WorldPoint operator -(WorldPoint v, WorldPoint w)
        {
            return new WorldPoint(v.X - w.X, v.Y - w.Y);
        }

        public static WorldPoint operator +(WorldPoint v, WorldPoint w)
        {
            return new WorldPoint(v.X + w.X, v.Y + w.Y);
        }

        public static double operator *(WorldPoint v, WorldPoint w)
        {
            return v.X * w.X + v.Y * w.Y;
        }

        public static WorldPoint operator *(WorldPoint v, double mult)
        {
            return new WorldPoint(v.X * mult, v.Y * mult);
        }

        public static WorldPoint operator *(double mult, WorldPoint v)
        {
            return new WorldPoint(v.X * mult, v.Y * mult);
        }

        public double Cross(WorldPoint v)
        {
            return X * v.Y - Y * v.X;
        }
    }
}
