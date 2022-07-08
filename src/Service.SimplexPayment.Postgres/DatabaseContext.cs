using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;
using Service.SimplexPayment.Domain.Models;

namespace Service.SimplexPayment.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "simplex";

        public const string IntentionsTableName = "intentions";
        public DbSet<SimplexIntention> Intentions { get; set; }
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {           
            modelBuilder.HasDefaultSchema(Schema);
            SetIntentions(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void SetIntentions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SimplexIntention>().ToTable(IntentionsTableName);
            modelBuilder.Entity<SimplexIntention>().HasKey(e => e.QuoteId);
            modelBuilder.Entity<SimplexIntention>().Property(e => e.CreationTime).HasDefaultValue(DateTime.MinValue);
            
            modelBuilder.Entity<SimplexIntention>().HasIndex(e => e.PaymentId);
            modelBuilder.Entity<SimplexIntention>().HasIndex(e => e.ClientIdHash);
            modelBuilder.Entity<SimplexIntention>().HasIndex(e => e.BlockchainTxHash);
            modelBuilder.Entity<SimplexIntention>().HasIndex(e => e.CreationTime);
            modelBuilder.Entity<SimplexIntention>().HasIndex(e => e.ClientId);
            modelBuilder.Entity<SimplexIntention>().HasIndex(e => e.FromCurrency);
            modelBuilder.Entity<SimplexIntention>().HasIndex(e => e.ToAsset);
            modelBuilder.Entity<SimplexIntention>().HasIndex(e => e.Status);
        }
       
        public async Task<int> UpsertAsync(IEnumerable<SimplexIntention> entities)
        {
            var result = await Intentions.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
    
    }
}
