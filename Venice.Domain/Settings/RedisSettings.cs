namespace Venice.Domain.Settings;

public class RedisSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string ConnectionString { get; set; }
}
