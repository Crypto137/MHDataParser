﻿using System.Text;
using MHDataParser.FileFormats;

namespace MHDataParser.CodeGeneration
{
    public class PrototypeClass
    {
        private readonly Dictionary<string, PrototypeField> _fieldDict = new();

        private readonly HashSet<string> _parents = new();

        private bool _hasPropertyMixin = false;

        public string Name { get; }

        public PrototypeClass(string name)
        {
            Name = name;
        }

        public void AddField(string fieldName, CalligraphyBaseType baseType, CalligraphyStructureType structureType, ulong subtype)
        {
            if (_fieldDict.ContainsKey(fieldName) == false)
                _fieldDict.Add(fieldName, new(this, fieldName, baseType, structureType, subtype));
        }

        public void AddParent(BlueprintId parentRef)
        {
            // If this is a property mixin reference, just flag this class and exit
            if (GameDatabase.IsPropertyMixinBlueprint(parentRef))
            {
                if (_hasPropertyMixin == false)
                {
                    Console.WriteLine($"Found property mixin in {Name}");
                    _hasPropertyMixin = true;
                }

                return;
            }

            string parentName = GameDatabase.GetBlueprintName(parentRef);

            if (GameDatabase.BlueprintDict.TryGetValue(parentName, out Blueprint parent) == false)
            {
                Console.WriteLine($"Failed to find parent blueprint {parentName} for {Name}");
                return;
            }

            if (parent.RuntimeBinding != Name && parent.RuntimeBinding != "Prototype")
                _parents.Add(parent.RuntimeBinding);
        }

        public string GenerateCode()
        {
            StringBuilder sb = new();

            string baseClass;
            if (_parents.Count == 0)
            {
                baseClass = "Prototype";        // No parents (inherit from the base Prototype class)
            }
            else if (_parents.Count == 1)
            {
                baseClass = _parents.First();   // Only a single parent, so we can be certain it's the one
            }
            else if (PrototypeParentLookupTable.TryGetParentForPrototype(Name, out string lookupParentName))
            {
                // We are using a hardcoded hierarchy lookup (e.g. entity hierarchy which stayed generally the same for all versions)
                Console.WriteLine($"Resolving parent conflict using hardcoded lookup: {Name} => {lookupParentName}");
                baseClass = lookupParentName;
            }
            else
            {
                // If we have multiple parents, we will default to Prototype, but list all potential parents as comments
                Console.WriteLine($"Multiple potential parents for {Name}:{_parents.Aggregate(string.Empty, (current, next) => $"{current} {next}")}");
                baseClass = "Prototype";
                foreach (string parentName in _parents)
                    sb.AppendLine($"// {parentName}");
            }

            sb.AppendLine($@"public class {Name} : {baseClass}");
            sb.AppendLine("{");
            if (Name != "PropertyPrototype")    // Ignore property mixin fields
            {
                // Sort fields for consistent output between versions so that we can more easily compare
                foreach (PrototypeField field in _fieldDict.Values.OrderBy(field => field.Name))
                    sb.AppendLine($"\t{field.GenerateCode()}");
            }
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
