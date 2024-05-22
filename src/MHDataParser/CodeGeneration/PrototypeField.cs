using MHDataParser.FileFormats;

namespace MHDataParser.CodeGeneration
{
    public class PrototypeField
    {
        public string FieldName { get; }
        public CalligraphyBaseType BaseType { get; }
        public CalligraphyStructureType StructureType { get; }
        public ulong Subtype { get; }

        public PrototypeField(string fieldName, CalligraphyBaseType baseType, CalligraphyStructureType structureType, ulong subtype)
        {
            FieldName = fieldName;
            BaseType = baseType;
            StructureType = structureType;
            Subtype = subtype;
        }

        public string GenerateCode()
        {
            string typeName;

            if (BaseType == CalligraphyBaseType.RHStruct)
            {
                string blueprintName = GameDatabase.GetBlueprintName((BlueprintId)Subtype);

                if (GameDatabase.BlueprintDict.TryGetValue(blueprintName, out Blueprint blueprint) == false)
                {
                    Console.WriteLine($"Invalid subtype {Subtype} for RHStruct field {FieldName}");
                    typeName = "Prototype";
                }
                else
                    typeName = blueprint.RuntimeBinding;
            }
            else
            {
                typeName = BaseType switch
                {
                    CalligraphyBaseType.Asset => "AssetId",
                    CalligraphyBaseType.Boolean => "bool",
                    CalligraphyBaseType.Curve => "CurveId",
                    CalligraphyBaseType.Double => "double",
                    CalligraphyBaseType.Long => "long",
                    CalligraphyBaseType.Prototype => "PrototypeId",
                    CalligraphyBaseType.String => "LocaleStringId",
                    CalligraphyBaseType.Type => "ulong",
                    _ => throw new()
                };
            }

            string typeSuffix = StructureType == CalligraphyStructureType.List ? "[]" : string.Empty;

            return $"public {typeName}{typeSuffix} {FieldName} {{ get; protected set; }}";
        }
    }
}
