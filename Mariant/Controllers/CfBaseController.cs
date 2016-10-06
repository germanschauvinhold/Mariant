using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebAsistida.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Teradata.Client.Provider;


namespace WebAsistida.lib
{

    abstract public class CfBaseController : CfBaseRootController
    {
        static readonly String strDelimiter = "\\\"";
        static readonly String propertySeparator = ",";
        static readonly String arrStartDelimiter = "[";
        static readonly String arrEndDelimiter = "]";
        ////////////////////////////////////////////////////////////////////////////////
        public enum BatchUpdateOption { UPSERT, DELETE};
        ////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Retorna el informacion de la tabla que maneja este controller.
        /// </summary>
        /// <returns></returns>
        abstract public CfDbTable tableInfo();
        ////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Para controllers de una tabla, el grupo es la misma tabla.
        /// </summary>
        /// <returns></returns>
        override public CfDbTableGroup tableInfoGroup()
        {
            CfDbTable[] Tables = new CfDbTable[1];
            Tables[0] = tableInfo();

            bool[] TableFilterFlag = new bool[1];
            TableFilterFlag[0] = false;

            String[] TableColFilter = new String[1];

            CfDbTableGroup tabInfoGrp = new CfDbTableGroup(Tables, TableFilterFlag, TableColFilter);

            return tabInfoGrp;
        }
        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        int estimateListResponseSize(TdDataReader rdr)
        {
            //Obtengo metadata de la query
            DataTable schemaTable = rdr.GetSchemaTable();
            Int32[] ColumnSize = new Int32[rdr.FieldCount];
            Int32[] NumericPrecision = new Int32[rdr.FieldCount];
            Int32[] NumericScale = new Int32[rdr.FieldCount];

            Int32 rowSize = 0;
            rowSize += arrStartDelimiter.Length;
            for (int idx = 0; idx < rdr.FieldCount; idx++)
            {
                ColumnSize[idx] = (Int32)schemaTable.Rows[idx]["ColumnSize"];
                NumericPrecision[idx] = (Int16)schemaTable.Rows[idx]["NumericPrecision"];
                NumericScale[idx] = (Int16)schemaTable.Rows[idx]["NumericScale"];

                if (NumericPrecision[idx] == 255 && NumericScale[idx] == 255)
                {
                    rowSize += ColumnSize[idx] + (2 * strDelimiter.Length) + propertySeparator.Length;
                }
                else
                {
                    rowSize += NumericPrecision[idx] + (2 * strDelimiter.Length) + propertySeparator.Length + 2 /* signo y punto decimal */;
                }
            }
            rowSize += arrEndDelimiter.Length;

            Int32 setSize = rowSize * rdr.RecordsReturned * 2;

            return setSize;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected HttpResponseMessage ListBase(JObject param, String qry_txt)
        {
            String conn_str;
            StringBuilder sbResponse;

            conn_str = Parametros().strTeradata;

            using (Teradata.Client.Provider.TdConnection cntn = new Teradata.Client.Provider.TdConnection(conn_str))
            {
                cntn.Open();

                using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn))
                {
                    String cmdTimeOutStr = Parametros().strTimeOut;
                    int cmdTimeOut;

                    if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                    {
                        cmd.CommandTimeout = cmdTimeOut;
                    }

                    Teradata.Client.Provider.TdDataReader rdr = cmd.ExecuteReader();

                    int respSize = this.estimateListResponseSize(rdr);

                    sbResponse = new StringBuilder(respSize);

                    //++ Start Object
                    sbResponse.Append('"');
                    sbResponse.Append("{");

                    //Comienzo a leer el result set.

                    //++ Start ResultSet
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append("ResultSetRows");
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append(':');
                    sbResponse.Append('[');

                    String recSep = "";
                    int recCnt = 0;
                    int recLimit = 50000;
                    while (recCnt++ < recLimit && rdr.Read())
                    {
                        String fldSep = "";
                        //++ Start fila
                        sbResponse.Append(recSep);
                        sbResponse.Append('[');
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            String value = rdr.IsDBNull(i) ? "" : rdr.GetString(i).ToString();
                            value = value.Replace("\\", "\\\\"); // Escape de slash.
                            value = value.Replace("\"", "\\\\\\\""); // Escape de comillas dobles.
                            value = value.Replace("\n", "\\\\n");//escape de nueva linea
                            sbResponse.Append(fldSep);
                            sbResponse.Append(strDelimiter);
                            sbResponse.Append(value);
                            sbResponse.Append(strDelimiter);
                            fldSep = ",";
                        }
                        sbResponse.Append(']');
                        recSep = ",";
                    }
                    sbResponse.Append(']');

                    sbResponse.Append(',');

                    //++ Start Metadata
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append("ResultSetMetaData");
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append(':');
                    sbResponse.Append('{');

                    //Registros afectados.
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append("RecordsAffected");
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append(':');
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append(rdr.RecordsAffected.ToString());
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append(',');

                    //Registros retornados.
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append("RecordsReturned");
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append(':');
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append(rdr.RecordsReturned.ToString());
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append(',');

                    //++ Start Datos de columnas
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append("Columns");
                    sbResponse.Append(strDelimiter);
                    sbResponse.Append(':');
                    sbResponse.Append('[');
                    String cSep = "";
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        //++ Start Column
                        sbResponse.Append(cSep);

                        sbResponse.Append('{');

                        // Columname
                        sbResponse.Append(strDelimiter);
                        sbResponse.Append("ColumnName");
                        sbResponse.Append(strDelimiter);
                        sbResponse.Append(':');
                        sbResponse.Append(strDelimiter);
                        sbResponse.Append(rdr.GetName(i));
                        sbResponse.Append(strDelimiter);

                        sbResponse.Append(',');

                        //Data type
                        sbResponse.Append(strDelimiter);
                        sbResponse.Append("DataType");
                        sbResponse.Append(strDelimiter);
                        sbResponse.Append(':');
                        sbResponse.Append(strDelimiter);
                        sbResponse.Append(rdr.GetDataTypeName(i));
                        sbResponse.Append(strDelimiter);


                        sbResponse.Append('}');
                        //-- End Column
                        cSep = ",";
                    }
                    sbResponse.Append(']');

