using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ceitcon_Data.Utilities
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    static public class Memento
    {
        static int maxLenght = 10;
        static private List<object> undo = new List<object>();
        static private List<object> redo = new List<object>();
        static public bool Enable = false;

        static public void Push(object item)
        {
            if (Enable)
            {
                if (undo.Count >= maxLenght)
                    undo.RemoveAt(0);
                undo.Add(item);
            }
        }

        static public object Pop()
        {
            var item = undo.Last();
            undo.Remove(item);
            return item;
        }

        static public void Clear()
        {
            undo.Clear();
        }

        static public bool HasValue()
        {
            return undo.Count > 0;
        }

        static public void PushR(object item)
        {
            if (Enable)
            {
                if (undo.Count >= maxLenght)
                    undo.RemoveAt(0);
                redo.Add(item);
            }
        }

        static public object PopR()
        {
            var item = redo.Last();
            redo.Remove(item);
            return item;
        }

        static public void ClearR()
        {
            redo.Clear();
        }

        static public bool HasValueR()
        {
            return redo.Count > 0;
        }
    }
}
