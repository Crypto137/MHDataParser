using MHDataParser.FileFormats;

namespace MHDataParser.JsonOutput
{
    public class PrototypeFileJson
    {
        public CalligraphyHeader Header { get; }
        public PrototypeJson Prototype { get; }

        public PrototypeFileJson(PrototypeFile prototypeFile)
        {
            Header = prototypeFile.Header;
            Prototype = new(prototypeFile.Prototype);
        }
    }

    public class PrototypeDataHeaderJson
    {
        public bool ReferenceExists { get; }
        public bool DataExists { get; }
        public bool PolymorphicData { get; }
        public string ReferenceType { get; }

        public PrototypeDataHeaderJson(PrototypeDataHeader header)
        {
            ReferenceExists = header.ReferenceExists;
            DataExists = header.DataExists;
            PolymorphicData = header.PolymorphicData;
            ReferenceType = GameDatabase.GetPrototypeName(header.ReferenceType);
        }
    }

    public class PrototypeJson
    {
        public PrototypeDataHeaderJson Header { get; }
        public PrototypeFieldGroupJson[] FieldGroups { get; }

        public PrototypeJson(Prototype prototype)
        {
            Header = new(prototype.Header);

            if (prototype.FieldGroups != null)
            {
                FieldGroups = new PrototypeFieldGroupJson[prototype.FieldGroups.Length];
                for (int i = 0; i < FieldGroups.Length; i++)
                    FieldGroups[i] = new(prototype.FieldGroups[i]);
            }
        }
    }

    public class PrototypeFieldGroupJson
    {
        public string DeclaringBlueprint { get; }
        public byte BlueprintCopyNumber { get; }
        public PrototypeSimpleFieldJson[] SimpleFields { get; }
        public PrototypeListFieldJson[] ListFields { get; }

        public PrototypeFieldGroupJson(PrototypeFieldGroup entry)
        {
            DeclaringBlueprint = GameDatabase.GetBlueprintName(entry.DeclaringBlueprintId);
            BlueprintCopyNumber = entry.BlueprintCopyNumber;

            SimpleFields = new PrototypeSimpleFieldJson[entry.SimpleFields.Length];
            for (int i = 0; i < SimpleFields.Length; i++)
                SimpleFields[i] = new(entry.SimpleFields[i]);

            ListFields = new PrototypeListFieldJson[entry.ListFields.Length];
            for (int i = 0; i < ListFields.Length; i++)
                ListFields[i] = new(entry.ListFields[i]);
        }
    }

    public class PrototypeSimpleFieldJson
    {
        public string Name { get; }
        public string Type { get; }
        public object Value { get; }

        public PrototypeSimpleFieldJson(PrototypeSimpleField field)
        {
            Name = GameDatabase.GetBlueprintFieldName(field.Id);
            Type = field.Type.ToString();

            switch (field.Type)
            {
                case CalligraphyBaseType.Asset:
                    var assetId = (StringId)field.Value;
                    string assetName = GameDatabase.GetAssetName(assetId);
                    string assetTypeName = GameDatabase.GetAssetTypeName(GameDatabase.GetAssetTypeId(assetId));
                    Value = $"{assetName} ({assetTypeName})";
                    break;
                case CalligraphyBaseType.Curve:
                    Value = GameDatabase.GetCurveName((CurveId)field.Value);
                    break;
                case CalligraphyBaseType.Prototype:
                    Value = GameDatabase.GetPrototypeName((PrototypeId)field.Value);
                    break;
                case CalligraphyBaseType.RHStruct:
                    Value = new PrototypeJson((Prototype)field.Value);
                    break;
                case CalligraphyBaseType.Type:
                    Value = GameDatabase.GetAssetTypeName((AssetTypeId)field.Value);
                    break;
                default:
                    Value = field.Value;
                    break;
            }
        }
    }

    public class PrototypeListFieldJson
    {
        public string Name { get; }
        public string Type { get; }
        public object[] Values { get; }

        public PrototypeListFieldJson(PrototypeListField field)
        {
            Name = GameDatabase.GetBlueprintFieldName(field.Id);
            Type = field.Type.ToString();

            Values = new object[field.Values.Length];
            for (int i = 0; i < Values.Length; i++)
            {
                switch (field.Type)
                {
                    case CalligraphyBaseType.Asset:
                        var assetId = (StringId)field.Values[i];
                        string assetName = GameDatabase.GetAssetName(assetId);
                        string assetTypeName = GameDatabase.GetAssetTypeName(GameDatabase.GetAssetTypeId(assetId));
                        Values[i] = $"{assetName} ({assetTypeName})";
                        break;
                    case CalligraphyBaseType.Curve:
                        Values[i] = GameDatabase.GetCurveName((CurveId)field.Values[i]);
                        break;
                    case CalligraphyBaseType.Prototype:
                        Values[i] = GameDatabase.GetPrototypeName((PrototypeId)field.Values[i]);
                        break;
                    case CalligraphyBaseType.RHStruct:
                        Values[i] = new PrototypeJson((Prototype)field.Values[i]);
                        break;
                    case CalligraphyBaseType.Type:
                        Values[i] = GameDatabase.GetAssetTypeName((AssetTypeId)field.Values[i]);
                        break;
                    default:
                        Values[i] = field.Values[i];
                        break;
                }
            }
        }
    }
}