using System.ComponentModel.DataAnnotations;

namespace KooliProjekt.BlazorWasm.Api
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Number plate is required.")]
        public string NumberPlate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Type ID must be greater than zero.")]
        public int TypeId { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Km rate must be zero or greater.")]
        public decimal Kmrate { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Time rate must be zero or greater.")]
        public decimal TimeRate { get; set; }

        public bool IsAvailable { get; set; }
    }
}
