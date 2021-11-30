using Anno.Rpc.Client.DynamicProxy;

namespace SamplesWebApi.Contract
{/// <summary>
 /// 对应Anno.Plugs.HelloWorldService 插件的 HelloAnnoModule 模块
 /// 接口名称和接口方法和 AnnoService端的 名称不一样的时候使用AnnoProxy 指定别名
 /// </summary>
    [AnnoProxy(Channel = "Jky.Web.Core.ModelManage", Router = "SayHi")]
    public interface INonstandardService
    {

        /// <summary>
        /// 和方法名称一致合一不用写注解
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string SayHi(string name);
    }
}
