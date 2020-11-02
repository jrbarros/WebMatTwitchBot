﻿using Google.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static WebMatBot.ScheduledMessage;

namespace WebMatBot
{
    public class AutomaticMessages
    {
        public static DateTime LastMessage { get; set; } = DateTime.Now.AddMinutes(-5);
        private static TimeSpan SpaceBetweenMessages = new TimeSpan(0,30,0);

        private static List<ScheduledMessage> Queue = new List<ScheduledMessage>();


        public static async void Start()
        {
            //inicialização tempo
            await Task.Delay(new TimeSpan(0, 1, 0));

            lock (Queue)
            {
                Queue.Add(DrinkWater(DateTime.Now));
                Queue.Add(Discord(DateTime.Now));
                Queue.Add(Youtube(DateTime.Now));
                Queue.Add(GitHub(DateTime.Now));
                Queue.Add(Donate(DateTime.Now));
                Queue.Add(Form(DateTime.Now));
            }


            do
            {
                ScheduledMessage Item;

                lock (Queue)
                    Item = Queue.OrderBy(q => q.DateSchedule).FirstOrDefault(item => item.DateSchedule <= DateTime.Now && DateTime.Now >= LastMessage.Add(SpaceBetweenMessages));

                if (Item != null)
                    Item.Action.Invoke(Item);

                await Task.Delay(20000);
            }
            while (true);
        }

        public static void AddQueue(ScheduledMessage Item)
        {
            lock (Queue)
                Queue.Add(Item);
        }

        public static void RemoveQueue(MessageType type)
        {
            ScheduledMessage item;
            lock (Queue)
                item = Queue.FirstOrDefault(q => q.TypeInfo == type);

            if (item != null)
                Queue.Remove(item);
        }

    }
}
