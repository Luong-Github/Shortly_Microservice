using AnalyticsService.Repositories;
using MediatR;

namespace AnalyticsService.ClickAnalytics.Queries
{
    public class GetTotalClickQuery : IRequest<int>
    {
        public string ShortCode { get; }
        public GetTotalClickQuery(string shortCode)
        {
            ShortCode = shortCode;
        }
    }

    public class GetTotalClickQueryHandler : IRequestHandler<GetTotalClickQuery, int>
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        public GetTotalClickQueryHandler(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }

        public async Task<int> Handle(GetTotalClickQuery request, CancellationToken cancellationToken)
        {
            return await _analyticsRepository.GetTotalClickAsync(request.ShortCode);
        }
    }
}
