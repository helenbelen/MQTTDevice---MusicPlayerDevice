using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Vlc.DotNet.Core;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net;

namespace MusicPlayerDevice
{

    public enum WeatherType {
        NOTSURE = 0,
        NORAIN = 1,
        RECENTRAIN = 2,
        FLOOD = 3,

    }



    public class MusicPlayer
    {

        static Dictionary<WeatherType, string> SongList = new Dictionary<WeatherType, string>()
        {
            { WeatherType.NORAIN, "It hasn't rained in a while" },
            {WeatherType.RECENTRAIN, "It has rained recently" },
            {WeatherType.FLOOD, "It has been raining frequently" },
            {WeatherType.NOTSURE,"There Is No Song Playing" },
        };

        static WeatherType selectedSong;
        static int myID;
        static MqttClient myClient;
        private static object s_LogLock = new object();

        public MusicPlayer()
        {
            myID = MusicPlayer.RegisterDeviceAsync().Result;
            myClient = createClient();

        }

        public static async Task<int> RegisterDeviceAsync() {


            string id = await Task.Run<string>(() => { return APIAdapter.RegisterDevice("MusicPlayer"); });
            int newid = int.Parse(id);
            return newid;
        }


        private static MqttClient createClient()
        {
            MqttClient client = new MqttClient(MQTTCommon.Resources.brokerUrl);
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
            client.Subscribe(new string[] { MQTTCommon.Resources.brokerTopic_subscribe }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            return client;
        }

        
    

         public static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            
            double value;
            int id;
            string message = System.Text.Encoding.Default.GetString(e.Message);
            try
            {
                string[] data = message.Split("-");
                id = int.Parse(data[0]);
                value = Double.Parse(data[1]);
               
            }
            catch(Exception ex)
            {
                value = -1;
                id = myID;
            }
            if (id != myID)
            {
                if (value >= 0 && value < .3)
                {

                    selectedSong = WeatherType.NORAIN;
                }
                else if (value >= .3 && value <= .5)
                {
                    selectedSong = WeatherType.RECENTRAIN;
                }
                else if (value > .5)
                {
                    selectedSong = WeatherType.FLOOD;
                }
                else
                {
                    selectedSong = WeatherType.NOTSURE;
                }
                //ParallelInvoke();
                TasksUsingThreadPool();
            }

        }

       
        public static void PublishMessage(object o)
        {
          
            myClient.Publish(MQTTCommon.Resources.brokerTopic_publish, Encoding.UTF8.GetBytes((String.Concat(myID.ToString(), "-", selectedSong.ToString()))));
            Log(o?.ToString());

            Console.WriteLine("Done Publishing!");

        }
        public static void PrintMessage(object o)
        {
         

            // APIAdapter.AddMusicData(URLType.ADDDATA, myID, selectedSong.ToString()).Wait();
            Console.WriteLine("Now Playing " + SongList[selectedSong]);
            Log(o?.ToString());
            Console.WriteLine("Done Printing.");
        }

        public static void TasksUsingThreadPool()
        {
            var tf = new TaskFactory();
            Task t1 = tf.StartNew(PrintMessage, "Printing Message...");
           
           
            Task t2 = Task.Factory.StartNew(PublishMessage,"Publishing Message...");
          
           
        }

        public static void TaskMethod(object o)
        {
            Log(o?.ToString());
        }
       public static void Log(string title)
        {
            lock (s_LogLock)
            {
                Console.WriteLine(title);
                Console.WriteLine($"Task id: {Task.CurrentId?.ToString() ?? "no task"}," + $"thread: {Thread.CurrentThread.ManagedThreadId}");
#if (!DNXCORE)
                Console.WriteLine($"is pooled thred: { Thread.CurrentThread.IsThreadPoolThread}");
#endif
                Console.WriteLine($"is background thread: {Thread.CurrentThread.IsBackground}");
                Console.WriteLine();
            }
        }



    }
}
