using System;
using System.IO;
using System.Text.RegularExpressions;

namespace callstack
{
    internal class ParseOptions 
    {
        enum ParseOption {
            None,
            SearchOptions,
            SourcePath,
            Extensions,
            Editor1,
            Editor2,
            Editor3,
            Fontsize,
            Color
        };
        private static int currentOption = 0; 
        
        internal static void ParseOptionsFromFile(string optionsFile, Options options) {
            string line;
            System.IO.StreamReader streamReader = null;
            try {
                streamReader = new StreamReader(optionsFile); // read options file
                while ((line = streamReader.ReadLine()) != null) { // Read the file line by line.
                    if (line.Equals("<Options>")) {
                        currentOption = (int)ParseOption.SearchOptions;
                        continue;
                    } else if (line.Equals("<SearchPath>")) {
                        currentOption = (int)ParseOption.SourcePath;
                        options.searchPaths.Clear();
                        continue;
                    } else if (line.Equals("<Extensions>")) {
                        currentOption = (int)ParseOption.Extensions;
                        continue;
                    } 
                    switch (currentOption) {
                        case (int)ParseOption.SearchOptions:
                            int indexOfEqual = line.IndexOf('=');
                            string lineTrimmed = line.Trim();
                            if (indexOfEqual != -1) {
                                string lineOption = lineTrimmed.Substring(0, indexOfEqual);
                                int optionId = 0;
                                foreach (Option o in options.GetList()) {
                                    if (o.ToString().Equals(lineOption)) {
                                        optionId = o.GetId();
                                        if (!lineTrimmed.Substring(indexOfEqual + 1).Equals("0"))
                                            options.SetValue(optionId, true);
                                    }
                                }
                            }
                            break;
                        case (int)ParseOption.SourcePath:
                            if (!line.Equals(""))
                                options.searchPaths.Add(line.TrimStart().TrimEnd());
                            break;
                        case (int)ParseOption.Extensions:
                            if (!line.Equals("")) {
                                string[] extensions = line.TrimStart().TrimEnd().Split(';');
                                if (extensions != null) {
                                    options.extensions.Clear();
                                    for (int i = 0; i < extensions.Length; i++) { /* cleanup extensions */
                                        if (!extensions[i].Equals("")) {
                                            if (extensions[i].Equals("*")) {
                                                options.extensions.Clear();
                                                options.extensions.Add("*"); // wildcard found, so do not filter extensions
                                                break;
                                            }
                                            Match match = Regex.Match(extensions[i], "^[a-zA-Z][a-zA-Z0-9]*$");
                                            if (match.Success) {
                                                options.extensions.Add(extensions[i]);
                                            }
                                        }
                                    }
                                    if (extensions.Length == 0)
                                        options.extensions.Add("*"); // use wildcard 
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            } catch (FileNotFoundException fnfe) { /* do not fetch options from file and use system default instead */
            } catch (Exception e) { /* ignore and use default options, if there are any exceptions during the read of the options file */
            } finally {
                if (streamReader != null)
                    streamReader.Close();
            }
            currentOption = 0;
        }        
    }
}
