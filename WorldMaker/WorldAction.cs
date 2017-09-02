using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldMaker.ui.enums;

namespace WorldMaker
{
    public class WorldAction
    {
        public ActionType Type { get { return type; } }
        public List<WorldPoint> Points { get { return points; } }
        public List<int> PointIndices { get { return pointIndices; } }
        public LineGraph Line { get { return line; } }
        public double Dx { get { return dx; }}
        public double Dy { get { return dy; } }

        private ActionType type;
        private List<WorldPoint> points = new List<WorldPoint>();
        private List<int> pointIndices = new List<int>();
        private LineGraph line;
        private double dx;
        private double dy;

        public WorldAction(ActionType type, List<WorldPoint> points, List<int> pointIndices)
        {
            this.type = type;
            this.points.AddRange(points);
            this.pointIndices.AddRange(pointIndices);
        }

        public WorldAction(ActionType type, List<WorldPoint> points, double dx, double dy)
        {
            this.type = type;
            this.points.AddRange(points);
            this.dx = dx;
            this.dy = dy;
        }

        public WorldAction(ActionType type, LineGraph line)
        {
            this.type = type;
            this.line = line;
        }
    }
}
