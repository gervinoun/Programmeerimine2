using System;
using KooliProjekt.Application.Behaviors;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Bookings
{
    public class SaveBookingCommand : IRequest<OperationResult>, ITransactional
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal KmStart { get; set; }
        public decimal KmEnd { get; set; }
        public string Status { get; set; }
    }
}
