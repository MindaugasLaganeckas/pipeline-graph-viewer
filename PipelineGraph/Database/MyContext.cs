using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PipelineGraph.Database
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        { }
        public DbSet<Graph> Graphs { get; set; }
    }
}