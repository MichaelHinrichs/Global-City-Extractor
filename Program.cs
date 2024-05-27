using System;
using System.IO;
using System.IO.Compression;

namespace Global_City_Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryReader br = new(File.OpenRead(args[0]));

            if (new string(br.ReadChars(3)) != "NxP")
                throw new Exception("This is not a nxp file.");

            br.BaseStream.Position = 8;
            int n = 0;
            string path = Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]);
            Directory.CreateDirectory(path);
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                int nameStart = (int)(br.BaseStream.Position + br.ReadByte());
                byte nameSize = br.ReadByte();
                int start = br.ReadInt32();
                int size = br.ReadInt32() - 4;
                int isCompressed = br.ReadInt32();//6
                if (br.BaseStream.Position != nameStart)
                    throw new Exception("Fuck!");

                string name = new(br.ReadChars(nameSize));
                br.ReadInt32();

                FileStream fs = File.Create(path + "\\" + n);
                if (isCompressed == 6)
                {
                    br.ReadInt16();
                    using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes(size - 2)), CompressionMode.Decompress))
                        ds.CopyTo(fs);
                }
                else
                {
                    BinaryWriter bw = new(fs);
                    bw.Write(br.ReadBytes(size));
                    bw.Close();
                }

                fs.Close();
                n++;
            }
        }
    }
}
