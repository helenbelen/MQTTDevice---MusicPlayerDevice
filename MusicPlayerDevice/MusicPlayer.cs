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
        NOTSURE =0,
        NORAIN = 1,
        RECENTRAIN =2,
        FLOOD = 3,

    }

    public class MusicPlayer
    {
        
        static Dictionary<WeatherType, string> SongList = new Dictionary<WeatherType, string>()
        {
            { WeatherType.NORAIN, "It hasn't rained in a while" },
            {WeatherType.RECENTRAIN, "It has rained recently" },
            {WeatherType.FLOOD, "It has been raining frequently" },
        };
        static MqttClient client;
        static WeatherType selectedSong;
        static int myID;


        public MusicPlayer()
        {

            client = new MqttClient(MQTTCommon.Resources.brokerUrl);
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
            client.Subscribe(new string[] {MQTTCommon.Resources.brokerTopic_subscribe }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            APIAdapter.RegisterDevice("MusicPlayer").Wait();
            myID = int.Parse(APIAdapter.APIResponse.ToString());
        }
    

         public static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            double value;
            string message = System.Text.Encoding.Default.GetString(e.Message);
            try
            {
                 value = Double.Parse(message.Split("-")[1]);
            }
            catch(Exception ex)
            {
                value = -1;
            }
         
            if (value >= 0 && value < .3) {

                selectedSong = WeatherType.NORAIN;
            }
            else if (value >= .3 && value <= .5)
            {
                selectedSong = WeatherType.RECENTRAIN;
            }
            else if( value > .5)
            {
                selectedSong = WeatherType.FLOOD;
            }
            else {
                selectedSong = WeatherType.NOTSURE;
            }
            PrintMessage();

        }

        public static string PrintMessage()
        {
            if(selectedSong == WeatherType.NOTSURE)
            {
               // APIAdapter.AddMusicData(URLType.ADDDATA, myID, "No Song").Wait();
                return "NO SONG";
            }

            client.Publish(MQTTCommon.Resources.brokerTopic_publish, Encoding.UTF8.GetBytes((String.Concat(myID.ToString()," ",selectedSong.ToString()))));

            // APIAdapter.AddMusicData(URLType.ADDDATA, myID, selectedSong.ToString()).Wait();
            return SongList[selectedSong];
        }


       



    }
}
