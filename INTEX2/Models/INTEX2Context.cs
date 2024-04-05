using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace INTEX2.Models
{
    public partial class INTEX2Context : DbContext
    {
        public INTEX2Context()
        {
        }

        public DbSet<AppUser> Users { get ; set; }

        public INTEX2Context(DbContextOptions<INTEX2Context> options)
            : base(options)
        {
        }
    }
}
