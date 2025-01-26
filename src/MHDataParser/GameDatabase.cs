using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using MHDataParser.CodeGeneration;
using MHDataParser.FileFormats;
using MHDataParser.JsonOutput;

namespace MHDataParser
{
    public static class GameDatabase
    {
        private static readonly string PakDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        private static readonly string CalligraphyPath = Path.Combine(PakDirectory, "Calligraphy.sip");
        private static readonly string ResourcePath = Path.Combine(PakDirectory, "mu_cdata.sip");
        private static readonly string LocoPath = Path.Combine(PakDirectory, "Loco");
        private static readonly string OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Output");

        private static readonly Dictionary<StringId, AssetTypeId> _assetIdToAssetTypeDict = new();

        private static readonly HashSet<BlueprintId> _propertyMixinBlueprints = new();
        private static readonly List<string> _propertyInfoPrototypes = new();

        private static readonly PakFile _calligraphyPak;
        private static readonly PakFile _resourcePak;

        private static JsonSerializerOptions _jsonOptions;

        public static bool IsInitialized { get; }

        // Calligraphy

        public static DataDirectory CurveDirectory { get; }
        public static DataDirectory AssetDirectory { get; }
        public static DataDirectory BlueprintDirectory { get; }
        public static DataDirectory PrototypeDirectory { get; }
        public static DataDirectory ReplacementDirectory { get; }

        public static DataRefManager<CurveId> CurveRefManager { get; } = new(true);
        public static DataRefManager<AssetTypeId> AssetTypeRefManager { get; } = new(true);
        public static DataRefManager<BlueprintId> BlueprintRefManager { get; } = new(true);
        public static DataRefManager<PrototypeId> PrototypeRefManager { get; } = new(true);
        public static DataRefManager<StringId> StringRefManager { get; } = new(false);

        public static Dictionary<string, Curve> CurveDict { get; } = new();
        public static Dictionary<string, AssetType> AssetTypeDict { get; } = new();
        public static Dictionary<string, Blueprint> BlueprintDict { get; } = new();
        public static Dictionary<string, PrototypeFile> PrototypeDict { get; } = new();

        // Resources

        public static Dictionary<string, CellPrototype> CellDict { get; } = new();
        public static Dictionary<string, DistrictPrototype> DistrictDict { get; } = new();
        public static Dictionary<string, EncounterPrototype> EncounterDict { get; } = new();
        public static Dictionary<string, PropSetPrototype> PropSetDict { get; } = new();
        public static Dictionary<string, PropPackagePrototype> PropDict { get; } = new();
        public static Dictionary<string, UIPrototype> UIDict { get; } = new();

        // Localization

        public static Dictionary<string, Locale> LocaleDict { get; } = new();

