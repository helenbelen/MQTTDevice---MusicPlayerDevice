using System;

namespace MusicPlayerDevice
{
    public class Program
    {
       
        
        static void Main(string[] args)
        {
            MusicPlayer player = new MusicPlayer();
            MusicPlayer.TasksUsingThreadPool();
            string str = "";
            do
            {

               // Console.WriteLine("Now Playing " + MusicPlayer.PrintMessage() + System.Environment.NewLine);
               
            } while (str != "n");
        }
    }
}
