using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldMaker.loadsave
{
    class SaveUtil
    {
        public static int Version { get; } = 1;

        public static bool Save(World world, FileInfo file)
        {
            try
            {
                using (var fs = new FileStream(file.FullName, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new BinaryWriter(fs))
                    {
                        //Version
                        writer.Write(Version);
                        saveContent(world, writer);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during Save: {0}", ex);
                return false;
            }
        }

        private static void saveContent(World world, BinaryWriter writer)
        {
            //World Size
            writer.Write(world.Width);
            writer.Write(world.Height);
            writer.Write(world.Lines.Count);

            foreach(LineGraph graph in world.Lines) saveGraph(graph, writer);
            
        }

        private static void saveGraph(LineGraph graph, BinaryWriter writer)
        {
            writer.Write(graph.IsConnected);
            writer.Write(graph.Count);

            foreach (WorldPoint point in graph) savePoint(point, writer);
        }

        private static void savePoint(WorldPoint point, BinaryWriter writer)
        {
            writer.Write(point.X);
            writer.Write(point.Y);
        }
    }
}
