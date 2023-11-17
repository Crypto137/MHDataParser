﻿namespace MHDataParser.FileFormats
{
    public class UIPrototype
    {
        public ResourceHeader Header { get; }
        public UIPanelPrototype[] UIPanels { get; }

        public UIPrototype(byte[] data)
        {
            using (MemoryStream stream = new(data))
            using (BinaryReader reader = new(stream))
            {
                Header = new(reader);

                try
                {
                    UIPanels = new UIPanelPrototype[reader.ReadUInt32()];
                    for (int i = 0; i < UIPanels.Length; i++)
                        UIPanels[i] = ReadUIPanelPrototype(reader);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse UI prototype: {e.Message}");
                }
            }
        }

        private UIPanelPrototype ReadUIPanelPrototype(BinaryReader reader)
        {
            UIPanelPrototype panelPrototype;
            ResourcePrototypeHash hash = (ResourcePrototypeHash)reader.ReadUInt32();

            switch (hash)
            {
                case ResourcePrototypeHash.StretchedPanelPrototype:
                    panelPrototype = new StretchedPanelPrototype(reader);
                    break;
                case ResourcePrototypeHash.AnchoredPanelPrototype:
                    panelPrototype = new AnchoredPanelPrototype(reader);
                    break;
                case ResourcePrototypeHash.None:
                    panelPrototype = null;
                    break;
                default:
                    throw new($"Unknown ResourcePrototypeHash {(uint)hash}");   // Throw an exception if there's a hash for a type we didn't expect
            }

            return panelPrototype;
        }
    }

    public class UIPanelPrototype
    {
        public ResourcePrototypeHash ProtoNameHash { get; protected set; }
        public string PanelName { get; protected set; }
        public string TargetName { get; protected set; }
        public uint ScaleMode { get; protected set; }
        public UIPanelPrototype Children { get; protected set; }
        public string WidgetClass { get; protected set; }
        public string SwfName { get; protected set; }
        public byte OpenOnStart { get; protected set; }
        public byte VisibilityToggleable { get; protected set; }
        public byte CanClickThrough { get; protected set; }
        public byte StaticPosition { get; protected set; }
        public byte EntityInteractPanel { get; protected set; }
        public byte UseNewPlacementSystem { get; protected set; }
        public byte KeepLoaded { get; protected set; }

        protected void ReadParentPanelFields(BinaryReader reader)
        {
            PanelName = reader.ReadFixedString32();
            TargetName = reader.ReadFixedString32();
            ScaleMode = reader.ReadUInt32();
            Children = ReadUIPanelPrototype(reader);
            WidgetClass = reader.ReadFixedString32();
            SwfName = reader.ReadFixedString32();
            OpenOnStart = reader.ReadByte();
            VisibilityToggleable = reader.ReadByte();
            CanClickThrough = reader.ReadByte();
            StaticPosition = reader.ReadByte();
            EntityInteractPanel = reader.ReadByte();
            UseNewPlacementSystem = reader.ReadByte();
            KeepLoaded = reader.ReadByte();
        }

        protected UIPanelPrototype ReadUIPanelPrototype(BinaryReader reader)
        {
            UIPanelPrototype panelPrototype;
            ResourcePrototypeHash hash = (ResourcePrototypeHash)reader.ReadUInt32();

            switch (hash)
            {
                case ResourcePrototypeHash.StretchedPanelPrototype:
                    panelPrototype = new StretchedPanelPrototype(reader);
                    break;
                case ResourcePrototypeHash.AnchoredPanelPrototype:
                    panelPrototype = new AnchoredPanelPrototype(reader);
                    break;
                case ResourcePrototypeHash.None:
                    panelPrototype = null;
                    break;
                default:
                    throw new($"Unknown ResourcePrototypeHash {(uint)hash}");   // Throw an exception if there's a hash for a type we didn't expect
            }

            return panelPrototype;
        }
    }

    public class StretchedPanelPrototype : UIPanelPrototype
    {
        public Vector2 TopLeftPin { get; }
        public string TL_X_TargetName { get; }
        public string TL_Y_TargetName { get; }
        public Vector2 BottomRightPin { get; }
        public string BR_X_TargetName { get; }
        public string BR_Y_TargetName { get; }

        public StretchedPanelPrototype(BinaryReader reader)
        {
            ProtoNameHash = ResourcePrototypeHash.StretchedPanelPrototype;

            TopLeftPin = new(reader);
            TL_X_TargetName = reader.ReadFixedString32();
            TL_Y_TargetName = reader.ReadFixedString32();
            BottomRightPin = new(reader);
            BR_X_TargetName = reader.ReadFixedString32();
            BR_Y_TargetName = reader.ReadFixedString32();

            ReadParentPanelFields(reader);
        }
    }

    public class AnchoredPanelPrototype : UIPanelPrototype
    {
        public Vector2 SourceAttachmentPin { get; }
        public Vector2 TargetAttachmentPin { get; }
        public Vector2 VirtualPixelOffset { get; }
        public string PreferredLane { get; }
        public Vector2 OuterEdgePin { get; }
        public Vector2 NewSourceAttachmentPin { get; }

        public AnchoredPanelPrototype(BinaryReader reader)
        {
            ProtoNameHash = ResourcePrototypeHash.AnchoredPanelPrototype;

            SourceAttachmentPin = new(reader);
            TargetAttachmentPin = new(reader);
            VirtualPixelOffset = new(reader);
            PreferredLane = reader.ReadFixedString32();
            OuterEdgePin = new(reader);
            NewSourceAttachmentPin = new(reader);

            ReadParentPanelFields(reader);
        }
    }
}
