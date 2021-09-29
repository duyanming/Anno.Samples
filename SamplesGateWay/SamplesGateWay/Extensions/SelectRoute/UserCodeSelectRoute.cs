using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Microsoft.AspNetCore
{
    public class UserCodeSelectRoute : ISelectRoute
    {
        /// <summary>
        /// 根据用户get参数city ，选取一个服务昵称中包含city的服务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string SelectRoute(HttpContext context)
        {
            string nickName = string.Empty;
            var Request = context.Request;
            //var headers = Request.Headers;
            //过滤服务的条件参数，可以根据业务自己定义
            string city = Request.Query["city"];
            var services = Anno.Rpc.Client.Connector.Micros;
            if (!string.IsNullOrEmpty(city) && services != null)
            {
                var service = services.FirstOrDefault(s => s.Mi.Nickname.Contains(city));
                if (service != null)
                {
                    nickName = service.Mi.Nickname;
                }
            }
#if DEBUG
            Console.WriteLine($"city:{city},Route:{nickName}.");
#endif
            return nickName;
        }
    }
}
