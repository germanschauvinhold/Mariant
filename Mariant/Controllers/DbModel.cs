/******************************************************************************
* Module       = 
* Version      = v23
* Date Time    = Jueves, 06 de Octubre de 2016 03:58:04 a.m.
* Ident        = 
* Author       = German Schauvinhold
* Company      = Mariant.
* Description  = Database model metadata.     
*******************************************************************************/
using System;
using System.Configuration;


namespace WebAsistida.lib
{
    public class DbModel
    {
        public static readonly CfDbTable _AuxTableCreationEventTrack;
        public static readonly CfDbTable _AuxTableEventTrack;
        public static readonly CfDbTable AuxiliarProductEventFilter;
        public static readonly CfDbTable HttpMethod;
        public static readonly CfDbTable HttpStatus;
        public static readonly CfDbTable ProductBarCodeHist;
        public static readonly CfDbTable WebActivity;
        public static readonly CfDbTable WebDbObject;
        public static readonly CfDbTable WebDbObjectPrecedent;
        public static readonly CfDbTable WebDbObjectProcess;
        public static readonly CfDbTable WebDbObjectQueue;
        public static readonly CfDbTable WebEvent;
        public static readonly CfDbTable WebEventMutex;
        public static readonly CfDbTable WebFileProcessEvent;
        public static readonly CfDbTable WebFileProcessGroupQuery;
        public static readonly CfDbTable WebFileProcessQuery;
        public static readonly CfDbTable WebFileProcessQueue;
        public static readonly CfDbTable WebFileProcessQueueLockAccess;
        public static readonly CfDbTable WebFileProcessQueuePrecedent;
        public static readonly CfDbTable WebFileProcessService;
        public static readonly CfDbTable WebFileProcessSheetGroup;
        public static readonly CfDbTable WebFileProcessStatus;
        public static readonly CfDbTable WebQueueEntryType;
        public static readonly CfDbTable WebQueueMediaType;
        public static readonly CfDbTable WebRole;
        public static readonly CfDbTable WebSession;
        public static readonly CfDbTable WebSessionRoles;
        public static readonly CfDbTable WebSheetCheckPointProcessEvent;
        public static readonly CfDbTable WebSheetProcessEvent;
        public static readonly CfDbTable WebUser;
        public static readonly CfDbTable OpPointHist;
        public static readonly CfDbTable MeasureUnit;
        public static readonly CfDbTable SubFamily;

        static DbModel()
        {

            /******************************************************************************
            ** Tabla  = _AuxTableCreationEventTrack       
            ******************************************************************************/





            _AuxTableCreationEventTrack = new CfDbTable("MDW_PR_TABLES", "_AuxTableCreationEventTrack", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "_AuxTableCreationEventTrack", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            _AuxTableCreationEventTrack.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  _AuxTableCreationEventTrack , "CreationWebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "CreacionEvento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  _AuxTableCreationEventTrack , "CreationWebUser_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "CreacionUsuario_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  _AuxTableCreationEventTrack , "CreationWebEvent_Dt" , "DATE" , true , false , true , false , false , false, false, "FechaCreacion", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  _AuxTableCreationEventTrack , "CreationWebEvent_Tm" , "TIME(6)" , true , false , true , false , false , false, false, "HoraCreacion", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            _AuxTableCreationEventTrack.setOrderByColumns(
                new String[]  {
            "CreationWebEvent_id",
            "CreationWebUser_id",
            "CreationWebEvent_Dt",
            "CreationWebEvent_Tm"
        });
            /******************************************************************************
            ** Tabla  = _AuxTableEventTrack       
            ******************************************************************************/





            _AuxTableEventTrack = new CfDbTable("MDW_PR_TABLES", "_AuxTableEventTrack", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "_AuxTableEventTrack", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            _AuxTableEventTrack.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  _AuxTableEventTrack , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "ActualizacionEvento_Id", null, null, true, false, false, false, false, false ),
            new CfDbColumn(  _AuxTableEventTrack , "LastOpWebUser_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "ActualizacionUsuario_Id", null, null, false, true, false, false, false, false ),
            new CfDbColumn(  _AuxTableEventTrack , "WebEvent_Dt" , "DATE" , true , false , true , false , false , false, false, "FechaActualizacion", null, null, false, false, true, false, false, false ),
            new CfDbColumn(  _AuxTableEventTrack , "WebEvent_Tm" , "TIME(6)" , true , false , true , false , false , false, false, "HoraActualizacion", null, null, false, false, false, true, false, false ),
            });
            // Order columns
            _AuxTableEventTrack.setOrderByColumns(
                new String[]  {
            "WebEvent_id",
            "LastOpWebUser_id",
            "WebEvent_Dt",
            "WebEvent_Tm"
        });
            /******************************************************************************
            ** Tabla  = AuxiliarProductEventFilter       
            ******************************************************************************/





            AuxiliarProductEventFilter = new CfDbTable("MDW_PR_TABLES", "AuxiliarProductEventFilter", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "AuxiliarProductEventFilter", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            AuxiliarProductEventFilter.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  AuxiliarProductEventFilter , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  AuxiliarProductEventFilter , "Product_id" , "CHAR(17)" , true , true , true , false , false , false, false, "Product_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  AuxiliarProductEventFilter , "ProductBarCode_id" , "CHAR(14)" , true , true , true , false , false , false, false, "ProductBarCode_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  AuxiliarProductEventFilter , "EffectiveDate" , "DATE" , true , true , true , false , true , false, false, "Fecha_Inicio_Vigencia_Dt", "CURRENT_DATE", null, false, false, false, false, true, false ),
            });
            // Order columns
            AuxiliarProductEventFilter.setOrderByColumns(
                new String[]  {
            "WebEvent_id",
            "Product_id",
            "ProductBarCode_id",
            "EffectiveDate"
        });
            /******************************************************************************
            ** Tabla  = HttpMethod       
            ******************************************************************************/





