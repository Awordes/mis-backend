using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Domain.Mercury
{
    public class VsdStatus
    {
        public static VsdStatus CREATED = new VsdStatus(0, "Создан");

        public static VsdStatus CONFIRMED = new VsdStatus(1, "Оформлен");

        public static VsdStatus WITHDRAWN = new VsdStatus(2, "Аннулирован");

        public static VsdStatus UTILIZED = new VsdStatus(3, "Погашен");

        public static VsdStatus FINALIZED = new VsdStatus(4, "Закрыт");
        
        public int Id { get; private set; }

        public string Title { get; private set; }

        public VsdStatus(int id, string title)
        {
            Title = title;
            Id = id;
        }

        public static IEnumerable<VsdStatus> GetAll() =>
            typeof(VsdStatus).GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<VsdStatus>();
    }
}