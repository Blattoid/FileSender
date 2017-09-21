using System;
using System.IO;
using System.Configuration;
using fileTransfer;
using hexConverter;

namespace Transfer_File_Across_Network
{
    class global_variables
    {
        public static string server_addr;
        public static int server_port;
        public static int use_port;

        //Detects if we are running on Linux. This is referenced by DecideWhichSlash().
        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
    }
    class Program
    {
        public static string DecideWhichSlash()
        {
            //Extremely simple; checks is IsLinux is true and returns the appropriate slash combination. This is to heed the filesystem structures.
            if (global_variables.IsLinux)
            {
                return "/";
            }
            else
            {
                return "\\";
            }
        }
        static void Main(string[] args)
        {
            try
            {
                //Read the configuration file for settings. We use public strings to make it avaliable outside of 'try'.
                global_variables.server_addr = ConfigurationManager.AppSettings.Get("server_address");
                global_variables.server_port = Int32.Parse(ConfigurationManager.AppSettings.Get("server_port"));
                global_variables.use_port = Int32.Parse(ConfigurationManager.AppSettings.Get("use_port"));
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error reading configuration file.\n\n" + e.Message + "\n\nPress any key to exit");
                Console.ReadKey();
                Environment.Exit(1);
            }

            //Let us begin.
            //Fancy title
            Console.WriteLine(@"___________.__.__           _________                  .___            ");
            Console.WriteLine(@"\_   _____/|__|  |   ____  /   _____/ ____   ____    __| _/___________ ");
            Console.WriteLine(@" |    __)  |  |  | _/ __ \ \_____  \_/ __ \ /    \  / __ |/ __ \_  __ \");
            Console.WriteLine(@" |     \   |  |  |_\  ___/ /        \  ___/|   |  \/ /_/ \  ___/|  | \/");
            Console.WriteLine(@" \___  /   |__|____/\___  >_______  /\___  >___|  /\____ |\___  >__|   ");
            Console.WriteLine(@"     \/                 \/        \/     \/     \/      \/    \/       ");
            Console.WriteLine(); //Newline to sparate title from instructions.

            Console.Write("Do you wish to send or recieve? (S/R) ");
            string usrinput = Convert.ToString(Console.ReadKey().KeyChar); //Read key input, then convert from char to string.
            Console.Write(Environment.NewLine); //newline

            usrinput = usrinput.ToUpper(); //convert to uppercase
            char formatted_input = Convert.ToChar(usrinput); //and back to char again

            if (Convert.ToString(formatted_input) == "S")
            {
                //list the files in the folder and by extension the files you can send.
                Console.WriteLine();
                foreach (string temp in Directory.GetFiles(Directory.GetCurrentDirectory()))
                {
                    string file = Path.GetFileName(temp);
                    Console.WriteLine(file);
                } 
                Console.Write("\nPlease enter file name to send: ");
                usrinput = Console.ReadLine();

                if (File.Exists(usrinput))
                {
                    //Check if file is over 715MB since that is the limit for loading files into memory. If so, report the file is too big. If not, continue as planned.
                    Int64 fileSize = new FileInfo(usrinput).Length;
                    if (fileSize < 715500000) { Actions.Send(usrinput); }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("File exceeds maximum size of 715MB. Sorry about that.");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("File does not exist.");
                }

            }
            else if (Convert.ToString(formatted_input) == "R")
            {
                Console.Write("Please enter destination file name: ");
                usrinput = Console.ReadLine();
                if (File.Exists(usrinput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("File already exists. Overwrite? (Y/N) ");

                    string usrinput2 = Convert.ToString(Console.ReadKey().KeyChar); //Readk key input, then convert from char to string.
                    Console.Write(Environment.NewLine); //newline
                    Console.ForegroundColor = ConsoleColor.Gray;

                    usrinput2 = usrinput2.ToUpper(); //convert to uppercase
                    char formatted_input2 = Convert.ToChar(usrinput2); //and back to char again

                    if (Convert.ToString(formatted_input2) == "Y")
                    {
                        Actions.Recv(usrinput);
                    }
                }
                else
                {
                    Actions.Recv(usrinput);
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Action completed. Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
    class Actions
    {
        public static void Send(string target_file)
        {
            Console.WriteLine("Sending file '" + target_file + "' to '" + Convert.ToString(global_variables.server_addr) + "' on port " + Convert.ToString(global_variables.server_port) + "");
            //toHex calls from hexConvert.cs#
            toHex.fileToHex(target_file); //convert target_file to hex

            //Create data stream
            string tempfile = (Directory.GetCurrentDirectory() + Program.DecideWhichSlash() + target_file + ".send.temp");

            TcpClients.Upload(global_variables.server_addr, global_variables.server_port, tempfile); //convert target_file to hex then send it to server

            //cleanup the temp file
            try { File.Delete(tempfile); }
            catch (IOException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error cleaning up temporary file '"+tempfile+"'.");
            }
        }
        public static void Recv(string destination_file)
        {

            Console.WriteLine("Destination file is '" + destination_file + "'.");
            string tempfile = Directory.GetCurrentDirectory() + Program.DecideWhichSlash() + destination_file + ".recv.temp";

            //Recieve file
            TcpClients.RecieveData(global_variables.use_port, tempfile);
            //fromHex calls from hexConvert.cs
            var stream = new FileStream(destination_file, FileMode.Append, FileAccess.Write);
            fromHex.convertHexStringToFile(File.ReadAllText(tempfile), stream);

            stream.Close();

            //cleanup the temp file
            File.Delete(tempfile);

        }
    }
}
