struct CalligraphyHeader {
    char signature[3];
    u8 version;         // 10 for versions 1.9-1.17, 11 for 1.18+
};

struct BlueprintReference {
    u64 blueprint_id;
    u8 num_of_copies;
};

struct BlueprintMember {
    u64 field_id;
    u16 field_name_length;
    char field_name[field_name_length];
    char base_type;
    char structure_type;
    
    if (base_type == 'A' || base_type == 'C'
     || base_type == 'P' || base_type == 'R') {
        u64 subtype;
    }
};

struct Blueprint {
    CalligraphyHeader header;
    u16 runtime_binding_length;
    char runtime_binding[runtime_binding_length];
    u64 default_prototype_id;
    u16 num_parents;
    BlueprintReference parents[num_parents];
    u16 num_contributing_blueprints;
    BlueprintReference contributing_blueprints[num_contributing_blueprints];
    u16 num_members;
    BlueprintMember members[num_members];
};

Blueprint blueprint @ 0x00;