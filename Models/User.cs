using System;
using System.Collections.Generic;

namespace EBBooksAPI.Models;

public partial class User
{
    public string Email { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FirebaseUuid { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
