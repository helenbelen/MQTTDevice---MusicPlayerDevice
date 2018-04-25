using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace MusicPlayerDevice
{
    public class MusicPlayer
    {
        MqttClient client;
        Dictionary<char, string> SongList = new Dictionary<char, string>()
        {
            { 'A', "Out There" },
            {'B', "Trolley Song" },
            {'C', "ABC Song" },
        };

        static char selectedSong;
        public MusicPlayer()
        {
            client = new MqttClient(MQTTCommon.Resources.brokerUrl);
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
            client.Subscribe(new string[] { "/test/+/soilMoisture" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

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
                return "NO SONG";
            }
            return SongList[selectedSong];
        }


    }
}
