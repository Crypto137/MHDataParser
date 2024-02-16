#pragma array_limit 2147483647
#pragma pattern_limit 2147483647

struct PakEntry {
    u64 file_hash;
    u32 file_name_length;
    char file_name[file_name_length];
    u32 mod_time;
    u32 offset;
    u32 compressed_size;
    u32 uncompressed_size;
};

struct PakFile {
    u32 signature;
    u32 version;
    u32 num_entries;
    PakEntry entries[num_entries];
    // data offsets start from here
};

PakFile file @ 0x00;