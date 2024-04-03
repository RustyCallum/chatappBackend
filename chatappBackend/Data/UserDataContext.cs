using chatappBackend.User;
using Microsoft.EntityFrameworkCore;

namespace chatappBackend.Data
{
    public class UserDataContext : DbContext
    {
        public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) { }

        public DbSet<Users> Users => Set<Users>();
    }
}
