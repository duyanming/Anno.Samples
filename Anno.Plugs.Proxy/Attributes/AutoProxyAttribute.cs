using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Plugs.Proxy
{
    /// <summary>
    /// 自动生成代理类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AutoProxyAttribute : Attribute
    {

    }

    public static class AutoProxyAttributeExtension
    {

        /// <summary>
        /// 从类型成员获取指定Attribute特性
        /// </summary>
        /// <typeparam name="T">Attribute特性类型</typeparam>
        /// <param name="memberInfo">类型类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>存在返回第一个，不存在返回null</returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = true) where T : Attribute
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(T), inherit);
            return attributes.FirstOrDefault() as T;
        }
    }
}
