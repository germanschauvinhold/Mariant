using System;

namespace WebAsistida.lib
{
    public class CfDbColumn
    {
        public CfDbTable DbTable;
        //public String DbName; //Deprecated
        //public String TbName; //Deprecated
        public String ColName;
        public String DataType;
        public bool isPk;
        public bool isFk;
        public bool isReq;

        public bool readOnly;
        public bool isStartDt;
        public bool isEndDt;
        public bool isDesc;
        public String ColLocalName;

        public bool isNumber;

        public String DefaultValue;
        public Func<String, bool> CheckFunc;
        public String CheckTxt;

        // Tracking
        public bool isTrkEvent;
        public bool isTrkUser;
        public bool isTrkDt;
        public bool isTrkTm;

        public bool isRequiredOnImport; //Es requerida cuando se importa archivo? Gerardo 2014-12-03.
        public bool isFillOnImport; //Es completado el campo cuando se requiere Nicolas 2015-09-21.


        public CfDbColumn( CfDbTable _DbTable, String _ColName, String _DataType,
                            bool _isPk, bool _isFk, bool _isReq, bool _isReadOnly = false,
                            bool _isStartDt = false, bool _isEndDt = false, bool _isDesc = false,
                            String _localName = null,
                            String _DefaultValue = null,
                            String _CheckTxt = null,
                            bool _isTrkEvent = false, bool _isTrkUser = false, bool _isTrkDt = false, bool _isTrkTm = false, // Tracking flags.
                            bool _isRequiredOnImport = true,
                            bool _isFillOnImport = false
            )
        {
            DbTable = _DbTable;
            //DbName = _DbTable.DbName;
            //TbName = _DbTable.TbName;
            ColName = _ColName;
            DataType = _DataType;
            isPk = _isPk;
            isFk = _isFk;
            isReq = _isReq;
            readOnly = _isReadOnly;
            isStartDt = _isStartDt;
            isEndDt = _isEndDt;
            isDesc = _isDesc;

            if (_localName == null || _localName.Equals(""))
            {
                throw new Exception("Falta nombre local para columna " + DbTable.DbName + "." + DbTable.TbName + "." + ColName);
            }
            else
            {
                ColLocalName = _localName;
            }

            if (
                _DataType.StartsWith("BYTEINT", StringComparison.CurrentCultureIgnoreCase) ||
                _DataType.StartsWith("SMALLINT", StringComparison.CurrentCultureIgnoreCase) ||
                _DataType.StartsWith("DECIMAL", StringComparison.CurrentCultureIgnoreCase) ||
                _DataType.StartsWith("NUMERIC", StringComparison.CurrentCultureIgnoreCase) ||
                _DataType.StartsWith("INTEGER", StringComparison.CurrentCultureIgnoreCase) ||
                _DataType.StartsWith("BIGINT", StringComparison.CurrentCultureIgnoreCase)
            )
            {
                isNumber = true;
            }
            else if (
                _DataType.StartsWith("DATE", StringComparison.CurrentCultureIgnoreCase) ||
                _DataType.StartsWith("TIME", StringComparison.CurrentCultureIgnoreCase) ||
                _DataType.StartsWith("VARCHAR", StringComparison.CurrentCultureIgnoreCase) ||
                _DataType.StartsWith("CHAR", StringComparison.CurrentCultureIgnoreCase) ||
                _DataType.StartsWith("CLOB", StringComparison.CurrentCultureIgnoreCase)
            )
            {
                isNumber = false;
            }
            else
            {
                throw new Exception("Tipo de dato no contemplado: " + _DataType + ".");
            }

            DefaultValue = _DefaultValue;
            CheckFunc = null;
            CheckTxt = _CheckTxt;

            //Tacking.

            isTrkEvent = _isTrkEvent;
            isTrkUser = _isTrkUser;
            isTrkDt = _isTrkDt;
            isTrkTm = _isTrkTm;

            isRequiredOnImport = _isRequiredOnImport;
            isFillOnImport = _isFillOnImport;
        }


        public CfDbColumn(CfDbColumn col)
        {
            this.DbTable = col.DbTable;
            this.ColName = col.ColName;
            this.DataType = col.DataType;
            this.isPk = col.isPk;
            this.isFk = col.isFk;
            this.isReq = col.isReq;
            this.readOnly = col.readOnly;
            this.isStartDt = col.isStartDt;
            this.isEndDt = col.isEndDt;
            this.isDesc = col.isDesc;
            this.ColLocalName = col.ColLocalName;
            this.isNumber = col.isNumber;
            this.DefaultValue = col.DefaultValue;
            this.CheckFunc = col.CheckFunc;
            this.CheckTxt = col.CheckTxt;
            this.isTrkEvent = col.isTrkEvent;
            this.isTrkUser = col.isTrkUser;
            this.isTrkDt = col.isTrkDt;
            this.isTrkTm = col.isTrkTm;
            this.isRequiredOnImport = col.isRequiredOnImport;
            this.isFillOnImport = col.isFillOnImport;
        }
    
    
    }


}