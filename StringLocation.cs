using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace callstack
{
    /*
     * StringLocation stores the location of searchstrings inside files, 
     * as well as the file and stringname, of all encountered findings.
     */
    class StringLocation
    {
        internal string stringname = "";
        internal string filename = "";
        internal int fileLocation = 0;

        public StringLocation(string stringname, string filename, int fileLocation) // Constructor
        {
            this.stringname = stringname;
            this.filename = filename;
            this.fileLocation = fileLocation;
        }
    }
}
