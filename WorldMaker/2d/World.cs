using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldMaker
{
    public class World
    {
        public List<LineGraph> Lines { get; set; }

        public int Width { get; }
        public int Height { get; }

        public World(int width, int height)
        {
            Width = width;
            Height = height;
            Lines = new List<LineGraph>();
        }
    }
}
