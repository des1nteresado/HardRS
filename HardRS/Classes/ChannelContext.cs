using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace HardRS
{
    class ChannelContext : DbContext
    {
        public ChannelContext()
            : base("DbConnection")
        { }

        public DbSet<Items> Items { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ImageOfChannel> ChannelImages { get; set; }
    }
}
