using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace SPHostedPages.WebApi.Models
{
    public class MyContext : DbContext
    {
        public DbSet<AppDomain> AppDomains { get; set; }
    }

    public class AppDomain
    {
        [Key]
        public string HostUrl { get; set; }
    }
}