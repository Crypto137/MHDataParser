using Prototype;

struct CalligraphyHeader {
    char signature[3];
    u8 version;         // 10 for versions 1.9-1.17, 11 for 1.18+
};

struct PrototypeDataHeader {
    u8 flags;
    // 0x01 ReferenceExists,
    // 0x02 InstanceDataExists,
    // 0x04 PolymorphicData
    if ((flags & 0x01) != 0) {
        u64 reference_type;
    }
};

struct SimpleField {
    u64 field_id;
    char calligraphy_base_type;
    
    if (calligraphy_base_type == 'R') {
        Prototype rh_struct;
    }
    else {
        u64 value;
    }
};

struct ListField {
    u64 field_id;
    char calligraphy_base_type;
    
    u16 num_values;
    if (calligraphy_base_type == 'R') {
        Prototype rh_struct[num_values];
    }
    else {
        u64 value[num_values];
    }
};

struct FieldGroup {
    u64 declaring_blueprint_id;
    u8 blueprint_copy_number;
    
    u16 num_simple_fields;
    SimpleField simple_fields[num_simple_fields];
    
    u16 num_list_fields;
    ListField list_fields[num_list_fields];
};

struct Prototype {
    PrototypeDataHeader header;
    if ((header.flags & 0x02) != 0) {
        u16 num_field_groups;
        FieldGroup field_groups[num_field_groups];
    }
};

struct PrototypeFile {
    CalligraphyHeader header;
    Prototype prototype;
};

PrototypeFile file @ 0x00;