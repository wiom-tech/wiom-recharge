using Confluent.Kafka;
using Confluent.Kafka.Admin;
using i2e1_basics.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace i2e1_core.Utilities
{
    public delegate void KafkaCallback(KafkaMessage kafkaMessage);
    public class KafkaMessage
    {
        public string name { get; set; }
        public Dictionary<string, string> attributes { get; set; }
    }

    public class kafkaHelper
    {
        public static AdminClientConfig adminConfig = new AdminClientConfig
        {
            BootstrapServers = I2e1ConfigurationManager.GetInstance().GetSetting("KafkaBroker"),
            ClientId = "i2e1-backend-admin"
        };

        public static ProducerConfig producerConfig = new ProducerConfig
        {
            BootstrapServers = I2e1ConfigurationManager.GetInstance().GetSetting("KafkaBroker"), // Kafka broker address
            ClientId = "i2e1-backend-producer", // Client ID for the producer
                                      // Add any additional configuration properties as needed
        };
        public static ConsumerConfig consumerConfig = new ConsumerConfig
        {
            BootstrapServers = I2e1ConfigurationManager.GetInstance().GetSetting("KafkaBroker"),
            AutoOffsetReset = AutoOffsetReset.Earliest
            // Add any additional configuration properties as needed
        };

        public static void CreateTopic(string topic, int numPartitions = 1, short replicationFactor = 1)
        {
            using (var adminClient = new AdminClientBuilder(adminConfig).Build())
            {
                try
                {
                    var topicSpecs = new List<TopicSpecification>
            {
                new TopicSpecification
                {
                    Name = topic,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor
                }
            };

                    adminClient.CreateTopicsAsync(topicSpecs).Wait();
                    Console.WriteLine($"Topic '{topic}' created successfully.");
                }
                catch (CreateTopicsException ex)
                {
                    Console.WriteLine($"An error occurred while creating topic '{topic}':");
                    foreach (var error in ex.Results)
                    {
                        Console.WriteLine($"- Topic: {error.Topic} Error: {error.Error.Reason}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred while creating topic '{topic}': {ex.Message}");
                }
            }
        }




        public static void DeleteTopic(string topicName)
        {
            using (var adminClient = new AdminClientBuilder(adminConfig).Build())
            {
                try
                {
                    adminClient.DeleteTopicsAsync(new[] { topicName }).Wait();
                    Console.WriteLine($"Topic '{topicName}' deleted successfully.");
                }
                catch (CreateTopicsException ex)
                {
                    Console.WriteLine($"An error occurred while deleting topic '{topicName}':");
                    foreach (var error in ex.Results)
                    {
                        Console.WriteLine($"- Topic: {error.Topic} Error: {error.Error.Reason}");
                    }
                }
            }
        }

        public static List<string> GetTopics()
        {
            using (var adminClient = new AdminClientBuilder(adminConfig).Build())
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                return metadata.Topics.Select(topic => topic.Topic).ToList();
            }
        }

        public static TopicMetadata GetTopicDetails(string topicName)
        {
            using (var adminClient = new AdminClientBuilder(adminConfig).Build())
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                return metadata.Topics.FirstOrDefault(topic => topic.Topic.Equals(topicName));
            }
        }

        public static void ProduceMessages(string topicName, KafkaMessage value)
        {
            using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                try
                {
                    var message = new Message<Null, string> { Key = null, Value = JsonConvert.SerializeObject(value) };
                    var deliveryResult = producer.ProduceAsync(topicName, message).GetAwaiter().GetResult();
                    if (deliveryResult.Status == PersistenceStatus.Persisted)
                    {
                        Console.WriteLine($"Message sent successfully. Topic: {deliveryResult.Topic}, Partition: {deliveryResult.Partition}, Offset: {deliveryResult.Offset}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send message.");
                    }

                    producer.Flush(TimeSpan.FromSeconds(10));
                }
                catch (ProduceException<Null, string> ex)
                {
                    Console.WriteLine($"An error occurred while producing message. Error: {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred while producing message: {ex.Message}");
                }
            }
        }



        public static void ConsumeMessages(string topic, string groupId, KafkaCallback kafkaCallback)
        {
            consumerConfig.GroupId = groupId;
            var localConsumerConfig = consumerConfig;


            CoreTask.Run("Kafka_polling_task", () =>
            {
                using (var consumer = new ConsumerBuilder<Ignore, string>(localConsumerConfig).Build())
                {
                    consumer.Subscribe(topic);

                    try
                    {
                        while (true)
                        {
                            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
                            if (consumeResult != null)
                            {
                                var kafkaMsg = JsonConvert.DeserializeObject<KafkaMessage>(consumeResult.Message.Value);
                                kafkaCallback(kafkaMsg);
                                Console.WriteLine($"Received message: Key = {consumeResult.Message.Key}, Value = {consumeResult.Message.Value}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while consuming messages from topic '{topic}': {ex.Message}");
                    }
                    finally
                    {
                        consumer.Close();
                    }
                }
            });
        }




        public static void ConsumeMessagesInBatch(string topic, string groupId, KafkaCallback kafkaCallback, int batchSize = 10)
        {
            consumerConfig.GroupId = groupId;
            var localConsumerConfig = consumerConfig;

            using (var consumer = new ConsumerBuilder<Ignore, string>(localConsumerConfig).Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while (true)
                    {
                        var batchCount = 0;
                        while (batchCount < batchSize)
                        {
                            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
                            if (consumeResult != null)
                            {
                                var kafkaMsg = JsonConvert.DeserializeObject<KafkaMessage>(consumeResult.Message.Value);
                                kafkaCallback(kafkaMsg);
                                Console.WriteLine($"Received message: Key = {consumeResult.Message.Key}, Value = {consumeResult.Message.Value}");
                                batchCount++;
                            }
                        }

                        consumer.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while consuming messages from topic '{topic}': {ex.Message}");
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

    }
}
