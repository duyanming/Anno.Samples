using System;
using System.IO;
using Anno.Rpc.Center;
using Topshelf;

namespace SamplesCenterWinService
{
    public class Program
    {
        public static string[] Args;

        /// <summary>
        /// Windows服务 Anno注册中心
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Program.Args = args;
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var rc = HostFactory.Run(x =>
            {
                x.Service<AnnoCenterServiceControl>();
                x.RunAsLocalSystem();

                x.SetDescription("Anno 注册中心");
                x.SetDisplayName("Anno Center Service");
                x.SetServiceName("AnnoCenterService");

                x.StartAutomatically();
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
