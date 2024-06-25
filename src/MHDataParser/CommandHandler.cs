using System.Reflection;

namespace MHDataParser
{
    public static class CommandHandler
    {
        private static readonly Dictionary<string, (Action, string)> _commandDict = new();

        static CommandHandler()
        {
            foreach (MethodInfo method in typeof(CommandHandler).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (method.IsDefined(typeof(CommandAttribute)) == false) continue;

                var attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute == null) continue;

                _commandDict[attribute.Name] = (method.CreateDelegate<Action>(), attribute.Description);
            }
        }

        public static void HandleCommand(string command)
        {
            if (_commandDict.TryGetValue(command.ToLower(), out var commandEntry) == false)
            {
                Console.WriteLine($"Command '{command}' does not exist");
                return;
            }

            commandEntry.Item1();
        }

        #region Command Implementations

        [Command("help", "Prints command list.")]
        private static void Help()
        {
            Console.WriteLine("\tCommand List");

            foreach (var kvp in _commandDict)
                Console.WriteLine($"{kvp.Key}: {kvp.Value.Item2}");
        }

        [Command("extract-pak", "Extracts all entries and data from loaded pak files.")]
        private static void ExtractPak() => GameDatabase.ExtractPak(true, true);

        [Command("extract-pak-entries", "Extracts all entries from loaded pak files.")]
        private static void ExtractPakEntries() => GameDatabase.ExtractPak(true, false);

        [Command("extract-pak-data", "Extracts all data from loaded pak files.")]
        private static void ExtractPakData() => GameDatabase.ExtractPak(false, true);

        [Command("export-all-data", "Exports all parsed Calligraphy and resource data.")]
        private static void ExportAllData()
        {
            ExportCalligraphy();
            ExportResources();
        }

        [Command("export-calligraphy", "Exports parsed Calligraphy data (directories, curves, asset types, blueprints, and prototypes).")]
        private static void ExportCalligraphy()
        {
            ExportDirectories();
            ExportCurves();
            ExportAssetTypes();
            ExportBlueprints();
            ExportPrototypes();
        }

        [Command("export-directories", "Exports parsed directories as TSV.")]
        private static void ExportDirectories() => GameDatabase.ExportDirectories();

        [Command("export-curves", "Exports parsed curves as TSV.")]
        private static void ExportCurves() => GameDatabase.ExportCurves();

        [Command("export-asset-types", "Exports parsed asset types as JSON.")]
        private static void ExportAssetTypes() => GameDatabase.ExportAssetTypes();

        [Command("export-blueprints", "Exports parsed blueprints as JSON.")]
        private static void ExportBlueprints() => GameDatabase.ExportBlueprints();

        [Command("export-prototypes", "Exports parsed prototypes as JSON.")]
        private static void ExportPrototypes() => GameDatabase.ExportPrototypes();

        [Command("export-resources", "Exports parsed resource data (cells, districts, encounters, props, prop sets, and UIs).")]
        private static void ExportResources()
        {
            ExportCells();
            ExportDistricts();
            ExportEncounters();
            ExportProps();
            ExportPropSets();
            ExportUIs();
        }

        [Command("export-cells", "Exports parsed cells as JSON.")]
        private static void ExportCells() => GameDatabase.ExportCells();

        [Command("export-districts", "Exports parsed districts as JSON.")]
        private static void ExportDistricts() => GameDatabase.ExportDistricts();

        [Command("export-encounters", "Exports parsed encounters as JSON.")]
        private static void ExportEncounters() => GameDatabase.ExportEncounters();

        [Command("export-props", "Exports parsed props as JSON.")]
        private static void ExportProps() => GameDatabase.ExportProps();

        [Command("export-prop-sets", "Exports parsed prop sets as JSON.")]
        private static void ExportPropSets() => GameDatabase.ExportPropSets();

        [Command("export-uis", "Exports parsed UIs as JSON.")]
        private static void ExportUIs() => GameDatabase.ExportUIs();

        [Command("export-prototype-enums", "Exports Calligraphy prototype hierarchy cache enums needed for archive serialization.")]
        private static void ExportPrototypeEnums() => GameDatabase.ExportPrototypeEnums();

        [Command("export-prototype-runtime-bindings", "Exports prototype runtime bindings.")]
        private static void ExportPrototypeRuntimeBindings() => GameDatabase.ExportPrototypeRuntimeBindings();

        [Command("export-blueprint-enums", "Exports Calligraphy blueprint hierarchy cache enums needed for property params.")]
        private static void ExportBlueprintEnums() => GameDatabase.ExportBlueprintEnums();

        [Command("export-property-info-table", "Exports data from property info prototypes to a TSV file.")]
        private static void ExportPropertyInfoTable() => GameDatabase.ExportPropertyInfoTable();

        [Command("generate-prototype-classes", "Generates C# classes from prototype blueprints.")]
        private static void GeneratePrototypeClasses() => GameDatabase.GeneratePrototypeClasses();

        [Command("export-asset-refs", "Exports asset data references to TSV.")]
        private static void ExportAssetRefs() => GameDatabase.ExportAssetRefs();

        [Command("export-locales", "Exports parsed locales with all of their strings as JSON.")]
        private static void ExportLocales() => GameDatabase.ExportLocales();

        #endregion

        [AttributeUsage(AttributeTargets.Method)]
        private class CommandAttribute : Attribute
        {
            public string Name { get; }
            public string Description { get; }

            public CommandAttribute(string name, string description)
            {
                Name = name.ToLower();
                Description = description;
            }
        }
    }
}
