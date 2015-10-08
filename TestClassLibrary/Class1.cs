using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace MousetailsAdventureLib
{
    public enum direction: byte
    {
        North, South, East, West, In, Out, Up, Down, Northwest, Southeast, Northeast, Southwest
    }

    public class World: Region
    {
        

        bool running=true;
        /*
        List<Room> rooms;
        */
        List<Command> commands;

        static Role[] rolesOrder = { Role.Region, Role.Actor, Role.Room, Role.PrimObj, Role.SecObj, Role.Other };

        StreamWriter saveto;

        public Player player {get {
            return _player;}}
        private Player _player;


        public World(bool LoadingFile):base()
        {
            commands = new List<Command>(){
                new Command(new StringMatcher("quit"), "quit"),
                new Command(new SeqMatcher(new Matcher[] {new StringMatcher("say"),
                                                          new StringAsVar("name")}),
                                                          "say"),
                new Command(new orMatcher(new SeqMatcher(new StringMatcher("go"),new DirectionMatcher("direction")),
                    new DirectionMatcher("direction")),"go"),
                new Command(new StringMatcher("look"),"look_room"),
                new Command(new SeqMatcher(new StringMatcher("take"),new ItemMatcher("item",true,false)),"take"),
                new Command(new SeqMatcher(new StringMatcher("drop"),new ItemMatcher("item",false,true)),"drop"),
                new Command(new orMatcher(new StringMatcher("inventory"),new StringMatcher("inv"),new StringMatcher("i")),"inventory"),
                new Command(new SeqMatcher(new orMatcher(new StringMatcher("x"),new StringMatcher("examine"),new StringMatcher("look at")),
                    new ItemMatcher("item",true,true)),"examine")
            };
            _player = new Player();

            

            Add(new BasicRulebook());

            if (!LoadingFile)
            {
                saveto = new StreamWriter( File.Open("save.sav", FileMode.Create));
            }

            Debug.WriteLine("fishy!");
        }
        public void Add(Room r){

        }
        public void quit(){
            if (saveto!=null)
            {
                saveto.Flush();
                saveto.Close();
            }
            running=false;
        }

        public List<Anything> FilterItems(string name, bool room, bool inv)
        {
            string[] words = name.Split(Util.splits);
            string noun = words[words.Length - 1];
            string[] adjectives = Util.slice<string>(words, 0, words.Length - 1);
            List<Anything> shortlist = new List<Anything>();

            IContainer [] temp;
            if (room && inv){
                temp = new IContainer[] { this.player.getParent(), this.player };
            }
            else if (room)
            {
                temp = new IContainer[] { this.player.getParent() };
            }
            else if (inv)
            {
                temp = new IContainer[] { this.player };
            }
            else
            {
                temp = new IContainer[0];
            }
            //Item j;
            foreach (Anything i in temp.SelectMany<IContainer, Anything>(x => x))
            {
                if (i is Item)
                {
                    if (((Item)i).matchstring(adjectives, noun))
                    {
                        shortlist.Add(i);
                    }

                }

            }
            return shortlist;
        }

        public void Run(){
            while (running){
                Console.Write(">");
                string s = Console.ReadLine();
                if (s == null)
                {
                    Console.SetIn(new StreamReader( Console.OpenStandardInput()));
                
                  }
                else
                {
                    interpretLine(s);
                    if (saveto != null && running)
                    {
                        saveto.WriteLine(s);
                    }
                }
            }
        }
        
        public void interpretLine(string s){
            Command finalcom=null;
            foreach (Command c in commands){
                if (c.match(s,this)){
                    finalcom=c;
                    break;
                }
            }
            if (finalcom != null){
                ExecCommand(finalcom);

            }
            else
            {
                Console.WriteLine("you don't know how to do that");
            }
        }

        public bool ExecCommand(Command finalcom)
        {
            Dictionary<string, object> args = finalcom.getArgs();

            List<IEnumerable<IRulebook>> rules=new List< IEnumerable<IRulebook>>() {
                this.getRulebook(), player.getRulebook(), player.getParent().getRulebook()
            };

            if (args.ContainsKey("item")){
                rules.Add(((Anything)args["item"]).getRulebook());
            }
            if (args.ContainsKey("item2"))
            {
                rules.Add(((Anything)args["item2"]).getRulebook());
            }

            IEnumerable<KeyValuePair<IRulebook, Role>> books = new PairKeyVal<IRulebook, Role>(rolesOrder,
                rules);
            foreach (KeyValuePair<IRulebook, Role> i in books)
            {
                if (i.Key.CheckEv(finalcom.name, args, i.Value, this))
                {
                    return false;
                }
            }
            foreach (KeyValuePair<IRulebook, Role> i in books)
            {
                if (i.Key.BeforeEv(finalcom.name, args, i.Value, this))
                {
                    break;
                }
            }

            bool suc = true;

            foreach (KeyValuePair<IRulebook, Role> i in books)
            {
                if (i.Key.InsteadEv(finalcom.name, args, i.Value, this))
                {
                    suc = false;
                    break;
                }
            }
            if (suc)
            {
                foreach (KeyValuePair<IRulebook, Role> i in books)
                {
                    if (i.Key.DoEv(finalcom.name, args, i.Value, this))
                    {
                        break;
                    }
                }


            }
            foreach (KeyValuePair<IRulebook, Role> i in books)
            {
                if (i.Key.AfterEv(finalcom.name, args, i.Value, this))
                {
                    break;
                }
            }
            return true;
        }
    }
    
    
}
