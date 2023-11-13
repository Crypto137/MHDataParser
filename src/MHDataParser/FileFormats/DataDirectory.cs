namespace MHDataParser.FileFormats
{
    /// <summary>
    /// An abstract class for data directory entries of all types.
    /// </summary>
    public abstract class DataRecord { }

    public class DataDirectory
    {
        public CalligraphyHeader Header { get; }
        public DataRecord[] Records { get; }

        public DataDirectory(byte[] data)
        {
            using (MemoryStream stream = new(data))
            using (BinaryReader reader = new(stream))
            {
                Header = new(reader);
                Records = new DataRecord[reader.ReadUInt32()];

                switch (Header.Magic)
                {
                    case "CDR":     // Curve
                        for (int i = 0; i < Records.Length; i++)
                            Records[i] = new CurveRecord(reader);
                        break;
                    case "TDR":     // Asset Type
                        for (int i = 0; i < Records.Length; i++)
                            Records[i] = new AssetTypeRecord(reader);
                        break;
                    case "BDR":     // Blueprint
                        for (int i = 0; i < Records.Length; i++)
                            Records[i] = new BlueprintRecord(reader);
                        break;
                    case "PDR":     // Prototype
                        for (int i = 0; i < Records.Length; i++)
                            Records[i] = new PrototypeRecord(reader);
                        break;
                    case "RDR":     // Replacement
                        for (int i = 0; i < Records.Length; i++)
                            Records[i] = new ReplacementRecord(reader);
                        break;
                }
            }
        }
    }

    public class CurveRecord : DataRecord     // CDR
    {
        public CurveId Id { get; }
        public CurveGuid Guid { get; }
        public byte Flags { get; }
        public string FilePath { get; }

        public CurveRecord(BinaryReader reader)
        {
            Id = (CurveId)reader.ReadUInt64();
            Guid = (CurveGuid)reader.ReadUInt64();
            Flags = reader.ReadByte();
            FilePath = reader.ReadFixedString16().Replace('\\', '/');

            GameDatabase.CurveRefManager.AddDataRef(Id, FilePath);
        }
    }

    public class AssetTypeRecord : DataRecord     // TDR
    {
        public AssetTypeId Id { get; }
        public AssetTypeGuid Guid { get; }
        public byte Flags { get; }
        public string FilePath { get; }

        public AssetTypeRecord(BinaryReader reader)
        {
            Id = (AssetTypeId)reader.ReadUInt64();
            Guid = (AssetTypeGuid)reader.ReadUInt64();
            Flags = reader.ReadByte();
            FilePath = reader.ReadFixedString16().Replace('\\', '/');

            GameDatabase.AssetTypeRefManager.AddDataRef(Id, FilePath);
        }
    }

    public class BlueprintRecord : DataRecord // BDR
    {
        public BlueprintId Id { get; }
        public BlueprintGuid Guid { get; }
        public byte Flags { get; }
        public string FilePath { get; }

        public BlueprintRecord(BinaryReader reader)
        {
            Id = (BlueprintId)reader.ReadUInt64();
            Guid = (BlueprintGuid)reader.ReadUInt64();
            Flags = reader.ReadByte();
            FilePath = reader.ReadFixedString16().Replace('\\', '/');

            GameDatabase.BlueprintRefManager.AddDataRef(Id, FilePath);
        }
    }

    public class PrototypeRecord : DataRecord // PDR
    {
        public PrototypeId Id { get; }
        public PrototypeGuid Guid { get; }
        public PrototypeId BlueprintId { get; }
        public byte Flags { get; }
        public string FilePath { get; }

        public PrototypeRecord(BinaryReader reader)
        {
            Id = (PrototypeId)reader.ReadUInt64();
            Guid = (PrototypeGuid)reader.ReadUInt64();
            BlueprintId = (PrototypeId)reader.ReadUInt64();
            Flags = reader.ReadByte();
            FilePath = reader.ReadFixedString16().Replace('\\', '/');

            GameDatabase.PrototypeRefManager.AddDataRef(Id, FilePath);
        }
    }

    public class ReplacementRecord : DataRecord   // RDR
    {
        public ulong OldGuid { get; }
        public ulong NewGuid { get; }
        public string Name { get; }

        public ReplacementRecord(BinaryReader reader)
        {
            OldGuid = reader.ReadUInt64();
            NewGuid = reader.ReadUInt64();
            Name = reader.ReadFixedString16();
        }
    }
}
