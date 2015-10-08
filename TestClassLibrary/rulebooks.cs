using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MousetailsAdventureLib
{

    public enum Role: byte { Actor, PrimObj, SecObj, Room, Region, Other }

    

    public interface IRulebook
    {
        bool CheckEv(string ev, Dictionary<string, object> args, Role r, World g);
        bool BeforeEv(string ev, Dictionary<string, object> args, Role r, World g);
        bool InsteadEv(string ev, Dictionary<string, object> args, Role r, World g);
        bool DoEv(string ev, Dictionary<string, object> args, Role r, World g);
        bool AfterEv(string ev, Dictionary<string, object> args, Role r, World g);
    }
    /// <summary>
    /// THIS SUMMARY IS OUTDATED, SEE BASIC
    /// RULEBOOK FOR UP-TO-DATE EXAMPLES
    /// To make your own rulebook, inherit from a higher level
    /// rulebook, use
    /// if (ev=="I want to handle this"){
    ///     ...
    /// }
    /// else if ...
    /// else{
    ///     return False
    /// }
    /// </summary>
    /// 
    public class BlankRulebook: IRulebook
    {
        /// <summary>
        /// Called before a event is processed further. Should
        /// return false if event is valid, and true otherwise.
        /// The difference between Check and Instead
        /// is that instead calls before and after
        /// while check dousn't
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="args"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <returns>If it did something</returns>
        public virtual bool CheckEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            return false;
        }
        /// <summary>
        /// Called before a event is handles. Should return
        /// if it did something.
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="args"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual bool BeforeEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            return false;
        }
        /// <summary>
        /// Called after "before" and before "after".
        /// Return if you did something. If you return
        /// true, the normal behovior will not happen
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="args"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual bool InsteadEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            return false;
        }
        /// <summary>
        /// Normal behavior for a event. Can be replaced by
        /// "Instead" or "Check" events.
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="args"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual bool DoEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            return false;
        }
        /// <summary>
        /// Called after a event is handles sucsesfully.
        /// Returns if it dis something.
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="args"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual bool AfterEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            return false;
        }
    }

    

    public class BasicRulebook : IRulebook
    {

        public bool DoEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            switch (ev)
            {
                case "quit":
                    g.quit();
                    return true;
                case "say":
                    Console.WriteLine("you say " + args["name"].ToString());
                    return true;
                case "go":
                    direction i = (direction)args["direction"];
                    Dictionary<direction, Room> directions = ((Room)g.player.getParent()).directions;
                    if (directions.ContainsKey(i))
                    {
                        g.player.moveTo(directions[i]);
                        Console.Write("you go ");
                        Console.WriteLine(i);
                        g.interpretLine("look");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("you run into a wall");
                        return true;
                    }
                case "look_room":
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    IContainer v = g.player.getParent();
                    Util.centerText(v.getName(), Console.WindowWidth);
                    Console.ResetColor();
                    Console.WriteLine(v.getDescription());
                    if (v.Length > 1)
                    {
                        Console.Write("there is a ");
                    }
                    int index = 0;
                    foreach (Anything l in v)
                    {
                        if (l != g.player)
                        {
                            Console.Write(l);
                            index += 1;
                            if (index == v.Length - 2)
                            {
                                Console.Write(" and a ");
                            }
                            else if (index < v.Length - 2)
                            {
                                Console.Write(", a ");
                            }
                        }
                        else
                        {
                        }
                    }
                    if (v.Length > 1)
                    {
                        Console.WriteLine(" here");
                    }
                    return true;
                case "take":
                    Item t = (Item)args["item"];
                    t.moveTo(g.player);
                    Console.WriteLine("you take the " + t.ToString() + ".");
                    return true;
                case "drop":
                    Item n = (Item)args["item"];
                    n.moveTo(g.player.getParent());
                    Console.WriteLine("you drop the " + n.ToString() + ".");
                    return true;
                case "inventory":
                    if (g.player.Length != 0)
                    {
                        Console.WriteLine("You are carying: ");
                        foreach (Anything l in g.player)
                        {
                            Console.WriteLine("- a " + l.ToString());
                        }
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("You arn't carying anything.");
                        return true;
                    }
                case "examine":
                    Anything o = (Anything)args["item"];
                    Console.WriteLine(o);
                    Console.WriteLine(o.getDescription());
                    return true;
                default:
                    return false;
            }

        }

        public bool CheckEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            return false;
        }

        public bool BeforeEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            return false;
        }

        public bool InsteadEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            return false;
        }

        public bool AfterEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            return false;
        }
    }
    public class CantPickUp: BlankRulebook
    {
        string msg;
        public CantPickUp(string message)
        {
            msg = message;
        }
        public CantPickUp()
        {
            msg = "You can't pick up that object.";
        }
        public override bool CheckEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            if (ev.Equals("take") && r == Role.PrimObj){
                Console.WriteLine(msg);
                return true;
            }
            return false;
        }
    }
    public class MessageBeforeAndAfter: BlankRulebook
    {
        public override bool BeforeEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            if (ev.Equals("examine") && r == Role.PrimObj)
            {
                Console.WriteLine("before");
                return true;
            }
            return false;
        }
        public override bool AfterEv(string ev, Dictionary<string, object> args, Role r, World g)
        {
            if (ev.Equals("examine") && r == Role.PrimObj)
            {
                Console.WriteLine("after");
                return true;
            }
            return false;
        }

    }
}
