/*
############################DOCUMENTACION######################################
###############################################################################
# Module       = 
# Version      = v1
# Date Time    = Jueves, 01 de Noviembre de 2012 03:20:42 p.m.
# Ident        = 
# Author       = German Schauvinhold
# Company      = Mariant.
# Description  = Controller de tabla ProductBarCodeHist       
###############################################################################
############################FIN DOCUMENTACION##################################
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebAsistida.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Teradata.Client.Provider;


namespace WebAsistida.lib
{
    public class ProductBarCodeHistController : CfBaseController
    {
        public override CfDbTable tableInfo()
        {
            return DbModel.ProductBarCodeHist;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override String SubmitProcess(JObject param, Dictionary<String, List<String>> dicQryTxt, EntryAction eaction)
        {
            int enabledNum = 0;
            String strconn = Parametros().strTeradataLoad;
            String _WebEvent_id = Request.Properties.ContainsKey("WebEvent_id") ? Request.Properties["WebEvent_id"].ToString() : "";
            String _WebUser_EMail = Request.Properties.ContainsKey("WebUser_EMail") ? Request.Properties["WebUser_EMail"].ToString() : "lourdes_martinez@carrefour.com";
            String _WebUser_id = Request.Properties.ContainsKey("WebUser_id") ? Request.Properties["WebUser_id"].ToString() : "lourdes_martinez";
            String _WebDbObject_id = this.ToString().Split('.')[2];
            String _WebQueueEntryType_id = eaction.ToString();
            String _WebQueueMediaType_id = ImportMedia.NONE.ToString();
            String submit_response;
            String resFileUri = "";
            String _status = "Pendiente";
            List<Dictionary<String, String>> dbObjPrecedents = null;
            String targetFilename = "";
            String _WebEventParameters_Txt = JsonConvert.SerializeObject(param, Newtonsoft.Json.Formatting.Indented);
            string PATH = HttpContext.Current.Server.MapPath("~/process/");
            String _WorkingDir_Name = PATH + _WebEvent_id + "_work";
            Directory.CreateDirectory(_WorkingDir_Name);
            _WorkingDir_Name = Regex.Replace(_WorkingDir_Name, @"\\", @"\\");

            _WebEventParameters_Txt = _WebEventParameters_Txt.Replace("\n", "");
            _WebEventParameters_Txt = _WebEventParameters_Txt.Replace("\r", "");
            int iStringSize = ASCIIEncoding.ASCII.GetByteCount(_WebEventParameters_Txt);

            if (iStringSize > 50000)
            {
                throw new Exception("El filtro supera el tamaño permitido. Seleccione menos datos en el filtro. Muchas gracias.");
            }

            TdConnection cntn = new TdConnection(strconn);
            cntn.Open();
            TdTransaction trx = cntn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                this.LockTableForQueue(cntn, trx);

                if (!_WebUser_id.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
                {
                    enabledNum = this.validateDbObjectQueueRank(_WebDbObject_id, cntn, trx);
                }

                if (enabledNum == 0)
                {
                    this.StartFileProcessQueueEvent(
                                _WebEvent_id,
                                _WebQueueEntryType_id,
                                _WebQueueMediaType_id,
                                _WebEventParameters_Txt,
                                "",
                                "",
                                targetFilename,
                                _WorkingDir_Name,
                                resFileUri,
                                _WebDbObject_id,
                                _status,
                                cntn,
                                trx
                        );

                    dbObjPrecedents = this.objectPrecedent(_WebEvent_id, cntn, trx, eaction);

                    if (dbObjPrecedents != null)
                    {
                        this.objectQueuePrecedent(
                                        dbObjPrecedents,
                                        cntn,
                                        trx
                            );
                    }

                    submit_response = "El archivo ha ingresado a la cola de procesos. Recibira el resultado via email. ";
                    submit_response += " Podra ver el estado del mismo, a traves de la ventana de control de cargas.";
                }
                else
                {
                    submit_response = "El archivo no ha ingresado a la cola de procesos en estos momentos por estar";
                    submit_response += " en un rango horario de restriccion";
                }

                trx.Commit();
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }

                submit_response = ex.Message;
            }
            finally
            {
                if (trx != null)
                {
                    trx.Dispose();
                }
                if (cntn != null)
                {
                    cntn.Close();
                }
            }

            JObject res_ret = new JObject(); // Objeto a retornar con resultado de lectura.
            JObject mdRs = new JObject(); //++Armo metadata del result set.

            mdRs.Add(new JProperty("SubmitResponse", submit_response));
            mdRs.Add(new JProperty("WebEvent_id", _WebEvent_id));
            res_ret.Add("ResultSetMetaData", mdRs);

            return JsonConvert.SerializeObject(res_ret, Newtonsoft.Json.Formatting.Indented);
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LockTableForQueue(TdConnection cntn, TdTransaction trx)
        {
            String qry_txt = @"LOCKING TABLE {0}.WebFileProcessQueueLockAccess FOR EXCLUSIVE";

            String schema_name = Parametros().strDbSchema;
            TdCommand cmd = new TdCommand(String.Format(qry_txt, schema_name), cntn, trx);
            String cmdTimeOutStr = Parametros().strTimeOut;
            int cmdTimeOut;

            if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
            {
                cmd.CommandTimeout = cmdTimeOut;
            }

            int rows_affected = cmd.ExecuteNonQuery();

            cmd.Dispose();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public String UpdateMasters(JObject param)
        {
            CfDbTable tabInfo = this.tableInfo();
            Dictionary<String, List<String>> dicQryTxt = new Dictionary<String, List<String>>();
            JObject joFilter = new JObject();
            String strResponseMsg;
            TdConnection cntn = new TdConnection(Parametros().strTeradata);

            cntn.Open();
            TdTransaction trx = cntn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                strResponseMsg = this.SubmitProcess(joFilter, dicQryTxt, EntryAction.MASTERS);
            }
            catch (Exception)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }
                throw;
            }
            finally
            {
                if (trx != null)
                {
                    trx.Dispose();
                }

                cntn.Close();
            }

            return strResponseMsg;
        }
    }
}
