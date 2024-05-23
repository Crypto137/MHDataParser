using MHDataParser.FileFormats;

namespace MHDataParser.CodeGeneration
{
    public static class PrototypeClassGenerator
    {
        public static void Generate(string outputPath)
        {
            Dictionary<string, PrototypeClass> prototypeClassDict = new();

            foreach (Blueprint blueprint in GameDatabase.BlueprintDict.Values)
            {
                string runtimeBinding = blueprint.RuntimeBinding;

                if (prototypeClassDict.TryGetValue(runtimeBinding, out PrototypeClass prototypeClass) == false)
                {
                    prototypeClass = new(runtimeBinding);
                    prototypeClassDict.Add(runtimeBinding, prototypeClass);
                }

                foreach (BlueprintReference blueprintRef in blueprint.Parents)
                    prototypeClass.AddParent(blueprintRef.BlueprintId);

                foreach (BlueprintMember member in blueprint.Members)
                    prototypeClass.AddField(member.FieldName, member.BaseType, member.StructureType, member.Subtype);
            }

            Console.WriteLine($"Found {prototypeClassDict.Count} unique runtime bindings");

            // validation should happen beforehand
            using (StreamWriter writer = new(outputPath))
            {
                // Sort classes for consistent output between versions so that we can more easily compare
                foreach (PrototypeClass prototypeClass in prototypeClassDict.Values.OrderBy(prototypeClass => prototypeClass.Name))
                    writer.WriteLine(prototypeClass.GenerateCode());
            }
        }
    }
}
