using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaVotacion.API.Migrations
{
    /// <inheritdoc />
    public partial class v11SotacionFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dignidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreDignidad = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dignidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Generos",
                columns: table => new
                {
                    IdGenero = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DetalleGenero = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generos", x => x.IdGenero);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreProvincia = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreRol = table.Column<string>(type: "text", nullable: false),
                    DescripcionRol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoProcesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreTipoProceso = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProcesos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposIdentificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DetalleTipIdentifiacion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposIdentificaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoVotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreTipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoVotos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ciudades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreCiudad = table.Column<string>(type: "text", nullable: false),
                    IdProvincia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciudades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ciudades_Provincias_IdProvincia",
                        column: x => x.IdProvincia,
                        principalTable: "Provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcesosElectorales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreProceso = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IdTipoProceso = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcesosElectorales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcesosElectorales_TipoProcesos_IdTipoProceso",
                        column: x => x.IdTipoProceso,
                        principalTable: "TipoProcesos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombres = table.Column<string>(type: "text", nullable: false),
                    Apellidos = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    ContrasenaHash = table.Column<string>(type: "text", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IntentosFallidos = table.Column<int>(type: "integer", nullable: false),
                    CuentaBloqueada = table.Column<bool>(type: "boolean", nullable: true),
                    IdRol = table.Column<int>(type: "integer", nullable: false),
                    IdTipoIdentificacion = table.Column<int>(type: "integer", nullable: false),
                    NumeroIdentificacion = table.Column<string>(type: "text", nullable: false),
                    IdGenero = table.Column<int>(type: "integer", nullable: false),
                    CodigoDactilar = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Generos_IdGenero",
                        column: x => x.IdGenero,
                        principalTable: "Generos",
                        principalColumn: "IdGenero",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Usuarios_TiposIdentificaciones_IdTipoIdentificacion",
                        column: x => x.IdTipoIdentificacion,
                        principalTable: "TiposIdentificaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parroquias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreParroquia = table.Column<string>(type: "text", nullable: false),
                    IdCiudad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parroquias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parroquias_Ciudades_IdCiudad",
                        column: x => x.IdCiudad,
                        principalTable: "Ciudades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Listas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreLista = table.Column<string>(type: "text", nullable: false),
                    NumeroLista = table.Column<int>(type: "integer", nullable: false),
                    IdProceso = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listas_ProcesosElectorales_IdProceso",
                        column: x => x.IdProceso,
                        principalTable: "ProcesosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreguntasConsultas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextoPregunta = table.Column<string>(type: "text", nullable: false),
                    NumeroPregunta = table.Column<int>(type: "integer", nullable: false),
                    IdProceso = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreguntasConsultas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreguntasConsultas_ProcesosElectorales_IdProceso",
                        column: x => x.IdProceso,
                        principalTable: "ProcesosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecintosElectorales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreRecinto = table.Column<string>(type: "text", nullable: false),
                    DetalleRecinto = table.Column<string>(type: "text", nullable: false),
                    DireccionRecinto = table.Column<string>(type: "text", nullable: true),
                    IdParroquia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecintosElectorales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecintosElectorales_Parroquias_IdParroquia",
                        column: x => x.IdParroquia,
                        principalTable: "Parroquias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreCandidato = table.Column<string>(type: "text", nullable: false),
                    IdLista = table.Column<int>(type: "integer", nullable: false),
                    IdDignidad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidatos_Dignidades_IdDignidad",
                        column: x => x.IdDignidad,
                        principalTable: "Dignidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidatos_Listas_IdLista",
                        column: x => x.IdLista,
                        principalTable: "Listas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpcionConsultas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextoOpcion = table.Column<string>(type: "text", nullable: false),
                    IdPregunta = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpcionConsultas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpcionConsultas_PreguntasConsultas_IdPregunta",
                        column: x => x.IdPregunta,
                        principalTable: "PreguntasConsultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JuntasReceptoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroJunta = table.Column<string>(type: "text", nullable: false),
                    IdGenero = table.Column<int>(type: "integer", nullable: false),
                    IdRecinto = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JuntasReceptoras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JuntasReceptoras_Generos_IdGenero",
                        column: x => x.IdGenero,
                        principalTable: "Generos",
                        principalColumn: "IdGenero",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JuntasReceptoras_RecintosElectorales_IdRecinto",
                        column: x => x.IdRecinto,
                        principalTable: "RecintosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Multimedias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UrlFoto = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    IdCandidato = table.Column<int>(type: "integer", nullable: false),
                    IdLista = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Multimedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Multimedias_Candidatos_IdCandidato",
                        column: x => x.IdCandidato,
                        principalTable: "Candidatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Multimedias_Listas_IdLista",
                        column: x => x.IdLista,
                        principalTable: "Listas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActasAuditorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdProceso = table.Column<int>(type: "integer", nullable: false),
                    IdJunta = table.Column<int>(type: "integer", nullable: false),
                    TotalSufragantesPadron = table.Column<int>(type: "integer", nullable: false),
                    VotosEnUrna = table.Column<int>(type: "integer", nullable: false),
                    HashSeguridad = table.Column<string>(type: "text", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActasAuditorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActasAuditorias_JuntasReceptoras_IdJunta",
                        column: x => x.IdJunta,
                        principalTable: "JuntasReceptoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActasAuditorias_ProcesosElectorales_IdProceso",
                        column: x => x.IdProceso,
                        principalTable: "ProcesosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RepresentantesJuntas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    IdJunta = table.Column<int>(type: "integer", nullable: false),
                    IdRol = table.Column<int>(type: "integer", nullable: false),
                    IdProceso = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepresentantesJuntas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepresentantesJuntas_JuntasReceptoras_IdJunta",
                        column: x => x.IdJunta,
                        principalTable: "JuntasReceptoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepresentantesJuntas_ProcesosElectorales_IdProceso",
                        column: x => x.IdProceso,
                        principalTable: "ProcesosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepresentantesJuntas_Roles_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepresentantesJuntas_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdJunta = table.Column<int>(type: "integer", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votantes_JuntasReceptoras_IdJunta",
                        column: x => x.IdJunta,
                        principalTable: "JuntasReceptoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votantes_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VotoDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTipoVoto = table.Column<int>(type: "integer", nullable: false),
                    IdJunta = table.Column<int>(type: "integer", nullable: false),
                    IdProceso = table.Column<int>(type: "integer", nullable: false),
                    IdLista = table.Column<int>(type: "integer", nullable: false),
                    IdDignidad = table.Column<int>(type: "integer", nullable: false),
                    IdOpcion = table.Column<int>(type: "integer", nullable: false),
                    IdPregunta = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotoDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotoDetalles_Dignidades_IdDignidad",
                        column: x => x.IdDignidad,
                        principalTable: "Dignidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotoDetalles_JuntasReceptoras_IdJunta",
                        column: x => x.IdJunta,
                        principalTable: "JuntasReceptoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotoDetalles_Listas_IdLista",
                        column: x => x.IdLista,
                        principalTable: "Listas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotoDetalles_OpcionConsultas_IdOpcion",
                        column: x => x.IdOpcion,
                        principalTable: "OpcionConsultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotoDetalles_PreguntasConsultas_IdPregunta",
                        column: x => x.IdPregunta,
                        principalTable: "PreguntasConsultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotoDetalles_ProcesosElectorales_IdProceso",
                        column: x => x.IdProceso,
                        principalTable: "ProcesosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotoDetalles_TipoVotos_IdTipoVoto",
                        column: x => x.IdTipoVoto,
                        principalTable: "TipoVotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultadosDetallesAuditorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdActa = table.Column<int>(type: "integer", nullable: false),
                    IdLista = table.Column<int>(type: "integer", nullable: false),
                    VotosContabilizados = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadosDetallesAuditorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultadosDetallesAuditorias_ActasAuditorias_IdActa",
                        column: x => x.IdActa,
                        principalTable: "ActasAuditorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultadosDetallesAuditorias_Listas_IdLista",
                        column: x => x.IdLista,
                        principalTable: "Listas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Padrones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HaVotado = table.Column<bool>(type: "boolean", nullable: false),
                    CodigoAcceso = table.Column<string>(type: "text", nullable: true),
                    IdProceso = table.Column<int>(type: "integer", nullable: false),
                    IdVotante = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Padrones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Padrones_ProcesosElectorales_IdProceso",
                        column: x => x.IdProceso,
                        principalTable: "ProcesosElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Padrones_Votantes_IdVotante",
                        column: x => x.IdVotante,
                        principalTable: "Votantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActasAuditorias_IdJunta",
                table: "ActasAuditorias",
                column: "IdJunta");

            migrationBuilder.CreateIndex(
                name: "IX_ActasAuditorias_IdProceso",
                table: "ActasAuditorias",
                column: "IdProceso");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_IdDignidad",
                table: "Candidatos",
                column: "IdDignidad");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_IdLista",
                table: "Candidatos",
                column: "IdLista");

            migrationBuilder.CreateIndex(
                name: "IX_Ciudades_IdProvincia",
                table: "Ciudades",
                column: "IdProvincia");

            migrationBuilder.CreateIndex(
                name: "IX_JuntasReceptoras_IdGenero",
                table: "JuntasReceptoras",
                column: "IdGenero");

            migrationBuilder.CreateIndex(
                name: "IX_JuntasReceptoras_IdRecinto",
                table: "JuntasReceptoras",
                column: "IdRecinto");

            migrationBuilder.CreateIndex(
                name: "IX_Listas_IdProceso",
                table: "Listas",
                column: "IdProceso");

            migrationBuilder.CreateIndex(
                name: "IX_Multimedias_IdCandidato",
                table: "Multimedias",
                column: "IdCandidato");

            migrationBuilder.CreateIndex(
                name: "IX_Multimedias_IdLista",
                table: "Multimedias",
                column: "IdLista");

            migrationBuilder.CreateIndex(
                name: "IX_OpcionConsultas_IdPregunta",
                table: "OpcionConsultas",
                column: "IdPregunta");

            migrationBuilder.CreateIndex(
                name: "IX_Padrones_IdProceso",
                table: "Padrones",
                column: "IdProceso");

            migrationBuilder.CreateIndex(
                name: "IX_Padrones_IdVotante",
                table: "Padrones",
                column: "IdVotante");

            migrationBuilder.CreateIndex(
                name: "IX_Parroquias_IdCiudad",
                table: "Parroquias",
                column: "IdCiudad");

            migrationBuilder.CreateIndex(
                name: "IX_PreguntasConsultas_IdProceso",
                table: "PreguntasConsultas",
                column: "IdProceso");

            migrationBuilder.CreateIndex(
                name: "IX_ProcesosElectorales_IdTipoProceso",
                table: "ProcesosElectorales",
                column: "IdTipoProceso");

            migrationBuilder.CreateIndex(
                name: "IX_RecintosElectorales_IdParroquia",
                table: "RecintosElectorales",
                column: "IdParroquia");

            migrationBuilder.CreateIndex(
                name: "IX_RepresentantesJuntas_IdJunta",
                table: "RepresentantesJuntas",
                column: "IdJunta");

            migrationBuilder.CreateIndex(
                name: "IX_RepresentantesJuntas_IdProceso",
                table: "RepresentantesJuntas",
                column: "IdProceso");

            migrationBuilder.CreateIndex(
                name: "IX_RepresentantesJuntas_IdRol",
                table: "RepresentantesJuntas",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_RepresentantesJuntas_IdUsuario",
                table: "RepresentantesJuntas",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosDetallesAuditorias_IdActa",
                table: "ResultadosDetallesAuditorias",
                column: "IdActa");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosDetallesAuditorias_IdLista",
                table: "ResultadosDetallesAuditorias",
                column: "IdLista");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdGenero",
                table: "Usuarios",
                column: "IdGenero");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdTipoIdentificacion",
                table: "Usuarios",
                column: "IdTipoIdentificacion");

            migrationBuilder.CreateIndex(
                name: "IX_Votantes_IdJunta",
                table: "Votantes",
                column: "IdJunta");

            migrationBuilder.CreateIndex(
                name: "IX_Votantes_IdUsuario",
                table: "Votantes",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDetalles_IdDignidad",
                table: "VotoDetalles",
                column: "IdDignidad");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDetalles_IdJunta",
                table: "VotoDetalles",
                column: "IdJunta");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDetalles_IdLista",
                table: "VotoDetalles",
                column: "IdLista");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDetalles_IdOpcion",
                table: "VotoDetalles",
                column: "IdOpcion");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDetalles_IdPregunta",
                table: "VotoDetalles",
                column: "IdPregunta");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDetalles_IdProceso",
                table: "VotoDetalles",
                column: "IdProceso");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDetalles_IdTipoVoto",
                table: "VotoDetalles",
                column: "IdTipoVoto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Multimedias");

            migrationBuilder.DropTable(
                name: "Padrones");

            migrationBuilder.DropTable(
                name: "RepresentantesJuntas");

            migrationBuilder.DropTable(
                name: "ResultadosDetallesAuditorias");

            migrationBuilder.DropTable(
                name: "VotoDetalles");

            migrationBuilder.DropTable(
                name: "Candidatos");

            migrationBuilder.DropTable(
                name: "Votantes");

            migrationBuilder.DropTable(
                name: "ActasAuditorias");

            migrationBuilder.DropTable(
                name: "OpcionConsultas");

            migrationBuilder.DropTable(
                name: "TipoVotos");

            migrationBuilder.DropTable(
                name: "Dignidades");

            migrationBuilder.DropTable(
                name: "Listas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "JuntasReceptoras");

            migrationBuilder.DropTable(
                name: "PreguntasConsultas");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "TiposIdentificaciones");

            migrationBuilder.DropTable(
                name: "Generos");

            migrationBuilder.DropTable(
                name: "RecintosElectorales");

            migrationBuilder.DropTable(
                name: "ProcesosElectorales");

            migrationBuilder.DropTable(
                name: "Parroquias");

            migrationBuilder.DropTable(
                name: "TipoProcesos");

            migrationBuilder.DropTable(
                name: "Ciudades");

            migrationBuilder.DropTable(
                name: "Provincias");
        }
    }
}
