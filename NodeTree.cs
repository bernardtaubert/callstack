using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace callstack
{
    class NodeTree
    {
        List<string> traversedNodes = new List<string>();

        public NodeTree(List<string> traversedNodes) // Constructor
        {
            this.traversedNodes.AddRange(traversedNodes);
        }

        public void printNodeTree(Node n, int spaces)
        {
            this.traversedNodes.Add(n.ToString());
            for (int i = 0; i < spaces; i++)
                Console.Write("  ");
            Console.WriteLine("+ " + n.ToString());
            foreach (Node child in n.children)
            {
                if (this.traversedNodes.Contains(child.ToString())) 
                {
                    for (int i = 0; i < spaces + 1; i++)
                        Console.Write("  ");
                    Console.WriteLine("- " + child.ToString()); // Recursion
                }
                else
                {
                    NodeTree nt = new NodeTree(this.traversedNodes);
                    nt.printNodeTree(child, spaces + 1);
                }
            }
        }
    }
}
