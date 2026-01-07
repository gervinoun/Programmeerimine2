using KooliProjekt.Application.Behaviors;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Cars
{
    public class SaveCarCommand : IRequest<OperationResult>, ITransactional
    {
        public int Id { get; set; }
        public string NumberPlate { get; set; }
        public int TypeId { get; set; }
        public decimal Kmrate { get; set; }
        public decimal TimeRate { get; set; }
        public bool IsAvailable { get; set; }
    }
}
