using System.Text.Json.Serialization;
using MHDataParser.FileFormats;

namespace MHDataParser.Prototypes
{
    public enum NaviContentTag
    {
        None = 0,
        OpaqueWall = 1,
        TransparentWall = 2,
        Blocking = 3,
        NoFly = 4,
        Walkable = 5,
        Obstacle = 6
    }

    [Flags]
    public enum NaviContentFlag
    {
        AddWalk = 1 << 0,
        RemoveWalk = 1 << 1,
        AddFly = 1 << 2,
        RemoveFly = 1 << 3,
        AddPower = 1 << 4,
        RemovePower = 1 << 5,
        AddSight = 1 << 6,
        RemoveSight = 1 << 7
    }

    public class NaviPatchPrototype
    {
        public Vector3[] Points { get; }
        public NaviPatchEdgePrototype[] Edges { get; }

        public NaviPatchPrototype(BinaryReader reader)
        {
            Points = new Vector3[reader.ReadUInt32()];
            for (int i = 0; i < Points.Length; i++)
                Points[i] = new(reader);

            Edges = new NaviPatchEdgePrototype[reader.ReadUInt32()];
            for (int i = 0; i < Edges.Length; i++)
                Edges[i] = new(reader);
        }
    }

    public class NaviPatchEdgePrototype
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResourcePrototypeHash ProtoNameHash { get; }
        public uint Index0 { get; }
        public uint Index1 { get; }
        public NaviContentFlag[] Flags0 { get; }
        public NaviContentFlag[] Flags1 { get; }

        public NaviPatchEdgePrototype(BinaryReader reader)
        {
            ProtoNameHash = (ResourcePrototypeHash)reader.ReadUInt32();
            Index0 = reader.ReadUInt32();
            Index1 = reader.ReadUInt32();

            Flags0 = new NaviContentFlag[reader.ReadUInt32()];
            for (int i = 0; i < Flags0.Length; i++)
                Flags0[i] = (NaviContentFlag)reader.ReadByte();

            Flags1 = new NaviContentFlag[reader.ReadUInt32()];
            for (int i = 0; i < Flags1.Length; i++)
                Flags1[i] = (NaviContentFlag)reader.ReadByte();
        }
    }
}
