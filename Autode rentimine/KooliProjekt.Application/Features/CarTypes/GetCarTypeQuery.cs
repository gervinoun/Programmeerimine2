using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.CarTypes
{
    public class GetCarTypeQuery : IRequest<OperationResult<object>>
    {
        public int Id { get; set; }
    }
}
