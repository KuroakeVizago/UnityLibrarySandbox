
namespace Vizago.IO
{
    
    using System.IO;
    
    public static class BinaryReaderExtension
    {
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            using var ms = new MemoryStream();
            reader.BaseStream.CopyTo(ms);
            return ms.ToArray();
        }

        public static void ReadAllBytes(this BinaryReader reader, out byte[] bytesInfo)
        {
            using var ms = new MemoryStream();
            reader.BaseStream.CopyTo(ms);
            bytesInfo = ms.ToArray();
        }

        [System.Obsolete("This method is deprecated, use ReadAllBytes() instead")]
        public static byte[] GetAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }
        }
        
    }
}
