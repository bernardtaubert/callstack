using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace callstack
{
    static class Cu
    {
        /*
         * AnyOfSimple
         *   Checks if a character is any of the ones contained in a string argument.
         */
        static public bool AnyOfSimple(char c, string s)
        {
            bool foundAny = false;

            foreach (char iter in s)
                if (iter == c)
                { 
                    foundAny = true;
                    break;
                }

            return foundAny;
        }

        /*
         * AnyOf 
         *   Checks if a character is any of the ones contained in a string argument.
         *   Ranges of characters as in A-Z or 0-9 are also supported.
         */
        static public bool AnyOf(char c, string s) 
        {
            bool foundAny = false;
            
            int i = s.IndexOf('-', 1);

            if (i != -1 && i < s.Length - 1)
            {
                int min = s[i - 1];
                int max = s[i + 1];

                if (min > max)
                {
                    int temp = min; // swap min and max
                    min = max;
                    max = temp;
                }

                if (c >= min && c <= max)
                    foundAny = true;

                s = s.Remove(i - 1, 3);

                if (foundAny)
                {
                    // do nothing
                }
                else
                {
                    // proceed search recursively
                    foundAny = AnyOf(c, s);
                }
            }

            if (foundAny)
            {
                // do nothing
            }
            else
            {
                foreach (char iter in s)
                    if (iter == c)
                    {
                        foundAny = true;
                        break;
                    }
            }

            return foundAny;
        }

        static public string ToString(ArrayList buffer)
        {
            string s = "";

            foreach (char c in buffer)
                s += c;

            return s.Trim();
        }
    }
}