        static GameDatabase()
        {
            // Make sure sip files are present
            if (File.Exists(CalligraphyPath) == false || File.Exists(ResourcePath) == false)
            {
                Console.WriteLine($"Calligraphy.sip and/or mu_cdata.sip are missing! Make sure you copied these files to {PakDirectory}.");
                IsInitialized = false;
                return;
            }

            Console.WriteLine("Initializing game database...");
            var stopwatch = Stopwatch.StartNew();

            // Load paks
            _calligraphyPak = new(CalligraphyPath);
            _resourcePak = new(ResourcePath);

            // Load directories
            CurveDirectory = new(_calligraphyPak.GetFile("Calligraphy/Curve.directory"));
            AssetDirectory = new(_calligraphyPak.GetFile("Calligraphy/Type.directory"));
            BlueprintDirectory = new(_calligraphyPak.GetFile("Calligraphy/Blueprint.directory"));
            PrototypeDirectory = new(_calligraphyPak.GetFile("Calligraphy/Prototype.directory"));
            ReplacementDirectory = new(_calligraphyPak.GetFile("Calligraphy/Replacement.directory"));

            // Load curves
            foreach (CurveRecord record in CurveDirectory.Records)
            {
                Curve curve = new(_calligraphyPak.GetFile($"Calligraphy/{record.FilePath}"));
                CurveDict.Add(record.FilePath, curve);
            }
            Console.WriteLine($"Parsed {CurveDict.Count} curves");

            // Load assets
            foreach (AssetTypeRecord record in AssetDirectory.Records)
            {
                AssetType type = new(_calligraphyPak.GetFile($"Calligraphy/{record.FilePath}"), record.Id);
                AssetTypeDict.Add(record.FilePath, type);
            }
            Console.WriteLine($"Parsed {AssetDirectory.Records.Length} assets of {_assetIdToAssetTypeDict.Count} types");

            // Load blueprints
            foreach (BlueprintRecord record in BlueprintDirectory.Records)
            {
                Blueprint blueprint = new(_calligraphyPak.GetFile($"Calligraphy/{record.FilePath}"));
                BlueprintDict.Add(record.FilePath, blueprint);

                // Add to property mixin lookup if needed
                if (record.FilePath.StartsWith("Property/Mixin/", StringComparison.Ordinal))
                    _propertyMixinBlueprints.Add(record.Id);

                // Property info
                if (record.FilePath.StartsWith("Property/Info/", StringComparison.Ordinal))
                    _propertyInfoPrototypes.Add(record.FilePath.Replace(".blueprint", ".defaults"));
            }

            Console.WriteLine($"Found {_propertyMixinBlueprints.Count} property mixin blueprints");
            Console.WriteLine($"Parsed {BlueprintDict.Count} blueprints");

            // Load Calligraphy prototypes
            foreach (PrototypeRecord record in PrototypeDirectory.Records)
            {
                try
                {
                    PrototypeFile prototypeFile = new(_calligraphyPak.GetFile($"Calligraphy/{record.FilePath}"));
                    PrototypeDict.Add(record.FilePath, prototypeFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse {record.FilePath}: {e.Message}");
                }

            }
            Console.WriteLine($"Parsed {PrototypeDict.Count} Calligraphy prototypes");

            // Ugly goto skip for unsupported file formats for now
            bool useLegacyFormat = PrototypeDirectory.Header.Version == 10;
            if (useLegacyFormat)
                Console.WriteLine("Legacy format detected");

            // Load resource prototypes
            foreach (PakEntry entry in _resourcePak.Entries)
            {
                PrototypeRefManager.AddDataRef((PrototypeId)HashHelper.HashPath($"&{entry.FilePath.ToLower()}"), entry.FilePath);

                switch (Path.GetExtension(entry.FilePath))
                {
                    case ".cell":
                        CellDict.Add(entry.FilePath, new(entry.Data, useLegacyFormat));
                        break;
                    case ".district":
                        DistrictDict.Add(entry.FilePath, new(entry.Data, useLegacyFormat));
                        break;
                    case ".encounter":
                        EncounterDict.Add(entry.FilePath, new(entry.Data, useLegacyFormat));
                        break;
                    case ".propset":
                        PropSetDict.Add(entry.FilePath, new(entry.Data));
                        break;
                    case ".prop":
                        PropDict.Add(entry.FilePath, new(entry.Data, useLegacyFormat));
                        break;
                    case ".ui":
                        UIDict.Add(entry.FilePath, new(entry.Data));
                        break;
                }
            }

            Console.WriteLine($"Parsed {CellDict.Count} cell prototypes");
            Console.WriteLine($"Parsed {DistrictDict.Count} district prototypes");
            Console.WriteLine($"Parsed {EncounterDict.Count} encounter prototypes");
            Console.WriteLine($"Parsed {PropSetDict.Count} prop set prototypes");
            Console.WriteLine($"Parsed {PropDict.Count} prop prototypes");
            Console.WriteLine($"Parsed {UIDict.Count} UI prototypes");

            // Load locales
            if (useLegacyFormat == false)
            {
                Console.WriteLine($"Loading locales...");

                foreach (string localePath in Directory.GetFiles(LocoPath))
                {
                    if (Path.GetExtension(localePath) != ".locale")
                    {
                        Console.WriteLine($"Found unknown file {Path.GetFileName(localePath)} in the Loco directory! Skipping...");
                        continue;
                    }

                    Locale locale = new(File.ReadAllBytes(localePath));
                    Console.WriteLine($"Detected locale: {locale.Name}");

                    // Load string files for this locale
                    string stringFileDir = Path.Combine(LocoPath, locale.Directory);

                    foreach (string stringFilePath in Directory.GetFiles(stringFileDir))
                    {
                        if (Path.GetExtension(stringFilePath) != ".string")
                        {
                            Console.WriteLine($"Found unknown file {Path.GetFileName(localePath)} in the string file directory for locale {locale.Name}! Skipping...");
                            continue;
                        }

                        StringFile stringFile = new(File.ReadAllBytes(stringFilePath));
                        locale.AddStringFile(stringFile);
                    }

                    LocaleDict.Add(Path.GetFileName(localePath), locale);
                }

                Console.WriteLine($"Loaded {LocaleDict.Count} locales");
            }
            else
            {
                Console.WriteLine("Legacy locales are not supported, skipping");
            }

            // Finish game database initialization
            stopwatch.Stop();
            Console.WriteLine($"Finished initializing game database in {stopwatch.ElapsedMilliseconds} ms");
            IsInitialized = true;
        }

        public static string GetAssetName(StringId assetId) => StringRefManager.GetReferenceName(assetId);
        public static string GetAssetTypeName(AssetTypeId assetTypeId) => AssetTypeRefManager.GetReferenceName(assetTypeId);
        public static string GetCurveName(CurveId curveId) => CurveRefManager.GetReferenceName(curveId);
        public static string GetBlueprintName(BlueprintId blueprintId) => BlueprintRefManager.GetReferenceName(blueprintId);
        public static string GetBlueprintFieldName(StringId fieldId) => StringRefManager.GetReferenceName(fieldId);
        public static string GetPrototypeName(PrototypeId prototypeId) => PrototypeRefManager.GetReferenceName(prototypeId);

        public static bool IsPropertyMixinBlueprint(BlueprintId blueprintId)
        {
            return _propertyMixinBlueprints.Contains(blueprintId);
        }

        public static void AddAssetIdLookup(StringId assetId, AssetTypeId assetTypeId)
        {
            _assetIdToAssetTypeDict.Add(assetId, assetTypeId);
        }

        public static AssetTypeId GetAssetTypeId(StringId assetId)
        {
            if (assetId == StringId.Invalid) return AssetTypeId.Invalid;
            return _assetIdToAssetTypeDict[assetId];
        }

        #region Export

        public static void ExtractPak(bool extractEntries, bool extractData)
        {
            if (Directory.Exists(OutputDirectory) == false) Directory.CreateDirectory(OutputDirectory);

            if (extractEntries)
            {
                Console.WriteLine("Extracting Calligraphy entries...");
                _calligraphyPak.ExtractEntries(Path.Combine(OutputDirectory, "Calligraphy.tsv"));
                Console.WriteLine("Extracting Resource entries...");
                _resourcePak.ExtractEntries(Path.Combine(OutputDirectory, "mu_cdata.tsv"));
            }

            if (extractData)
            {
                Console.WriteLine("Extracting Calligraphy data...");
                _calligraphyPak.ExtractData(OutputDirectory);
                Console.WriteLine("Extracting Resource data...");
                _resourcePak.ExtractData(OutputDirectory);
            }

            Console.WriteLine("Done");
        }

        public static void ExportDirectories()
        {
            Console.WriteLine("Exporting directories...");

            string dir = Path.Combine(OutputDirectory, "Parsed", "Calligraphy");
            if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);

            using (StreamWriter writer = new(Path.Combine(dir, "Curve.directory.tsv")))
            {
                foreach (CurveRecord record in CurveDirectory.Records)
                    writer.WriteLine($"{record.Id}\t{record.Guid}\t{record.Flags}\t{record.FilePath}");
            }

            using (StreamWriter writer = new(Path.Combine(dir, "Type.directory.tsv")))
            {
                foreach (AssetTypeRecord record in AssetDirectory.Records)
                    writer.WriteLine($"{record.Id}\t{record.Guid}\t{record.Flags}\t{record.FilePath}");
            }

            using (StreamWriter writer = new(Path.Combine(dir, "Blueprint.directory.tsv")))
            {
                foreach (BlueprintRecord record in BlueprintDirectory.Records)
                    writer.WriteLine($"{record.Id}\t{record.Guid}\t{record.Flags}\t{record.FilePath}");
            }

            using (StreamWriter writer = new(Path.Combine(dir, "Prototype.directory.tsv")))
            {
                foreach (PrototypeRecord record in PrototypeDirectory.Records)
                    writer.WriteLine($"{record.Id}\t{record.Guid}\t{record.BlueprintId}\t{record.Flags}\t{record.FilePath}");
            }

            using (StreamWriter writer = new(Path.Combine(dir, "Replacement.directory.tsv")))
            {
                foreach (ReplacementRecord record in ReplacementDirectory.Records)
                    writer.WriteLine($"{record.OldGuid}\t{record.NewGuid}\t{record.Name}");
            }

            Console.WriteLine("Done");
        }

