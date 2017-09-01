using System;
using System.Linq;
using System.Text;
using System.IO;
using Transfer_File_Across_Network;

namespace hexConverter
{
    class fromHex
    {
        public static void convertfromHex(string filename)
        {
            var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            //convertHexStringToFile();
        }
        public static void convertHexStringToFile(string hexdata, FileStream output)
        {
            var twoCharacterBuffer = new StringBuilder();
            var oneByte = new byte[1];
            foreach (var character in hexdata.Where(c => c != ' '))
            {
                twoCharacterBuffer.Append(character);

                if (twoCharacterBuffer.Length == 2)
                {
                    oneByte[0] = (byte)Convert.ToByte(twoCharacterBuffer.ToString(), 16);
                    output.Write(oneByte, 0, 1);
                    twoCharacterBuffer.Clear();
                }
            }
        }
    }
    class toHex
    {
        public static void fileToHex(string Filename)
        {
            string filepath = Directory.GetCurrentDirectory() + Program.DecideWhichSlash() + Filename;
            //Get number of characters in file
            var fileInfo = new FileInfo(filepath);
            long filelength = fileInfo.Length;

            //Open file as byte array
            byte[] rawData = File.ReadAllBytes(filepath);

            //Convert file to hex
            string hex = BitConverter.ToString(rawData);
            hex = hex.Replace("-", ""); //remove all '-'s. This is necessary to prevent corruption.

            //Save to temporary file called (filename_with_extension).temp
            try
            {
                File.Delete(filepath + ".send.temp");
                File.WriteAllText(filepath + ".send.temp", hex);
            }
            catch (FileNotFoundException)
            {
                File.WriteAllText(filepath + ".send.temp", hex);
            }
        }
    }
}
