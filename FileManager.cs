using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace KeyBinder
{
    class FileManager
    {
        private static string filePath = @"Resources/hotkeys.txt";

        public static List<string[]> getFileContent()
        {
            HashSet<string[]> fileContent = new HashSet<string[]>();
            

            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine("");
                    sw.Flush();
                    sw.Close();
                }
            }

            List<string> lines = File.ReadAllLines(filePath).ToList();

            foreach (string line in lines)
            {
                string[] entries = line.Split(',');

                if (entries.Count() == 4)
                {
                    fileContent.Add(entries);
                }
            }

            return fileContent.ToList();
        }

        public static void saveDataToFile(ViewModel viewModel)
        {
            using (StreamWriter sw = new StreamWriter(File.Open(filePath, FileMode.Create)))
            {
                try
                {
                    foreach (Hotkey hotkey in viewModel.DataGridItems)
                    {
                        if (!string.IsNullOrEmpty(hotkey.hotkey))
                        {
                            sw.WriteLine(hotkey.toCSV());
                        }
                    }
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                }
            }
        }
    }
}
