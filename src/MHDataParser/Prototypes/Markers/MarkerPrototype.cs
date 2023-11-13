﻿using System.Text.Json.Serialization;
using MHDataParser.FileFormats;

namespace MHDataParser.Prototypes.Markers
{
    /// <summary>
    /// This is a parent class for all other MarkerPrototypes.
    /// </summary>
    public class MarkerPrototype
    {
        [JsonPropertyOrder(1), JsonConverter(typeof(JsonStringEnumConverter))]
        public ResourcePrototypeHash ProtoNameHash { get; protected set; }    // DJB hash of the class name
        [JsonPropertyOrder(15)]
        public Vector3 Position { get; protected set; }
        [JsonPropertyOrder(16)]
        public Vector3 Rotation { get; protected set; }
    }
}
