using AutoMapper;

namespace KodoomOstad.IocConfig.CustomMapping
{
    public interface IMapExplicitly
    {
        void RegisterMappings(IProfileExpression profile);
    }
}
