using AutoMapper;
using OliveMorocco.Business.DTOs.Common;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Mapping;

public class VenteProfile : Profile
{
    public VenteProfile()
    {
        CreateMap<CreateDevisClientLigneDto, DevisClientLigne>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.DevisClientId, o => o.Ignore())
            .ForMember(d => d.DevisClient, o => o.Ignore())
            .ForMember(d => d.Produit, o => o.Ignore())
            .ForMember(d => d.Conditionnement, o => o.MapFrom(s => s.Conditionnement ?? string.Empty));

        CreateMap<CreateDevisClientDto, DevisClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.BonsCommandeClient, o => o.Ignore())
            .ForMember(d => d.BonsLivraisonClient, o => o.Ignore())
            .ForMember(d => d.FacturesClient, o => o.Ignore())
            .ForMember(d => d.Note, o => o.MapFrom(s => s.Note ?? string.Empty));

        CreateMap<UpdateDevisClientDto, DevisClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Numero, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.BonsCommandeClient, o => o.Ignore())
            .ForMember(d => d.BonsLivraisonClient, o => o.Ignore())
            .ForMember(d => d.FacturesClient, o => o.Ignore())
            .ForMember(d => d.Note, o => o.MapFrom(s => s.Note ?? string.Empty));

        CreateMap<CreateBonCommandeClientLigneDto, BonCommandeClientLigne>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.BonCommandeClientId, o => o.Ignore())
            .ForMember(d => d.BonCommandeClient, o => o.Ignore())
            .ForMember(d => d.Produit, o => o.Ignore())
            .ForMember(d => d.Conditionnement, o => o.MapFrom(s => s.Conditionnement ?? string.Empty));

        CreateMap<CreateBonCommandeClientDto, BonCommandeClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.Devis, o => o.Ignore())
            .ForMember(d => d.Facture, o => o.Ignore())
            .ForMember(d => d.FactureId, o => o.Ignore())
            .ForMember(d => d.BonsLivraisonClient, o => o.Ignore())
            .ForMember(d => d.Note, o => o.MapFrom(s => s.Note ?? string.Empty));

        CreateMap<UpdateBonCommandeClientDto, BonCommandeClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Numero, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.Devis, o => o.Ignore())
            .ForMember(d => d.Facture, o => o.Ignore())
            .ForMember(d => d.FactureId, o => o.Ignore())
            .ForMember(d => d.BonsLivraisonClient, o => o.Ignore())
            .ForMember(d => d.Note, o => o.MapFrom(s => s.Note ?? string.Empty));

        CreateMap<CreateBonLivraisonClientLigneDto, BonLivraisonClientLigne>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.BLId, o => o.Ignore())
            .ForMember(d => d.BonLivraisonClient, o => o.Ignore())
            .ForMember(d => d.Produit, o => o.Ignore());

        CreateMap<CreateBonLivraisonClientDto, BonLivraisonClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.Devis, o => o.Ignore())
            .ForMember(d => d.BonCommandeClient, o => o.Ignore())
            .ForMember(d => d.Facture, o => o.Ignore())
            .ForMember(d => d.FactureId, o => o.Ignore())
            .ForMember(d => d.Note, o => o.MapFrom(s => s.Note ?? string.Empty));

        CreateMap<UpdateBonLivraisonClientDto, BonLivraisonClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Numero, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.Devis, o => o.Ignore())
            .ForMember(d => d.BonCommandeClient, o => o.Ignore())
            .ForMember(d => d.Facture, o => o.Ignore())
            .ForMember(d => d.FactureId, o => o.Ignore())
            .ForMember(d => d.Note, o => o.MapFrom(s => s.Note ?? string.Empty));

        CreateMap<CreateFactureClientLigneDto, FactureClientLigne>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.FactureClientId, o => o.Ignore())
            .ForMember(d => d.FactureClient, o => o.Ignore())
            .ForMember(d => d.BonLivraison, o => o.Ignore())
            .ForMember(d => d.Produit, o => o.Ignore())
            .ForMember(d => d.Conditionnement, o => o.MapFrom(s => s.Conditionnement ?? string.Empty));

        CreateMap<CreateFacturePaiementDto, PaiementClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.FactureClientId, o => o.Ignore())
            .ForMember(d => d.FactureClient, o => o.Ignore());

        CreateMap<CreateFactureClientDto, FactureClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.Devis, o => o.Ignore())
            .ForMember(d => d.BonsCommandeClient, o => o.Ignore())
            .ForMember(d => d.BonsLivraisonClient, o => o.Ignore())
            .ForMember(d => d.AvoirsClient, o => o.Ignore())
            .ForMember(d => d.Note, o => o.MapFrom(s => s.Note ?? string.Empty))
            .ForMember(d => d.BonCommandeReference, o => o.MapFrom(s => s.BonCommandeReference ?? string.Empty));

        CreateMap<UpdateFactureClientDto, FactureClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Numero, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.Devis, o => o.Ignore())
            .ForMember(d => d.EstPayee, o => o.Ignore())
            .ForMember(d => d.BonsCommandeClient, o => o.Ignore())
            .ForMember(d => d.BonsLivraisonClient, o => o.Ignore())
            .ForMember(d => d.AvoirsClient, o => o.Ignore())
            .ForMember(d => d.Note, o => o.MapFrom(s => s.Note ?? string.Empty))
            .ForMember(d => d.BonCommandeReference, o => o.MapFrom(s => s.BonCommandeReference ?? string.Empty));

        CreateMap<CreateAvoirClientLigneDto, AvoirClientLigne>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.AvoirClientId, o => o.Ignore())
            .ForMember(d => d.AvoirClient, o => o.Ignore())
            .ForMember(d => d.Produit, o => o.Ignore())
            .ForMember(d => d.Conditionnement, o => o.MapFrom(s => s.Conditionnement ?? string.Empty));

        CreateMap<CreateAvoirClientDto, AvoirClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.Facture, o => o.Ignore())
            .ForMember(d => d.Motif, o => o.MapFrom(s => s.Motif ?? string.Empty));

        CreateMap<UpdateAvoirClientDto, AvoirClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Numero, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.Facture, o => o.Ignore())
            .ForMember(d => d.Motif, o => o.MapFrom(s => s.Motif ?? string.Empty));

        CreateMap<Tiers, ClientDto>();
        CreateMap<ClientDto, UpdateClientDto>();

        CreateMap<CreateClientDto, Tiers>()
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

        CreateMap<UpdateClientDto, Tiers>()
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

    }
}
