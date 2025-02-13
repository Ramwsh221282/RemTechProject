using RemTechCommon.Utils.ResultPattern;

namespace RemTechCommon.Utils.Extensions;

public static class ArrayExtensions
{
    public static Result<T> GetByIndex<T>(this List<T> collection, int index)
    {
        if (index < 0 || index > collection.Count - 1)
            return new Error("Index is out of range");

        return collection[index];
    }
}
