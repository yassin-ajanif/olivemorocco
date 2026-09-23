using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using OliveMorocco.Business.Mapping;

namespace OliveMorocco.Business.Tests.Helpers;

internal static class MapperFactory
{
    public static IMapper Create()
        => new MapperConfiguration(
            cfg => cfg.AddProfile<VenteProfile>(),
            NullLoggerFactory.Instance).CreateMapper();
}
