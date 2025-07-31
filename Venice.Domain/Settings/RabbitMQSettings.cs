namespace Venice.Domain.Settings;

public class RabbitMQSettings
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; }
    public int Port { get; set; }
    public bool SslEnabled { get; set; }
    public string SslServerName { get; set; }
}
