using System.Reflection;

namespace SolarHomeAuto.Domain.Utils
{
    public static class SimpleMapper
    {
        private static readonly BindingFlags PropertiesToMap = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;

        public static TTo MapToSubClass<TFrom, TTo>(TFrom from) where TTo : TFrom, new()
        {
            var to = new TTo();

            MapToSubClass(from, to);

            return to;
        }

        public static void MapToSubClass<TFrom, TTo>(TFrom from, TTo to) where TTo : TFrom
        {
            foreach (var prop in typeof(TFrom).GetProperties(PropertiesToMap))
            {
                var val = prop.GetValue(from);
                prop.SetValue(to, val);
            }
        }

        public static TTo MapToSuperClass<TFrom, TTo>(TFrom from) 
            where TFrom : TTo
            where TTo: new()
        {
            var to = new TTo();

            MapToSuperClass(from, to);

            return to;
        }

        public static void MapToSuperClass<TFrom, TTo>(TFrom from, TTo to) where TFrom : TTo
        {
            foreach (var prop in typeof(TTo).GetProperties(PropertiesToMap))
            {
                var val = prop.GetValue(from);
                prop.SetValue(to, val);
            }
        }
    }
}
