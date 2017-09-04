using System;
using System.Data;
using Npgsql;
using System.Collections;

namespace DataCollection
{
    public class DBConnect
    {
        private static String _connStr = "Server=localhost;Port=5432;User Id=postgres;Password=adidas9038;Database=MP;CommandTimeout=300;";
        public static NpgsqlConnection con;

        public DBConnect()
        {
            
        }

        public static NpgsqlConnection OpenConnection()
        {
            try
            {
                con = new NpgsqlConnection(_connStr);
                con.Open();
            }
            catch (Exception e) 
            {
                Console.Write("Error Occur:" + e.Message);
            }
            return con;
        }

        public static void CloseConnection() 
        {
            try
            {
                con.Close();
            }
            catch(Exception e){
                Console.Write("Error Occur:" + e.Message);
            }
        }

        public static DataTable extractData(String sqlCommand)
        {
            DataTable dt = new DataTable();

            try
            {
                NpgsqlConnection conn = OpenConnection();
                NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlCommand;
                NpgsqlDataAdapter nda = new NpgsqlDataAdapter(cmd);
                nda.Fill(dt);
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        public static DataTable executeQuery(String sqlCommand, ArrayList sqlParam)
        {
            DataTable dt = new DataTable();
            try
            {
				NpgsqlConnection conn = OpenConnection();
				NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = sqlCommand;
                if ( sqlParam != null )
                {
                    for (int i = 0; i < sqlParam.Count; i++)
                    {
                        NpgsqlParameter param = (NpgsqlParameter)sqlParam[i];
                        //cmd.Parameters.AddWithValue();
                    }
                }

				NpgsqlDataAdapter nda = new NpgsqlDataAdapter(cmd);
				nda.Fill(dt);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public static void Insert()
        {
			try
			{
				NpgsqlConnection conn = OpenConnection();
				NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO tweets (id, origintext) VALUES (:id,:text);";
                cmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Integer));
                cmd.Parameters.Add(new NpgsqlParameter("text", NpgsqlTypes.NpgsqlDbType.Varchar));

                cmd.Prepare();

                cmd.Parameters[0].Value = 1;
                cmd.Parameters[1].Value = "hahsf'sfdwv;sdf";
                int recordAffected = cmd.ExecuteNonQuery();
                Console.WriteLine(recordAffected);
                conn.Close();
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				con.Close();
			}    
        }
    }
}
