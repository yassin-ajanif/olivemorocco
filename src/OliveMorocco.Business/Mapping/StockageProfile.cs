using AutoMapper;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Mapping;

public class StockageProfile : Profile
{
    public StockageProfile()
    {
        CreateMap<CreateVarieteDto, Variete>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Nom, o => o.MapFrom(s => s.Nom.Trim()))
            .ForMember(d => d.Code, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Code) ? null : s.Code.Trim()))
            .ForMember(d => d.RegionOrigine, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.RegionOrigine) ? null : s.RegionOrigine.Trim()))
            .ForMember(d => d.SecteurVarietes, o => o.Ignore())
            .ForMember(d => d.Produits, o => o.Ignore())
            .ForMember(d => d.Recoltes, o => o.Ignore())
            .ForMember(d => d.Pressages, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());

        CreateMap<CreateIntrantDto, Intrant>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Nom, o => o.MapFrom(s => s.Nom.Trim()))
            .ForMember(d => d.Unite, o => o.MapFrom(s => s.Unite.Trim()))
            .ForMember(d => d.InterventionLignes, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());

        CreateMap<UpdateIntrantDto, Intrant>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Nom, o => o.MapFrom(s => s.Nom.Trim()))
            .ForMember(d => d.Unite, o => o.MapFrom(s => s.Unite.Trim()))
            .ForMember(d => d.InterventionLignes, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());

        CreateMap<CreateSecteurVarieteLineDto, SecteurVariete>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.SecteurId, o => o.Ignore())
            .ForMember(d => d.Secteur, o => o.Ignore())
            .ForMember(d => d.Variete, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());

        CreateMap<CreateSecteurDto, Secteur>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Nom, o => o.MapFrom(s => s.Nom.Trim()))
            .ForMember(d => d.Code, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Code) ? null : s.Code.Trim()))
            .ForMember(d => d.SecteurVarietes, o => o.MapFrom(s => s.Lignes))
            .ForMember(d => d.Interventions, o => o.Ignore())
            .ForMember(d => d.Recoltes, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());

        CreateMap<UpdateSecteurDto, Secteur>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Nom, o => o.MapFrom(s => s.Nom.Trim()))
            .ForMember(d => d.Code, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Code) ? null : s.Code.Trim()))
            .ForMember(d => d.SecteurVarietes, o => o.MapFrom(s => s.Lignes))
            .ForMember(d => d.Interventions, o => o.Ignore())
            .ForMember(d => d.Recoltes, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());

        CreateMap<CreateProduitDto, Produit>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.StockActuel, o => o.Ignore())
            .ForMember(d => d.Variete, o => o.Ignore())
            .ForMember(d => d.ImageData, o => o.Ignore())
            .ForMember(d => d.MouvementsStock, o => o.Ignore())
            .ForMember(d => d.DevisClientLignes, o => o.Ignore())
            .ForMember(d => d.BonCommandeClientLignes, o => o.Ignore())
            .ForMember(d => d.BonLivraisonClientLignes, o => o.Ignore())
            .ForMember(d => d.FactureClientLignes, o => o.Ignore())
            .ForMember(d => d.AvoirClientLignes, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore())
            .ForMember(d => d.CodeBarre, o => o.MapFrom(s => NormalizeOptional(s.CodeBarre)));

        CreateMap<UpdateProduitDto, Produit>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.StockActuel, o => o.Ignore())
            .ForMember(d => d.Variete, o => o.Ignore())
            .ForMember(d => d.ImageData, o => o.Ignore())
            .ForMember(d => d.MouvementsStock, o => o.Ignore())
            .ForMember(d => d.DevisClientLignes, o => o.Ignore())
            .ForMember(d => d.BonCommandeClientLignes, o => o.Ignore())
            .ForMember(d => d.BonLivraisonClientLignes, o => o.Ignore())
            .ForMember(d => d.FactureClientLignes, o => o.Ignore())
            .ForMember(d => d.AvoirClientLignes, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore())
            .ForMember(d => d.CodeBarre, o => o.MapFrom(s => NormalizeOptional(s.CodeBarre)));
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
