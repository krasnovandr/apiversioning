namespace TestWebJobDotNETCore.Configuration
{
    using TestApi.Configuration;

    public class MainSettings
    {
        public ServiceBusSettings ServiceBusSettings { get; set; }
        public string MessageToPush { get; set; }
        public string Version { get; set; }
    }
}
