using AutoMapper;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Business.Mapping;

public class AchatProfile : Profile
{
    public AchatProfile()
    {
        CreateMap<Tiers, FournisseurDto>();
        CreateMap<FournisseurDto, UpdateFournisseurDto>();

        CreateMap<CreateFournisseurDto, Tiers>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Type, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore())
            .ForMember(d => d.DevisClients, o => o.Ignore())
            .ForMember(d => d.BonsCommandeClient, o => o.Ignore())
            .ForMember(d => d.BonsLivraisonClient, o => o.Ignore())
            .ForMember(d => d.FacturesClient, o => o.Ignore())
            .ForMember(d => d.AvoirsClient, o => o.Ignore())
            .ForMember(d => d.BonsCommandeFournisseur, o => o.Ignore())
            .ForMember(d => d.BonsReception, o => o.Ignore())
            .ForMember(d => d.FacturesFournisseur, o => o.Ignore())
            .ForMember(d => d.AvoirsFournisseur, o => o.Ignore())
            .ForMember(d => d.Pressages, o => o.Ignore());

        CreateMap<UpdateFournisseurDto, Tiers>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Type, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore())
            .ForMember(d => d.DevisClients, o => o.Ignore())
            .ForMember(d => d.BonsCommandeClient, o => o.Ignore())
            .ForMember(d => d.BonsLivraisonClient, o => o.Ignore())
            .ForMember(d => d.FacturesClient, o => o.Ignore())
            .ForMember(d => d.AvoirsClient, o => o.Ignore())
            .ForMember(d => d.BonsCommandeFournisseur, o => o.Ignore())
            .ForMember(d => d.BonsReception, o => o.Ignore())
            .ForMember(d => d.FacturesFournisseur, o => o.Ignore())
            .ForMember(d => d.AvoirsFournisseur, o => o.Ignore())
            .ForMember(d => d.Pressages, o => o.Ignore());

        CreateMap<CreateBonCommandeFournisseurDto, BonCommandeFournisseur>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Fournisseur, o => o.Ignore())
            .ForMember(d => d.BonsReception, o => o.Ignore())
            .ForMember(d => d.Lignes, o => o.MapFrom(s => s.Lignes));

        CreateMap<CreateBonCommandeFournisseurLigneDto, BonCommandeFournisseurLigne>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.BonCommandeFournisseurId, o => o.Ignore())
            .ForMember(d => d.BonCommandeFournisseur, o => o.Ignore())
            .ForMember(d => d.Intrant, o => o.Ignore())
            .ForMember(d => d.Conditionnement, o => o.MapFrom(s => s.Conditionnement ?? string.Empty));

        CreateMap<UpdateBonCommandeFournisseurDto, BonCommandeFournisseur>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Numero, o => o.Ignore())
            .ForMember(d => d.Fournisseur, o => o.Ignore())
            .ForMember(d => d.BonsReception, o => o.Ignore())
            .ForMember(d => d.Lignes, o => o.MapFrom(s => s.Lignes));

        CreateMap<CreateBonReceptionDto, BonReception>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Fournisseur, o => o.Ignore())
            .ForMember(d => d.BonCommande, o => o.Ignore())
            .ForMember(d => d.FactureFournisseur, o => o.Ignore())
            .ForMember(d => d.FactureFournisseurLignes, o => o.Ignore())
            .ForMember(d => d.TotalTtc, o => o.Ignore())
            .ForMember(d => d.Lignes, o => o.MapFrom(s => s.Lignes));

        CreateMap<CreateBonReceptionLigneDto, BonReceptionLigne>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.BRId, o => o.Ignore())
            .ForMember(d => d.BonReception, o => o.Ignore())
            .ForMember(d => d.Intrant, o => o.Ignore());

        CreateMap<UpdateBonReceptionDto, BonReception>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Numero, o => o.Ignore())
            .ForMember(d => d.Fournisseur, o => o.Ignore())
            .ForMember(d => d.BonCommande, o => o.Ignore())
            .ForMember(d => d.FactureFournisseur, o => o.Ignore())
            .ForMember(d => d.FactureFournisseurLignes, o => o.Ignore())
            .ForMember(d => d.TotalTtc, o => o.Ignore())
            .ForMember(d => d.Lignes, o => o.MapFrom(s => s.Lignes));

        CreateMap<CreateFactureFournisseurDto, FactureFournisseur>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Fournisseur, o => o.Ignore())
            .ForMember(d => d.Paiements, o => o.Ignore())
            .ForMember(d => d.BonsReception, o => o.Ignore())
            .ForMember(d => d.Pressages, o => o.Ignore())
            .ForMember(d => d.Lignes, o => o.MapFrom(s => s.Lignes));

        CreateMap<CreateFactureFournisseurLigneDto, FactureFournisseurLigne>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.FactureFournisseurId, o => o.Ignore())
            .ForMember(d => d.FactureFournisseur, o => o.Ignore())
            .ForMember(d => d.BonReception, o => o.Ignore())
            .ForMember(d => d.Intrant, o => o.Ignore())
            .ForMember(d => d.Conditionnement, o => o.MapFrom(s => s.Conditionnement ?? string.Empty));

        CreateMap<UpdateFactureFournisseurDto, FactureFournisseur>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Numero, o => o.Ignore())
            .ForMember(d => d.Fournisseur, o => o.Ignore())
            .ForMember(d => d.Paiements, o => o.Ignore())
            .ForMember(d => d.BonsReception, o => o.Ignore())
            .ForMember(d => d.Pressages, o => o.Ignore())
            .ForMember(d => d.Lignes, o => o.MapFrom(s => s.Lignes));

        CreateMap<CreateAvoirFournisseurDto, AvoirFournisseur>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Fournisseur, o => o.Ignore())
            .ForMember(d => d.Lignes, o => o.MapFrom(s => s.Lignes));

        CreateMap<CreateAvoirFournisseurLigneDto, AvoirFournisseurLigne>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.AvoirFournisseurId, o => o.Ignore())
            .ForMember(d => d.AvoirFournisseur, o => o.Ignore())
            .ForMember(d => d.Intrant, o => o.Ignore())
            .ForMember(d => d.Conditionnement, o => o.MapFrom(s => s.Conditionnement ?? string.Empty));

        CreateMap<UpdateAvoirFournisseurDto, AvoirFournisseur>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Numero, o => o.Ignore())
            .ForMember(d => d.Fournisseur, o => o.Ignore())
            .ForMember(d => d.Lignes, o => o.MapFrom(s => s.Lignes));
    }
}