        public static void ExportCurves()
        {
            Console.WriteLine("Exporting curves...");

            foreach (CurveRecord record in CurveDirectory.Records)  // use TSV for curves
            {
                string path = Path.Combine(OutputDirectory, "Parsed", "Calligraphy", $"{record.FilePath}.tsv");
                string dir = Path.GetDirectoryName(path);
                if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);

                using (StreamWriter sw = new(path))
                {
                    Curve curve = CurveDict[record.FilePath];
                    for (int i = 0; i < curve.Values.Length; i++)
                        sw.WriteLine($"{curve.StartPosition + i}\t{curve.Values[i]}");
                }
            }

            Console.WriteLine("Done");
        }

        public static void ExportAssetTypes()
        {
            Console.WriteLine("Exporting asset types...");
            SerializeDictAsJson(AssetTypeDict, "Calligraphy");
            Console.WriteLine("Done");
        }

        public static void ExportBlueprints()
        {
            Console.WriteLine("Exporting blueprints...");
            SerializeDictAsJson(BlueprintDict, "Calligraphy");
            Console.WriteLine("Done");
        }

        public static void ExportPrototypes()
        {
            Console.WriteLine("Exporting prototypes...");
            SerializeDictAsJson(PrototypeDict, "Calligraphy");
            Console.WriteLine("Done");
        }

