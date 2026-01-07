using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.CarTypes
{

    public class DeleteCarTypeCommand : IRequest<OperationResult>
    {
        public int Id { get; set; }
    }
}
