using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace callstack
{
    internal class Nodes
    {
        List<Node> nodes = new List<Node>();

        public Nodes() // Constructor
        {
        }

        public void Add(Node n)
        {
            Node node = n;
            if (!this.nodes.Any(x => x.ToString() == n.ToString())) // does a node with the same name already exist?
                nodes.Add(node);
        }

        public void Add(string name)
        {
            Node node = new Node(name);
            if ( !this.nodes.Any(n => n.ToString() == name)) // does a node with the same name already exist?
                nodes.Add(node);
        }

        // AddLink adds a parent (i.e. a calling function) 
        public void AddLink(string name, string parent)
        {
            Node node = new Node(name);
            if ( !this.nodes.Any(n => n.ToString() == name)) // does a node with the same name already exist?
            {
                node.AddParent(parent);
                this.nodes.Add(node);

                Node p = new Node(parent);
                if ( !this.nodes.Any(n => n.ToString() == parent)) // does a node with the same name already exist?
                {
                    p.AddChild(name);
                    this.nodes.Add(p);
                }
                else
                {
                    foreach (Node n in nodes)
                        if (n.ToString().Equals(p.ToString())) // get the existing node with the corresponding name
                            n.AddChild(name);
                }
            }
            else // a node with the same name already exists
            {
                Nodes newParents = new Nodes();

                foreach (Node n in nodes)
                {
                    if (n.ToString().Equals(name)) // get the existing node with the corresponding name
                    {
                        if ( !n.parents.Any(x => x.ToString() == parent)) // does the parent already exist?
                        {
                            Node p = new Node(parent);

                            p.children.Add(n);
                            n.parents.Add(p);

                            if ( !this.nodes.Any(x => x.ToString() == parent)) // does a node with the same name already exist?
                            {
                                newParents.Add(p);
                            }
                            else
                            {
                                foreach (Node x in nodes)
                                    if (x.ToString().Equals(parent))
                                        x.AddChild(n);
                            }
                        }
                        else // a node with the same parent already exists
                        {

                        }
                    }
                }

                foreach (Node n in newParents)
                    this.nodes.Add(n);

                //this.nodes.Clear(); // Copy nodes back to this.nodes
                //foreach (Node n in tempNodes)
                //    this.nodes.Add(n);
            }
        }
        
        public List<Node> GetOrphans()
        {
            List<Node> orphans = new List<Node>();
            
            // foreach node in orphans
            foreach (Node n in this.nodes)
            {
                if (n.parents.Count == 0) // If parents == empty
                    orphans.Add(n); // Add node to orphans
            }
            
            return orphans;
        }

        public IEnumerator<Node> GetEnumerator()
        {
            foreach (Node n in nodes)
                yield return n;
        }

        public void Reverse()
        {
            nodes.Reverse();
        }
    }
}

