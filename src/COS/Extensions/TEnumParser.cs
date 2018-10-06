using System;
using System.Linq;

namespace System
{
    public class TEnumParser<TEnum>
    {
        public TEnum ParseFromShortName(string shortName)
        {
            return FindByShortName(shortName);
        }

        public TEnum ParseFromDisplayName(string displayName)
        {
            return FindByDisplayName(displayName);
        }

        private TEnum FindByShortName(string shortName)
        {
            var allItems = Enum.GetValues(typeof(TEnum));
            var item = allItems.Cast<TEnum>().FirstOrDefault(x => (x as Enum).GetShortName() == shortName);
            return item;
        }

        private TEnum FindByDisplayName(string displayName)
        {
            var allItems = Enum.GetValues(typeof(TEnum));
            var item = allItems.Cast<TEnum>().FirstOrDefault(x => (x as Enum).GetDisplayName() == displayName);
            return item;
        }
    }
}