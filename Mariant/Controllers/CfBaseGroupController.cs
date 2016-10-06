using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebAsistida.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Teradata.Client.Provider;

namespace WebAsistida.lib
{
    abstract public class CfBaseGroupController : CfBaseRootController
    {
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public HttpResponseMessage Count(JObject param)
        {
            CfDbTable[] tabInfoGrp = this.tableInfoGroup().Tables;
            Dictionary<String, String> tablesQryTxt = new Dictionary<String, String>();

            int tabIdx = 0;
            CfDbTable tabInfoFilter = tabInfoGrp[0];
            String qry_text_filter = tabInfoFilter.listQryText(param, CfDbTable.ListOption.LIST_ALL, CfDbTable.OrderByOption.WITHOUT_ORDER_BY, CfDbTable.LockByOption.WITHOUT_LOCKING, CfDbTable.ColumnListOption.DEFAULT_COLUMNS);
            foreach (CfDbTable tabInfo in tabInfoGrp)
            {
                String qry_txt = this.qryTextBuilder(tabInfo, param, CfDbTable.ListOption.LIST_ALL, tabIdx, qry_text_filter);
                tabIdx++;
                tablesQryTxt.Add(tabInfo.TbName, qry_txt);
            }

            return ListGroupBase(tablesQryTxt);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public HttpResponseMessage CountC(JObject param)
        {
            CfDbTable[] tabInfoGrp = this.tableInfoGroup().Tables;
            Dictionary<String, String> tablesQryTxt = new Dictionary<String, String>();

            int tabIdx = 0;
            CfDbTable tabInfoFilter = tabInfoGrp[0];
            String qry_text_filter = tabInfoFilter.listQryText(param, CfDbTable.ListOption.LIST_CURRENT, CfDbTable.OrderByOption.WITHOUT_ORDER_BY, CfDbTable.LockByOption.WITHOUT_LOCKING, CfDbTable.ColumnListOption.DEFAULT_COLUMNS);
            foreach (CfDbTable tabInfo in tabInfoGrp)
            {
                String qry_txt = this.qryTextBuilder(tabInfo, param, CfDbTable.ListOption.LIST_CURRENT, tabIdx, qry_text_filter);
                tabIdx++;
                tablesQryTxt.Add(tabInfo.TbName, qry_txt);
            }

            return ListGroupBase(tablesQryTxt);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public HttpResponseMessage CountCF(JObject param)
        {
            CfDbTable[] tabInfoGrp = this.tableInfoGroup().Tables;
            Dictionary<String, String> tablesQryTxt = new Dictionary<String, String>();

            int tabIdx = 0;
            CfDbTable tabInfoFilter = tabInfoGrp[0];
            String qry_text_filter = tabInfoFilter.listQryText(param, CfDbTable.ListOption.LIST_CURRENT_AND_FUTURE, CfDbTable.OrderByOption.WITHOUT_ORDER_BY, CfDbTable.LockByOption.WITHOUT_LOCKING, CfDbTable.ColumnListOption.DEFAULT_COLUMNS);
            foreach (CfDbTable tabInfo in tabInfoGrp)
            {
                String qry_txt = this.qryTextBuilder(tabInfo, param, CfDbTable.ListOption.LIST_CURRENT_AND_FUTURE, tabIdx, qry_text_filter);
                tabIdx++;
                tablesQryTxt.Add(tabInfo.TbName, qry_txt);
            }

            return ListGroupBase(tablesQryTxt);
        }
        ////////////////////////////////////////////////////////////////////////////////
        protected String qryTextBuilder(CfDbTable tabInfo, JObject param, CfDbTable.ListOption listOption, int tabIdx,
                String qry_text_filter
            )
        {
            //Deberia tener un atributo en la tabla que indique si es normal o excepcion.
            //Si es excepcion deberia ser current and future, si es normal , current.
            Boolean[] tabFilterGrp = this.tableInfoGroup().TableFilterFlag;
            String[] tabColSelGrp = this.tableInfoGroup().TableColFilter;
            String qry_txt = tabInfo.listLockQryText(param);

            if (tabFilterGrp[tabIdx] == false)
            {
                qry_txt += "SELECT COUNT(*) RecCnt\n";
                qry_txt += "FROM\n";
                qry_txt += "(\n";
                qry_txt += qry_text_filter;
                qry_txt += ") tmpFil00\n";
            }
            else
            {
                qry_txt += "SELECT COUNT(*) RecCnt\n";
                qry_txt += "FROM\n";
                qry_txt += "(\n";
                qry_txt += "SELECT\n";
                qry_txt += "tmpBase.*\n";
                qry_txt += "FROM\n";
                qry_txt += "(\n";
                qry_txt += tabInfo.listQryText(param, listOption, CfDbTable.OrderByOption.WITHOUT_ORDER_BY, CfDbTable.LockByOption.WITHOUT_LOCKING, CfDbTable.ColumnListOption.DEFAULT_COLUMNS);
                qry_txt += ")tmpBase\n";
                qry_txt += "INNER JOIN\n";
                qry_txt += "(\n";
                qry_txt += "SELECT DISTINCT\n";

                String sep = " ";
                foreach (String tabCol in tabColSelGrp)
                {
                    qry_txt += sep;
                    qry_txt += "tmpFtr01";
                    qry_txt += ".";
                    qry_txt += tabCol;
                    qry_txt += "\n";
                    sep = ",";
                }

                qry_txt += "FROM\n";
                qry_txt += "(\n";
                qry_txt += qry_text_filter;
                qry_txt += ")tmpFtr01\n";
                qry_txt += ")tmpFtr02\n";
                qry_txt += "ON\n";

                String join_sep = "    ";
                foreach (String tabCol in tabColSelGrp)
                {
                    qry_txt += join_sep;
                    qry_txt += "tmpFtr02";
                    qry_txt += ".";
                    qry_txt += tabCol;
                    qry_txt += " = ";
                    qry_txt += "tmpBase";
                    qry_txt += ".";
                    qry_txt += tabCol;
                    qry_txt += "\n";
                    join_sep = "AND ";
                }

                qry_txt += ")tmpPBC\n";
            }

            return qry_txt;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected HttpResponseMessage ListGroupBase(Dictionary<String, String> tablesQryTxt)
        {
            JObject param = new JObject();
            String conn_str = Parametros().strTeradata;

            using (Teradata.Client.Provider.TdConnection cntn = new Teradata.Client.Provider.TdConnection(conn_str))
            {
                cntn.Open();

                foreach (KeyValuePair<String, String> pair in tablesQryTxt)
                {
                    using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(pair.Value, cntn))
                    {
                        String cmdTimeOutStr = Parametros().strTimeOut;
                        int cmdTimeOut;

                        if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                        {
                            cmd.CommandTimeout = cmdTimeOut;
                        }

                        TdDataReader rdr = cmd.ExecuteReader();
                        int rec_cnt = rdr.RecordsReturned;
                        String countResult = "";

                        if (rec_cnt != 0)  // o podria ser con rdr.RecordsReturned()
                        {
                            while (rdr.Read())
                            {
                                for (int i = 0; i < rdr.FieldCount; i++)
                                {
                                    countResult = rdr.IsDBNull(i) ? "" : rdr.GetString(i).ToString();
                                }
                            }
                        }
                        rdr.Dispose();
                        rdr.Close();
                        cmd.Dispose();

                        this.AddingParameters(param, pair.Key, countResult);
                    }
                }

                cntn.Close();
            }
            
            HttpResponseMessage rspMsg = new HttpResponseMessage(HttpStatusCode.OK);

            rspMsg.Content = new StringContent(JsonConvert.SerializeObject(param, Newtonsoft.Json.Formatting.Indented));
            rspMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return rspMsg;
        }
    }
}
