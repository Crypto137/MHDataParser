namespace MHDataParser
{
    public static class CommandHandler
    {
        public static void HandleCommand(string command)
        {
            switch (command.ToLower())
            {
                case "help":                    OnHelp();               break;

                case "extract-pak":             OnExtractPak();         break;
                case "extract-pak-entries":     OnExtractPakEntries();  break;
                case "extract-pak-data":        OnExtractPakData();     break;

                case "export-all-data":         OnExportAllData();      break;

                case "export-calligraphy":      OnExportCalligraphy();  break;
                case "export-directories":      OnExportDirectories();  break;
                case "export-curves":           OnExportCurves();       break;
                case "export-asset-types":      OnExportAssetTypes();   break;
                case "export-blueprints":       OnExportBlueprints();   break;
                case "export-prototypes":       OnExportPrototypes();   break;

                case "export-resources":        OnExportResources();    break;
                case "export-cells":            OnExportCells();        break;
                case "export-districts":        OnExportDistricts();    break;
                case "export-encounters":       OnExportEncounters();   break;
                case "export-props":            OnExportProps();        break;
                case "export-prop-sets":        OnExportPropSets();     break;
                case "export-uis":              OnExportUIs();          break;

                case "export-locales":          OnExportLocales();      break;

                case "export-prototype-enums":  OnExportPrototypeEnums(); break;

                default: Console.WriteLine($"Command '{command}' does not exist"); break;
            }
        }

        private static void OnHelp()
        {
            Console.WriteLine("\tCommand List");

            Console.WriteLine("extract-pak: Extracts all entries and data from loaded pak files.");
            Console.WriteLine("extract-pak-entries: Extracts all entries from loaded pak files.");
            Console.WriteLine("extract-pak-data: Extracts all data from loaded pak files.");

            Console.WriteLine();

            Console.WriteLine("export-all-data: Exports all parsed data.");

            Console.WriteLine();

            Console.WriteLine("export-calligraphy: Exports parsed Calligraphy data (directories, curves, asset types, blueprints, and prototypes).");
            Console.WriteLine("export-directories: Exports parsed directories as TSV.");
            Console.WriteLine("export-curves: Exports parsed curves as TSV.");
            Console.WriteLine("export-asset-types: Exports parsed asset types as JSON.");
            Console.WriteLine("export-blueprints: Exports parsed blueprints as JSON.");
            Console.WriteLine("export-prototypes: Exports parsed prototypes as JSON.");

            Console.WriteLine();

            Console.WriteLine("export-resources: Exports parsed resource data (cells, districts, encounters, props, prop sets, and UIs.");
            Console.WriteLine("export-cells: Exports parsed cells as JSON.");
            Console.WriteLine("export-districts: Exports parsed districts as JSON.");
            Console.WriteLine("export-encounters: Exports parsed encounters as JSON.");
            Console.WriteLine("export-props: Exports parsed props as JSON.");
            Console.WriteLine("export-prop-sets: Exports parsed prop sets as JSON.");
            Console.WriteLine("export-uis: Exports parsed UIs as JSON.");
            Console.WriteLine("export-prototype-enums: Exports Calligraphy prototype hierarchy cache enums needed for archive serialization.");

            Console.WriteLine();

            Console.WriteLine("export-locales: Exports parsed locales with all of their strings as JSON.");
        }

        private static void OnExtractPak() => GameDatabase.ExtractPak(true, true);
        private static void OnExtractPakEntries() => GameDatabase.ExtractPak(true, false);
        private static void OnExtractPakData() => GameDatabase.ExtractPak(false, true);
        
        private static void OnExportAllData()
        {
            OnExportCalligraphy();
            OnExportResources();
        }

        private static void OnExportCalligraphy()
        {
            OnExportDirectories();
            OnExportCurves();
            OnExportAssetTypes();
            OnExportBlueprints();
            OnExportPrototypes();
        }

        private static void OnExportDirectories() => GameDatabase.ExportDirectories();
        private static void OnExportCurves() => GameDatabase.ExportCurves();
        private static void OnExportAssetTypes() => GameDatabase.ExportAssetTypes();
        private static void OnExportBlueprints() => GameDatabase.ExportBlueprints();
        private static void OnExportPrototypes() => GameDatabase.ExportPrototypes();

        private static void OnExportResources()
        {
            OnExportCells();
            OnExportDistricts();
            OnExportEncounters();
            OnExportProps();
            OnExportPropSets();
            OnExportUIs();
        }

        private static void OnExportCells() => GameDatabase.ExportCells();
        private static void OnExportDistricts() => GameDatabase.ExportDistricts();
        private static void OnExportEncounters() => GameDatabase.ExportEncounters();
        private static void OnExportProps() => GameDatabase.ExportProps();
        private static void OnExportPropSets() => GameDatabase.ExportPropSets();
        private static void OnExportUIs() => GameDatabase.ExportUIs();

        private static void OnExportLocales() => GameDatabase.ExportLocales();

        private static void OnExportPrototypeEnums() => GameDatabase.ExportPrototypeEnums();
    }
}
