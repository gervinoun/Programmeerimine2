using KooliProjekt.Application.Data;

namespace KooliProjekt.Application.Data.Repositories
{
    public class InvoiceLineRepository : BaseRepository<InvoiceLine>, IInvoiceLineRepository
    {
        public InvoiceLineRepository(ApplicationDbContext db) : base(db) { }
    }
}
