using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Plugs.Proxy.Common
{
    public static class CommonExtensions
    {

        /// <summary>
        /// 将object转换为string类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>string。</returns>
        public static string ToString(this object o, string t)
        {
            string info = string.Empty;
            if (o == null)
            {
                info = t;
            }
            else
            {
                info = o.ToString();
            }
            return info;
        }

        /// <summary>
        /// 将object转换为float类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>float。</returns>
        public static float ToFloat(this object o, float t = default(float))
        {
            float info;
            if (!float.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 转换成Int/Int32
        /// </summary>
        /// <param name="s"></param>
        /// <param name="round">是否四舍五入，默认false</param>
        /// <returns></returns>
        public static int ToInt(this object s, bool round = false)
        {
            if (s == null || s == DBNull.Value)
                return 0;

            if (s.GetType().IsEnum)
            {
                return (int)s;
            }

            if (s is bool b)
                return b ? 1 : 0;

            if (int.TryParse(s.ToString(), out int result))
                return result;

            var f = s.ToFloat();
            return round ? Convert.ToInt32(f) : (int)f;
        }

        /// <summary>
        /// 判断类型是否为Nullable类型
        /// </summary>
        /// <param name="type"> 要处理的类型 </param>
        /// <returns> 是返回True，不是返回False </returns>
        public static bool IsNullableType(this Type type)
        {
            return ((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
