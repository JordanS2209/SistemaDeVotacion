using SistemaVotacion.Modelos;
using Microsoft.EntityFrameworkCore;

public class SistemaVotacionAPIContext : DbContext
{
    public SistemaVotacionAPIContext(DbContextOptions<SistemaVotacionAPIContext> options)
        : base(options)
    {
    }
    public DbSet<SistemaVotacion.Modelos.Candidato> Candidatos { get; set; } = default!;
    public DbSet<SistemaVotacion.Modelos.ActaAuditoria> ActasAuditorias { get; set; } = default!;
    public DbSet<SistemaVotacion.Modelos.Ciudad> Ciudades {get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.Dignidad> Dignidades{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.Genero> Generos {get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.HistorialLogin> HistoralesLogins {get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.JuntaReceptora> JuntasReceptoras {get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.Lista> Listas {get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.Multimedia> Multimedias{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.OpcionConsulta> OpcionConsultas{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.Padron> Padrones{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.Parroquia> Parroquias{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.PreguntaConsulta> PreguntasConsultas{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.ProcesoElectoral> ProcesosElectorales{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.Provincia> Provincias{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.RecintoElectoral> RecintosElectorales{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.RepresentanteJunta>  RepresentantesJuntas{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.ResultadoDetalleAuditoria> ResultadosDetallesAuditorias{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.Rol> Roles{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.TipoIdentificacion> TiposIdentificaciones{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.TipoProceso> TipoProcesos{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.TipoVoto> TipoVotos{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.Usuario> Usuarios{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.Votante> Votantes{get;set;} = default!;
    public DbSet<SistemaVotacion.Modelos.VotoDetalle> VotoDetalles{get;set;} = default!;
    

}
   

