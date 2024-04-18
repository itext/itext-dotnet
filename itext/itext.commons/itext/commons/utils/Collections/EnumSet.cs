using System;
using System.Collections.Generic;
using System.Linq;

namespace iText.Commons.Utils.Collections
{
    public class EnumSet<T> : SortedSet<T> where T : System.Enum
    {
        public static EnumSet<TE> Of<TE>(TE first, params TE[] elements)  where  TE : Enum
        {
            var set = new EnumSet<TE>();
            set.Add(first);
            set.AddAll(elements);
            return set;
        }

        public static EnumSet<TE> AllOf<TE>()  where  TE : Enum
        {
            var set = new EnumSet<TE>();
            foreach (var item in Enum.GetValues( typeof(TE)))
            {
                set.Add((TE) item);    
            }
            return set;
        }

        public static EnumSet<TE> ComplementOf<TE>(EnumSet<TE> other) where  TE : Enum
        {
            var set = new EnumSet<TE>();
            set.AddAll(AllOf<TE>().Except(other));
            return set;
        }
    }
}