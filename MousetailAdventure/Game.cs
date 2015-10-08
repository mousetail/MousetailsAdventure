using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MousetailsAdventureLib;

namespace MousetailAdventure
{
    class Game
    {
        bool loadingfile;
        public Game(bool loadingfile)
        {
            this.loadingfile = loadingfile;
        }

        World w;
        public void Init()
        {
            
            w = new World(loadingfile);
            Room bank = new Room(w, "the bank",
@"Long room. Allong the sides are many cosics with
tellers sleeping behind their desks. At at the north
end is a row of tall, narrow, plain wooden doors
leading outside, at the south end a big, itricately
carved wooden door") {w.player};
            Room sidewalk = new Room(w, "The sidewalk",
                @"Long line of rought stone tiles allong a street")
                {
                    new Item("golden candlestick","elegant white-golden candlestick",new string[] {"golden","white-golden","elagent"},new string[] {"candlestick","stick"})
                };
            Room safe = new Room(w, "The safe",
                @"inside of a big metal cube. This safe is no longer in use, but some left over coins still lie on the floor.") {
                new Item("teddy bear","Small furry creature, staring at you with its glassy eyes...",new string[] {"teddy","furry"},
                    new string[] {"creature","bear"},new CantPickUp("The bear appears to be glued to the floor.")),
                new Item("plastic fish","Hideous yellow blob, bought cheaply from some low-level toy shop",new string[] {"plastic","yellow"},new string[] {"fish","blob"},
                    new MessageBeforeAndAfter())
            };

            safe.Connect(bank,direction.North);
            sidewalk.Connect(bank, direction.South);

            
            

        }
        public void Run()
        {
            w.Run();
        }
    }
}
