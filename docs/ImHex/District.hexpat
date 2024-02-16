struct Vector3 {
    float x;
    float y;
    float z;
};

struct BinaryResourceHeader {
    u8 cooker_version;
    u8 endianness;
    u16 unk;
    u32 prototype_data_version;
    u32 class_hash;
};

struct CellConnectorMarkerPrototype {
    Vector3 Extents;
};

struct DotCornerMarkerPrototype {
    Vector3 Extents;
};

struct EntityMarkerPrototype {
    u64 EntityGuid;     // Prototype GUID
    u32 LastKnownEntityNameLength;
    char LastKnownEntityName[LastKnownEntityNameLength];
    u64 Modifier1Guid;  // Prototype GUID
    u64 Modifier2Guid;  // Prototype GUID
    u64 Modifier3Guid;  // Prototype GUID
    u32 EncounterSpawnPhase;
    u8 OverrideSnapToFloor;
    u8 OverrideSnapToFloorValue;
    u64 FilterGuid;     // Prototype GUID
    u32 LastKnownFilterNameLength;
    char LastKnownFilterName[LastKnownFilterNameLength];
};

struct RoadConnectionMarkerPrototype {
    Vector3 Extents;
};

struct ResourceMarkerPrototype {
    u32 ResourceLength;
    char Resource[ResourceLength];
};

struct UnrealPropMarkerPrototype {
    u32 UnrealClassNameLength;
    char UnrealClassName[UnrealClassNameLength];
    u32 UnrealQualifiedNameLength;
    char UnrealQualifiedName[UnrealQualifiedNameLength];
    u32 UnrealArchetypeNameLength;
    char UnrealArchetypeName[UnrealArchetypeNameLength];
};

struct MarkerPrototype {
    u32 ProtoNameHash;
    
    if (ProtoNameHash == 2901607432) {
        CellConnectorMarkerPrototype CellConnectorMarker;
    }
    else if (ProtoNameHash == 468664301) {
        DotCornerMarkerPrototype DotCornerMarker;
    }
    else if (ProtoNameHash == 3862899546) {
        EntityMarkerPrototype EntityMarker;
    }
    else if (ProtoNameHash == 576407411) {
        RoadConnectionMarkerPrototype RoadConnectionMarker;
    }
    else if (ProtoNameHash == 3468126021) {
        ResourceMarkerPrototype ResourceMarker;
    }
    else if (ProtoNameHash == 913217989) {
        UnrealPropMarkerPrototype UnrealPropMarker;
    }
    
    if (ProtoNameHash != 0) {
        Vector3 Position;
        Vector3 Rotation;
    }
};

struct MarkerSetPrototype {
    u32 NumMarkers;
    MarkerPrototype Markers[NumMarkers];
};

struct PathNodePrototype {
    u32 ProtoNameHash;
    Vector3 Position;
};

struct PathNodeSetPrototype {
    u32 ProtoNameHash;
    u16 Group;
    u32 NumPathNodes;
    PathNodePrototype PathNodes[NumPathNodes];
    u16 NumNodes;
};

struct PathCollectionPrototype {
    u32 NumPathNodeSets;
    PathNodeSetPrototype PathNodeSets[NumPathNodeSets];
};

struct DistrictPrototype {
    MarkerSetPrototype CellMarkerSet;
    MarkerSetPrototype MarkerSet;
    PathCollectionPrototype PathCollection;
};

struct DistrictFile {
    BinaryResourceHeader header;
    DistrictPrototype district;
};

DistrictFile file @ 0x00;