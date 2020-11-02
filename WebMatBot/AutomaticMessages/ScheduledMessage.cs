﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WebMatBot.Core;

namespace WebMatBot
{
    public class ScheduledMessage
    {
        public DateTime DateSchedule { get; set; }
        public Action<ScheduledMessage> Action { get; set; }
        public TimeSpan WaitingTime { get; set; }
        public string Message { get; set; }
        public MessageType TypeInfo { get; set; }

        public ScheduledMessage(DateTime dateSche, Action<ScheduledMessage> act, TimeSpan waitTime, string message, MessageType type)
        {
            DateSchedule = dateSche;
            Action = act;
            WaitingTime = waitTime;
            Message = message;
            TypeInfo = type;
        }

        public static ScheduledMessage Discord(DateTime date) =>
            new ScheduledMessage(date, async (ScheduledMessage schM) =>
            {
                if (DateTime.Now >= AutomaticMessages.LastMessage)
                {
                    //envia a mensagem
                    await Engine.Respond(schM.Message);

                    AutomaticMessages.LastMessage = DateTime.Now;

                    AutomaticMessages.RemoveQueue(schM.TypeInfo);
                    AutomaticMessages.AddQueue(Discord(DateTime.Now.AddMinutes(schM.WaitingTime.TotalMinutes)));
                }

            }, 
                new TimeSpan(0, 45, 0), //
                "Você também pode participar do nosso discord... // "+
                "                Join us on Discord... " + StreamerDefault.Discord, 
                MessageType.Discord);

        public static ScheduledMessage Youtube(DateTime date) => 
            new ScheduledMessage(date,async (ScheduledMessage schM) =>
            {
                if (DateTime.Now >= AutomaticMessages.LastMessage)
                {
                    //envia a mensagem
                    await Engine.Respond(schM.Message);

                    AutomaticMessages.LastMessage = DateTime.Now;

                    AutomaticMessages.RemoveQueue(schM.TypeInfo);
                    AutomaticMessages.AddQueue(Youtube(DateTime.Now.AddMinutes(schM.WaitingTime.TotalMinutes)));
                }

            }, 
                new TimeSpan(0, 45, 0),
                "Confira o nosso canal no Youtube... // " +
                "                We are also on YouTube... " + StreamerDefault.Youtube,
                MessageType.YouTube);

        public static ScheduledMessage GitHub(DateTime date) =>
            new ScheduledMessage(date, async (ScheduledMessage schM) =>
            {
                if (DateTime.Now >= AutomaticMessages.LastMessage)
                {
                    //envia a mensagem
                    await Engine.Respond(schM.Message);

                    AutomaticMessages.LastMessage = DateTime.Now;

                    AutomaticMessages.RemoveQueue(schM.TypeInfo);
                    AutomaticMessages.AddQueue(GitHub(DateTime.Now.AddMinutes(schM.WaitingTime.TotalMinutes)));
                }

            },
                new TimeSpan(0, 45, 0),
                "O nosso bot, todo em C#, está disponível no GitHub... // " +
                "                Check our chat bot on GitHub... " + StreamerDefault.GitHub,
                MessageType.GitHub);

        public static ScheduledMessage Donate(DateTime date) =>
            new ScheduledMessage(date, async (ScheduledMessage schM) =>
            {
                if (DateTime.Now >= AutomaticMessages.LastMessage)
                {
                    //envia a mensagem
                    await Engine.Respond(schM.Message);

                    AutomaticMessages.LastMessage = DateTime.Now;

                    AutomaticMessages.RemoveQueue(schM.TypeInfo);
                    AutomaticMessages.AddQueue(Donate(DateTime.Now.AddMinutes(schM.WaitingTime.TotalMinutes)));
                }

            },
                new TimeSpan(0, 45, 0),
                "Sinta-se livre para nos apoiar financeiramente... // " +
                "                Feel free helping us... " + StreamerDefault.PayPal + " " + StreamerDefault.PicPay,
                MessageType.Donate);

        public static ScheduledMessage Form(DateTime date) =>
            new ScheduledMessage(date, async (ScheduledMessage schM) =>
            {
                if (DateTime.Now >= AutomaticMessages.LastMessage)
                {
                    //envia a mensagem
                    await Engine.Respond(schM.Message);

                    AutomaticMessages.LastMessage = DateTime.Now;

                    AutomaticMessages.RemoveQueue(schM.TypeInfo);
                    AutomaticMessages.AddQueue(Form(DateTime.Now.AddMinutes(schM.WaitingTime.TotalMinutes)));
                }

            },
                new TimeSpan(0, 45, 0),
                "Temos uma pesquisa para conhecer mais o nosso chat... // " +
                "                Let me know more about you... " + StreamerDefault.Form,
                MessageType.Form);

        public static ScheduledMessage DrinkWater(DateTime date) =>
            new ScheduledMessage(date, async (ScheduledMessage schM) =>
            {
                if (DateTime.Now >= AutomaticMessages.LastMessage)
                {
                    //envia a mensagem
                    await Engine.Respond(schM.Message);

                    AutomaticMessages.LastMessage = DateTime.Now;

                    AutomaticMessages.RemoveQueue(schM.TypeInfo);
                    AutomaticMessages.AddQueue(DrinkWater(DateTime.Now.AddMinutes(schM.WaitingTime.TotalMinutes)));
                }

            },
                new TimeSpan(0, 7, 30),
                "Não se esqueça de beber Vodka/Água... // " +
                "                Remember to drink Vodka/Water... ",
                MessageType.Water);

        public enum MessageType
        {
            Discord,
            YouTube,
            Donate,
            GitHub,
            Form,
            Water,
        }
    }
}
