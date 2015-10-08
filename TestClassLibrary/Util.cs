using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MousetailsAdventureLib
{
    static class Util
    {
        public static T[] slice<T>(T[] l, int from, int to)
        {
            T[] r = new T[to - from];
            for (int i = from; i < to; i++)
            {
                r[i-from]=l[i];
            }
            return r;
        }

        public static T identity<T>(T f)
        {
            return f;
        }
        public static IEnumerable<T> chain<T>(params IEnumerable<T>[] e)
        {
            return e.SelectMany<IEnumerable<T>, T>(identity<IEnumerable<T>>);
        }

        

        public static void centerText(string text, int length)
        {
            int pad = length / 2 + (text.Length / 2);
            
            Console.WriteLine(text.PadLeft(pad).PadRight(length));
        }

        public static char[] splits = { ' ' };
        public static void WriteArray(IEnumerable<Object> num){
            foreach (Object o in num){
                Console.Write(o.ToString());
                Console.Write(", ");
            }
            Console.WriteLine();

    }
    }
    public class AddMultiple<T>: IEnumerable<T>
    {
        dynamic c;

        public AddMultiple(dynamic c)
        {
            this.c = c;
        }
        public void Add(T item)
        {
            c.Add(item);
        }


        public IEnumerator<T> GetEnumerator()
        {
            return c.getEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return c.getEnumerator();
        }
    }
    /// <summary>
    /// Strange class for specific purposes. Cycles threw all the
    /// IEnumerables in q and flattens them. Combines every output value with one value
    /// from f. eg PairKeyValue({a,b},{c,d},{e,f}) gives you
    /// c,a
    /// d,a
    /// e,b
    /// f,b
    /// </summary>
    /// <typeparam name="T1">The type for the "Key", or the nested items in the list</typeparam>
    /// <typeparam name="T2">The type for the "Value", or the items in the second list</typeparam>
    class PairKeyVal<T1, T2> : IEnumerator<KeyValuePair<T1,T2>>, IEnumerable<KeyValuePair<T1,T2>>
    {
        IEnumerator<T2> tags;
        IEnumerator<IEnumerable<T1>> items;
        IEnumerator<T1> current;

        public PairKeyVal(IEnumerable<T2> f, params IEnumerable<T1>[] q)
        {
            tags = f.GetEnumerator();
            items = ((IEnumerable<IEnumerable<T1>>) q) .GetEnumerator();
            Reset();
        }
        public PairKeyVal(IEnumerable<T2> f, IEnumerable<IEnumerable<T1>> q)
        {
            tags = f.GetEnumerator();
            items = q.GetEnumerator();
            Reset();
        }

        public KeyValuePair<T1,T2> Current
        {
            get { return new KeyValuePair<T1,T2>(current.Current,tags.Current); }
        }

        public void Dispose()
        {
            
        }

        object System.Collections.IEnumerator.Current
        {
            get { return current.Current; }
        }

        public bool MoveNext()
        {
            while (!current.MoveNext())
            {
                if (!nextCat())
                {
                    return false;
                }
            }
            return true;
        }
        public bool nextCat()
        {
            if (items.MoveNext())
            {
                current = items.Current.GetEnumerator();
                tags.MoveNext();
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Reset()
        {
            tags.Reset();
            items.Reset();
            items.MoveNext();
            tags.MoveNext();
            current = items.Current.GetEnumerator();
        }
    
        public IEnumerator<KeyValuePair<T1,T2>> GetEnumerator()
        {
            Reset();
 	        return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
 	        return this;
        }
    }
}
