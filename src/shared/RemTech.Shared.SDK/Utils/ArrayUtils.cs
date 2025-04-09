namespace RemTech.Shared.SDK.Utils;

public static class ArrayUtils
{
    public static bool AreAllUnique<T>(this T[] array, Func<T, T, bool> matcFn, out T? duplicate)
    {
        if (array.Length == 0)
        {
            duplicate = default;
            return true;
        }

        T item = array[0];

        for (int i = 1; i < array.Length; i++)
        {
            T element = array[i];
            if (matcFn(item, element))
            {
                duplicate = element;
                return false;
            }

            item = element;
        }

        duplicate = default;
        return true;
    }

    public static bool IsEmpty<T>(this T[] array) => array.Length == 0;
}
