using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MousetailsAdventureLib
{
    public class Command
    {
        public Matcher matcher;
        public String name;
        public Command(Matcher m, String name)
        {
            this.matcher = m;
            this.name = name;
        }
        public bool match(string s, World g)
        {
            return matcher.match(s, g);
        }
        public Dictionary<string, object> getArgs()
        {
            Dictionary<string, object> ar = matcher.getArgs();
            if (ar != null)
            {
                return ar;
            }
            else
            {
                return new Dictionary<string, object>();
            }
        }
    }

    public class SeqMatcher : Matcher
    {

        Matcher[] prob;
        /*
        public SeqMatcher(Matcher[] prob)
        {
            this.prob = prob;
        }
         */
        public SeqMatcher(params Matcher[] prob)
        {
            this.prob = prob;
        }

        public override bool match(string s, World g)
        {
            string[] words = s.Split(Util.splits);

            bool suc = true;
            if (words.Length < prob.Length)
            {
                return false;
            }
            int istart = 0;
            int iend = words.Length;
            int j = 0;
            string[] xwords;
            while (istart < words.Length)
            {
                suc = false;
                while (iend > istart)
                {
                    xwords = Util.slice(words, istart, iend);
                    if (prob[j].match(string.Join(" ", xwords),g))
                    {
                        suc = true;
                        istart = iend;
                        iend = words.Length;
                        j += 1;
                        if (j>=prob.Length && istart<words.Length){
                            //Console.WriteLine("I understood you untill " + words[istart]);
                            return false;
                        }
                        break;
                    }
                    else
                    {
                        iend -= 1;
                    }
                }

                if (!suc)
                {
                    //Debugger.Log(1, "parser", "failure due to iend>words.length");
                    return false;
                }

            }
            if (j == prob.Length)
            {
                return true;
            }
            else
            {
                //Debugger.Log(1, "parser", "failure due to prob <= j");
                return false;
            }
        }
        public override Dictionary<string, object> getArgs()
        {
            Dictionary<string, object> a = new Dictionary<string, object>();
            foreach (Matcher m in prob)
            {
                Dictionary<string, object> dict = m.getArgs();
                if (dict != null)
                {
                    foreach (var KeyValuePair in dict)
                    {
                        a.Add(KeyValuePair.Key, KeyValuePair.Value);
                    }
                }
            }
            return a;
        }
    }
    abstract public class Matcher
    {
        abstract public bool match(string s, World g);
        public virtual Dictionary<string,object> getArgs()
        {
            return null;
        }
    }
    class StringMatcher : Matcher
    {
        public string Text;
        public StringMatcher(string what)
        {
            Text = what;
        }
        public override bool match(string s, World g)
        {
            bool result = s.Equals(Text);
            if (result)
            {
                //Debugger.Log(1, "parser", ", sucsess");
            }
            else
            {
                //Debugger.Log(1, "parser", ", failure");
            }
            return result;

        }
    }
    class StringAsVar : Matcher
    {
        string varname;
        string value;
        public StringAsVar(string varname)
        {
            this.varname = varname;
        }

        public override bool match(string s, World g)
        {
            value = s;
            return true;
        }
        public override Dictionary<string, object> getArgs()
        {
            return new Dictionary<string,object>{{varname,value}};
        }
    }
    class DirectionMatcher: Matcher
    {
        string name;
        direction d;

        public DirectionMatcher(string n)
        {
            name = n;
        }

        public override bool match(string s, World g)
        {
            switch (s)
            {
                case "n":
                case "north":
                    d = direction.North;
                    return true;
                case "e":
                case "east":
                    d = direction.East;
                    return true;
                case "s":
                case "south":
                    d = direction.South;
                    return true;
                case "w":
                case "west":
                    d = direction.West;
                    return true;
                case "in":
                    d = direction.In;
                    return true;
                case "out":
                    d = direction.Out;
                    return true;
                case "u":
                case "up":
                    d = direction.Up;
                    return true;
                case "d":
                case "down":
                    d = direction.Down;
                    return true;
                case "northeast":
                case "ne":
                    d = direction.Northeast;
                    return true;
                case "northwest":
                case "nw":
                    d = direction.Northwest;
                    return true;
                case "sw":
                case "southwest":
                    d = direction.Southwest;
                    return true;
                case "se":
                case "southeast":
                    d = direction.Southeast;
                    return true;
                default:
                    return false;
            }
        }
        public override Dictionary<string, object> getArgs()
        {
            return new Dictionary<string, object>() { { name, d } };
        }
    }
    public class orMatcher: Matcher{
        Matcher[] m;
        Matcher selected;
        public orMatcher(params Matcher[] ms){
            m=ms;
        }
        public override bool match(string s, World g)
        {
            selected = null;
            foreach (Matcher f in m)
            {
                if (f.match(s,g)){
                    selected=f;
                }
            }
            if (selected==null){
                return false;
            }
            else{
                return true;
            }
        }
        public override Dictionary<string, object> getArgs()
        {
            return selected.getArgs();
        }
    }
    public class ItemMatcher: Matcher
    {

        bool inv;
        bool room;

        public ItemMatcher(string name, bool room, bool inv) {
            this.name = name;
            this.inv = inv;
            this.room = room;
        }
        string name;
        Anything itm;
        public override bool match(string s, World g)
        {
            List<Anything> f = g.FilterItems(s,room,inv);
            if (f.Count == 1)
            {
                itm = f[0];
                return true;
            }
            else
            {
                return false;
            }
        }
        public override Dictionary<string, object> getArgs()
        {
            return new Dictionary<string, object>() { { name, itm } };
        }
    }
}