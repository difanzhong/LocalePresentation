using System;
using System.Data;
using System.Data.SqlTypes;
using Npgsql;
using NpgsqlTypes;

namespace DataCollection
{
    public class Suburb
    {
        public String loc_pid { get; set; }
        public String suburbName { get; set; }
        public String geometryData { get; set; }
        public int    suburbId { get; set; }

        
        /*** 
            find the outline of suburb polygon 
             based on point coordinates given 
        ***/
        public static String getSuburbOutline(double longitude, double latitude)
        {
            
            DataTable dt = new DataTable();
            String Sql =
                "SELECT row_to_json(fc) FROM ( SELECT 'FeatureCollection' As type, array_to_json(array_agg(f)) As features FROM (SELECT 'Feature' As type, ST_AsGeoJSON(lg.geom)::json As geometry, row_to_json((id, suburb_name)) As properties FROM suburbs As lg WHERE St_Within(St_SetSRID(St_MakePoint(:long,:lati), 4326), lg.geom) ) As f )  As fc;";

            try
            {
                NpgsqlConnection conn = DBConnect.OpenConnection();
                NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = Sql;
                cmd.Parameters.Add((new NpgsqlParameter("long", NpgsqlDbType.Double)));
                cmd.Parameters.Add((new NpgsqlParameter("lati", NpgsqlDbType.Double)));
                cmd.Prepare();
                cmd.Parameters[0].Value = longitude;
                cmd.Parameters[1].Value = latitude;
                NpgsqlDataAdapter nda = new NpgsqlDataAdapter(cmd);
                nda.Fill(dt);
                
            }
            catch (Exception e)
            {
                Console.WriteLine("get suburb outline failed at sql query");
                throw e;
            }
            finally
            {
                DBConnect.CloseConnection();
            }

            return dt.Rows.Count > 0? dt.Rows[0][0].ToString(): null;
        }
    }
}