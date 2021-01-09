using System.Collections.Generic;

namespace PipelineGraph
{
    public class Session
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public List<Node> Nodes { get; set; }
    }
}