using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace callstack
{
    internal class Options /* search options container class */
    {
        internal List<string> extensions = new List<string>();
        internal List<string> searchPaths = new List<string>();

        internal Option SearchSubDirectories;
        internal enum AvailableOptions {
            SearchSubDirectories = 0,
        };
        internal List<Option> list;
        public Options() { // Constructor
            list = new List<Option>();
            SearchSubDirectories = new Option(false, "SearchSubDirectories", (int)AvailableOptions.SearchSubDirectories);
            list.Add(SearchSubDirectories);
        }
        #region GettersSetters
        public void SetValue(AvailableOptions optionId, bool value) {
            foreach (Option o in list) {
                if (o.GetId() == (int)optionId) {
                    o.SetValue(value);
                    break;
                }
            }
        }
        public void SetValue(int optionId, bool value) {
            foreach (Option o in list) {
                if (o.GetId() == optionId) {
                    o.SetValue(value);
                    break;
                }
            }
        }
        public bool GetValue(AvailableOptions id) {
            bool value = false;
            foreach (Option o in list) {
                if (o.GetId() == (int)id) {
                    value = o.GetValue();
                    break;
                }
            }
            return value;
        }
        public List<Option> GetList() {
            return list;
        }
        #endregion
    }
    public class Option : IComparable<Option> {
        private string name;
        private bool value;
        private int id;
        public Option(bool value, string name, int id) { // Constructor
            this.name = name;
            this.value = value;
            this.id = id;
        }
        override public string ToString() {
            return name;
        }
        public bool GetValue() {
            return value;
        }
        public void SetValue(bool value) {
            this.value = value;
        }
        public int GetId() {
            return id;
        }
        public int CompareTo(Option that) {
            if (this.id > that.id) return -1;
            if (this.id == that.id) return 0;
            return 1;
        }
    }
}