        public static void ExportCells()
        {
            Console.WriteLine("Exporting cells...");
            SerializeDictAsJson(CellDict);
            Console.WriteLine("Done");
        }

        public static void ExportDistricts()
        {
            Console.WriteLine("Exporting districts...");
            SerializeDictAsJson(DistrictDict);
            Console.WriteLine("Done");
        }

        public static void ExportEncounters()
        {
            Console.WriteLine("Exporting encounters...");
            SerializeDictAsJson(EncounterDict);
            Console.WriteLine("Done");
        }

        public static void ExportProps()
        {
            Console.WriteLine("Exporting props...");
            SerializeDictAsJson(PropDict);
            Console.WriteLine("Done");
        }

        public static void ExportPropSets()
        {
            Console.WriteLine("Exporting prop sets...");
            SerializeDictAsJson(PropSetDict);
            Console.WriteLine("Done");
        }

        public static void ExportUIs()
        {
            Console.WriteLine("Exporting UIs...");
            SerializeDictAsJson(UIDict);
            Console.WriteLine("Done");
        }

        public static void ExportLocales()
        {
            Console.WriteLine("Exporting locales...");
            SerializeDictAsJson(LocaleDict, "Loco");
            Console.WriteLine("Done");
        }

        public static void ExportAssetRefs()
        {
            List<StringId> assetRefList = new();
            foreach (AssetType assetType in AssetTypeDict.Values)
            {
                foreach (Asset asset in assetType.Assets)
                    assetRefList.Add(asset.Id);
            }

            assetRefList.Sort();

            using (FileStream fileStream = File.OpenWrite(Path.Combine(OutputDirectory, "Assets.tsv")))
            using (StreamWriter writer = new(fileStream))
            {
                foreach (StringId assetRef in assetRefList)
                    writer.WriteLine($"{(ulong)assetRef}\t{GetAssetName(assetRef)}");
            }

            Console.WriteLine($"Exported {assetRefList.Count} asset data references");
        }

