using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace ChatApi
{
    public partial class ApiConnection
    {
        private readonly object SocketLocker = new();
        static readonly Queue<MessageContents> MessageQueue = new();

        private async Task SendMessage(string channel, string message)
        {
            string toSend = $"{Hycybh.MSG} {{ \"channel\": \"{channel}\", \"message\": \"{message}\" }}";
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {channel}: {message}");
            await Client.SendAsync(toSend);
        }

        private async Task SendAd(string channel, string message)
        {
            string toSend = $"{Hycybh.LRP} {{ \"channel\": \"{channel}\", \"message\": \"{message}\" }}";
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {channel}: {message}");
            await Client.SendAsync(toSend);
        }

        public async Task SetStatus(string statusMessage, ChatStatus status, string sendingUser)
        {
            string toSend = $"{Hycybh.STA} {{ \"status\": \"{status.ToString()}\", \"statusmsg\": \"{statusMessage}\", \"character\": \"{sendingUser}\" }}";
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {status.ToString()}: {statusMessage}");
            await Client.SendAsync(toSend);
        }

        private async Task SendWhisper(string targetUser, string message)
        {
            string toSend = $"{Hycybh.PRI} {{ \"recipient\": \"{targetUser}\", \"message\": \"{message}\" }}";
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {targetUser}: {message}");
            await Client.SendAsync(toSend);
        }

        private void StartReplyThread()
        {
            Timer timer = new(){Interval = 1001};
            timer.Elapsed += new ElapsedEventHandler(ReplyTicker);
            timer.Start();
            Console.WriteLine("Starting Reply-ticket");
        }

        private async void ReplyTicker(object source, ElapsedEventArgs e)
        {
            MessageContents message;
            try
            {
                lock (SocketLocker)
                {
                    if (MessageQueue.Count == 0) return;
                    message = MessageQueue.Dequeue();
                }
            }
            catch
            {
                Console.WriteLine("Error dequeueing message when queue not empty.");
                return;
            }


            if (message.messageType == MessageType.Whisper)
            {
                await SendWhisper(message.sendinguser, message.message);
            }
            else if (message.messageType == MessageType.Advertisement)
            {
                await SendAd(message.channel, message.message);
            }
            else if (message.messageType == MessageType.Basic)
            {
                await SendMessage(message.channel, message.message);
            }
            else
            {
                Console.WriteLine("Bad reply: " + message.channel + " / " + message.sendinguser + " / " + message);
            }
        }
    }
}
