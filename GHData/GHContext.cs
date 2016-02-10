using System.Data.Entity;
using GHModel;

namespace GHData
{
    /// <summary>Database context for Global Health App</summary>
    public class GHContext : DbContext
    {
        const string ConnectionString = "Server=LOCALHOST;Database=GHData;User Id=CREATE ONE;Password=CREATEONE;";
        public GHContext() : base(ConnectionString){}
        public DbSet<HealthUpdate>HealthUpdates { get; set; }        
    }
}
