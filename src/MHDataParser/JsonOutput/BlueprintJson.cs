using MHDataParser.FileFormats;

namespace MHDataParser.JsonOutput
{
    public class BlueprintJson
    {
        public string RuntimeBinding { get; }
        public string DefaultPrototype { get; }
        public BlueprintReferenceJson[] Parents { get; }
        public BlueprintReferenceJson[] ContributingBlueprints { get; }
        public BlueprintMemberJson[] Members { get; }

        public BlueprintJson(Blueprint blueprint)
        {
            RuntimeBinding = blueprint.RuntimeBinding;
            DefaultPrototype = GameDatabase.GetPrototypeName(blueprint.DefaultPrototypeId);

            Parents = new BlueprintReferenceJson[blueprint.Parents.Length];
            for (int i = 0; i < Parents.Length; i++)
                Parents[i] = new(blueprint.Parents[i]);

            ContributingBlueprints = new BlueprintReferenceJson[blueprint.ContributingBlueprints.Length];
            for (int i = 0; i < ContributingBlueprints.Length; i++)
                ContributingBlueprints[i] = new(blueprint.ContributingBlueprints[i]);

            Members = new BlueprintMemberJson[blueprint.Members.Length];
            for (int i = 0; i < Members.Length; i++)
                Members[i] = new(blueprint.Members[i]);
        }        
    }

    public class BlueprintReferenceJson
    {
        public string Name { get; }
        public byte Flags { get; }

        public BlueprintReferenceJson(BlueprintReference reference)
        {
            Name = GameDatabase.GetPrototypeName(reference.Id);
            Flags = reference.Flags;
        }
    }

    public class BlueprintMemberJson
    {
        public ulong FieldId { get; }
        public string FieldName { get; }
        public string ValueType { get; }
        public string ContainerType { get; }
        public string Subtype { get; }

        public BlueprintMemberJson(BlueprintMember member)
        {
            FieldId = (ulong)member.FieldId;
            FieldName = member.FieldName;
            ValueType = member.ValueType.ToString();
            ContainerType = member.ContainerType.ToString();

            switch (member.ValueType)
            {
                // Only these types have subtypes
                case CalligraphyValueType.Asset:
                    Subtype = GameDatabase.GetAssetTypeName((AssetTypeId)member.Subtype);
                    break;

                case CalligraphyValueType.Curve:
                    Subtype = GameDatabase.GetCurveName((CurveId)member.Subtype);
                    break;

                case CalligraphyValueType.Prototype:
                case CalligraphyValueType.RHStruct:
                    Subtype = GameDatabase.GetPrototypeName((PrototypeId)member.Subtype);
                    break;
            }
        }
    }
}