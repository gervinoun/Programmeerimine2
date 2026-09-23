using KooliProjekt.Application.Data;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.UnitTests
{
    public class TestBase
    {
        protected ApplicationDbContext GetFaultyDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>();

            var dbContext = new ApplicationDbContext(options.Options);

            return dbContext;
        }
    }
}