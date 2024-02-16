#pragma array_limit 2147483647
#pragma pattern_limit 2147483647

struct CalligraphyHeader {
    char signature[3];
    u8 version;         // 10 for versions 1.9-1.17, 11 for 1.18+
};

struct PrototypeRecord {
    u64 id;
    u64 guid;
    u64 blueprint_id;
    u8 flags;
    u16 file_path_length;
    char file_path[file_path_length];
};

struct PrototypeDirectory {
    CalligraphyHeader header;
    u32 num_records;
    PrototypeRecord records[num_records];
};

PrototypeDirectory directory @ 0x00;