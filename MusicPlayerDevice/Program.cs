using System;

namespace MusicPlayerDevice
{
    public class Program
    {
        static MusicPlayer player;
        
        static void Main(string[] args)
        {
            player = new MusicPlayer();
        
            string str = "";
            do
            {

               // Console.WriteLine("Now Playing " + MusicPlayer.PrintMessage() + System.Environment.NewLine);
               
            } while (str != "n");
        }
    }
}
