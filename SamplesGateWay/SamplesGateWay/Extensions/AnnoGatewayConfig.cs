namespace Microsoft.AspNetCore
{
    /// <summary>
    /// Anno网关配置文件
    /// </summary>
    public class AnnoGatewayConfig
    {
        public string AppName { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool TraceOnOff { get; set; }
    }
}
