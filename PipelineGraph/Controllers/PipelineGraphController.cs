using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PipelineGraph.Database;

namespace PipelineGraph.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PipelineGraphController : ControllerBase
    {
        private readonly ILogger<PipelineGraphController> _logger;
        private readonly DbContextOptions<MyContext> _options;
        public PipelineGraphController(ILogger<PipelineGraphController> logger)
        {
            _logger = logger;
            _options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;
        }
      
        [HttpPost]
        [Route("init")]
        public ActionResult Init([FromBody]List<List<string>> edges)
        {
            var sessionId = Guid.NewGuid().ToString("N").ToLower();
            var session = new Session{SessionId = sessionId, Nodes = new List<Node>()};
            var nodeMap = new Dictionary<string, Node>();
            using var context = new MyContext(_options);
            
            foreach (var edge in edges)
            {
                var node1 = CreateNode(nodeMap, edge[0], session);
                var node2 = CreateNode(nodeMap, edge[1], session);
                node1.Nodes.Add(node2.Name);
            }
            context.Sessions.Add(session);
            context.SaveChanges();
            return Ok(sessionId);
        }
        
        [HttpPost]
        [Route("updatestatus/{id}")]
        public ActionResult UpdateJobStatus([FromBody]List<string> newJobStatus, string id)
        {
            using var context = new MyContext(_options);
            try
            {
                if (!Enum.TryParse(newJobStatus[1], true, out JobStatus status))
                {
                    return BadRequest();
                }
                var session = context.Sessions
                    .Include(s => s.Nodes)
                    .Single(s => s.SessionId == id);
                var node = session.Nodes.Single(n =>
                    string.Compare(n.Name, newJobStatus[0], StringComparison.OrdinalIgnoreCase) == 0);
                node.Status = status;
                context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        private static Node CreateNode(Dictionary<string, Node> nodeMap, string node, Session session)
        {
            if (nodeMap.ContainsKey(node))
            {
                return nodeMap[node];
            }
            var n = new Node {Name = node, Nodes = new List<String>()};
            nodeMap.Add(node, n);
            session.Nodes.Add(n);
            return n;
        }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            using var context = new MyContext(_options);
            try
            {
                var session = context.Sessions
                    .Include(s => s.Nodes)
                    .Single(s => s.SessionId == id);
                var graph = "graph LR;" +
                            "\nclassDef failure fill:#f44,stroke:#333,stroke-width:2px" +
                            "\nclassDef success fill:#4f4,stroke:#333,stroke-width:2px" +
                            "\nclassDef running fill:#aaf,stroke:#333,stroke-width:2px" +
                            "\nclassDef notstarted fill:#fff,stroke:#333,stroke-width:2px";;
                foreach (var node in session.Nodes)
                {
                    foreach (var targetNodeName in node.Nodes)
                    {
                        var targetNode = session.Nodes.Single(n =>
                            string.Compare(n.Name, targetNodeName, StringComparison.OrdinalIgnoreCase) == 0);
                        graph += $"\n{node.Id}({node.Name}) --> {targetNode.Id}({targetNode.Name})";
                    }
                }
                graph += StyleNodes(session, JobStatus.Running, "running");
                graph += StyleNodes(session, JobStatus.FinishedError, "failure");
                graph += StyleNodes(session, JobStatus.FinishedSuccess, "success");
                graph += StyleNodes(session, JobStatus.NotStarted, "notstarted");
                return new JsonResult(graph);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        private static string StyleNodes(Session session, JobStatus status, string styleName)
        {
            var running = session.Nodes.Where(n => n.Status == status).ToList();
            if (running.Count == 0) return "";
            var graph = "\nclass ";
            graph = running.Aggregate(graph, (current, node) => current + $"{node.Id},");
            graph += $" {styleName}";
            return graph;
        }
    }
}