        public static void ExportPrototypeEnums()
        {
            Console.WriteLine("Exporting prototype enums...");

            List<PrototypeId> prototypeEnumList = new(PrototypeRefManager.Count + 1) { 0 };
            List<PrototypeId> entityPrototypeEnumList = new() { 0 };
            List<PrototypeId> inventoryPrototypeEnumList = new() { 0 };
            List<PrototypeId> powerPrototypeEnumList = new() { 0 };

            // Get all prototype ids and sort them to get enum for the base Prototype class
            foreach (var kvp in PrototypeRefManager)
                prototypeEnumList.Add(kvp.Key);

            prototypeEnumList.Sort();

            // Populate lists for EntityPrototype, InventoryPrototype, and PowerPrototype subclasses
            foreach (PrototypeId prototypeId in prototypeEnumList)
            {
                PrototypeRecord record = PrototypeDirectory.GetPrototypeRecord(prototypeId);
                if (record == null) continue;   // We don't need resource prototypes, so we just skip them

                string blueprintName = GetBlueprintName((BlueprintId)record.BlueprintId);
                if (BlueprintDict.TryGetValue(blueprintName, out Blueprint blueprint) == false)
                {
                    Console.WriteLine($"Failed to get blueprint for prototype id {prototypeId}");
                    continue;
                }

                string runtimeBinding = blueprint.RuntimeBinding;

                if (PrototypeHierarchyHelper.IsEntityPrototype(runtimeBinding))
                    entityPrototypeEnumList.Add(prototypeId);
                else if (PrototypeHierarchyHelper.IsInventoryPrototype(runtimeBinding))
                    inventoryPrototypeEnumList.Add(prototypeId);
                else if (PrototypeHierarchyHelper.IsPowerPrototype(runtimeBinding))
                    powerPrototypeEnumList.Add(prototypeId);
            }

            // Save output
            string dir = Path.Combine(OutputDirectory, "PrototypeEnums");
            if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);

            File.WriteAllLines(Path.Combine(dir, "Prototype.tsv"),
                prototypeEnumList.Select(id => ((ulong)id).ToString()));

            File.WriteAllLines(Path.Combine(dir, "EntityPrototype.tsv"),
                entityPrototypeEnumList.Select(id => ((ulong)id).ToString()));

            File.WriteAllLines(Path.Combine(dir, "InventoryPrototype.tsv"),
                inventoryPrototypeEnumList.Select(id => ((ulong)id).ToString()));

            File.WriteAllLines(Path.Combine(dir, "PowerPrototype.tsv"),
                powerPrototypeEnumList.Select(id => ((ulong)id).ToString()));

