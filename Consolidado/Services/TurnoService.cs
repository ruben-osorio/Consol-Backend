using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Consolidado.Models;
using Microsoft.Extensions.Configuration;

namespace Consolidado.Services
{
    public class TurnoService : ITurnoService
    {
        private readonly string _connectionString;

        public TurnoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration), 
                    "La cadena de conexión 'DefaultConnection' no puede ser nula.");
        }

        public async Task<IEnumerable<TurnoModel>> GetTurnos()
        {
            var turnos = new List<TurnoModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT id_turno, alias, definicion, in_ahead_margin, in_above_margin, out_ahead_margin, out_above_margin, duration FROM rrhh_turnos ORDER BY alias", connection))
                {
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            turnos.Add(new TurnoModel
                            {
                                IdTurno = Convert.ToInt32(reader["id_turno"]),
                                Alias = reader["alias"].ToString(),
                                Definicion = reader["definicion"].ToString(),
                                InAheadMargin = Convert.ToInt32(reader["in_ahead_margin"]),
                                InAboveMargin = Convert.ToInt32(reader["in_above_margin"]),
                                OutAheadMargin = Convert.ToInt32(reader["out_ahead_margin"]),
                                OutAboveMargin = Convert.ToInt32(reader["out_above_margin"]),
                                Duration = Convert.ToInt32(reader["duration"])
                            });
                        }
                    }
                }
            }

            return turnos;
        }

        public async Task<bool> SincronizarTurnos()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_SincronizarTurnos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        await command.ExecuteNonQueryAsync();
                    }
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ActualizarDefinicionTurno(int idTurno, string definicion)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("UPDATE rrhh_turnos SET definicion = @definicion WHERE id_turno = @idTurno", connection))
                    {
                        command.Parameters.AddWithValue("@idTurno", idTurno);
                        command.Parameters.AddWithValue("@definicion", definicion);
                        
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<TurnoModel?> GetTurnoById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT id_turno, alias, definicion, in_ahead_margin, in_above_margin, out_ahead_margin, out_above_margin, duration FROM rrhh_turnos WHERE id_turno = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new TurnoModel
                            {
                                IdTurno = Convert.ToInt32(reader["id_turno"]),
                                Alias = reader["alias"].ToString(),
                                Definicion = reader["definicion"].ToString(),
                                InAheadMargin = Convert.ToInt32(reader["in_ahead_margin"]),
                                InAboveMargin = Convert.ToInt32(reader["in_above_margin"]),
                                OutAheadMargin = Convert.ToInt32(reader["out_ahead_margin"]),
                                OutAboveMargin = Convert.ToInt32(reader["out_above_margin"]),
                                Duration = Convert.ToInt32(reader["duration"])
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
} 