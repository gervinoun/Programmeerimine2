using KooliProjekt.Application.Data;

namespace KooliProjekt.Application.Data.Repositories
{
    public class CarTypeRepository : BaseRepository<CarType>, ICarTypeRepository
    {
        public CarTypeRepository(ApplicationDbContext db) : base(db) { }
    }
}
