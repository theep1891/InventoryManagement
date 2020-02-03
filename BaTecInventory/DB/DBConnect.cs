using System;
using System.Collections.Generic;
using System.ComponentModel;
using BaTecInventory.Models;
using MySql.Data.MySqlClient;
using System.Linq;

namespace BaTecInventory.DB
{
    public class DBConnect
    {
        private readonly string connectionIdentication =
           "datasource=127.0.0.1;" +
           "port=3306;" +
           "username =root;" +
           "password =;"
           //"Convert Zero Datetime = True;"
           /*"Allow Zero Datetime=True;"*/
           ;

        private readonly MySqlCommand dbCommand;
        private readonly MySqlConnection dbConnection;


        public DBConnect()
        {
            dbConnection = new MySqlConnection(connectionIdentication);
            dbCommand = new MySqlCommand
            {
                Connection = dbConnection
            };
        }

        public int FindID(string query, string columnName)
        {
            int returnvalue = -1;

            dbCommand.CommandText = query;
            dbCommand.Parameters.Clear();

            try
            {
                dbCommand.Connection.Open();

                var reader = dbCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        returnvalue = int.Parse(reader[columnName].ToString());
                    }

                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbCommand.Connection.Close();
            }

            return returnvalue;
        }
        
        public List<User> Users()
        {
            var returnList = new List<User>();

            dbCommand.CommandText = "SELECT * FROM inventory.user";
            dbCommand.Parameters.Clear();

            try
            {
                dbCommand.Connection.Open();

                var reader = dbCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        var temp = new User(
                            int.Parse(reader["userid"].ToString()),
                            reader["username"].ToString(),
                            reader["password"].ToString(),
                            reader["useremail"].ToString(),
                            int.Parse(reader["userphonenumber"].ToString())
                        );

                        returnList.Add(temp);
                    }
                reader.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbCommand.Connection.Close();
            }

            return returnList;
        }

        public void Create(string query)
        {
            dbCommand.CommandText = query;

            dbCommand.Connection.Open();
            try
            {
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbCommand.Connection.Close();
            }
        }

        public void Edit(string query)
        {
            dbCommand.CommandText = query;
            //dbCommand.Parameters.Clear();
            //dbCommand.Parameters.AddWithValue("@Name", Name);

            dbCommand.Connection.Open();
            try
            {
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbCommand.Connection.Close();
            }
        }

        public void Delete(string query)
        {
            dbCommand.CommandText = query;
            //dbCommand.Parameters.Clear();
            //dbCommand.Parameters.AddWithValue("@ID", Id);

            dbCommand.Connection.Open();
            try
            {
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbCommand.Connection.Close();
            }
        }

        public void DataValidation(string query)
        {
            dbCommand.CommandText = query;


            dbCommand.Connection.Open();
            try
            {
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbCommand.Connection.Close();
            }
        }

        public List<List<string>> Show(string query)
        {
            var returnList = new List<List<string>>();

            dbCommand.CommandText = query;
            dbCommand.Parameters.Clear();

            try
            {
                dbCommand.Connection.Open();

                var reader = dbCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        List<string> temp = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            temp.Add(reader[i].ToString());
                        }
                        returnList.Add(temp);
                    }

                reader.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbCommand.Connection.Close();
            }

            return returnList;
        }

        public List<string> AllList(string query)
        {
            var returnList = new List<string>();

            dbCommand.CommandText = query;
            dbCommand.Parameters.Clear();

            try
            {
                dbCommand.Connection.Open();

                var reader = dbCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        returnList.Add(reader[0].ToString());
                    }

                reader.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbCommand.Connection.Close();
            }

            return returnList;
        }

        public string Select(string query)
        {
            string returnValue = string.Empty;

            dbCommand.CommandText = query;
            dbCommand.Parameters.Clear();

            try
            {
                dbCommand.Connection.Open();

                var reader = dbCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        returnValue = reader[0].ToString();
                    }

                reader.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbCommand.Connection.Close();
            }

            return returnValue;
        }

       


        
    }
}