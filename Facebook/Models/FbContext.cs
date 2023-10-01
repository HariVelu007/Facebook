using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class FbContext : DbContext
    {
        public FbContext(DbContextOptions<FbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<DataKeys> DataKeys { get; set; }
    }
    
}
