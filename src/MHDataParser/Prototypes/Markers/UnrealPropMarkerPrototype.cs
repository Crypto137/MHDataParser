﻿using System.Text.Json.Serialization;
using MHDataParser.FileFormats;

namespace MHDataParser.Prototypes.Markers
{
    public class UnrealPropMarkerPrototype : MarkerPrototype
    {
        [JsonPropertyOrder(2)]
        public string UnrealClassName { get; }
        [JsonPropertyOrder(3)]
        public string UnrealQualifiedName { get; }
        [JsonPropertyOrder(4)]
        public string UnrealArchetypeName { get; }

        public UnrealPropMarkerPrototype(BinaryReader reader)
        {
            ProtoNameHash = ResourcePrototypeHash.UnrealPropMarkerPrototype;

            UnrealClassName = reader.ReadFixedString32();
            UnrealQualifiedName = reader.ReadFixedString32();
            UnrealArchetypeName = reader.ReadFixedString32();

            Position = new(reader);
            Rotation = new(reader);
        }
    }
}
