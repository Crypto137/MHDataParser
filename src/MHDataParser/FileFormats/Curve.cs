namespace MHDataParser.FileFormats
{
    public class Curve
    {
        public CalligraphyHeader Header { get; }
        public int StartPosition { get; }
        public int EndPosition { get; }
        public double[] Values { get; }

        public Curve(byte[] data)
        {
            using (MemoryStream stream = new(data))
            using (BinaryReader reader = new(stream))
            {
                Header = new(reader);
                StartPosition = reader.ReadInt32();
                EndPosition = reader.ReadInt32();

                Values = new double[EndPosition - StartPosition + 1];
                for (int i = 0; i < Values.Length; i++)
                    Values[i] = reader.ReadDouble();
            }
        }
    }
}
