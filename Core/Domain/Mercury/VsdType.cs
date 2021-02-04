using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Domain.Mercury
{
    public class VsdType
    {
        public static VsdType TRANSPORT = new VsdType(0, "Транспортный");

        public static VsdType PRODUCTIVE = new VsdType(1, "Производственный");

        public static VsdType RETURNABLE = new VsdType(2, "Возвратный");

        public static VsdType INCOMING = new VsdType(3, "Входящий");

        public static VsdType OUTGOING = new VsdType(4, "Исходящий");
        
        public int Id { get; private set; }

        public string Title { get; private set; }

        public VsdType(int id, string title)
        {
            Title = title;
            Id = id;
        }

        public static IEnumerable<VsdType> GetAll() =>
            typeof(VsdType).GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<VsdType>();
    }
}