#pragma pattern_limit 2147483647

struct StringVariation {
    u64 flags_consumed;
    u16 flags_produced;
    u32 string_offset;
    char string[] @ string_offset;
};

struct StringMapEntry {
    u64 id;
    u16 num_variants;
    u16 flags_produced;
    u32 string_offset;
    char string[] @ string_offset;
    if (num_variants > 0) {
        StringVariation variants[num_variants - 1];
    }
};

struct StringFile {
    char Signature[3];
    u8 Version;
    u16 num_string_map_entries;
    StringMapEntry string_map[num_string_map_entries];
};

StringFile string_file @ 0x00;