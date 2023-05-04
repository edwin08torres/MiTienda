using System;
using System.Collections.Generic;

namespace SistemaVentas.Entity;

public partial class Identificacion
{
    public int IdTipoIdentificacion { get; set; }

    public string NombreIdentificacion { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Cliente> Clientes { get; } = new List<Cliente>();
}
