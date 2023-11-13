using System.Diagnostics;
using System.Text.Json;
using MHDataParser.FileFormats;
using MHDataParser.JsonOutput;

namespace MHDataParser
{
    public static class GameDatabase
    {
        private static readonly string PakDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        private static readonly string CalligraphyPath = Path.Combine(PakDirectory, "Calligraphy.sip");
        private static readonly string ResourcePath = Path.Combine(PakDirectory, "mu_cdata.sip");
        private static readonly string OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Output");

        private static readonly Dictionary<StringId, AssetTypeId> _assetIdToAssetTypeDict = new();

        private static readonly PakFile _calligraphyPak;
        private static readonly PakFile _resourcePak;

        private static JsonSerializerOptions _jsonOptions;

        public static bool IsInitialized { get; }

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

        public static Dictionary<string, CellPrototype> CellDict { get; } = new();
        public static Dictionary<string, DistrictPrototype> DistrictDict { get; } = new();
        public static Dictionary<string, EncounterPrototype> EncounterDict { get; } = new();
        public static Dictionary<string, PropSetPrototype> PropSetDict { get; } = new();
        public static Dictionary<string, PropPrototype> PropDict { get; } = new();
        public static Dictionary<string, UIPrototype> UIDict { get; } = new();

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
            }
            Console.WriteLine($"Parsed {BlueprintDict.Count} blueprints");

            // Load Calligraphy prototypes
            foreach (PrototypeRecord record in PrototypeDirectory.Records)
            {
                PrototypeFile prototypeFile = new(_calligraphyPak.GetFile($"Calligraphy/{record.FilePath}"));
                PrototypeDict.Add(record.FilePath, prototypeFile);
            }
            Console.WriteLine($"Parsed {PrototypeDict.Count} Calligraphy prototypes");

            // Load resource prototypes
            foreach (PakEntry entry in _resourcePak.Entries)
            {
                PrototypeRefManager.AddDataRef((PrototypeId)HashHelper.HashPath($"&{entry.FilePath.ToLower()}"), entry.FilePath);

                switch (Path.GetExtension(entry.FilePath))
                {
                    case ".cell":
                        CellDict.Add(entry.FilePath, new(entry.Data));
                        break;
                    case ".district":
                        DistrictDict.Add(entry.FilePath, new(entry.Data));
                        break;
                    case ".encounter":
                        EncounterDict.Add(entry.FilePath, new(entry.Data));
                        break;
                    case ".propset":
                        PropSetDict.Add(entry.FilePath, new(entry.Data));
                        break;
                    case ".prop":
                        PropDict.Add(entry.FilePath, new(entry.Data));
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
                    foreach (double value in CurveDict[record.FilePath].Values)
                        sw.WriteLine(value);
                }
            }

            Console.WriteLine("Done");
        }

        public static void ExportAssetTypes()
        {
            Console.WriteLine("Exporting asset types...");
            SerializeDictAsJson(AssetTypeDict, true);
            Console.WriteLine("Done");
        }

        public static void ExportBlueprints()
        {
            Console.WriteLine("Exporting blueprints...");
            SerializeDictAsJson(BlueprintDict, true);
            Console.WriteLine("Done");
        }

        public static void ExportPrototypes()
        {
            Console.WriteLine("Exporting prototypes...");
            SerializeDictAsJson(PrototypeDict, true);
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
            SerializeDictAsJson(PrototypeDict);
            Console.WriteLine("Done");
        }

        public static void ExportUIs()
        {
            Console.WriteLine("Exporting UIs...");
            SerializeDictAsJson(UIDict);
            Console.WriteLine("Done");
        }

        private static void SerializeDictAsJson<T>(Dictionary<string, T> dict, bool addCalligraphyPrefix = false)
        {
            if (_jsonOptions == null) InitializeJsonOptions();

            foreach (var kvp in dict)
            {
                string path = addCalligraphyPrefix
                    ? Path.Combine(OutputDirectory, "Parsed", "Calligraphy", $"{kvp.Key}.json")
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
                MaxDepth = 128          // 64 is not enough for prototypes
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
