using AutoMapper;
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
