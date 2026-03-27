using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace i2e1_core.Utilities
{
    public class MQTTManager
    {
        private static string username = "i2e1_mqtt";
        private static string password = "thereisnopasswd";
        private static string mqttURI = "mqtt.i2e1.com";

        private static string usernameSSL = "wiom";
        private static string passwordSSL = "d3rt5490549054856mxci0ru";
        private static string mqttSSLURI = "mqtt.i2e1.in";

        public static void Publish(string topic, string data,uint ttl=3* 24 * 60 * 60)
        {
            try
            {
                string prefix = "prod";
                if (!Constants.IS_PRODUCTION)
                {
                    prefix = "test";
                }

                string finalTopic = $"{prefix}/{topic}";

                var options = new MqttClientOptionsBuilder()
                           .WithTcpServer(mqttURI, 1883)
                           .WithClientId("1111")
                           .WithCredentials(username, password)
                           .WithCleanSession()
                           .Build();

                //Getting an mqtt Instance
                IMqttClient MqttClient = new MqttFactory().CreateMqttClient();

                //Wiring up all the events...



                MqttClient.ApplicationMessageReceivedAsync += (e => 
                {
                    return Task.Run(() =>
                    {
                        Console.WriteLine(e.ApplicationMessage);
                    });
                });
                //MqttClient.ConnectedAsync += (/*async*/ e =>
                //{
                //    return Task.Run(() =>
                //    {
                //        Console.WriteLine("### CONNECTED WITH BROKER ###");
                //    });
                //});

                MqttClient.ConnectAsync(options).ContinueWith((m)=> {
                    MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                    .WithMessageExpiryInterval(ttl)
                    .WithTopic(finalTopic)
                    .WithPayload(data)
                    .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)2)
                    .WithRetainFlag(false)
                    .Build());
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"failed to publish {data} to topic {topic}");
                Console.WriteLine(e.Message);
            }

        }

        public static void PublishSSL(string topic, string data, uint ttl = 3 * 24 * 60 * 60)
        {
            try
            {
                string prefix = "prod";
                if (!Constants.IS_PRODUCTION)
                {
                    prefix = "test";
                }

                string finalTopic = $"{prefix}/{topic}";

                var options = new MqttClientOptionsBuilder()
                           .WithTcpServer(mqttSSLURI, 8883)
                           .WithClientId("1111")
                           .WithCredentials(usernameSSL, passwordSSL)
                           .WithCleanSession()
                           .WithTls()
                           .Build();

                //Getting an mqtt Instance
                IMqttClient MqttClient = new MqttFactory().CreateMqttClient();

                //Wiring up all the events...



                MqttClient.ApplicationMessageReceivedAsync += (e =>
                {
                    return Task.Run(() =>
                    {
                        Console.WriteLine(e.ApplicationMessage);
                    });
                });
                //MqttClient.ConnectedAsync += (/*async*/ e =>
                //{
                //    return Task.Run(() =>
                //    {
                //        Console.WriteLine("### CONNECTED WITH BROKER ###");
                //    });
                //});

                MqttClient.ConnectAsync(options).ContinueWith((m) => {
                    MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                    .WithMessageExpiryInterval(ttl)
                    .WithTopic(finalTopic)
                    .WithPayload(data)
                    .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)2)
                    .WithRetainFlag(false)
                    .Build());
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"failed to publish {data} to topic {topic}");
                Console.WriteLine(e.Message);
            }

        }

    }
}
