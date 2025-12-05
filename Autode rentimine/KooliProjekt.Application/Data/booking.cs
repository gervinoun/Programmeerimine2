using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Data
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [Required]
        public decimal KmStart { get; set; }

        public decimal KmEnd { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
    }
}
