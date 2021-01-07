using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
      
        [HttpGet]
        public ActionResult Get()
        {
            using var context = new MyContext(_options);
            var ids = "";
            foreach (var contextGraph in context.Graphs)
            {
                ids += contextGraph.Id + "\n";
            }
            return new JsonResult(ids);
        }
        
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            using var context = new MyContext(_options);
            var graph = context.Graphs.Find(id);
            if (graph == null) return BadRequest();
            return new JsonResult(graph.Description);
        }
        
        [HttpPost]
        public ActionResult Post([FromBody]string jsonParameters)
        {
            var splits = jsonParameters.Split(";");
            using var context = new MyContext(_options);
            switch (splits.Length)
            {
                case 3:
                {
                    var graph = new Graph()
                    {
                        Description = $"graph LR;\n{splits[0]}-- {splits[1]} --->{splits[2]};"
                    };

                    context.Graphs.Add(graph);
                    context.SaveChanges();
                    return Ok(graph.Id);
                }
                case 4:
                {
                    var id = splits[0];
                    var graph = context.Graphs.Find(Int32.Parse(id));
                    if (graph == null) return BadRequest();
                    
                    graph.Description = $"{graph.Description}\n{splits[1]}-- {splits[2]} --->{splits[3]}";
                    context.Graphs.Update(graph);
                    context.SaveChanges();
                    
                    return Ok(graph.Id);
                }
                default:
                    return BadRequest();
            }
        }
    }
}