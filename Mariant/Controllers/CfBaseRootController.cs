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

    abstract public class CfBaseRootController : ApiController
    {
        /// <summary>
        /// Retorna el grupo de tablas que representa este controller.
        /// </summary>
        /// <returns>CfDbTable[]</returns>
        abstract public CfDbTableGroup tableInfoGroup();
        public Parameters Parametros()
        {
            Parameters Parametros = new Parameters();

            Parametros.Parametros();

            return Parametros;
        }
        
        ////////////////////////////////////////////////////////////////////////////////
        const int maxfilesize = 20971520;
        public enum EntryAction { IMPORT_UPSERT, IMPORT_DELETE, EXPORT, PROCESS, EXPORT_QUERY, PROCESS_EXPR, PROCESS_REQUEST, MASTERS };
        public enum ImportMedia { EXCEL, TXT, NONE };
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        //[AuthorizeUploadActivity]
        public async Task<HttpResponseMessage> UploadFileUpsert()
        {
            // Check we're uploading a file
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            string PATH = HttpContext.Current.Server.MapPath("~/uploads/");
            // Create the stream provider, and tell it sort files in my c:\temp\uploads folder
            var provider = new MultipartFormDataStreamProvider(PATH);

            // Read using the stream
            var bodyparts = await Request.Content.ReadAsMultipartAsync(provider);
            IList<string> newFiles = new List<string>();
            IList<string> newExtension = new List<string>();
            foreach (MultipartFileData fd in bodyparts.FileData)
            {
                String oldName = fd.LocalFileName;
                String srcName = fd.Headers.ContentDisposition.FileName;
                Regex rgx = new Regex(@"[^\w\\\.@-]");
                srcName = rgx.Replace(srcName, "");

                srcName = (srcName.Contains("\"")) ? srcName.Substring(1, srcName.Length - 2) : srcName;

                String newName =
                                Path.GetDirectoryName(oldName) + Path.DirectorySeparatorChar +
                                Path.GetFileNameWithoutExtension(srcName) +
                                "_" +
                                Path.GetFileNameWithoutExtension(oldName) +
                                Path.GetExtension(srcName);

                File.Move(oldName, newName);
                newFiles.Add(newName);
                newExtension.Add(Path.GetExtension(srcName));
            }
            //String _RepEvent_id = Request.Properties.ContainsKey("RepEvent_id") ? Request.Properties["RepEvent_id"].ToString() : "";

            Request.Properties.Add("FormData", provider.FormData);
            Request.Properties.Add("FileData", provider.FileData);
            Request.Properties.Add("FileDataNames", newFiles);
            Request.Properties.Add("ExtensionFiles", newExtension);

            String[] values = provider.FormData.GetValues("CfApiAuthorization");
            String[] _PurchaseModule_id = provider.FormData.GetValues("PurchaseModule_id");

            String _RepSession_id;
            String _RepUser_id;
            String[] _RepSessionRoles;
            String _RepUser_EMail;

            AuthenticationToken.Parse(values[0], out _RepSession_id, out _RepUser_id, out _RepSessionRoles, out _RepUser_EMail);

            Request.Properties.Add("RepSession_id", _RepSession_id);
            Request.Properties.Add("RepUser_id", _RepUser_id);
            Request.Properties.Add("RepSessionRoles", _RepSessionRoles);
            Request.Properties.Add("RepUser_EMail", _RepUser_EMail);

            Request.Headers.Add("CfApiAuthorization", values[0]);

            if (_PurchaseModule_id != null)
            {
                Request.Properties.Add("PurchaseModule_id", _PurchaseModule_id[0]);
            }

            return fileProcessQueue(EntryAction.IMPORT_UPSERT);
            //return processUploadedFile(EntryAction.IMPORT_UPSERT);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        //[AuthorizeUploadActivity]
        public async Task<HttpResponseMessage> UploadFileDelete()
        {
            // Check we're uploading a file
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            string PATH = HttpContext.Current.Server.MapPath("~/uploads/");

            // Create the stream provider, and tell it sort files in my c:\temp\uploads folder
            var provider = new MultipartFormDataStreamProvider(PATH);

            // Read using the stream
            var bodyparts = await Request.Content.ReadAsMultipartAsync(provider);

            IList<string> newFiles = new List<string>();
            IList<string> newExtension = new List<string>(); 
            foreach (MultipartFileData fd in bodyparts.FileData)
            {
                String oldName = fd.LocalFileName;
                String srcName = fd.Headers.ContentDisposition.FileName;

                srcName = (srcName.Contains("\"")) ? srcName.Substring(1, srcName.Length - 2) : srcName;

                String newName =
                                Path.GetDirectoryName(oldName) + Path.DirectorySeparatorChar +
                                Path.GetFileNameWithoutExtension(srcName) +
                                "_" +
                                Path.GetFileNameWithoutExtension(oldName) +
                                Path.GetExtension(srcName);
                
                File.Move(oldName, newName);
                newFiles.Add(newName); 
                newExtension.Add(Path.GetExtension(srcName));
            }
            //String _RepEvent_id = Request.Properties.ContainsKey("RepEvent_id") ? Request.Properties["RepEvent_id"].ToString() : "";

            Request.Properties.Add("FormData", provider.FormData);
            Request.Properties.Add("FileData", provider.FileData);
            Request.Properties.Add("FileDataNames", newFiles);
            Request.Properties.Add("ExtensionFiles", newExtension);

            String[] values = provider.FormData.GetValues("CfApiAuthorization");
            String[] _PurchaseModule_id = provider.FormData.GetValues("PurchaseModule_id");

            String _RepSession_id;
            String _RepUser_id;
            String[] _RepSessionRoles;
            String _RepUser_EMail;

            AuthenticationToken.Parse(values[0], out _RepSession_id, out _RepUser_id, out _RepSessionRoles, out _RepUser_EMail);

            Request.Properties.Add("RepSession_id", _RepSession_id);
            Request.Properties.Add("RepUser_id", _RepUser_id);
            Request.Properties.Add("RepSessionRoles", _RepSessionRoles);
            Request.Properties.Add("RepUser_EMail", _RepUser_EMail);

            Request.Headers.Add("CfApiAuthorization", values[0]);

            if (_PurchaseModule_id != null)
            {
                Request.Properties.Add("PurchaseModule_id", _PurchaseModule_id[0]);
            }

            return fileProcessQueue(EntryAction.IMPORT_DELETE);
            //return processUploadedFile(EntryAction.IMPORT_DELETE);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected HttpResponseMessage fileProcessQueue(EntryAction eaction)
        {
            String import_response = "Respuesta default";

            //valida si la carga/borrado tiene permiso para el rol asociado al usuario. 2014-08-11_Nicolas_Oliveto
            try
            {
                AuthorizeActivityAttribute.UploadAuthorization(Request, ControllerContext);
            }
            catch (UnauthorizedAccessException ex)
            {
                HttpResponseMessage response = Request.CreateResponse();

                response.StatusCode = HttpStatusCode.OK;

                response.Content = new StringContent(
    @"<HTML>
<HEAD>
<TITLE>File Upload Return</TITLE>
</HEAD>
<BODY>" + ex.Message +
    @"
</BODY>
</HTML>", new System.Text.ASCIIEncoding(), "text/html");

                return response;
            }

            List<String> files = (List<String>)(Request.Properties.ContainsKey("FileDataNames") ? Request.Properties["FileDataNames"] : null);
            List<String> extensions = (List<String>)(Request.Properties.ContainsKey("ExtensionFiles") ? Request.Properties["ExtensionFiles"] : null);

            // ¿Habria que hacer un ciclo por si mandan mas de un archivo? Gerardo
            String srcLocalFileName = Regex.Replace(files.First<String>(), @"\\", @"\\");
            String srcExtension = extensions.First<String>();
            ImportMedia imedia;
            String _status = "Pendiente";
            String strconn = Parametros().strTeradataLoad;
            String _RepEvent_id = Request.Properties.ContainsKey("RepEvent_id") ? Request.Properties["RepEvent_id"].ToString() : "";
            String _RepSession_id = Request.Properties.ContainsKey("RepSession_id") ? Request.Properties["RepSession_id"].ToString() : "";
            String _RepUser_id = Request.Properties.ContainsKey("RepUser_id") ? Request.Properties["RepUser_id"].ToString() : "lourdes_martinez";
            String _RepUser_EMail = Request.Properties.ContainsKey("RepUser_EMail") ? Request.Properties["RepUser_EMail"].ToString() : "lourdes_martinez@carrefour.com";
            String _PurchaseModule_id = Request.Properties.ContainsKey("PurchaseModule_id") ? Request.Properties["PurchaseModule_id"].ToString() : "";
            String resFileName = Path.GetDirectoryName(srcLocalFileName) + Path.DirectorySeparatorChar.ToString() + Path.GetFileNameWithoutExtension(srcLocalFileName) + "_result" + Path.GetExtension(srcLocalFileName);
            resFileName = Regex.Replace(resFileName, @"\\", @"\\");
            String workingDir = Path.GetDirectoryName(srcLocalFileName) + Path.DirectorySeparatorChar.ToString() + Path.GetFileNameWithoutExtension(srcLocalFileName) + "_work";
            Directory.CreateDirectory(workingDir); // Para usar en import.
            workingDir = Regex.Replace(workingDir, @"\\", @"\\");

            //armo el link url para la descarga.
            String rootUri = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, "/");
            String[] dirParts = Path.GetDirectoryName(resFileName).Split(Path.DirectorySeparatorChar);
            String srcFileName = rootUri + dirParts[dirParts.Length - 2] + "/" + dirParts[dirParts.Length - 1] + "/" + Path.GetFileName(srcLocalFileName);
            String resFileUri = rootUri + dirParts[dirParts.Length - 2] + "/" + dirParts[dirParts.Length - 1] + "/" + Path.GetFileName(resFileName);
            String _RepDbObject_id = this.ToString().Split('.')[2];
            String _RunDate_id = DateTime.Today.ToString("yyyy-MM-dd");
            int enabledNum = 0;

            if (srcExtension == ".txt")
            {
                imedia = ImportMedia.TXT;
            }
            else
            {
                imedia = ImportMedia.EXCEL;
            }

            String _RepQueueEntryType_id = eaction.ToString();
            String _RepQueueMediaType_id = imedia.ToString();

            JObject param = new JObject();
            this.CreateParameters(param, "RepEvent_id", _RepEvent_id);
            this.CreateParameters(param, "RepUser_id", _RepUser_id);
            this.CreateParameters(param, "RunDate_id", _RunDate_id);
            this.CreateParameters(param, "RepSession_id", _RepSession_id);
            this.CreateParameters(param, "PurchaseModule_id", _PurchaseModule_id);
            String _RepEventParameters_Txt = JsonConvert.SerializeObject(param, Newtonsoft.Json.Formatting.None);
            _RepEventParameters_Txt = _RepEventParameters_Txt.Replace("\n", "");
            _RepEventParameters_Txt = _RepEventParameters_Txt.Replace("\r", "");


            using (TdConnection cntn = new TdConnection(strconn))
            {
                cntn.Open();

                //Aca empiezo a grabar en las tablas
                using (TdTransaction trx = cntn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        // Si el archivo tiene mas de 20mb lo rechazo
                        FileInfo inputfile = new FileInfo(srcLocalFileName);
                        long filesize = inputfile.Length;

                        if (filesize > maxfilesize)
                        {
                            throw new Exception("Carga Rechazada. Motivo: El archivo debe ser menor a 20MB");
                        }

                        if (!_RepUser_id.Equals("admin",StringComparison.CurrentCultureIgnoreCase))
                        {
                            enabledNum = this.validateDbObjectQueueRank(_RepDbObject_id, cntn, trx);
                        }

                        if (enabledNum == 0)
                        {
                            this.LockTableForQueue(cntn, trx);

                            this.StartFileProcessQueueEvent(_RepEvent_id, _RepQueueEntryType_id, _RepQueueMediaType_id,
                                        _RepEventParameters_Txt, srcFileName, srcLocalFileName, resFileName, workingDir,
                                        resFileUri, _RepDbObject_id, _status, cntn, trx
                                );

                            List<Dictionary<String, String>> dbObjPrecedents = this.objectPrecedent(_RepEvent_id, cntn, trx, eaction);

                            if (dbObjPrecedents != null)
                            {
                                this.objectQueuePrecedent(dbObjPrecedents, cntn, trx);
                            }

                            import_response = "El archivo ha ingresado a la cola de carga. Recibira el resultado via email. ";
                            import_response += " Podra ver el estado del mismo, a traves de la ventana de control de cargas.";
                        }
                        else
                        {
                            import_response = "El archivo no ha ingresado a la cola de carga en estos momentos por estar";
                            import_response += " en un rango horario de restriccion";
                        }

                        trx.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (trx != null) { trx.Rollback(); }

                        import_response = ex.Message;
                    }
                    finally
                    {
                        if (trx != null) { trx.Dispose(); }

                        if (cntn != null) { cntn.Close(); }
                    }
                }
            }

            HttpResponseMessage rspMsg = Request.CreateResponse();

            rspMsg.StatusCode = HttpStatusCode.OK;

            rspMsg.Content = new StringContent(
@"<HTML>
<HEAD>
<TITLE>File Upload Return</TITLE>
</HEAD>
<BODY>" + import_response +
@"
</BODY>
</HTML>", new System.Text.ASCIIEncoding(), "text/html");

            return rspMsg;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void CreateParameters(JObject param, String NameProperty, String listOption)
        {
            JProperty prop = new JProperty(NameProperty);
            prop.Value = listOption;
            param.Add(prop);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public String Downloadfile(JObject param)
        {
            return DownloadfileBase(param, CfDbTable.ListOption.LIST_CURRENT_AND_FUTURE, "", EntryAction.EXPORT);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public String DownloadfileHist(JObject param)
        {
            return DownloadfileBase(param, CfDbTable.ListOption.LIST_ALL, "Hist_", EntryAction.EXPORT);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public virtual String DownloadfileQuery(JObject param)
        {
            return DownloadfileBase(param, CfDbTable.ListOption.LIST_CURRENT_AND_FUTURE, "", EntryAction.EXPORT_QUERY);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        [AuthorizeActivity]
        public String DownloadfileQueryHist(JObject param)
        {
            return DownloadfileBase(param, CfDbTable.ListOption.LIST_ALL, "Hist_", EntryAction.EXPORT_QUERY);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected String DownloadfileBase(JObject param, CfDbTable.ListOption listOption, String fNamePrefix, 
                EntryAction _eaction
            )
        {
            String fecha = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
            String mainTableName = tableInfoGroup().Tables[0].TbLocalName.Replace(" ", "_");
            String strconn = Parametros().strTeradataLoad;
            
            String _RepEvent_id = Request.Properties.ContainsKey("RepEvent_id") ? Request.Properties["RepEvent_id"].ToString() : "";
            String _RepUser_EMail = Request.Properties.ContainsKey("RepUser_EMail") ? Request.Properties["RepUser_EMail"].ToString() : "lourdes_martinez@carrefour.com";
            String _RepUser_id = Request.Properties.ContainsKey("RepUser_id") ? Request.Properties["RepUser_id"].ToString() : "lourdes_martinez";
            String _RepDbObject_id = this.ToString().Split('.')[2];
            String _RepQueueEntryType_id = _eaction.ToString();
            String _RepQueueMediaType_id = "";
            String submit_response;
            String typemedia = "";
            ImportMedia imedia;
            String _status = "Pendiente";
            List<Dictionary<String, String>> dbObjPrecedents = null;

            string PATH = HttpContext.Current.Server.MapPath("~/downloads/");
            String rootUri = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, "/");
            String uniqueNumber = Guid.NewGuid().ToString("D");
            String targetFilename = "";
            Dictionary<String, List<String>> dicQryText = new Dictionary<String,List<String>>();

            //Aca armo el param con where
            if (listOption == CfDbTable.ListOption.LIST_ALL)
            {
                this.AddingParameters(param, "ListOption", "LIST_ALL");
            }
            else
            {
                this.AddingParameters(param, "ListOption", "LIST_CURRENT_AND_FUTURE");
            }

            String _RepEventParameters_Txt = JsonConvert.SerializeObject(param, Newtonsoft.Json.Formatting.Indented);
            _RepEventParameters_Txt = _RepEventParameters_Txt.Replace("\n", "");
            _RepEventParameters_Txt = _RepEventParameters_Txt.Replace("\r", "");
            int iStringSize = ASCIIEncoding.ASCII.GetByteCount(_RepEventParameters_Txt);

            if (iStringSize > 50000)
            {
                throw new Exception("El filtro supera el tamaño permitido. Seleccione menos datos en el filtro. Muchas gracias.");
            }

            TdConnection cntn = new TdConnection(strconn);
            cntn.Open();

            if (_eaction == EntryAction.EXPORT_QUERY)
            {
                dicQryText = this.QryTextBuilder(param, listOption, _RepEvent_id, _RepUser_id, cntn);
            }

            TdTransaction trx = cntn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                //Aca defino si cargo los exportquery, y armo las query
                JArray imp_med = null;

                if (param != null)
                {
                    imp_med = (JArray)param["ImportMedia"]; //Lee una property del objeto que es un array.
                }
                else
                {
                    throw new Exception("Falta parametro ImportMedia");
                }

                if (imp_med != null)
                {
                    foreach (JObject jobmedia in imp_med)
                    {
                        JArray val_list = (JArray)jobmedia["Type"];

                        typemedia = val_list[0].ToString();
                    }
                }

                if (typemedia == "TXT")
                {
                    imedia = ImportMedia.TXT;
                    targetFilename = PATH + fNamePrefix + mainTableName + "-" + fecha + "_" + uniqueNumber + ".zip";
                    targetFilename = Regex.Replace(targetFilename, @"\\", @"\\");
                }
                else
                {
                    imedia = ImportMedia.EXCEL;
                    targetFilename = PATH + fNamePrefix + mainTableName + "-" + fecha + "_" + uniqueNumber + ".xlsx";
                    targetFilename = Regex.Replace(targetFilename, @"\\", @"\\");
                }
                _RepQueueMediaType_id = imedia.ToString();

                String[] dirParts = Path.GetDirectoryName(targetFilename).Split(Path.DirectorySeparatorChar);
                String resFileUri = rootUri + dirParts[dirParts.Length - 2] + "/" + dirParts[dirParts.Length - 1] + "/" + Path.GetFileName(targetFilename);
                String _WorkingDir_Name = PATH + Path.GetFileNameWithoutExtension(targetFilename) + "_work";
                Directory.CreateDirectory(_WorkingDir_Name);
                _WorkingDir_Name = Regex.Replace(_WorkingDir_Name, @"\\", @"\\");

                this.LockTableForQueue(cntn, trx);

                if (dicQryText.Count != 0)
                {
                    this.insertQryTextToExport(dicQryText, cntn, trx, _RepEvent_id);
                }

                this.StartFileProcessQueueEvent(
                            _RepEvent_id,
                            _RepQueueEntryType_id,
                            _RepQueueMediaType_id,
                            _RepEventParameters_Txt,
                            "",
                            "",
                            targetFilename,
                            _WorkingDir_Name,
                            resFileUri,
                            _RepDbObject_id,
                            _status,
                            cntn,
                            trx
                    );

                dbObjPrecedents = this.objectPrecedent(_RepEvent_id, cntn, trx, _eaction);

                if (dbObjPrecedents != null)
                {
                    this.objectQueuePrecedent(
                                    dbObjPrecedents,
                                    cntn,
                                    trx
                        );
                }

                trx.Commit();

                submit_response = "El archivo ha ingresado a la cola de carga. Recibira el resultado via email. ";
                submit_response += " Podra ver el estado del mismo, a traves de la ventana de control de cargas.";
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
            mdRs.Add(new JProperty("RepEvent_id", _RepEvent_id));
            res_ret.Add("ResultSetMetaData", mdRs);

            return JsonConvert.SerializeObject(res_ret, Newtonsoft.Json.Formatting.Indented);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        public virtual String SubmitProcess(JObject param, Dictionary<String, List<String>> dicQryTxt, EntryAction eaction)
        {
            int enabledNum = 0;
            String strconn = Parametros().strTeradataLoad;
            String _RepEvent_id = Request.Properties.ContainsKey("RepEvent_id") ? Request.Properties["RepEvent_id"].ToString() : "";
            String _RepUser_EMail = Request.Properties.ContainsKey("RepUser_EMail") ? Request.Properties["RepUser_EMail"].ToString() : "lourdes_martinez@carrefour.com";
            String _RepUser_id = Request.Properties.ContainsKey("RepUser_id") ? Request.Properties["RepUser_id"].ToString() : "lourdes_martinez";
            String _RepDbObject_id = this.ToString().Split('.')[2];
            String _RepQueueEntryType_id = eaction.ToString();
            String _RepQueueMediaType_id = ImportMedia.NONE.ToString();
            String submit_response;
            String resFileUri = "";
            String _status = "Pendiente";
            List<Dictionary<String, String>> dbObjPrecedents = null;
            String targetFilename = "";
            String _RepEventParameters_Txt = JsonConvert.SerializeObject(param, Newtonsoft.Json.Formatting.Indented);
            string PATH = HttpContext.Current.Server.MapPath("~/process/");
            String _WorkingDir_Name = PATH + _RepEvent_id + "_work";
            Directory.CreateDirectory(_WorkingDir_Name);
            _WorkingDir_Name = Regex.Replace(_WorkingDir_Name, @"\\", @"\\");

            _RepEventParameters_Txt = _RepEventParameters_Txt.Replace("\n", "");
            _RepEventParameters_Txt = _RepEventParameters_Txt.Replace("\r", "");
            int iStringSize = ASCIIEncoding.ASCII.GetByteCount(_RepEventParameters_Txt);

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

                if (!_RepUser_id.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
                {
                    enabledNum = this.validateDbObjectQueueRank(_RepDbObject_id, cntn, trx);
                }

                if (enabledNum == 0)
                {
                    if (eaction == EntryAction.PROCESS)
                    {
                        if (dicQryTxt.Count != 0)
                        {
                            this.insertQryTextToExport(dicQryTxt, cntn, trx, _RepEvent_id);
                        }
                        else
                        {
                            throw new Exception("Faltan queries para ejecutar el proceso");
                        }
                    }

                    this.StartFileProcessQueueEvent(
                                _RepEvent_id,
                                _RepQueueEntryType_id,
                                _RepQueueMediaType_id,
                                _RepEventParameters_Txt,
                                "",
                                "",
                                targetFilename,
                                _WorkingDir_Name,
                                resFileUri,
                                _RepDbObject_id,
                                _status,
                                cntn,
                                trx
                        );

                    dbObjPrecedents = this.objectPrecedent(_RepEvent_id, cntn, trx, eaction);

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
            mdRs.Add(new JProperty("RepEvent_id", _RepEvent_id));
            res_ret.Add("ResultSetMetaData", mdRs);

            return JsonConvert.SerializeObject(res_ret, Newtonsoft.Json.Formatting.Indented);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected String fillProductEventFilterText(JObject param, String RepEvent_id, CfDbTable.ListOption list_option, String DbName,
                String TbName
            )
        {
            String schema_name = Parametros().strDbSchema;

            String fill_filter_qry_txt = ""; // Query final.

            //Query base 
            String base_qry = DbModel.ProductBarCodeHist.listQryText(param, list_option, CfDbTable.OrderByOption.WITHOUT_ORDER_BY,
                    CfDbTable.LockByOption.WITHOUT_LOCKING, CfDbTable.ColumnListOption.DEFAULT_COLUMNS);

            fill_filter_qry_txt += "INSERT INTO " + DbName + "." + TbName + "\n";
            fill_filter_qry_txt += "(\n";
            fill_filter_qry_txt += "RepEvent_id\n";
            fill_filter_qry_txt += ",Product_id\n";
            fill_filter_qry_txt += ",ProductBarCode_id\n";
            fill_filter_qry_txt += ")\n";
            fill_filter_qry_txt += "SELECT\n";
            fill_filter_qry_txt += "'" + RepEvent_id + "' RepEvent_id\n";
            fill_filter_qry_txt += ",Product_id\n";
            fill_filter_qry_txt += ",ProductBarCode_id\n";
            fill_filter_qry_txt += "FROM\n";
            fill_filter_qry_txt += "(\n";
            fill_filter_qry_txt += base_qry;
            fill_filter_qry_txt += ") tmpRecCnt\n";
            fill_filter_qry_txt += ";";

            return fill_filter_qry_txt;
        }
        //////////////////////////////////////////////////////////////////////
        [NonAction]
        private void LockTableForQueue(TdConnection cntn, TdTransaction trx)
        {
            String qry_txt = @"
LOCKING TABLE {0}.RepFileProcessQueueLockAccess FOR EXCLUSIVE";

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
        //////////////////////////////////////////////////////////////////////
        [NonAction]
        protected List<Dictionary<String, String>> objectPrecedent(String _RepEvent_id, TdConnection cntn, TdTransaction trx,
                EntryAction eaction
            )
        {
            String qry_txt = "";

            qry_txt += "LOCKING TABLE {0}.RepFileProcessQueue FOR ACCESS\n";
            qry_txt += "LOCKING TABLE {0}.RepDbObjectPrecedent FOR ACCESS\n";
            qry_txt += "SELECT\n";
            qry_txt += "tmp2.RepEvent_id,\n";
            qry_txt += "tmp1.RepEvent_id AS PrecedentRepEvent_id\n";
            qry_txt += "FROM\n";
            qry_txt += "(\n";
            qry_txt += "SELECT\n";
            qry_txt += "rfpq.RepEvent_id,\n";
            qry_txt += "rfpq.Enqueue_Dt,\n";
            qry_txt += "rfpq.Enqueue_Tm,\n";
            qry_txt += "rfpq.RepDbObject_id,\n";
            qry_txt += "rfpq.RepFileProcessStatus_id\n";
            qry_txt += "FROM\n";
            qry_txt += "{0}.RepFileProcessQueue rfpq\n";

            if (eaction == EntryAction.IMPORT_DELETE || eaction == EntryAction.IMPORT_UPSERT ||
                    eaction == EntryAction.PROCESS || eaction == EntryAction.PROCESS_EXPR
                )
            {
                qry_txt += "WHERE\n";
                qry_txt += "rfpq.RepQueueEntryType_id NOT IN ('EXPORT_QUERY','EXPORT')\n";
            }
            else
            {
                qry_txt += "WHERE\n";
                qry_txt += "rfpq.RepQueueEntryType_id IN ('EXPORT_QUERY','EXPORT')\n";
            }

            qry_txt += ") tmp1,\n";
            qry_txt += "(\n";
            qry_txt += "SELECT\n";
            qry_txt += "rfpq.RepEvent_id,\n";
            qry_txt += "rfpq.Enqueue_Dt,\n";
            qry_txt += "rfpq.Enqueue_Tm,\n";
            qry_txt += "rdop.PrecedentRepDbObject_id\n";
            qry_txt += "FROM\n";
            qry_txt += "{0}.RepFileProcessQueue rfpq\n";
            qry_txt += "INNER JOIN\n";
            qry_txt += "{0}.RepDbObjectPrecedent rdop\n";
            qry_txt += "ON\n";
            qry_txt += "rdop.RepDbObject_id = rfpq.RepDbObject_id\n";
            qry_txt += "WHERE\n";
            qry_txt += "rfpq.RepEvent_id = ?\n";

            if (eaction == EntryAction.EXPORT || eaction == EntryAction.EXPORT_QUERY)
            {
                qry_txt += "AND rdop.RepDbObject_id = rdop.PrecedentRepDbObject_id\n";
            }

            qry_txt += ") tmp2\n";
            qry_txt += "WHERE\n";
            qry_txt += "((tmp2.Enqueue_Dt > tmp1.Enqueue_Dt)\n";
            qry_txt += "OR ((tmp2.Enqueue_Dt = tmp1.Enqueue_Dt)\n";
            qry_txt += "AND (tmp2.Enqueue_Tm > tmp1.Enqueue_Tm)))\n";
            qry_txt += "AND tmp2.PrecedentRepDbObject_id = tmp1.RepDbObject_id\n";
            qry_txt += "AND tmp1.RepFileProcessStatus_id NOT IN ('Finalizado','Cancelado','Finalizado con error','Finalizado con alerta','Cancelado Automatico')\n";
            qry_txt += ";\n";

            String schema_name = Parametros().strDbSchema;
            String conn_str = Parametros().strTeradataLoad;
            List<Dictionary<String, String>> results = null;

            using (TdCommand cmd = new TdCommand(String.Format(qry_txt, schema_name), cntn, trx))
            {
                TdParameter tp;

                tp = new TdParameter("RepEvent_id", System.Data.DbType.String); tp.Value = _RepEvent_id; cmd.Parameters.Add(tp);

                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                TdDataReader rdr = cmd.ExecuteReader();
                int rec_cnt = rdr.RecordsReturned;

                if (rec_cnt != 0)
                {
                    // Si hay registros de salida, creo la lista,...
                    results = new List<Dictionary<String, String>>();

                    // y la lleno.
                    while (rdr.Read())
                    {
                        Dictionary<String, String> rec = new Dictionary<String, String>();

                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            rec.Add(rdr.GetName(i), rdr.IsDBNull(i) ? "" : rdr.GetString(i).ToString());
                        }
                        results.Add(rec);
                    }
                }
                rdr.Dispose();
                rdr.Close();
                cmd.Dispose();
            }

            return results;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void StartFileProcessQueueEvent(String _RepEvent_id, String _RepQueueEntryType_id, String _RepQueueMediaType_id,
                String _RepEventParameters_Txt, String _File_Name, String _LocalFile_Name, String _ResponseFile_Name, String _WorkingDir_Name,
                String _UrlFile_Name, String _RepObject_id, String _RepFileProcessStatus_id, TdConnection cntn, TdTransaction trx
            )
        {
            String schema_name = Parametros().strDbSchema;
            String conn_str = Parametros().strTeradataLoad;

            String start_file_process_queue_event_txt = @"
INSERT INTO {0}.RepFileProcessQueue
(
    RepEvent_id,
    RepQueueEntryType_id,
    RepQueueMediaType_id,
    RepEventParameters_Txt,
    File_Name,
    LocalFile_Name,
    ResponseFile_Name,
    WorkingDir_Name,
    UrlFile_Name,
    RepDbObject_id,
    Enqueue_Dt,
    Enqueue_Tm,
    RepFileProcessStatus_id
)
VALUES
(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, CURRENT_DATE, CAST ( CURRENT_TIME(6) AS TIME(6) ), ? )";

            using (TdCommand cmd = new TdCommand(String.Format(start_file_process_queue_event_txt, schema_name), cntn, trx))
            {
                TdParameter tp;

                tp = new TdParameter("RepEvent_id", System.Data.DbType.String); tp.Value = _RepEvent_id; cmd.Parameters.Add(tp);
                tp = new TdParameter("RepQueueEntryType_id", System.Data.DbType.String); tp.Value = _RepQueueEntryType_id; cmd.Parameters.Add(tp);
                tp = new TdParameter("RepQueueMediaType_id", System.Data.DbType.String); tp.Value = _RepQueueMediaType_id; cmd.Parameters.Add(tp);
                tp = new TdParameter("RepEventParameters_Txt", System.Data.DbType.String); tp.Value = _RepEventParameters_Txt; cmd.Parameters.Add(tp);
                tp = new TdParameter("File_Name", System.Data.DbType.String); tp.Value = _File_Name; cmd.Parameters.Add(tp);
                tp = new TdParameter("LocalFile_Name", System.Data.DbType.String); tp.Value = _LocalFile_Name; cmd.Parameters.Add(tp);
                tp = new TdParameter("ResponseFile_Name", System.Data.DbType.String); tp.Value = _ResponseFile_Name; cmd.Parameters.Add(tp);
                tp = new TdParameter("WorkingDir_Name", System.Data.DbType.String); tp.Value = _WorkingDir_Name; cmd.Parameters.Add(tp);
                tp = new TdParameter("UrlFile_Name", System.Data.DbType.String); tp.Value = _UrlFile_Name; cmd.Parameters.Add(tp);
                tp = new TdParameter("RepObject_id", System.Data.DbType.String); tp.Value = _RepObject_id; cmd.Parameters.Add(tp);
                tp = new TdParameter("RepFileProcessStatus_id", System.Data.DbType.String); tp.Value = _RepFileProcessStatus_id; cmd.Parameters.Add(tp);

                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                int rows_affected = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void objectQueuePrecedent(List<Dictionary<String, String>> dbObjPrecedents, TdConnection cntn, TdTransaction trx)
        {
            String schema_name = Parametros().strDbSchema;
            String conn_str = Parametros().strTeradataLoad;
            String[] paramNames;

            // Tomo string de upsert.
            String qry_txt = DbModel.RepFileProcessQueuePrecedent.mergeQryText(out paramNames);

            using (TdCommand cmd = new TdCommand(qry_txt, cntn, trx))
            {
                foreach (String pName in paramNames)
                {
                    TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                    cmd.Parameters.Add(tp);
                }

                //Inserta cada elemento del array.
                foreach (Dictionary<String, String> rec in dbObjPrecedents)
                {
                    //Asigna parametros.
                    foreach (TdParameter tp in cmd.Parameters)
                    {
                        String value;
                        if (!rec.TryGetValue(tp.ParameterName, out value))
                        {
                            throw new Exception("Falta informar campo '" + tp.ParameterName + "'.");
                        }
                        tp.Value = value;
                    }

                    String cmdTimeOutStr = Parametros().strTimeOut;
                    int cmdTimeOut;

                    if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                    {
                        cmd.CommandTimeout = cmdTimeOut;
                    }

                    int rows_affected = cmd.ExecuteNonQuery();
                }

                //Libera recursos de base de datos.
                cmd.Dispose();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void AddingParameters(JObject param, String NameProperty, String ValueProperty)
        {
            JArray paramParam = new JArray(ValueProperty);
            JProperty prop = new JProperty(NameProperty);
            JObject joparam = new JObject();
            joparam.Add(prop);
            joparam.Property(NameProperty).Value = paramParam;
            JArray japaram = new JArray();
            japaram.Add(joparam);
            param.Add(NameProperty, japaram);
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected virtual Dictionary<String, List<String>> QryTextBuilder(JObject param, CfDbTable.ListOption list_option, String RepEvent_id,
                String RepUser_id, TdConnection cntn
            )
        {
            Dictionary<String, List<String>> dicQryText = new Dictionary<String, List<String>>();

            foreach (CfDbTable tabInfo in tableInfoGroup().Tables)
            {
                List<String> rec = new List<String>();
                String qry_text = tabInfo.listQryText(param, list_option, CfDbTable.OrderByOption.WITH_ORDER_BY, CfDbTable.LockByOption.WITH_LOCKING, CfDbTable.ColumnListOption.DEFAULT_COLUMNS);
                rec.Add(qry_text);
                dicQryText.Add(tabInfo.TbName, rec);
            }

            return dicQryText;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void insertQryTextToExport(Dictionary<String, List<String>> dicQryText, TdConnection cntn, TdTransaction trx, 
                String RepEvent_id
            )
        {
            String schema_name = Parametros().strDbSchema;
            String conn_str = Parametros().strTeradataLoad;
            int tabIndex = 0;


            foreach (KeyValuePair<String, List<String>> pair in dicQryText)
            {
                String Sheet_Name = pair.Key;
                List<String> listQryText = pair.Value;

                this.RepFileProcessGroupQuery(Sheet_Name, cntn, trx, RepEvent_id, schema_name, conn_str, tabIndex);

                this.RepFileProcessQuery(listQryText, cntn, trx, RepEvent_id, schema_name, conn_str, tabIndex);

                tabIndex++;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void RepFileProcessGroupQuery(String Sheet_Name, TdConnection cntn, TdTransaction trx, String RepEvent_id, 
                String schema_name, String conn_str, int tabIndex
            )
        {
            String[] paramNames;
            String qry_txt = DbModel.RepFileProcessSheetGroup.mergeQryText(out paramNames);

            using (TdCommand cmd = new TdCommand(qry_txt, cntn, trx))
            {
                foreach (String pName in paramNames)
                {
                    TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                    cmd.Parameters.Add(tp);
                }

                //Le asigno el evento pues siempre es el mismo
                cmd.Parameters["RepEvent_id"].Value = RepEvent_id;
                cmd.Parameters["Sheet_Num"].Value = tabIndex;
                cmd.Parameters["Sheet_Name"].Value = Sheet_Name;

                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                int rows_affected = cmd.ExecuteNonQuery();

                //Libera recursos de la base de datos.
                cmd.Dispose();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void RepFileProcessQuery(List<String> listQryText, TdConnection cntn, TdTransaction trx, String RepEvent_id, 
                String schema_name, String conn_str, int tabIndex
            )
        {
            String[] paramNames;
            String qry_txt = DbModel.RepFileProcessQuery.mergeQryText(out paramNames);

            using (TdCommand cmd = new TdCommand(qry_txt, cntn, trx))
            {
                foreach (String pName in paramNames)
                {
                    TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                    cmd.Parameters.Add(tp);
                }

                //Le asigno el evento pues siempre es el mismo
                cmd.Parameters["RepEvent_id"].Value = RepEvent_id;
                cmd.Parameters["Sheet_Num"].Value = tabIndex;
                int querynum = 0;

                for (int i = 0; i < listQryText.Count(); i++)
                {
                    String qry_text = listQryText[i];
                    //int ilinenum = 0;

                    cmd.Parameters["Query_Num"].Value = querynum;

                    //cmd.Parameters["RepFileProcessQueryLine_id"].Value = ilinenum;
                    cmd.Parameters["RepFileProcessQueryLine_Txt"].Value = qry_text;

                    String cmdTimeOutStr = Parametros().strTimeOut;
                    int cmdTimeOut;

                    if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                    {
                        cmd.CommandTimeout = cmdTimeOut;
                    }

                    int rows_affected = cmd.ExecuteNonQuery();

                    querynum++;
                }

                //Libera recursos de la base de datos.
                cmd.Dispose();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected int validateDbObjectQueueRank(String _RepDbObject, TdConnection cntn, TdTransaction trx)
        {
            String currentTime = DateTime.Now.ToString("HH:mm:ss");
            String schema_name = Parametros().strDbSchema;

            String qry_txt = @"
SELECT
    QueueEnabled_Ind
FROM
" + schema_name + @".RepDbObjectQueue
WHERE
    RepDbObject_id = '" + _RepDbObject + @"'
AND CURRENT_DATE Between EffectiveDate and ExpirationDate
AND CAST('" + currentTime + @"' AS TIME(0)) Between EffectiveTime and ExpirationTime
AND QueueEnabled_Ind IN ('N')
;";

            TdCommand cmd = new TdCommand(qry_txt, cntn, trx);

            String cmdTimeOutStr = Parametros().strTimeOut;
            int cmdTimeOut;

            if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
            {
                cmd.CommandTimeout = cmdTimeOut;
            }

            TdDataReader rdr = cmd.ExecuteReader();
            int rec_cnt = rdr.RecordsReturned;

            rdr.Dispose();
            rdr.Close();
            cmd.Dispose();

            return rec_cnt;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void checkParentRI(CfDbTable tabInfo, JObject param, Teradata.Client.Provider.TdConnection cntn)
        {
            int parentIdx;
            int parentCnt = tabInfo.getParentTablesCnt();
            String excMsg = "";
            String excSep = "";
            JObject cond_record = null;
            JObject cond_null = null;
            JObject param_record = null;

            if (param != null)
            {
                cond_null = (JObject)param["isNull"];
                cond_record = (JObject)param["Record"];
            }

            if (cond_record != null)
            {
                param_record = cond_record;
            }
            else
            {
                param_record = param;
            }

            // Verifica TODAS las foreing key. Si alguna falla se genera una excepcion.
            for (parentIdx = 0; parentIdx < parentCnt; parentIdx++)
            {
                String qry_txt;
                String parentTableName;
                String parentTableLocalName;
                String[] paramNames;
                String[] paramLocalNames;
                JProperty t_null = null;

                qry_txt = tabInfo.riParentCheckQryText(parentIdx, out parentTableName, out paramNames, out parentTableLocalName, out paramLocalNames);

                using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn))
                {
                    foreach (String pName in paramNames)
                    {
                        if (cond_null != null)
                        {
                            t_null = cond_null.Property(pName);
                        }

                        JProperty t = param_record.Property(pName);
                        TdParameter tp = new TdParameter(t.Name, System.Data.DbType.String);
                        tp.Value = t.Value.ToString();
                        cmd.Parameters.Add(tp);
                    }

                    if (t_null == null)
                    {
                        String cmdTimeOutStr = Parametros().strTimeOut;
                        int cmdTimeOut;

                        if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                        {
                            cmd.CommandTimeout = cmdTimeOut;
                        }

                        TdDataReader rdr = cmd.ExecuteReader();
                        cmd.Dispose();

                        if (!rdr.HasRows)  // o podria ser con rdr.RecordsReturned()
                        {
                            //Si no encuentra registro padre, la transaccion no puede llevarse a cabo.          
                            excMsg += excSep;
                            excMsg += "No existe registro en ";
                            //excMsg += parentTableName;
                            excMsg += parentTableLocalName;
                            excMsg += " con clave (";
                            String sep = "";
                            foreach (String pName in paramNames)
                            {
                                excMsg += sep;
                                //excMsg += pName;
                                int indexpos = Array.IndexOf(paramNames, pName);
                                if (indexpos != -1)
                                {
                                    excMsg += paramLocalNames[indexpos];
                                }
                                else
                                {
                                    throw new Exception("Campos inexistente: " + pName);
                                }
                                sep += ", ";
                            }
                            excMsg += ") = (";
                            sep = "";
                            foreach (String pName in paramNames)
                            {
                                excMsg += sep;
                                JProperty t = param_record.Property(pName);
                                excMsg += t.Value.ToString();
                                sep += ", ";
                            }
                            excMsg += ").";
                            excSep = "\n";
                        }
                        rdr.Dispose();
                        rdr.Close();
                    }
                }
            }
            if (!excMsg.Equals("")) // Si fallo alguna columna.
            {
                throw new Exception(excMsg);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void checkChildRI(CfDbTable tabInfo, JObject param, Teradata.Client.Provider.TdConnection cntn)
        {
            int childIdx;
            int childCnt = tabInfo.getChildTablesCnt();
            String excMsg = "";
            String excSep = "";

            // Verifica TODAS las foreing key. Si alguna falla se genera una excepcion.
            for (childIdx = 0; childIdx < childCnt; childIdx++)
            {
                String qry_txt;
                String childTableName;
                String[] paramNames;
                String childTableLocalName;
                String[] paramLocalNames;

                qry_txt = tabInfo.riChildCheckQryText(childIdx, out childTableName, out paramNames, out childTableLocalName, out paramLocalNames);

                using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn))
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

                    TdDataReader rdr = cmd.ExecuteReader();
                    cmd.Dispose();

                    if (rdr.HasRows)  // o podria ser con rdr.RecordsReturned()
                    {
                        //Si encuentra registros hijos, la transaccion no puede llevarse a cabo.

                        // Armo mensaje de salida.          
                        excMsg += excSep;
                        excMsg += "Existe registro en ";
                        //excMsg += childTableName;
                        excMsg += childTableLocalName;
                        excMsg += " con clave (";
                        String sep = "";
                        foreach (String pName in paramNames)
                        {
                            excMsg += sep;
                            //excMsg += pName;
                            int indexpos = Array.IndexOf(paramNames, pName);
                            if (indexpos != -1)
                            {
                                excMsg += paramLocalNames[indexpos];
                            }
                            else
                            {
                                throw new Exception("Campos inexistente: " + pName);
                            }
                            sep += ", ";
                        }
                        excMsg += ") = (";
                        sep = "";
                        foreach (String pName in paramNames)
                        {
                            excMsg += sep;
                            JProperty t = param.Property(pName);
                            excMsg += t.Value.ToString();
                            sep += ", ";
                        }
                        excMsg += ").";
                        excSep = "\n";
                    }
                    rdr.Dispose();
                }
            }
            if (!excMsg.Equals("")) // Si fallo alguna columna.
            {
                throw new Exception(excMsg);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected List<Dictionary<String, String>> getOverlappingPaceFollower(JObject param, CfDbTable tabInfo, TdConnection cntn, TdTransaction trx)
        {
            //Variables para query que lista registros fuente.
            String qry_txt;
            String[] paramNames;

            List<Dictionary<String, String>> recordList = null;

            // Tomo string de consulta.
            qry_txt = tabInfo.overlapsQryText(param, out paramNames);

            using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx))
            {
                //Asigna parametros.
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

                TdDataReader rdr = cmd.ExecuteReader();
                int rec_cnt = rdr.RecordsReturned;
                cmd.Dispose();

                if (rec_cnt != 0)
                {
                    // Si hay registros de salida, creo la lista,...
                    recordList = new List<Dictionary<String, String>>();

                    // y la lleno.
                    while (rdr.Read())
                    {
                        Dictionary<String, String> rec = new Dictionary<String, String>();
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            rec.Add(rdr.GetName(i), rdr.IsDBNull(i) ? "" : rdr.GetString(i).ToString());
                        }
                        recordList.Add(rec);
                    }
                }
                rdr.Dispose();
                rdr.Close();
            }
            return recordList;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected int splitOverlappingPaceFollower(JObject param, CfDbTable tabInfo, TdConnection cntn, TdTransaction trx)
        {
            String qry_txt;
            String[] paramNames;
            int rows_affected = 0;

            if (tabInfo.isReadOnly)
            {
                throw new Exception("Invalid operation on a readonly table.");
            }

            //Variables para query upsert.
            //String qry_txt;
            //String[] paramNames;

            List<Dictionary<String, String>> overlappingRecs = this.getOverlappingPaceFollower(param, tabInfo, cntn, trx);

            if (overlappingRecs != null)
            {
                // Tomo string de upsert.
                if (tabInfo.isHist)
                {
                    qry_txt = tabInfo.mergeHistQryText(out paramNames);
                }
                else
                {
                    qry_txt = tabInfo.mergeQryText(out paramNames);
                }

                // No hace falta chequear foreing keys porque los registros ya estan en la base.
                using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx))
                {
                    //Crea parametros.
                    foreach (String pName in paramNames)
                    {
                        TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                        cmd.Parameters.Add(tp);
                    }
                    //Inserta cada elemento del array.
                    foreach (Dictionary<String, String> rec in overlappingRecs)
                    {
                        //Asigna parametros.
                        foreach (TdParameter tp in cmd.Parameters)
                        {
                            String value;
                            if (tp.ParameterName == tabInfo.TrkEvent_Column.ColName)
                            {
                                value = Request.Properties.ContainsKey("RepEvent_id") ? Request.Properties["RepEvent_id"].ToString() : "";
                            }
                            else if (tp.ParameterName == tabInfo.TrkUser_Column.ColName)
                            {
                                value = Request.Properties.ContainsKey("RepUser_id") ? Request.Properties["RepUser_id"].ToString() : "nicolas_oliveto";
                            }
                            else if (!rec.TryGetValue(tp.ParameterName, out value))
                            {
                                throw new Exception("Falta informar campo '" + tp.ParameterName + "'.");
                            }

                            tp.Value = value;
                        }

                        String cmdTimeOutStr = Parametros().strTimeOut;
                        int cmdTimeOut;

                        if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                        {
                            cmd.CommandTimeout = cmdTimeOut;
                        }

                        rows_affected += cmd.ExecuteNonQuery();
                    }
                    //Libera recursos de base de datos.
                    cmd.Dispose();
                }
            }
            return rows_affected;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected List<Dictionary<String, String>> getOverlappingPaceFollowing(JObject param, CfDbTable tabInfo, TdConnection cntn, TdTransaction trx)
        {
            //Variables para query que lista registros fuente.
            String qry_txt;
            String[] paramNames;

            List<Dictionary<String, String>> recordList = null;

            // Tomo string de consulta.
            qry_txt = tabInfo.overlapsQryText(param, out paramNames);

            using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx))
            {
                //Asigna parametros.
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

                TdDataReader rdr = cmd.ExecuteReader();
                int rec_cnt = rdr.RecordsReturned;
                cmd.Dispose();

                if (rec_cnt != 0)
                {
                    // Si hay registros de salida, creo la lista,...
                    recordList = new List<Dictionary<String, String>>();

                    // y la lleno.
                    while (rdr.Read())
                    {
                        Dictionary<String, String> rec = new Dictionary<String, String>();
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            rec.Add(rdr.GetName(i), rdr.IsDBNull(i) ? "" : rdr.GetString(i).ToString());
                        }
                        recordList.Add(rec);
                    }
                }
                rdr.Dispose();
                rdr.Close();
            }
            return recordList;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected int splitOverlappingPaceFollowing(JObject param, CfDbTable tabInfo, TdConnection cntn, TdTransaction trx)
        {
            String qry_txt;
            String[] paramNames;
            int rows_affected = 0;

            if (tabInfo.isReadOnly)
            {
                throw new Exception("Invalid operation on a readonly table.");
            }

            //Variables para query upsert.
            //String qry_txt;
            //String[] paramNames;

            List<Dictionary<String, String>> overlappingRecs = this.getOverlappingPaceFollowing(param, tabInfo, cntn, trx);

            if (overlappingRecs != null)
            {
                // Tomo string de upsert.
                if (tabInfo.isHist)
                {
                    qry_txt = tabInfo.mergeHistQryText(out paramNames);
                }
                else
                {
                    qry_txt = tabInfo.mergeQryText(out paramNames);
                }

                // No hace falta chequear foreing keys porque los registros ya estan en la base.
                using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx))
                {
                    //Crea parametros.
                    foreach (String pName in paramNames)
                    {
                        TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                        cmd.Parameters.Add(tp);
                    }
                    //Inserta cada elemento del array.
                    foreach (Dictionary<String, String> rec in overlappingRecs)
                    {
                        //Asigna parametros.
                        foreach (TdParameter tp in cmd.Parameters)
                        {
                            String value;
                            if (!rec.TryGetValue(tp.ParameterName, out value))
                            {
                                throw new Exception("Falta informar campo '" + tp.ParameterName + "'.");
                            }
                            tp.Value = value;
                        }

                        String cmdTimeOutStr = Parametros().strTimeOut;
                        int cmdTimeOut;

                        if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                        {
                            cmd.CommandTimeout = cmdTimeOut;
                        }

                        rows_affected += cmd.ExecuteNonQuery();
                    }

                    //Libera recursos de base de datos.
                    cmd.Dispose();
                }
            }
            return rows_affected;
        }
        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected int deleteOverlappingPaceFollower(JObject param, CfDbTable tabInfo, TdConnection cntn, TdTransaction trx)
        {
            String qry_txt;
            String[] paramNames;

            int rows_affected = 0;

            if (tabInfo.isReadOnly)
            {
                throw new Exception("Invalid operation on a readonly table.");
            }

            //Variables para query upsert.
            //String qry_txt;
            //String[] paramNames;

            List<Dictionary<String, String>> overlappingRecs = this.getOverlappingPaceFollower(param, tabInfo, cntn, trx);

            if (overlappingRecs != null)
            {
                // Tomo string de upsert.
                if (tabInfo.isHist)
                {
                    qry_txt = tabInfo.deleteHistQryText(out paramNames);
                }
                else
                {
                    qry_txt = tabInfo.deleteQryText(out paramNames);
                }

                // No hace falta chequear foreing keys porque los registros ya estan en la base.
                using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx))
                {
                    //Crea parametros.
                    foreach (String pName in paramNames)
                    {
                        TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                        cmd.Parameters.Add(tp);
                    }
                    //Inserta cada elemento del array.
                    foreach (Dictionary<String, String> rec in overlappingRecs)
                    {
                        //Asigna parametros.
                        foreach (TdParameter tp in cmd.Parameters)
                        {
                            String value;
                            if (!rec.TryGetValue(tp.ParameterName, out value))
                            {
                                throw new Exception("Falta informar campo '" + tp.ParameterName + "'.");
                            }
                            tp.Value = value;
                        }

                        String cmdTimeOutStr = Parametros().strTimeOut;
                        int cmdTimeOut;

                        if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                        {
                            cmd.CommandTimeout = cmdTimeOut;
                        }

                        rows_affected += cmd.ExecuteNonQuery();
                    }

                    //Libera recursos de base de datos.
                    cmd.Dispose();
                }
            }
            return rows_affected;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected List<JObject> listByCondition(JObject param, CfDbTable tabInfo, TdConnection cntn, TdTransaction trx, int maxRecCnt)
        {
            //Variables para query que lista registros fuente.
            String qry_txt;

            List<JObject> recordList = null;
            JObject assembly = new JObject();
            JArray japaram = new JArray();

            JArray cond_list = (JArray)param["Where"]; //Lee una property del objeto que es un array.

            if (cond_list != null)
            {
                foreach (JObject cond in cond_list)
                {
                    foreach (JProperty condFld in cond.Properties())
                    {
                        String fldName = condFld.Name;

                        if (!fldName.Equals(tabInfo.StartDt_Column.ColName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            JArray paramParam = (JArray)condFld.Value;
                            JProperty prop = new JProperty(fldName);
                            JObject joparam = new JObject();
                            joparam.Add(prop);
                            joparam.Property(fldName).Value = paramParam;
                            japaram.Add(joparam);
                        }
                    }
                }
            }
            assembly.Add("Where", japaram);

            // Tomo string de consulta.
            qry_txt = tabInfo.listQryText(assembly, CfDbTable.ListOption.LIST_CURRENT_AND_FUTURE, CfDbTable.OrderByOption.WITHOUT_ORDER_BY, CfDbTable.LockByOption.WITHOUT_LOCKING, CfDbTable.ColumnListOption.DEFAULT_COLUMNS);

            using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn, trx))
            {
                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                TdDataReader rdr = cmd.ExecuteReader();
                int rec_cnt = rdr.RecordsReturned;
                cmd.Dispose();

                if (rec_cnt != 0)
                {
                    // Si hay registros de salida, creo la lista,...
                    recordList = new List<JObject>();

                    // y la lleno.
                    int readRecCnt = 0;
                    while (readRecCnt++ <= maxRecCnt && rdr.Read())
                    {
                        JObject rec = new JObject();
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            rec.Add(rdr.GetName(i), rdr.IsDBNull(i) ? "" : rdr.GetString(i).ToString());
                        }
                        recordList.Add(rec);
                    }
                }
                rdr.Dispose();
                rdr.Close();
            }
            return recordList;
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected int deleteByCondition(JObject param, CfDbTable tabInfo, TdConnection cntn)
        {
            String qry_txt;
            String[] paramNames;

            int rows_affected = 0;

            if (tabInfo.isReadOnly)
            {
                throw new Exception("Invalid operation on a readonly table.");
            }

            // Tomo string de delete.
            if (tabInfo.isHist)
            {
                qry_txt = tabInfo.deleteHistQryText(out paramNames);
            }
            else
            {
                qry_txt = tabInfo.deleteQryText(out paramNames);
            }

            TdCommand cmd = null;
            TdCommand[] riCmdArray;
            String[] relatedTableArray;
            String[] childTabLocalArray;
            List<String[]> childColLocalList;
            List<String[]> childColList;

            TdTransaction trx = cntn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                this.prepareChildRI(tabInfo, cntn, out riCmdArray, out relatedTableArray, out childTabLocalArray, out childColLocalList, out childColList);

                using (cmd = new TdCommand(qry_txt, cntn, trx))
                {
                    //Crea parametros.
                    foreach (String pName in paramNames)
                    {
                        TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                        cmd.Parameters.Add(tp);
                    }

                    //Leo primer tanda de registros.
                    List<JObject> toDeleteRecs = this.listByCondition(param, tabInfo, cntn, trx, 500);
                    while (toDeleteRecs != null && toDeleteRecs.Count > 0)
                    {
                        //Inserta cada elemento del array.
                        foreach (JObject rec in toDeleteRecs)
                        {

                            applyChildRI(tabInfo, rec, cntn, riCmdArray, trx, relatedTableArray, childTabLocalArray, childColLocalList, childColList);

                            //Asigna parametros.
                            foreach (TdParameter tp in cmd.Parameters)
                            {
                                JProperty prop = rec.Property(tp.ParameterName);
                                if (prop == null)
                                {
                                    throw new Exception("Falta informar campo '" + tp.ParameterName + "'.");
                                }
                                tp.Value = prop.Value.ToString();
                            }

                            foreach (TdParameter tp in cmd.Parameters)
                            {
                                if (tp.ParameterName.Equals(tabInfo.StartDt_Column.ColName,StringComparison.CurrentCultureIgnoreCase))
                                {
                                    tp.Value = DateTime.Today.ToString("yyyy-MM-dd");
                                }
                            }

                            String cmdTimeOutStr = Parametros().strTimeOut;
                            int cmdTimeOut;

                            if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                            {
                                cmd.CommandTimeout = cmdTimeOut;
                            }

                            rows_affected += cmd.ExecuteNonQuery();

                            //Procesa pace followers.
                            if (tabInfo.paceFollowerTabs != null)
                            {
                                foreach (CfDbTable tbl in tabInfo.paceFollowerTabs)
                                {
                                    rows_affected += deleteOverlappingPaceFollower(param, tbl, cntn, trx);
                                }
                            }
                        }
                        //Leo siguiente tanda de registros.
                        toDeleteRecs = this.listByCondition(param, tabInfo, cntn, trx, 500);
                    }
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

            return rows_affected;
        }
        ////////////////////////////////////////////////////////////////////////////////
        // Prepara un array de commands para verificar integridad.
        [NonAction]
        protected void prepareParentRI(CfDbTable tabInfo, TdConnection cntn, out TdCommand[] cmdArray, out String[] parentTabArray,
                out String[] parentTabLocalArray, out List<String[]> parentColLocalList, out List<String[]> parentColList)
        {
            int parentIdx;
            int parentCnt = tabInfo.getParentTablesCnt();

            // Crea arrays de salida.
            cmdArray = new TdCommand[parentCnt]; //Comando para ejecutar query.
            parentTabArray = new String[parentCnt]; //Lista de tablas padre.
            parentTabLocalArray = new String[parentCnt]; //Lista de tablas padre en castellano.
            parentColLocalList = new List<String[]>(); //Lista de arrays de columnas de tablas padre en castellano.
            parentColList = new List<String[]>(); //Lista de arrays de columnas de tablas padre.

            // Verifica TODAS las foreing key. Si alguna falla se genera una excepcion.
            for (parentIdx = 0; parentIdx < parentCnt; parentIdx++)
            {
                String qry_txt;
                String parentTableName;
                String parentTableLocalName;
                String[] paramNames;
                String[] paramLocalNames;

                qry_txt = tabInfo.riParentCheckQryText(parentIdx, out parentTableName, out paramNames, out parentTableLocalName, out paramLocalNames);

                cmdArray[parentIdx] = new TdCommand(qry_txt, cntn);
                parentTabArray[parentIdx] = parentTableName;
                parentTabLocalArray[parentIdx] = parentTableLocalName;
                parentColLocalList.Add(paramLocalNames);
                parentColList.Add(paramNames);

                //Armo array con posiciones de parametros.
                int parIdx = 0;
                foreach (String pName in paramNames)
                {
                    TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                    cmdArray[parentIdx].Parameters.Add(tp);
                    parIdx++;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void applyParentRI(CfDbTable tabInfo, JObject theRow, TdConnection cntn, TdCommand[] cmdArray, TdTransaction trx, String[] parentTabArray,
                String[] parentTabLocalArray, List<String[]> parentColLocalList, List<String[]> parentColList)
        {
            int parentIdx;
            int parentCnt = tabInfo.getParentTablesCnt();
            String excMsg = "";
            String excSep = "";

            // Verifica TODAS las foreing key. Si alguna falla se genera una excepcion.
            for (parentIdx = 0; parentIdx < parentCnt; parentIdx++)
            {
                //String[] paramNames;

                //qry_txt = tabInfo.riParentCheckQryText(parentIdx, out parentTableName, out paramNames);

                TdCommand cmd = cmdArray[parentIdx];
                cmd.Connection = cntn;
                cmd.Transaction = trx;
                String parentTableName = parentTabArray[parentIdx];
                String parentTableLocalName = parentTabLocalArray[parentIdx];
                String[] colLocalName = parentColLocalList[parentIdx];
                String[] colName = parentColList[parentIdx];

                foreach (TdParameter cmdParam in cmd.Parameters)
                {
                    JProperty prop = theRow.Property(cmdParam.ParameterName);
                    if (prop == null)
                    {
                        //Esto no deberia pasar, pero conviene verificar. Gerardo.
                        throw new Exception("Falta columna " + cmdParam.ParameterName + " en registro a verificar.");
                    }
                    cmdParam.Value = prop.Value.ToString();
                }

                //Ejecuto
                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                TdDataReader rdr = cmd.ExecuteReader();
                //cmd.Dispose();

                if (!rdr.HasRows)  // o podria ser con rdr.RecordsReturned()
                {
                    //Si no encuentra registro padre, la transaccion no puede llevarse a cabo.

                    // Armo lista de claves y valores.                       
                    String keyList = "";
                    String keySep = "";
                    String valList = "";
                    String valSep = "";

                    for (int i = 0; i < cmd.Parameters.Count; i++)
                    {
                        keyList += keySep;
                        //keyList += cmd.Parameters[i].ParameterName;
                        int indexpos = Array.IndexOf(colName, cmd.Parameters[i].ParameterName);
                        if (indexpos != -1)
                        {
                            keyList += colLocalName[indexpos];
                        }
                        else
                        {
                            throw new Exception("Campo inexistente: " + cmd.Parameters[i].ParameterName);
                        }
                        keySep = ",";

                        valList += valSep;
                        valList += "'";
                        valList += cmd.Parameters[i].Value;
                        valList += "'";
                        valSep = ",";
                    }

                    // Armo mensaje de salida.          
                    excMsg += excSep;
                    excMsg += "No existe registro en ";
                    //excMsg += parentTableName;
                    excMsg += parentTableLocalName;
                    excMsg += " con clave (";
                    excMsg += keyList;
                    excMsg += ") = (";
                    excMsg += valList;
                    excMsg += ").";
                    excSep = "\n";
                }
                //cmd.Dispose();
                rdr.Dispose();
                rdr.Close();
            }
            if (!excMsg.Equals("")) // Si fallo alguna columna.
            {
                throw new Exception(excMsg);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        // Prepara un array de commands para verificar integridad.
        [NonAction]
        protected void prepareChildRI(CfDbTable tabInfo, TdConnection cntn, out TdCommand[] cmdArray, out String[] childTabArray,
            out String[] childTabLocalArray, out List<String[]> childColLocalList, out List<String[]> childColList)
        {
            int childIdx;
            int childCnt = tabInfo.getChildTablesCnt();

            // Crea arrays de salida.
            cmdArray = new TdCommand[childCnt]; //Comando para ejecutar query.
            childTabArray = new String[childCnt]; //Lista de tablas hijas.
            childTabLocalArray = new String[childCnt]; //Lista de tablas hijas en castellano.
            childColLocalList = new List<String[]>(childCnt); //Lista de arrays de nombres de columnas de tablas hijas en castellano.
            childColList = new List<String[]>(childCnt); //Lista de arrays de nombres de columnas de tablas hijas.

            // Verifica TODAS las hijas. Si alguna tiene registros se genera una excepcion.
            for (childIdx = 0; childIdx < childCnt; childIdx++)
            {
                String qry_txt;
                String childTableName;
                String childTableLocalName;
                String[] paramNames;
                String[] paramLocalNames;

                qry_txt = tabInfo.riChildCheckQryText(childIdx, out childTableName, out paramNames, out childTableLocalName, out paramLocalNames);

                cmdArray[childIdx] = new TdCommand(qry_txt, cntn);
                childTabArray[childIdx] = childTableName;
                childTabLocalArray[childIdx] = childTableLocalName;
                childColList.Add(paramNames);
                childColLocalList.Add(paramLocalNames);

                //Armo array con posiciones de parametros.
                int parIdx = 0;
                foreach (String pName in paramNames)
                {
                    TdParameter tp = new TdParameter(pName, System.Data.DbType.String);
                    cmdArray[childIdx].Parameters.Add(tp);
                    parIdx++;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        [NonAction]
        protected void applyChildRI(CfDbTable tabInfo, JObject theRow, TdConnection cntn, TdCommand[] cmdArray, TdTransaction trx, String[] childTabArray,
                    String[] childTabLocalArray, List<String[]> childColLocalList, List<String[]> childColList)
        {
            int childIdx;
            int childCnt = tabInfo.getChildTablesCnt();
            String excMsg = "";
            String excSep = "";

            // Verifica TODAS las foreing key. Si alguna falla se genera una excepcion.
            for (childIdx = 0; childIdx < childCnt; childIdx++)
            {
                //String[] paramNames;

                //qry_txt = tabInfo.riParentCheckQryText(parentIdx, out parentTableName, out paramNames);

                TdCommand cmd = cmdArray[childIdx];
                cmd.Connection = cntn;
                cmd.Transaction = trx;
                String childTableName = childTabArray[childIdx];
                String childTableLocalName = childTabLocalArray[childIdx];
                String[] colLocalName = childColLocalList[childIdx];
                String[] colName = childColList[childIdx];

                foreach (TdParameter cmdParam in cmd.Parameters)
                {
                    JProperty prop = theRow.Property(cmdParam.ParameterName);
                    if (prop == null)
                    {
                        //Esto no deberia pasar, pero conviene verificar. Gerardo.
                        throw new Exception("Falta columna " + cmdParam.ParameterName + " en registro a verificar.");
                    }
                    cmdParam.Value = prop.Value.ToString();
                }

                //Ejecuto
                String cmdTimeOutStr = Parametros().strTimeOut;
                int cmdTimeOut;

                if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                {
                    cmd.CommandTimeout = cmdTimeOut;
                }

                TdDataReader rdr = cmd.ExecuteReader();
                //cmd.Dispose();

                if (rdr.HasRows)  // o podria ser con rdr.RecordsReturned()
                {
                    //Si encuentra registros hijos, la transaccion no puede llevarse a cabo.

                    // Armo lista de claves y valores.                       
                    String keyList = "";
                    String keySep = "";
                    String valList = "";
                    String valSep = "";

                    for (int i = 0; i < cmd.Parameters.Count; i++)
                    {
                        keyList += keySep;
                        //keyList += cmd.Parameters[i].ParameterName;
                        int indexpos = Array.IndexOf(colName, cmd.Parameters[i].ParameterName);
                        if (indexpos != -1)
                        {
                            keyList += colLocalName[indexpos];
                        }
                        else
                        {
                            throw new Exception("Campo inexistente: " + cmd.Parameters[i].ParameterName);
                        }
                        keySep = ",";

                        valList += valSep;
                        valList += "'";
                        valList += cmd.Parameters[i].Value;
                        valList += "'";
                        valSep = ",";
                    }

                    // Armo mensaje de salida.          
                    excMsg += excSep;
                    excMsg += "Existe registro en ";
                    //excMsg += childTableName;
                    excMsg += childTableLocalName;
                    excMsg += " con clave (";
                    excMsg += keyList;
                    excMsg += ") = (";
                    excMsg += valList;
                    excMsg += ").";
                    excSep = "\n";
                }
                rdr.Dispose();
                rdr.Close();
            }
            if (!excMsg.Equals("")) // Si fallo alguna columna.
            {
                throw new Exception(excMsg);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        protected void setEventProperty(JObject param, CfDbColumn c, String requestPropertyName)
        {
            if (c != null)
            {
                JProperty prop = param.Property(c.ColName);

                if (prop != null)
                {
                    prop.Value = Request.Properties[requestPropertyName].ToString();
                }
                else
                {
                    param.Add(c.ColName, Request.Properties[requestPropertyName].ToString());
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////
    }
}
