using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldMaker._2d;

namespace WorldMaker
{
    public class LineGraph : IEnumerable<WorldPoint>
    {
        public int Count { get { return points.Count; } }
        public bool IsConnected { get; set; }

        private List<WorldPoint> points;

        private double minX;
        private double maxX;
        private double minY;
        private double maxY;

        public WorldRectangle Bounds
        {
            get
            {
                return new WorldRectangle(minX, minY, maxX-minX, maxY-minY);
            }
        }

        public LineGraph() : this(false) {}

        public LineGraph(bool isConnected)
        {
            points = new List<WorldPoint>();
            IsConnected = isConnected;
        }

        public void RecalculateBounds()
        {
            if (Count > 0)
            {
                minX = this[0].X;
                maxX = this[0].X;
                minY = this[0].Y;
                maxY = this[0].Y;

                foreach (WorldPoint point in this)
                {
                    stretchBounds(point.X, point.Y);
                }
            }
        }

        public void Add(double x, double y)
        {
            stretchBounds(x, y);
            points.Add(new WorldPoint(x, y));
        }

        public bool Remove(WorldPoint point)
        {
            return points.Remove(point);
        }

        //Returns the index of the closest point or line segment along with the distance
        public int Grab(double x, double y, double range, out double distance, out bool fromLine)
        {
            if (minX - range <= x && maxX + range >= x &&
                minY - range <= y && maxY + range >= y)
            {
                double closestPointDistance = range;

                int closestPointIndex = -1;

                WorldRectangle rect = new WorldRectangle(x - range, y - range, range * 2, range * 2);
                for (int i = 0; i < points.Count; i++)
                {
                    WorldPoint point = points[i];

                    if (rect.Contains(point))
                    {
                        double dx = x - point.X;
                        double dy = y - point.Y;
                        double pointDistance = Math.Sqrt(dx * dx + dy * dy);
                        if (pointDistance < closestPointDistance)
                        {
                            closestPointDistance = pointDistance;
                            closestPointIndex = i;
                        }
                    }
                }
                if (closestPointIndex != -1)
                {
                    distance = closestPointDistance;
                    fromLine = false;
                    return closestPointIndex;
                }
                else
                {
                    double closestLineDistance = range;
                    int closestLineIndex = -1;
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        WorldPoint point1 = points[i];
                        WorldPoint point2 = points[i + 1];

                        WorldPoint closest;
                        double lineDistance = FindDistanceToSegment(new WorldPoint(x, y), point1, point2, out closest);

                        if (lineDistance < closestLineDistance)
                        {
                            closestLineDistance = lineDistance;
                            closestLineIndex = i;
                        }

                    }
                    if (IsConnected && Count >= 3)
                    {
                        WorldPoint point1 = points[0];
                        WorldPoint point2 = points[Count-1];

                        WorldPoint closest;
                        double lineDistance = FindDistanceToSegment(new WorldPoint(x, y), point1, point2, out closest);

                        if (lineDistance < closestLineDistance)
                        {
                            closestLineDistance = lineDistance;
                            closestLineIndex = Count - 1;
                        }
                    }

                    if (closestLineIndex != -1)
                    {
                        distance = closestLineDistance;
                        fromLine = true;
                        return closestLineIndex;
                    }
                }
            }
            distance = 0;
            fromLine = false;
            return -1;
        }

        private void stretchBounds(double x, double y)
        {
            if (Count == 0)
            {
                minX = x;
                maxX = x;
                minY = y;
                maxY = y;
            }
            else
            {
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }
        }

        public WorldPoint this[int index]{
            get
            {
                return points[index];
            }
            set
            {
                stretchBounds(value.X, value.Y);
                points[index] = value;
            }
        }

        public void RemoveAt(int index)
        {
            points.RemoveAt(index);
        }

        public IEnumerator<WorldPoint> GetEnumerator()
        {
            foreach(WorldPoint point in points) yield return point;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        private double FindDistanceToSegment(WorldPoint pt, WorldPoint p1, WorldPoint p2, out WorldPoint closest)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            double t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new WorldPoint(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new WorldPoint(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new WorldPoint(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
