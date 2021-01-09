using System;
using System.Collections.Generic;

namespace PipelineGraph
{
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public JobStatus Status { get; set; }
        public List<String> Nodes { get; set; }
    }
}