using Azure.Core;
using MediatR;
using UrlService.Repositories;

namespace UrlService.Services.UrlShortening.Queries
{
    public class GetOriginalUrlQuery : IRequest<string>
    {
        public string ShortCode { get; }
        public GetOriginalUrlQuery(string shortCode)
        {
            ShortCode = shortCode;
        }
    }

    public class GetOriginalUrlQueryHandler : IRequestHandler<GetOriginalUrlQuery, string>
    {
        private readonly IUrlRepository _urlRepository;

        public GetOriginalUrlQueryHandler(IUrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }

        public async Task<string> Handle(GetOriginalUrlQuery request, CancellationToken cancellationToken)
        {
            var urlEntity = await _urlRepository.GetByShortCodeAsync(request.ShortCode);
            return urlEntity?.OriginalUrl ?? throw new KeyNotFoundException("Short Url not found");
        }
    }
}
