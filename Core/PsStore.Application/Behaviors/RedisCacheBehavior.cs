using MediatR;
using PsStore.Application.Interfaces.RedisCache;

namespace PsStore.Application.Behaviors
{
    public class RedisCacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IRedisCacheService redisCacheService;

        public RedisCacheBehavior(IRedisCacheService redisCacheService)
        {
            this.redisCacheService = redisCacheService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is ICacheableQuery query)
            {
                var cacheKey = query.CacheKey;
                var cacheTime = query.CacheTime;

                var cacheData = await redisCacheService.GetAsync<TResponse>(cacheKey);
                if (cacheData is not null)
                    return cacheData;

                var response = await next();
                if (response is not null)
                    await redisCacheService.SetASync(cacheKey, response, DateTime.Now.AddMinutes(cacheTime));

                return response;
            }

            return await next();
        }
    }

}
