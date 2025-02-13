using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KServerTools.Common;

namespace server.Repositories;

/// <summary>
/// Define the user entity. It needs to inherit from IEntity to use the repository.
/// </summary>
public record UserEntity(string Username, string Email, byte[] Hash, byte[] Salt) : IEntity {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Define the user entity lookup. It needs to inherit from IEntityLookup to use the repository.
/// </summary>
public record UserEntityLookup(string Username) : IEntityLookup;

/// <summary>
/// The User Repository.
/// </summary>
public interface IUserRepository : IRepository<UserEntity, UserEntityLookup> {
}