                    sbResponse.Append('}');
                    //-- End Metadata.

                    sbResponse.Append("}");
                    sbResponse.Append('"');
                    //-- End Object

                    // Libera recursos de base.
                    rdr.Dispose();
                    rdr.Close();
                    cmd.Dispose();
                }
                // Cierra conexion a la base.
                cntn.Close();
            }

            HttpResponseMessage rspMsg = new HttpResponseMessage(HttpStatusCode.OK);
            rspMsg.Content = new StringContent(sbResponse.ToString());
            rspMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            sbResponse.Clear();//agregado
            return rspMsg;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public virtual HttpResponseMessage List(JObject param)
        {
            CfDbTable tabInfo = this.tableInfo();

            // Tomo string de consulta.
            String qry_txt = tabInfo.listQryText(param, CfDbTable.ListOption.LIST_ALL, CfDbTable.OrderByOption.WITH_ORDER_BY, CfDbTable.LockByOption.WITH_LOCKING, CfDbTable.ColumnListOption.DEFAULT_COLUMNS);

            return ListBase(param, qry_txt);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public HttpResponseMessage ListC(JObject param)
        {
            CfDbTable tabInfo = this.tableInfo();

            // Tomo string de consulta.
            String qry_txt = tabInfo.listQryText(param, CfDbTable.ListOption.LIST_CURRENT, CfDbTable.OrderByOption.WITH_ORDER_BY, CfDbTable.LockByOption.WITH_LOCKING,CfDbTable.ColumnListOption.DEFAULT_COLUMNS);

            return ListBase(param, qry_txt);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public HttpResponseMessage ListCF(JObject param)
        {
            CfDbTable tabInfo = this.tableInfo();

            // Tomo string de consulta.
            String qry_txt = tabInfo.listQryText(param, CfDbTable.ListOption.LIST_CURRENT_AND_FUTURE, CfDbTable.OrderByOption.WITH_ORDER_BY, CfDbTable.LockByOption.WITH_LOCKING, CfDbTable.ColumnListOption.DEFAULT_COLUMNS);

            return ListBase(param, qry_txt);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public virtual HttpResponseMessage Count(JObject param)
        {
            CfDbTable tabInfo = this.tableInfo();

            // Tomo string de consulta.
            String qry_txt = tabInfo.countQryText(param, CfDbTable.ListOption.LIST_ALL, CfDbTable.OrderByOption.WITHOUT_ORDER_BY, CfDbTable.LockByOption.WITHOUT_LOCKING);

            return ListBase(param, qry_txt);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public HttpResponseMessage CountC(JObject param)
        {
            CfDbTable tabInfo = this.tableInfo();

            // Tomo string de consulta.
            String qry_txt = tabInfo.countQryText(param, CfDbTable.ListOption.LIST_CURRENT, CfDbTable.OrderByOption.WITHOUT_ORDER_BY, CfDbTable.LockByOption.WITHOUT_LOCKING);

            return ListBase(param, qry_txt);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public HttpResponseMessage CountCF(JObject param)
        {
            CfDbTable tabInfo = this.tableInfo();

            // Tomo string de consulta.
            String qry_txt = tabInfo.countQryText(param, CfDbTable.ListOption.LIST_CURRENT_AND_FUTURE, CfDbTable.OrderByOption.WITHOUT_ORDER_BY, CfDbTable.LockByOption.WITHOUT_LOCKING);

            return ListBase(param, qry_txt);
        }
        ////////////////////////////////////////////////////////////////////////////////
        // POST api/base/upsert
        [HttpPost]
        [AuthorizeActivity]
        public virtual String Upsert(JObject param)
        {
            int rows_affected;
            CfDbTable tabInfo = tableInfo();

            if (tabInfo.isReadOnly)
            {
                throw new Exception("Invalid operation on a readonly table.");
            }

            // Completa datos de tracking, si la tabla tiene esos campos.
            setEventProperty(param, tabInfo.TrkEvent_Column, "WebEvent_id");
            setEventProperty(param, tabInfo.TrkUser_Column, "WebUser_id");

            String qry_txt = "";
            String[] paramNames;
            String upsert_response = "";
            String conn_str = Parametros().strTeradata;
            JObject res_ret = new JObject(); // Objeto a retornar con resultado de lectura.
            String _WebDbObject_id = this.ToString().Split('.')[2];
            String _WebEvent_id = Request.Properties.ContainsKey("WebEvent_id") ? Request.Properties["WebEvent_id"].ToString() : "";
            String _WebUser_id = Request.Properties.ContainsKey("WebUser_id") ? Request.Properties["WebUser_id"].ToString() : "nicolas_oliveto";

            TdConnection cntn = new Teradata.Client.Provider.TdConnection(conn_str);

            cntn.Open();

            //Controlo foreign keys.
            this.checkParentRI(tabInfo, param, cntn);

            // Tomo string de consulta.
            if (tabInfo.isHist)
            {
                qry_txt = tabInfo.mergeHistQryText(out paramNames);
            }
            else
            {
                qry_txt = tabInfo.mergeQryText(out paramNames);
            }

            int updateFlag = validateDbObjectProcessRank(_WebDbObject_id, _WebUser_id, _WebEvent_id, cntn);

            if (updateFlag == 0)
            {
                TdTransaction trx = cntn.BeginTransaction(IsolationLevel.Serializable);
                TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx);

                try
                {
                    //Asigna Parametros
                    foreach (String pName in paramNames)
                    {
                        JProperty t = param.Property(pName);

                        if (t == null)
                        {
                            throw new Exception("Falta informar campo '" + pName + "'.");
                        }
                        TdParameter tp = new TdParameter(t.Name, System.Data.DbType.String);

                        tp.Value = t.Value.ToString();
                        cmd.Parameters.Add(tp);
                    }

                    String cmdTimeOutStr = Parametros().strTimeOut;

                    int cmdTimeOut;

                    if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                    {
                        cmd.CommandTimeout = cmdTimeOut;
                    }

                    rows_affected = cmd.ExecuteNonQuery();

                    //Procesa pace followers.
                    if (tabInfo.paceFollowerTabs != null)
                    {
                        foreach (CfDbTable tbl in tabInfo.paceFollowerTabs)
                        {
                            rows_affected += splitOverlappingPaceFollower(param, tbl, cntn, trx);
                        }
                    }

                    // Post upsert - Especial por motivo de currentselection
                    this.AfterUpsert(tabInfo, param, cntn, trx);

                    upsert_response = "";
                    trx.Commit();
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
                    //Libera recursos de base de datos.
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                    if (trx != null)
                    {
                        trx.Dispose();
                    }
                }

                //Post Upsert dispara un proceso.
                this.AfterUpsertProcess(tabInfo, param, cntn);
            }
            else
            {
                rows_affected = 0;
                upsert_response = "No se pueden actualizar o borrar registros en estos momentos por estar";
                upsert_response += " en un rango horario de restriccion";
            }

            //Cierra conexion a la base.
            cntn.Dispose();
            cntn.Close();

            //++Armo metadata del result set.
            JObject mdRs = new JObject();

            mdRs.Add(new JProperty("RecordsAffected", rows_affected.ToString()));
            mdRs.Add(new JProperty("SubmitResponse", upsert_response));

            res_ret.Add("ResultSetMetaData", mdRs);
            return JsonConvert.SerializeObject(res_ret, Newtonsoft.Json.Formatting.Indented);
        }
        ////////////////////////////////////////////////////////////////////////////////
        // POST api/base/batch_upsert
        public virtual String Batch_Base(JArray batch_param, BatchUpdateOption batch_option)
        {
            int rows_affected = 0;
            CfDbTable tabInfo = tableInfo();

            if (tabInfo.isReadOnly)
            {
                throw new Exception("Invalid operation on a readonly table.");
            }

            String qry_txt = "";
            String[] paramNames;
            String batchupsert_response = "";
            String _WebDbObject_id = this.ToString().Split('.')[2];
            String _WebEvent_id = Request.Properties.ContainsKey("WebEvent_id") ? Request.Properties["WebEvent_id"].ToString() : "";
            String _WebUser_id = Request.Properties.ContainsKey("WebUser_id") ? Request.Properties["WebUser_id"].ToString() : "nicolas_oliveto";

            String conn_str = Parametros().strTeradata;

            JObject res_ret = new JObject(); // Objeto a retornar con resultado de lectura.
            TdConnection cntn = new Teradata.Client.Provider.TdConnection(conn_str);

            cntn.Open();

            // Controlo foreign keys.
            foreach (JObject param in batch_param)
            {
                this.checkParentRI(tabInfo, param, cntn);
            }

            // Tomo string de consulta.
            if (tabInfo.isHist)
            {
                qry_txt = tabInfo.mergeHistQryText(out paramNames);
            }
            else
            {
                qry_txt = tabInfo.mergeQryText(out paramNames);
            }

            int batchupdateFlag = validateDbObjectProcessRank(_WebDbObject_id, _WebUser_id, _WebEvent_id, cntn);

            if (batchupdateFlag == 0)
            {
                TdTransaction trx = cntn.BeginTransaction(IsolationLevel.Serializable);
                TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx);

                try
                {
                    if (batch_option == BatchUpdateOption.DELETE || batch_option == BatchUpdateOption.UPSERT)
                    {
                        if (tabInfo.paceFollowingTab != null)
                        {
                            CloseHist(batch_param, cntn, trx);
                        }
                    }

                    if (batch_option == BatchUpdateOption.UPSERT)
                    {
                        //Crea parametros.
                        foreach (String pName in paramNames)
                        {
                            TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                            cmd.Parameters.Add(tp);
                        }

                        //Inserta cada elemento del array.
                        foreach (JObject param in batch_param)
                        {
                            // Completa datos de tracking, si la tabla tiene esos campos.
                            setEventProperty(param, tabInfo.TrkEvent_Column, "WebEvent_id");
                            setEventProperty(param, tabInfo.TrkUser_Column, "WebUser_id");

                            //Asigna parametros.
                            foreach (TdParameter tp in cmd.Parameters)
                            {
                                JProperty t = param.Property(tp.ParameterName);
                                if (t == null)
                                {
                                    throw new Exception("Falta informar campo '" + tp.ParameterName + "'.");
                                }
                                tp.Value = t.Value.ToString();
                            }

                            String cmdTimeOutStr = Parametros().strTimeOut;
                            int cmdTimeOut;

                            if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                            {
                                cmd.CommandTimeout = cmdTimeOut;
                            }

                            rows_affected += cmd.ExecuteNonQuery();
                        }
                    }

                    //Obtengo el primer elemento para actualizar al pacefollowing.
                    JObject paramf = (JObject)batch_param[0];

                    if (tabInfo.paceFollowingTab != null)
                    {
                        CfDbTable tbl = tabInfo.paceFollowingTab;
                        rows_affected += splitOverlappingPaceFollower(paramf, tbl, cntn, trx);
                    }

                    batchupsert_response = "";

                    trx.Commit();
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
                    //Libera recursos de base de datos.
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                    if (trx != null)
                    {
                        trx.Dispose();
                    }
                }
            }
            else
            {
                rows_affected = 0;
                batchupsert_response = "No se pueden actualizar o borrar registros en estos momentos por estar";
                batchupsert_response += " en un rango horario de restriccion";
            }

            //Cierra conexion a la base.
            cntn.Dispose();
            cntn.Close();

            //++Armo metadata del result set.
            JObject mdRs = new JObject();

            mdRs.Add(new JProperty("RecordsAffected", rows_affected.ToString()));
            mdRs.Add(new JProperty("SubmitResponse", batchupsert_response));

            res_ret.Add("ResultSetMetaData", mdRs);
            return JsonConvert.SerializeObject(res_ret, Newtonsoft.Json.Formatting.Indented);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public virtual String Batch_Upsert(JArray batch_param)
        {
            return Batch_Base(batch_param, BatchUpdateOption.UPSERT);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public String Batch_Delete(JArray batch_param)
        {
            return Batch_Base(batch_param, BatchUpdateOption.DELETE);
        }
        ////////////////////////////////////////////////////////////////////////////////
        public void CloseHist(JArray batch_param, TdConnection cntn, TdTransaction trx)
        {
            int rows_affected = 0;
            CfDbTable tabInfo = tableInfo();
            String[] paramNames_upd;
            
            String qry_txt_upd = tabInfo.CloseHistQryText(out paramNames_upd);

            TdCommand cmd_upd = new Teradata.Client.Provider.TdCommand(qry_txt_upd, cntn, trx);

            //Crea parametros.
            foreach (String pName in paramNames_upd)
            {
                TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                cmd_upd.Parameters.Add(tp);
            }

            //Inserta cada elemento del array.
            foreach (JObject param in batch_param)
            {
                // Completa datos de tracking, si la tabla tiene esos campos.
                setEventProperty(param, tabInfo.TrkEvent_Column, "WebEvent_id");
                setEventProperty(param, tabInfo.TrkUser_Column, "WebUser_id");

                //Asigna parametros.
                foreach (TdParameter tp in cmd_upd.Parameters)
                {
                    JProperty t = param.Property(tp.ParameterName);
                    if (t == null)
                    {
                        throw new Exception("Falta informar campo '" + tp.ParameterName + "'.");
                    }
                    tp.Value = t.Value.ToString();
                }

                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd_upd.CommandTimeout = cmdTimeOut;
                }

                rows_affected += cmd_upd.ExecuteNonQuery();

                break; //se rompe el ciclo porque solo se necesita una sola ejecucion, quizas lo mejor sería tomar solo el primer elemento del array de jobject
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void AfterUpsert(CfDbTable tabInfo, JObject param, TdConnection cntn, TdTransaction trx)
        {
            int rows_affected = 0;

            if (tabInfo.AfterUpsertQryTxt != null)
            {
                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                foreach (CfDbQryTxtParamFunc qryTxtFunc in tabInfo.AfterUpsertQryTxt)
                {
                    String after_qry_txt = qryTxtFunc(param);
                    TdCommand after_cmd = new TdCommand(after_qry_txt, cntn, trx);

                    if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                    {
                        after_cmd.CommandTimeout = cmdTimeOut;
                    }

                    rows_affected += after_cmd.ExecuteNonQuery();
                    after_cmd.Dispose();
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void AfterUpsertProcess(CfDbTable tabInfo, JObject param, TdConnection cntn)
        {
            Dictionary<String, List<String>> dicQryTxt = new Dictionary<String, List<String>>();
            int rec_cnt = 0;

            if (tabInfo.AfterUpsertDeleteValidateProcessQryTxt != null)
            {
                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;
                String currentTime = DateTime.Now.ToString("HH:mm:ss");

                CfDbQryTextValidateProcessFunc qrytxtValidateProcessFunc = tabInfo.AfterUpsertDeleteValidateProcessQryTxt;
                String qry_txt = qrytxtValidateProcessFunc(currentTime);

                TdCommand cmd = new TdCommand(qry_txt, cntn);

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                TdDataReader rdr = cmd.ExecuteReader();
                rec_cnt = rdr.RecordsReturned;

                rdr.Dispose();
                rdr.Close();
                cmd.Dispose();
            }

            if (rec_cnt == 0)
            {
                if (tabInfo.AfterUpsertProcessQryTxt != null)
                {
                    CfDbQryTxtProcessParamFunc qrytxtProcessFunc = tabInfo.AfterUpsertProcessQryTxt;

                    List<String> SQLSteps = qrytxtProcessFunc(param);
                    dicQryTxt.Add(tabInfo.TbLocalName, SQLSteps);

                    if (dicQryTxt.Count == 0)
                    {
                        throw new Exception("No se han podido cargar los pasos necesarios para la ejecucion del proceso.");
                    }

                    this.SubmitProcess(param, dicQryTxt, EntryAction.PROCESS);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void AfterDeleteProcess(CfDbTable tabInfo, JObject param, TdConnection cntn)
        {
            Dictionary<String, List<String>> dicQryTxt = new Dictionary<String, List<String>>();
            int rec_cnt = 0;

            if (tabInfo.AfterUpsertDeleteValidateProcessQryTxt != null)
            {
                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;
                String currentTime = DateTime.Now.ToString("HH:mm:ss");

                CfDbQryTextValidateProcessFunc qrytxtValidateProcessFunc = tabInfo.AfterUpsertDeleteValidateProcessQryTxt;
                String qry_txt = qrytxtValidateProcessFunc(currentTime);

                TdCommand cmd = new TdCommand(qry_txt, cntn);

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                TdDataReader rdr = cmd.ExecuteReader();
                rec_cnt = rdr.RecordsReturned;

                rdr.Dispose();
                rdr.Close();
                cmd.Dispose();
            }

            if (rec_cnt == 0)
            {
                if (tabInfo.AfterDeleteProcessQryTxt != null)
                {
                    CfDbQryTxtProcessParamFunc qrytxtProcessFunc = tabInfo.AfterDeleteProcessQryTxt;

                    List<String> SQLSteps = qrytxtProcessFunc(param);
                    dicQryTxt.Add(tabInfo.TbLocalName, SQLSteps);

                    if (dicQryTxt.Count == 0)
                    {
                        throw new Exception("No se han podido cargar los pasos necesarios para la ejecucion del proceso.");
                    }

                    this.SubmitProcess(param, dicQryTxt, EntryAction.PROCESS);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        // POST api/base/ExclusiveUpsert
        // Metodo particular utilizado unicamente para actualizar la tabla de cola
        [HttpPost]
        [AuthorizeActivity]
        public String ExclusiveUpsert(JObject param)
        {
            int rows_affected;
            CfDbTable tabInfo = tableInfo();
            JObject mdRs = new JObject();
            JObject res_ret = new JObject(); // Objeto a retornar con resultado de lectura.
            String conn_str = Parametros().strTeradataLoad;
            String qry_txt;
            String strWebDbObject;
            TdConnection cntn = new Teradata.Client.Provider.TdConnection(conn_str);

            if (tabInfo.isReadOnly)
            {
                throw new Exception("Invalid operation on a readonly table.");
            }

            if(param.Property("WebDbObject_id") == null)
            {
                throw new Exception("Falta informar Objeto");
            }
            strWebDbObject = param.Property("WebDbObject_id").Value.ToString();

            cntn.Open();

            //Controlo foreign keys.
            this.checkParentRI(tabInfo, param, cntn);

            // No se toma string de consulta por ser un metodo particular.
            String updWebFileQueueProcess = @"
LOCKING TABLE {CDW}.WebEvent FOR ACCESS
LOCKING TABLE {CDW}.WebSession FOR ACCESS
LOCKING TABLE {CDW}.WebUser FOR ACCESS
LOCKING TABLE {CDW}.WebFileProcessStatus FOR ACCESS
UPDATE __tbl
FROM
{CDW}.WebFileProcessQueue __tbl,
(
SELECT
tmp1.WebEvent_id
,tmp1.WebFileProcessStatus_id
FROM
(
SELECT
CAST ( '{WebEvent_id}' AS VARCHAR(50)) WebEvent_id
,CAST ( '{WebFileProcessStatus_id}' AS VARCHAR(50)) WebFileProcessStatus_id
) tmp1
INNER JOIN
(
SELECT
rfpq.WebEvent_id
FROM
{CDW}.WebFileProcessQueue rfpq
INNER JOIN
{CDW}.WebEvent re
ON
re.WebEvent_id = rfpq.WebEvent_id
INNER JOIN
{CDW}.WebSession rs
ON
rs.WebSession_id = re.WebSession_id
INNER JOIN
{CDW}.WebUser ru
ON
ru.WebUser_id = rs.WebUser_id
WHERE
WebFileProcessStatus_id IN (
SELECT
WebFileProcessStatus_id
FROM
{CDW}.WebFileProcessStatus
WHERE
AcceptChange_Ind = 'Y' )
AND ru.WebUser_id <> 'admin'
) tmp2
ON
tmp2.WebEvent_id = tmp1.WebEvent_id
) __upd
SET
WebFileProcessStatus_id = __upd.WebFileProcessStatus_id
WHERE
__tbl.WebEvent_id = __upd.WebEvent_id;";


            if (param.Property("WebEvent_id") == null)
            {
                throw new Exception("Falta informar campo 'WebEvent_id'.");
            }
            JProperty WebEvent = param.Property("WebEvent_id");

            if (param.Property("WebFileProcessStatus_id") == null)
            {
                throw new Exception("Falta informar campo 'WebFileProcessStatus_id'.");
            }
            JProperty WebFileProcessStatus = param.Property("WebFileProcessStatus_id");

            object fmtData = new
            {
                CDW = tabInfo.DbName,
                WebEvent_id = WebEvent.Value.ToString(),
                WebFileProcessStatus_id = WebFileProcessStatus.Value.ToString(),
            };

            qry_txt = StringTools.Format(updWebFileQueueProcess, fmtData);

            TdTransaction trx = cntn.BeginTransaction(IsolationLevel.Serializable);
            TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx);

            try
            {
                this.LockTableFileProcessQueueExclusive(cntn, trx);

                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                rows_affected = cmd.ExecuteNonQuery();

                if (strWebDbObject.Equals("WebDailySheetDESCController", StringComparison.CurrentCultureIgnoreCase)
                        || strWebDbObject.Equals("WebDailySheetSTDController", StringComparison.CurrentCultureIgnoreCase)
                        || strWebDbObject.Equals("WebDailySheetXDController", StringComparison.CurrentCultureIgnoreCase))
                {
                    this.AfterUpsert(tabInfo, param, cntn, trx);
                }

                trx.Commit();
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
                //Libera recursos de base de datos.
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (trx != null)
                {
                    trx.Dispose();
                }
            }

            //Cierra conexion a la base.
            cntn.Dispose();
            cntn.Close();

            //++Armo metadata del result set.
            mdRs.Add(new JProperty("RecordsAffected", rows_affected.ToString()));

            res_ret.Add("ResultSetMetaData", mdRs);
            return JsonConvert.SerializeObject(res_ret, Newtonsoft.Json.Formatting.Indented);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void LockTableFileProcessQueueExclusive(TdConnection cntn, TdTransaction trx)
        {
            String strDbSchema = Parametros().strDbSchema;

            String strQryTxt = @"
LOCKING TABLE {0}.WebFileProcessQueueLockAccess FOR EXCLUSIVE";

            TdCommand cmd = new TdCommand(String.Format(strQryTxt, strDbSchema), cntn, trx);
            String cmdTimeOutStr = Parametros().strTimeOut;
            int cmdTimeOut;

            if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
            {
                cmd.CommandTimeout = cmdTimeOut;
            }

            int rows_affected = cmd.ExecuteNonQuery();

            cmd.Dispose();
        }
        ////////////////////////////////////////////////////////////////////////////////
        // DELETE api/delete/delete
        [HttpPost]
        [AuthorizeActivity]
        public virtual String Delete(JObject param)
        {
            int rows_affected = 0;
            CfDbTable tabInfo = tableInfo();

            if (tabInfo.isReadOnly)
            {
                throw new Exception("Invalid operation on a readonly table.");
            }

            // Completa datos de tracking, si la tabla tiene esos campos.
            setEventProperty(param, tabInfo.TrkEvent_Column, "WebEvent_id");
            setEventProperty(param, tabInfo.TrkUser_Column, "WebUser_id");

            String qry_txt = "";
            String[] paramNames;
            String delete_response = "";

            String conn_str = Parametros().strTeradata;
            JObject res_ret = new JObject(); // Objeto a retornar con resultado de lectura.
            String _WebDbObject_id = this.ToString().Split('.')[2];
            String _WebEvent_id = Request.Properties.ContainsKey("WebEvent_id") ? Request.Properties["WebEvent_id"].ToString() : "";
            String _WebUser_id = Request.Properties.ContainsKey("WebUser_id") ? Request.Properties["WebUser_id"].ToString() : "nicolas_oliveto";

            TdConnection cntn = new Teradata.Client.Provider.TdConnection(conn_str);

            cntn.Open();

            //Controlo foreign keys
            this.checkChildRI(tabInfo, param, cntn);

            // Tomo string de consulta.
            if (tabInfo.isHist)
            {
                qry_txt = tabInfo.deleteHistQryText(out paramNames);
            }
            else
            {
                qry_txt = tabInfo.deleteQryText(out paramNames);
            }

            int deleteFlag = validateDbObjectProcessRank(_WebDbObject_id, _WebUser_id, _WebEvent_id, cntn);

            if (deleteFlag == 0)
            {
                TdTransaction trx = cntn.BeginTransaction(IsolationLevel.Serializable);
                TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx);

                try
                {
                    //String[] debugParamValues = new String[50];
                    //int dpvi = 0;
                    //Asigna parametros.
                    foreach (String pName in paramNames)
                    {
                        JProperty t = param.Property(pName);
                        TdParameter tp = new TdParameter(t.Name, System.Data.DbType.String);
                        tp.Value = t.Value.ToString();
                        cmd.Parameters.Add(tp);
                        //debugParamValues[dpvi++] = t.Value.ToString();
                    }

                    String cmdTimeOutStr = Parametros().strTimeOut;
                    int cmdTimeOut;

                    if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                    {
                        cmd.CommandTimeout = cmdTimeOut;
                    }

                    rows_affected = cmd.ExecuteNonQuery();

                    // Procesa borrado para tablas de excepcion cuando la tabla es normal.
                    if (tabInfo.ExceptionTable_Ind.Equals("N", StringComparison.CurrentCultureIgnoreCase)
                        && tabInfo.ExceptionTab != null)
                    {
                        rows_affected += this.DeleteExceptionTab(param, tabInfo.ExceptionTab, cntn, trx);
                    }

                    //Procesa pace followers.
                    if (tabInfo.paceFollowerTabs != null)
                    {
                        foreach (CfDbTable tbl in tabInfo.paceFollowerTabs)
                        {
                            rows_affected += deleteOverlappingPaceFollower(param, tbl, cntn, trx);
                        }
                    }

                    delete_response = "";

                    trx.Commit();
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
                    //Libera recursos de base de datos.
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                    if (trx != null)
                    {
                        trx.Dispose();
                    }
                }

                //Post After Delete ejecuta un process
                this.AfterDeleteProcess(tabInfo, param, cntn);
            }
            else
            {
                rows_affected = 0;
                delete_response = "No se pueden actualizar o borrar registros en estos momentos por estar";
                delete_response += " en un rango horario de restriccion";
            }

            cntn.Dispose();
            cntn.Close();

            //++Armo metadata del result set.
            JObject mdRs = new JObject();

            mdRs.Add(new JProperty("RecordsAffected", rows_affected.ToString()));
            mdRs.Add(new JProperty("SubmitResponse", delete_response));

            res_ret.Add("ResultSetMetaData", mdRs);
            return JsonConvert.SerializeObject(res_ret, Newtonsoft.Json.Formatting.Indented);
        }
        ////////////////////////////////////////////////////////////////////////////////
        // DELETE api/delete/delete
        [HttpPost]
        [AuthorizeActivity]
        public virtual String Conditional_Delete(JObject param)
        {
            int rows_affected = 0;
            CfDbTable tabInfo = tableInfo();

            if (tabInfo.isReadOnly)
            {
                throw new Exception("Invalid operation on a readonly table.");
            }

            // Completa datos de tracking, si la tabla tiene esos campos.
            setEventProperty(param, tabInfo.TrkEvent_Column, "WebEvent_id");
            setEventProperty(param, tabInfo.TrkUser_Column, "WebUser_id");

            String conn_str = Parametros().strTeradataLoad;

            String condDelete_response = "";
            JObject res_ret = new JObject(); // Objeto a retornar con resultado de lectura.
            String _WebDbObject_id = this.ToString().Split('.')[2];
            String _WebEvent_id = Request.Properties.ContainsKey("WebEvent_id") ? Request.Properties["WebEvent_id"].ToString() : "";
            String _WebUser_id = Request.Properties.ContainsKey("WebUser_id") ? Request.Properties["WebUser_id"].ToString() : "nicolas_oliveto";

            using (Teradata.Client.Provider.TdConnection cntn = new Teradata.Client.Provider.TdConnection(conn_str))
            {
                cntn.Open();

                int condDeleteFlag = validateDbObjectProcessRank(_WebDbObject_id, _WebUser_id, _WebEvent_id, cntn);

                if (condDeleteFlag == 0)
                {
                    rows_affected = deleteByCondition(param, tabInfo, cntn);

                    // Procesa borrado para tablas de excepcion cuando la tabla es normal.
                    if (tabInfo.ExceptionTable_Ind.Equals("N", StringComparison.CurrentCultureIgnoreCase)
                        && tabInfo.ExceptionTab != null)
                    {
                        rows_affected += deleteByCondition(param, tabInfo.ExceptionTab, cntn);
                    }

                    condDelete_response = "";

                    //Post After Delete ejecuta un process
                    this.AfterDeleteProcess(tabInfo, param, cntn);
                }
                else
                {
                    rows_affected = 0;
                    condDelete_response = "No se pueden actualizar o borrar registros en estos momentos por estar";
                    condDelete_response += " en un rango horario de restriccion";
                }

                cntn.Close();
            }
            //++Armo metadata del result set.
            JObject mdRs = new JObject();

            mdRs.Add(new JProperty("RecordsAffected", rows_affected.ToString()));
            mdRs.Add(new JProperty("SubmitResponse", condDelete_response));

            res_ret.Add("ResultSetMetaData", mdRs);
            return JsonConvert.SerializeObject(res_ret, Newtonsoft.Json.Formatting.Indented);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected int DeleteExceptionTab(JObject param, CfDbTable tabInfoExc, TdConnection cntn, TdTransaction trx)
        {
            int rows_affected = 0;

            if (tabInfoExc.isReadOnly)
            {
                throw new Exception("Invalid operation on a readonly table.");
            }

            String qry_txt = "";
            String[] paramNames;

            JObject res_ret = new JObject(); // Objeto a retornar con resultado de lectura.

            //Controlo foreign keys
            this.checkChildRI(tabInfoExc, param, cntn);

            // Tomo string de consulta.
            if (tabInfoExc.isHist)
            {
                qry_txt = tabInfoExc.deleteHistQryText(out paramNames);
            }
            else
            {
                qry_txt = tabInfoExc.deleteQryText(out paramNames);
            }

            using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx))
            {
                foreach (String pName in paramNames)
                {
                    JProperty t = param.Property(pName);
                    TdParameter tp = new TdParameter(t.Name, System.Data.DbType.String);
                    tp.Value = t.Value.ToString();
                    cmd.Parameters.Add(tp);
                }

                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                rows_affected = cmd.ExecuteNonQuery();

                //Libera recursos de la base.
                cmd.Dispose();

                //Procesa pace followers.
                if (tabInfoExc.paceFollowerTabs != null)
                {
                    foreach (CfDbTable tbl in tabInfoExc.paceFollowerTabs)
                    {
                        rows_affected += deleteOverlappingPaceFollower(param, tbl, cntn, trx);
                    }
                }
            }

            return rows_affected;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void GetProductEventFilterTbName(out String DbName, out String TbName)
        {
            DbName = Parametros().strDbSchema;
            TbName = "AuxiliarProductEventFilter";
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected int validateDbObjectProcessRank(String _WebDbObject, String _WebUser_id, String _WebEvent_id,
                TdConnection cntn
            )
        {
            int rec_cnt = 0;
            String schema_name = Parametros().strDbSchema;

            if (!_WebUser_id.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
            {
                String currentTime = DateTime.Now.ToString("HH:mm:ss");
                String qry_txt = @"
LOCKING TABLE " + schema_name + @".WebDbObjectProcess FOR ACCESS
SELECT
    ProcessEnabled_Ind
FROM
" + schema_name + @".WebDbObjectProcess
WHERE
    WebDbObject_id = '" + _WebDbObject + @"'
AND CURRENT_DATE Between EffectiveDate and ExpirationDate
AND CAST('" + currentTime + @"' AS TIME(0)) Between EffectiveTime and ExpirationTime
AND ProcessEnabled_Ind IN ('N')
;";
                TdCommand cmd = new TdCommand(qry_txt, cntn);
                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                TdDataReader rdr = cmd.ExecuteReader();

                rec_cnt = rdr.RecordsReturned;

                rdr.Dispose();
                rdr.Close();
                cmd.Dispose();
            }

            return rec_cnt;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public String existingProcessUserEvent(JObject param)
        {
            String _WebEvent_id = Request.Properties.ContainsKey("WebEvent_id") ? Request.Properties["WebEvent_id"].ToString() : "";
            String _WebUser_id = Request.Properties["WebUser_id"].ToString();
            String submit_response;
            String controller = this.ToString().Split('.')[2];
            String conn_str = Parametros().strTeradata;
            CfDbTable tabInfo = this.tableInfo();


            String qry_txt = @"
LOCKING TABLE {0}.WebFileProcessQueue FOR ACCESS
LOCKING TABLE {0}.WebEvent FOR ACCESS
LOCKING TABLE {0}.WebSession FOR ACCESS
LOCKING TABLE {0}.WebUser FOR ACCESS
SELECT
rfpq.WebEvent_id
FROM
{0}.WebFileProcessQueue rfpq
INNER JOIN
{0}.WebEvent re
ON
re.WebEvent_id = rfpq.WebEvent_id
INNER JOIN
{0}.WebSession rs
ON
rs.WebSession_id = re.WebSession_id
INNER JOIN
{0}.WebUser ru
ON
ru.WebUser_id = rs.WebUser_id
WHERE
    ru.WebUser_id = '" + _WebUser_id + @"'
AND rfpq.WebDbObject_id = '" + controller + @"'
AND rfpq.WebFileProcessStatus_id NOT IN ('Finalizado','Cancelado','Finalizado con error','Finalizado con alerta','Cancelado Automatico')
;";

            TdConnection cntn = new TdConnection(conn_str);

            cntn.Open();

            TdCommand cmd = new TdCommand(String.Format(qry_txt, tabInfo.DbName), cntn);
            String cmdTimeOutStr = Parametros().strTimeOut;
            int cmdTimeOut;

            if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
            {
                cmd.CommandTimeout = cmdTimeOut;
            }

            TdDataReader rdr = cmd.ExecuteReader();

            if (rdr.RecordsReturned != 0)
            {
                submit_response = "Ud. ya esta ejecutando un proceso relacionado con esta carga.";
            }
            else
            {
                submit_response = "Disponible";
            }

            rdr.Dispose();
            rdr.Close();
            cmd.Dispose();

            JObject res_ret = new JObject(); // Objeto a retornar con resultado de lectura.
            JObject mdRs = new JObject(); //++Armo metadata del result set.

            mdRs.Add(new JProperty("SubmitResponse", submit_response));
            mdRs.Add(new JProperty("WebEvent_id", _WebEvent_id));
            res_ret.Add("ResultSetMetaData", mdRs);

            return JsonConvert.SerializeObject(res_ret, Newtonsoft.Json.Formatting.Indented);
        }
        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////
    }
}

