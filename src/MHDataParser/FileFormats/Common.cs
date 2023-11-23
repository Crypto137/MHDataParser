using System.Text;

namespace MHDataParser.FileFormats
{
    public enum CalligraphyBaseType : byte
    {
        Asset = 0x41,       // A (Id reference to an asset)
        Boolean = 0x42,     // B (Stored as a UInt64)
        Curve = 0x43,       // C (Id reference to a curve)
        Double = 0x44,      // D (For all floating point values)
        Long = 0x4c,        // L (For all integer values)
        Prototype = 0x50,   // P (Id reference to another prototype)
        RHStruct = 0x52,    // R (Embedded prototype without an id, the name is mentioned in EntitySelectorActionPrototype::Validate)
        String = 0x53,      // S (Id reference to a localized string)
        Type = 0x54         // T (Id reference to an AssetType)
    }

    public enum CalligraphyStructureType : byte
    {
        Simple = 0x53,      // Simple
        List = 0x4c         // List (only for assets, prototypes, rhstructs, and types)
    }

    // Bit field enums

    [Flags]
    public enum CurveRecordFlags : byte
    {
        None        = 0
        // Although curve records do have a field for flags, it's 0 for all records
    }

    [Flags]
    public enum AssetTypeRecordFlags : byte
    {
        None        = 0,
        Protected   = 1 << 0    // AssetDirectory::AssetTypeIsProtected()
    }

    [Flags]
    public enum AssetValueFlags : byte
    {
        None        = 0,
        Protected   = 1 << 0    // AssetType::AssetIsProtected()
    }

    [Flags]
    public enum BlueprintRecordFlags : byte
    {
        None        = 0,
        Protected   = 1 << 0    // DataDirectory::BlueprintIsProtected()
    }

    [Flags]
    public enum PrototypeRecordFlags : byte
    {
        None        = 0,
        Abstract    = 1 << 0,   // DataDirectory::PrototypeIsAbstract()
        Protected   = 1 << 1    // DataDirectory::PrototypeIsProtected()
    }

    /// <summary>
    /// Precalculated DJB2 hashes for known resource prototypes.
    /// </summary>
    public enum ResourcePrototypeHash : uint
    {
        None = 0,
        CellConnectorMarkerPrototype = 2901607432,
        DotCornerMarkerPrototype = 468664301,
        EntityMarkerPrototype = 3862899546,
        RoadConnectionMarkerPrototype = 576407411,
        ResourceMarkerPrototype = 3468126021,
        UnrealPropMarkerPrototype = 913217989,
        PathNodeSetPrototype = 1572935802,
        PathNodePrototype = 908860270,
        PropSetTypeListPrototype = 1819714054,
        PropSetTypeEntryPrototype = 2348267420,
        ProceduralPropGroupPrototype = 2480167290,
        StretchedPanelPrototype = 805156721,
        AnchoredPanelPrototype = 1255662575
    }

    public readonly struct CalligraphyHeader
    {
        public string Magic { get; }    // File signature
        public byte Version { get; }    // 10 for versions 1.9-1.17, 11 for 1.18+

        public CalligraphyHeader(BinaryReader reader)
        {
            Magic = Encoding.UTF8.GetString(reader.ReadBytes(3));
            Version = reader.ReadByte();
        }
    }

    public readonly struct ResourceHeader
    {
        public uint Signature { get; }
        public uint Version { get; }
        public uint ClassId { get; }

        public ResourceHeader(BinaryReader reader)
        {
            Signature = reader.ReadUInt32();
            Version = reader.ReadUInt32();
            ClassId = reader.ReadUInt32();
        }
    }

    public readonly struct Vector2
    {
        public float X { get; }
        public float Y { get; }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
        }
    }

    public readonly struct Vector3
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Vector3(BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }
    }

    public readonly struct Aabb
    {
        public Vector3 Min { get; }
        public Vector3 Max { get; }

        public Aabb(BinaryReader reader)
        {
            Max = new(reader);
            Min = new(reader);
        }
    }
}
