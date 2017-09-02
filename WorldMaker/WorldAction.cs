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
        public List<LineGraph> Lines { get { return lines; } }
        public List<WorldPoint> Points { get { return points; } }
        public double Dx { get { return dx; }}
        public double Dy { get { return dy; } }

        private ActionType type;
        private List<LineGraph> lines = new List<LineGraph>();
        private List<WorldPoint> points = new List<WorldPoint>();
        private double dx;
        private double dy;

        public WorldAction(ActionType type, LineGraph line)
        {
            this.type = type;
            lines.Add(line);
        }

        public WorldAction(ActionType type, List<LineGraph> lines, List<WorldPoint> points)
        {
            this.type = type;
            this.lines.AddRange(lines);
            this.points.AddRange(points);
        }

        public WorldAction(ActionType type, List<LineGraph> lines, List<WorldPoint> points, double dx, double dy)
        {
            this.type = type;
            this.lines.AddRange(lines);
            this.points.AddRange(points);
            this.dx = dx;
            this.dy = dy;
        }
    }
}
