using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MousetailsAdventureLib;
using System.IO;

namespace MousetailAdventure
{
    class Program
    {
        static void Main(string[] args)
        {
            Game g;
            if (args.Length >= 1)
            {
                string finename = args[0];
                TextReader s = File.OpenText(finename);
                Console.SetIn(s);
                g = new Game(true);
            }
            else
            {
                g = new Game(false);
            }
            g.Init();
            g.Run();
        }
    }
}
