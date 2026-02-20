using ConsoleClient.Enums;
using Microsoft.Extensions.Configuration;

namespace ConsoleClient.Clients.Urls
{
    internal class Url
    {
        private readonly string baseUrl;
        
        private class Ports
        {
            public uint authPort = 80;
            public uint messageRealTimePort = 80;
            public uint restPort = 80;
        }

        private readonly Ports ports;

        public Url(bool productionUrls)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            if (productionUrls)
            {
                configurationBuilder.AddJsonFile("urls.Production.json");
            }
            else
            {
                configurationBuilder.AddJsonFile("urls.Development.json");
            }

            IConfiguration configuration = configurationBuilder.Build();
            baseUrl = configuration.GetValue<string>("BaseUrl")
                        ?? throw new Exception("Failed to get BaseUrl");

            ports = new Ports();
            if (!productionUrls)
            {
                ports.authPort = UInt32.Parse(
                                    configuration.GetValue<string>("Ports:Auth")
                                    ?? throw new Exception("Failed to get Ports:Auth"));
                ports.messageRealTimePort = UInt32.Parse(
                                    configuration.GetValue<string>("Ports:MessageRealTime")
                                    ?? throw new Exception("Failed to get Ports:MessageRealTime"));
                ports.restPort = UInt32.Parse(
                                    configuration.GetValue<string>("Ports:REST")
                                    ?? throw new Exception("Failed to get Ports:REST"));
            }
        }

        private uint Port(Service service)
        {
            switch (service)
            {
                case Service.Auth:
                    return ports.authPort;
                case Service.MessageRealTime:
                    return ports.messageRealTimePort;
                case Service.REST:
                    return ports.restPort;
                default:
                    throw new Exception($"Port method don't have a case for {service.ToString()}.");
            }
        }

        public string FromControllerAction(Service service, Controller controller, string action)
        {
            return $"{baseUrl}:{Port(service)}/{controller.ToString()}/{action}";
        }

        public string ChatHub()
        {
            return $"{baseUrl}:{Port(Service.MessageRealTime)}/{Service.MessageRealTime.ToString()}";
        }
    }
}
