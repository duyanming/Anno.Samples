using Anno.Rpc.Client.DynamicProxy;

namespace Furion.Application.AnnoContract
{
    /// <summary>
    /// 对应Anno.Plugs.HelloWorldService 插件的 HelloAnnoModule 模块
    /// 接口名称和接口方法和 AnnoService端的 名称不一样的时候使用AnnoProxy 指定别名
    /// </summary>
    [AnnoProxy(Channel = "Anno.Plugs.CurrentLimiting")]
    public interface ICurrentLimitingService
    {
        #region Action
        /// <summary>
        /// 和方法名称一致合一不用写注解
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [AnnoProxy(Router = "Action_CurrentLimiting", Method = "SayHi_TokenBucket")]
        string SayHi_TokenBucket_Action(string name);

        [AnnoProxy(Router = "Action_CurrentLimiting", Method = "GoodBye")]
        string GoodBye_Action(string name);
        #endregion

        #region Module
        /// <summary>
        /// 和方法名称一致合一不用写注解
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [AnnoProxy(Router = "Module_CurrentLimiting", Method = "SayHi")]
        string SayHi_Module(string name);

        [AnnoProxy(Router = "Module_CurrentLimiting", Method = "GoodBye")]
        string GoodBye_Module(string name);
        #endregion

        #region Global
        /// <summary>
        /// 和方法名称一致合一不用写注解
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [AnnoProxy(Router = "Global_CurrentLimiting", Method = "SayHi")]
        string SayHi_Global(string name);

        [AnnoProxy(Router = "Global_CurrentLimiting", Method = "GoodBye")]
        string GoodBye_Global(string name);
        #endregion
    }
}
