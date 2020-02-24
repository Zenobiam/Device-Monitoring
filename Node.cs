using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitoringDevices
{
   public class Node
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public List<Node> Children { get; set; }

        public Node()
        {
            Children = new List<Node>();
        }
    }
}
