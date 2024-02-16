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

struct NaviPatchEdgePrototype {
    u32 ProtoNameHash;
    u32 Index0;
    u32 Index1;
    u32 NumFlags0;
    u8 Flags0[NumFlags0];
    u32 NumFlags1;
    u8 Flags1[NumFlags1];
};

struct NaviPatchPrototype {
    u32 NumPoints;
    Vector3 Points[NumPoints];
    u32 NumEdges;
    NaviPatchEdgePrototype Edges[NumEdges];
};

struct NaviPatchSourcePrototype {
    u32 NaviPatchCrc;
    NaviPatchPrototype NaviPatch;
    NaviPatchPrototype PropPatch;
    float PlayableArea;
    float SpawnableArea;
};

struct ProceduralPropGroupPrototype {
    u32 ProtoNameHash;
    u32 NameIdLength;
    char NameId[NameIdLength];
    u32 PrefabPathLength;
    char PrefabPath[PrefabPathLength];
    Vector3 MarkerPosition;
    Vector3 MarkerRotation;
    MarkerSetPrototype Objects;
    NaviPatchSourcePrototype NaviPatchSource;
    u16 RandomRotationDegrees;
    u16 RandomPosition;
};

struct PropPackagePrototype {
    u32 NumPropGroups;
    ProceduralPropGroupPrototype PropGroups[NumPropGroups];
};

struct PropFile {
    BinaryResourceHeader header;
    PropPackagePrototype prop;
};

PropFile file @ 0x00;