using WebAsistida.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace WebAsistida.Filters
{
    public class AuthorizeActivityAttribute : AuthorizationFilterAttribute, IAuthorizationFilter 
    {
        /*
        private class RoleActivity
        {
            String activityName;
            String roleName;

            public RoleActivity(String _activityName, String _roleName)
            {
                activityName = _activityName;
                roleName = _roleName;
            }
        }

        static HashSet<RoleActivity> roleActivitySet;
         */
        public static Parameters Parametros()
        {
            Parameters Parametros = new Parameters();

            Parametros.Parametros();

            return Parametros;
        }

        static Dictionary<String, List<String>> _Activity2RoleList;

        static AuthorizeActivityAttribute()
        {
            //Comentar para provar en casa
            _Activity2RoleList = getRoleActivity();
        }

        static Dictionary<String, List<String>> getRoleActivity()
        {
            String conn_str;
            String qry_txt;
            //HashSet<RoleActivity> newRoleActivitySet;
            String dbschema = Parametros().strDbSchema;

            Dictionary<String, List<String>> newActivity2RoleList;

            qry_txt = "SELECT\n";
            qry_txt += "  RepRole_id,\n";
            qry_txt += "  RepActivity_id\n";
            qry_txt += "FROM\n";
            qry_txt += dbschema + ".V_CurrentRoleActivity\n";
            qry_txt += "ORDER BY\n";
            qry_txt += "  RepActivity_id,\n";
            qry_txt += "  RepRole_id\n";

            conn_str = Parametros().strTeradata;

            using (Teradata.Client.Provider.TdConnection cntn = new Teradata.Client.Provider.TdConnection(conn_str))
            {
                cntn.Open();

                /*
                // Tomo string de consulta.
                String qry_txt = tabInfo.listQryText(param);
                */

                using (Teradata.Client.Provider.TdCommand cmd = new Teradata.Client.Provider.TdCommand(qry_txt, cntn))
                {
                    String cmdTimeOutStr = Parametros().strTimeOut;
                    int cmdTimeOut;

                    if (int.TryParse(cmdTimeOutStr, out cmdTimeOut))
                    {
                        cmd.CommandTimeout = cmdTimeOut;
                    }

                    Teradata.Client.Provider.TdDataReader rdr = cmd.ExecuteReader();

                    //newRoleActivitySet = new HashSet<RoleActivity>();
                    newActivity2RoleList = new Dictionary<String, List<String>>();

                    String _RepRole_id;
                    String _RepActivity_id;
                    String Last_RepActivity_id = "";
                    //LinkedList<String> currRoleList = null;
                    List<String> currRoleList = null;
                    while (rdr.Read())
                    {
                        _RepRole_id = rdr.GetString(0).ToString().ToLower();
                        _RepActivity_id = rdr.GetString(1).ToString().ToLower();

                        if ( ! Last_RepActivity_id.Equals(_RepActivity_id))
                        {
                            currRoleList = new List<String>();
                            newActivity2RoleList.Add(_RepActivity_id, currRoleList);
                            Last_RepActivity_id = _RepActivity_id;
                        }
                        currRoleList.Add(_RepRole_id);
                    }
                }
            }
            return newActivity2RoleList;
        }

        //public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            String actionName = actionContext.ActionDescriptor.ActionName.ToLower();
            String controllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            GenericPrincipal gprincipal = (GenericPrincipal)Thread.CurrentPrincipal;

            String _RepActivity_id = controllerName + "/" + actionName;

            List<String> currRoleList;
            if (_Activity2RoleList.TryGetValue(_RepActivity_id, out currRoleList))
            {
                for (int i = 0; i < currRoleList.Count(); i++)
                {
                    if (gprincipal.IsInRole(currRoleList[i]))
                    {
                        return;
                    }
                }
            }
            //Comentar para provar en casa
            throw new UnauthorizedAccessException("Usuario " + gprincipal.Identity.Name + " no tiene permiso para invocar " + _RepActivity_id + ".");
        }



        // ASINCRONICO

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            await InternalActionExecuting(actionContext, cancellationToken);

            if (actionContext.Response != null)
            {
                return actionContext.Response;
            }

            HttpActionExecutedContext executedContext;

            try
            {
                var response = await continuation();
                executedContext = new HttpActionExecutedContext(actionContext, null)
                {
                    Response = response
                };
            }
            catch (Exception exception)
            {
                executedContext = new HttpActionExecutedContext(actionContext, exception);
            }

            await InternalActionExecuted(executedContext, cancellationToken);
            return executedContext.Response;
        }

        public  virtual Task InternalActionExecuting(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            String actionName = actionContext.ActionDescriptor.ActionName.ToLower();
            String controllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            GenericPrincipal gprincipal = (GenericPrincipal)Thread.CurrentPrincipal;

            String _RepActivity_id = controllerName + "/" + actionName;

            List<String> currRoleList;
            if (_Activity2RoleList.TryGetValue(_RepActivity_id, out currRoleList))
            {
                for (int i = 0; i < currRoleList.Count(); i++)
                {
                    if (gprincipal.IsInRole(currRoleList[i]))
                    {
                        return null; //no va el null aca
                    }
                }
            }
            //Comentar para provar en casa
            throw new UnauthorizedAccessException("Usuario " + gprincipal.Identity.Name + " no tiene permiso para invocar " + _RepActivity_id + ".");
    
            //pre execution hook
        }

        public virtual Task InternalActionExecuted(HttpActionExecutedContext actionExecutedContext,
            CancellationToken cancellationToken)
        {
            return null;
            //post execution hook
        }

        //para validar metodos de carga/borrado
        public static void UploadAuthorization(HttpRequestMessage Request, HttpControllerContext ControllerContext)
        {
            IEnumerable<string> auth_coll = null;
            IPrincipal parsedPrincipal = null;
            String RepUser_id = Request.Properties.ContainsKey("RepUser_id") ? Request.Properties["RepUser_id"].ToString() : null;

            if (Request.Headers.Contains("CfApiAuthorization"))
            {
                auth_coll = Request.Headers.GetValues("CfApiAuthorization");
            }

            if (auth_coll != null)
            {
                IEnumerator<string> auth_enum = auth_coll.GetEnumerator();
                auth_enum.MoveNext();
                String cryptAuthToken = auth_enum.Current;
                String authVal = CryptEngine.Decrypt(cryptAuthToken, true);

                if (authVal != null)
                {
                    String[] credentials = authVal.Split(new[] { ':' });
                    String rolestring = credentials[2];
                    String[] RepSessionRoles = rolestring.Split(new[] { ',' });

                    GenericIdentity identity = new GenericIdentity(RepUser_id, "Custom");
                    parsedPrincipal = new GenericPrincipal(identity, RepSessionRoles);
                    Thread.CurrentPrincipal = parsedPrincipal;
                }
            }

            String actionName = ControllerContext.RouteData.Values["action"].ToString().ToLower();
            String controllerName = ControllerContext.RouteData.Values["controller"].ToString().ToLower(); ;
            GenericPrincipal gprincipal = (GenericPrincipal)Thread.CurrentPrincipal;

            String _RepActivity_id = controllerName + "/" + actionName;

            List<String> currRoleList;
            if (_Activity2RoleList.TryGetValue(_RepActivity_id, out currRoleList))
            {
                for (int i = 0; i < currRoleList.Count(); i++)
                {
                    if (gprincipal.IsInRole(currRoleList[i]))
                    {
                        return;
                    }
                }
            }

            throw new UnauthorizedAccessException("Usuario " + gprincipal.Identity.Name + " no tiene permiso para invocar " + _RepActivity_id + ".");
        }
    }
}