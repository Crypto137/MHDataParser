using System.Globalization;

namespace SqliteGpakRepacker
{
    public class SqliteVersion
    {
        public float Version { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Version.ToString(CultureInfo.InvariantCulture)} - {Description}";
        }
    }
}
