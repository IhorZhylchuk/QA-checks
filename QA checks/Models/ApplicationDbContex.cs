using Microsoft.EntityFrameworkCore;

namespace QA_checks.Models
{
    public class ApplicationDbContex: DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<QAchecks> QAchecks { get; set; }
        public ApplicationDbContex(DbContextOptions<ApplicationDbContex> dbContext) : base(dbContext)
        {

        }
    }
}
