using Confluent.Kafka;
using System;
using System.Threading;

class Consumer
{

    static void Main(string[] args)
    {
        Environment.GetEnvironmentVariable("");

        ConsumerConfig config = new ConsumerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVER"),
            GroupId = Environment.GetEnvironmentVariable("GROUP_ID"),
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        string topic = Environment.GetEnvironmentVariable("TOPIC_NAME");

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };

        using (var consumer = new ConsumerBuilder<string, string>(
            config).Build())
        {
            consumer.Subscribe(topic);
            try
            {
                while (true)
                {
                    var cr = consumer.Consume(cts.Token);
                    Console.WriteLine($"Consumed event from topic {topic} with key {cr.Message.Key,-10} and value {cr.Message.Value}");
                }
            }
            catch (OperationCanceledException)
            {
                // Ctrl-C was pressed.
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}