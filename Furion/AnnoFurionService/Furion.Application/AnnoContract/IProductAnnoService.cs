using Anno.Rpc.Client.DynamicProxy;
using System.Threading.Tasks;

namespace Furion.Application.AnnoContract
{
    /// <summary>
    /// 对应Anno.Plugs.HelloWorldService 插件的 HelloAnnoModule 模块
    /// 接口名称和接口方法和 AnnoService端的 名称不一样的时候使用AnnoProxy 指定别名
    /// </summary>
    [AnnoProxy(Channel = "Anno.Plugs.Furion", Router = "Product")]
    public interface IProductAnnoService
    {
        Task<string> SayHi(string name);
    }
}
