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

        public DbSet<SistemaVotacion.Modelos.ActaAuditoria> ActasAuditorias { get; set; } = default!;

        public DbSet<SistemaVotacion.Modelos.Ciudad> Ciudades { get; set; } = default!;

        public DbSet<SistemaVotacion.Modelos.Dignidad> Dignidades { get; set; } = default!;
    }
