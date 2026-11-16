namespace KooliProjekt.BlazorWasm.Api
{
    public class Car
    {
        public int Id { get; set; }
        public string NumberPlate { get; set; }
        public int TypeId { get; set; }
        public decimal Kmrate { get; set; }
        public decimal TimeRate { get; set; }
        public bool IsAvailable { get; set; }
    }
}
