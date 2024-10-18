// This file contains prototype definitions that are different for version 1.10

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
	
    // No filters in 1.10
    //u64 FilterGuid;     // Prototype GUID
    //u32 LastKnownFilterNameLength;
    //char LastKnownFilterName[LastKnownFilterNameLength];
};


struct CellPrototype {
    AABB BoundingBox;
    u32 Type;
    u32 Walls;
    u32 FillerEdges;
    //u32 RoadConnections;     // No road connections in 1.10
    u32 ClientMapLength;
    char ClientMap[ClientMapLength];
    MarkerSetPrototype InitializeSet;
    MarkerSetPrototype MarkerSet;
    NaviPatchSourcePrototype NaviPatchSource;
    u8 IsOffsetInMapFile;
    HeightMapPrototype HeightMap;
    u32 NumHotspotPrototypes;
    u64 HotspotPrototypes[NumHotspotPrototypes]; // Prototype GUIDs
};
