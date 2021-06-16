using System;
using System.IO;
using Anno.Rpc.Server;
using Topshelf;

namespace SamplesWinService
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
                x.Service<AnnoServiceControl>();
                x.RunAsLocalSystem();

                x.SetDescription("Anno Win 服务");
                x.SetDisplayName("Anno Service");
                x.SetServiceName("AnnoService");

                x.StartAutomatically();
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
