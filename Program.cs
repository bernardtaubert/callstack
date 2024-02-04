using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace callstack
{
    class Program
    {
        internal enum state
        {
            inRoot = 0,
            inCharLiteral = 1,
            inStringLiteral = 2,
            inSingleLineComment = 3,
            inMultiLineComment = 4,
            inFunction = 5,
            inDefine = 6,
            parseRootParameter = 7,
        };
        static int count_parenthesis;
        static bool leadingAmpersand = false;
        static bool searchFinished = false;
        static Nodes nodes = new Nodes();
        static List<string> searchComplete = new List<string>();
        static List<StringLocation> stringLocations = new List<StringLocation>();

        static string recursionOutput = "";
        static string orphansOutput = "";

        static void Main(string[] args)
        {
            List<string> allFiles = new List<string>(); // create an Array of search Files filtered by search pattern (sum of all paths)
            List<string> files = new List<string>(); // create an Array of search Files filtered by search pattern in a specified path

            Options options = new Options(); // search options container
            ParseOptions.ParseOptionsFromFile("default_options.txt", options);

            if (args.Length == 0)
            {
                Console.WriteLine("Please specify the function identifier (search string) as first input argument");
                return;
            }
            else
            {
                foreach (string path in options.searchPaths)
                {
                    files = FileUtil.GetFiles(path, options.extensions, options.GetValue(Options.AvailableOptions.SearchSubDirectories));

                    foreach (string file in files)
                        allFiles.Add(file); // sum all files
                }



                nodes.Add(args[0]); // the searchstring!!!
                List<Node> orphans = nodes.GetOrphans();
                while (!searchFinished)
                {
                    searchFinished = true;
                    orphans = nodes.GetOrphans();
                    foreach (Node o in orphans)
                    {
                        if (!searchComplete.Contains(o.ToString()))
                        {
                            ParseFiles(allFiles, o.ToString());
                            searchComplete.Add(o.ToString());
                        }
                    }
                    orphans = nodes.GetOrphans();
                    foreach (Node o in orphans)
                        if (!searchComplete.Contains(o.ToString()))
                            searchFinished = false;
                }


                Console.WriteLine("Show Tree:");
                foreach (Node n in nodes)
                {
                    if (n.parents.Count == 0)
                    {
                        NodeTree nt = new NodeTree(new List<string>());
                        nt.printNodeTree(n, 0);
                    }
                }

                List<List<string>> recursiveCallstacks = new List<List<string>>();
                foreach (Node n in nodes)
                {
                    if (n.parents.Count != 0)
                    {
                        NodeTreeRecursive ntr = new NodeTreeRecursive(new List<string>());
                        recursiveCallstacks = ntr.getRecursiveCallstacks(n);
                        foreach (List<string> callstack in recursiveCallstacks)
                        {
                            int spaces = 0;
                            foreach (string s in callstack)
                            {
                                for (int i = 0; i < spaces; i++)
                                    recursionOutput += "  ";   //Console.Write("  ");
                                recursionOutput += "- " + s + "\n"; //Console.WriteLine("- " + s);
                                spaces++;
                            }
                        }
                    }
                }
                if (!recursionOutput.Equals(""))
                {
                    Console.WriteLine("\nRecursion:");
                    Console.WriteLine(recursionOutput);
                }

                // Show the filelocations and searchstrings of all encountered findings:
                /*
                Console.WriteLine("");
                foreach (StringLocation sl in stringLocations)
                {
                    Console.WriteLine("Name=" + sl.stringname + " filePos=" + sl.fileLocation + " file=" + sl.filename);
                }*/

                // Show the searchstrings, which were searched already.
                foreach (string s in searchComplete)
                {
                    //Console.WriteLine("String s=" + s);
                    SearchStringLocations(allFiles, s);
                }
                if (!orphansOutput.Equals(""))
                {
                    Console.WriteLine("\nOrphans:");
                    Console.WriteLine(orphansOutput);
                }

                Console.ReadKey();
            }
        }

        static private void ParseFiles(List<string> allFiles, string searchString)
        {
            foreach (string file in allFiles)
            {
                ParseFile(file, searchString); //string text = File.ReadAllText(file);
            }
        }
        static private void ParseFile(string file, string searchString)
        {
            int position = 0; // offset in file
            state s = state.inRoot;
            state lastState = state.inRoot;
            StreamReader sr = new StreamReader(file);
            
            int i;
            ArrayList buffer = new ArrayList();
            string identifier = "";
            char? lastChar = null;
            while ((i = sr.Read()) != -1)
            {
                char c = (char)i;
                position++;
                buffer.Add(c);
                switch (s)
                {
                    #region inRoot
                    case state.inRoot:
                        if (buffer.Count == 1 && (char)buffer[buffer.Count - 1] == '/')
                            continue; // first character indicates comment

                        //Console.Write((char)i);

                        if (c == '\'')
                        {
                            s = state.inCharLiteral;
                            lastState = state.inRoot;
                            continue;
                        }
                        if (c == '\"')
                        {
                            s = state.inStringLiteral;
                            lastState = state.inRoot;
                            continue;
                        }
                        if (buffer.Count >= 2)
                        {
                            if ((char)buffer[buffer.Count - 2] == '/' && (char)buffer[buffer.Count - 1] == '*')
                            {
                                s = state.inMultiLineComment;
                                lastState = state.inRoot;
                                buffer[buffer.Count - 1] = '/'; // overwrite asterisk so that it will not be confused with the comment ending one
                                continue;
                            }
                            else if ((char)buffer[buffer.Count - 2] == '/' && (char)buffer[buffer.Count - 1] == '/')
                            {
                                s = state.inSingleLineComment;
                                lastState = state.inRoot;
                                continue;
                            }
                            else
                            {
                                //    Console.Write(c);
                            }
                        }
                        if (Cu.AnyOf(c, " \t\r\n"))
                        {
                            if (!Cu.ToString(buffer).Equals(""))
                            {
                                identifier = Cu.ToString(buffer); // remember the identifier
                                buffer.Clear();
                            }
                        }
                        else if (c == '(')
                        {
                            buffer = buffer.GetRange(0, buffer.Count - 1); // remove parenthesis '('

                            if (!Cu.ToString(buffer).Equals(""))
                            {
                                identifier = Cu.ToString(buffer.GetRange(0, buffer.Count));

                                // Console.WriteLine("IDENTIFIER = " + identifier + " FILEPOS = " + position);
                                stringLocations.Add(new StringLocation(identifier, file, position));
                            }
                            //Console.WriteLine("Buffer = " + temp);
                            buffer.Clear();

                            s = state.parseRootParameter;

                            //Console.WriteLine(" Identifier: " + identifier);
                            count_parenthesis = 1;
                            continue;

                        }
                        else if (c == '#')
                        {
                            // TODO
                        }

                        break;
                    #endregion
                    #region inFunction
                    case state.inFunction:
                        if (c == '\'')
                        {
                            s = state.inCharLiteral;
                            lastState = state.inFunction;
                            continue;
                        }
                        if (c == '\"')
                        {
                            s = state.inStringLiteral;
                            lastState = state.inFunction;
                            continue;
                        }
                        if (buffer.Count >= 2)
                        {
                            if ((char)buffer[buffer.Count - 2] == '/' && (char)buffer[buffer.Count - 1] == '*')
                            {
                                s = state.inMultiLineComment;
                                lastState = state.inFunction;
                                buffer[buffer.Count - 1] = '/'; // overwrite asterisk so that it will not be confused with the comment ending one
                                continue;
                            }
                            else if ((char)buffer[buffer.Count - 2] == '/' && (char)buffer[buffer.Count - 1] == '/')
                            {
                                s = state.inSingleLineComment;
                                lastState = state.inFunction;
                                continue;
                            }
                        }
                        if (c == '&')
                            leadingAmpersand = true;

                        if (Cu.AnyOf(c, "_a-zA-Z0-9")) // check if the character is an alphanumeric identifier
                        {
                            if (buffer.Count >= 2 && Cu.AnyOf((char)buffer[buffer.Count - 2], " \t*+-./\r\n\\~^&()[]{}?:><=")) // check if it was the start of the alphanumeric identifier
                            {
                                buffer.Clear();
                                buffer.Add(c);
                            }
                        }
                        if (Cu.AnyOf(c, " \t*+-./\r\n\\~^&()[]{}?:><="))
                        {
                            if (buffer.Count-1 == searchString.Length && Cu.ToString(buffer.GetRange(0, searchString.Length)).Equals(searchString)) // found the searchstring?
                            {
                                nodes.AddLink(searchString, identifier);
                                stringLocations.Add(new StringLocation(searchString, file, position));
                                // Console.WriteLine("Found the searchstring: " + searchString + " inside function: " + identifier);
                                // if (leadingAmpersand)
                                    // Console.WriteLine("It has a leading ampersand");
                            }
                        }
                        if (!Cu.AnyOf(c, " \t&()_a-zA-Z0-9"))
                            leadingAmpersand = false;
                        switch (c)
                        {
                            case '{':
                                count_parenthesis++;
                                break;
                            case '}':
                                count_parenthesis--;
                                if (count_parenthesis == 0)
                                {
                                    // Console.WriteLine("Exiting Function-Identifier: " + identifier);
                                    s = state.inRoot;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    #endregion
                    case state.inMultiLineComment:
                        //Console.Write(c);
                        if ((char)buffer[buffer.Count - 2] == '*' && (char)buffer[buffer.Count - 1] == '/')
                        {
                            s = lastState; // restore lastState
                            buffer.Clear();

                            //foreach (char ch in buffer)
                            //    Console.Write(ch);
                        }
                        break;
                    case state.inSingleLineComment:
                        if ((char)buffer[buffer.Count - 2] == (char)13 && (char)buffer[buffer.Count - 1] == (char)10) // check presence of newline
                        {
                            s = lastState; // restore lastState
                            buffer.Clear();

                            //foreach (char ch in buffer)
                            //    Console.Write(ch);
                        }
                        break;
                    case state.inCharLiteral:
                        if ((char)buffer[buffer.Count - 1] == (char)'\\') // skip next sign after escape character
                        {
                            sr.Read();
                            position++;
                        }
                        if ((char)buffer[buffer.Count - 1] == (char)'\'')
                            s = lastState; // restore lastState
                        break;
                    case state.inStringLiteral:
                        if ((char)buffer[buffer.Count - 1] == (char)'\\') // skip next sign after escape character
                        {
                            sr.Read();
                            position++;
                        }
                        if ((char)buffer[buffer.Count - 1] == (char)'\"')
                            s = lastState; // restore lastState
                        break;
                    #region parseRootParameter
                    case state.parseRootParameter:
                        if (c == '\'')
                        {
                            s = state.inCharLiteral;
                            lastState = state.parseRootParameter;
                            continue;
                        }
                        if (c == '\"')
                        {
                            s = state.inStringLiteral;
                            lastState = state.parseRootParameter;
                            continue;
                        }
                        if (buffer.Count >= 2)
                        {
                            if ((char)buffer[buffer.Count - 2] == '/' && (char)buffer[buffer.Count - 1] == '*')
                            {
                                s = state.inMultiLineComment;
                                lastState = state.parseRootParameter;
                                buffer[buffer.Count - 1] = '/'; // overwrite asterisk so that it will not be confused with the comment ending one
                                continue;
                            }
                            else if ((char)buffer[buffer.Count - 2] == '/' && (char)buffer[buffer.Count - 1] == '/')
                            {
                                s = state.inSingleLineComment;
                                lastState = state.parseRootParameter;
                                continue;
                            }
                        }
                        if (Cu.AnyOf(c, " \t\r\n_,a-zA-Z0-9*"))
                        {
                        }
                        else
                        {
                            switch (c)
                            {
                                case '(':
                                    count_parenthesis++;
                                    break;
                                case ')':
                                    count_parenthesis--;
                                    break;
                                case '{':
                                    if (count_parenthesis == 0)
                                    {
                                        //Console.WriteLine(" Identifier: " + identifier);
                                        count_parenthesis = 1;
                                        s = state.inFunction;
                                    }
                                    else
                                    {
                                        s = state.inRoot; // return to root !!!
                                    }
                                    break;
                                case '/': // first character indicates comment
                                    continue;
                                default:
                                    s = state.inRoot; // return to root !!!
                                    break;
                            }
                        }
                        /*
                        if (ParseParameter(sr))
                        {
                            Console.WriteLine(" Identifier: " + identifier);
                            s = state.inFunction;
                            lastState = state.inFunction;
                        }*/
                        break;
                    default:
                        break;
                    #endregion
                }

                if (lastChar != null)
                {

                }
                lastChar = c;
            }

            sr.Close();
        }

        public static void SearchStringLocations(List<string> allFiles, string searchString)
        {
            foreach (string file in allFiles)
            {
                SearchStrings(file, searchString);
            }
        }
        public static void SearchStrings(string file, string searchString)
        {
            int position = 0; // offset in file
            state s = state.inRoot;
            state lastState = state.inRoot;
            StreamReader sr = new StreamReader(file);

            int i;
            ArrayList buffer = new ArrayList();
            char? lastChar = null;
            while ((i = sr.Read()) != -1)
            {
                char c = (char)i;
                position++;
                buffer.Add(c);
                switch (s)
                {
                    #region inRoot
                    case state.inRoot:
                        if (buffer.Count == 1 && (char)buffer[buffer.Count - 1] == '/')
                            continue; // first character indicates comment

                        if (c == '\'')
                        {
                            s = state.inCharLiteral;
                            lastState = state.inRoot;
                            continue;
                        }
                        if (c == '\"')
                        {
                            s = state.inStringLiteral;
                            lastState = state.inRoot;
                            continue;
                        }
                        if (buffer.Count >= 2)
                        {
                            if ((char)buffer[buffer.Count - 2] == '/' && (char)buffer[buffer.Count - 1] == '*')
                            {
                                s = state.inMultiLineComment;
                                lastState = state.inRoot;
                                buffer[buffer.Count - 1] = '/'; // overwrite asterisk so that it will not be confused with the comment ending one
                                continue;
                            }
                            else if ((char)buffer[buffer.Count - 2] == '/' && (char)buffer[buffer.Count - 1] == '/')
                            {
                                s = state.inSingleLineComment;
                                lastState = state.inRoot;
                                continue;
                            }
                        }
                        if (buffer.Count >= searchString.Length && c == searchString[searchString.Length - 1])
                        {
                            bool foundSearchString = true;
                            string bufferEndswith = Cu.ToString(buffer.GetRange(buffer.Count - (searchString.Length), searchString.Length));
                            //Console.WriteLine("Buffer = " + bufferEndswith + " BufferCount = " + buffer.Count + " BufferLength = " + bufferEndswith.Length + " StringLength = " + searchString.Length);
                            if (bufferEndswith.Length != searchString.Length)
                                foundSearchString = false;
                            else
                            {
                                for (int j = 0; j < searchString.Length; j++)
                                {
                                    if (bufferEndswith[j] != searchString[j])
                                    {
                                        foundSearchString = false;
                                        break;
                                    }
                                }
                            }
                            if (foundSearchString)
                            {
                                if (stringLocations.Any(x => x.filename.Equals(file) && x.stringname.Equals(bufferEndswith) && x.fileLocation == position + 1))
                                {
                                    //Console.WriteLine(bufferEndswith);
                                }
                                else
                                {
                                    //Console.WriteLine(bufferEndswith + " in file = " + file + " at position = " + (position - bufferEndswith.Length));
                                    orphansOutput += bufferEndswith + " in file = " + file + " at position = " + (position - bufferEndswith.Length) + "\n";
                                }
                            }
                        }
                        break;
                    #endregion
                    case state.inMultiLineComment:
                        if ((char)buffer[buffer.Count - 2] == '*' && (char)buffer[buffer.Count - 1] == '/')
                        {
                            s = lastState; // restore lastState
                            buffer.Clear();
                        }
                        break;
                    case state.inSingleLineComment:
                        if ((char)buffer[buffer.Count - 2] == (char)13 && (char)buffer[buffer.Count - 1] == (char)10) // check presence of newline
                        {
                            s = lastState; // restore lastState
                            buffer.Clear();
                        }
                        break;
                    case state.inCharLiteral:
                        if ((char)buffer[buffer.Count - 1] == (char)'\\') // skip next sign after escape character
                        {
                            sr.Read();
                            position++;
                        }
                        if ((char)buffer[buffer.Count - 1] == (char)'\'')
                            s = lastState; // restore lastState
                        break;
                    case state.inStringLiteral:
                        if ((char)buffer[buffer.Count - 1] == (char)'\\') // skip next sign after escape character
                        {
                            sr.Read();
                            position++;
                        }
                        if ((char)buffer[buffer.Count - 1] == (char)'\"')
                            s = lastState; // restore lastState
                        break;
                }
                lastChar = c;
            }

            sr.Close();
        }
    }
}
