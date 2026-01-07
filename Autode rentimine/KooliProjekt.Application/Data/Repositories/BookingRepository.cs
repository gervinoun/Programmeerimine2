using KooliProjekt.Application.Data;

namespace KooliProjekt.Application.Data.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext db) : base(db) { }
    }
}