            HttpMethod = new CfDbTable("MDW_PR_TABLES", "HttpMethod", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "HttpMethod", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            HttpMethod.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  HttpMethod , "HttpMethod_Cd" , "VARCHAR(50)" , true , false , true , false , false , false, false, "HttpMethod_Cd", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  HttpMethod , "HttpMethod_Desc" , "VARCHAR(255)" , false , false , false , false , false , false, true, "HttpMethod_Desc", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            HttpMethod.setOrderByColumns(
                new String[]  {
            "HttpMethod_Cd"
        });
            /******************************************************************************
            ** Tabla  = HttpStatus       
            ******************************************************************************/





            HttpStatus = new CfDbTable("MDW_PR_TABLES", "HttpStatus", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "HttpStatus", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            HttpStatus.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  HttpStatus , "HttpStatus_Cd" , "VARCHAR(50)" , true , false , true , false , false , false, false, "HttpStatus_Cd", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  HttpStatus , "HttpStatus_Desc" , "VARCHAR(255)" , false , false , true , false , false , false, true, "HttpStatus_Desc", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            HttpStatus.setOrderByColumns(
                new String[]  {
            "HttpStatus_Cd"
        });
            /******************************************************************************
            ** Tabla  = ProductBarCodeHist       
            ******************************************************************************/





            ProductBarCodeHist = new CfDbTable("MDW_PR_TABLES", "ProductBarCodeHist", /* ReadOnly */ false, /* Hist */ true, /* Exception Ind */ "N",/* LocalName */ "Datos EAN", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            ProductBarCodeHist.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  ProductBarCodeHist , "Product_id" , "CHAR(17)" , true , false , true , false , false , false, false, "Product_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "ProductBarCode_id" , "CHAR(14)" , true , false , true , false , false , false, false, "ProductBarCode_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "EffectiveDate" , "DATE" , true , false , true , false , true , false, false, "Fecha_Inicio_Vigencia_Dt", "CURRENT_DATE", null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "ExpirationDate" , "DATE" , false , false , true , false , false , true, false, "Fecha_Cierre_Vigencia_Dt", "9999-12-31", null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Creation_Date" , "DATE" , false , false , false , false , false , false, false, "Fecha_Creacion_Dt", "1900-01-01", null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Pack_Qty" , "INTEGER" , false , false , true , false , false , false, false, "PCB_Cant", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Layer_Qty" , "DECIMAL(18,4)" , false , false , true , false , false , false, false, "Bultos_Piso_Proveedor", null, "( Layer_Qty >= 0 AND Layer_Qty <= Pallet_Qty)", false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Pallet_Qty" , "DECIMAL(18,4)" , false , false , true , false , false , false, false, "Bultos_Pallet_Proveedor", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "StoreDelivery_Ind" , "CHAR(3)" , false , false , false , false , false , false, false, "Entrega_Directa_Tienda_Ind", null, "(StoreDelivery_Ind IN ('S','N'))", false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "SrcSupplier_id" , "VARCHAR(50)" , false , false , true , false , false , false, false, "SrcSupplier_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "ResponsibleRepUSer_id" , "VARCHAR(50)" , false , false , true , false , false , false, false, "ResponsibleRepUSer_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "PurchaseStatus_id" , "VARCHAR(50)" , false , false , true , false , false , false, false, "PurchaseStatus_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "PurchaseUnit_id" , "VARCHAR(50)" , false , false , true , false , false , false, false, "PurchaseUnit_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "MinPurchase_Qty" , "DECIMAL(18,4)" , false , false , true , false , false , false, false, "Min_Compra_Bultos_Cant", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "InternalProductCode" , "CHAR(14)" , false , false , false , false , false , false, false, "Producto_Interno_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Weight_Ind" , "CHAR(3)" , false , false , false , false , false , false, false, "Pesable_Ind", null, "(Weight_Ind IN ('S','N'))", false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "WeightPack_Rate" , "DECIMAL(18,12)" , false , false , false , false , false , false, false, "Ratio_Variabilidad_Pesables", null, "(WeightPack_Rate >= 0)", false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "ShelfLife_Cnt" , "INTEGER" , false , false , false , false , false , false, false, "DiasVidaUtil", null, "(ShelfLife_Cnt >= 0)", false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Color_id" , "CHAR(7)" , false , false , true , false , false , false, false, "Color_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Size_id" , "CHAR(6)" , false , false , true , false , false , false, false, "Size_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "MeasureUnit_id" , "CHAR(3)" , false , true , false , false , false , false, false, "Unidad_Medida_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Delete_Ind" , "CHAR(3)" , false , false , true , false , false , false, false, "Delete_Ind", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Ticket_Desc" , "VARCHAR(255)" , false , false , true , false , false , false, false, "Ticket_Desc", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Short_Desc" , "VARCHAR(255)" , false , false , true , false , false , false, false, "Short_Desc", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Long_Desc" , "VARCHAR(255)" , false , false , true , false , false , false, false, "Long_Desc", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Net_Content_Qty" , "DECIMAL(18,4)" , false , false , false , false , false , false, false, "Net_Content_Qty", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Gross_Content_Qty" , "DECIMAL(18,4)" , false , false , false , false , false , false, false, "Gross_Content_Qty", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "SubFamily_id" , "CHAR(22)" , false , true , true , false , false , false, false, "SubFamilia_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "PartOfInventory_id" , "CHAR(1)" , false , true , true , false , false , false, false, "PartOfInventory_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "ProductState_id" , "CHAR(1)" , false , true , true , false , false , false, false, "Estado_Producto_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "PartOfCollection_id" , "CHAR(1)" , false , false , true , false , false , false, false, "PartOfCollection_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "Seasonality_id" , "CHAR(1)" , false , true , true , false , false , false, false, "Estacion_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "ProductCategory_id" , "CHAR(1)" , false , true , true , false , false , false, false, "Categoria_SKU_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "ProductType_id" , "CHAR(1)" , false , true , false , false , false , false, false, "Tipo_SKU_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  ProductBarCodeHist , "WebEvent_id" , "VARCHAR(50)" , false , true , false , false , false , false, false, "ActualizacionEvento_Id", null, null, true, false, false, false, false, false ),
            new CfDbColumn(  ProductBarCodeHist , "LastOpWebUser_id" , "VARCHAR(50)" , false , true , false , false , false , false, false, "ActualizacionUsuario_Id", null, null, false, true, false, false, false, false ),
            new CfDbColumn(  ProductBarCodeHist , "WebEvent_Dt" , "DATE" , false , true , false , false , false , false, false, "FechaActualizacion", null, null, false, false, true, false, false, false ),
            new CfDbColumn(  ProductBarCodeHist , "WebEvent_Tm" , "TIME(6)" , false , true , false , false , false , false, false, "HoraActualizacion", null, null, false, false, false, true, false, false ),
            });
            // Order columns
            ProductBarCodeHist.setOrderByColumns(
                new String[]  {
            "Product_id",
            "ProductBarCode_id",
            "EffectiveDate"
        });
            /******************************************************************************
            ** Tabla  = WebActivity       
            ******************************************************************************/





