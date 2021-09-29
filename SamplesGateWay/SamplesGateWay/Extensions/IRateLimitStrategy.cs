using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore
{
    public interface IRateLimitStrategy
    {
        /// <summary>
        /// true 代表不受限制
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool Request(HttpContext context);
    }
}
