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

namespace MusicPlayerDevice
{
    public class MusicPlayer
    {
        MqttClient client;
        Dictionary<char, string> SongList = new Dictionary<char, string>()
        {
            { 'A', "Sunshine Song" },
            {'B', "Rencent Rain Son" },
            {'C', "Flood Song" },
        };

        static char selectedSong;
        static private readonly string baseURL = MQTTCommon.Resources.webApiUrl;
        static HttpClient httpClient;
        static JArray result;
        static int myID;
        public MusicPlayer()
        {
            client = new MqttClient(MQTTCommon.Resources.brokerUrl);
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
            client.Subscribe(new string[] { "/test/+/soilMoisture" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            APIAdapter.RegisterDevice("MusicPlayer").Wait();
            myID = int.Parse(APIAdapter.APIResponse.ToString());
           
        }
    


    public static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            double value = Double.Parse(System.Text.Encoding.Default.GetString(e.Message));
         
            if (value >= 0 && value < .3) {

                selectedSong = 'A';
            }
            else if (value >= .3 && value <= .5)
            {
                selectedSong = 'B';
            }
            else if( value > .5)
            {
                selectedSong = 'C';
            }
            else {
                selectedSong = 'N';
            }

           
        }

        public string PrintMessage()
        {
            if(selectedSong == 'N' || selectedSong == 0)
            {
                APIAdapter.AddMusicData(URLType.ADDDATA, myID, "No Song").Wait();
                return "NO SONG";
            }
            APIAdapter.AddMusicData(URLType.ADDDATA, myID, SongList[selectedSong]).Wait();
            return SongList[selectedSong];
        }


       



    }
}
