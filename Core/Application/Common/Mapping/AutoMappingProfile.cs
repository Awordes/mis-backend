using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace Core.Application.Common.Mapping
{
    class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            ApplyMappingFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingFromAssembly(Assembly executingAssembly)
        {
            var types = executingAssembly.GetExportedTypes()
                .Where(type =>
                    type.GetInterfaces()
                    .Any(i => i.IsGenericType && (
                        i.GetGenericTypeDefinition() == typeof(IMapFrom<>)
                        || i.GetGenericTypeDefinition() == typeof(IMapTo<>)
                     ))).ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping")
                    ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping")
                    ?? type.GetInterface("IMapTo`1")?.GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}
