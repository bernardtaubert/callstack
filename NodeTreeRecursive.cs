using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace callstack
{
    class NodeTreeRecursive
    {
        NodeTreeRecursive root = null;
        List<string> traversedNodes = new List<string>();
        List<List<string>> recursiveCallstacks = new List<List<string>>();

        public NodeTreeRecursive(List<string> traversedNodes) // Constructor
        {
            this.traversedNodes.AddRange(traversedNodes);
        }

        public NodeTreeRecursive(List<string> traversedNodes, NodeTreeRecursive ntr) // Constructor
        {            
            this.traversedNodes.AddRange(traversedNodes);
            this.root = ntr;
        }

        public List<List<string>> getRecursiveCallstacks(Node n)
        {
            this.traversedNodes.Add(n.ToString());
            foreach (Node child in n.children)
            {
                if (this.traversedNodes.Contains(child.ToString())) 
                {
                    this.traversedNodes.Add(child.ToString()); // Recursion
                    if (this.root == null)
                        this.recursiveCallstacks.Add(traversedNodes);
                    else
                        this.root.recursiveCallstacks.Add(traversedNodes);
                }
                else
                {
                    if (this.root == null)
                    {
                        NodeTreeRecursive ntr = new NodeTreeRecursive(this.traversedNodes, this);
                        ntr.getRecursiveCallstacks(child);
                    }
                    else
                    {
                        NodeTreeRecursive ntr = new NodeTreeRecursive(this.traversedNodes, this.root);
                        ntr.getRecursiveCallstacks(child);
                    }
                }
            }
            if (this.root == null)
                return this.recursiveCallstacks;
            else
                return null;
        }
    }
}
