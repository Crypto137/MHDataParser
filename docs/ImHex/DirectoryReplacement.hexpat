struct CalligraphyHeader {
    char signature[3];
    u8 version;         // 10 for versions 1.9-1.17, 11 for 1.18+
};

struct ReplacementRecord {
    u64 old_guid;
    u64 new_guid;
    u16 name_length;
    char name[name_length];
};

struct ReplacementDirectory {
    CalligraphyHeader header;
    u32 num_records;
    ReplacementRecord records[num_records];
};

ReplacementDirectory directory @ 0x00;