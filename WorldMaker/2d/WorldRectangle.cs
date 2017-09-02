using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldMaker._2d
{
    public class WorldRectangle
    {
        public double Left
        {
            get { return x; }
            set
            {
                width += (value - x);
                x = value;
            }
        }

        public double Top
        {
            get { return y; }
            set
            {
                height += (value - y);
                y = value;
            }
        }

        public double Right
        {
            get { return x + width; }
            set { width = (value - x); }
        }

        public double Bottom
        {
            get { return y + height; }
            set { height = (value - y); }
        }

        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        private double x;
        private double y;
        private double width;
        private double height;

        public WorldRectangle(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool Contains(WorldPoint point)
        {
            return Left <= point.X && point.X <= Right &&
                Top <= point.Y && point.Y <= Bottom;
        }
    }
}
