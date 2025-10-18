using System.ComponentModel.DataAnnotations;

namespace ProjetoInvestimentos.Models;

/// <summary>
/// Modelo para perfis de usuário
/// </summary>
public class UserProfile
{
    /// <summary>
    /// ID único do usuário
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Email do usuário
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// CPF do usuário
    /// </summary>
    [Required]
    [StringLength(11)]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Dados adicionais do usuário (JSON)
    /// </summary>
    public string? Dados { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime CriadoEm { get; set; }

    /// <summary>
    /// Data de alteração
    /// </summary>
    public DateTime AlteradoEm { get; set; }

    /// <summary>
    /// Nome do usuário (não mapeado - usado para compatibilidade)
    /// </summary>
    public string? Nome { get; set; }
}