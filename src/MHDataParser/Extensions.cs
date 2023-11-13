using System.Text;

namespace MHDataParser
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        public static string ToHexString(this byte[] byteArray)
        {
            return byteArray.Aggregate("", (current, b) => current + b.ToString("X2"));
        }

        public static string ReadFixedString16(this BinaryReader reader)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadUInt16()));
        }

        public static string ReadFixedString32(this BinaryReader reader)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
        }

        /// <summary>
        /// Read a null-terminated string at the current position.
        /// </summary>
        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            StringBuilder sb = new();

            while (true)
            {
                byte b = reader.ReadByte();
                if (b == 0x00) break;
                sb.Append((char)b);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Read a null-terminated string at the specified offset.
        /// </summary>
        public static string ReadNullTerminatedString(this BinaryReader reader, long offset)
        {
            long pos = reader.BaseStream.Position;              // Remember the current position
            reader.BaseStream.Seek(offset, 0);                  // Move to the offset
            string result = reader.ReadNullTerminatedString();  // Read the string
            reader.BaseStream.Seek(pos, 0);                     // Return to the original position
            return result;
        }
    }
}
