using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Anno.Plugs.Proxy.Common;
using Anno.EngineData;

using Newtonsoft.Json.Linq;
using Anno.Const;
using Anno.Const.Attribute;

namespace Anno.Plugs.Proxy
{
    /// <summary>
    /// 自动代理引擎
    /// </summary>
    public class AutoProxyEngine
    {
        private static readonly Dictionary<Type, string> _builtInTypeNames = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" },
            { typeof(void), "void" },
        };

        /// <summary>
        /// 根据<see cref="AutoProxyAttribute"/>获取需要自动生成的代理接口
        /// </summary>
        /// <param name="prefix">程序集前缀，默认为 Anno.Plugs. </param>
        public static void Create(string prefix = "Anno.Plugs.")
        {
#if !DEBUG
            Log.Log.Warn("AutoProxyEngine.Create仅支持DEBUG环境下编译生成", typeof(AutoProxyEngine));
            return;
#endif
            var assemblies = AssemblyHelper.Load(d => d.Name.StartsWith(prefix));

            var proxys = new List<Dictionary<Type, List<MethodInfo>>>();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(d => d.BaseType == typeof(BaseModule));
                var proxyDic = new Dictionary<Type, List<MethodInfo>>();
                foreach (var proxyType in types)
                {
                    var Methods = proxyType.GetMethods().Where(d => d.DeclaringType == proxyType);
                    var proxyMethods = new List<MethodInfo>();
                    foreach (var method in Methods)
                    {
                        //如果特性标记为类本身则获取所有方法
                        if (proxyType.GetAttribute<AutoProxyAttribute>() != null || method.GetAttribute<AutoProxyAttribute>() != null)
                        {
                            proxyMethods.Add(method);
                        }
                    }
                    if (proxyMethods.Count > 0)
                        proxyDic.Add(proxyType, proxyMethods);
                }
                if (proxyDic.Count > 0)
                    proxys.Add(proxyDic);
            }

