using KooliProjekt.Application.Data;

namespace KooliProjekt.Application.Data.Repositories
{
    public class CarRepository : BaseRepository<Car>, ICarRepository
    {
        public CarRepository(ApplicationDbContext db) : base(db) { }
    }
}
