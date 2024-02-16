struct CalligraphyHeader {
    char signature[3];
    u8 version;         // 10 for versions 1.9-1.17, 11 for 1.18+
};

struct Curve {
    CalligraphyHeader header;
    u32 start_position;
    u32 end_position;
    double values[end_position - start_position + 1];
};

Curve curve @ 0x00;