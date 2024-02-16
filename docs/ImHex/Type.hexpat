struct CalligraphyHeader {
    char signature[3];
    u8 version;         // 10 for versions 1.9-1.17, 11 for 1.18+
};

struct Asset {
    u64 id;
    u64 guid;
    u8 flags;
    u16 name_length;
    char name[name_length];
};

struct AssetType {
    CalligraphyHeader header;
    u16 num_assets;
    Asset assets[num_assets];
};

AssetType type @ 0x00;