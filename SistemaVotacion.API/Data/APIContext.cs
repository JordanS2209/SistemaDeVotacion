using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;

    public class APIContext : DbContext
    {
        public APIContext (DbContextOptions<APIContext> options)
            : base(options)
        {
        }

        public DbSet<SistemaVotacion.Modelos.Votante> Votantes { get; set; } = default!;

        public DbSet<SistemaVotacion.Modelos.Candidato> Candidatos { get; set; } = default!;

        public DbSet<SistemaVotacion.Modelos.Lista> Listas { get; set; } = default!;

        public DbSet<SistemaVotacion.Modelos.HistorialAcceso> HistorialAccesos { get; set; } = default!;

        public DbSet<SistemaVotacion.Modelos.Multimedia> Multimedias { get; set; } = default!;

        public DbSet<SistemaVotacion.Modelos.Rol> Roles { get; set; } = default!;

        public DbSet<SistemaVotacion.Modelos.Usuario> Usuarios { get; set; } = default!;

public DbSet<SistemaVotacion.Modelos.Provincia> Provincias { get; set; } = default!;

public DbSet<SistemaVotacion.Modelos.RecintoElectoral> RecintoElectorales { get; set; } = default!;

public DbSet<SistemaVotacion.Modelos.ResultadoDetalleAuditoria> ResultadoDetalleAuditorias { get; set; } = default!;

public DbSet<SistemaVotacion.Modelos.TipoIdentificacion> TipoIdentificaciones { get; set; } = default!;

public DbSet<SistemaVotacion.Modelos.TipoProceso> TipoProcesos { get; set; } = default!;

public DbSet<SistemaVotacion.Modelos.TipoVoto> TipoVotos { get; set; } = default!;

public DbSet<SistemaVotacion.Modelos.VotoDetalle> VotoDetalles { get; set; } = default!;
    }
