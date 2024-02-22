using System.Collections;
using System.Linq;

namespace GUI.Utils;

public static class Extensions
{
    public static IList ToIList(this IEnumerable enumerable)
    {
        if (enumerable is IList ilist)
            return ilist;
            
        return enumerable.OfType<object>().ToList();
    }
}