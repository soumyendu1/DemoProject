using Microsoft.EntityFrameworkCore;

namespace DemoProject.Models
{
    public class DemoDBContext : DbContext
    {
        public DemoDBContext(DbContextOptions<DemoDBContext> options) : base(options)
        {
        }
    }
}
