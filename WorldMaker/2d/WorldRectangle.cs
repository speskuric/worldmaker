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
                width += (x - value);
                x = value;
            }
        }

        public double Top
        {
            get { return y; }
            set
            {
                height += (y - value);
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
            return Contains(point.X, point.Y);
        }

        public bool Contains(double X, double Y)
        {
            return Left <= X && X <= Right &&
                Top <= Y && Y <= Bottom;
        }

        public static void TrimLineSegment(WorldPoint p1, WorldPoint p2, WorldRectangle viewBounds)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            //Trim off Left
            if (p1.X < viewBounds.Left && p2.X > viewBounds.Left)
            {
                double trimPercent = (viewBounds.Left - p1.X) / dx;
                p1.X = viewBounds.Left;
                if (dy != 0) p1.Y = p1.Y + dy * trimPercent;
            }
            else if (p1.X > viewBounds.Left && p2.X < viewBounds.Left)
            {
                double trimPercent = (viewBounds.Left - p2.X) / dx;
                p2.X = viewBounds.Left;
                if (dy != 0) p2.Y = p2.Y + dy * trimPercent;
            }

            //TRIM THE REST
            // AND MAKE SURE THE RECTANGLES ARE ALSO TRIMMED CORRECTLY
        }
    }
}
