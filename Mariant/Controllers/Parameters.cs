using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

//Falta documentacion sobre el objeto y cada uno de los metodos.
namespace WebAsistida.lib
{
    public class Parameters
    {
        public Parameters(){}

        public string xmlfile { get; set; }
        public string strTeradataLoad { get; set; }
        public string strTeradata { get; set; }
        public string strTimeOut { get; set; }
        public string strDbSchema { get; set; }
        public string strDbUserFld { get; set; }
        public string strDbPassFld { get; set; }
        public string strIpConfig { get; set; }
        public string strInformix { get; set; }


        public Parameters Parametros()
        {
            if (File.Exists(@"C:\inetpub\wwwroot\reposicionasistida\Web.config"))
            {
                this.xmlfile = @"C:\inetpub\wwwroot\reposicionasistida\Web.config";
            }
            else if (File.Exists(@"C:\VS2012_Projects\ReposicionAsistida\ReposicionAsistida\Web.config"))
            {
                this.xmlfile = @"C:\VS2012_Projects\ReposicionAsistida\ReposicionAsistida\Web.config";
            }
            else if (File.Exists(@"C:\Users\NO255003\Documents\Projects\ReposicionAsistida\ReposicionAsistida\Web.config"))
            {
                this.xmlfile = @"C:\Users\NO255003\Documents\Projects\ReposicionAsistida\ReposicionAsistida\Web.config";
            }
            else
            {
                throw new Exception("No se puede encontrar archivo de configuracion.");
            }

            this.XMLParser("/configuration/applicationSettings/ReposicionAsistida.Properties.Settings", "setting", "name");
            return this;
        }

        private void XMLParser(string parentnode, string nodename, string idname)
        {
            XmlDocument XmlFile = new XmlDocument();
            XmlFile.Load(this.xmlfile);

            XmlNode ParameterListNode =
                XmlFile.SelectSingleNode(parentnode);
            XmlNodeList ParameterNodeList =
                ParameterListNode.SelectNodes(nodename);

            foreach (XmlNode node in ParameterNodeList)
            {
                switch (node.Attributes.GetNamedItem(idname).Value)
                {
                    case "DW_TERADATA_LOADER":
                        this.strTeradataLoad = node.InnerText;
                        break;
                    case "DW_TERADATA":
                        this.strTeradata = node.InnerText;
                        break;
                    case "DB_CMD_TIMEOUT":
                        this.strTimeOut = node.InnerText;
                        break;
                    case "DB_SCHEMA":
                        this.strDbSchema = node.InnerText;
                        break;
                    case "DB_USER_FLD":
                        this.strDbUserFld = node.InnerText;
                        break;
                    case "DB_PASS_FLD":
                        this.strDbPassFld = node.InnerText;
                        break;
                    case "IPCONFIG":
                        this.strIpConfig = node.InnerText;
                        break;
                    case "DW_INFORMIX_LOADER":
                        this.strInformix = node.InnerText;
                        break;
                }
            }
        }
    }
}