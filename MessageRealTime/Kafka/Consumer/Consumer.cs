using Confluent.Kafka;
using MessageRealTime.Data;
using MessageRealTime.Dtos;
using MessageRealTime.Kafka.EventTypes;
using MessageRealTime.Kafka.Keys;
using MessageRealTime.Kafka.Values;
using MessageRealTime.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace MessageRealTime.Kafka
{
    public class Consumer : IConsumer
    {
        // Kafka consumer
        private readonly string bootstrapServers;
        private const string topic = "my-topic";
        private const string groupId = "someGroupId";
        private readonly IConsumer<string, string> consumer;

        private readonly IDataAccess _dataAccess;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;

        public Consumer(IConfiguration configuration, IDataAccess dataAccess, IHubContext<ChatHub, IChatClient> hubContext)
        {
            _dataAccess = dataAccess;
            _hubContext = hubContext;

            Console.WriteLine("Constructing KafkaConsumer...");

            // Get bootstrapServers, for Kafka
            {
                string? kbs = configuration.GetValue<string>("kafkaBootstrapServers");
                if (kbs == null)
                {
                    throw new Exception("Couldn't get kafkaBootstrapServers from configuration files.");
                }
                bootstrapServers = kbs;
            }

            // Kafka consumer
            ConsumerConfig consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe(topic);
        }

        public async Task ConsumeMessagesFromKafkaAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("Starting to consume messages from Kafka...");

                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<string, string> consumeResult = consumer.Consume();
                    Console.WriteLine($"Consumed message: key = {consumeResult.Message.Key}, value = {consumeResult.Message.Value}");
                    
                    try
                    {
                        await ProcessConsumedMessage(consumeResult);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nError processing consumed message: {ex.Message}\n");
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                consumer.Close();
            }
        }

        private async Task ProcessConsumedMessage(ConsumeResult<string, string> consumeResult)
        {
            Console.WriteLine("KafkaConsumer processing consumed message...");

            // First deserialize the key
            string serializedKey = consumeResult.Message.Key;
            Key? key = JsonSerializer.Deserialize<Key>(serializedKey);
            if (key == null)
            {
                throw new Exception("Error: Null key.");
            }

            // Use the information in the key to deserialize the value
            string serializedValue = consumeResult.Message.Value;
            switch(key.EventType)
            {
                case EventType.MESSAGE_UPDATED_EVENT:
                    await ProcessEventMessageUpdated(serializedValue);
                    break;
                /*
                case EventType.ROOM_CREATED_EVENT:
                    await ProcessEventRoomCreated(serializedValue);
                    break;
                case EventType.ROOM_DELETED_EVENT:
                    await ProcessEventRoomDeleted(serializedValue);
                    break;
                case EventType.ADD_USER_TO_ROOM_EVENT:
                    await ProcessEventAddUserToRoom(serializedValue);
                    break;
                case EventType.REMOVE_USER_FROM_ROOM_EVENT:
                    await ProcessEventRemoveUserFromRoom(serializedValue);
                    break;
                */
                default:
                    Console.WriteLine("Warning: Event not processed");
                    break;
            }

            Console.WriteLine("KafkaConsumer consumed message successfully.");
        }

        private string GroupName(int roomId)
        {
            return Convert.ToString(roomId);
        }

        private async Task ProcessEventMessageUpdated(string serializedValue)
        {
            Console.WriteLine("ProcessEventMessageUpdated");
            MessageUpdated? value = Serializer<MessageUpdated>.Deserialize(serializedValue);
            if(value == null)
            {
                Console.WriteLine("Error: Failed to deserialize Kafka value.");
                return;
            }

            // Notify the client that the message was updated
            string groupName = GroupName(value.RoomId);
            NotificationDto notificationDto = new()
            {
                Content = Serializer<MessageUpdated>.Serialize(value)
            };
            await _hubContext.Clients.Group(groupName).ReceiveNotificationAsync(notificationDto);
        }

        /*
        private async Task ProcessEventRoomCreated(string serializedValue)
        {
            Console.WriteLine("ProcessEventRoomCreated");
            RoomCreated? value = Serializer<RoomCreated>.Deserialize(serializedValue);
            if (value == null)
            {
                Console.WriteLine("Error: Failed to deserialize Kafka value.");
                return;
            }
        }

        private async Task ProcessEventRoomDeleted(string serializedValue)
        {
            Console.WriteLine("ProcessEventRoomDeleted");
            RoomDeleted? value = Serializer<RoomDeleted>.Deserialize(serializedValue);
            if (value == null)
            {
                Console.WriteLine("Error: Failed to deserialize Kafka value.");
                return;
            }
        }

        private async Task ProcessEventAddUserToRoom(string serializedValue)
        {
            Console.WriteLine("ProcessEventAddUserToRoom");
            AddUserToRoom? value = Serializer<AddUserToRoom>.Deserialize(serializedValue);
            if (value == null)
            {
                Console.WriteLine("Error: Failed to deserialize Kafka value.");
                return;
            }
        }

        private async Task ProcessEventRemoveUserFromRoom(string serializedValue)
        {
            Console.WriteLine("ProcessEventRemoveUserFromRoom");
            RemoveUserFromRoom? value = Serializer<RemoveUserFromRoom>.Deserialize(serializedValue);
            if (value == null)
            {
                Console.WriteLine("Error: Failed to deserialize Kafka value.");
                return;
            }
        }
        */

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            GC.SuppressFinalize(this);
            consumer.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
