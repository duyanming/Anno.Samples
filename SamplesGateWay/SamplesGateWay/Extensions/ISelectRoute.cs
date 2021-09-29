using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore
{
    public interface ISelectRoute
    {
        /// <summary>
        /// 选择一个服务（昵称 nickName）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        string SelectRoute(HttpContext context);
    }
}
