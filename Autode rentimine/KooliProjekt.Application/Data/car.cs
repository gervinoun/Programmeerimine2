using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Data
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string NumberPlate { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        public decimal Kmrate { get; set; }
        
        [Required]
        public decimal TimeRate { get; set; }

        [Required]
        public bool IsAvailable { get; set; }
    }
}
