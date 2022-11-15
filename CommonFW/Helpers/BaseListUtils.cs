using System.Collections.Generic;
using System.Linq;

namespace Demo.CommonFramework.Helpers
{
    public static class BaseListUtils
    {
        public enum EOrderType
        {
            Ascendant,
            Descendant
        }

        /// <summary>
        /// Determines whether [is list ordered] [the specified ordered list].
        /// </summary>
        /// <param name="orderedList">The ordered list.</param>
        /// <returns>
        ///   <c>true</c> if [is list ordered] [the specified ordered list]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsListOrdered(List<string> orderedList)
        {
            var orderedByDsc = orderedList.OrderBy(d => d);
            return orderedList.SequenceEqual(orderedByDsc);
        }

        /// <summary>
        /// Determines whether [is list ordered by descending] [the specified ordered list].
        /// </summary>
        /// <param name="orderedList">The ordered list.</param>
        /// <returns>
        ///   <c>true</c> if [is list ordered by descending] [the specified ordered list]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsListOrderedByDescending(List<string> orderedList)
        {
            var orderedByDsc = orderedList.OrderByDescending(d => d);
            return orderedList.SequenceEqual(orderedByDsc);
        }
    }
}
