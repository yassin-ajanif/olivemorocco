using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.DTOs.Vente;

public record ClientDto(
    int Id,
    TypeTiers Type,
    string Nom,
    string Adresse,
    string Ville,
    string Telephone,
    string Email,
    string ICE,
    string ConditionsPaiement,
    bool Actif);

public record CreateClientDto(
    string Nom,
    string Adresse,
    string Ville,
    string Telephone,
    string Email,
    string ICE,
    string ConditionsPaiement,
    bool Actif = true);

public record UpdateClientDto(
    string Nom,
    string Adresse,
    string Ville,
    string Telephone,
    string Email,
    string ICE,
    string ConditionsPaiement,
    bool Actif);
