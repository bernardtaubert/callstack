using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace callstack
{
    internal class Node : IComparable
    {
        internal List<Node> parents = new List<Node>();
        internal List<Node> children = new List<Node>();
        string name = "";

        public Node(string name) // Constructor
        {
            this.name = name;
        }
        
        public void AddParent(string name)
        {
            if ( !this.parents.Any(n => n.ToString() == name)) // does a node with the same name already exist?
                this.parents.Add(new Node(name));
        }        

        public void AddChild(string name)
        {
            if ( !this.children.Any(n => n.ToString() == name)) // does a node with the same name already exist?
                this.children.Add(new Node(name));
        }

        public void AddChild(Node child)
        {
            if (!this.children.Any(n => n.ToString() == child.name)) // does a node with the same name already exist?
                this.children.Add(child);
        }  
        
        override public string ToString() 
        {
            return name;
        }

        public int CompareTo(Object obj)
        {
            if (obj == null) return 1;
            Node n = obj as Node;
            if (n != null) return name.CompareTo(n.ToString());
            throw new ArgumentException("Object is not of Type Node");
        }
    }
}
