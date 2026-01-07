using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Data.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Entity
    {
        protected readonly ApplicationDbContext Db;

        public BaseRepository(ApplicationDbContext db)
        {
            Db = db;
        }

        public IQueryable<T> Query()
        {
            return Db.Set<T>().AsQueryable();
        }

        public Task<T> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return Db.Set<T>().FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task AddAsync(T entity, CancellationToken ct = default)
        {
            return Db.Set<T>().AddAsync(entity, ct).AsTask();
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            return Db.SaveChangesAsync(ct);
        }
    }
}
