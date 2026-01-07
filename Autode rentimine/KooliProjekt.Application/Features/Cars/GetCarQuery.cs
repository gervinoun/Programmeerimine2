using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Cars
{
    public class GetCarQuery : IRequest<OperationResult<object>>
    {
        public int Id { get; set; }
    }
}
