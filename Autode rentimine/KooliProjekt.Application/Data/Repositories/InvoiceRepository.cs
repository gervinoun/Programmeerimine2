using KooliProjekt.Application.Data;

namespace KooliProjekt.Application.Data.Repositories
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(ApplicationDbContext db) : base(db) { }
    }
}
