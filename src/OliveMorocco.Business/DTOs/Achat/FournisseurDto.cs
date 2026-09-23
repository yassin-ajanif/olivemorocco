using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.DTOs.Achat;

public record FournisseurDto(
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

public record CreateFournisseurDto(
    string Nom,
    string Adresse,
    string Ville,
    string Telephone,
    string Email,
    string ICE,
    string ConditionsPaiement,
    bool Actif = true);

public record UpdateFournisseurDto(
    string Nom,
    string Adresse,
    string Ville,
    string Telephone,
    string Email,
    string ICE,
    string ConditionsPaiement,
    bool Actif);
