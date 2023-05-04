using System;
using System.Collections.Generic;

namespace SistemaVentas.Entity;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string? Nombre { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? TipoIdentificacion { get; set; }

    public string? Identificacion { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int IdTipoIdentificacion { get; set; }

    public virtual Identificacion IdTipoIdentificacionNavigation { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; } = new List<Venta>();
}
