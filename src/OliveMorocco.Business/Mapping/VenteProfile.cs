using AutoMapper;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Business.Mapping;

public class VenteProfile : Profile
{
    public VenteProfile()
    {
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
