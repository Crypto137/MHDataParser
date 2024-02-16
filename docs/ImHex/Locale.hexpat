struct LocaleFlag {
    u16 bit_value;
    u16 bit_mask;
    u16 flag_string_length;
    char flag_string[flag_string_length];
};

struct Locale {
    char Signature[3];
    u8 Version;
    u16 name_length;
    char name[name_length];
    u16 language_display_name_length;
    char language_display_name[language_display_name_length];
    u16 region_display_name_length;
    char region_display_name[region_display_name_length];
    u16 directory_length;
    char directory[directory_length];
    u8 num_flags;
    LocaleFlag flags[num_flags];
};

Locale locale @ 0x00;