using System;
using System.IO;
using System.Diagnostics;

namespace RegistrySettings
{
    class Program
    {
        // add to registry the key 'hashcheck' for all files in order to compute a hash
        static void Main(string[] args)
        {
            // filename of reg-file which must be added
            const string regFileName = "AddContextMenu.reg";

            try
            {
                // paths to icon and exe-file
                string pathToIco = Path.Combine(Environment.CurrentDirectory, "Hash.ico").Replace("\\", "\\\\");
                string pathToExe = Path.Combine(Environment.CurrentDirectory, "HashCheck.exe %1").Replace("\\", "\\\\").Replace("%1", "\\\"%1\\\"");

                // read the template of reg-file
                string regFile = File.ReadAllText(regFileName);

                // set the value of Icon
                int iPosIcon = regFile.IndexOf("=", regFile.IndexOf("Icon"));
                regFile = regFile.Insert(iPosIcon + 1, string.Format($"\"{pathToIco}\""));

                // set the value of Exe-program
                int iPosExe = regFile.IndexOf("=", regFile.IndexOf("command"));
                regFile = regFile.Insert(iPosExe + 1, string.Format($"\"{pathToExe}\""));

                // overwrite reg-file with correct paths
                File.WriteAllText(regFileName, regFile);

                // run reg-file and add keys to regedit
                Process.Start(regFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}
