using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaVotacion.API.Migrations
{
    /// <inheritdoc />
    public partial class v01SistemaVotacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreProv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreRol = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DescripcionRol = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoIdentificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DetalleTipIdentifiacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoIdentificacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoProceso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreTipoProceso = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProceso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoVoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreTipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoVoto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ciudad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreCiu = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdProvincia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciudad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ciudad_Provincia_IdProvincia",
                        column: x => x.IdProvincia,
                        principalTable: "Provincia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdRol = table.Column<int>(type: "integer", nullable: false),
                    IdTipoIdentificacion = table.Column<int>(type: "integer", nullable: false),
                    NumeroIdentificacion = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    CodigoDactilar = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaExpedicion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Usuario_TipoIdentificacion_IdTipoIdentificacion",
                        column: x => x.IdTipoIdentificacion,
                        principalTable: "TipoIdentificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcesoElectoral",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreProceso = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdTipoProceso = table.Column<int>(type: "integer", nullable: false),
                    TipoProcesoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcesoElectoral", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcesoElectoral_TipoProceso_TipoProcesoId",
                        column: x => x.TipoProcesoId,
                        principalTable: "TipoProceso",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Parroquia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreParro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdCiudad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parroquia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parroquia_Ciudad_IdCiudad",
                        column: x => x.IdCiudad,
                        principalTable: "Ciudad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialAcceso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    IntentosFallidos = table.Column<int>(type: "integer", nullable: false),
                    CuentaBloqueada = table.Column<bool>(type: "boolean", nullable: true),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialAcceso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialAcceso_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dignidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreDignidad = table.Column<string>(type: "text", nullable: false),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dignidad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dignidad_ProcesoElectoral_ProcesoElectoralId",
                        column: x => x.ProcesoElectoralId,
                        principalTable: "ProcesoElectoral",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Lista",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreLista = table.Column<string>(type: "text", nullable: false),
                    NumeroLista = table.Column<int>(type: "integer", nullable: false),
                    IdProceso = table.Column<int>(type: "integer", nullable: false),
                    ProcesosId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lista", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lista_ProcesoElectoral_ProcesosId",
                        column: x => x.ProcesosId,
                        principalTable: "ProcesoElectoral",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecintoElectoral",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreRecinto = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    DetalleRecinto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DireccionRecinto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IdParroquia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecintoElectoral", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecintoElectoral_Parroquia_IdParroquia",
                        column: x => x.IdParroquia,
                        principalTable: "Parroquia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreCandidato = table.Column<string>(type: "text", nullable: false),
                    IdLista = table.Column<int>(type: "integer", nullable: false),
                    IdDignidad = table.Column<int>(type: "integer", nullable: false),
                    ListaId = table.Column<int>(type: "integer", nullable: true),
                    DignidadId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidato_Dignidad_DignidadId",
                        column: x => x.DignidadId,
                        principalTable: "Dignidad",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Candidato_Lista_ListaId",
                        column: x => x.ListaId,
                        principalTable: "Lista",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JuntaReceptora",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroJunta = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Genero = table.Column<string>(type: "text", nullable: true),
                    IdRecinto = table.Column<int>(type: "integer", nullable: false),
                    RecintosId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JuntaReceptora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JuntaReceptora_RecintoElectoral_RecintosId",
                        column: x => x.RecintosId,
                        principalTable: "RecintoElectoral",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Multimedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UrlFoto = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    IdCandidato = table.Column<int>(type: "integer", nullable: true),
                    IdLista = table.Column<int>(type: "integer", nullable: true),
                    CandidatoId = table.Column<int>(type: "integer", nullable: true),
                    ListaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Multimedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Multimedia_Candidato_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidato",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Multimedia_Lista_ListaId",
                        column: x => x.ListaId,
                        principalTable: "Lista",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Votante",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdJunta = table.Column<int>(type: "integer", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    JuntaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votante", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votante_JuntaReceptora_JuntaId",
                        column: x => x.JuntaId,
                        principalTable: "JuntaReceptora",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Votante_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VotoDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTipoVoto = table.Column<int>(type: "integer", nullable: false),
                    IdJunta = table.Column<int>(type: "integer", nullable: false),
                    IdProceso = table.Column<int>(type: "integer", nullable: false),
                    TipoVotoId = table.Column<int>(type: "integer", nullable: true),
                    JuntaId = table.Column<int>(type: "integer", nullable: true),
                    ProcesoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotoDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotoDetalle_JuntaReceptora_JuntaId",
                        column: x => x.JuntaId,
                        principalTable: "JuntaReceptora",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VotoDetalle_ProcesoElectoral_ProcesoId",
                        column: x => x.ProcesoId,
                        principalTable: "ProcesoElectoral",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VotoDetalle_TipoVoto_TipoVotoId",
                        column: x => x.TipoVotoId,
                        principalTable: "TipoVoto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Padron",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HaVotado = table.Column<bool>(type: "boolean", nullable: false),
                    IdProceso = table.Column<int>(type: "integer", nullable: false),
                    IdVotante = table.Column<int>(type: "integer", nullable: false),
                    ProcesoId = table.Column<int>(type: "integer", nullable: true),
                    VotanteId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Padron", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Padron_ProcesoElectoral_ProcesoId",
                        column: x => x.ProcesoId,
                        principalTable: "ProcesoElectoral",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Padron_Votante_VotanteId",
                        column: x => x.VotanteId,
                        principalTable: "Votante",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidato_DignidadId",
                table: "Candidato",
                column: "DignidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidato_ListaId",
                table: "Candidato",
                column: "ListaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ciudad_IdProvincia",
                table: "Ciudad",
                column: "IdProvincia");

            migrationBuilder.CreateIndex(
                name: "IX_Dignidad_ProcesoElectoralId",
                table: "Dignidad",
                column: "ProcesoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialAcceso_IdUsuario",
                table: "HistorialAcceso",
                column: "IdUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JuntaReceptora_RecintosId",
                table: "JuntaReceptora",
                column: "RecintosId");

            migrationBuilder.CreateIndex(
                name: "IX_Lista_ProcesosId",
                table: "Lista",
                column: "ProcesosId");

            migrationBuilder.CreateIndex(
                name: "IX_Multimedia_CandidatoId",
                table: "Multimedia",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Multimedia_ListaId",
                table: "Multimedia",
                column: "ListaId");

            migrationBuilder.CreateIndex(
                name: "IX_Padron_ProcesoId",
                table: "Padron",
                column: "ProcesoId");

            migrationBuilder.CreateIndex(
                name: "IX_Padron_VotanteId",
                table: "Padron",
                column: "VotanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Parroquia_IdCiudad",
                table: "Parroquia",
                column: "IdCiudad");

            migrationBuilder.CreateIndex(
                name: "IX_ProcesoElectoral_TipoProcesoId",
                table: "ProcesoElectoral",
                column: "TipoProcesoId");

            migrationBuilder.CreateIndex(
                name: "IX_RecintoElectoral_IdParroquia",
                table: "RecintoElectoral",
                column: "IdParroquia");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdRol",
                table: "Usuario",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdTipoIdentificacion",
                table: "Usuario",
                column: "IdTipoIdentificacion");

            migrationBuilder.CreateIndex(
                name: "IX_Votante_IdUsuario",
                table: "Votante",
                column: "IdUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votante_JuntaId",
                table: "Votante",
                column: "JuntaId");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDetalle_JuntaId",
                table: "VotoDetalle",
                column: "JuntaId");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDetalle_ProcesoId",
                table: "VotoDetalle",
                column: "ProcesoId");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDetalle_TipoVotoId",
                table: "VotoDetalle",
                column: "TipoVotoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialAcceso");

            migrationBuilder.DropTable(
                name: "Multimedia");

            migrationBuilder.DropTable(
                name: "Padron");

            migrationBuilder.DropTable(
                name: "VotoDetalle");

            migrationBuilder.DropTable(
                name: "Candidato");

            migrationBuilder.DropTable(
                name: "Votante");

            migrationBuilder.DropTable(
                name: "TipoVoto");

            migrationBuilder.DropTable(
                name: "Dignidad");

            migrationBuilder.DropTable(
                name: "Lista");

            migrationBuilder.DropTable(
                name: "JuntaReceptora");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "ProcesoElectoral");

            migrationBuilder.DropTable(
                name: "RecintoElectoral");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "TipoIdentificacion");

            migrationBuilder.DropTable(
                name: "TipoProceso");

            migrationBuilder.DropTable(
                name: "Parroquia");

            migrationBuilder.DropTable(
                name: "Ciudad");

            migrationBuilder.DropTable(
                name: "Provincia");
        }
    }
}
