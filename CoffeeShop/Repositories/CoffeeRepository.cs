﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CoffeeShop.Models;


namespace CoffeeShop.Repositories
{
    public class CoffeeRepository
    {
        private readonly string _connectionString;
        public CoffeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection Connection
        {
            get { return new SqlConnection(_connectionString); }
        }

        public List<Coffee> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id AS cId, 
                                               Title, 
                                               BeanVarietyId, 
                                               [Name], 
                                               Region 
                                          FROM Coffee c
                                               JOIN BeanVariety bv ON bv.Id = BeanVarietyId";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var coffees = new List<Coffee>();
                        while (reader.Read())
                        {
                            var coffee = new Coffee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                BeanVarietyId = reader.GetInt32(reader.GetOrdinal("BeanVarietyId")),
                                BeanVariety = new BeanVariety()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Region = reader.GetString(reader.GetOrdinal("Region")),
                                }
                            };

                            coffees.Add(coffee);
                        }

                        return coffees;
                    }

                }
            }
        }

        public Coffee Get(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id AS cId, 
                                               Title, 
                                               BeanVarietyId, 
                                               [Name], 
                                               Region,
                                               Notes
                                          FROM Coffee c
                                               JOIN BeanVariety bv ON bv.Id = BeanVarietyId";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Coffee coffee = null;
                        if (reader.Read())
                        {
                            coffee = new Coffee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                BeanVarietyId = reader.GetInt32(reader.GetOrdinal("BeanVarietyId")),
                                BeanVariety = new BeanVariety()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Region = reader.GetString(reader.GetOrdinal("Region")),
                                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes"))
                                }
                            };
                        };
                        return coffee;
                    }

                }
            }
        }

        public void Update(Coffee coffee)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Coffee
                           SET Id = @id,
                               Title = @title,
                               BeanVarietyId = @beanVarietyId";
                    cmd.Parameters.AddWithValue("@id", coffee.Id);
                    cmd.Parameters.AddWithValue("@title", coffee.Title);
                    cmd.Parameters.AddWithValue("@beanVarietyId", coffee.BeanVarietyId);

                    cmd.ExecuteNonQuery();

                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Coffee WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}