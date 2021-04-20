using Furion.Application.AnnoContract;
using Furion.DynamicApiController;
using System.Threading.Tasks;

namespace Furion.Application
{
    public class FurionAppService : IDynamicApiController
    {
        private readonly IHelloAnnoService _helloAnnoService;

        public FurionAppService(IHelloAnnoService helloAnnoService)
        {
            _helloAnnoService = helloAnnoService;
        }
        public string Get()
        {
            return $"Hello {nameof(Furion)}";
        }
        /// <summary>
        /// 说问候，你好啊 SayHi
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string SayHi(string name)
        {
            return _helloAnnoService.SayHi(name);
        }
        /// <summary>
        /// 异步 说问候，你好啊 TaskSayHi
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<string> TaskSayHi(string name)
        {
            return await _helloAnnoService.TaskSayHi(name);
        }
        /// <summary>
        /// 说再见 GoodBye
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GoodBye(string name)
        {
            return _helloAnnoService.GoodBye(name);
        }

        /// <summary>
        /// 说再见 别名 GoodByeAlias
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GoodByeAlias(string name)
        {
            return _helloAnnoService.GoodByeAlias(name);
        }
    }
}