            CreateProxyFile(proxys);
        }

        /// <summary>
        /// 生成文件
        /// </summary>
        /// <param name="assemblies"></param>
        private static void CreateProxyFile(List<Dictionary<Type, List<MethodInfo>>> assemblies)
        {
            var filePath = $"D:\\" + CustomConfiguration.Settings["ProxyPath"];
            foreach (var assembly in assemblies)
            {
                var interafes = new List<string>();
                var dllPath = Path.Combine(filePath, $"{assembly.First().Key.Namespace.Replace("Plugs", "Proxy")}");

                foreach (var item in assembly)
                {
                    CreateInterfaceFile(dllPath, item, interafes);
                }
                CreateCsprojFile(dllPath, assembly.First().Key);
                CreateBootstrap(dllPath, assembly.First().Key, interafes);
            }
        }

        /// <summary>
        /// 创建项目文件
        /// </summary>
        /// <param name="dllPath"></param>
        /// <param name="type"></param>
        private static void CreateCsprojFile(string dllPath, Type type)
        {
            var csprojSb = new StringBuilder();
            csprojSb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            csprojSb.AppendLine();
            csprojSb.AppendLine("  <PropertyGroup>");
            csprojSb.AppendLine("    <TargetFramework>net5.0</TargetFramework>");
            csprojSb.AppendLine("  </PropertyGroup>");
            csprojSb.AppendLine();
            csprojSb.AppendLine();
            csprojSb.AppendLine("  <ItemGroup>");
            csprojSb.AppendLine("    <PackageReference Include=\"Anno.Rpc.Client.DynamicProxy\" Version=\"1.7.0.1\" />");
            csprojSb.AppendLine("    <PackageReference Include=\"Anno.Const\" Version=\"1.7.0.1\" />");
            csprojSb.AppendLine("  </ItemGroup>");
            csprojSb.AppendLine();
            csprojSb.AppendLine();
            csprojSb.AppendLine("</Project>");

            FileHelper.Write(Path.Combine(dllPath, $"{type.Namespace.Replace("Plugs", "Proxy")}.csproj"), csprojSb.ToString());
        }

        private static void CreateBootstrap(string dllPath, Type type, List<string> interfaces)
        {
            var classSb = new StringBuilder();
            var argusings = new List<string>()
            {
                "Anno.EngineData",
                "Anno.Loader",
                "Autofac",
                "Microsoft.Extensions.DependencyInjection",
                "Anno.Rpc.Client.DynamicProxy",
            };
            var contentSb = new StringBuilder();

            contentSb.AppendLine($"{" ",8}public void ConfigurationBootstrap()");
            contentSb.AppendLine($"{" ",8}" + "{");
            contentSb.AppendLine();
            contentSb.AppendLine($"{" ",8}" + "}");
            contentSb.AppendLine();


            contentSb.AppendLine($"{" ",8}public void PreConfigurationBootstrap()");
            contentSb.AppendLine($"{" ",8}" + "{");
            contentSb.AppendLine($"{" ",12}try");
            contentSb.AppendLine($"{" ",12}" + "{");
            contentSb.AppendLine($"{" ",16}var services = IocLoader.GetAutoFacContainerBuilder();");
            interfaces.ForEach(f =>
            {
                contentSb.AppendLine($"{" ",16}services.RegisterInstance(AnnoProxyBuilder.GetService<{f}>());");
            });
            contentSb.AppendLine($"{" ",12}" + "}");
            contentSb.AppendLine($"{" ",12}catch");
            contentSb.AppendLine($"{" ",12}" + "{");
            contentSb.AppendLine($"{" ",16}var services = IocLoader.GetServiceDescriptors();");
            interfaces.ForEach(f =>
            {
                contentSb.AppendLine($"{" ",16}services.AddSingleton(AnnoProxyBuilder.GetService<{f}>());");
            });
            contentSb.AppendLine($"{" ",12}" + "}");
            contentSb.AppendLine($"{" ",8}" + "}");

            WriteUsing(argusings, classSb);

            classSb.AppendLine($"namespace {type.Namespace.Replace("Plugs", "Proxy")}");
            classSb.AppendLine("{");
            classSb.AppendLine($"{" ",4}public class Bootstrap : IPlugsConfigurationBootstrap");
            classSb.AppendLine($"{" ",4}" + "{");
            classSb.AppendLine(contentSb.ToString());
            classSb.AppendLine($"{" ",4}" + "}");
            classSb.AppendLine("}");
            FileHelper.Write(Path.Combine(dllPath, "Bootstrap.cs"), classSb.ToString());
        }

        /// <summary>
        /// 创建接口
        /// </summary>
        /// <param name="dllPath"></param>
        /// <param name="item"></param>
        private static void CreateInterfaceFile(string dllPath, KeyValuePair<Type, List<MethodInfo>> item, List<string> intefaces)
        {
            var classSb = new StringBuilder();
            var contentSb = new StringBuilder();
            var interfaceName = $"I{item.Key.Name.Replace("Plugs", "Proxy").Replace("Module", "Service")}";
            var usings = new List<string>()
            {
                "Anno.Rpc.Client.DynamicProxy",
                "Anno.Const.Attribute",
            };
            foreach (var method in item.Value)
            {
                var inputArgs = method.GetParameters();
                var outPutArgs = method.ReturnType;
                var isTask = method.ReturnType.IsAssignableTo(typeof(Task));

                //添加Using
                inputArgs?.ForEach(f =>
                {
                    if (!usings.Contains(f.ParameterType.Namespace))
                    {
                        usings.Add(f.ParameterType.Namespace);
                    }
                });

                if (!usings.Contains(outPutArgs.Namespace))
                {
                    usings.Add(outPutArgs.Namespace);
                }

                outPutArgs.GenericTypeArguments?.ForEach(f =>
                {
                    if (!usings.Contains(f.Namespace))
                        usings.Add(f.Namespace);
                });

                //添加接口
                var descInfo = method.GetCustomAttribute<AnnoInfoAttribute>();
                if (descInfo != null)
                {
                    contentSb.AppendLine($"{" ",8}/// <summary>");
                    contentSb.AppendLine($"{" ",8}/// {descInfo.Desc}");
                    contentSb.AppendLine($"{" ",8}/// </summary>");
                    inputArgs?.ForEach(f =>
                    {
                        contentSb.AppendLine($"{" ",8}/// <param name=\"{f.Name}\"></param>");
                    });
                    contentSb.AppendLine($"{" ",8}/// <returns></returns>");
                    contentSb.AppendLine($"{" ",8}[AnnoInfo(Desc = \"{descInfo.Desc}\")]");
                }
                contentSb.AppendLine($"{" ",8}{GetTypeName(outPutArgs)} {method.Name}({string.Join(",", inputArgs.Select(d => $"{GetTypeName(d.ParameterType)} {d.Name}"))});");
                if (item.Value.Count > 1)
                {
                    contentSb.AppendLine();
                }
                FilterArgs(dllPath, method, usings);
            }

            WriteUsing(usings, classSb);

            classSb.AppendLine($"namespace {item.Key.Namespace.Replace("Plugs", "Proxy")}");
            classSb.AppendLine("{");
            classSb.AppendLine($"{" ",4}[AnnoProxy(Channel = \"{item.Key.Namespace.Replace("Service", "")}\", Router = \"{item.Key.Name.Replace("Module", "")}\")]");
            classSb.AppendLine($"{" ",4}public interface {interfaceName}");
            classSb.AppendLine($"{" ",4}" + "{");
            classSb.AppendLine(contentSb.ToString());
            classSb.AppendLine($"{" ",4}" + "}");
            classSb.AppendLine("}");
            FileHelper.Write(Path.Combine(dllPath, $"{interfaceName}.cs"), classSb.ToString());
            intefaces.Add(interfaceName);
        }

        /// <summary>
        /// 过滤参数
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="method">方法信息</param>
        /// <param name="usings">引用</param>
        private static void FilterArgs(string path, MethodInfo method, List<string> usings)
        {

            var dtoPath = Path.Combine(path, "Dtos");

            var argTypes = method.GetParameters().Select(d => d.ParameterType).ToList();
            argTypes.Add(method.ReturnType);

            var types = GetMethodArgs(argTypes);

            types.ForEach(f =>
            {
                CreateClassFile(method, f, dtoPath, usings);
            });
        }

        private static List<Type> GetMethodArgs(List<Type> list)
        {
            var types = new List<Type>();
            foreach (var item in list)
            {
                if (item.IsGenericType)
                {
                    types.AddRange(GetMethodArgs(item.GenericTypeArguments.ToList()));
                }
                else
                    types.Add(item);
            }
            return types;
        }

        /// <summary>
        /// 创建参数文件
        /// </summary>
        /// <param name="method"></param>
        /// <param name="argType"></param>
        /// <param name="typePath"></param>
        /// <param name="usings"></param>
        private static void CreateClassFile(MethodInfo method, Type argType, string typePath, List<string> usings)
        {
            if (argType.Module.Name.StartsWith("System.") || argType.Namespace == "Anno.EngineData")
                return;

            if (argType.IsAbstract || argType.IsInterface)
            {
                var msg = $"warning:方法:{method.DeclaringType.Name}.{method.Name}中的参数类型{argType.Name}非法,请勿使用抽象类或接口";
                Log.Log.WriteLine(msg, ConsoleColor.Red);
                throw new Exception(msg);
            }

            var argsNamespace = $"{method.ReflectedType.Namespace.Replace("Plugs", "Proxy")}.{typePath.Split('\\').Last()}";
            usings.Add($"{argsNamespace}");
            if (usings.Contains(argType.Namespace))
            {
                usings.Remove(argType.Namespace);
            }
            var classSb = new StringBuilder();
            var argusings = new List<string>();
            var classType = "class";
            if (argType.IsEnum)
            {
                argusings.Add("System.ComponentModel");
                classType = "enum";
            }
            if (argType.IsInterface)
                classType = "interface";
            var contentSb = new StringBuilder();

            if (argType.IsEnum)
            {
                foreach (var field in argType.GetFields())
                {
                    if (field.FieldType.FullName != argType.FullName)
                        continue;
                    var attrs = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attrs.Length > 0)
                    {
                        var desc = attrs[0] is DescriptionAttribute
                            descriptionAttribute
                            ? descriptionAttribute.Description
                            : field.Name;

                        contentSb.AppendLine($"{" ",8}/// <summary>");
                        contentSb.AppendLine($"{" ",8}/// {desc}");
                        contentSb.AppendLine($"{" ",8}/// </summary>");
                        contentSb.AppendLine($"{" ",8}[Description(\"{desc}\")]");
                    }
                    contentSb.AppendLine($"{" ",8}{field.Name} = {field.GetValue(argType).ToInt()},");
                }
            }
            else if (argType.IsSubclassOfGeneric(typeof(Dictionary<,>)))
            {
                if (!argusings.Contains(argType.BaseType.Namespace))
                    argusings.Add(argType.BaseType.Namespace);
                foreach (var type in argType.BaseType.GenericTypeArguments)
                {
                    if (type.IsSubclassOfGeneric(typeof(Dictionary<,>)))
                    {
                        if (!argusings.Contains(type.BaseType.Namespace))
                            argusings.Add(type.BaseType.Namespace);
                    }
                    else
                    {
                        if (!argusings.Contains(type.Namespace))
                            argusings.Add(type.Namespace);
                    }
                    CreateClassFile(method, type.IsNullableType() ? type.GenericTypeArguments[0] : type, typePath, argusings);
                }
            }
            else
            {
                foreach (var property in argType.GetProperties())
                {
                    if (!argusings.Contains(property.PropertyType.Namespace))
                        argusings.Add(property.PropertyType.Namespace);
                    if (property.PropertyType.IsEnum || property.PropertyType.IsGenericType || !property.PropertyType.Module.Name.StartsWith("System."))
                    {
                        if (property.PropertyType.IsGenericType)
                        {
                            property.PropertyType.GenericTypeArguments.ForEach(f =>
                            {
                                CreateClassFile(method, f, typePath, argusings);
                            });
                        }
                        else
                        {
                            CreateClassFile(method, property.PropertyType.IsNullableType() ? property.PropertyType.GenericTypeArguments[0] : property.PropertyType, typePath, argusings);
                        }
                    }
                    contentSb.AppendLine($"{" ",8}{(argType.IsInterface ? "" : "public ")}{GetTypeName(property.PropertyType)} {property.Name} " + "{ get; set; }");
                }
            }

            WriteUsing(argusings, classSb);

            classSb.AppendLine($"namespace {argsNamespace}");
            classSb.AppendLine("{");
            if (argType.IsSubclassOfGeneric(typeof(Dictionary<,>)))
            {
                classSb.AppendLine($"{" ",4}public {classType} {argType.Name} : {GetTypeName(argType.BaseType)}");
            }
            else
                classSb.AppendLine($"{" ",4}public {classType} {argType.Name}");
            classSb.AppendLine($"{" ",4}" + "{");
            classSb.AppendLine(contentSb.ToString());
            classSb.AppendLine($"{" ",4}" + "}");
            classSb.AppendLine("}");

            FileHelper.Write(Path.Combine(typePath, $"{argType.Name}.cs"), classSb.ToString());
        }

        /// <summary>
        /// 写入Using
        /// </summary>
        /// <param name="usings"></param>
        /// <param name="builder"></param>
        private static void WriteUsing(List<string> usings, StringBuilder builder)
        {
            foreach (var item in usings.OrderBy(d => d).GroupBy(d => d.Split('.').First()))
            {
                item.Distinct().ForEach(f =>
                {
                    builder.AppendLine($"using {f};");
                });
                builder.AppendLine();
            }
        }

        private static string GetTypeName(Type type)
        {
            var typeName = type.Name;
            if (_builtInTypeNames.ContainsKey(type))
                typeName = _builtInTypeNames[type];
            else
            {
                if (type.IsNullableType())
                {
                    if (_builtInTypeNames.ContainsKey(type.GenericTypeArguments[0]))
                        typeName = _builtInTypeNames[type.GenericTypeArguments[0]];
                    else
                        typeName = type.GenericTypeArguments[0].Name;
                }
                else
                {
                    if (type.IsGenericType)
                    {
                        typeName = $"{type.Name.Replace($"`{type.GenericTypeArguments.Length}", "")}<{string.Join(", ", type.GenericTypeArguments.Select(d => GetTypeName(d)))}>";
                    }
                }

            }
            if (type.IsNullableType())
                typeName += "?";
            return typeName;
        }
    }
}
