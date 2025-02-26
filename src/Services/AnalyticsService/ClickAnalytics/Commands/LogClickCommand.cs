using AnalyticsService.Models;
using AnalyticsService.Repositories;
using MediatR;
using Shared.Domain.Abstractions;

namespace AnalyticsService.ClickAnalytics.Commands
{
    public class LogClickCommand : IRequest
    {
        public string ShortCode { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? CreatedBy { get; set; }
    }

    public class LogClickCommandHandler : IRequestHandler<LogClickCommand>
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        public LogClickCommandHandler(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }
        public async Task Handle(LogClickCommand request, CancellationToken cancellationToken)
        {
            var clickRecord = new ClickRecord
            {
                ShortCode = request.ShortCode,
                UserId = request.UserId,
                IpAddress = request.IpAddress,
                CreatedDate = request.CreatedDate
            };

            await _analyticsRepository.LogClickAsync(clickRecord);
        }
    }
}
