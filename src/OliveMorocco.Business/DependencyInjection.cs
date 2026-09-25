using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OliveMorocco.Business.Mapping;
using OliveMorocco.Business.Services;
using OliveMorocco.Business.Services.Achat;
using OliveMorocco.Business.Services.Vente;
using OliveMorocco.DataAccess;

namespace OliveMorocco.Business;

public static class DependencyInjection
{
    /// <summary>
    /// Registers DataAccess, AutoMapper, FluentValidation, and business services.
    /// Web host should call only this — never <c>AddDataAccess</c> directly.
    /// </summary>
    public static IServiceCollection AddBusiness(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDataAccess(connectionString);

        services.AddAutoMapper(cfg =>
        {
            var licenseKey = Environment.GetEnvironmentVariable("AUTOMAPPER_LICENSE_KEY");
            if (!string.IsNullOrWhiteSpace(licenseKey))
                cfg.LicenseKey = licenseKey;
        }, typeof(VenteProfile), typeof(AchatProfile));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ITiersUsageService, TiersUsageService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IFournisseurService, FournisseurService>();
        services.AddScoped<IBonCommandeFournisseurService, BonCommandeFournisseurService>();
        services.AddScoped<IBonReceptionService, BonReceptionService>();
        services.AddScoped<IFactureFournisseurService, FactureFournisseurService>();
        services.AddScoped<IAvoirFournisseurService, AvoirFournisseurService>();
        services.AddScoped<IIntrantSuggestionService, IntrantSuggestionService>();
        services.AddScoped<IChargeService, ChargeService>();

        return services;
    }
}
