using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Data
{
    public class Invoice : Entity
    {
        

        [Required]
        public int BookingId { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public decimal Total { get; set; }
    }
}
