struct CalligraphyHeader {
    char signature[3];
    u8 version;         // 10 for versions 1.9-1.17, 11 for 1.18+
};

struct AssetTypeRecord {
    u64 id;
    u64 guid;
    u8 flags;
    u16 file_path_length;
    char file_path[file_path_length];
};

struct AssetTypeDirectory {
    CalligraphyHeader header;
    u32 num_records;
    AssetTypeRecord records[num_records];
};

AssetTypeDirectory directory @ 0x00;