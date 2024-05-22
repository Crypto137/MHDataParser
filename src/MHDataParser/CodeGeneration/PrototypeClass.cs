using System.Text;
using MHDataParser.FileFormats;

namespace MHDataParser.CodeGeneration
{
    public class PrototypeClass
    {
        private readonly Dictionary<string, PrototypeField> _fieldDict = new();

        public string ClassName { get; }

        public PrototypeClass(string className)
        {
            ClassName = className;
        }

        public void AddField(string fieldName, CalligraphyBaseType baseType, CalligraphyStructureType structureType, ulong subtype)
        {
            _fieldDict.Add(fieldName, new(fieldName, baseType, structureType, subtype));
        }

        public bool HasField(string fieldName)
        {
            return _fieldDict.ContainsKey(fieldName);
        }

        public string GenerateCode()
        {
            StringBuilder sb = new();

            sb.AppendLine($@"public class {ClassName}");
            sb.AppendLine("{");
            // Sort fields for consistent output between versions so that we can more easily compare
            foreach (PrototypeField field in _fieldDict.Values.OrderBy(field => field.FieldName))
                sb.AppendLine($"\t{field.GenerateCode()}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
