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
        public string Blueprint { get; }
        public byte NumOfCopies { get; }

        public BlueprintReferenceJson(BlueprintReference reference)
        {
            Blueprint = GameDatabase.GetBlueprintName(reference.BlueprintId);
            NumOfCopies = reference.NumOfCopies;
        }
    }

    public class BlueprintMemberJson
    {
        public ulong FieldId { get; }
        public string FieldName { get; }
        public string BaseType { get; }
        public string StructureType { get; }
        public string Subtype { get; }

        public BlueprintMemberJson(BlueprintMember member)
        {
            FieldId = (ulong)member.FieldId;
            FieldName = member.FieldName;
            BaseType = member.BaseType.ToString();
            StructureType = member.StructureType.ToString();

            switch (member.BaseType)
            {
                // Only these base types have subtypes
                case CalligraphyBaseType.Asset:
                    Subtype = GameDatabase.GetAssetTypeName((AssetTypeId)member.Subtype);
                    break;

                case CalligraphyBaseType.Curve:
                    Subtype = GameDatabase.GetCurveName((CurveId)member.Subtype);
                    break;

                case CalligraphyBaseType.Prototype:
                case CalligraphyBaseType.RHStruct:
                    Subtype = GameDatabase.GetPrototypeName((PrototypeId)member.Subtype);
                    break;
            }
        }
    }
}