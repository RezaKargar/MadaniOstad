using AutoMapper;
using System;
using System.Linq;

namespace KodoomOstad.IocConfig.CustomMapping
{
    public class ConventionalMappingProfile : Profile
    {
        public ConventionalMappingProfile()
        {
            var projectName = "KodoomOstad";

            var mapFromType = typeof(IMapFrom<>);
            var mapToType = typeof(IMapTo<>);
            var explicitMapType = typeof(IMapExplicitly);

            var modelTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name.StartsWith(projectName))
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Type = t,
                    MapFrom = GetModelType(t, mapFromType),
                    MapTo = GetModelType(t, mapToType),
                    ExplicitMap =
                        t.GetInterfaces()
                            .Where(i => i == explicitMapType)
                            .Select(i => (IMapExplicitly)Activator.CreateInstance(t))
                            .FirstOrDefault()
                });


            foreach (var modelRegistration in modelTypes)
            {
                if (modelRegistration.MapFrom != null)
                {
                    CreateMap(modelRegistration.MapFrom, modelRegistration.Type);
                }

                if (modelRegistration.MapTo != null)
                {
                    CreateMap(modelRegistration.Type, modelRegistration.MapTo);
                }

                modelRegistration.ExplicitMap?.RegisterMappings(this);
            }

        }

        private Type GetModelType(Type type, Type mapType) =>
            type.GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == mapType)
                ?.GetGenericArguments()
                .First();

    }
}
