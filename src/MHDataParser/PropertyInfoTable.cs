namespace MHDataParser
{
    public enum PropertyDataType
    {
        Boolean,
        Real,
        Integer,
        Prototype,
        Curve,
        Asset,
        EntityId,
        Time,
        Guid,
        RegionId,
        Int21Vector3
    }

    public readonly struct PropertyInfo
    {
        public readonly string Name;
        public readonly PropertyDataType DataType;

        public PropertyInfo(string name, PropertyDataType dataType)
        {
            Name = name;
            DataType = dataType;
        }
    }

    public class PropertyInfoTable
    {
        private readonly List<PropertyInfo> _infoList = new();

        public void Add(string name, PropertyDataType dataType)
        {
            _infoList.Add(new(name, dataType));
        }

        public void SaveToFile(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            using (FileStream fileStream = File.OpenWrite(filePath))
            using (StreamWriter writer = new(fileStream))
            {
                foreach (var info in _infoList.OrderBy(info => info.Name))
                    writer.WriteLine($"{info.Name}\t{info.DataType}");
            }
        }
    }
}
