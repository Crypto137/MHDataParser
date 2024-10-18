using System.Text.Json.Serialization;
using MHDataParser.FileFormats;

namespace MHDataParser.Prototypes.Markers
{
    public class EntityMarkerPrototype : MarkerPrototype
    {
        [JsonPropertyOrder(2)]
        public PrototypeGuid EntityGuid { get; }
        [JsonPropertyOrder(3)]
        public string LastKnownEntityName { get; }
        [JsonPropertyOrder(4)]
        public PrototypeGuid Modifier1Guid { get; }
        [JsonPropertyOrder(6)]
        public PrototypeGuid Modifier2Guid { get; }
        [JsonPropertyOrder(8)]
        public PrototypeGuid Modifier3Guid { get; }
        [JsonPropertyOrder(10)]
        public uint EncounterSpawnPhase { get; }
        [JsonPropertyOrder(11)]
        public byte OverrideSnapToFloor { get; }
        [JsonPropertyOrder(12)]
        public byte OverrideSnapToFloorValue { get; }
        [JsonPropertyOrder(13)]
        public PrototypeGuid FilterGuid { get; }
        [JsonPropertyOrder(14)]
        public string LastKnownFilterName { get; }

        public EntityMarkerPrototype(BinaryReader reader, bool useLegacyFormat)
        {
            ProtoNameHash = ResourcePrototypeHash.EntityMarkerPrototype;

            EntityGuid = (PrototypeGuid)reader.ReadUInt64();
            LastKnownEntityName = reader.ReadFixedString32();
            Modifier1Guid = (PrototypeGuid)reader.ReadUInt64();
            // eFlagDontCook Modifier1Text = reader.ReadFixedString32();
            Modifier2Guid = (PrototypeGuid)reader.ReadUInt64();
            // eFlagDontCook Modifier2Text = reader.ReadFixedString32();
            Modifier3Guid = (PrototypeGuid)reader.ReadUInt64();
            // eFlagDontCook Modifier3Text = reader.ReadFixedString32();
            EncounterSpawnPhase = reader.ReadUInt32();
            OverrideSnapToFloor = reader.ReadByte();
            OverrideSnapToFloorValue = reader.ReadByte();

            if (useLegacyFormat == false)
            {
                FilterGuid = (PrototypeGuid)reader.ReadUInt64();
                LastKnownFilterName = reader.ReadFixedString32();
            }

            Position = new(reader);
            Rotation = new(reader);
        }
    }
}
