using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anno.Rpc.Client.DynamicProxy;

namespace ConsoleClient.Contract
{
    /// <summary>
    /// 对应Anno.Plugs.HelloWorldService 插件的 HelloAnnoModule 模块
    /// 接口名称和接口方法和 AnnoService端的 名称不一样的时候使用AnnoProxy 指定别名
    /// </summary>
    [AnnoProxy(Channel = "Anno.Plugs.HelloWorld", Router = "HelloAnno")]
    public interface IHelloAnnoService
    {
        /// <summary>
        /// 和方法名称一致合一不用写注解
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //[AnnoProxy(Method = "SayHi")]
        // 客户端 暂时不要写Task
        //Task<string> SayHi(string name);
        string SayHi(string name);


        string GoodBye(string name);

        [AnnoProxy(Method = "GoodBye")]
        string GoodByeAlias(string name);
    }
}
