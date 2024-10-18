using MHDataParser.Prototypes;
using MHDataParser.Prototypes.Markers;
using System.Text.Json.Serialization;

namespace MHDataParser.FileFormats
{
    public class CellPrototype
    {
        public ResourceHeader Header { get; }
        public Aabb BoundingBox { get; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Cell.Type Type { get; }
        public uint Walls { get; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Cell.Filler FillerEdges { get; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Cell.Type RoadConnections { get; }
        public string ClientMap { get; }
        public MarkerSetPrototype InitializeSet { get; }
        public MarkerSetPrototype MarkerSet { get; }
        public NaviPatchSourcePrototype NaviPatchSource { get; }
        public byte IsOffsetInMapFile { get; }
        public HeightMapPrototype HeightMap { get; }
        public PrototypeGuid[] HotspotPrototypes { get; }

        public CellPrototype(byte[] data, bool useLegacyFormat)
        {
            using (MemoryStream stream = new(data))
            using (BinaryReader reader = new(stream))
            {
                Header = new(reader);
                BoundingBox = new(reader);
                Type = (Cell.Type)reader.ReadUInt32();
                Walls = reader.ReadUInt32();
                FillerEdges = (Cell.Filler)reader.ReadUInt32();

                if (useLegacyFormat == false)
                    RoadConnections = (Cell.Type)reader.ReadUInt32();
                
                ClientMap = reader.ReadFixedString32();
                InitializeSet = new(reader, useLegacyFormat);
                MarkerSet = new(reader, useLegacyFormat);
                NaviPatchSource = new(reader);
                IsOffsetInMapFile = reader.ReadByte();
                HeightMap = new(reader);

                HotspotPrototypes = new PrototypeGuid[reader.ReadUInt32()];
                for (int i = 0; i < HotspotPrototypes.Length; i++)
                    HotspotPrototypes[i] = (PrototypeGuid)reader.ReadUInt64();
            }
        }
    }

    public class HeightMapPrototype
    {
        public Vector2 HeightMapSize { get; }
        public short[] HeightMapData { get; }
        public byte[] HotspotData { get; }

        public HeightMapPrototype(BinaryReader reader)
        {
            HeightMapSize = new(reader.ReadUInt32(), reader.ReadUInt32());

            HeightMapData = new short[reader.ReadUInt32()];
            for (int i = 0; i < HeightMapData.Length; i++)
                HeightMapData[i] = reader.ReadInt16();

            HotspotData = new byte[reader.ReadUInt32()];
            for (int i = 0; i < HotspotData.Length; i++)
                HotspotData[i] = reader.ReadByte();
        }
    }

    public class Cell
    {
        public enum Type
        {
            None = 0,
            N = 1,
            E = 2,
            S = 4,
            W = 8,
            NS = 5,
            EW = 10,
            NE = 3,
            NW = 9,
            ES = 6,
            SW = 12,
            ESW = 14,
            NSW = 13,
            NEW = 11,
            NES = 7,
            NESW = 15,
            NESWdNW = 159,
            NESWdNE = 207,
            NESWdSW = 63,
            NESWdSE = 111,
            NESWcN = 351,
            NESWcE = 303,
            NESWcS = 159,
            NESWcW = 207,
        }

        public enum WallGroup
        {
            N = 254,
            E = 251,
            S = 239,
            W = 191,
            NE = 250,
            ES = 235,
            SW = 175,
            NW = 190,
            NS = 238,
            EW = 187,
            NES = 234,
            ESW = 171,
            NSW = 174,
            NEW = 186,
            NESW = 170,
            WideNE = 248,
            WideES = 227,
            WideSW = 143,
            WideNW = 62,
            WideNES = 224,
            WideESW = 131,
            WideNSW = 14,
            WideNEW = 56,
            WideNESW = 0,
            WideNESWcN = 130,
            WideNESWcE = 10,
            WideNESWcS = 40,
            WideNESWcW = 160,
        }

        public enum Filler
        {
            N = 1,
            NE = 2,
            E = 4,
            SE = 8,
            S = 16,
            SW = 32,
            W = 64,
            NW = 128,
            C = 256,
        }
    }
}
