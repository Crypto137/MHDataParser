using MHDataParser.FileFormats;

namespace MHDataParser.CodeGeneration
{
    public class PrototypeField
    {
        private readonly PrototypeClass _prototypeClass;

        public string Name { get; }
        public CalligraphyBaseType BaseType { get; }
        public CalligraphyStructureType StructureType { get; }
        public ulong Subtype { get; }

        public PrototypeField(PrototypeClass prototypeClass, string name, CalligraphyBaseType baseType, CalligraphyStructureType structureType, ulong subtype)
        {
            _prototypeClass = prototypeClass;

            Name = name;
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
                    Console.WriteLine($"Invalid subtype {Subtype} for RHStruct field {Name} in {_prototypeClass.Name}, defaulting to Prototype");
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
                    CalligraphyBaseType.Type => "AssetTypeId",
                    _ => throw new()
                };
            }

            string typeSuffix = StructureType == CalligraphyStructureType.List ? "[]" : string.Empty;

            return $"public {typeName}{typeSuffix} {Name} {{ get; protected set; }}";
        }
    }
}
