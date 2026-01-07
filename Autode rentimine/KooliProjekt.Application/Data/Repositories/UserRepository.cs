using KooliProjekt.Application.Data;

namespace KooliProjekt.Application.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db) : base(db) { }
    }
}
