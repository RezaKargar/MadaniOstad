using AutoMapper;

namespace MadaniOstad.IocConfig.CustomMapping
{
    public interface IMapExplicitly
    {
        void RegisterMappings(IProfileExpression profile);
    }
}