            WebActivity = new CfDbTable("MDW_PR_TABLES", "WebActivity", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebActivity", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebActivity.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebActivity , "WebActivity_id" , "VARCHAR(100)" , true , false , true , false , false , false, false, "WebActivity_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebActivity , "WebActivity_Desc" , "VARCHAR(255)" , false , false , true , false , false , false, true, "WebActivity_Desc", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebActivity.setOrderByColumns(
                new String[]  {
            "WebActivity_id"
        });
            /******************************************************************************
            ** Tabla  = WebDbObject       
            ******************************************************************************/





            WebDbObject = new CfDbTable("MDW_PR_TABLES", "WebDbObject", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebDbObject", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebDbObject.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebDbObject , "WebDbObject_id" , "VARCHAR(50)" , true , false , true , false , false , false, false, "Objeto_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObject , "WebDbObject_Desc" , "VARCHAR(255)" , false , false , false , false , false , false, true, "Objeto_Desc", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebDbObject.setOrderByColumns(
                new String[]  {
            "WebDbObject_id"
        });
            /******************************************************************************
            ** Tabla  = WebDbObjectPrecedent       
            ******************************************************************************/





            WebDbObjectPrecedent = new CfDbTable("MDW_PR_TABLES", "WebDbObjectPrecedent", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebDbObjectPrecedent", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebDbObjectPrecedent.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebDbObjectPrecedent , "WebDbObject_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Objeto_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectPrecedent , "PrecedentWebDbObject_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Objeto_Id", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebDbObjectPrecedent.setOrderByColumns(
                new String[]  {
            "WebDbObject_id",
            "PrecedentWebDbObject_id"
        });
            /******************************************************************************
            ** Tabla  = WebDbObjectProcess       
            ******************************************************************************/





            WebDbObjectProcess = new CfDbTable("MDW_PR_TABLES", "WebDbObjectProcess", /* ReadOnly */ false, /* Hist */ true, /* Exception Ind */ "N",/* LocalName */ "WebDbObjectProcess", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebDbObjectProcess.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebDbObjectProcess , "WebDbObject_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Objeto_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectProcess , "EffectiveDate" , "DATE" , true , false , true , false , true , false, false, "Fecha_Inicio_Vigencia_Dt", "CURRENT_DATE", null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectProcess , "WebDbObjectProcessLine_Num" , "INTEGER" , true , false , true , false , false , false, false, "Numero_Linea", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectProcess , "ExpirationDate" , "DATE" , false , false , false , false , false , true, false, "Fecha_Cierre_Vigencia_Dt", "9999-12-31", null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectProcess , "EffectiveTime" , "TIME(0)" , false , false , false , false , false , false, false, "Hora_Inicio_Vigencia_Tm", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectProcess , "ExpirationTime" , "TIME(0)" , false , false , false , false , false , false, false, "Hora_Cierre_Vigencia_Tm", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectProcess , "ProcessEnabled_Ind" , "CHAR(3)" , false , false , false , false , false , false, false, "Proceso_Habilitado_Ind", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebDbObjectProcess.setOrderByColumns(
                new String[]  {
            "WebDbObject_id",
            "EffectiveDate",
            "WebDbObjectProcessLine_Num"
        });
            /******************************************************************************
            ** Tabla  = WebDbObjectQueue       
            ******************************************************************************/





            WebDbObjectQueue = new CfDbTable("MDW_PR_TABLES", "WebDbObjectQueue", /* ReadOnly */ false, /* Hist */ true, /* Exception Ind */ "N",/* LocalName */ "WebDbObjectQueue", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebDbObjectQueue.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebDbObjectQueue , "WebDbObject_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Objeto_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectQueue , "EffectiveDate" , "DATE" , true , false , true , false , true , false, false, "Fecha_Inicio_Vigencia_Dt", "CURRENT_DATE", null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectQueue , "WebDbObjectQueueLine_Num" , "INTEGER" , true , false , true , false , false , false, false, "Numero_Linea", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectQueue , "ExpirationDate" , "DATE" , false , false , false , false , false , true, false, "Fecha_Cierre_Vigencia_Dt", "9999-12-31", null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectQueue , "EffectiveTime" , "TIME(0)" , false , false , false , false , false , false, false, "Hora_Inicio_Vigencia_Tm", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectQueue , "ExpirationTime" , "TIME(0)" , false , false , false , false , false , false, false, "Hora_Cierre_Vigencia_Tm", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebDbObjectQueue , "QueueEnabled_Ind" , "CHAR(3)" , false , false , false , false , false , false, false, "Encolado_Habilitado_Ind", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebDbObjectQueue.setOrderByColumns(
                new String[]  {
            "WebDbObject_id",
            "EffectiveDate",
            "WebDbObjectQueueLine_Num"
        });
            /******************************************************************************
            ** Tabla  = WebEvent       
            ******************************************************************************/





            WebEvent = new CfDbTable("MDW_PR_TABLES", "WebEvent", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebEvent", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebEvent.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebEvent , "WebEvent_id" , "VARCHAR(50)" , true , false , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "WebSession_id" , "VARCHAR(50)" , false , true , true , false , false , false, false, "Sesion_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "WebActivity_id" , "VARCHAR(100)" , false , true , true , false , false , false, true, "WebActivity_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "HttpMethod_Cd" , "VARCHAR(50)" , false , true , false , false , false , false, false, "HttpMethod_Cd", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "Event_Uri" , "VARCHAR(1000)" , false , false , false , false , false , false, false, "Event_Uri", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "EventIPAddress_Txt" , "VARCHAR(1000)" , false , false , false , false , false , false, false, "EventIPAddress_Txt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "EventReferrer_Uri" , "VARCHAR(1000)" , false , false , false , false , false , false, false, "EventReferrer_Uri", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "EventRequest_Dt" , "DATE" , false , false , true , false , false , false, false, "EventRequest_Dt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "EventRequest_Tm" , "TIME(6)" , false , false , true , false , false , false, false, "EventRequest_Tm", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "HttpStatus_Cd" , "VARCHAR(50)" , false , true , false , false , false , false, false, "HttpStatus_Cd", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "ReasonPhrase_Txt" , "VARCHAR(1000)" , false , false , false , false , false , false, false, "ReasonPhrase_Txt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "Success_Ind" , "CHAR(3)" , false , false , false , false , false , false, false, "Success_Ind", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "EventResponse_Dt" , "DATE" , false , false , false , false , false , false, false, "EventResponse_Dt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "EventResponse_Tm" , "TIME(6)" , false , false , false , false , false , false, false, "EventResponse_Tm", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "EventRequestContent_Txt" , "VARCHAR(1000)" , false , false , false , false , false , false, false, "EventRequestContent_Txt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebEvent , "EventResponseContent_Txt" , "VARCHAR(1000)" , false , false , false , false , false , false, false, "EventResponseContent_Txt", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebEvent.setOrderByColumns(
                new String[]  {
            "WebEvent_id"
        });
            /******************************************************************************
            ** Tabla  = WebEventMutex       
            ******************************************************************************/





            WebEventMutex = new CfDbTable("MDW_PR_TABLES", "WebEventMutex", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebEventMutex", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebEventMutex.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebEventMutex , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebEventMutex.setOrderByColumns(
                new String[]  {
            "WebEvent_id"
        });
            /******************************************************************************
            ** Tabla  = WebFileProcessEvent       
            ******************************************************************************/





            WebFileProcessEvent = new CfDbTable("MDW_PR_TABLES", "WebFileProcessEvent", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebFileProcessEvent", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebFileProcessEvent.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebFileProcessEvent , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessEvent , "File_Name" , "VARCHAR(255)" , false , false , true , false , false , false, false, "Archivo_Nombre", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessEvent , "LocalFile_Name" , "VARCHAR(255)" , false , false , true , false , false , false, false, "Archivo_Local_Nombre", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessEvent , "ProcessStart_Dt" , "DATE" , false , false , true , false , false , false, false, "Fecha_Inicio_Proceso", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessEvent , "ProcessStart_Tm" , "TIME(6)" , false , false , true , false , false , false, false, "Hora_Inicio_Proceso", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessEvent , "ProcessEnd_Dt" , "DATE" , false , false , false , false , false , false, false, "Fecha_Fin_Proceso", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessEvent , "ProcessEnd_Tm" , "TIME(6)" , false , false , false , false , false , false, false, "Hora_Fin_Proceso", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessEvent , "Success_Ind" , "CHAR(3)" , false , false , false , false , false , false, false, "Finalizado_Ind", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessEvent , "ReasonPhrase_Txt" , "VARCHAR(10000)" , false , false , false , false , false , false, false, "Proceso_Descripcion_Txt", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebFileProcessEvent.setOrderByColumns(
                new String[]  {
            "WebEvent_id"
        });
            /******************************************************************************
            ** Tabla  = WebFileProcessGroupQuery       
            ******************************************************************************/





            WebFileProcessGroupQuery = new CfDbTable("MDW_PR_TABLES", "WebFileProcessGroupQuery", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebFileProcessGroupQuery", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebFileProcessGroupQuery.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebFileProcessGroupQuery , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessGroupQuery , "Sheet_Num" , "INTEGER" , true , true , true , false , false , false, false, "Sheet_Num", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessGroupQuery , "Query_Num" , "INTEGER" , true , false , true , false , false , false, false, "Query_Num", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebFileProcessGroupQuery.setOrderByColumns(
                new String[]  {
            "WebEvent_id",
            "Sheet_Num",
            "Query_Num"
        });
            /******************************************************************************
            ** Tabla  = WebFileProcessQuery       
            ******************************************************************************/





            WebFileProcessQuery = new CfDbTable("MDW_PR_TABLES", "WebFileProcessQuery", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebFileProcessQuery", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebFileProcessQuery.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebFileProcessQuery , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQuery , "Sheet_Num" , "INTEGER" , true , true , true , false , false , false, false, "Sheet_Num", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQuery , "Query_Num" , "INTEGER" , true , true , true , false , false , false, false, "Query_Num", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQuery , "WebFileProcessQueryLine_Txt" , "CLOB(128K)" , false , false , true , false , false , false, false, "WebFileProcessQueryLine_Txt", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebFileProcessQuery.setOrderByColumns(
                new String[]  {
            "WebEvent_id",
            "Sheet_Num",
            "Query_Num"
        });
            /******************************************************************************
            ** Tabla  = WebFileProcessQueue       
            ******************************************************************************/





            WebFileProcessQueue = new CfDbTable("MDW_PR_TABLES", "WebFileProcessQueue", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebFileProcessQueue", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebFileProcessQueue.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebFileProcessQueue , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "WebQueueEntryType_id" , "VARCHAR(50)" , false , true , true , false , false , false, false, "Tipo_Accion_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "WebQueueMediaType_id" , "VARCHAR(50)" , false , true , true , false , false , false, false, "Tipo_Archivo_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "WebEventParameters_Txt" , "VARCHAR(50000)" , false , false , false , false , false , false, false, "Parametros_Evento_Txt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "File_Name" , "VARCHAR(255)" , false , false , true , false , false , false, false, "Archivo_Nombre", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "LocalFile_Name" , "VARCHAR(255)" , false , false , true , false , false , false, false, "Archivo_Local_Name", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "ResponseFile_Name" , "VARCHAR(255)" , false , false , false , false , false , false, false, "Archivo_Respuesta_Nombre", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "WorkingDir_Name" , "VARCHAR(255)" , false , false , false , false , false , false, false, "Dir_Trabajo_Nombre", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "UrlFile_Name" , "VARCHAR(255)" , false , false , false , false , false , false, false, "Url_Nombre", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "WebDbObject_id" , "VARCHAR(50)" , false , true , true , false , false , false, false, "Objeto_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "Enqueue_Dt" , "DATE" , false , false , true , false , false , false, false, "Fecha", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "Enqueue_Tm" , "TIME(6)" , false , false , true , false , false , false, false, "Hora", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueue , "WebFileProcessStatus_id" , "VARCHAR(50)" , false , true , true , false , false , false, false, "Estado_Proceso_Id", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebFileProcessQueue.setOrderByColumns(
                new String[]  {
            "WebEvent_id"
        });
            /******************************************************************************
            ** Tabla  = WebFileProcessQueueLockAccess       
            ******************************************************************************/





            WebFileProcessQueueLockAccess = new CfDbTable("MDW_PR_TABLES", "WebFileProcessQueueLockAccess", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebFileProcessQueueLockAccess", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebFileProcessQueueLockAccess.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebFileProcessQueueLockAccess , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebFileProcessQueueLockAccess.setOrderByColumns(
                new String[]  {
            "WebEvent_id"
        });
            /******************************************************************************
            ** Tabla  = WebFileProcessQueuePrecedent       
            ******************************************************************************/





            WebFileProcessQueuePrecedent = new CfDbTable("MDW_PR_TABLES", "WebFileProcessQueuePrecedent", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebFileProcessQueuePrecedent", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebFileProcessQueuePrecedent.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebFileProcessQueuePrecedent , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessQueuePrecedent , "PrecedentWebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebFileProcessQueuePrecedent.setOrderByColumns(
                new String[]  {
            "WebEvent_id",
            "PrecedentWebEvent_id"
        });
            /******************************************************************************
            ** Tabla  = WebFileProcessService       
            ******************************************************************************/





            WebFileProcessService = new CfDbTable("MDW_PR_TABLES", "WebFileProcessService", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebFileProcessService", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebFileProcessService.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebFileProcessService , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessService , "ServiceReasonPhrase_Txt" , "VARCHAR(1000)" , false , false , false , false , false , false, false, "Mensaje_Servicio_Txt", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebFileProcessService.setOrderByColumns(
                new String[]  {
            "WebEvent_id"
        });
            /******************************************************************************
            ** Tabla  = WebFileProcessSheetGroup       
            ******************************************************************************/





            WebFileProcessSheetGroup = new CfDbTable("MDW_PR_TABLES", "WebFileProcessSheetGroup", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebFileProcessSheetGroup", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebFileProcessSheetGroup.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebFileProcessSheetGroup , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessSheetGroup , "Sheet_Num" , "INTEGER" , true , false , true , false , false , false, false, "Sheet_Num", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessSheetGroup , "Sheet_Name" , "VARCHAR(255)" , false , false , false , false , false , false, false, "Sheet_Name", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebFileProcessSheetGroup.setOrderByColumns(
                new String[]  {
            "WebEvent_id",
            "Sheet_Num"
        });
            /******************************************************************************
            ** Tabla  = WebFileProcessStatus       
            ******************************************************************************/





            WebFileProcessStatus = new CfDbTable("MDW_PR_TABLES", "WebFileProcessStatus", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebFileProcessStatus", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebFileProcessStatus.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebFileProcessStatus , "WebFileProcessStatus_id" , "VARCHAR(50)" , true , false , true , false , false , false, false, "Estado_Proceso_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessStatus , "WebFileProcessStatus_Desc" , "VARCHAR(255)" , false , false , false , false , false , false, true, "Estado_Proceso_Desc", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebFileProcessStatus , "AcceptChange_Ind" , "CHAR(3)" , false , false , false , false , false , false, true, "Acepta_Cambios_Ind", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebFileProcessStatus.setOrderByColumns(
                new String[]  {
            "WebFileProcessStatus_id"
        });
            /******************************************************************************
            ** Tabla  = WebQueueEntryType       
            ******************************************************************************/





            WebQueueEntryType = new CfDbTable("MDW_PR_TABLES", "WebQueueEntryType", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebQueueEntryType", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebQueueEntryType.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebQueueEntryType , "WebQueueEntryType_id" , "VARCHAR(50)" , true , false , true , false , false , false, false, "Tipo_Accion_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebQueueEntryType , "WebQueueEntryType_Desc" , "VARCHAR(255)" , false , false , true , false , false , false, true, "Tipo_Accion_Desc", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebQueueEntryType.setOrderByColumns(
                new String[]  {
            "WebQueueEntryType_id"
        });
            /******************************************************************************
            ** Tabla  = WebQueueMediaType       
            ******************************************************************************/





            WebQueueMediaType = new CfDbTable("MDW_PR_TABLES", "WebQueueMediaType", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebQueueMediaType", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebQueueMediaType.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebQueueMediaType , "WebQueueMediaType_id" , "VARCHAR(50)" , true , false , true , false , false , false, false, "Tipo_Archivo_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebQueueMediaType , "WebQueueMediaType_Desc" , "VARCHAR(255)" , false , false , true , false , false , false, true, "Tipo_Archivo_Desc", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebQueueMediaType.setOrderByColumns(
                new String[]  {
            "WebQueueMediaType_id"
        });
            /******************************************************************************
            ** Tabla  = WebRole       
            ******************************************************************************/





            WebRole = new CfDbTable("MDW_PR_TABLES", "WebRole", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebRole", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebRole.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebRole , "WebRole_id" , "VARCHAR(50)" , true , false , true , false , false , false, false, "WebRole_id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebRole , "WebRole_Desc" , "VARCHAR(255)" , false , false , true , false , false , false, true, "WebRole_Desc", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebRole.setOrderByColumns(
                new String[]  {
            "WebRole_id"
        });
            /******************************************************************************
            ** Tabla  = WebSession       
            ******************************************************************************/





            WebSession = new CfDbTable("MDW_PR_TABLES", "WebSession", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebSession", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebSession.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebSession , "WebSession_id" , "VARCHAR(50)" , true , false , true , false , false , false, false, "Sesion_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "WebUser_id" , "VARCHAR(50)" , false , true , true , false , false , false, false, "Usuario_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "LogOn_Dt" , "DATE" , false , false , true , false , false , false, false, "LogOn_Dt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "LogOn_Tm" , "TIME(6)" , false , false , false , false , false , false, false, "LogOn_Tm", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "LogOnSource" , "VARCHAR(1000)" , false , false , true , false , false , false, false, "LogOnSource", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "LogOff_Dt" , "DATE" , false , false , false , false , false , false, false, "LogOff_Dt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "LogOff_Tm" , "TIME(6)" , false , false , false , false , false , false, false, "LogOff_Tm", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "LogOff_Txt" , "VARCHAR(100)" , false , false , false , false , false , false, false, "LogOff_Txt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "WebUserBussinessUnit_Name" , "VARCHAR(255)" , false , false , false , false , false , false, false, "WebUserBussinessUnit_Name", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "WebUserOrganization_Name" , "VARCHAR(255)" , false , false , false , false , false , false, false, "WebUserOrganization_Name", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "WebUserOrganizationSector_Name" , "VARCHAR(255)" , false , false , false , false , false , false, false, "WebUserOrganizationSector_Name", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "WebUserTitle_Name" , "VARCHAR(255)" , false , false , false , false , false , false, false, "WebUserTitle_Name", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "ManagerWebUser_id" , "VARCHAR(50)" , false , true , false , false , false , false, false, "Usuario_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "LastEvent_Dt" , "DATE" , false , false , false , false , false , false, false, "LastEvent_Dt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSession , "LastEvent_Tm" , "TIME(6)" , false , false , false , false , false , false, false, "LastEvent_Tm", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebSession.setOrderByColumns(
                new String[]  {
            "WebSession_id"
        });
            /******************************************************************************
            ** Tabla  = WebSessionRoles       
            ******************************************************************************/





            WebSessionRoles = new CfDbTable("MDW_PR_TABLES", "WebSessionRoles", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebSessionRoles", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebSessionRoles.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebSessionRoles , "WebSession_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Sesion_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSessionRoles , "WebRole_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "WebRole_id", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebSessionRoles.setOrderByColumns(
                new String[]  {
            "WebSession_id",
            "WebRole_id"
        });
            /******************************************************************************
            ** Tabla  = WebSheetCheckPointProcessEvent       
            ******************************************************************************/





            WebSheetCheckPointProcessEvent = new CfDbTable("MDW_PR_TABLES", "WebSheetCheckPointProcessEvent", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebSheetCheckPointProcessEvent", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebSheetCheckPointProcessEvent.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebSheetCheckPointProcessEvent , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetCheckPointProcessEvent , "Sheet_Name" , "VARCHAR(255)" , true , true , true , false , false , false, false, "Hoja_Nombre", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetCheckPointProcessEvent , "CheckPoint_Num" , "INTEGER" , true , false , true , false , false , false, false, "CheckPoint_Num", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetCheckPointProcessEvent , "RunningRecord_Cnt" , "INTEGER" , false , false , false , false , false , false, false, "RunningRecord_Cnt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetCheckPointProcessEvent , "CheckPoint_Dt" , "DATE" , false , false , true , false , false , false, false, "CheckPoint_Dt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetCheckPointProcessEvent , "CheckPoint_Tm" , "TIME(6)" , false , false , true , false , false , false, false, "CheckPoint_Tm", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebSheetCheckPointProcessEvent.setOrderByColumns(
                new String[]  {
            "WebEvent_id",
            "Sheet_Name",
            "CheckPoint_Num"
        });
            /******************************************************************************
            ** Tabla  = WebSheetProcessEvent       
            ******************************************************************************/





            WebSheetProcessEvent = new CfDbTable("MDW_PR_TABLES", "WebSheetProcessEvent", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "WebSheetProcessEvent", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebSheetProcessEvent.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebSheetProcessEvent , "WebEvent_id" , "VARCHAR(50)" , true , true , true , false , false , false, false, "Evento_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "Sheet_Name" , "VARCHAR(255)" , true , false , true , false , false , false, false, "Hoja_Nombre", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "Sheet_Order" , "INTEGER" , false , false , true , false , false , false, false, "Hoja_Orden", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "ProcessStart_Dt" , "DATE" , false , false , false , false , false , false, false, "ProcessStart_Dt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "ProcessStart_Tm" , "TIME(6)" , false , false , false , false , false , false, false, "ProcessStart_Tm", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "ProcessEnd_Dt" , "DATE" , false , false , false , false , false , false, false, "ProcessEnd_Dt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "ProcessEnd_Tm" , "TIME(6)" , false , false , false , false , false , false, false, "ProcessEnd_Tm", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "Input_Cnt" , "INTEGER" , false , false , false , false , false , false, false, "Entradas_Cnt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "RIFail_Cnt" , "VARCHAR(50)" , false , false , false , false , false , false, false, "IR_Errores_Cnt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "Applied_Cnt" , "INTEGER" , false , false , false , false , false , false, false, "Aplicados_Cnt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "Success_Cnt" , "INTEGER" , false , false , false , false , false , false, false, "Satisfactorios_Cnt", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "Success_Ind" , "CHAR(3)" , false , false , false , false , false , false, false, "Satisfactorio_Ind", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebSheetProcessEvent , "ReasonPhrase_Txt" , "VARCHAR(1000)" , false , false , false , false , false , false, false, "Proceso_Descripcion_Txt", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebSheetProcessEvent.setOrderByColumns(
                new String[]  {
            "WebEvent_id",
            "Sheet_Name"
        });
            /******************************************************************************
            ** Tabla  = WebUser       
            ******************************************************************************/





            WebUser = new CfDbTable("MDW_PR_TABLES", "WebUser", /* ReadOnly */ false, /* Hist */ false, /* Exception Ind */ "N",/* LocalName */ "Responsable", /* RejectOverlap */ false, /* Export Query */ false, /* Parent join type */ "Default", /* Stg Coll Stat */ false);

            WebUser.setColumns(
                    new CfDbColumn[] {
                        //new CfDbColumn( CfDbTable,   _ColName ,  	_DataType      , _isPk	 , _isFk   , _isReq   , _isReadOnly , _isStartDt , _isEndDt, _isDesc, _LocalName, _DefaultValue, _CheckFunc, _isTrkEvent, _isTrkUser, _isTrkDt, _isTrkTm, _isReqOnImport, _isFillOnImport ),   
            new CfDbColumn(  WebUser , "WebUser_id" , "VARCHAR(50)" , true , false , true , false , false , false, false, "Usuario_Id", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebUser , "WebUser_GivenName" , "VARCHAR(255)" , false , false , false , false , false , false, true, "Usuario_Nombre", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebUser , "WebUser_LastName" , "VARCHAR(255)" , false , false , false , false , false , false, false, "Usuario_Apellido", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebUser , "WebUser_Name" , "VARCHAR(255)" , false , false , false , false , false , false, false, "Usuario_NombreCompleto", null, null, false, false, false, false, true, false ),
            new CfDbColumn(  WebUser , "WebUser_EMail" , "VARCHAR(1000)" , false , false , false , false , false , false, false, "Usuario_EMail", null, null, false, false, false, false, true, false ),
            });
            // Order columns
            WebUser.setOrderByColumns(
                new String[]  {
            "WebUser_id"
        });



            /******************************************************************************
            *******************************************************************************
            *******************************************************************************
            *******************************************************************************
            ** Relaciones
            *******************************************************************************
            *******************************************************************************
            *******************************************************************************
            ******************************************************************************/

            /******************************************************************************
            ** Tabla  = _AuxTableCreationEventTrack       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_14d916ec_4e773736_55f29dbb_129135aa_0_ = new CfDbRelation(WebUser, _AuxTableCreationEventTrack);
                rel_14d916ec_4e773736_55f29dbb_129135aa_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_14d916ec_4e773736_55f29dbb_129135aa_0_, "WebUser_id", "CreationWebUser_id" ),
            });


                CfDbRelation rel_862aa921_4ae91b0b_3088793_7f0bb937_0_ = new CfDbRelation(WebEvent, _AuxTableCreationEventTrack);
                rel_862aa921_4ae91b0b_3088793_7f0bb937_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_862aa921_4ae91b0b_3088793_7f0bb937_0_, "WebEvent_id", "CreationWebEvent_id" ),
            });





                _AuxTableCreationEventTrack.setRelationToParentTables(new CfDbRelation[] {

rel_14d916ec_4e773736_55f29dbb_129135aa_0_,

rel_862aa921_4ae91b0b_3088793_7f0bb937_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = _AuxTableEventTrack       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_eb87df5_43ad7dfc_ccf6f9b_3ce799b_0_ = new CfDbRelation(_AuxTableEventTrack, OpPointHist);
                rel_eb87df5_43ad7dfc_ccf6f9b_3ce799b_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_eb87df5_43ad7dfc_ccf6f9b_3ce799b_0_, "WebEvent_id", "WebEvent_id" ),
                new CfDbRelAttribute( rel_eb87df5_43ad7dfc_ccf6f9b_3ce799b_0_, "LastOpWebUser_id", "LastOpWebUser_id" ),
                new CfDbRelAttribute( rel_eb87df5_43ad7dfc_ccf6f9b_3ce799b_0_, "WebEvent_Dt", "WebEvent_Dt" ),
                new CfDbRelAttribute( rel_eb87df5_43ad7dfc_ccf6f9b_3ce799b_0_, "WebEvent_Tm", "WebEvent_Tm" ),
            });


                CfDbRelation rel_e12b64d4_49ec2634_ce7a8bad_fa0f82e5_0_ = new CfDbRelation(_AuxTableEventTrack, ProductBarCodeHist);
                rel_e12b64d4_49ec2634_ce7a8bad_fa0f82e5_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_e12b64d4_49ec2634_ce7a8bad_fa0f82e5_0_, "WebEvent_id", "WebEvent_id" ),
                new CfDbRelAttribute( rel_e12b64d4_49ec2634_ce7a8bad_fa0f82e5_0_, "LastOpWebUser_id", "LastOpWebUser_id" ),
                new CfDbRelAttribute( rel_e12b64d4_49ec2634_ce7a8bad_fa0f82e5_0_, "WebEvent_Dt", "WebEvent_Dt" ),
                new CfDbRelAttribute( rel_e12b64d4_49ec2634_ce7a8bad_fa0f82e5_0_, "WebEvent_Tm", "WebEvent_Tm" ),
            });


                ;


                _AuxTableEventTrack.setRelationToChildTables(new CfDbRelation[] {

rel_eb87df5_43ad7dfc_ccf6f9b_3ce799b_0_,

rel_e12b64d4_49ec2634_ce7a8bad_fa0f82e5_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = AuxiliarProductEventFilter       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_8dc0fd78_45b9c39c_2ed5d5a2_d0a50f76_0_ = new CfDbRelation(ProductBarCodeHist, AuxiliarProductEventFilter);
                rel_8dc0fd78_45b9c39c_2ed5d5a2_d0a50f76_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_8dc0fd78_45b9c39c_2ed5d5a2_d0a50f76_0_, "Product_id", "Product_id" ),
                new CfDbRelAttribute( rel_8dc0fd78_45b9c39c_2ed5d5a2_d0a50f76_0_, "ProductBarCode_id", "ProductBarCode_id" ),
                new CfDbRelAttribute( rel_8dc0fd78_45b9c39c_2ed5d5a2_d0a50f76_0_, "EffectiveDate", "EffectiveDate" ),
            });


                CfDbRelation rel_27a9b20a_4f6a2e82_36e3199b_59510523_0_ = new CfDbRelation(WebEvent, AuxiliarProductEventFilter);
                rel_27a9b20a_4f6a2e82_36e3199b_59510523_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_27a9b20a_4f6a2e82_36e3199b_59510523_0_, "WebEvent_id", "WebEvent_id" ),
            });





                AuxiliarProductEventFilter.setRelationToParentTables(new CfDbRelation[] {

rel_8dc0fd78_45b9c39c_2ed5d5a2_d0a50f76_0_,

rel_27a9b20a_4f6a2e82_36e3199b_59510523_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = HttpMethod       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_8bf8002d_4e8cd36d_4ff5e79e_d895f51e_0_ = new CfDbRelation(HttpMethod, WebEvent);
                rel_8bf8002d_4e8cd36d_4ff5e79e_d895f51e_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_8bf8002d_4e8cd36d_4ff5e79e_d895f51e_0_, "HttpMethod_Cd", "HttpMethod_Cd" ),
            });


                ;


                HttpMethod.setRelationToChildTables(new CfDbRelation[] {

rel_8bf8002d_4e8cd36d_4ff5e79e_d895f51e_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = HttpStatus       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_61ec5253_495908ab_9876848b_64a0e6a5_0_ = new CfDbRelation(HttpStatus, WebEvent);
                rel_61ec5253_495908ab_9876848b_64a0e6a5_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_61ec5253_495908ab_9876848b_64a0e6a5_0_, "HttpStatus_Cd", "HttpStatus_Cd" ),
            });


                ;


                HttpStatus.setRelationToChildTables(new CfDbRelation[] {

rel_61ec5253_495908ab_9876848b_64a0e6a5_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = ProductBarCodeHist       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_cc8f3d4_47c735b5_77c372bf_31f9e84d_0_ = new CfDbRelation(MeasureUnit, ProductBarCodeHist);
                rel_cc8f3d4_47c735b5_77c372bf_31f9e84d_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_cc8f3d4_47c735b5_77c372bf_31f9e84d_0_, "MeasureUnit_id", "MeasureUnit_id" ),
            });


                CfDbRelation rel_fbbe049b_4cf2ffe6_8f7f5eb8_f48c824f_0_ = new CfDbRelation(SubFamily, ProductBarCodeHist);
                rel_fbbe049b_4cf2ffe6_8f7f5eb8_f48c824f_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_fbbe049b_4cf2ffe6_8f7f5eb8_f48c824f_0_, "SubFamily_id", "SubFamily_id" ),
            });





                ProductBarCodeHist.setRelationToParentTables(new CfDbRelation[] {

rel_cc8f3d4_47c735b5_77c372bf_31f9e84d_0_,

rel_fbbe049b_4cf2ffe6_8f7f5eb8_f48c824f_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebActivity       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_d6564e8_476a659a_6297a989_2f69a0f9_0_ = new CfDbRelation(WebActivity, WebEvent);
                rel_d6564e8_476a659a_6297a989_2f69a0f9_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_d6564e8_476a659a_6297a989_2f69a0f9_0_, "WebActivity_id", "WebActivity_id" ),
            });


                ;


                WebActivity.setRelationToChildTables(new CfDbRelation[] {

rel_d6564e8_476a659a_6297a989_2f69a0f9_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebDbObject       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_d620cde0_4b9e6d5a_aea232a7_bdcc9487_0_ = new CfDbRelation(WebDbObject, WebDbObjectProcess);
                rel_d620cde0_4b9e6d5a_aea232a7_bdcc9487_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_d620cde0_4b9e6d5a_aea232a7_bdcc9487_0_, "WebDbObject_id", "WebDbObject_id" ),
            });


                CfDbRelation rel_80fefe58_44c9e83d_71b5bf9d_80674840_0_ = new CfDbRelation(WebDbObject, WebDbObjectQueue);
                rel_80fefe58_44c9e83d_71b5bf9d_80674840_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_80fefe58_44c9e83d_71b5bf9d_80674840_0_, "WebDbObject_id", "WebDbObject_id" ),
            });


                CfDbRelation rel_d3645cdd_43998218_5e48a8e_df39575_0_ = new CfDbRelation(WebDbObject, WebFileProcessQueue);
                rel_d3645cdd_43998218_5e48a8e_df39575_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_d3645cdd_43998218_5e48a8e_df39575_0_, "WebDbObject_id", "WebDbObject_id" ),
            });


                ;


                WebDbObject.setRelationToChildTables(new CfDbRelation[] {

rel_d620cde0_4b9e6d5a_aea232a7_bdcc9487_0_,

rel_80fefe58_44c9e83d_71b5bf9d_80674840_0_,

rel_d3645cdd_43998218_5e48a8e_df39575_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebDbObjectPrecedent       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_7add0ff7_45cceedd_a804f789_3c5a9ae3_0_ = new CfDbRelation(WebDbObject, WebDbObjectPrecedent);
                rel_7add0ff7_45cceedd_a804f789_3c5a9ae3_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_7add0ff7_45cceedd_a804f789_3c5a9ae3_0_, "WebDbObject_id", "PrecedentWebDbObject_id" ),
            });


                CfDbRelation rel_fd23ea06_4dcd7024_8380f6ba_dc1ff856_0_ = new CfDbRelation(WebDbObject, WebDbObjectPrecedent);
                rel_fd23ea06_4dcd7024_8380f6ba_dc1ff856_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_fd23ea06_4dcd7024_8380f6ba_dc1ff856_0_, "WebDbObject_id", "WebDbObject_id" ),
            });





                WebDbObjectPrecedent.setRelationToParentTables(new CfDbRelation[] {

rel_7add0ff7_45cceedd_a804f789_3c5a9ae3_0_,

rel_fd23ea06_4dcd7024_8380f6ba_dc1ff856_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebDbObjectProcess       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_d620cde0_4b9e6d5a_aea232a7_bdcc9487_0_ = new CfDbRelation(WebDbObject, WebDbObjectProcess);
                rel_d620cde0_4b9e6d5a_aea232a7_bdcc9487_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_d620cde0_4b9e6d5a_aea232a7_bdcc9487_0_, "WebDbObject_id", "WebDbObject_id" ),
            });





                WebDbObjectProcess.setRelationToParentTables(new CfDbRelation[] {

rel_d620cde0_4b9e6d5a_aea232a7_bdcc9487_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebDbObjectQueue       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_80fefe58_44c9e83d_71b5bf9d_80674840_0_ = new CfDbRelation(WebDbObject, WebDbObjectQueue);
                rel_80fefe58_44c9e83d_71b5bf9d_80674840_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_80fefe58_44c9e83d_71b5bf9d_80674840_0_, "WebDbObject_id", "WebDbObject_id" ),
            });





                WebDbObjectQueue.setRelationToParentTables(new CfDbRelation[] {

rel_80fefe58_44c9e83d_71b5bf9d_80674840_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebEvent       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_d6564e8_476a659a_6297a989_2f69a0f9_0_ = new CfDbRelation(WebActivity, WebEvent);
                rel_d6564e8_476a659a_6297a989_2f69a0f9_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_d6564e8_476a659a_6297a989_2f69a0f9_0_, "WebActivity_id", "WebActivity_id" ),
            });


                CfDbRelation rel_69dc9231_4ea8c7aa_ab8b359b_1d10a80d_0_ = new CfDbRelation(WebSession, WebEvent);
                rel_69dc9231_4ea8c7aa_ab8b359b_1d10a80d_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_69dc9231_4ea8c7aa_ab8b359b_1d10a80d_0_, "WebSession_id", "WebSession_id" ),
            });





                WebEvent.setRelationToParentTables(new CfDbRelation[] {

rel_d6564e8_476a659a_6297a989_2f69a0f9_0_,

rel_69dc9231_4ea8c7aa_ab8b359b_1d10a80d_0_,

});

            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_27bbc2f2_45cdc21b_59fd01af_8cbda0c3_0_ = new CfDbRelation(WebEvent, WebFileProcessQueue);
                rel_27bbc2f2_45cdc21b_59fd01af_8cbda0c3_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_27bbc2f2_45cdc21b_59fd01af_8cbda0c3_0_, "WebEvent_id", "WebEvent_id" ),
            });


                CfDbRelation rel_11de4f10_47eddcd2_95540b8_f39dd197_0_ = new CfDbRelation(WebEvent, WebFileProcessEvent);
                rel_11de4f10_47eddcd2_95540b8_f39dd197_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_11de4f10_47eddcd2_95540b8_f39dd197_0_, "WebEvent_id", "WebEvent_id" ),
            });


                ;


                WebEvent.setRelationToChildTables(new CfDbRelation[] {

rel_27bbc2f2_45cdc21b_59fd01af_8cbda0c3_0_,

rel_11de4f10_47eddcd2_95540b8_f39dd197_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebEventMutex       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_dc6644db_42473b03_cab56e9d_266bdc6f_0_ = new CfDbRelation(WebEvent, WebEventMutex);
                rel_dc6644db_42473b03_cab56e9d_266bdc6f_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_dc6644db_42473b03_cab56e9d_266bdc6f_0_, "WebEvent_id", "WebEvent_id" ),
            });





                WebEventMutex.setRelationToParentTables(new CfDbRelation[] {

rel_dc6644db_42473b03_cab56e9d_266bdc6f_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebFileProcessEvent       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_11de4f10_47eddcd2_95540b8_f39dd197_0_ = new CfDbRelation(WebEvent, WebFileProcessEvent);
                rel_11de4f10_47eddcd2_95540b8_f39dd197_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_11de4f10_47eddcd2_95540b8_f39dd197_0_, "WebEvent_id", "WebEvent_id" ),
            });





                WebFileProcessEvent.setRelationToParentTables(new CfDbRelation[] {

rel_11de4f10_47eddcd2_95540b8_f39dd197_0_,

});

            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_d4d20b2_45c3c6d8_f6edb1ba_36f77515_0_ = new CfDbRelation(WebFileProcessEvent, WebSheetProcessEvent);
                rel_d4d20b2_45c3c6d8_f6edb1ba_36f77515_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_d4d20b2_45c3c6d8_f6edb1ba_36f77515_0_, "WebEvent_id", "WebEvent_id" ),
            });


                ;


                WebFileProcessEvent.setRelationToChildTables(new CfDbRelation[] {

rel_d4d20b2_45c3c6d8_f6edb1ba_36f77515_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebFileProcessGroupQuery       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_2443ff46_4ffa35bf_57a867b3_5f7957cc_0_ = new CfDbRelation(WebFileProcessGroupQuery, WebFileProcessQuery);
                rel_2443ff46_4ffa35bf_57a867b3_5f7957cc_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_2443ff46_4ffa35bf_57a867b3_5f7957cc_0_, "WebEvent_id", "WebEvent_id" ),
                new CfDbRelAttribute( rel_2443ff46_4ffa35bf_57a867b3_5f7957cc_0_, "Sheet_Num", "Sheet_Num" ),
                new CfDbRelAttribute( rel_2443ff46_4ffa35bf_57a867b3_5f7957cc_0_, "Query_Num", "Query_Num" ),
            });


                ;


                WebFileProcessGroupQuery.setRelationToChildTables(new CfDbRelation[] {

rel_2443ff46_4ffa35bf_57a867b3_5f7957cc_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebFileProcessQuery       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebFileProcessQueue       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_34b19a27_4cbe4a4e_16ee17ad_dc5c9b0a_0_ = new CfDbRelation(WebQueueMediaType, WebFileProcessQueue);
                rel_34b19a27_4cbe4a4e_16ee17ad_dc5c9b0a_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_34b19a27_4cbe4a4e_16ee17ad_dc5c9b0a_0_, "WebQueueMediaType_id", "WebQueueMediaType_id" ),
            });


                CfDbRelation rel_d3645cdd_43998218_5e48a8e_df39575_0_ = new CfDbRelation(WebDbObject, WebFileProcessQueue);
                rel_d3645cdd_43998218_5e48a8e_df39575_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_d3645cdd_43998218_5e48a8e_df39575_0_, "WebDbObject_id", "WebDbObject_id" ),
            });


                CfDbRelation rel_b6e27ffe_48208e65_590f8c95_1e62fb27_0_ = new CfDbRelation(WebQueueEntryType, WebFileProcessQueue);
                rel_b6e27ffe_48208e65_590f8c95_1e62fb27_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_b6e27ffe_48208e65_590f8c95_1e62fb27_0_, "WebQueueEntryType_id", "WebQueueEntryType_id" ),
            });


                CfDbRelation rel_48f84de1_4e246c54_ace43b98_fcc78966_0_ = new CfDbRelation(WebFileProcessStatus, WebFileProcessQueue);
                rel_48f84de1_4e246c54_ace43b98_fcc78966_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_48f84de1_4e246c54_ace43b98_fcc78966_0_, "WebFileProcessStatus_id", "WebFileProcessStatus_id" ),
            });


                CfDbRelation rel_27bbc2f2_45cdc21b_59fd01af_8cbda0c3_0_ = new CfDbRelation(WebEvent, WebFileProcessQueue);
                rel_27bbc2f2_45cdc21b_59fd01af_8cbda0c3_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_27bbc2f2_45cdc21b_59fd01af_8cbda0c3_0_, "WebEvent_id", "WebEvent_id" ),
            });





                WebFileProcessQueue.setRelationToParentTables(new CfDbRelation[] {

rel_27bbc2f2_45cdc21b_59fd01af_8cbda0c3_0_,

rel_34b19a27_4cbe4a4e_16ee17ad_dc5c9b0a_0_,

rel_d3645cdd_43998218_5e48a8e_df39575_0_,

rel_b6e27ffe_48208e65_590f8c95_1e62fb27_0_,

rel_48f84de1_4e246c54_ace43b98_fcc78966_0_,

});

            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_36ca26df_491eb1cc_69dfbeac_2cba9b9b_0_ = new CfDbRelation(WebFileProcessQueue, WebFileProcessService);
                rel_36ca26df_491eb1cc_69dfbeac_2cba9b9b_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_36ca26df_491eb1cc_69dfbeac_2cba9b9b_0_, "WebEvent_id", "WebEvent_id" ),
            });


                CfDbRelation rel_8d64f967_4545ca82_cc2b1_92c511e2_0_ = new CfDbRelation(WebFileProcessQueue, WebFileProcessSheetGroup);
                rel_8d64f967_4545ca82_cc2b1_92c511e2_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_8d64f967_4545ca82_cc2b1_92c511e2_0_, "WebEvent_id", "WebEvent_id" ),
            });


                ;


                WebFileProcessQueue.setRelationToChildTables(new CfDbRelation[] {

rel_36ca26df_491eb1cc_69dfbeac_2cba9b9b_0_,

rel_8d64f967_4545ca82_cc2b1_92c511e2_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebFileProcessQueueLockAccess       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_45c75c70_48f55a4d_2bc16998_513a2985_0_ = new CfDbRelation(WebFileProcessQueue, WebFileProcessQueueLockAccess);
                rel_45c75c70_48f55a4d_2bc16998_513a2985_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_45c75c70_48f55a4d_2bc16998_513a2985_0_, "WebEvent_id", "WebEvent_id" ),
            });





                WebFileProcessQueueLockAccess.setRelationToParentTables(new CfDbRelation[] {

rel_45c75c70_48f55a4d_2bc16998_513a2985_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebFileProcessQueuePrecedent       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_e6ce34fb_4be305dc_b3f1687_5ea9eaf8_0_ = new CfDbRelation(WebFileProcessQueue, WebFileProcessQueuePrecedent);
                rel_e6ce34fb_4be305dc_b3f1687_5ea9eaf8_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_e6ce34fb_4be305dc_b3f1687_5ea9eaf8_0_, "WebEvent_id", "PrecedentWebEvent_id" ),
            });


                CfDbRelation rel_8048ec03_40d34b27_be365fa5_8ffd3e68_0_ = new CfDbRelation(WebFileProcessQueue, WebFileProcessQueuePrecedent);
                rel_8048ec03_40d34b27_be365fa5_8ffd3e68_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_8048ec03_40d34b27_be365fa5_8ffd3e68_0_, "WebEvent_id", "WebEvent_id" ),
            });





                WebFileProcessQueuePrecedent.setRelationToParentTables(new CfDbRelation[] {

rel_e6ce34fb_4be305dc_b3f1687_5ea9eaf8_0_,

rel_8048ec03_40d34b27_be365fa5_8ffd3e68_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebFileProcessService       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_36ca26df_491eb1cc_69dfbeac_2cba9b9b_0_ = new CfDbRelation(WebFileProcessQueue, WebFileProcessService);
                rel_36ca26df_491eb1cc_69dfbeac_2cba9b9b_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_36ca26df_491eb1cc_69dfbeac_2cba9b9b_0_, "WebEvent_id", "WebEvent_id" ),
            });





                WebFileProcessService.setRelationToParentTables(new CfDbRelation[] {

rel_36ca26df_491eb1cc_69dfbeac_2cba9b9b_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebFileProcessSheetGroup       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_8d64f967_4545ca82_cc2b1_92c511e2_0_ = new CfDbRelation(WebFileProcessQueue, WebFileProcessSheetGroup);
                rel_8d64f967_4545ca82_cc2b1_92c511e2_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_8d64f967_4545ca82_cc2b1_92c511e2_0_, "WebEvent_id", "WebEvent_id" ),
            });





                WebFileProcessSheetGroup.setRelationToParentTables(new CfDbRelation[] {

rel_8d64f967_4545ca82_cc2b1_92c511e2_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebFileProcessStatus       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_48f84de1_4e246c54_ace43b98_fcc78966_0_ = new CfDbRelation(WebFileProcessStatus, WebFileProcessQueue);
                rel_48f84de1_4e246c54_ace43b98_fcc78966_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_48f84de1_4e246c54_ace43b98_fcc78966_0_, "WebFileProcessStatus_id", "WebFileProcessStatus_id" ),
            });


                ;


                WebFileProcessStatus.setRelationToChildTables(new CfDbRelation[] {

rel_48f84de1_4e246c54_ace43b98_fcc78966_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebQueueEntryType       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_b6e27ffe_48208e65_590f8c95_1e62fb27_0_ = new CfDbRelation(WebQueueEntryType, WebFileProcessQueue);
                rel_b6e27ffe_48208e65_590f8c95_1e62fb27_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_b6e27ffe_48208e65_590f8c95_1e62fb27_0_, "WebQueueEntryType_id", "WebQueueEntryType_id" ),
            });


                ;


                WebQueueEntryType.setRelationToChildTables(new CfDbRelation[] {

rel_b6e27ffe_48208e65_590f8c95_1e62fb27_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebQueueMediaType       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_34b19a27_4cbe4a4e_16ee17ad_dc5c9b0a_0_ = new CfDbRelation(WebQueueMediaType, WebFileProcessQueue);
                rel_34b19a27_4cbe4a4e_16ee17ad_dc5c9b0a_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_34b19a27_4cbe4a4e_16ee17ad_dc5c9b0a_0_, "WebQueueMediaType_id", "WebQueueMediaType_id" ),
            });


                ;


                WebQueueMediaType.setRelationToChildTables(new CfDbRelation[] {

rel_34b19a27_4cbe4a4e_16ee17ad_dc5c9b0a_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebRole       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_a4aa1231_4c54ddf3_18f6d3aa_113a3f11_0_ = new CfDbRelation(WebRole, WebSessionRoles);
                rel_a4aa1231_4c54ddf3_18f6d3aa_113a3f11_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_a4aa1231_4c54ddf3_18f6d3aa_113a3f11_0_, "WebRole_id", "WebRole_id" ),
            });


                ;


                WebRole.setRelationToChildTables(new CfDbRelation[] {

rel_a4aa1231_4c54ddf3_18f6d3aa_113a3f11_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebSession       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_d0215d97_4c8d767b_1056558c_764648f4_0_ = new CfDbRelation(WebUser, WebSession);
                rel_d0215d97_4c8d767b_1056558c_764648f4_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_d0215d97_4c8d767b_1056558c_764648f4_0_, "WebUser_id", "ManagerWebUser_id" ),
            });


                CfDbRelation rel_8139917f_46a5a0a7_ad1c2781_86a6d639_0_ = new CfDbRelation(WebUser, WebSession);
                rel_8139917f_46a5a0a7_ad1c2781_86a6d639_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_8139917f_46a5a0a7_ad1c2781_86a6d639_0_, "WebUser_id", "WebUser_id" ),
            });





                WebSession.setRelationToParentTables(new CfDbRelation[] {

rel_d0215d97_4c8d767b_1056558c_764648f4_0_,

rel_8139917f_46a5a0a7_ad1c2781_86a6d639_0_,

});

            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_fae6adb8_4f6d33c9_5df41b85_f0fcae1c_0_ = new CfDbRelation(WebSession, WebSessionRoles);
                rel_fae6adb8_4f6d33c9_5df41b85_f0fcae1c_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_fae6adb8_4f6d33c9_5df41b85_f0fcae1c_0_, "WebSession_id", "WebSession_id" ),
            });


                CfDbRelation rel_69dc9231_4ea8c7aa_ab8b359b_1d10a80d_0_ = new CfDbRelation(WebSession, WebEvent);
                rel_69dc9231_4ea8c7aa_ab8b359b_1d10a80d_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_69dc9231_4ea8c7aa_ab8b359b_1d10a80d_0_, "WebSession_id", "WebSession_id" ),
            });


                ;


                WebSession.setRelationToChildTables(new CfDbRelation[] {

rel_fae6adb8_4f6d33c9_5df41b85_f0fcae1c_0_,

rel_69dc9231_4ea8c7aa_ab8b359b_1d10a80d_0_,

});

            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebSessionRoles       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_a4aa1231_4c54ddf3_18f6d3aa_113a3f11_0_ = new CfDbRelation(WebRole, WebSessionRoles);
                rel_a4aa1231_4c54ddf3_18f6d3aa_113a3f11_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_a4aa1231_4c54ddf3_18f6d3aa_113a3f11_0_, "WebRole_id", "WebRole_id" ),
            });


                CfDbRelation rel_fae6adb8_4f6d33c9_5df41b85_f0fcae1c_0_ = new CfDbRelation(WebSession, WebSessionRoles);
                rel_fae6adb8_4f6d33c9_5df41b85_f0fcae1c_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_fae6adb8_4f6d33c9_5df41b85_f0fcae1c_0_, "WebSession_id", "WebSession_id" ),
            });





                WebSessionRoles.setRelationToParentTables(new CfDbRelation[] {

rel_a4aa1231_4c54ddf3_18f6d3aa_113a3f11_0_,

rel_fae6adb8_4f6d33c9_5df41b85_f0fcae1c_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebSheetCheckPointProcessEvent       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebSheetProcessEvent       
            ******************************************************************************/



            {// Parent Tables.


                CfDbRelation rel_d4d20b2_45c3c6d8_f6edb1ba_36f77515_0_ = new CfDbRelation(WebFileProcessEvent, WebSheetProcessEvent);
                rel_d4d20b2_45c3c6d8_f6edb1ba_36f77515_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_d4d20b2_45c3c6d8_f6edb1ba_36f77515_0_, "WebEvent_id", "WebEvent_id" ),
            });





                WebSheetProcessEvent.setRelationToParentTables(new CfDbRelation[] {

rel_d4d20b2_45c3c6d8_f6edb1ba_36f77515_0_,

});

            } //Parent Tables


            {// Child Tables.



            }// Child Tables.


            /******************************************************************************
            ** Tabla  = WebUser       
            ******************************************************************************/



            {// Parent Tables.



            } //Parent Tables


            {// Child Tables.


                CfDbRelation rel_d0215d97_4c8d767b_1056558c_764648f4_0_ = new CfDbRelation(WebUser, WebSession);
                rel_d0215d97_4c8d767b_1056558c_764648f4_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_d0215d97_4c8d767b_1056558c_764648f4_0_, "WebUser_id", "ManagerWebUser_id" ),
            });


                CfDbRelation rel_8139917f_46a5a0a7_ad1c2781_86a6d639_0_ = new CfDbRelation(WebUser, WebSession);
                rel_8139917f_46a5a0a7_ad1c2781_86a6d639_0_.setRelAtts(new CfDbRelAttribute[] {
                new CfDbRelAttribute( rel_8139917f_46a5a0a7_ad1c2781_86a6d639_0_, "WebUser_id", "WebUser_id" ),
            });


                ;


                WebUser.setRelationToChildTables(new CfDbRelation[] {

rel_d0215d97_4c8d767b_1056558c_764648f4_0_,

rel_8139917f_46a5a0a7_ad1c2781_86a6d639_0_,

});

            }// Child Tables.




        } // static DbModel

    } // class DbModel
} // namespace WebAsistida.lib