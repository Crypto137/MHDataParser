struct BinaryResourceHeader {
    u8 cooker_version;
    u8 endianness;
    u16 unk;
    u32 prototype_data_version;
    u32 class_hash;
};

struct PropSetTypeEntryPrototype {
    u32 ProtoNameHash;
    u32 NameIdLength;
    char NameId[NameIdLength];
    u32 ResourcePackageLength;
    char ResourcePackage[ResourcePackageLength];
};

struct PropSetTypeListPrototype {
    u32 ProtoNameHash;
    u32 NumPropShapeEntries;
    PropSetTypeEntryPrototype PropShapeEntries[NumPropShapeEntries];
    u64 PropType;   // Prototype GUID
};

struct PropSetPrototype {
    u32 NumPropShapeLists;
    PropSetTypeListPrototype PropShapeLists[NumPropShapeLists];
    u32 PropSetPackageLength;
    char PropSetPackat[PropSetPackageLength];
};

struct PropSetFile {
    BinaryResourceHeader header;
    PropSetPrototype prop_set;
};

PropSetFile file @ 0x00;