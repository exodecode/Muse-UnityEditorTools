using System.Collections;
using UnityEngine;

namespace Muse
{
    public static class Extensions
    {
        public static string TrimAfterLastChar(this string s, char c, bool includeChar = false) =>
            s.Substring(0, s.LastIndexOf(c) + (includeChar ? 0 : 1));

        public static string TrimBeforeLastChar(this string s, char c, bool includeChar = false) =>
            s.Substring(s.LastIndexOf(c) + (includeChar ? 1 : 0));

        public static string Flatten(this string[] array, string separator) =>
            string.Join(separator, array);

        public static T[] GetSurroundingElements<T>(this T[] a, int index, int width)
        {
            var length = a.Length;
            Debug.Assert((length % width == 0), "Array length is not divisible by width so could not calculate surronding elements.");

            //corners
            var isTopLeft = index == 0;
            var isTopRight = index == width - 1;
            var isBottomLeft = index == length - width;
            var isBottomRight = index == length - 1;

            var isLeftEdge = index % width == 0;
            var isRightEdge = (index + 1) % width == 0;
            var isTopEdge = index < width;
            var isBottomEdge = index > (length - width - 1);

            if (isTopLeft)
                return new T[] { a[index + 1], a[width], a[width + 1] };
            else if (isTopRight)
                return new T[] { a[width - 2], a[(width * 2) - 2], a[(width * 2) - 1] };
            else if (isBottomLeft)
                return new T[] { a[length - (width * 2)], a[(length - (width * 2)) + 1], a[(length - width) + 1] };
            else if (isBottomRight)
                return new T[] { a[length - width - 2], a[length - width - 1], a[length - 2] };
            else if (isTopEdge)
                return new T[] { a[index - 1], a[index + 1], a[(index + width) - 1], a[index + width], a[index + width + 1] };
            else if (isBottomEdge)
                return new T[] { a[index - width - 1], a[index - width], a[(index - width) + 1], a[index - 1], a[index + 1] };
            else if (isLeftEdge)
                return new T[] { a[index - width], a[(index - width) + 1], a[index + 1], a[index + width], a[index + width + 1] };
            else if (isRightEdge)
                return new T[] { a[index - width - 1], a[index - width], a[index - 1], a[(index + width) - 1], a[index + width] };
            else
                return new T[] { a[index - width - 1], a[index - width], a[(index - width) + 1], a[index - 1], a[index + 1], a[(index + width) - 1], a[index + width], a[index + width + 1] };
        }
    }
}