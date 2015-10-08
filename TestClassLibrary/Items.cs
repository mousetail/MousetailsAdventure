using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MousetailsAdventureLib
{
    public interface IContainer: Anything, IEnumerable<Anything>{
        void Add(Anything item);
        void Remove(Anything item);
        int Length { get; }
    }
    public interface Anything{
        /// <summary>
        /// Internal use method, don't use.
        /// Sets internal parent, only
        /// called by a container when
        /// you add it. I hope to implement
        /// a "Move" method for external
        /// use
        /// </summary>
        /// <param name="c">The container to add to</param>
        void setParent(IContainer c);
        IContainer getParent();
        string getDescription();
        string getName();
        IEnumerable<IRulebook> getRulebook();
        void Add(IRulebook r);
    }
    public class Region : IContainer
    {

        List<Anything> items;
        public IContainer parent;
        List<IRulebook> rules;
        public Region()
        {
            items = new List<Anything>();
            rules = new List<IRulebook>() {};
        }

        public void Add(Anything item)
        {
            items.Add(item);
            item.setParent(this);

        }
        public void Add(IRulebook item)
        {
            rules.Insert(0, item);
        }

        public void Remove(Anything item)
        {
            items.Remove(item);
        }
        public void setParent(IContainer item)
        {
            parent = item;
        }
        public IEnumerator<Anything> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }


        public IContainer getParent()
        {
            return parent;
        }


        public virtual string getDescription()
        {
            return "";
        }


        public virtual string getName()
        {
            return "";

        }
        public int Length { get { return items.Count; } }


        public IEnumerable<IRulebook> getRulebook()
        {
            return rules;
        }
    }

    public class Room: Region
    {
        string name;
        string description;
        string[] altnames;
        public Dictionary<direction, Room> directions;


        public Room(World w, string name):this(w,name,name)
        {
            
        }
        public Room(World w, string name, string description)
        {
            w.Add(this);
            this.name = name;
            this.description = description;
            this.altnames = new string[] { name };
            directions = new Dictionary<direction, Room>();
            setParent(w);
        }
        public void Connect(Room r, direction d)
        {
            Connect(r, d, (direction)((int)d ^ 1));
        }
        public void Connect(Room r, direction? d, direction? d2)
        {
            if (d != null && r!=null)
            {
                directions[(direction)d] = r;
            }
            else if (r==null && d!=null)
            {
                directions.Remove((direction)d);
            }
            if (d2 != null)
            {
                r.Connect(this, d2, null);
            }

        }
        public override string getDescription()
        {
            return description;
        }
        public override string getName()
        {
            return name;
        }
    }

    public class Item: Anything
    {
        string name;
        string description;
        string[] adjectives;
        string[] nouns;
        IContainer parent;
        List<IRulebook> rules;
        public Item(string name)
        {
            this.name = name;
            this.description = name;
            string[] words = name.Split(Util.splits);
            this.nouns = new string[] { words[words.Length - 1] };
            this.adjectives = Util.slice<string>(words, 0, words.Length - 1);
            this.rules = new List<IRulebook>();
        }
        public Item(string name, string description, string[] adjectives, string[] nouns, params IRulebook[] rules){
            this.name = name;
            this.description = description;
            this.nouns = nouns;
            this.adjectives = adjectives;
            this.rules = new List<IRulebook>();
            foreach (IRulebook i in rules){
                this.rules.Add(i);
            }
        }

        public override string ToString()
        {
            return name;
        }
        public void setParent(IContainer c)
        {
            parent = c;
        }
        public IContainer getParent()
        {
            return parent;
        }


        public string getDescription()
        {
            return description;
        }
        public void moveTo(IContainer c)
        {
            parent.Remove(this);
            c.Add(this);
            parent = c;
        }


        public string getName()
        {
            return name;
        }

        public bool matchstring(string[] adjectives, string noun)
        {

            nouns.Contains(noun);
            if (nouns.Contains(noun)){
                if (adjectives.All(this.adjectives.Contains))
                {
                    return true;
                }
            }
            return false;
        }


        public IEnumerable<IRulebook> getRulebook()
        {
            return rules;
        }


        public void Add(IRulebook r)
        {
            rules.Add(r);
        }
    }
    public class Player : Item, IContainer
    {
        List<Anything> inventory;

        public Player()
            : base("me")
        {
            inventory = new List<Anything>();
        }

        public void Add(Anything item)
        {
            inventory.Add(item);
        }

        public void Remove(Anything item)
        {
            inventory.Remove(item);
        }

        public IEnumerator<Anything> GetEnumerator()
        {
            
            return inventory.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return inventory.GetEnumerator();
        }
        public int Length { get { return inventory.Count; } }
    }
}