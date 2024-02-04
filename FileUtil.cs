using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace callstack
{
    static class FileUtil
    {
        /* This function returns the line number of a specific index inside a textfile */
        public static int GetLineNumber(string file, int index)
        {
            string text = File.ReadAllText(file);
            int countNewLine = 1;
            if (text.Length > 1)
            {
                for (int i = 0; i < index; i++)
                {
                    if (text[i] == LanguageConventions.newLine[1])
                    { // text[i + 1]
                        countNewLine++;
                    }
                }
            }
            return countNewLine;
        }

        static public List<string> GetFiles(string path, List<string> extensions, bool searchSubDirectories)
        {
            string[] allFiles = null;
            List<string> files = new List<string>(); // create an Array of search Files filtered by search pattern

            if (searchSubDirectories)
                allFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            else
                allFiles = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < allFiles.Length; i++)
            {
                string fileToLower = allFiles[i].ToLower();
                foreach (string ext in extensions)
                {
                    if (fileToLower.EndsWith("." + ext.ToLower()))
                        files.Add(allFiles[i]);
                }
            }
            return files;
        }
    }
}
