using System;

namespace MusicPlayerDevice
{
    public class Program
    {
        static MusicPlayer player = new MusicPlayer();
        static void Main(string[] args)
        {
            string str = "";
            do
            {

                Console.WriteLine("Now Playing " + player.PrintMessage() + System.Environment.NewLine);
                str = Console.ReadLine();
            } while (str != "n");
        }
    }
}
