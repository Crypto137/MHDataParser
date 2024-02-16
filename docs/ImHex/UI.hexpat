struct String {
    u32 length;
    char text[length];
};

struct Vector2 {
    float x;
    float y;
};

struct BinaryResourceHeader {
    u8 cooker_version;
    u8 endianness;
    u16 unk;
    u32 prototype_data_version;
    u32 class_hash;
};

struct StretchedPanelPrototype {
    Vector2 TopLeftPin;
    String TL_X_TargetName;
    String TL_Y_TargetName;
    Vector2 BottomRightPin;
    String BR_X_TargetName;
    String BR_Y_TargetName;
};

struct AnchoredPanelPrototype {
    Vector2 SourceAttachmentPin;
    Vector2 TargetAttachmentPin;
    Vector2 VirtualPixelOffset;
    String PreferredLane;
    Vector2 OuterEdgePin;
    Vector2 NewSourceAttachmentPin;
};

struct UIPanelPrototype {
    u32 ProtoNameHash;
    
    if (ProtoNameHash == 805156721) {
        StretchedPanelPrototype StretchedPanel;
    }
    else if (ProtoNameHash == 1255662575) {
        AnchoredPanelPrototype AnchoredPanel;
    }
    
    if (ProtoNameHash != 0) {
        String PanelName;
        String TargetName;
        u32 ScaleMode;
        UIPanelPrototype Children;
        String WidgetClass;
        String SwfName;
        u8 OpenOnStart;
        u8 VisibilityToggleable;
        u8 CanClickThrough;
        u8 StaticPosition;
        u8 EntityInteractPanel;
        u8 UseNewPlacementSystem;
        u8 KeepLoaded;
    }
};

struct UIPrototype {
    u32 NumUIPanels;
    UIPanelPrototype UIPanels[NumUIPanels];
};

struct UIFile {
    BinaryResourceHeader header;
    UIPrototype ui;
};

UIFile file @ 0x00;