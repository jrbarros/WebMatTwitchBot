﻿using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebMatBot.Lights;
using System.Linq;
using System.Net.Mail;

namespace WebMatBot.Core
{
    public static class PubSubEngine
    {
        private static ClientWebSocket webSocket { get; set; }

        private readonly static IDictionary<string, Action<IDictionary<string, object>>> Topics = new Dictionary<string, Action<IDictionary<string, object>>> 
        {
            { $"channel-bits-events-v1.{Parameters.ChannelID}", async (line) => await TasksQueueOutput.QueueAddSpeech( async() => await TasksQueueOutput.QueueAddSpeech( async() => await AudioVisual.Party("", Parameters.User))) },
            { $"channel-bits-badge-unlocks.{Parameters.ChannelID}" ,async (line) => await TasksQueueOutput.QueueAddSpeech( async() => await TasksQueueOutput.QueueAddSpeech( async() => await AudioVisual.Party("", Parameters.User))) } ,
            { $"channel-points-channel-v1.{Parameters.ChannelID}", async (line) => await ChannelPoints(line) },
            { $"channel-subscribe-events-v1.{Parameters.ChannelID}",async (line) => await TasksQueueOutput.QueueAddSpeech( async() => await TasksQueueOutput.QueueAddSpeech( async() => await AudioVisual.Party("", Parameters.User))) },
        };
        private readonly static string Auth = Parameters.OAuth.Split(":")[1]; //https://twitchtokengenerator.com/

        public static async void Start()
        {
            Task.Run(() => PingPong());

            do
            {
                using (var socket = new ClientWebSocket())
                    try
                    {
                        await socket.ConnectAsync(new Uri("wss://pubsub-edge.twitch.tv"), CancellationToken.None);

                        webSocket = socket;

                        

                        var obj = new
                        {
                            type = "LISTEN",
                            data = new
                            {
                                topics = Topics.Keys,
                                auth_token = Auth,
                            },
                        };

                        await Send(Newtonsoft.Json.JsonConvert.SerializeObject(obj), CancellationToken.None);

                        //reponde para o chat
                        //await Engine.Respond("Ouvindo PubSub... Começando a SkyNet...");

                        await Receive(CancellationToken.None);

                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"ERROR - {ex.Message}");
                    }
            } while (true);
        }

        public static async Task Receive(CancellationToken stoppingToken)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            while (!stoppingToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await webSocket.ReceiveAsync(buffer, stoppingToken);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        var input = await reader.ReadToEndAsync();

                        await Analizer(input);
                    }
                }
            };
        }

        private static Task Analizer(string JsonInput)
        {
            var Obj = new { type = "", data = new { topic = "", message = ""} };
            Obj = JsonConvert.DeserializeAnonymousType(JsonInput, Obj);
            if (Obj.type == "MESSAGE")
            {
                IDictionary<string, object> message = JsonConvert.DeserializeObject<Dictionary<string, object>>(Obj.data.message);

                return CheckCommand(message, Obj.data.topic);
            }

            return Task.CompletedTask;
            
        }
        private static async Task CheckCommand(IDictionary<string, object> message, string topic)
        {
            Action<IDictionary<string, object>> command = null;
            bool isDone = false;

            // verifica comandos
            for (int i = 0; i < Topics.Count && !isDone; i++)
            {
                if (topic == Topics.ElementAt(i).Key)
                {
                    command = Topics.ElementAt(i).Value;
                    isDone = true;
                }
            }

            if (isDone)
                command.Invoke(message);
        }


        public static async Task Send(string data, CancellationToken stoppingToken) =>
            await webSocket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, stoppingToken);

        public static async void PingPong()
        {
            while (true)
            {
                await Task.Delay(new TimeSpan(0,3,0));
                if (webSocket.State == WebSocketState.Open)
                    await Send("PING", CancellationToken.None);
            }
        }


        private static async Task ChannelPoints(IDictionary<string, object> message)
        {

            var data = message["data"];
            var redemption = ((Newtonsoft.Json.Linq.JObject)data)["redemption"];
            var reward = ((Newtonsoft.Json.Linq.JObject)redemption)["reward"];
            var title = ((Newtonsoft.Json.Linq.JObject)reward).GetValue("title").ToString();

            if (title.ToLower().Contains("xandão"))
            {
                await TasksQueueOutput.QueueAddSpeech(async () => await TasksQueueOutput.QueueAddSpeech(async () => await AudioVisual.Xandao("", Parameters.User)));
            }
        }
    }
}
