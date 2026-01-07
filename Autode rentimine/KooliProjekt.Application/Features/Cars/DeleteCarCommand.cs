using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Cars
{

    public class DeleteCarCommand : IRequest<OperationResult>
    {
        public int Id { get; set; }
    }
}