            Console.WriteLine(string.Format("Done, ref counts: Prototype = {0}, EntityPrototype = {1}, InventoryPrototype = {2}, PowerPrototype = {3}",
                prototypeEnumList.Count, entityPrototypeEnumList.Count, inventoryPrototypeEnumList.Count, powerPrototypeEnumList.Count));
        }

        public static void ExportPrototypeRuntimeBindings()
        {
            string filePath = Path.Combine(OutputDirectory, $"PrototypeRuntimeBindings.tsv");
            string dir = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);

            using (StreamWriter writer = new(filePath))
            {
                foreach (var record in PrototypeDirectory.Records)
                {
                    PrototypeRecord prototypeRecord = (PrototypeRecord)record;
                    Blueprint blueprint = BlueprintDict[GetBlueprintName((BlueprintId)prototypeRecord.BlueprintId)];
                    writer.WriteLine($"{(ulong)prototypeRecord.Id}\t{blueprint.RuntimeBinding}");
                }
            }

            Console.Write("Done");
        }

        public static void ExportBlueprintEnums()
        {
            // Test implementation, does not take blueprint hierarchy into account, but can be useful for standalone blueprints
            Console.WriteLine("Exporting blueprint enums...");

            Dictionary<BlueprintId, List<PrototypeId>> blueprintEnumDict = new();

            foreach (PrototypeRecord record in PrototypeDirectory.Records)
            {
                if (blueprintEnumDict.TryGetValue((BlueprintId)record.BlueprintId, out List<PrototypeId> list) == false)
                {
                    list = new() { PrototypeId.Invalid };
                    blueprintEnumDict.Add((BlueprintId)record.BlueprintId, list);
                }

                list.Add(record.Id);
            }

            foreach (var kvp in blueprintEnumDict)
            {
                kvp.Value.Sort();

                string filePath = Path.Combine(OutputDirectory, "BlueprintEnums", $"{GetBlueprintName(kvp.Key)}.tsv");
                string dir = Path.GetDirectoryName(filePath);
                if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);

                using (StreamWriter writer = new(filePath))
                {
                    foreach (PrototypeId id in kvp.Value)
                        writer.WriteLine($"{(ulong)id}\t{GetPrototypeName(id)}");
                }
            }

            Console.WriteLine("Done");
        }

        public static void GeneratePrototypeClasses()
        {
            Console.WriteLine("Generating prototype classes...");
            if (Directory.Exists(OutputDirectory) == false) Directory.CreateDirectory(OutputDirectory);
            string filePath = Path.Combine(OutputDirectory, "Prototypes.cs");
            PrototypeClassGenerator.Generate(filePath);
            Console.WriteLine("Done");
        }

        public static void ExportPropertyInfoTable()
        {
            PropertyInfoTable propertyInfoTable = new();

            foreach (string propertyInfoPrototypeName in _propertyInfoPrototypes)
            {
                if (PrototypeDict.TryGetValue(propertyInfoPrototypeName, out PrototypeFile prototypeFile) == false)
                {
                    Console.WriteLine($"Property info prototype {propertyInfoPrototypeName} not found");
                    continue;
                }

                foreach (PrototypeSimpleField field in prototypeFile.Prototype.FieldGroups[0].SimpleFields)
                {
                    if (GetBlueprintFieldName(field.Id) == "Type")
                    {
                        if (Enum.TryParse(GetAssetName((StringId)field.Value), out PropertyDataType dataType) == false)
                        {
                            Console.WriteLine($"Failed to parse data type for property info {propertyInfoPrototypeName}");
                            break;
                        }

                        propertyInfoTable.Add(Path.GetFileNameWithoutExtension(propertyInfoPrototypeName), dataType);
                        break;
                    }
                }
            }

            propertyInfoTable.SaveToFile(Path.Combine(OutputDirectory, "PropertyInfoTable.tsv"));
            Console.WriteLine("Done");
        }

        private static void SerializeDictAsJson<T>(Dictionary<string, T> dict, string prefix = "")
        {
            if (_jsonOptions == null) InitializeJsonOptions();

            foreach (var kvp in dict)
            {
                string path = prefix != ""
                    ? Path.Combine(OutputDirectory, "Parsed", prefix, $"{kvp.Key}.json")
                    : Path.Combine(OutputDirectory, "Parsed", $"{kvp.Key}.json");
                string dir = Path.GetDirectoryName(path);
                if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);

                File.WriteAllText(path, JsonSerializer.Serialize((object)kvp.Value, _jsonOptions));
            }
        }

        private static void InitializeJsonOptions()
        {
            _jsonOptions = new()
            {
                WriteIndented = true,
                MaxDepth = 256,                                         // 64 is not enough for prototypes
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)   // This is needed to export localized strings correctly
            };

            _jsonOptions.Converters.Add(new BlueprintConverter());
            _jsonOptions.Converters.Add(new PrototypeFileConverter());
            _jsonOptions.Converters.Add(new MarkerPrototypeConverter());
            _jsonOptions.Converters.Add(new NaviPatchPrototypeConverter());
            _jsonOptions.Converters.Add(new UIPanelPrototypeConverter());
        }

        #endregion
    }
}
