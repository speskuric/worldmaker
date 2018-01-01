using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldMaker.loadsave
{
    class LoadUtil
    {
        public static int Version { get; } = 1;

        public static World Load(FileInfo file)
        {
            try
            {
                using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(fs))
                    {
                        //Version
                        int version = reader.ReadInt32();
                        if (version != Version) throw new Exception("Incompatible Versions " + version + " and " + Version);
                        return loadContent(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during Load: {0}", ex);
                return null;
            }
        }

        private static World loadContent(BinaryReader reader)
        {
            //World Size
            int worldWidth = reader.ReadInt32();
            int worldHeight = reader.ReadInt32();
            World world = new World(worldWidth, worldHeight);

            int lineCount = reader.ReadInt32();
            for(int i = 0; i < lineCount; i++) world.Lines.Add(loadGraph(reader));

            return world;
        }

        private static LineGraph loadGraph(BinaryReader reader)
        {
            bool isConnected = reader.ReadBoolean();
            int pointCount = reader.ReadInt32();

            LineGraph line = new LineGraph(isConnected);

            for (int i = 0; i < pointCount; i++) line.Add(loadPoint(reader, line));

            return line;    
        }

        private static WorldPoint loadPoint(BinaryReader reader, LineGraph parent)
        {
            double x = reader.ReadDouble();
            double y = reader.ReadDouble();
            return new WorldPoint(x, y, parent);
        }
    }
}
