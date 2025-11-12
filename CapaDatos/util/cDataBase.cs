using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.util
{
    public class cDataBase
    {
        private OleDbConnection oleDbCnn;
        private SqlConnection sqlCnn;
        private readonly IConfiguration _configuration;
        public cDataBase(IConfiguration configuration)
        {
            _configuration = configuration;

            //if (coneccion == "")
            //{ // aqui ver que conexiones encuentra 
            //}

            string connString = _configuration.GetConnectionString("ListasConnectionString");
            oleDbCnn = new OleDbConnection(connString.ToString());

            string SqlconnStr = _configuration.GetConnectionString("SarlaftConnectionString");
            sqlCnn = new SqlConnection(SqlconnStr.ToString());

            //string connString = ConfigurationManager.ConnectionStrings["ListasConnectionString"].ToString();
            //oleDbCnn = new OleDbConnection(connString.ToString());

            //string SqlconnStr = ConfigurationManager.ConnectionStrings["SarlaftConnectionString"].ToString();
            //sqlCnn = new SqlConnection(SqlconnStr.ToString());

        }

        public void conectar()
        {
            oleDbCnn.Open();
        }

        public void desconectar()
        {
            oleDbCnn.Close();
        }

        public DataTable ejecutarConsulta(String txtQuery)
        {
            DataTable dtInformation = new DataTable();
#pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(txtQuery, oleDbCnn);
#pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            oleDbDataAdapter.SelectCommand.CommandType = CommandType.Text;
            oleDbDataAdapter.SelectCommand.CommandTimeout = 3600;
            oleDbDataAdapter.Fill(dtInformation);
            return dtInformation;
        }

        public int ejecutarTratamiento(String txtQuery)
        {
            int cantidad = 0;
            DataTable dtInformation = new DataTable();
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(txtQuery, oleDbCnn);
            oleDbDataAdapter.SelectCommand.CommandType = CommandType.Text;
            oleDbDataAdapter.SelectCommand.CommandTimeout = 3600;
            oleDbDataAdapter.Fill(dtInformation);
            cantidad = dtInformation.Rows.Count;
            return cantidad;
        }

        public DataTable buscarIdEstado(string txtQuery)
        {
            DataTable dtInformation = new DataTable();
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(txtQuery, oleDbCnn);
            oleDbDataAdapter.SelectCommand.CommandType = CommandType.Text;
            oleDbDataAdapter.SelectCommand.CommandTimeout = 3600;
            oleDbDataAdapter.Fill(dtInformation);
            return dtInformation;
        }
        public void ejecutarQuery(String txtQuery)
        {
            OleDbCommand oleDbCmn = new OleDbCommand(txtQuery, oleDbCnn);
            oleDbCmn.CommandType = CommandType.Text;
            oleDbCmn.CommandTimeout = 3600;
            oleDbCmn.ExecuteNonQuery();
        }


        public void ejecutaQueryadapter(String txtQuery)
        {
            SqlConnection cnn = sqlCnn;
            try
            {
                cnn.Open();
                SqlCommand sqlCmd = new SqlCommand(txtQuery, cnn);
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandTimeout = 3600;
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {//capturar error                            

                throw new Exception(ex.Message);
            }
            finally
            {
                cnn.Close();
            }



            OleDbCommand oleDbCmn = new OleDbCommand(txtQuery, oleDbCnn);
            oleDbCmn.CommandType = CommandType.Text;
            oleDbCmn.CommandTimeout = 3600;
            oleDbCmn.ExecuteNonQuery();
        }



        public void ejecutarSP(String txtNombreSP)
        {
            OleDbCommand oleDbCmn = new OleDbCommand(txtNombreSP, oleDbCnn);
            oleDbCmn.CommandType = CommandType.StoredProcedure;
            oleDbCmn.CommandTimeout = 3600;
            oleDbCmn.ExecuteNonQuery();
        }

        public void ejecutarSPParametros(String txtNombreSP, OleDbParameter[] objParameter)
        {
            OleDbCommand oleDbCmm = new OleDbCommand(txtNombreSP, oleDbCnn);
            oleDbCmm.CommandType = CommandType.StoredProcedure;
            foreach (OleDbParameter objParametro in objParameter)
            {
                oleDbCmm.Parameters.Add(objParametro);
            }
            oleDbCmm.CommandTimeout = 3600;
            oleDbCmm.ExecuteNonQuery();
        }

        /// <summary>
        /// Ejecuta procedimiento almacenado que devuelve un valor integer. El procedimiento a llamar debe tener el parametro de salida @Resultado
        /// </summary>
        /// <param name="NombreSp">Nombre del procedimiento</param>
        /// <param name="Parametros">Lista de parametros</param>
        /// <returns>Int</returns> 
        public int EjecutarSPParametrosReturnInteger(string NombreSp, List<SqlParameter> Parametros)
        {
            SqlConnection cnn = sqlCnn;
            try
            {
                cnn.Open();
                SqlCommand sqlCmd = new SqlCommand(NombreSp, cnn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddRange(Parametros.ToArray());
                sqlCmd.CommandTimeout = 3600;
                sqlCmd.ExecuteNonQuery();
                int resultado = (int)sqlCmd.Parameters["@Resultado"].Value;

                return resultado;
            }
            catch (Exception ex)
            {//capturar error                            

                throw new Exception(ex.Message);
            }
            finally
            {
                cnn.Close();
            }
        }

        public int EjecutarSPParametros(string NombreSp, List<SqlParameter> Parametros)
        {
            SqlConnection cnn = sqlCnn;
            try
            {
                cnn.Open();
                SqlCommand sqlCmd = new SqlCommand(NombreSp, cnn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddRange(Parametros.ToArray());
                sqlCmd.CommandTimeout = 3600;
                sqlCmd.ExecuteNonQuery();
                int resultado = sqlCmd.ExecuteNonQuery();
                // int resultado = (int)sqlCmd.Parameters["@Resultado"].Value;
                return resultado;
            }
            catch (Exception ex)
            {//capturar error                            

                throw new Exception(ex.Message);
            }
            finally
            {
                cnn.Close();
            }
        }

        public void EjecutarSPParametrosSinRetorno(string NombreSp, List<SqlParameter> Parametros)
        {
            SqlConnection cnn = sqlCnn;
            try
            {
                cnn.Open();
                SqlCommand sqlCmd = new SqlCommand(NombreSp, cnn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddRange(Parametros.ToArray());
                sqlCmd.CommandTimeout = 3600;
                sqlCmd.ExecuteNonQuery();
                //int resultado = sqlCmd.ExecuteNonQuery();
                // int resultado = (int)sqlCmd.Parameters["@Resultado"].Value;
                //return resultado;
            }
            catch (Exception ex)
            {//capturar error                            

                throw new Exception(ex.Message);
            }
            finally
            {
                cnn.Close();
            }
        }

        public void EjecutarSPParametrosSinRetornonuew(string NombreSp, List<SqlParameter> Parametros)
        {
            SqlConnection cnn = sqlCnn;
            try
            {
                cnn.Open();
                SqlCommand sqlCmd = new SqlCommand(NombreSp, cnn);
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.Parameters.AddRange(Parametros.ToArray());
                sqlCmd.CommandTimeout = 3600;
                sqlCmd.ExecuteNonQuery();
                //int resultado = sqlCmd.ExecuteNonQuery();
                // int resultado = (int)sqlCmd.Parameters["@Resultado"].Value;
                //return resultado;
            }
            catch (Exception ex)
            {//capturar error                            

                throw new Exception(ex.Message);
            }
            finally
            {
                cnn.Close();
            }
        }

        public int ejecutarQueryReturn(string txtQuery)
        {
            try
            {
                conectar();
                OleDbCommand oleDbCmn = new OleDbCommand(txtQuery, oleDbCnn);
                oleDbCmn.CommandType = CommandType.Text;
                oleDbCmn.CommandTimeout = 3600;
                int resultado = oleDbCmn.ExecuteNonQuery();
                return resultado;
            }
            catch (Exception ex)
            {   //capturar error                            

                throw new Exception(ex.Message);
            }
            finally
            {
                desconectar();
            }
        }

        /// <summary>
        /// Ejecuta procedimiento almacenado que retorna una Datatable
        /// </summary>
        /// <param name="NombreSp">Nombre del procedimiento</param>
        /// <param name="Parametros">Lista de parametros</param>
        /// <returns>Int</returns>
        public DataTable EjecutarSPParametrosReturnDatatable(string NombreSp, List<SqlParameter> Parametros)
        {
            SqlConnection cnn = sqlCnn;
            try
            {
                DataTable dt = new DataTable();
                cnn.Open();
                SqlCommand sqlCmd = new SqlCommand(NombreSp, cnn);
                SqlDataAdapter sqlAdt = new SqlDataAdapter();
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddRange(Parametros.ToArray());
                sqlCmd.CommandTimeout = 3600;
                sqlAdt.SelectCommand = sqlCmd;
                sqlAdt.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnn.Close();
            }
        }

        public System.Data.DataSet EjecutarSp(string NombreSp, List<SqlParameter> Parametros)
        {
            SqlConnection cnn = sqlCnn;
            try
            {
                System.Data.DataSet ds = new System.Data.DataSet();
                DataTable dt = new DataTable();
                cnn.Open();
                SqlCommand sqlCmd = new SqlCommand(NombreSp, cnn);
                SqlDataAdapter sqlAdt = new SqlDataAdapter();
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddRange(Parametros.ToArray());
                sqlCmd.CommandTimeout = 3600;
                sqlAdt.SelectCommand = sqlCmd;
                sqlAdt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnn.Close();
            }
        }

        public void mtdConectarSql()
        {
            sqlCnn.Open();
        }

        public void mtdDesconectarSql()
        {
            sqlCnn.Close();
        }



        public DataTable mtdEjecutarConsultaSQL(string strConsulta)
        {

            DataTable dtInfo = new DataTable();
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(strConsulta, sqlCnn);
            sqlAdapter.SelectCommand.CommandType = CommandType.Text;
            sqlAdapter.SelectCommand.CommandTimeout = 3600;
            sqlAdapter.Fill(dtInfo);

            return dtInfo;
        }

        public byte[] mtdEjecutarConsultaSqlPdf(string strConsulta)
        {
            byte[] bInfo = null;

            using (SqlCommand sqlComm = new SqlCommand(strConsulta, sqlCnn))
            {
                SqlDataReader dr = sqlComm.ExecuteReader(System.Data.CommandBehavior.Default);
                if (dr.Read())
                {
                    bInfo = (byte[])dr.GetValue(1);
                }
                dr.Close();
            }
            return bInfo;
        }

        public DataTable EjecutarConsultaConParametros(string textoSql, List<SqlParameter> parametros)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cnn = new SqlConnection(_configuration.GetConnectionString("SarlaftConnectionString")))
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand(textoSql, cnn))
                {
                    cmd.CommandType = CommandType.Text;
                    if (parametros != null)
                        cmd.Parameters.AddRange(parametros.ToArray());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }


    }
}
