using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;


namespace WebAsistida.lib
{
	public class CfDbTable
	{
		public Parameters Parametros()
		{
			Parameters Parametros = new Parameters();

			Parametros.Parametros();

			return Parametros;
		}

		//---------------------------------------------------------------------------------
		// Members.
		public String DbName;
		public String TbName;
		public CfDbColumn[] DbCols;
		//System.Collections.Generic.List<CfDbColumn> DbCols2;
		public bool isReadOnly; // Indica si puede actualizarse o no..
		public bool isHist;

		/// <summary>
		/// ExceptionTable_Ind
		/// Se utiliza para distingir tablas historicas.
		/// Hay Tres casos:
		/// 
		/// Si vale 'N'
		/// La tabla es historica normal, si en el import viene valores de start_date con el current date, se toma;
		/// si viene con start_date mayores al dia de la fecha, se rechaza; y
		/// si el start date es menor al dia de la fecha, se toma el dia de la fecha.
		/// 
		/// Si vale 'S'
		/// La tabla es historica de excepciones, si en el import vienen valores de start_date con current_date o posterior, se toma;
		/// si viene con start_date menor a current_date, se toma el dia de la fecha.
		/// 
		/// Si vale 'I'.
		/// La tabla realmente no es historica. Tiene fecha de inicio y fin pero solo para marcar periodos,
		/// por ejemplo ExceptionalSales. Se toman los valores como se informan.
		/// 
		/// Estas validaciones hay que hacerlas al importar.
		/// En la aplicacion, deberian generar una excepcion.
		/// </summary>
		public String ExceptionTable_Ind;

		public String TbLocalName; // Local name, used in massive loads and extracts.

		public bool RejectedOverlap_Ind; // No me permite borrar el dato cuando ya existe.

		public bool ExpQuery_Ind; // Indica si la tabla tiene export query.

		public String ParentJointType; //Indica el tipo de join con la tabla padre.

		public bool StgCollStat; //Indica si se colectan estadisticas para la tabla en staging.

		// Datos que se reutilizaran varias veces.
		public int PkFldCnt;   //Cantidad de columnas de la pk.
		public int NoPkFldCnt; //Cantidad de columnas que no son pk.

		public CfDbColumn[] PkColumns; // Nombre de columnas de la pk.
		public CfDbColumn[] NoPkColumns; // Nombre de columnas que no son pk.
		public CfDbColumn StartDt_Column; // Nombre de columna que indica el inicio de vigencia un registro.
		public CfDbColumn EndDt_Column; // Nombre de columna que indica el fin de vigencia un registro.

		/* 
		 * Tablas lookup con datos adicionales.
		 * Solo se utilizan para lecturas (metodos list y read).
		 * La pk de estas tablas deben coincidir con columnas de
		 * la tabla principal, para que puedan joinearse.
		 * Las tablas secundarias se joinean solo con la principal,
		 * no entre si. Por lo que si hay varios niveles de anidamiento,
		 * hacer una vista que presente todo como una sola tabla.
		*/
		public CfDbTable[] lkpTabs;

		/*
		 * Para definir el orden de filas, se recibe un array de strings con las
		 * columnas que se usaran para el order by.
		*/
		public String[] orderCols;

		/*
		 * Para definir que columnas exportar, se recibe un array de strings con las
		 * columnas.
		*/
		public String[] exportCols;

		/*
		 * Para acrecentar con descripciones, se cuenta con las tablas padre
		 * de las foreign key de la tabla.
		*/
		public CfDbRelation[] relParentTabs;

		/*
		 * Las siguientes tablas tienen como fk, a la clave de esta tabla. Y pueden
		 * usarse para filtrar registros de esta tabla, a traves de una subquery.
		*/
		public CfDbRelation[] relChildTabs;

		/*
		 * Las siguientes tablas tienen como foreign key a la clave de esta, aunque no
		 * necesariamente sean hijas. Tambien pueden
		 * usarse para filtrar registros de esta tabla, a traves de una subquery.
		*/
		public CfDbTable[] filtTabs;


		/// <summary>
		/// Las siguientes tablas tienen la particularidad de seguir el mismo
		/// ritmo de actualizacion que la tabla "this", y suelen compartir parte
		/// de la clave primaria.
		/// Si se crea un registro en la tabla "this", si existen registros en estas
		/// tablas que se superpongan, se desdobla ese registro, para mantener la
		/// correspondencia.
		/// Ejemplo: para PurchaseCondition, la pace follower es "PurchaseConditionWDayHist.
		/// </summary>
		public CfDbTable[] paceFollowerTabs;
		public CfDbTable paceFollowingTab; // Current table follows this table.


		/// <summary>
		/// Si la tabla corriente es una excepcion, indica cual es la tabla de datos normales.
		/// </summary>
		public CfDbTable NormalTab;

		/// <summary>
		///  Si la tabla corriente es normal, indica cual es su tabla de excepcion, si la tuviera.
		/// </summary>
		public CfDbTable ExceptionTab;

		/*
		 * Para listar filas, se contemplan tres opciones, listar todo lo disponible,
		 * por defecto cuando la tabla no tiene historia; y cuando la tabla tiene historia,
		 * estan las opciones de listar lo vigente, y listar lo vigente mas lo futuro.
		 * Si la tabla tabla no tiene historia, todas las opciones deben producir el mismo
		 * resultado.
		*/
		public enum ListOption { LIST_ALL, LIST_CURRENT, LIST_CURRENT_AND_FUTURE };
		public enum OrderByOption { WITH_ORDER_BY, WITHOUT_ORDER_BY };
		public enum LockByOption { WITH_LOCKING, WITHOUT_LOCKING };

		/*
		 * Opciones de columnas a listar. Por defecto, o para exportar.
		 * Si no se informan explicitamente columnas para exportar,
		 * es equivalente a la opcion por defecto.
		*/
		public enum ColumnListOption { DEFAULT_COLUMNS, EXPORT_COLUMNS };


		public CfDbColumn TrkEvent_Column; // Nombre de columna de evento que inserta/actualiza registro.
		public CfDbColumn TrkUser_Column; // Nombre de columna de usuario que inserta/actualiza registro.
		public CfDbColumn TrkDt_Column; // Nombre de columna de fecha de insercion/actualizacion de registro.
		public CfDbColumn TrkTm_Column; // Nombre de columna de hora de insercion/actualizacion de registro.

		//Funciones que retornar instrucciones sql a ejecutar luego de un upsert.
		public CfDbQryTxtParamFunc[] AfterUpsertQryTxt;
		public CfDbQryTxtProcessParamFunc AfterUpsertProcessQryTxt;

		//Funciones que retornar instrucciones sql a ejecutar antes de un upsert.
		public CfDbQryTxtParamFunc[] BeforeUpsertQryTxt;
		public CfDbQryTxtProcessParamFunc BeforeUpsertProcessQryTxt;

		//Funciones que retornar instrucciones sql a ejecutar luego de un delete.
		public CfDbQryTxtParamFunc[] AfterDeleteQryTxt;
		public CfDbQryTxtProcessParamFunc AfterDeleteProcessQryTxt;

		//Funciones que retornar instrucciones sql a ejecutar antes de un delete.
		public CfDbQryTxtParamFunc[] BeforeDeleteQryTxt;
		public CfDbQryTxtProcessParamFunc BeforeDeleteProcessQryTxt;

		//Funciones que retornan instrucciones sql a ejecutar luego de un metodo particular de pedidos forzados.
		public CfDbQryTxtImportProcessFunc[] AfterUpdateProcessFirstStepQryTxt;
		public CfDbQryTxtImportProcessFunc[] AfterUpdateProcessSecondStepQryTxt;

		//Funciones para ejecutar en carga masiva.
		public CfStageTableDbQryTxtFunc[] ImportStageTableQryTxt; // Funciones de validacion extra en carga masiva.
		public CfStageTableDbQryTxtFunc[] DeleteStageTableQryTxt;
		public CfAfterStageChckQryTxtFunc[] AfterStageChckQryTxt;
		public CfStageImportDbQryTxtFunc[] AfterFldImportProcessQryTxt;
		public CfDbQryTxtImportProcessParamFunc[] AfterImportUpsertProcessQryTxt;
		public CfDbQryTxtImportProcessParamFunc[] AfterImportDeleteProcessQryTxt;
		public CfDbImportQryTxtFunc[] BeforeImportUpsertQryTxt;
		public CfDbImportQryTxtFunc[] BeforeImportDeleteQryTxt;
		public CfDbQryTextValidateProcessFunc AfterUpsertDeleteValidateProcessQryTxt;
		public CfDbQryTextPreTransmissionFunc AfterImportLockTransmissionQryTxt;
		public CfDbQryTextPreTransmissionFunc AfterImportPreTransmissionQryTxt;
		public CfDbQryTxtImportTransmissionFunc[] AfterImportUpsertTransQryTxt; // Para transmitir dps de un import upsert.
		public CfDbQryTxtImportProcessFunc[] BeforeImportUpsertUpdateQryTxt;
		public CfDbQryTxtImportProcessFunc[] AfterImportUpsertUpdateQryTxt;
		// Se dividen el after forzados en 5 steps para armar 4 transacciones y una ejecucion normal de procedures
		public CfDbQryTxtImportProcessFunc AfterImportUpsertFirstStepFrcdRqstQryTxt;
		public CfDbQryTxtImportProcessFunc AfterImportUpsertSecondStepFrcdRqstQryTxt;
		public CfDbQryTxtImportFrcdRqstFunc[] AfterImportUpsertThirdStepFrcdRqstQryTxt;
		public CfDbQryTxtImportProcessFunc[] AfterImportUpsertFourthStepFrcdRqstQryTxt;
		public CfDbQryTxtImportProcessFunc[] AfterImportUpsertFifthStepFrcdRqstQryTxt;

		//---------------------------------------------------------------------------------
		//Si la tabla tiene una clase extradata, invoca el constructor.
		private void UpdateExtraData(String instanceName = null)
		{
			String eff_instanceName = String.IsNullOrEmpty(instanceName) ? this.TbName : instanceName;
			String ExtraDataClassName = "WebAsistida.lib.DbModelExtraDataTables." + eff_instanceName + "ExtraData";
			String ExtraDataMethodName = "ExtraData";
			Type[] ExtraDataMethodParamTypes = new Type[] { this.GetType() };

			Type ExtraDataClass = Type.GetType(ExtraDataClassName, false, true);

			if (ExtraDataClass != null)
			{
				MethodInfo meth = ExtraDataClass.GetMethod(ExtraDataMethodName, ExtraDataMethodParamTypes);
				if (meth != null)
				{
					object[] parlist = new object[] { this };
					meth.Invoke(null, parlist);
				}
			}
		}

		// Constructor
		public CfDbTable(String _DbName, String _TbName, bool _isReadOnly = false, bool _isHist = false, String _ExceptionTable_Ind = "N", 
				String _localName = null, bool _RejectOverlap_Ind = false, bool _ExpQuery_Ind = false, String _ParentJointType = "Default", 
				bool _Stg_Coll_Stat = false)
		{
			String schema_name = Parametros().strDbSchema;

			if (_DbName.Equals("CDW_R01", StringComparison.CurrentCultureIgnoreCase) && schema_name != null )
			{
				DbName = schema_name; //La base de Web es diferente en desarrollo.
			}
			else
			{
				DbName = _DbName;
			}

			TbName = _TbName;
			isReadOnly = _isReadOnly;
			isHist = _isHist;
			ExceptionTable_Ind = _ExceptionTable_Ind;

			if (_localName == null || _localName.Equals("") )
			{
				TbLocalName = _TbName;
			}
			else
			{
				TbLocalName = _localName;
			}

			RejectedOverlap_Ind = _RejectOverlap_Ind;
			ExpQuery_Ind = _ExpQuery_Ind;
			ParentJointType = _ParentJointType;
			StgCollStat = _Stg_Coll_Stat;

			this.UpdateExtraData();
		}

		//---------------------------------------------------------------------------------
		// Copy Constructor
		protected CfDbTable( CfDbTable prevTable, String instanceName )
		{
			// ++ Equivalente a constructor principal
			this.DbName = prevTable.DbName;
			this.TbName = prevTable.TbName;
			this.isReadOnly = prevTable.isReadOnly;
			this.isHist = prevTable.isHist;
			this.ExceptionTable_Ind = prevTable.ExceptionTable_Ind;
			this.TbLocalName = prevTable.TbLocalName;

			this.RejectedOverlap_Ind = prevTable.RejectedOverlap_Ind;
			this.ExpQuery_Ind = prevTable.ExpQuery_Ind;
			this.ParentJointType = prevTable.ParentJointType;
			this.StgCollStat = prevTable.StgCollStat;

			this.PkFldCnt = prevTable.PkFldCnt;
			this.NoPkFldCnt = prevTable.NoPkFldCnt;

			if (prevTable.DbCols != null && this.DbCols == null)
			{
				CfDbColumn[] AuxColumns = new CfDbColumn[prevTable.DbCols.Length];
				for (int i = 0; i < prevTable.DbCols.Length; i++)
				{
					AuxColumns[i] = new CfDbColumn(prevTable.DbCols[i]);
				}
				//Array.Copy(prevTable.DbCols, AuxColumns, prevTable.DbCols.Length);
				this.setColumns(AuxColumns);
			}

			this.UpdateExtraData(instanceName);
			
			//El resto se inicializa en el mismo orden que en constructor de DbModel.
			// Defino primero las columnas ya que las necesito para redefinir los localnames particulares.
			if (prevTable.orderCols != null)
			{
				String[] AuxOrderByColumns = new String[prevTable.orderCols.Length];
				Array.Copy(prevTable.orderCols, AuxOrderByColumns, prevTable.orderCols.Length);
				this.setOrderByColumns(AuxOrderByColumns);
			}

			if (prevTable.relParentTabs != null)
			{
				CfDbRelation[] AuxRelParentTabs = new CfDbRelation[prevTable.relParentTabs.Length];
				Array.Copy(prevTable.relParentTabs, AuxRelParentTabs, prevTable.relParentTabs.Length);
				this.setRelationToParentTables(AuxRelParentTabs);
			}

			if (prevTable.relChildTabs != null)
			{
				CfDbRelation[] AuxRelChildTab = new CfDbRelation[prevTable.relChildTabs.Length];
				Array.Copy(prevTable.relChildTabs, AuxRelChildTab, prevTable.relChildTabs.Length);
				this.setRelationToChildTables(AuxRelChildTab);
			}
		}

		//---------------------------------------------------------------------------------
		public CfDbTable Clone( String instancelName )
		{
			CfDbTable aux = new CfDbTable(this, instancelName);
			return aux;
		}
		//---------------------------------------------------------------------------------

		private void initTblInfo()
		{
			int pk_cnt = 0;
			int no_pk_cnt = 0;

			foreach (CfDbColumn c in this.DbCols)
			{
				if (c.isStartDt) continue;
				if (c.isEndDt) continue;

				if (c.isPk)
				{
					PkFldCnt++;
					continue;
				}
				NoPkFldCnt++;
			}

			PkColumns = new CfDbColumn[PkFldCnt];
			NoPkColumns = new CfDbColumn[NoPkFldCnt];

			// Preselecciono los campos que tengo que usar varias veces.
			foreach (CfDbColumn c in this.DbCols)
			{
				if (c.isStartDt)
				{
					StartDt_Column = c;
					continue;
				}
				if( c.isEndDt )
				{
					EndDt_Column = c;
					continue;
				}
				if( c.isPk )
				{
					PkColumns[pk_cnt] = c;
					pk_cnt++;
					continue;
				}
				NoPkColumns[no_pk_cnt] = c;
				no_pk_cnt++;

				// Tracking. Se consideraran parte del grupo no_pk.
				if( c.isTrkEvent )
				{
					TrkEvent_Column = c;
				}
				if( c.isTrkUser )
				{
					TrkUser_Column = c;
				}
				if( c.isTrkDt )
				{
					TrkDt_Column = c;
				}
				if( c.isTrkTm )
				{
					TrkTm_Column = c;
				}
			}
		}


		//---------------------------------------------------------------------------------

		public CfDbColumn[] setColumns(CfDbColumn[] _DbCols)
		{
			CfDbColumn[] temp = this.DbCols;
			this.DbCols = _DbCols;

			//Inicializa datos de uso comun.
			this.initTblInfo();

			return temp;
		}


		public CfDbColumn getColumn( String _columnName )
		{
			foreach (CfDbColumn c in this.DbCols)
			{
				if( c.ColName.ToLower().Equals(_columnName.ToLower()))
				{
					return c;
				}
			}
			return null;
		}
		//---------------------------------------------------------------------------------

		public String[] setOrderByColumns(String[] _orderCols)
		{
			String[] temp = this.orderCols;

			orderCols = _orderCols;
			return temp;
		}

		//---------------------------------------------------------------------------------

		public String[] setExportColumns(String[] _exportCols)
		{
			String[] temp = this.exportCols;

			exportCols = _exportCols;
			return temp;
		}

		public void setColumnsLocalNames(Dictionary<String, String> _collocalnames)
		{
			for (int i = 0; i < this.DbCols.Length; i++)
			{
				String locName = "";

				if (_collocalnames.TryGetValue(this.DbCols[i].ColName, out locName))
				{
					this.DbCols[i].ColLocalName = locName;
				}
			}
		}

		//---------------------------------------------------------------------------------
		//---------------------------------------------------------------------------------
		public CfDbTable[] setExtraDataTables(CfDbTable[] newLkpTabs)
		{
			CfDbTable[] temp = this.lkpTabs;

			this.lkpTabs = newLkpTabs;
			return temp;
		}

		//---------------------------------------------------------------------------------
		public CfDbTable[] setFilterTables(CfDbTable[] newFiltTabs)
		{
			CfDbTable[] temp = this.filtTabs;

			this.filtTabs = newFiltTabs;
			return temp;
		}

		//---------------------------------------------------------------------------------
		public CfDbTable[] setPaceFollowerTables(CfDbTable[] newPaceFollowerTabs)
		{
			CfDbTable[] temp = this.paceFollowerTabs;

			this.paceFollowerTabs = newPaceFollowerTabs;

			foreach (CfDbTable tab in newPaceFollowerTabs)
			{
				tab.paceFollowingTab = this;
			}

			return temp;
		}

		//---------------------------------------------------------------------------------
		public void setNormalTable(CfDbTable normTab)
		{
			if( ! this.ExceptionTable_Ind.Equals("Y",StringComparison.CurrentCultureIgnoreCase))
			{
				// this debe ser una tabla de excepcion.
				throw new Exception("Seteo de tabla normal invalido, para tabla que no es excepcion.");
			}

			if( ! normTab.ExceptionTable_Ind.Equals("N", StringComparison.CurrentCultureIgnoreCase))
			{
				// normTab debe ser una tabla normal.
				throw new Exception("Seteo de tabla normal invalido. La tabla informada como normal, no lo es.");
			}

			this.NormalTab = normTab;
			normTab.ExceptionTab = this;
		}
		//---------------------------------------------------------------------------------

		public void setRelationToParentTables(CfDbRelation[] _relParentTabs)
		{
			this.relParentTabs = _relParentTabs;
		}

		public void setRelationToChildTables(CfDbRelation[] _relChildTabs)
		{
			this.relChildTabs = _relChildTabs;
		}

		//---------------------------------------------------------------------------------
		public CfDbColumn searchColumnInOwnCols(String _colName)
		{
			foreach (CfDbColumn c in this.DbCols)
			{
				if ( _colName.ToLower().Equals(c.ColName.ToLower()))
				{
					return c;
				}
			}
			return null;
		}
		//---------------------------------------------------------------------------------
		public CfDbColumn searchColumnInParentTableDescCols(String _colName)
		{
			if (this.relParentTabs == null)
			{
				return null;
			}

			foreach (CfDbRelation r in this.relParentTabs)
			{
				foreach (CfDbColumn c in r.ParentTable.DbCols)
				{
					if (c.isDesc && _colName.ToLower().Equals( c.ColName.ToLower())) // Se agrega si la columna es descripcion.
					{
						return c;
					}
				}
			}
			return null;
		}
		//---------------------------------------------------------------------------------
		//searchColumnInChildTables
		//---------------------------------------------------------------------------------
		public CfDbRelAttribute searchRelAttInChildTables(String _colName)
		{
			if( this.relChildTabs == null )
			{
				return null;
			}

			foreach (CfDbRelation r in this.relChildTabs)
			{
				/*Esto puede servir para otro.
				foreach (CfDbColumn c in r.ChildTable.DbCols)
				{
					if ( _colName.ToLower().Equals(c.ColName.ToLower()) )
					{
						return c;
					}
				}
				*/
				foreach (CfDbRelAttribute ra in r.RelAtts)
				{
					if( _colName.ToLower().Equals(ra.ParentColumn.ColName.ToLower()) )
					{
						return ra;
					}
				}
			}
			return null;
		}

		//---------------------------------------------------------------------------------
		public CfDbColumn searchColumnInFilterTables(String _colName)
		{
			if (this.filtTabs == null)
			{
				return null;
			}

			foreach (CfDbTable t in this.filtTabs)
			{
				foreach (CfDbColumn c in t.DbCols)
				{
					if (_colName.ToLower().Equals(c.ColName.ToLower()))
					{
						return c;
					}
				}
			}
			return null;
		}

		//---------------------------------------------------------------------------------

		public CfDbColumn searchColumnInExtraDataTables(String _colName)
		{
			if (this.lkpTabs == null)
			{
				return null;
			}

			foreach (CfDbTable t in this.lkpTabs )
			{
				foreach (CfDbColumn c in t.DbCols)
				{
					if ( _colName.ToLower().Equals(c.ColName.ToLower()))
					{
						return c;
					}
				}
			}
			return null;
		}
		//---------------------------------------------------------------------------------

		

		//Genera string de consulta.
		public String listQryText(JObject param, ListOption list_option, OrderByOption orderby_option, LockByOption lock_option, ColumnListOption clmnlist_option)
		{
			// Armo string de consulta.
			String sep = " ";
			int tabCnt = 0;                         // Contador para alias de tabla. 0 es la principal.
			String tabStr = "t" + tabCnt.ToString("D2");  // Contador en string para alias de tabla.
			String tabMainStr = tabStr;                // Siempre apunta a la tabla principal.
			String qry_txt = "";
			List<String> dbAliasList = new List<String>(); //Lista de alias para validar si las columnas de filtro estan disponibles.
			List<String> uniqueColList = new List<String>();
			List<String> strTbNameList = new List<String>();
			List<ColumnProperties> ltColumnSel = new List<ColumnProperties>();
			List<TableProperties> ltAllTables = new List<TableProperties>();
			List<TableProperties> ltTableSel = new List<TableProperties>();

			// Aqui se lista los lockeos
			if (lock_option == LockByOption.WITH_LOCKING)
			{
				strTbNameList.Add(this.TbName);
				qry_txt += "LOCKING TABLE ";
				qry_txt += this.DbName;
				qry_txt += ".";
				qry_txt += this.TbName;
				qry_txt += " FOR ACCESS";
				qry_txt += "\n";

				if (this.relParentTabs != null)
				{
					foreach (CfDbRelation r in this.relParentTabs)
					{
						if (!strTbNameList.Exists(x => x.Equals(r.ParentTable.TbName, StringComparison.CurrentCultureIgnoreCase)))
						{
							strTbNameList.Add(r.ParentTable.TbName);
							qry_txt += "LOCKING TABLE ";
							qry_txt += r.ParentTable.DbName;
							qry_txt += ".";
							qry_txt += r.ParentTable.TbName;
							qry_txt += " FOR ACCESS";
							qry_txt += "\n";


							// Si la tabla padre tiene una columna que
							// es descripcion y ademas foreign key, tambien
							// joineo su padre.
							foreach (CfDbColumn c in r.ParentTable.DbCols)
							{
								if (c.isDesc && c.isFk) // Si tiene una columna descripcion y fk 
								{
									// tengo que joinear al padre, porque inclui su descripcion.

									CfDbRelation grandParentRelation = null;
									CfDbTable grandParentTable = null;
									//Busco cual es la tabla que origina esta fk de la tabla padre.
									foreach (CfDbRelation r2 in r.ParentTable.relParentTabs)
									{
										//Busco el atributo.
										foreach (CfDbRelAttribute ra2 in r2.RelAtts)
										{
											// Comparo con ChildColumn, porque es el nombre que migro a la tabla
											if (c.ColName.Equals(ra2.ChildColumn.ColName, StringComparison.CurrentCultureIgnoreCase))
											{
												grandParentRelation = r2;
												grandParentTable = ra2.ParentColumn.DbTable;
											}
										}
									}
									if (grandParentTable != null)
									{
										if (!strTbNameList.Exists(x => x.Equals(grandParentTable.TbName, StringComparison.CurrentCultureIgnoreCase)))
										{
											strTbNameList.Add(grandParentTable.TbName);
											qry_txt += "LOCKING TABLE ";
											qry_txt += grandParentTable.DbName;
											qry_txt += ".";
											qry_txt += grandParentTable.TbName;
											qry_txt += " FOR ACCESS";
											qry_txt += "\n";
										}
									}
								}
							}
						}
					}
				}

				if (this.lkpTabs != null)
				{
					foreach (CfDbTable t in this.lkpTabs)
					{
						if (!strTbNameList.Exists(x => x.Equals(t.TbName, StringComparison.CurrentCultureIgnoreCase)))
						{
							strTbNameList.Add(t.TbName);
							qry_txt += "LOCKING TABLE ";
							qry_txt += t.DbName;
							qry_txt += ".";
							qry_txt += t.TbName;
							qry_txt += " FOR ACCESS";
							qry_txt += "\n";
						}
					}
				}

				if (this.filtTabs != null)
				{
					foreach (CfDbTable t in this.filtTabs)
					{
						if (!strTbNameList.Exists(x => x.Equals(t.TbName, StringComparison.CurrentCultureIgnoreCase)))
						{
							strTbNameList.Add(t.TbName);
							qry_txt += "LOCKING TABLE ";
							qry_txt += t.DbName;
							qry_txt += ".";
							qry_txt += t.TbName;
							qry_txt += " FOR ACCESS";
							qry_txt += "\n";
						}
					}
				}
			}

			qry_txt += "SELECT\n";

			//Chequeo si informo lista de columnas.
			JArray col_list = null;

			if (param != null)
			{
				col_list = (JArray)param["ColumnList"]; //Lee una property del objeto que es un array.
			}

			if (col_list != null)
			{
				String col_sep = "";
				foreach (String col_name in col_list)
				{
					qry_txt += col_sep;
					qry_txt += col_name;
					qry_txt += "\n";
					col_sep = ",";
				}
			}
			else if (clmnlist_option == ColumnListOption.EXPORT_COLUMNS && this.exportCols != null && this.exportCols.Length > 0)
			{
				//Exporta columnas preseleccionadas. Gerardo 2015-01-07.
				String col_sep = "";
				foreach (String col_name in this.exportCols)
				{
					qry_txt += col_sep;
					qry_txt += col_name;
					qry_txt += "\n";
					col_sep = ",";
				}
			}
			else
			{
				qry_txt += "*\n";
			}


			qry_txt += "FROM\n";
			qry_txt += "(\n";
			qry_txt += "SELECT\n";

			//Columnas tabla principal.
			foreach (CfDbColumn c in this.DbCols)
			{
				if (!uniqueColList.Exists(x => x.Equals(c.ColName, StringComparison.CurrentCultureIgnoreCase)))
				{
					ColumnProperties cpColumn = new ColumnProperties();
					cpColumn.TableAlias = tabMainStr;
					cpColumn.ColName = c.ColName;
					cpColumn.TableName = c.DbTable.TbName;
					cpColumn.ColPk = c.isPk;
					ltColumnSel.Add(cpColumn);
					dbAliasList.Add(c.ColName);
					uniqueColList.Add(c.ColName);
				}
			}

			/* Se invierte el orden de listado de columnas para que la primer ocurrencia de campos
				* sea el de las extra data tables. Idea Gerardo Natale. Ejecucion Nicolas Oliveto 2015-03-27
				*/
			//Columnas tablas secundarias.
			if (this.lkpTabs != null)
			{
				foreach (CfDbTable t in this.lkpTabs)
				{
					tabCnt++;
					tabStr = "t" + tabCnt.ToString("D2");
					foreach (CfDbColumn c in t.DbCols)
					{
						if (c.isPk)   // Si es pk,
							continue;   // ya deberia estar en la tabla principal.

						if (!uniqueColList.Exists(x => x.Equals(c.ColName, StringComparison.CurrentCultureIgnoreCase)))
						{
							ColumnProperties cpColumn = new ColumnProperties();
							cpColumn.TableAlias = tabStr;
							cpColumn.ColName = c.ColName;
							cpColumn.TableName = c.DbTable.TbName;
							cpColumn.ColPk = c.isPk;
							ltColumnSel.Add(cpColumn);

							if (!ltAllTables.Exists(ta => ta.TableAlias.Equals(tabStr, StringComparison.CurrentCultureIgnoreCase)))
							{
								TableProperties tpTable = new TableProperties();
								tpTable.TableName = t.TbName;
								tpTable.TableDbName = t.DbName;
								tpTable.TableAlias = tabStr;
								tpTable.PkColumns = t.PkColumns;
								tpTable.JointType = "INNER JOIN";
								tpTable.JoinTableAlias = tabMainStr;
								ltAllTables.Add(tpTable);
							}

							dbAliasList.Add(c.ColName);
							uniqueColList.Add(c.ColName);
						}
					}
				}
			}

			// Columnas de tablas padre. Se agregan columna descripcion.
			if (this.relParentTabs != null)
			{
				foreach (CfDbRelation r in this.relParentTabs)
				{
					tabCnt++;
					tabStr = "t" + tabCnt.ToString("D2");

					String joinType;
					String parentTabStr = tabStr; // This alias corresponds to the parent in process. tabCnt may be increased if there are any grandparent table.
					String schema_name = Parametros().strDbSchema;

					if (this.ParentJointType.Equals("Inner", StringComparison.CurrentCultureIgnoreCase))
					{
						joinType = "INNER JOIN";
					}
					else if (this.ParentJointType.Equals("Outer", StringComparison.CurrentCultureIgnoreCase))
					{
						joinType = "LEFT OUTER JOIN";
					}
					else
					{
						if ((!this.DbName.Equals(schema_name, StringComparison.CurrentCultureIgnoreCase)) || (!r.ParentTable.DbName.Equals(schema_name, StringComparison.CurrentCultureIgnoreCase)))
						{
							/*
								* Si las dos tablas a joinear no son de CDW_R01,
								* las joineo con outer join, porque el DW no garantiza
								* integridad.
								* Podria hacerlo con todas, pero quiero forzar la
								* integridad.
							*/
							joinType = "LEFT OUTER JOIN";
						}
						else
						{
							joinType = "INNER JOIN";
						}
					}

					foreach (CfDbColumn c in r.ParentTable.DbCols)
					{
						if (c.isDesc) // Se agrega si la columna es descripcion.
						{
							//Las siguientes se usan para achicar el alias.
							String oldStrDescr = "Description";
							String newStrDescr = "Desc";

							// Por si la tabla tiene prefijo.
							String parentColName = r.RelAtts[0].ParentColumn.ColName;
							String ChildColName = r.RelAtts[0].ChildColumn.ColName;

							String aliasPrefix = "";
							String ColAlias = "";
							String DescColAliasClause;

							if (parentColName.Equals(ChildColName, StringComparison.CurrentCultureIgnoreCase)) //Si la columna original migro sin cambio de nombre.
							{
								DescColAliasClause = "";
								ColAlias = c.ColName;
							}
							else
							{
								String cName = c.ColName; // Nombre original.

								// Cambio _Description por _Desc, para controlar largo.
								int pos = cName.LastIndexOf(oldStrDescr, StringComparison.CurrentCultureIgnoreCase);
								if (pos >= 0)
								{
									cName = cName.Remove(pos, oldStrDescr.Length).Insert(pos, newStrDescr);
								}

								//Le pongo a la descripcion el mismo prefijo que al id.
								pos = ChildColName.LastIndexOf(parentColName, StringComparison.CurrentCultureIgnoreCase);
								aliasPrefix = ChildColName.Substring(0, pos);
								ColAlias = aliasPrefix + cName;

								DescColAliasClause = " AS ";
								DescColAliasClause += ColAlias;
							}

							if (!uniqueColList.Exists(x => x.Equals(ColAlias, StringComparison.CurrentCultureIgnoreCase)))
							{
								ColumnProperties cpColumn = new ColumnProperties();
								cpColumn.TableAlias = parentTabStr;
								cpColumn.ColName = c.ColName;
								cpColumn.TableName = c.DbTable.TbName;
								cpColumn.ColAlias = ColAlias;
								cpColumn.ColPk = c.isPk;
								ltColumnSel.Add(cpColumn);

								if (!ltAllTables.Exists(ta => ta.TableAlias.Equals(parentTabStr, StringComparison.CurrentCultureIgnoreCase)))
								{
									TableProperties tpTable = new TableProperties();
									tpTable.TableName = r.ParentTable.TbName;
									tpTable.TableDbName = r.ParentTable.DbName;
									tpTable.TableAlias = parentTabStr;
									tpTable.RelAtts = r.RelAtts;
									tpTable.JointType = joinType;
									tpTable.JoinTableAlias = tabMainStr;
									ltAllTables.Add(tpTable);
								}

								dbAliasList.Add(ColAlias);
								uniqueColList.Add(ColAlias);
							}

							// Si ademas de descripcion, es foreing key, traigo la descripcion del padre.
							if (c.isFk)
							{
								CfDbRelation grandParentRelation = null;
								CfDbTable grandParentTable = null;
								String childColumnName = null;

								//Busco cual es la tabla que origina esta fk de la tabla padre.
								foreach (CfDbRelation r2 in r.ParentTable.relParentTabs)
								{
									//Busco el atributo.
									foreach (CfDbRelAttribute ra2 in r2.RelAtts)
									{
										// Comparo con ChildColumn, porque es el nombre que migro a la tabla
										if (c.ColName.Equals(ra2.ChildColumn.ColName, StringComparison.CurrentCultureIgnoreCase))
										{
											//Encontre la columna, entre los padres de la tabla padre.
											grandParentRelation = r2;
											grandParentTable = ra2.ParentColumn.DbTable;
											childColumnName = ra2.ChildColumn.ColName;
										}
									}
								}
								if (grandParentTable != null)
								{
									String grandparentColName = grandParentRelation.RelAtts[0].ParentColumn.ColName;
									String prev_name = "";

									String joinType2;
									if (this.ParentJointType.Equals("Inner", StringComparison.CurrentCultureIgnoreCase))
									{
										joinType2 = "INNER JOIN";
									}
									else if (this.ParentJointType.Equals("Outer", StringComparison.CurrentCultureIgnoreCase))
									{
										joinType2 = "LEFT OUTER JOIN";
									}
									else
									{
										if ((!r.ParentTable.DbName.Equals(schema_name, StringComparison.CurrentCultureIgnoreCase)) || (!grandParentTable.DbName.Equals(schema_name, StringComparison.CurrentCultureIgnoreCase)))
										{
											/*
												* Si las dos tablas a joinear no son de CDW_R01,
												* las joineo con outer join, porque el DW no garantiza
												* integridad.
												* Podria hacerlo con todas, pero quiero forzar la
												* integridad.
											*/
											joinType2 = "LEFT OUTER JOIN";
										}
										else
										{
											joinType2 = "INNER JOIN";
										}
									}

									//Entre las columnas de esa tabla padre.
									foreach (CfDbColumn c2 in grandParentTable.DbCols)
									{
										String name = grandParentTable.TbName;

										// Que sea descripcion y que no sean fk, asi corto la cadena.
										if (c2.isDesc && !c2.isFk)
										{
											//La inserto en la lista de columnas de salida.
											if (!prev_name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
											{
												tabCnt++; //Incremento nro de tabla para el alias
												tabStr = "t" + tabCnt.ToString("D2"); // creo el alias de la tabla
											}
											prev_name = name;

											String DescColAliasClause2 = "";
											String ColAlias2 = "";

											if (parentColName.ToLower().Equals(ChildColName.ToLower()) && grandparentColName.Equals(parentColName, StringComparison.CurrentCultureIgnoreCase)) //Si la columna original migro sin cambio de nombre.
											{
												DescColAliasClause2 = "";
												ColAlias2 = c2.ColName;
											}
											else
											{
												String cName2 = c2.ColName; // Nombre original.

												// Cambio _Description por _Desc, para controlar largo.
												int pos = cName2.ToLower().LastIndexOf(oldStrDescr.ToLower());
												if (pos >= 0)
												{
													cName2 = cName2.Remove(pos, oldStrDescr.Length).Insert(pos, newStrDescr);
												}

												//Le pongo a la descripcion el mismo prefijo que al id.
												pos = ChildColName.LastIndexOf(grandparentColName, StringComparison.CurrentCultureIgnoreCase);
												if (pos == -1 && aliasPrefix.Equals("", StringComparison.CurrentCultureIgnoreCase))
												{
													pos = childColumnName.LastIndexOf(grandparentColName, StringComparison.CurrentCultureIgnoreCase);

													if (pos >= 0)
													{
														aliasPrefix = childColumnName.Substring(0, pos);
													}
												}
												else if (pos >= 0 && aliasPrefix.Equals("", StringComparison.CurrentCultureIgnoreCase))
												{
													aliasPrefix = ChildColName.Substring(0, pos);
												}

												ColAlias2 = aliasPrefix + cName2;

												DescColAliasClause2 = " AS ";
												DescColAliasClause2 += ColAlias2;
											}

											if (!uniqueColList.Exists(x => x.Equals(ColAlias2, StringComparison.CurrentCultureIgnoreCase)))
											{
												ColumnProperties cpColumn = new ColumnProperties();
												cpColumn.TableAlias = tabStr;
												cpColumn.ColName = c2.ColName;
												cpColumn.TableName = c2.DbTable.TbName;
												cpColumn.ColAlias = ColAlias2;
												cpColumn.ColPk = c2.isPk;
												ltColumnSel.Add(cpColumn);

												if (!ltAllTables.Exists(ta => ta.TableAlias.Equals(tabStr, StringComparison.CurrentCultureIgnoreCase)))
												{
													TableProperties tpTable = new TableProperties();
													tpTable.TableName = grandParentTable.TbName;
													tpTable.TableDbName = grandParentTable.DbName;
													tpTable.TableAlias = tabStr;
													tpTable.RelAtts = grandParentRelation.RelAtts;
													tpTable.JointType = joinType2;
													tpTable.JoinTableAlias = parentTabStr;
													ltAllTables.Add(tpTable);
												}

												dbAliasList.Add(ColAlias2);
												uniqueColList.Add(ColAlias2);
											}
										}
									}
								}
							}
						}
					}
				}
			}

			//Chequeo si informo lista de columnas.
			JArray sel_list = null;

			if (param != null)
			{
				sel_list = (JArray)param["SelectList"]; //Lee una property del objeto que es un array.
			}

			sep = "";
			if (sel_list != null)
			{
				List<String> ltColumnNames = sel_list.Select(jv => (String)jv).ToList();

				foreach (ColumnProperties column in ltColumnSel)
				{
					if (column.ColPk)
					{
						qry_txt += sep;
						qry_txt += column.TableAlias;
						qry_txt += ".";
						qry_txt += column.ColName;
						if (column.ColAlias != null)
						{
							qry_txt += column.ColAlias;
						}
						qry_txt += "\n";
						sep = ",";
					}
					else if (column.ColAlias != null && column.ColAlias != "")
					{
						if (ltColumnNames.Exists(x => x.Equals(column.ColAlias, StringComparison.CurrentCultureIgnoreCase)))
						{
							qry_txt += sep;
							qry_txt += column.TableAlias;
							qry_txt += ".";
							qry_txt += column.ColName;
							qry_txt += " AS ";
							qry_txt += column.ColAlias;
							qry_txt += "\n";
							sep = ",";

							if (!ltTableSel.Exists(ta => ta.TableAlias.Equals(column.TableAlias, StringComparison.CurrentCultureIgnoreCase)))
							{
								TableProperties tp = ltAllTables.Find(j => j.TableAlias.Equals(column.TableAlias, StringComparison.CurrentCultureIgnoreCase));
								if (tp != null)
								{
									ltTableSel.Add(tp);
								}
							}
						}
					}
					else if (ltColumnNames.Exists(x => x.Equals(column.ColName, StringComparison.CurrentCultureIgnoreCase)))
					{
						qry_txt += sep;
						qry_txt += column.TableAlias;
						qry_txt += ".";
						qry_txt += column.ColName;
						qry_txt += "\n";
						sep = ",";

						if (!ltTableSel.Exists(ta => ta.TableAlias.Equals(column.TableAlias, StringComparison.CurrentCultureIgnoreCase)))
						{
							TableProperties tp = ltAllTables.Find(j => j.TableAlias.Equals(column.TableAlias, StringComparison.CurrentCultureIgnoreCase));
							if (tp != null)
							{
								ltTableSel.Add(tp);
							}
						}
					}
				}
			}
			else
			{
				foreach (ColumnProperties column in ltColumnSel)
				{
					qry_txt += sep;
					qry_txt += column.TableAlias;
					qry_txt += ".";
					qry_txt += column.ColName;
					if (column.ColAlias != null)
					{
						qry_txt += " AS ";
						qry_txt += column.ColAlias;
					}
					qry_txt += "\n";
					sep = ",";
				}

				ltTableSel = ltAllTables;
			}

			// Clausula from
			qry_txt += "FROM\n";

			// Tabla principal. No se evalua ya que siempre debe ir la tabla principal
			if (!this.isHist || list_option == ListOption.LIST_ALL)
			{
				// Si no tiene historia o elige listar todo, es el caso simple.
				qry_txt += this.DbName;
				qry_txt += ".";
				qry_txt += this.TbName;
				qry_txt += " AS ";
				qry_txt += tabMainStr;
				qry_txt += "\n";
			}
			else
			{
				//Es con historia, ademas tengo que ver que opcion.
				//En una subquery filtro los registros que me interesan.
				qry_txt += "(SELECT * FROM ";
				qry_txt += this.DbName;
				qry_txt += ".";
				qry_txt += this.TbName;
				qry_txt += "\n";
				qry_txt += "WHERE\n";

				if (list_option == ListOption.LIST_CURRENT)
				{
					qry_txt += "CURRENT_DATE BETWEEN ";
					qry_txt += this.StartDt_Column.ColName;
					qry_txt += " AND ";
					qry_txt += this.EndDt_Column.ColName;
					qry_txt += "\n";
				}
				else if (list_option == ListOption.LIST_CURRENT_AND_FUTURE)
				{
					qry_txt += "CURRENT_DATE <= ";
					qry_txt += this.EndDt_Column.ColName;
					qry_txt += "\n";
				}
				else
				{
					throw new Exception("Opcion de listado invalido (" + list_option.ToString() + ").");
				}
				qry_txt += ") ";
				qry_txt += tabMainStr;
				qry_txt += "\n";
			}


			//Tablas Definidas.
			foreach (TableProperties t in ltTableSel)
			{
				if (t.PkColumns != null)
				{
					qry_txt += t.JointType;
					qry_txt += "\n";
					qry_txt += t.TableDbName;
					qry_txt += ".";
					qry_txt += t.TableName;
					qry_txt += " AS ";
					qry_txt += t.TableAlias;
					qry_txt += "\n";
					qry_txt += "ON\n";

					String join_sep = "    ";
					foreach (CfDbColumn c in t.PkColumns) // Join por pk tabla secundaria.
					{
						qry_txt += join_sep;
						qry_txt += t.TableAlias;
						qry_txt += ".";
						qry_txt += c.ColName;

						qry_txt += "=";

						qry_txt += t.JoinTableAlias;
						qry_txt += ".";
						qry_txt += c.ColName;

						qry_txt += "\n";
						join_sep = "AND ";
					}
				}
				else if (t.RelAtts != null)
				{
					qry_txt += t.JointType;
					qry_txt += "\n";
					qry_txt += t.TableDbName;
					qry_txt += ".";
					qry_txt += t.TableName;
					qry_txt += " AS ";
					qry_txt += t.TableAlias;
					qry_txt += "\n";
					qry_txt += "ON\n";

					String join_sep = "    ";
					foreach (CfDbRelAttribute r in t.RelAtts) // Join por pk tabla secundaria.
					{
						qry_txt += join_sep;
						qry_txt += t.TableAlias;
						qry_txt += ".";
						qry_txt += r.ParentColumn.ColName;

						qry_txt += "=";

						qry_txt += t.JoinTableAlias;
						qry_txt += ".";
						qry_txt += r.ChildColumn.ColName;

						qry_txt += "\n";
						join_sep = "AND ";
					}
				}
			}

			qry_txt += ")t000\n";

			String where_op = "WHERE\n"; // Iniciador clausla where, luego del primer grupo de condiciones, vacio.
			String condition_op = ""; // Conector logico entre condiciones, inicialmente vacio.
			//"{'Where':[{'Section_id':['01   1   4','01   1   1']}]}",

			JArray cond_list = null;

			if (param != null)
			{
				cond_list = (JArray)param["Where"]; //Lee una property del objeto que es un array.
			}

			if (cond_list != null)
			{
				Int16 aux_table_num = 0;
				String aux_table_str = "";
				String where_clause = "";

				foreach (JObject cond in cond_list)
				{
					foreach (JProperty condFld in cond.Properties())
					{
						String fldName = condFld.Name;

						//Armo lista de valores posibles.
						JArray val_list = (JArray)cond[fldName]; //Lee una property del objeto que es un array.
						String cond_val_list = "";
						String cond_val_list_sep = "";

						foreach (String val in val_list)
						{
							cond_val_list += cond_val_list_sep;
							cond_val_list += "'";
							cond_val_list += val;
							cond_val_list += "'";
							cond_val_list_sep = ", ";
						}
						if (cond_val_list.Length != 0) // Si no esta vacia hay valores, y aplica la condicion.
						{
							CfDbColumn fcol;
							CfDbRelAttribute ratt;
							///////////////////////////////////////////////////////////////////////////////////
							// Aca hay que confirmar si la columna de filtro es parte de las columnas de salida
							// o si hay que filtrar los registros que forman parte de una tabla hija.
							///////////////////////////////////////////////////////////////////////////////////
							// Si alguna es alguna de las columnas disponibles.
							if( dbAliasList.Exists( x => x.Equals(fldName,StringComparison.CurrentCultureIgnoreCase ) ) )
							{
								where_clause += condition_op;
								where_clause += fldName;
								where_clause += " IN (";
								where_clause += cond_val_list;
								where_clause += ")\n";
								condition_op = "AND ";
							}
							else if ( ( ratt = this.searchRelAttInChildTables(fldName) ) != null )
							{
								// Filtro por id en tabla hija. Busco por una FK de la tabla
								// hija que no sea la PK de la tabla que invoca el metodo.

								// Para filtrar por un atributo que esta en una tabla hija
								// hago una subquery correlacionada.
								// Deberia seleccionarse si existe al menos un registro en la
								// tabla hija, para el registro de la tabla padre.

								aux_table_num++;
								aux_table_str = "a" + aux_table_num.ToString("D2");

								where_clause += condition_op;
								where_clause += " EXISTS (\n";
								where_clause += "SELECT 1\n";
								where_clause += "FROM\n";
								where_clause += ratt.Relation.ChildTable.DbName;
								where_clause += ".";
								where_clause += ratt.Relation.ChildTable.TbName;
								where_clause += " ";
								where_clause += aux_table_str;
								where_clause += "\n";
								where_clause += "WHERE\n";
								// Que el campo filtrado este en la lista
								where_clause += aux_table_str;
								where_clause += ".";
								where_clause += ratt.ChildColumn.ColName;
								where_clause += " IN (";
								where_clause += cond_val_list;
								where_clause += ")\n";

								//A continuacion join entre tabla hija y tabla padre
								// la tabla padre esta en la query exterior.
								
								//Busco relacion con la tabla this.

								foreach (CfDbRelation r2 in ratt.Relation.ChildTable.relParentTabs)
								{
									if (r2.ParentTable == this && r2.ChildTable == ratt.Relation.ChildTable)
									{
										//Encontre una relacion con tabla que tiene la columna de filtro.
										foreach (CfDbRelAttribute ratt2 in r2.RelAtts)
										{
											where_clause += "AND ";
											where_clause += aux_table_str;
											where_clause += ".";
											where_clause += ratt2.ChildColumn.ColName;
											where_clause += " = t000."; // La columna padre ya esta en el select principal
											where_clause += ratt2.ParentColumn.ColName;
											where_clause += "\n";
										}
									}
								}
								// Ademas, si la tabla hija lleva historia, debo tomar los vigentes.
								if (ratt.Relation.ChildTable.isHist)
								{
									where_clause += "AND ";
									where_clause += "CURRENT_DATE BETWEEN ";
									where_clause += aux_table_str;
									where_clause += ratt.Relation.ChildTable.StartDt_Column.ColName;
									where_clause += " AND ";
									where_clause += ratt.Relation.ChildTable.EndDt_Column.ColName;
									where_clause += "\n";
								}

								where_clause += ")\n";
								condition_op = "AND ";
							}
							else if ((fcol = this.searchColumnInFilterTables(fldName)) != null)
							{
								// Filtro por id en tabla filtro. Busco por una FK de la tabla
								// hija que no sea la PK de la tabla que invoca el metodo.

								// Para filtrar por un atributo que esta en una tabla hija
								// o en una tabla filtro hago una subquery correlacionada.
								// Deberia seleccionarse si existe al menos un registro en la
								// tabla hija o filtro, para el registro de la tabla padre.

								aux_table_num++;
								aux_table_str = "a" + aux_table_num.ToString("D2");

								where_clause += condition_op;
								where_clause += " EXISTS (\n";
								where_clause += "SELECT 1\n";
								where_clause += "FROM\n";
								where_clause += fcol.DbTable.DbName;
								where_clause += ".";
								where_clause += fcol.DbTable.TbName;
								where_clause += " ";
								where_clause += aux_table_str;
								where_clause += "\n";
								where_clause += "WHERE\n";
								// Que el campo filtrado este en la lista
								where_clause += aux_table_str;
								where_clause += ".";
								where_clause += fcol.ColName;
								where_clause += " IN (";
								where_clause += cond_val_list;
								where_clause += ")\n";

								// A continuacion join entre tabla filtro y la tabla principal
								// La tabla filtro tendra las columnas que son pk en la tabla
								// principal.

								foreach (CfDbColumn c in this.PkColumns)
								{
									CfDbColumn aux_table_col;
									if ((aux_table_col = this.searchColumnInFilterTables(c.ColName)) != null)
									{
										where_clause += "AND ";
										where_clause += aux_table_str;
										where_clause += ".";
										where_clause += c.ColName;
										where_clause += " = t000."; // La columna padre ya esta en el select principal
										where_clause += c.ColName;
										where_clause += "\n";
									}
								}

								// Ademas, si la tabla filtro lleva historia, debo tomar los vigentes.
								if (fcol.DbTable.isHist)
								{
									where_clause += "AND ";
									where_clause += "CURRENT_DATE BETWEEN ";
									where_clause += aux_table_str;
									where_clause += ".";
									where_clause += fcol.DbTable.StartDt_Column.ColName;
									where_clause += " AND ";
									where_clause += aux_table_str;
									where_clause += ".";
									where_clause += fcol.DbTable.EndDt_Column.ColName;
									where_clause += "\n";
								}

								where_clause += ")\n";
								condition_op = "AND ";
							}
							/*
							else
							{
								throw new Exception("Columna de filtro invalida (" + fldName + ").");
							}
							 */
						}
					}
				}
				if (where_clause.Length != 0) // Si no esta vacia hay filtros, y se agrega clausula "WHERE"
				{
					qry_txt += where_op;
					qry_txt += where_clause;
					where_op = "";
					condition_op = "AND ";
				}
			} 


			//"{'WhereFilter':[{ LogicOp:"OR","FieldName":"Pack_Qty","Condition":"<", "Value":"5"}],}",
			//"{'WhereFilter':[{ LogicOp:"OR","FieldName":"TrackingGroup_id","Condition":"IN", "ValueArray":["1","2","3"]}],}",
			//"{'WhereFilter':[{ LogicOp:"OR","Parenthesis":"(" } ],}",
			//"{'WhereFilter':[{ "Parenthesis":")" } ],}",

			/*
			 * Condition Operators
			 *  =,<>,<,>,<=,>=,
			 *  like, not like
			 * 
			 * Logical Operators
			 * OR,AND
			*/
			JArray cond_filt_list = null;

			if (param != null)
			{
				cond_filt_list = (JArray)param["WhereFilter"]; //Lee una property del objeto que es un array.
			}

			int parenthesesLevel = 0; // Para validar que los parentesis esten equilibrados. Gerardo 2014-01-08

			if (cond_filt_list != null)
			{
				Stack<String> prevLogOperatorStk = new Stack<String>(); // Esta pila funciona como memoria del operador vigente por default cuando hay parentesis. Gerardo 2014-01-09.
				String where_filt_clause = "";
				String filter_condition_op = "";
				String filter_default_op_clause_op = "";
				String[] validOperators = { "=", "<>", "<", ">", "<=", ">=", "LIKE", "NOT LIKE", "IN", "NOT IN" };
				String[] validLogicOperators = { "AND", "OR" };
				String[] validParentheses = { "(", ")" };
				Int16 aux_table_num = 0;
				String aux_table_str = "";

				foreach (JObject cond_filt in cond_filt_list)
				{
					String logicOp = ( cond_filt["LogicOp"] != null ? cond_filt["LogicOp"].ToString().ToUpper(): null);
					String fldName = (cond_filt["FieldName"] != null ? cond_filt["FieldName"].ToString() : null);
					String condOp = (cond_filt["Condition"] != null ? cond_filt["Condition"].ToString().ToUpper() : null);
					String value = (cond_filt["Value"] != null ? cond_filt["Value"].ToString().ToUpper() : null);
					JArray valueArray = (JArray) (cond_filt["ValueArray"] != null ? cond_filt["ValueArray"] : null);
					String parenthesis = (cond_filt["Parenthesis"] != null ? cond_filt["Parenthesis"].ToString().ToUpper() : null);

					if (fldName != null && parenthesis != null) // No se aceptaran parentisis y condiciones de columna en el mismo elemento. Gerardo 2014-01-08
					{
						throw new Exception("Formato de condicion de columna y parentesis invalido.");
					}

					//Proceso operador logico (AND/OR)
					if (logicOp == null)    // Si no vino definido
					{
						logicOp = filter_default_op_clause_op; // Es el operador por defecto (puede ser vacio).
					}
					else if (!validLogicOperators.Contains(logicOp))
					{
						throw new Exception("Operador logico invalido (" + logicOp + ").");
					}
					else if (logicOp != null && filter_condition_op.Equals("") && parenthesis == null)
					{   // Si informo operador, y es el primero, es un error
						// Se ignora.                         
					}
					else if ( logicOp != null && parenthesis != null && parenthesis.Equals(")"))  // Gerardo 2014-01-08.
					{
						//El parentesis que cierra no toma el operador logico.
						// Se ignora.
					}
					else if (logicOp != null && parenthesis != null && parenthesis.Equals("("))  // Gerardo 2014-01-08.
					{
						//Es la nueva condicion para informar operador con parentesis.
						where_filt_clause += logicOp + " "; // Se agrega un blanco como separador
					}
					else
					{   // Llego el operador y es valido.
						filter_condition_op = logicOp + " "; // Se agrega un blanco como separador
					}

					// Proceso parentesis.
					if (parenthesis != null && parenthesis.Equals("("))
					{
						where_filt_clause += "\n(\n";
						parenthesesLevel++; // En la apertura suma.
						prevLogOperatorStk.Push(filter_condition_op);
						filter_condition_op = "";
						continue; //No es necesario continuar con el elemento actual. Salta al siguiente.
					}

					if (parenthesis != null && parenthesis.Equals(")"))
					{
						where_filt_clause += "\n)\n";
						parenthesesLevel--; // En el cierre resta.
						filter_condition_op = prevLogOperatorStk.Pop();
						continue; //No es necesario continuar con el elemento actual. Salta al siguiente.
					}

					//Proceso operador de condicion (=,<>,...)
					if( ! validOperators.Contains(condOp) )
					{
						throw new Exception("Operador de comparacion invalido (" + condOp + ").");
					}

					///////////////////////////////////////////////////////////////////////////////////
					// Aca hay que confirmar si la columna de filtro es parte de las columnas de salida.
					///////////////////////////////////////////////////////////////////////////////////
					// Si alguna es alguna de las columnas disponibles.
					CfDbRelAttribute ratt;
					CfDbColumn fcol;

					if (dbAliasList.Exists(x => x.Equals(fldName, StringComparison.CurrentCultureIgnoreCase)))
					{
						where_filt_clause += filter_condition_op;

						if (condOp.Equals("LIKE") || condOp.Equals("NOT LIKE"))
						{
							/*
							 * Esto seria mejor si en la DfCBColumn hubiera
							 * una campo que indique el dominio. Gerardo.
							 */

							where_filt_clause += "TRIM( TRAILING '.' FROM CAST( ";
							where_filt_clause += fldName;
							where_filt_clause += " AS VARCHAR(255)))";
						}
						else
						{
							where_filt_clause += fldName;
						}

						where_filt_clause += " ";
						where_filt_clause += condOp;
						where_filt_clause += " ";

						if (condOp.Equals("IN", StringComparison.CurrentCultureIgnoreCase) ||
							condOp.Equals("NOT IN", StringComparison.CurrentCultureIgnoreCase)
							)
						{
							where_filt_clause += "(";
							//Lista de valores.
							String lsep = "";
							foreach (JValue j in valueArray.Children())
							{
								String val = j.ToString();

								where_filt_clause += lsep;
								where_filt_clause += "'";
								where_filt_clause += val;
								where_filt_clause += "'";
								lsep = ",";
							}
							where_filt_clause += ")";
						}
						else
						{
							//Valor unico.
							where_filt_clause += "'";
							where_filt_clause += value;
							where_filt_clause += "'";
						}
						where_filt_clause += "\n";
						filter_condition_op = "AND ";
					}
					else if ((ratt = this.searchRelAttInChildTables(fldName)) != null)
					{
						// Filtro por id en tabla hija. Busco por una FK de la tabla
						// hija que no sea la PK de la tabla que invoca el metodo.

						// Para filtrar por un atributo que esta en una tabla hija
						// hago una subquery correlacionada.
						// Deberia seleccionarse si existe al menos un registro en la
						// tabla hija, para el registro de la tabla padre.

						aux_table_num++;
						aux_table_str = "a" + aux_table_num.ToString("D2");

						where_filt_clause += filter_condition_op;
						where_filt_clause += " EXISTS (\n";
						where_filt_clause += "SELECT 1\n";
						where_filt_clause += "FROM\n";
						where_filt_clause += ratt.Relation.ChildTable.DbName;
						where_filt_clause += ".";
						where_filt_clause += ratt.Relation.ChildTable.TbName;
						where_filt_clause += " ";
						where_filt_clause += aux_table_str;
						where_filt_clause += "\n";
						where_filt_clause += "WHERE\n";

						// Que el campo filtrado este en la lista
						if (condOp.Equals("LIKE") || condOp.Equals("NOT LIKE"))
						{
							/*
							 * Esto seria mejor si en la DfCBColumn hubiera
							 * una campo que indique el dominio. Gerardo.
							 */

							where_filt_clause += "TRIM( TRAILING '.' FROM CAST( ";
							where_filt_clause += aux_table_str;
							where_filt_clause += ".";
							where_filt_clause += ratt.ChildColumn.ColName;
							where_filt_clause += " AS VARCHAR(255)))";
						}
						else
						{
							where_filt_clause += aux_table_str;
							where_filt_clause += ".";
							where_filt_clause += ratt.ChildColumn.ColName;
						}

						where_filt_clause += " ";
						where_filt_clause += condOp;
						where_filt_clause += " ";

						if (condOp.Equals("IN", StringComparison.CurrentCultureIgnoreCase) ||
							condOp.Equals("NOT IN", StringComparison.CurrentCultureIgnoreCase)
							)
						{
							where_filt_clause += "(";
							//Lista de valores.
							String lsep = "";
							foreach (JValue j in valueArray.Children())
							{
								String val = j.ToString();

								where_filt_clause += lsep;
								where_filt_clause += "'";
								where_filt_clause += val;
								where_filt_clause += "'";
								lsep = ",";
							}
							where_filt_clause += ")\n";
						}
						else
						{
							//Valor unico.
							where_filt_clause += "'";
							where_filt_clause += value;
							where_filt_clause += "'\n";
						}

						//A continuacion join entre tabla hija y tabla padre
						// la tabla padre esta en la query exterior.

						//Busco relacion con la tabla this.

						foreach (CfDbRelation r2 in ratt.Relation.ChildTable.relParentTabs)
						{
							if (r2.ParentTable == this && r2.ChildTable == ratt.Relation.ChildTable)
							{
								//Encontre una relacion con tabla que tiene la columna de filtro.
								foreach (CfDbRelAttribute ratt2 in r2.RelAtts)
								{
									where_filt_clause += "AND ";
									where_filt_clause += aux_table_str;
									where_filt_clause += ".";
									where_filt_clause += ratt2.ChildColumn.ColName;
									where_filt_clause += " = t000."; // La columna padre ya esta en el select principal
									where_filt_clause += ratt2.ParentColumn.ColName;
									where_filt_clause += "\n";
								}
							}
						}
						// Ademas, si la tabla hija lleva historia, debo tomar los vigentes.
						if (ratt.Relation.ChildTable.isHist)
						{
							where_filt_clause += "AND ";
							where_filt_clause += "CURRENT_DATE BETWEEN ";
							where_filt_clause += aux_table_str;
							where_filt_clause += ratt.Relation.ChildTable.StartDt_Column.ColName;
							where_filt_clause += " AND ";
							where_filt_clause += ratt.Relation.ChildTable.EndDt_Column.ColName;
							where_filt_clause += "\n";
						}

						where_filt_clause += ")\n";
						filter_condition_op = "AND ";
					}
					else if ((fcol = this.searchColumnInFilterTables(fldName)) != null)
					{
						// Filtro por id en tabla filtro. Busco por una FK de la tabla
						// hija que no sea la PK de la tabla que invoca el metodo.

						// Para filtrar por un atributo que esta en una tabla hija
						// o en una tabla filtro hago una subquery correlacionada.
						// Deberia seleccionarse si existe al menos un registro en la
						// tabla hija o filtro, para el registro de la tabla padre.

						aux_table_num++;
						aux_table_str = "a" + aux_table_num.ToString("D2");

						where_filt_clause += filter_condition_op;
						where_filt_clause += " EXISTS (\n";
						where_filt_clause += "SELECT 1\n";
						where_filt_clause += "FROM\n";
						where_filt_clause += fcol.DbTable.DbName;
						where_filt_clause += ".";
						where_filt_clause += fcol.DbTable.TbName;
						where_filt_clause += " ";
						where_filt_clause += aux_table_str;
						where_filt_clause += "\n";
						where_filt_clause += "WHERE\n";

						// Que el campo filtrado este en la lista
						if (condOp.Equals("LIKE") || condOp.Equals("NOT LIKE"))
						{
							/*
							 * Esto seria mejor si en la DfCBColumn hubiera
							 * una campo que indique el dominio. Gerardo.
							 */

							where_filt_clause += "TRIM( TRAILING '.' FROM CAST( ";
							where_filt_clause += aux_table_str;
							where_filt_clause += ".";
							where_filt_clause += fcol.ColName;
							where_filt_clause += " AS VARCHAR(255)))";
						}
						else
						{
							where_filt_clause += aux_table_str;
							where_filt_clause += ".";
							where_filt_clause += fcol.ColName;
						}

						where_filt_clause += " ";
						where_filt_clause += condOp;
						where_filt_clause += " ";

						if (condOp.Equals("IN", StringComparison.CurrentCultureIgnoreCase) ||
							condOp.Equals("NOT IN", StringComparison.CurrentCultureIgnoreCase)
							)
						{
							where_filt_clause += "(";
							//Lista de valores.
							String lsep = "";
							foreach (JValue j in valueArray.Children())
							{
								String val = j.ToString();

								where_filt_clause += lsep;
								where_filt_clause += "'";
								where_filt_clause += val;
								where_filt_clause += "'";
								lsep = ",";
							}
							where_filt_clause += ")\n";
						}
						else
						{
							//Valor unico.
							where_filt_clause += "'";
							where_filt_clause += value;
							where_filt_clause += "'\n";
						}

						// A continuacion join entre tabla filtro y la tabla principal
						// La tabla filtro tendra las columnas que son pk en la tabla
						// principal.
						foreach (CfDbColumn c in this.PkColumns)
						{
							CfDbColumn aux_table_col;
							if ((aux_table_col = this.searchColumnInFilterTables(c.ColName)) != null)
							{
								where_filt_clause += "AND ";
								where_filt_clause += aux_table_str;
								where_filt_clause += ".";
								where_filt_clause += c.ColName;
								where_filt_clause += " = t000."; // La columna padre ya esta en el select principal
								where_filt_clause += c.ColName;
								where_filt_clause += "\n";
							}
						}

						// Ademas, si la tabla filtro lleva historia, debo tomar los vigentes.
						if (fcol.DbTable.isHist)
						{
							where_filt_clause += "AND ";
							where_filt_clause += "CURRENT_DATE BETWEEN ";
							where_filt_clause += aux_table_str;
							where_filt_clause += ".";
							where_filt_clause += fcol.DbTable.StartDt_Column.ColName;
							where_filt_clause += " AND ";
							where_filt_clause += aux_table_str;
							where_filt_clause += ".";
							where_filt_clause += fcol.DbTable.EndDt_Column.ColName;
							where_filt_clause += "\n";
						}

						where_filt_clause += ")\n";
						filter_condition_op = "AND ";
					}
					else
					{
						where_filt_clause += filter_condition_op;
						where_filt_clause += "'" + fldName + "' = '" + fldName + "'\n";
						filter_condition_op = "AND ";
					}

					filter_default_op_clause_op = "AND ";
				}

				//Valido parentesis equilibrados. Gerardo 2014-01-08
				if (parenthesesLevel != 0)
				{
					throw new Exception("Parentesis no equilibrados en condicion.");
				}

				if (where_filt_clause.Length != 0) // Si no esta vacia hay filtros, y se agrega clausula 
				{
					//qry_txt += "WHERE\n";
					qry_txt += where_op;
					qry_txt += condition_op;
					qry_txt += " (\n";
					qry_txt += where_filt_clause;
					qry_txt += ")\n";
					where_op = "";
					condition_op = "AND ";
				}
			}

			// Ventana de filas?
			if( param != null && param["StartRowNumber"] != null && param["RowCount"] != null ) 
			{
				String startRowNumber = param["StartRowNumber"].ToString(); //Lee fila inicialar
				String rowCount = param["RowCount"].ToString(); //Lee cantidad de filas.

				//Consulta una ventana de filas.
				
				qry_txt += "QUALIFY\n";
				qry_txt += "ROW_NUMBER() OVER (\n";
				qry_txt += "ORDER BY\n";
			
				String osep = "";

				foreach (String ordc in this.orderCols)
				{
					qry_txt += osep;
					qry_txt += ordc;
					qry_txt += "\n";
					osep = ",";
				}
				qry_txt += (") BETWEEN " + startRowNumber + " AND ( " + startRowNumber + " + " + rowCount + " - 1 )");
			}

			//Orden de salida.

			if (orderby_option == OrderByOption.WITH_ORDER_BY)
			{
				if (this.orderCols != null)
				{
					String osep = "";

					qry_txt += "ORDER BY\n";
					foreach (String ordc in this.orderCols)
					{
						qry_txt += osep;
						qry_txt += ordc;
						qry_txt += "\n";
						osep = ",";
					}
				}
			}
			return qry_txt;
		}

		//Genera order by para export en excel
		public String orderColumns(OrderByOption orderby_option)
		{
			String qry_txt = "";

			if (orderby_option == OrderByOption.WITH_ORDER_BY)
			{
				if (this.orderCols != null)
				{
					String osep = "";

					qry_txt += "ORDER BY\n";
					foreach (String ordc in this.orderCols)
					{
						qry_txt += osep;
						qry_txt += ordc;
						qry_txt += "\n";
						osep = ",";
					}
				}
			}

			return qry_txt;
		}

		//Genera lista de columnas del query para list/listc/listcf.
		//public void listColumns(JObject param, out String[] pTabNames, out String[] pColNames, out String[] pSubNames, out Boolean[] pisPK, out Boolean[] pisFK, out String[] pColLocal)
		public void listColumns(JObject param, out String[] pTabNames, out String[] pSubNames, out Boolean[] pisPK, out Boolean[] pisFK, out String[] pColLocal, out Boolean[] pDesc)
		{
			// Armo string de consulta.
			int tabCnt = 0;                         // Contador para alias de tabla. 0 es la principal.
			String tabStr = "t" + tabCnt.ToString("D2");  // Contador en string para alias de tabla.
			String tabMainStr = tabStr;                // Siempre apunta a la tabla principal.
			List<String> dbAliasList = new List<String>(); //Lista de alias para validar si las columnas de filtro estan disponibles.
			List<String> dbTabNameList = new List<String>();
			//List<String> dbColNameList = new List<String>();
			List<String> dbSubNameList = new List<String>();
			List<Boolean> dbisPKNameList = new List<Boolean>();
			List<Boolean> dbisFKNameList = new List<Boolean>();
			List<String> dbColLocalNameList = new List<String>();
			List<Boolean> dbColDesc = new List<Boolean>();

			//Chequeo si informo lista de columnas.
			JArray col_list = null;

			if (param != null)
			{
				col_list = (JArray)param["ColumnList"]; //Lee una property del objeto que es un array.
			}


			 //Columnas tabla principal.
			foreach (CfDbColumn c in this.DbCols)
			{
				dbAliasList.Add(c.ColName);
				dbTabNameList.Add(c.DbTable.TbName);
				dbSubNameList.Add(c.ColName);
				dbisPKNameList.Add(c.isPk);
				dbisFKNameList.Add(c.isFk);
				dbColLocalNameList.Add(c.ColLocalName);
				dbColDesc.Add(c.isDesc);

			}

			// Columnas de tablas padre. Se agregan columna descripcion.
			if (this.relParentTabs != null)
			{
				foreach (CfDbRelation r in this.relParentTabs)
				{
					tabCnt++;
					tabStr = "t" + tabCnt.ToString("D2");

					String parentTabStr = tabStr; // This alias corresponds to the parent in process. tabCnt may be increased if there are any grandparent table.

					foreach (CfDbColumn c in r.ParentTable.DbCols)
					{
						if (c.isDesc) // Se agrega si la columna es descripcion.
						{
							//Las siguientes se usan para achicar el alias.
							String oldStrDescr = "Description";
							String newStrDescr = "Desc";

							// Por si la tabla tiene prefijo.
							String parentColName = r.RelAtts[0].ParentColumn.ColName;
							String ChildColName = r.RelAtts[0].ChildColumn.ColName;

							String parentColLocalName = r.RelAtts[0].ParentColumn.ColLocalName;
							String childColLocalName = r.RelAtts[0].ChildColumn.ColLocalName;

							String aliasPrefix = "";
							String ColAlias = "";
							String DescColAliasClause;

							String aliasLocalPrefix = "";
							String ColLocalAlias = "";

							if (parentColName.Equals(ChildColName, StringComparison.CurrentCultureIgnoreCase)) //Si la columna original migro sin cambio de nombre.
							{
								DescColAliasClause = "";
								ColAlias = c.ColName;

								ColLocalAlias = c.ColLocalName;
							}
							else
							{
								String cName = c.ColName; // Nombre original.

								// Cambio _Description por _Desc, para controlar largo.
								int pos = cName.LastIndexOf(oldStrDescr, StringComparison.CurrentCultureIgnoreCase);
								if (pos >= 0)
								{
									cName = cName.Remove(pos, oldStrDescr.Length).Insert(pos, newStrDescr);
								}

								//Le pongo a la descripcion el mismo prefijo que al id.
								pos = ChildColName.LastIndexOf(parentColName, StringComparison.CurrentCultureIgnoreCase);
								aliasPrefix = ChildColName.Substring(0, pos);
								ColAlias = aliasPrefix + cName;

								int posLocal = childColLocalName.LastIndexOf(parentColLocalName, StringComparison.CurrentCultureIgnoreCase);
								aliasLocalPrefix = childColLocalName.Substring(0, posLocal);
								ColLocalAlias = aliasLocalPrefix + c.ColLocalName;

								DescColAliasClause = " AS ";
								DescColAliasClause += ColAlias;
							}

							dbAliasList.Add(ColAlias);
							dbTabNameList.Add(c.DbTable.TbName);
							dbSubNameList.Add(ColAlias);
							dbisPKNameList.Add(c.isPk);
							dbisFKNameList.Add(c.isFk);
							dbColLocalNameList.Add(ColLocalAlias);
							dbColDesc.Add(c.isDesc);


							// Si ademas de descripcion, es foreing key, traigo la descripcion del padre.
							if (c.isFk)
							{
								CfDbRelation grandParentRelation = null;
								CfDbTable grandParentTable = null;

								//Busco cual es la tabla que origina esta fk de la tabla padre.
								foreach (CfDbRelation r2 in r.ParentTable.relParentTabs)
								{
									//Busco el atributo.
									foreach (CfDbRelAttribute ra2 in r2.RelAtts)
									{
										// Comparo con ChildColumn, porque es el nombre que migro a la tabla
										if (c.ColName.Equals(ra2.ChildColumn.ColName, StringComparison.CurrentCultureIgnoreCase))
										{
											//Encontre la columna, entre los padres de la tabla padre.
											grandParentRelation = r2;
											grandParentTable = ra2.ParentColumn.DbTable;
										}
									}
								}
								if (grandParentTable != null)
								{
									//Entre las columnas de esa tabla padre.
									foreach (CfDbColumn c2 in grandParentTable.DbCols)
									{
										// Que sea descripcion y que no sean fk, asi corto la cadena.
										if (c2.isDesc && !c2.isFk)
										{
											//La inserto en la lista de columnas de salida.

											tabCnt++; //Incremento nro de tabla para el alias
											tabStr = "t" + tabCnt.ToString("D2"); // creo el alias de la tabla

											String DescColAliasClause2 = "";
											String ColAlias2 = "";

											String ColLocalAlias2 = "";

											if (parentColName.ToLower().Equals(ChildColName.ToLower())) //Si la columna original migro sin cambio de nombre.
											{
												DescColAliasClause2 = "";
												ColAlias2 = c2.ColName;

												ColLocalAlias2 = c2.ColLocalName;
											}
											else
											{
												String cName2 = c2.ColName; // Nombre original.

												// Cambio _Description por _Desc, para controlar largo.
												int pos = cName2.ToLower().LastIndexOf(oldStrDescr.ToLower());
												if (pos >= 0)
												{
													cName2 = cName2.Remove(pos, oldStrDescr.Length).Insert(pos, newStrDescr);
												}

												//Le pongo a la descripcion el mismo prefijo que al id.
												ColAlias2 = aliasPrefix + cName2;

												ColLocalAlias2 = aliasLocalPrefix + c2.ColLocalName;

												DescColAliasClause2 = " AS ";
												DescColAliasClause2 += ColAlias2;
											}

											dbAliasList.Add(ColAlias2);
											dbTabNameList.Add(c2.DbTable.TbName);
											dbSubNameList.Add(ColAlias2);
											dbisPKNameList.Add(c2.isPk);
											dbisFKNameList.Add(c2.isFk);
											dbColLocalNameList.Add(ColLocalAlias2);
											dbColDesc.Add(c2.isDesc);

										}
									}
								}
							}
						}
					}
				}
			}

			//Columnas tablas secundarias.
			if (this.lkpTabs != null)
			{
				foreach (CfDbTable t in this.lkpTabs)
				{
					tabCnt++;
					tabStr = "t" + tabCnt.ToString("D2");
					foreach (CfDbColumn c in t.DbCols)
					{
						if (c.isPk)   // Si es pk,
							continue;   // ya deberia estar en la tabla principal.

						dbAliasList.Add(c.ColName);
						dbTabNameList.Add(c.DbTable.TbName);
						dbSubNameList.Add(c.ColName);
						dbisPKNameList.Add(c.isPk);
						dbisFKNameList.Add(c.isFk);
						dbColLocalNameList.Add(c.ColLocalName);
						dbColDesc.Add(c.isDesc);

					}
				}
			}

			//return qry_txt;

			if (col_list != null)
			{
				List<String> tmpTabNameList = new List<String>();
				//List<String> tmpColNameList = new List<String>();
				List<String> tmpSubNameList = new List<String>();
				List<Boolean> tmpisPKNameList = new List<Boolean>();
				List<Boolean> tmpisFKNameList = new List<Boolean>();
				List<String> tmpColLocalNameList = new List<String>();
				List<Boolean> tmpColDescList = new List<Boolean>();

				foreach (String col_name in col_list)
				{
					List<String>.Enumerator tabNameEnum = dbTabNameList.GetEnumerator();
					//List<String>.Enumerator colNameEnum = dbColNameList.GetEnumerator();
					List<String>.Enumerator subNameEnum = dbSubNameList.GetEnumerator();
					List<Boolean>.Enumerator isPKEnum = dbisPKNameList.GetEnumerator();
					List<Boolean>.Enumerator isFKEnum = dbisFKNameList.GetEnumerator();
					List<String>.Enumerator colLocalNameEnum = dbColLocalNameList.GetEnumerator();
					List<Boolean>.Enumerator colDescEnum = dbColDesc.GetEnumerator();

					while (subNameEnum.MoveNext())
					{
						tabNameEnum.MoveNext();
						//colNameEnum.MoveNext();
						isPKEnum.MoveNext();
						isFKEnum.MoveNext();
						colLocalNameEnum.MoveNext();
						colDescEnum.MoveNext();

						if (subNameEnum.Current.Equals(col_name, StringComparison.CurrentCultureIgnoreCase))
						{
							tmpTabNameList.Add(tabNameEnum.Current);
							//tmpColNameList.Add(colNameEnum.Current);
							tmpSubNameList.Add(subNameEnum.Current);
							tmpisPKNameList.Add(isPKEnum.Current);
							tmpisFKNameList.Add(isFKEnum.Current);
							tmpColLocalNameList.Add(colLocalNameEnum.Current);
							tmpColDescList.Add(colDescEnum.Current);
						}
					}
				}
				pTabNames = tmpTabNameList.ToArray();
				//pColNames = tmpColNameList.ToArray();
				pSubNames = tmpSubNameList.ToArray();
				pisPK = tmpisPKNameList.ToArray();
				pisFK = tmpisFKNameList.ToArray();
				pColLocal = tmpColLocalNameList.ToArray();
				pDesc = tmpColDescList.ToArray();
			}
			else
			{
				pTabNames = dbTabNameList.ToArray();
				//pColNames = dbColNameList.ToArray();
				pSubNames = dbSubNameList.ToArray();
				pisPK = dbisPKNameList.ToArray();
				pisFK = dbisFKNameList.ToArray();
				pColLocal = dbColLocalNameList.ToArray();
				pDesc = dbColDesc.ToArray();
			}
			return;
		}

		//Genera Lock Access para la consulta. Lo necesito para los cnt.
		public String countQryText(JObject param, CfDbTable.ListOption list_option, CfDbTable.OrderByOption order_option, 
				CfDbTable.LockByOption lock_option
			)
		{
			List<String> strTbNameList = new List<String>();

			strTbNameList.Add(this.TbName);
			// Armo string de lock access.
			String qry_txt = "";
			qry_txt += "LOCKING TABLE ";
			qry_txt += this.DbName;
			qry_txt += ".";
			qry_txt += this.TbName;
			qry_txt += " FOR ACCESS";
			qry_txt += "\n";

			if (this.relParentTabs != null)
			{
				foreach (CfDbRelation r in this.relParentTabs)
				{
					if (!strTbNameList.Exists(x => x.Equals(r.ParentTable.TbName, StringComparison.CurrentCultureIgnoreCase)))
					{
						strTbNameList.Add(r.ParentTable.TbName);
						qry_txt += "LOCKING TABLE ";
						qry_txt += r.ParentTable.DbName;
						qry_txt += ".";
						qry_txt += r.ParentTable.TbName;
						qry_txt += " FOR ACCESS";
						qry_txt += "\n";

						// Si la tabla padre tiene una columna que
						// es descripcion y ademas foreign key, tambien
						// joineo su padre.
						foreach (CfDbColumn c in r.ParentTable.DbCols)
						{
							if (c.isDesc && c.isFk) // Si tiene una columna descripcion y fk 
							{
								// tengo que joinear al padre, porque inclui su descripcion.
								CfDbRelation grandParentRelation = null;
								CfDbTable grandParentTable = null;

								//Busco cual es la tabla que origina esta fk de la tabla padre.
								foreach (CfDbRelation r2 in r.ParentTable.relParentTabs)
								{
									//Busco el atributo.
									foreach (CfDbRelAttribute ra2 in r2.RelAtts)
									{
										// Comparo con ChildColumn, porque es el nombre que migro a la tabla
										if (c.ColName.Equals(ra2.ChildColumn.ColName, StringComparison.CurrentCultureIgnoreCase))
										{
											grandParentRelation = r2;
											grandParentTable = ra2.ParentColumn.DbTable;
										}
									}
								}

								if (grandParentTable != null)
								{
									if (!strTbNameList.Exists(x => x.Equals(grandParentTable.TbName, StringComparison.CurrentCultureIgnoreCase)))
									{
										strTbNameList.Add(grandParentTable.TbName);
										qry_txt += "LOCKING TABLE ";
										qry_txt += grandParentTable.DbName;
										qry_txt += ".";
										qry_txt += grandParentTable.TbName;
										qry_txt += " FOR ACCESS";
										qry_txt += "\n";
									}
								}
							}
						}
					}
				}
			}

			if (this.lkpTabs != null)
			{
				foreach (CfDbTable t in this.lkpTabs)
				{
					if (!strTbNameList.Exists(x => x.Equals(t.TbName, StringComparison.CurrentCultureIgnoreCase)))
					{
						strTbNameList.Add(t.TbName);
						qry_txt += "LOCKING TABLE ";
						qry_txt += t.DbName;
						qry_txt += ".";
						qry_txt += t.TbName;
						qry_txt += " FOR ACCESS";
						qry_txt += "\n";
					}
				}
			}

			if (this.filtTabs != null)
			{
				foreach (CfDbTable t in this.filtTabs)
				{
					if (!strTbNameList.Exists(x => x.Equals(t.TbName, StringComparison.CurrentCultureIgnoreCase)))
					{
						strTbNameList.Add(t.TbName);
						qry_txt += "LOCKING TABLE ";
						qry_txt += t.DbName;
						qry_txt += ".";
						qry_txt += t.TbName;
						qry_txt += " FOR ACCESS";
						qry_txt += "\n";
					}
				}
			}

			qry_txt += "SELECT COUNT(*) RecCnt\n";
			qry_txt += "FROM\n";
			qry_txt += "(\n";
			qry_txt += this.listQryText(param, list_option, order_option, lock_option, CfDbTable.ColumnListOption.DEFAULT_COLUMNS);
			qry_txt += ") tmpRecCnt\n";

			return qry_txt;
		}//Fin countQryText

		public String listLockQryText(JObject param)
		{
			String lock_txt = "";
			List<String> strTbNameList = new List<String>();

			strTbNameList.Add(this.TbName);
			lock_txt += "LOCKING TABLE ";
			lock_txt += this.DbName;
			lock_txt += ".";
			lock_txt += this.TbName;
			lock_txt += " FOR ACCESS";
			lock_txt += "\n";

			if (this.relParentTabs != null)
			{
				foreach (CfDbRelation r in this.relParentTabs)
				{
					if (!strTbNameList.Exists(x => x.Equals(r.ParentTable.TbName, StringComparison.CurrentCultureIgnoreCase)))
					{
						strTbNameList.Add(r.ParentTable.TbName);
						lock_txt += "LOCKING TABLE ";
						lock_txt += r.ParentTable.DbName;
						lock_txt += ".";
						lock_txt += r.ParentTable.TbName;
						lock_txt += " FOR ACCESS";
						lock_txt += "\n";

						// Si la tabla padre tiene una columna que
						// es descripcion y ademas foreign key, tambien
						// joineo su padre.
						foreach (CfDbColumn c in r.ParentTable.DbCols)
						{
							if (c.isDesc && c.isFk) // Si tiene una columna descripcion y fk 
							{
								// tengo que joinear al padre, porque inclui su descripcion.

								CfDbRelation grandParentRelation = null;
								CfDbTable grandParentTable = null;
								//Busco cual es la tabla que origina esta fk de la tabla padre.
								foreach (CfDbRelation r2 in r.ParentTable.relParentTabs)
								{
									//Busco el atributo.
									foreach (CfDbRelAttribute ra2 in r2.RelAtts)
									{
										// Comparo con ChildColumn, porque es el nombre que migro a la tabla
										if (c.ColName.Equals(ra2.ChildColumn.ColName, StringComparison.CurrentCultureIgnoreCase))
										{
											grandParentRelation = r2;
											grandParentTable = ra2.ParentColumn.DbTable;
										}
									}
								}
								if (grandParentTable != null)
								{
									if (!strTbNameList.Exists(x => x.Equals(grandParentTable.TbName, StringComparison.CurrentCultureIgnoreCase)))
									{
										strTbNameList.Add(grandParentTable.TbName);
										lock_txt += "LOCKING TABLE ";
										lock_txt += grandParentTable.DbName;
										lock_txt += ".";
										lock_txt += grandParentTable.TbName;
										lock_txt += " FOR ACCESS";
										lock_txt += "\n";
									}
								}
							}
						}
					}
				}
			}

			if (this.lkpTabs != null)
			{
				foreach (CfDbTable t in this.lkpTabs)
				{
					if (!strTbNameList.Exists(x => x.Equals(t.TbName, StringComparison.CurrentCultureIgnoreCase)))
					{
						strTbNameList.Add(t.TbName);
						lock_txt += "LOCKING TABLE ";
						lock_txt += t.DbName;
						lock_txt += ".";
						lock_txt += t.TbName;
						lock_txt += " FOR ACCESS";
						lock_txt += "\n";
					}
				}
			}

			if (this.filtTabs != null)
			{
				foreach (CfDbTable t in this.filtTabs)
				{
					if (!strTbNameList.Exists(x => x.Equals(t.TbName, StringComparison.CurrentCultureIgnoreCase)))
					{
						strTbNameList.Add(t.TbName);
						lock_txt += "LOCKING TABLE ";
						lock_txt += t.DbName;
						lock_txt += ".";
						lock_txt += t.TbName;
						lock_txt += " FOR ACCESS";
						lock_txt += "\n";
					}
				}
			}

			return lock_txt;
		}
		

		/*
		public String listQryText(JObject param, OrderByOption ordby_option)
		{
			return list_base_QryText(param, ListOption.LIST_ALL,ordby_option);
		}

		public String listCurrentQryText(JObject param, OrderByOption ordby_option)
		{
			return list_base_QryText(param, ListOption.LIST_CURRENT, ordby_option);
		}

		public String listCurrentAndFutureQryText(JObject param, OrderByOption ordby_option)
		{
			return list_base_QryText(param, ListOption.LIST_CURRENT_AND_FUTURE, ordby_option);
		}
		*/
		//---------------------------------------------------------------------------------
		//---------------------------------------------------------------------------------
		public String readQryText(JObject param)
		{
			// Armo string de consulta.
			String sep = " ";
			String qry_txt = "SELECT";
			foreach (CfDbColumn c in this.DbCols)
			{
				qry_txt += sep;
				qry_txt += c.ColName;
				sep = ",";
			}
			qry_txt += " FROM ";
			qry_txt += this.DbName;
			qry_txt += ".";
			qry_txt += this.TbName;
			qry_txt += " WHERE";

			String cond_sep = "    ";
			foreach (JProperty t in param.Properties())
			{
				qry_txt += cond_sep;
				qry_txt += t.Name.ToString();
				qry_txt += "=";
				//qry_txt += t.Value.ToString();
				qry_txt += "?"; // Si se vuelve a usar la misma qry, ya podria tener el plan.
				cond_sep = "AND ";
			}
			return qry_txt;
		}
		//----------------------------------------------------------------------------------------------
		// Merge, para insertar/actualizar en tablas que no llevan historia. Alcanza una sola operacion.
		public String mergeQryText( out String[] paramNames)
		{
			// Armo string de consulta.
			String sep = " ";
			int param_cnt=0;
 
			// Se crea array de salida con lista de parametros de query.
			// Para el tamaño se descuenta fecha y hora de tracking, si estan definidas. 
			int paramSize = this.PkFldCnt + this.NoPkFldCnt - (this.TrkDt_Column != null ? 1 : 0) - (this.TrkTm_Column != null ? 1 : 0);
			paramNames = new String[ paramSize ];
			
			String qry_txt = "MERGE INTO ";
			qry_txt += this.DbName;
			qry_txt += ".";
			qry_txt += this.TbName;
			qry_txt += " AS __tbl\n";
			qry_txt += "USING (\n";
			qry_txt += "SELECT\n";
	
			foreach (CfDbColumn c in this.DbCols)
			{
				qry_txt += sep;

				if (c.isTrkDt) // Tracking date.
				{
					qry_txt += "CURRENT_DATE";
				}
				else if (c.isTrkTm) // Tracking time.
				{
					qry_txt += "CURRENT_TIME(6)";
				}
				else
				{
					qry_txt += "CAST( ? AS ";
					qry_txt += c.DataType;
					qry_txt += " )";
					paramNames[param_cnt++] = c.ColName; //Inserto en lista, y "luego" incremento indice.
				}
				qry_txt += " ";
				qry_txt += c.ColName;

				qry_txt += "\n";
				sep = ",";
			}
			qry_txt += ") AS __upd\n";
			/*
						sep = " ";
						foreach (CfDbColumn c in this.DbCols)
						{
							qry_txt += sep;
							qry_txt += '"';
							qry_txt += c.ColName;
							qry_txt += '"';
							sep = ",\n";
						}
						qry_txt += ") ON\n";
   
			 */

			// Join condition
			qry_txt += "ON\n";

			sep = "    ";
			foreach (CfDbColumn c in this.PkColumns)
			{
				if( c.isPk )
				{
					qry_txt += sep;
					qry_txt += "__tbl.";
					qry_txt += c.ColName;
					qry_txt += "=";
					qry_txt += "__upd.";
					qry_txt += c.ColName;
					qry_txt += "\n";
					sep = "AND ";
				}
			}
			// If Matched

			if (this.NoPkColumns.Length > 0)
			{
				qry_txt += "WHEN MATCHED THEN UPDATE\n";
				qry_txt += "SET\n";
			}
			
			sep = " ";
			foreach (CfDbColumn c in this.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += c.ColName;
				qry_txt += " = ";
				qry_txt += "__upd."; qry_txt += c.ColName;
				qry_txt += "\n";
				sep = ",";
			}
			qry_txt += " WHEN NOT MATCHED THEN INSERT\n";
			qry_txt += "(\n";
			sep = " ";
			foreach (CfDbColumn c in this.DbCols)
			{
					qry_txt += sep;
					qry_txt += c.ColName;
					qry_txt += "\n";
					sep = ",";
			}
			qry_txt += ")\n";
			qry_txt += "VALUES\n";
			qry_txt += "(\n";

			sep = " ";
			foreach (CfDbColumn c in this.DbCols)
			{
				qry_txt += sep;
				qry_txt += "__upd."; qry_txt += c.ColName;
				qry_txt += "\n";
				sep = ",";
			}
			qry_txt += ")\n";

			return qry_txt;
		}
		//---------------------------------------------------------------------------------

		public String mergeHistQryText(out String[] paramNames)
		{
			//Genera multistatement para deletear/actualizar registros preexistentes
			// e insertar nuevo registro.
			return this.merge_delete_HistQryText(true, out paramNames);
		}
		//----------------------------------------------------------------------------------------------
		// Merge, para insertar/actualizar en tablas que llevan historia. Alcanza una sola operacion?.
		private String merge_delete_HistQryText(bool forMerge, out String[] paramNames)
		{
			// Armo string de consulta.
			String sep = " ";
			String qry_txt ="";

			/*
			 * La cantidad maxima de parametros que voy a utilizar son
			 * ( campos clave + campos no clave + fecha desde + fecha hasta + fecha desde original para join ) +
			 * ( 4 * ( 2 (fecha desde y fecha hasta) + campos clave (sin fecha)) ) +
			 * cantidad total de campos (por el insert)
			*/
			int max_param_cnt=0;
			int param_cnt = 0;

			if (forMerge)
			{
				// En el merge se contemplan los parametros del registro que se inserta.
				// Para el tamaño se descuenta fecha y hora de tracking, si estan definidas. 
				max_param_cnt = (this.PkFldCnt + this.NoPkFldCnt + 2 + 1) + (4 * (this.PkFldCnt + 2))
					 - (this.TrkDt_Column != null ? 1 : 0) - (this.TrkTm_Column != null ? 1 : 0);

				/*
				// Ya viene corregido. Gerardo 2013-06-25.
				//Por cada campo no clave que es numerico, necesito 5 posiciones extra por el case para las ',' (comas).
				foreach (CfDbColumn c in this.NoPkColumns)
				{
					if (c.isNumber)
					{
						max_param_cnt += 5;
					}
				}
				*/
			}
			else
			{
				// En el delete se contemplan solo los parametros para corregir registros existentes.
				max_param_cnt = (4 * (this.PkFldCnt + 2));
			}

			//Se crea array de salida con lista de parametros de query.
			paramNames = new String[max_param_cnt];
			param_cnt = 0;


			// Comienzo a armar el sql de actualizacion.

			/* 
				Caso 4
				El periodo del registro existente esta completamente contenido en el periodo del nuevo registro.
				El registro existente debe eliminarse.
			*/

			qry_txt += "DELETE --Caso 4\n";
			qry_txt += this.DbName;
			qry_txt += ".";
			qry_txt += this.TbName;
			qry_txt += "\n";
			qry_txt += "WHERE\n";

			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += col.ColName;
				qry_txt += " = CAST(? AS ";
				qry_txt += col.DataType; ;
				qry_txt += ")\n";
				sep = "AND ";
				paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
			}
			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += "CAST( ? AS DATE FORMAT 'YYYY-MM-DD') <= ";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";
			sep = "AND ";
			paramNames[param_cnt++] = StartDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.


			// EndDt
			qry_txt += sep;
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " <= CAST( ? AS DATE FORMAT 'YYYY-MM-DD')";
			qry_txt += "\n";
			sep = "AND ";
			paramNames[param_cnt++] = this.EndDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.

			qry_txt += ";";



			qry_txt += "MERGE INTO ";
			qry_txt += this.DbName;
			qry_txt += ".";
			qry_txt += this.TbName;
			qry_txt += " AS __tbl\n";
			qry_txt += "USING(\n";


			/*
			Caso 1
			El nuevo registro esta en medio del  periodo de un unico registro.
			El registro existente debe partirse en un primer periodo precedente al 
			nuevo ( nro 1), y otro posterior al nuevo ( nro 3). El nuevo, sera el 2 en 
			la nueva serie.
			*/
			
			qry_txt += "SELECT --Caso 1\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in this.PkColumns)
			{
					qry_txt += sep;
					qry_txt += "o.";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
			}
			/*-- Necesito una columna con StartDt original, para joinear,
			 * ya que el inicio es una columna que puede cambiar,
			 * en otros casos, y es pk.
			 * En el registro identificado 3, como es un registro nuevo
			 * debe ser igual a StartDt.
			*/
			qry_txt += sep;
			qry_txt += "(CASE\n";
			qry_txt += "WHEN Ord.Ord = 1 THEN o.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += "WHEN Ord.Ord = 3 THEN n.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " + 1\n";
			
			qry_txt += "END)";
			qry_txt += " AS _J_"; // Para armar el alias de esta columna uso el prefijo "_J_".
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";


			/* Armo la columna con StartDt, teniendo en cuenta si corresponde al periodo
			 * previo al nuevo registro o periodo posterior.
			 * Esto se hace mediante una columna que se genera dinamicamente, que puede ser
			 * '1' para el registro previo y '3' para el registro posterior. El '2' corresponderia
			 * al nuevo registro, pero no se usa.
			 * Si la nueva fecha de fin es '9999-12-31' no hace falta generar la fila posterior, ya
			 * que esa es la maxima fecha que se puede representar.
			*/
			qry_txt += sep;
			qry_txt += "(CASE\n";
			qry_txt += "WHEN Ord.Ord = 1 THEN o.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			/*
			 * Este caso no sucede por el filtro de esta subquery.
			qry_txt += "WHEN Ord.Ord = 3 AND n.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " = DATE '9999-12-31' THEN n.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " + 1\n";
			*/

			qry_txt += "WHEN Ord.Ord = 3 THEN n.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " + 1\n";
			
			qry_txt += "END)";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			/* Armo la columna con EndDt, teniendo en cuenta si corresponde al periodo
			 * previo al nuevo registro o periodo posterior.
			 * Esto se hace mediante una columna que se genera dinamicamente, que puede ser
			 * '1' para el registro previo y '3' para el registro posterior. El '2' corresponderia
			 * al nuevo registro, pero no se usa.
			*/

			qry_txt += sep;
			qry_txt += "(CASE Ord.Ord\n";
			qry_txt += "WHEN 1 THEN n.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " - 1\n ";
			qry_txt += "WHEN 3 THEN o.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";
			qry_txt += "END)";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";
	
			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in this.NoPkColumns)
			{
					qry_txt += sep;
					qry_txt += "o.";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
			}

			//Clausula from
			qry_txt += "FROM\n";
			qry_txt += this.DbName;
			qry_txt += ".";
			qry_txt += this.TbName;
			qry_txt += " AS o\n";
			qry_txt += "INNER JOIN\n";
			qry_txt += "(SELECT\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in this.PkColumns)
			{
					qry_txt += sep;
					qry_txt += "CAST( ? AS ";
					qry_txt += col.DataType;
					qry_txt += " ) ";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
					paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
			}
			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += "CAST( ? AS DATE FORMAT 'YYYY-MM-DD') ";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";
			sep = ",";
			paramNames[param_cnt++] = this.StartDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.


			// EndDt
			qry_txt += sep;
			qry_txt += "CAST( ? AS DATE FORMAT 'YYYY-MM-DD') ";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";
			sep = ",";
			paramNames[param_cnt++] = this.EndDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.

			qry_txt += ")n\n";
			qry_txt += "ON\n";

			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
					qry_txt += sep;
					qry_txt += "o.";
					qry_txt += col.ColName;
					qry_txt += "=n.";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = "AND ";
			}
			// Condiciones de fecha.
			// AND	o.T_Start <= n.T_Start AND n.T_End <= o.T_End

			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " <= ";
			qry_txt += "n.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";
			sep = "AND ";

			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " <= ";
			qry_txt += "o.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";

			// La siguiente tabla permite generar dos registros de salida.
			qry_txt += "CROSS JOIN\n";
			qry_txt += "(\n";
			qry_txt += "SELECT Ord FROM  (SELECT 1 Ord ) t1\n"; //Para generar el registro previo al nuevo.
			qry_txt += "UNION\n";
			qry_txt += "SELECT Ord FROM  (SELECT 3 Ord ) t3\n"; //Para generar el registro posterior al nuevo.
			qry_txt += ") Ord\n";

			qry_txt += "WHERE\n";

			//Filtro por inicio de periodo. El registro 1 no sale si el nuevo empieza el mismo dia.
			qry_txt += "    NOT (  Ord.Ord = 1 AND o.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " = n.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += ")\n";

			//Filtro por fin de periodo. El registro 3 no sale si el nuevo termina el mismo dia.
			qry_txt += "AND NOT (  Ord.Ord = 3 AND n.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " = o.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " )\n";

			qry_txt += "UNION\n";

			/*
				Caso 2
				El periodo del nuevo registro comienza antes que el periodo del registro existente,
				y finaliza antes que el fin del  registro existente.
				El registro existente debe actualizarse para que cominenze luego del registro nuevo.
			*/

			qry_txt += "SELECT --Caso 2\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}
			/*-- Necesito una columna con StartDt original, para joinear,
			 * ya que el inicio es una columna que puede cambiar,
			 * en otros casos, y es pk.
			*/
			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " AS _J_"; // Para armar el alias de esta columna uso el prefijo "_J_".
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " + 1 ";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			// EndDt
			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " ";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";


			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in this.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Clausula from
			qry_txt += "FROM\n";
			qry_txt += this.DbName;
			qry_txt += ".";
			qry_txt += this.TbName;
			qry_txt += " AS o\n";
			qry_txt += "INNER JOIN\n";
			qry_txt += "(SELECT\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "CAST( ? AS ";
				qry_txt += col.DataType;
				qry_txt += " ) ";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
				paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
			}
			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += "CAST( ? AS DATE FORMAT 'YYYY-MM-DD') ";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";
			sep = ",";
			paramNames[param_cnt++] = this.StartDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.


			// EndDt
			qry_txt += sep;
			qry_txt += "CAST( ? AS DATE FORMAT 'YYYY-MM-DD') ";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";
			sep = ",";
			paramNames[param_cnt++] = this.EndDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.

			qry_txt += ")n\n";
			qry_txt += "ON\n";

			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "=n.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}
			// Condiciones de fecha.
			//AND	n.T_Start < o.T_Start
			//AND   o.T_Start <= n.T_End
			//AND	n.T_End <= o.T_End

			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " < ";
			qry_txt += "o.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " <= ";
			qry_txt += "n.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " <= ";
			qry_txt += "o.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += "UNION\n";


			/*
			Caso 3
			El periodo del nuevo registro comienza despues que el periodo del registro existente,
			y finaliza despues que el fin del  registro existente.

			El registro existente debe actualizarse para que finalice antes que el registro nuevo.
			*/

			qry_txt += "SELECT --Caso 3\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}
			/*-- Necesito una columna con StartDt original, para joinear,
			 * ya que el inicio es una columna que puede cambiar,
			 * en otros casos, y es pk.
			*/
			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " AS _J_"; // Para armar el alias de esta columna uso el prefijo "_J_".
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " ";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			// EndDt
			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " - 1 ";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";


			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in this.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Clausula from
			qry_txt += "FROM\n";
			qry_txt += this.DbName;
			qry_txt += ".";
			qry_txt += this.TbName;
			qry_txt += " AS o\n";
			qry_txt += "INNER JOIN\n";
			qry_txt += "(SELECT\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach ( CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "CAST( ? AS ";
				qry_txt += col.DataType;
				qry_txt += " ) ";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
				paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
			}
			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += "CAST( ? AS DATE FORMAT 'YYYY-MM-DD') ";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";
			sep = ",";
			paramNames[param_cnt++] = this.StartDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.


			// EndDt
			qry_txt += sep;
			qry_txt += "CAST( ? AS DATE FORMAT 'YYYY-MM-DD') ";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";
			sep = ",";
			paramNames[param_cnt++] = this.EndDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.

			qry_txt += ")n\n";
			qry_txt += "ON\n";

			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "=n.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}
			// Condiciones de fecha.
			//AND	o.T_Start < n.T_Start
			//AND   n.T_Start < o.T_End
			//AND	o.T_End < n.T_End

			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " < ";
			qry_txt += "n.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += " <= ";
			qry_txt += "o.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += " < ";
			qry_txt += "n.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";

			if (forMerge) // Inicio seccion insert nuevo registro. Vale para merge, se omite para delete.
			{
				qry_txt += "UNION\n";

				/*
				 * Este es el registro nuevo completo, para la insercion.
				 * Se agrega columna StartDt para join.
				 * Se hace un join anidado para que la union lo vea como tabla.
				*/

				qry_txt += "SELECT\n";
				//Primero la pk, que no sea StartDt.
				sep = " ";
				foreach (CfDbColumn col in this.PkColumns)
				{
					qry_txt += sep;
					qry_txt += "n.";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
				}
				/*-- Necesito una columna con StartDt original, para joinear,
				 * ya que el inicio es una columna que puede cambiar,
				 * en otros casos, y es pk.
				*/
				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " AS _J_"; // Para armar el alias de esta columna uso el prefijo "_J_".
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += "\n";

				//Luego colummnas fecha
				// StartDt
				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += "\n";

				// EndDt
				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += this.EndDt_Column.ColName;
				qry_txt += "\n";


				//Al final, el resto de las columnas.
				foreach (CfDbColumn col in this.NoPkColumns)
				{
					qry_txt += sep;
					qry_txt += "n.";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
				}

				//Clausula from
				qry_txt += "FROM\n";
				qry_txt += "(SELECT\n";

				//Primero la pk, que no sea StartDt.
				sep = " ";
				foreach (CfDbColumn col in this.PkColumns)
				{
					qry_txt += sep;
					qry_txt += "CAST( ? AS ";
					qry_txt += col.DataType;
					qry_txt += " ) ";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
					paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
				}

				/*-- Necesito una columna con StartDt original, para joinear,
				 * ya que el inicio es una columna que puede cambiar,
				 * en otros casos, y es pk.
				*/
				qry_txt += sep;
				qry_txt += "CAST( ? AS DATE FORMAT 'YYYY-MM-DD') _J_";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += "\n";
				sep = ",";
				paramNames[param_cnt++] = this.StartDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.

				//Luego colummnas fecha
				// StartDt
				qry_txt += sep;
				qry_txt += "CAST( ? AS DATE FORMAT 'YYYY-MM-DD') ";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += "\n";
				sep = ",";
				paramNames[param_cnt++] = this.StartDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.


				// EndDt
				qry_txt += sep;
				qry_txt += "CAST( ? AS DATE FORMAT 'YYYY-MM-DD') ";
				qry_txt += this.EndDt_Column.ColName;
				qry_txt += "\n";
				sep = ",";
				paramNames[param_cnt++] = this.EndDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.

				//Al final, el resto de las columnas.
				foreach (CfDbColumn col in this.NoPkColumns)
				{
					qry_txt += sep;
					
					if (col.isTrkDt) // Tracking date.
					{
					qry_txt += "CURRENT_DATE";
					}
					else if (col.isTrkTm) // Tracking time.
					{
						qry_txt += "CURRENT_TIME(6)";
					}
					else
					{
						qry_txt += "CAST( ";
						/*
						 * Se comento porque ya viene corregido. Gerardo 2013-06-25
						if (col.isNumber) // Si el dato es numero, si viene con ',' (coma), lo reemplazo por '.' (punto).
						{
							//Busca ',' y reemplaza por '.', sino el original.
							qry_txt += "(CASE WHEN INDEX(?,',') <> 0 THEN SUBSTR( ?,1, INDEX(?,',') -1 ) || '.' || SUBSTR( ?, INDEX(?,',') + 1 ) ELSE ? END)";
							paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
							paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
							paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
							paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
							paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
							paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
						}
						else
						{
							qry_txt += "?"; // Sino, simplemente el string de entrada.
							paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
						}
						*/
						qry_txt += "?";
						paramNames[param_cnt++] = col.ColName; //Inserto en lista, y "luego" incremento indice.
						qry_txt += " AS ";
						qry_txt += col.DataType;
						qry_txt += " )";
					}
					qry_txt += " ";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
				}
				qry_txt += ")n\n";
			} // Fin seccion insert nuevo registro.

			qry_txt += ")__u\n";
			qry_txt += "ON\n";

			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "__tbl.";
				qry_txt += col.ColName;
				qry_txt += "=";
				qry_txt += "__u.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}
			//Fecha de join, la original del registro.
			qry_txt += sep;
			qry_txt += "__tbl."; // En la tabla uso el nombre original.
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "=";
			qry_txt += "__u._J_"; // En el update uso la fecha reservada para el updata. con prefijo _J_
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += "WHEN MATCHED THEN UPDATE\n";
			qry_txt += "SET\n";

			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "=";
			qry_txt += "__u."; // Para armar el alias de esta columna uso el prefijo "_J_".
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += ",";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "=";
			qry_txt += "__u."; // Para armar el alias de esta columna uso el prefijo "_J_".
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";

			//Actualizo las columnas que no son pk
			sep = ",";
			//qry_txt += ",";
			foreach (CfDbColumn c in this.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += c.ColName;
				qry_txt += " = ";
				qry_txt += "(CASE WHEN ";
				qry_txt += "__u._J_";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " = ";
				qry_txt += "__u.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " THEN ";
				qry_txt += "__u.";
				qry_txt += c.ColName;
				qry_txt += " ELSE ";
				qry_txt += "__tbl.";
				qry_txt += c.ColName;
				qry_txt += " END)";
				qry_txt += "\n";
				sep = ",";
			}

			qry_txt += "WHEN NOT MATCHED THEN INSERT\n";
			qry_txt += "(\n";


			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			// EndDt
			qry_txt += sep;
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";


			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in this.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			qry_txt += ")\n";
			qry_txt += "VALUES\n";
			qry_txt += "(\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "__u.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += "__u.";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";

			// EndDt
			qry_txt += sep;
			qry_txt += "__u.";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += "\n";


			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in this.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += "__u.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			qry_txt += ")\n";
			
			///////////////////
			return qry_txt;
		}
	//---------------------------------------------------------------------------------

		// Delete, para deletear registros en tablas que no llevan historia. Alcanza una sola operacion.
		public String deleteQryText( out String[] paramNames)
		{
			// Armo string de consulta.
			String sep;
			int param_cnt=0;
 
			//Se crea array de salida con lista de parametros de query.
			paramNames = new String[ this.PkFldCnt ];
			
			String qry_txt = "DELETE FROM\n";
			qry_txt += this.DbName;
			qry_txt += ".";
			qry_txt += this.TbName;
			qry_txt += "\n";
			qry_txt += "WHERE\n";

			sep = "    ";
			foreach (CfDbColumn c in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += c.ColName;
				qry_txt += " = CAST( ? AS ";
				qry_txt += c.DataType;
				qry_txt += " )\n";
				sep = "AND ";
				paramNames[param_cnt++] = c.ColName; //Inserto en lista, y "luego" incremento indice.
			}
			return qry_txt;
		}

		//---------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------------------
		// Merge, para insertar/actualizar en tablas que llevan historia. Alcanza una sola operacion?.
		public String deleteHistQryText(out String[] paramNames)
		{
			//Genera multistatement para deletear/actualizar registros preexistentes.
			//El false, produce que no genere statement para insertar el registro.
			return this.merge_delete_HistQryText(false, out paramNames);
		}


		/// <summary>
		/// Retorna cantidad de tablas padre.
		/// </summary>
		/// <returns></returns>
		public int getParentTablesCnt()
		{
			if (this.relParentTabs == null)
			{
				return 0;
			}
			else
			{
				return this.relParentTabs.Length;
			}
		}

		//Retorna cantidad de tablas hijas
		public int getChildTablesCnt()
		{
			if (this.relChildTabs == null)
			{
				return 0;
			}
			else
			{
				return this.relChildTabs.Length;
			}
		}

		/// <summary>
		/// Este metodo sirve para controlar la integridad de un registro cuando se inserta.
		/// Genera el comando sql, para verificar una foreign key.
		/// El parametro "relNo" indica que relacion se va a verificar, y en "paramNames",
		/// se pasa la lista de parametros de la consulta.
		/// </summary>
		/// <param name="relNo"></param>
		/// <param name="paramNames"></param>
		/// <returns></returns>
		public String riParentCheckQryText(int relNo, out String parentTableName, out String[] paramNames, out String parentTableLocalName, 
				out String[] paramLocalNames)
		{
			String qry_txt = "";
			int max_param_cnt;
			int param_cnt = 0;
		   
			int size = this.relParentTabs.Length;
			if (relNo < 0 || this.relParentTabs.Length <= relNo)
			{
				throw new Exception("Indice de tabla padre invalido.");
			}

			CfDbRelation parRel = this.relParentTabs[relNo];
			max_param_cnt = parRel.RelAtts.Length;

			parentTableName = parRel.ParentTable.TbName;
			parentTableLocalName = parRel.ParentTable.TbLocalName;
			paramNames = new String[max_param_cnt];
			paramLocalNames = new String[max_param_cnt];

			qry_txt += "SELECT 1\n";
			qry_txt += "FROM\n";
			qry_txt += parRel.ParentTable.DbName;
			qry_txt += ".";
			qry_txt += parRel.ParentTable.TbName;
			qry_txt += "\n";
			qry_txt += "WHERE\n";

			String sep = "    ";
			foreach (CfDbRelAttribute ra in parRel.RelAtts)
			{
					qry_txt += sep;
					qry_txt += ra.ParentColumn.ColName;
					qry_txt += " = CAST( ? AS ";
					qry_txt += ra.ParentColumn.DataType;
					qry_txt += " )\n";
					sep = "AND ";
					paramNames[param_cnt] = ra.ChildColumn.ColName; //Inserto en lista el nombre de la columna de la tabla hija, y "luego" incremento indice.
					paramLocalNames[param_cnt++] = ra.ChildColumn.ColLocalName; //Inserto en lista el nombre de la columna de la tabla hija en castellano, y "luego" incremento indice.
			}
			return qry_txt;
		}

		public String riChildCheckQryText(int relNo, out String childTableName, out String[] paramNames, out String childTableLocalName, 
				out String[] paramLocalNames)
		{
			String qry_txt = "";
			int max_param_cnt;
			int param_cnt = 0;

			int size = this.relChildTabs.Length;
			if (relNo < 0 || this.relChildTabs.Length <= relNo)
			{
				throw new Exception("Indice de tabla hija invalido.");
			}

			CfDbRelation parRel = this.relChildTabs[relNo];
			max_param_cnt = parRel.RelAtts.Length;

			childTableName = parRel.ChildTable.TbName;
			childTableLocalName = parRel.ChildTable.TbLocalName;
			paramNames = new String[max_param_cnt];
			paramLocalNames = new String[max_param_cnt];

			qry_txt += "SELECT TOP 1 1\n";
			qry_txt += "FROM\n";
			qry_txt += parRel.ChildTable.DbName;
			qry_txt += ".";
			qry_txt += parRel.ChildTable.TbName;
			qry_txt += "\n";
			qry_txt += "WHERE\n";

			String sep = "    ";
			foreach (CfDbRelAttribute ra in parRel.RelAtts)
			{
				qry_txt += sep;
				qry_txt += ra.ChildColumn.ColName;
				qry_txt += " = CAST( ? AS ";
				qry_txt += ra.ChildColumn.DataType;
				qry_txt += " )\n";
				sep = "AND ";
				paramNames[param_cnt] = ra.ParentColumn.ColName; //En lista el nombre de la columna en latabla padre, y "luego" incremento indice.
				paramLocalNames[param_cnt++] = ra.ParentColumn.ColLocalName; //En lista el nombre de la columna en castellano en la tabla padre, y "luego" incremento indice.
			}
			return qry_txt;
		}

		/// <summary>
		/// Query para listar registros que se superponen a un periodo requerido.
		/// Sirva para leer registros que se deben splitear segun otro registro recibido.
		/// Se filtra por los campos de la pk informados en el objeto "param".
		/// </summary>
		/// <param name="paramNames"></param>
		/// <returns></returns>
		public String overlapsQryText(JObject param, out String[] paramNames)
		{
			// Armo string de consulta.
			String sep = " ";
			int param_cnt = 0;

			//Cuento claves a usar.
			int pkFldCnt = 0;
			foreach (CfDbColumn k in this.PkColumns)
			{
				if (param.Property(k.ColName) != null)
				{
					pkFldCnt++;
				}
			}

			//Se crea array de salida con lista de parametros de query.
			paramNames = new String[pkFldCnt + 2 + 2]; // Suma campos pk, mas start y end dates, en el select + 2 dates en el where.

			String qry_txt = "SELECT\n";

			//Columnas tabla principal.
			foreach (CfDbColumn c in this.DbCols)
			{
				qry_txt += sep;
				if( c.ColName.Equals(this.StartDt_Column.ColName,StringComparison.CurrentCultureIgnoreCase))
				{
					qry_txt += "CAST( ? AS ";
					qry_txt += c.DataType;
					qry_txt += " ) ";
					qry_txt += c.ColName;
					sep = ",";
					paramNames[param_cnt++] = c.ColName; //Inserto en lista, y "luego" incremento indice.
				}
				else if( c.ColName.Equals(this.EndDt_Column.ColName,StringComparison.CurrentCultureIgnoreCase))
				{
					qry_txt += "CAST( ? AS ";
					qry_txt += c.DataType;
					qry_txt += " ) ";
					qry_txt += c.ColName;
					sep = ",";
					paramNames[param_cnt++] = c.ColName; //Inserto en lista, y "luego" incremento indice.
				}
				else
				{
					qry_txt += c.ColName;
				}
				qry_txt += "\n";
				sep = ",";
			}
			qry_txt += "FROM\n";
			qry_txt += this.DbName;
			qry_txt += ".";
			qry_txt += this.TbName;
			qry_txt += "\n";
			qry_txt += "WHERE\n";

			String cond_sep = "    ";
			foreach (CfDbColumn k in this.PkColumns)
			{
				if (param.Property(k.ColName) != null)
				{
					qry_txt += cond_sep;
					qry_txt += k.ColName;
					qry_txt += " = ?\n";// Si se vuelve a usar la misma qry, ya podria tener el plan.
					paramNames[param_cnt++] = k.ColName; //Inserto en lista, y "luego" incremento indice.
					cond_sep = "AND ";
				}
			}

			// Que el periodo se superponga con el solicitado
			qry_txt += cond_sep;
			qry_txt += "(";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += ", ";
			qry_txt += this.EndDt_Column.ColName;
			qry_txt += ") OVERLAPS (CAST( ? AS DATE ) , CAST( ? AS DATE ))\n";
			paramNames[param_cnt++] = this.StartDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.
			paramNames[param_cnt++] = this.EndDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.

			qry_txt += "QUALIFY\n";
			qry_txt += "    ROW_NUMBER() OVER (\n";
			qry_txt += "        PARTITION BY ";

			String lsep = "";
			foreach (CfDbColumn k in this.PkColumns)
			{
					qry_txt += lsep;
					qry_txt += k.ColName;
					lsep = ", ";
			}

			qry_txt += "\n";
			qry_txt += "        ORDER BY ";
			qry_txt += this.StartDt_Column.ColName;
			qry_txt += "\n";
			qry_txt += "    ) = 1";

			return qry_txt;
		}

		/*
		/// <summary>
		/// Query para listar registros que coincide la clave primaria 
		/// con parte de la clave de otra tabla
		/// Sirve para saber si el registro se puede insertar en la tabla
		/// cuando es una frecuencia o bien cuando una excepcion debe
		/// saber si existe la relacion que la origina
		/// Se filtra por los campos de la pk informados en el objeto "param".
		/// </summary>
		/// <param name="paramNames"></param>
		/// <returns></returns>
		public String listFollowNormalTbl(JObject param, ListOption list_option, out String[] paramNames)
		{
			// Armo string de consulta.
			String sep = " ";
			int param_cnt = 0;

			int tabCnt = 0;                         // Contador para alias de tabla. 0 es la principal.
			String tabStr = "t" + tabCnt.ToString("D2");  // Contador en string para alias de tabla.
			String tabMainStr = tabStr;                // Siempre apunta a la tabla principal.

			//Cuento claves a usar.
			int pkFldCnt = 0;
			foreach (CfDbColumn k in this.PkColumns)
			{
				if (param.Property(k.ColName) != null)
				{
					pkFldCnt++;
				}
			}

			if (param.Property(this.StartDt_Column.ColName) != null)
			{
				pkFldCnt++;
			}


			//Se crea array de salida con lista de parametros de query.
			paramNames = new String[pkFldCnt];

			String qry_txt = "SELECT\n";

			//Columnas tabla principal.
			foreach (CfDbColumn c in this.DbCols)
			{
				qry_txt += sep;
				qry_txt += c.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			qry_txt += "FROM\n";

			// Tabla principal.
			if (!this.isHist || list_option == ListOption.LIST_ALL)
			{
				// Si no tiene historia o elige listar todo, es el caso simple.
				qry_txt += this.DbName;
				qry_txt += ".";
				qry_txt += this.TbName;
				qry_txt += "\n";
			}
			else
			{
				//Es con historia, ademas tengo que ver que opcion.
				//En una subquery filtro los registros que me interesan.
				qry_txt += "(SELECT * FROM ";
				qry_txt += this.DbName;
				qry_txt += ".";
				qry_txt += this.TbName;
				qry_txt += "\n";
				qry_txt += "WHERE\n";

				if (list_option == ListOption.LIST_CURRENT)
				{
					qry_txt += "CURRENT_DATE BETWEEN ";
					qry_txt += this.StartDt_Column.ColName;
					qry_txt += " AND ";
					qry_txt += this.EndDt_Column.ColName;
					qry_txt += "\n";
				}
				else if (list_option == ListOption.LIST_CURRENT_AND_FUTURE)
				{
					qry_txt += "CURRENT_DATE <= ";
					qry_txt += this.EndDt_Column.ColName;
					qry_txt += "\n";
				}
				else
				{
					throw new Exception("Opcion de listado invalido (" + list_option.ToString() + ").");
				}
				qry_txt += ") ";
				qry_txt += tabMainStr;
				qry_txt += "\n";
			}

			qry_txt += "WHERE\n";

			String cond_sep = "    ";
			foreach (CfDbColumn k in this.PkColumns)
			{
				if (param.Property(k.ColName) != null)
				{
					qry_txt += cond_sep;
					qry_txt += k.ColName;
					qry_txt += " = ?\n";// Si se vuelve a usar la misma qry, ya podria tener el plan.
					paramNames[param_cnt++] = k.ColName; //Inserto en lista, y "luego" incremento indice.
					cond_sep = "AND ";
				}
			}


			// Que la fecha de inicio de periodo sea igual al solicitado si lista todo
			if (list_option == ListOption.LIST_ALL)
			{
				qry_txt += cond_sep;
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " = CAST( ? AS ";
				qry_txt += this.StartDt_Column.DataType;
				qry_txt += " )\n";
				paramNames[param_cnt++] = this.StartDt_Column.ColName; //Inserto en lista, y "luego" incremento indice.
			}

			return qry_txt;
		}
		*/


		////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////

		// Carga masiva via fastload.


		public String stgDbName() { return this.DbName + "_STG"; }
		public String stgTbName() { return this.TbName; }
		public String stgE1TbName() { return Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E1"; }
		public String stgE2TbName() { return Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E2"; }
		public String stgErrorTbName() { return Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; }
		public String stgUpdTbName() { return Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_U"; }




		public String fastloadScript(String inDataFileName, out String[] DefineColumnList, out String[] DefineLocalNameList, out bool[] isNumber,
			out bool[] isRequiredOnImport, out bool[] isFillOnImport, out String RowNumColumnName)
		{
			String scriptTxt = "";
			RowNumColumnName = "__IN_FILE_ROW_NUMBER";

			String sleepStr = "6"; // Parametro en config.
			String tenacityStr = "1"; // Parametro en config.
			String maxSessionsStr = "12"; // Parametro en config.
			String minSessionsStr = "12"; // Parametro en config.
			String tdpidStr = Parametros().strIpConfig; // Parametro en config.
			String userStr = Parametros().strDbUserFld;
			String pwdStr = Parametros().strDbPassFld;

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgE1TbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") +  "_E1"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.
			String stgE2TbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E2";  // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.
			String stgUpdTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_U"; // Tabla para seleccionar registros a aplicar.

			//String inFileStr = ""; // Pipe name, generarlo en el momento.

			/* No voy a usar named pipes.
			//scriptTxt = AXSMOD_SETTINGS;
			String np_AXSMOD_log_directory_Str = "./tmp"; // Parametro en config.
			String np_AXSMOD_fallback_file_Str = "Load_clifre_tarj_2_stg_clifre_tarj.fbak"; //Armado en el momento.
			String np_AXSMOD_fallback_directory_Str = "./tmp";  // Parametro en config.
			*/

			scriptTxt += "/***********************************************************************\n";
			scriptTxt += "Description  = Carga de datos a tabla de staging.\n";
			scriptTxt += "***********************************************************************/\n";

			/* No voy a usar named pipes.
			// ++ scriptTxt = AXSMOD_SETTINGS;
			scriptTxt += "AXSMOD np_AXSMOD \"";
			scriptTxt += "log_directory = '" + np_AXSMOD_log_directory_Str + "' ";
			scriptTxt += "log_level = 4 	block_size = 65536 	fallback_file = '";
			scriptTxt += np_AXSMOD_fallback_file_Str + "' 	fallback_directory = '";
			scriptTxt += np_AXSMOD_fallback_directory_Str + "' 	confirm_fallback_deletion = n \";\n";
			scriptTxt += "\n";
			// -- scriptTxt = AXSMOD_SETTINGS;
			*/

			scriptTxt += "SLEEP " + sleepStr + ";\n";
			scriptTxt += "TENACITY " + tenacityStr + ";\n";
			scriptTxt += "SESSIONS " + maxSessionsStr + " " + minSessionsStr + ";\n";
			scriptTxt += "SET RECORD VARTEXT \"|\";\n";
			scriptTxt += "DATEFORM ANSIDATE;\n";
			scriptTxt += "ERRLIMIT 1;\n";
			scriptTxt += ".LOGON " + tdpidStr + "/" + userStr + "," + pwdStr + ";\n";
			scriptTxt += "\n";
			scriptTxt += "DROP TABLE " + stgDbStr + "." + stgTbStr + ";\n";
			scriptTxt += "DROP TABLE " + stgDbStr + "." + stgE1TbStr + ";\n";
			scriptTxt += "DROP TABLE " + stgDbStr + "." + stgE2TbStr + ";\n";
			scriptTxt += "DROP TABLE " + stgDbStr + "." + stgErrorTbStr + ";\n";
			scriptTxt += "DROP TABLE " + stgDbStr + "." + stgUpdTbStr + ";\n";

			//Drop de followers en staging.
			if (this.paceFollowerTabs != null)
			{
				for (int followerNum = 0; followerNum < this.paceFollowerTabs.Length; followerNum++)
				{
					String stgUpdFollowerTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_F" + followerNum.ToString(); // Tabla para seleccionar registros a aplicar del follower.
					scriptTxt += "DROP TABLE " + stgDbStr + "." + stgUpdFollowerTbStr + ";\n";
				}
			}

			if (this.ExceptionTable_Ind.Equals("N", StringComparison.CurrentCultureIgnoreCase)
				&& this.ExceptionTab != null && this.ExceptionTab.paceFollowerTabs != null)
			{
				for (int followerNum = 0; followerNum < this.ExceptionTab.paceFollowerTabs.Length; followerNum++)
				{
					String stgUpdExceptionFollowerTbStr = Regex.Replace(this.ExceptionTab.TbName, "(?<!^)[aeui](?!$)", "") + "_F" + followerNum.ToString(); // Tabla para seleccionar registros a aplicar del follower.
					scriptTxt += "DROP TABLE " + stgDbStr + "." + stgUpdExceptionFollowerTbStr + ";\n";
				}
			}

			scriptTxt += "\n";

			// Tabla principal.
			scriptTxt += "/* ++ Tabla target ++ */\n";
			scriptTxt += "CREATE TABLE " + stgDbStr + "." + stgTbStr + "\n";
			scriptTxt += "(\n";
			scriptTxt += "\n";
			scriptTxt += "/* ++ Datos de archivo ++ */\n";
			scriptTxt += "\n";
			scriptTxt += RowNumColumnName + " INTEGER\n";
			scriptTxt += "\n";
			scriptTxt += "/* -- Datos de archivo -- */\n";
			scriptTxt += "\n";

			String sep = ",";
			foreach (CfDbColumn col in this.DbCols)
			{
				if ((col.isTrkEvent || col.isTrkUser || col.isTrkDt || col.isTrkTm) && !col.isPk)
					continue;

				scriptTxt += "\t" + sep + col.ColName + " " + col.DataType + "\n";
				sep = ",";
			}

			scriptTxt += ")\n";
			scriptTxt += "PRIMARY INDEX\n";
			scriptTxt += "(\n";

			sep = "";
			foreach (CfDbColumn col in this.PkColumns)
			{
				if ((col.isTrkEvent || col.isTrkUser || col.isTrkDt || col.isTrkTm) && !col.isPk)
					continue;

				scriptTxt += "\t" + sep + col.ColName + "\n";
				sep = ",";
			}

			scriptTxt += ")\n";
			scriptTxt += ";\n";
			scriptTxt += "/* -- Tabla target -- */\n";

			// Tabla errores.
			scriptTxt += "/* ++ Tabla target ++ */\n";
			scriptTxt += "CREATE TABLE " + stgDbStr + "." + stgErrorTbStr + "\n";
			scriptTxt += "(\n";
			scriptTxt += RowNumColumnName + " INTEGER\n";
			scriptTxt += ",MsgDt DATE\n";
			scriptTxt += ",MsgTm TIME(6)\n";
			scriptTxt += ",MsgType VARCHAR(50)\n";
			scriptTxt += ",Msg VARCHAR(1000)\n";
			scriptTxt += ",ColumnName VARCHAR(50)\n";
			scriptTxt += ")\n";
			scriptTxt += "PRIMARY INDEX(" + RowNumColumnName + ")\n";
			scriptTxt += ";\n";
			scriptTxt += "\n";

			scriptTxt += "CREATE TABLE " + stgDbStr + "." + stgUpdTbStr + " AS " +  stgDbStr + "." + stgTbStr + " WITH NO DATA;\n";

			scriptTxt += "\n";
			scriptTxt += "BEGIN LOADING\n";
			scriptTxt += "\t" + stgDbStr + "." + stgTbStr + "\n";
			scriptTxt += "ERRORFILES\n";
			scriptTxt += "\t" + stgDbStr + "." + stgE1TbStr + ",\n";
			scriptTxt += "\t" + stgDbStr + "." + stgE2TbStr + "\n";
			scriptTxt += ";\n";
			scriptTxt += "\n";

			// Determino cantidad de columnas que debo cargar. Excluyo las de tracking.
			int colCount = 0;
			foreach (CfDbColumn col in this.DbCols)
			{
				if ((col.isTrkEvent || col.isTrkUser || col.isTrkDt || col.isTrkTm) && !col.isPk)
					continue;
				colCount++;
			}

			DefineColumnList = new String[colCount + 1]; // Agrego una columna mas para el nro de fila.
			DefineLocalNameList = new String[colCount + 1]; // Agrego una columna mas para el nro de fila.
			isNumber = new bool[colCount + 1]; // Agrego una columna mas para el nro de fila.
			isRequiredOnImport = new bool[colCount + 1]; //Agrego una columna mas para el nro de fila. Gerardo 2014-12-03
			isFillOnImport = new bool[colCount + 1]; //Agrego una columna mas para el nro de fila. Gerardo 2015-09-21

			scriptTxt += "DEFINE\n";
			scriptTxt += RowNumColumnName + " (VARCHAR(270))\n";

			int colIdx = 0;
			DefineColumnList[colIdx] = RowNumColumnName;
			DefineLocalNameList[colIdx] = RowNumColumnName;
			isNumber[colIdx] = true;
			isRequiredOnImport[colIdx] = true;
			isFillOnImport[colIdx] = false;

			sep = ",";
			foreach (CfDbColumn col in this.DbCols)
			{
				if ((col.isTrkEvent || col.isTrkUser || col.isTrkDt || col.isTrkTm) && !col.isPk)
					continue;

				scriptTxt += "\t" + sep + col.ColName + "(VARCHAR(270))\n";
				sep = ",";
				colIdx++;
				DefineColumnList[colIdx] = col.ColName;
				DefineLocalNameList[colIdx] = col.ColLocalName;
				isNumber[colIdx] = col.isNumber;
				isRequiredOnImport[colIdx] = col.isRequiredOnImport;
				isFillOnImport[colIdx] = col.isFillOnImport;
			}
			scriptTxt += "FILE=" + inDataFileName + "\n";
			scriptTxt += ";\n";
			scriptTxt += "\n";

			scriptTxt += "INSERT INTO " + stgDbStr + "." + stgTbStr + "\n";
			scriptTxt += "(\n";
			scriptTxt += "__IN_FILE_ROW_NUMBER\n";
			sep = ",";
			foreach (CfDbColumn col in this.DbCols)
			{
				if ((col.isTrkEvent || col.isTrkUser || col.isTrkDt || col.isTrkTm) && !col.isPk)
					continue;

				scriptTxt += "\t" + sep + col.ColName + "\n";
				sep = ",";
			}
			scriptTxt += ")\n";
			scriptTxt += "VALUES\n";
			scriptTxt += "(\n";
			scriptTxt += "\t:" + RowNumColumnName + "\n";
			sep = ",";
			foreach (CfDbColumn col in this.DbCols)
			{
				if ((col.isTrkEvent || col.isTrkUser || col.isTrkDt || col.isTrkTm) && !col.isPk)
					continue;

				scriptTxt += "\t" + sep + ":" + col.ColName + "\n";
				sep = ",";
			}
			scriptTxt += ");\n";
			scriptTxt += "END LOADING;\n";


			scriptTxt += "LOGOFF;\n";

			return scriptTxt;
		}


		/// <summary>
		/// Este metodo sirve para controlar la integridad de registros en tabla temporal.
		/// Genera el comando sql, para verificar una foreign key.
		/// El parametro "relNo" indica que relacion se va a verificar.",
		/// </summary>
		/// <param name="relNo"></param>
		/// <returns></returns>
		public String riStgTableParentCheckQryText(int relNo ) // No se si neceito el nombre de tabla., out String parentTableName )
		{
			String qry_txt = "";
			//int max_param_cnt;
			//int param_cnt = 0;

			int size = this.relParentTabs.Length;
			if (relNo < 0 || this.relParentTabs.Length <= relNo)
			{
				throw new Exception("Indice de tabla padre invalido.");
			}

			CfDbRelation parRel = this.relParentTabs[relNo];
			//max_param_cnt = parRel.RelAtts.Length;

			//parentTableName = parRel.ParentTable.TbName;
			//paramNames = new String[max_param_cnt];

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.


			String keyStr = "";
			String valueStr = "";
			String keySep = "";
			String valueSep = "";

			foreach (CfDbRelAttribute ra in parRel.RelAtts)
			{
				keyStr += keySep ;
				keyStr += ra.ParentColumn.ColLocalName;
				keySep = ", ";

				valueStr += valueSep;
				valueStr += "COALESCE(TRIM( CAST ( chld.";
				valueStr += ra.ChildColumn.ColName;
				valueStr += " AS VARCHAR(1000) ) ),'null')";
				valueSep = " || ', ' || ";
			}

			qry_txt += "LOCKING TABLE " + parRel.ParentTable.DbName + "." + parRel.ParentTable.TbName + " FOR ACCESS\n";
			qry_txt += "INSERT INTO " + stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n";
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ")\n";

			qry_txt += "SELECT\n";
			qry_txt += "chld.__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",CURRENT_DATE\n";
			qry_txt += ",CURRENT_TIME(6)\n";
			qry_txt += ",'Error'\n";
			qry_txt += ",'No existe [" + keyStr + "] = [' || ";
			qry_txt += valueStr + " || '] en "+ parRel.ParentTable.TbLocalName + ".'\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " chld\n";
			qry_txt += "LEFT OUTER JOIN\n";
			qry_txt += parRel.ParentTable.DbName;
			qry_txt += ".";
			qry_txt += parRel.ParentTable.TbName;
			qry_txt += " prnt\n";
			qry_txt += "ON\n";

			String sep = "    ";
			foreach (CfDbRelAttribute ra in parRel.RelAtts)
			{
				qry_txt += sep + "prnt."; 
				qry_txt += ra.ParentColumn.ColName;
				qry_txt += " = chld.";
				qry_txt += ra.ChildColumn.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}
			qry_txt += "WHERE\n";
			qry_txt += "prnt.";
			qry_txt += parRel.RelAtts[0].ParentColumn.ColName;
			qry_txt += " IS NULL\n;";

			return qry_txt;
		}


		public String riStgTableChildCheckQryText(int relNo) // No se si neceito el nombre de tabla., out String parentTableName )
		{
			String qry_txt = "";

			int size = this.relChildTabs.Length;
			if (relNo < 0 || this.relChildTabs.Length <= relNo)
			{
				throw new Exception("Indice de tabla hija invalido.");
			}

			CfDbRelation chldRel = this.relChildTabs[relNo];

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.


			String keyStr = "";
			String valueStr = "";
			String keySep = "";
			String valueSep = "";

			foreach (CfDbRelAttribute ra in chldRel.RelAtts)
			{
				keyStr += keySep;
				keyStr += ra.ChildColumn.ColLocalName;
				keySep = ", ";

				valueStr += valueSep;
				valueStr += "COALESCE(TRIM( CAST ( prnt.";
				valueStr += ra.ParentColumn.ColName;
				valueStr += " AS VARCHAR(1000) ) ),'null')";
				valueSep = " || ', ' || ";
			}

			qry_txt += "LOCKING TABLE " + chldRel.ChildTable.DbName + "." + chldRel.ChildTable.TbName + " FOR ACCESS\n";
			qry_txt += "INSERT INTO " + stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n";
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ")\n";

			qry_txt += "SELECT\n";
			qry_txt += "prnt.__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",CURRENT_DATE\n";
			qry_txt += ",CURRENT_TIME(6)\n";
			qry_txt += ",'Error'\n";
			qry_txt += ",'Existe [" + keyStr + "] = [' || ";
			qry_txt += valueStr + " || '] en " + chldRel.ChildTable.TbLocalName + ".'\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " prnt\n";
			qry_txt += "WHERE\n";
			qry_txt += "EXISTS (\n";
			qry_txt += "SELECT 1\n";
			qry_txt += "FROM\n";
			qry_txt += "(\n";
			qry_txt += "SELECT\n";
			String sep = "    ";
			foreach (CfDbRelAttribute ra in chldRel.RelAtts)
			{
				qry_txt += sep + ra.ChildColumn.ColName;
				qry_txt += "\n";
				sep = ",";
			}
			qry_txt += sep + "COUNT(*) CNT\n";
			qry_txt += "FROM\n";
			qry_txt += chldRel.ChildTable.DbName;
			qry_txt += ".";
			qry_txt += chldRel.ChildTable.TbName;
			qry_txt += "\n";
			qry_txt += "GROUP BY\n";
			sep = "    ";
			foreach (CfDbRelAttribute ra in chldRel.RelAtts)
			{
				qry_txt += sep + ra.ChildColumn.ColName;
				qry_txt += "\n";
				sep = ",";
			}
			qry_txt += ") chldt\n";
			qry_txt += "WHERE\n";

			sep = "    ";
			foreach (CfDbRelAttribute ra in chldRel.RelAtts)
			{
				qry_txt += sep + "chldt.";
				qry_txt += ra.ChildColumn.ColName;
				qry_txt += " = prnt.";
				qry_txt += ra.ParentColumn.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}
			qry_txt += ")\n";

			return qry_txt;
		}

		public String checkStgTableStartDateQryText(String currentDate)
		{
			String qry_txt = "";

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.

			qry_txt += "INSERT INTO " + stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n"; 
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ")\n";

			qry_txt += "SELECT\n";
			qry_txt += "t.__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",CURRENT_DATE\n";
			qry_txt += ",CURRENT_TIME(6)\n";
			qry_txt += ",CASE\n";
			qry_txt += " WHEN " + this.StartDt_Column.ColName + "< DATE '" + currentDate + "' THEN 'Warn'\n";
			qry_txt += " WHEN " + this.StartDt_Column.ColName + "> DATE '" + currentDate + "' THEN 'Error'\n";
			qry_txt += "END\n";
			qry_txt += ",CASE\n";
			qry_txt += " WHEN " + this.StartDt_Column.ColName + "< DATE '" + currentDate + "' THEN 'Fecha de inicio de vigencia efectiva = " + currentDate + ".'\n";
			qry_txt += " WHEN " + this.StartDt_Column.ColName + "> DATE '" + currentDate + "' THEN 'La fecha de inicio de vigencia efectiva es mayor que " + currentDate + ".'\n";
			qry_txt += "END\n";
			//qry_txt += ",'Warn'\n";
			//qry_txt += ",'Fecha de inicio de vigencia efectiva = " + currentDate + ".'\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " t\n";
			qry_txt += "WHERE\n";

			//Si es tabla de excepcion y la fecha es menor que la de hoy, va la de hoy.(Acepta corriente y futuras)
			//Si la tabla no es de excepcion, y la fecha no es la de hoy, va la de hoy.(Acepta solo corriente).
			//Si la tabla es indiferente 'I', toma lo que entra.
			if (this.ExceptionTable_Ind.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
			{
				qry_txt += this.StartDt_Column.ColName + " < DATE '" + currentDate + "'\n";
				//startDateValue.CompareTo(currentDate) < 0
			}
			if(this.ExceptionTable_Ind.Equals("N", StringComparison.CurrentCultureIgnoreCase) )
			{
				qry_txt += this.StartDt_Column.ColName + " <> DATE '" + currentDate + "'\n";
			}
			return qry_txt;
		}

		public String updateStgTableStartDateQryText(String currentDate)
		{
			String qry_txt = "";
			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.

			qry_txt += "UPDATE t\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " t,\n";
			qry_txt += stgDbStr + "." + stgErrorTbStr + " e\n";

			qry_txt += "SET\n";
			qry_txt += this.StartDt_Column.ColName + " = DATE '" + currentDate + "'\n";

			qry_txt += "WHERE\n";
			qry_txt += "e.Msg LIKE 'Fecha de inicio de vigencia efectiva%'\n";
			qry_txt += "AND e.__IN_FILE_ROW_NUMBER = t.__IN_FILE_ROW_NUMBER\n";

			return qry_txt;
		}

		public String checkStgTableEndDateQryText()
		{
			String qry_txt = "";

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.

			qry_txt += "INSERT INTO " + stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n";
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ")\n";

			qry_txt += "SELECT\n";
			qry_txt += "t.__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",CURRENT_DATE\n";
			qry_txt += ",CURRENT_TIME(6)\n";
			qry_txt += ",'Error'\n";
			qry_txt += ",'La fecha de fin de vigencia es menor que la fecha de inicio.'\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " t\n";
			qry_txt += "WHERE\n";
			qry_txt += this.EndDt_Column.ColName + " < " + this.StartDt_Column.ColName + "\n";

			return qry_txt;
		}



		public String checkStgTableFollowingTabQryTxt()
		{
			String qry_txt = "";

			if (this.paceFollowingTab == null)
				return qry_txt;

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.

			qry_txt += "LOCKING TABLE " + this.paceFollowingTab.DbName + "." + this.paceFollowingTab.TbName + " FOR ACCESS\n";
			qry_txt += "INSERT INTO " + stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n";
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ")\n";
			qry_txt += "SELECT\n";
			qry_txt += "exc.__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",CURRENT_DATE\n";
			qry_txt += ",CURRENT_TIME(6)\n";
			qry_txt += ",'Error'\n";
			qry_txt += ",'La vigencia de la frecuencia no se corresponde con la vigencia de la tabla " + this.paceFollowingTab.TbLocalName + ".'\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " exc\n";
			qry_txt += "WHERE\n";
			qry_txt += "NOT EXISTS(\n";
			qry_txt += "SELECT 1\n";
			qry_txt += "FROM\n";
			qry_txt += this.paceFollowingTab.DbName + "." + this.paceFollowingTab.TbName + " fwng\n";
			qry_txt += "WHERE\n";

			String sep = "";
			foreach (CfDbColumn col in this.paceFollowingTab.PkColumns)
			{
				qry_txt += sep + "fwng." + col.ColName + " = exc." + col.ColName + "\n";
				sep = "AND ";
			}

			qry_txt += sep + "fwng." + this.paceFollowingTab.StartDt_Column.ColName + " = exc." + this.StartDt_Column.ColName + "\n";
			qry_txt += sep + "fwng." + this.paceFollowingTab.EndDt_Column.ColName + " = exc." + this.EndDt_Column.ColName + "\n";
			qry_txt += ")\n";

			return qry_txt;
		}


		public String checkStgTableNormalTabQryTxt()
		{
			String qry_txt = "";

			if (this.NormalTab == null)
				return qry_txt;                   

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.

			qry_txt += "LOCKING TABLE " + this.NormalTab.DbName + "." + this.NormalTab.TbName + " FOR ACCESS\n";
			qry_txt += "INSERT INTO " + stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n";
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ")\n";
			qry_txt += "SELECT\n";
			qry_txt += "exc.__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",CURRENT_DATE\n";
			qry_txt += ",CURRENT_TIME(6)\n";
			qry_txt += ",'Error'\n";
			qry_txt += ",'No existe relacion que precede a la excepcion.'\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " exc\n";
			qry_txt += "LEFT OUTER JOIN\n";
			qry_txt += this.NormalTab.DbName + "." + this.NormalTab.TbName + " norm\n";
			qry_txt += "ON\n";

			String sep = "";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep + "norm." + col.ColName + " = exc." + col.ColName + "\n";
				sep = "AND ";
			}

			qry_txt += sep + "CURRENT_DATE BETWEEN norm." + this.NormalTab.StartDt_Column.ColName + " AND norm." + this.NormalTab.EndDt_Column.ColName + "\n";
			qry_txt += "WHERE\n";
			qry_txt += "norm." + this.PkColumns[0].ColName + " IS NULL";

			return qry_txt;
		}

		public String checkStgNonExistingRows()
		{
			String qry_txt = "";
			String sep = "";
			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.

			qry_txt += "LOCKING TABLE " + this.DbName + "." + this.TbName + " FOR ACCESS\n";
			qry_txt += "INSERT INTO " + stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n";
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ")\n";
			qry_txt += "SELECT\n";
			qry_txt += "t.__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",CURRENT_DATE\n";
			qry_txt += ",CURRENT_TIME(6)\n";
			qry_txt += ",'Error'\n";
			qry_txt += ",'La relacion que quiere borrar no existe en la tabla " + this.TbLocalName + ".'";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " t\n";
			qry_txt += "LEFT OUTER JOIN\n";
			qry_txt += "(\n";

			/*
			Caso 1
			El nuevo registro esta en medio del  periodo de un unico registro.
			El registro existente debe partirse en un primer periodo precedente al 
			nuevo ( nro 1), y otro posterior al nuevo ( nro 3). El nuevo, sera el 2 en 
			la nueva serie.
			*/

			qry_txt += "SELECT --Caso 1\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in this.DbCols)
			{
				if (!(col.isTrkDt || col.isTrkEvent || col.isTrkTm || col.isTrkUser))
				{
					qry_txt += sep;
					qry_txt += "n.";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
				}
			}

			//Clausula from
			qry_txt += "FROM\n";
			qry_txt += stgDbStr;
			qry_txt += ".";
			qry_txt += stgTbStr;
			qry_txt += " AS n\n";
			qry_txt += "INNER JOIN\n";
			qry_txt += "(SELECT\n";

			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Luego colummnas fecha si es historia
			if (this.isHist)
			{
				// StartDt
				qry_txt += sep + this.StartDt_Column.ColName + "\n";
				sep = ",";

				// EndDt
				qry_txt += sep + this.EndDt_Column.ColName + "\n";
				sep = ",";
			}

			qry_txt += "FROM\n";
			qry_txt += this.DbName + "." + this.TbName + "\n";
			qry_txt += ")o\n";
			qry_txt += "ON\n";

			//Armo join con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "= n.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}

			//Luego colummnas fecha si es historia
			if (this.isHist)
			{
				// Condiciones de fecha.
				// AND	o.T_Start <= n.T_Start AND n.T_End <= o.T_End
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " <= ";
				qry_txt += "n.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += "\n";
				sep = "AND ";

				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += this.EndDt_Column.ColName;
				qry_txt += " <= ";
				qry_txt += "o.";
				qry_txt += this.EndDt_Column.ColName;
				qry_txt += "\n";
			}

			qry_txt += "UNION\n";

			/*
				Caso 2
				El periodo del nuevo registro comienza antes que el periodo del registro existente,
				y finaliza antes que el fin del  registro existente.
				El registro existente debe actualizarse para que cominenze luego del registro nuevo.
			*/

			qry_txt += "SELECT --Caso 2\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in this.DbCols)
			{
				if (!(col.isTrkDt || col.isTrkEvent || col.isTrkTm || col.isTrkUser))
				{
					qry_txt += sep;
					qry_txt += "n.";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
				}
			}

			//Clausula from
			qry_txt += "FROM\n";
			qry_txt += stgDbStr;
			qry_txt += ".";
			qry_txt += stgTbStr;
			qry_txt += " AS n\n";
			qry_txt += "INNER JOIN\n";
			qry_txt += "(SELECT\n";

			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			if (this.isHist)
			{
				//Luego colummnas fecha
				// StartDt
				qry_txt += sep + this.StartDt_Column.ColName + "\n";
				sep = ",";

				// EndDt
				qry_txt += sep + this.EndDt_Column.ColName + "\n";
				sep = ",";
			}

			qry_txt += "FROM\n";
			qry_txt += this.DbName + "." + this.TbName + "\n";
			qry_txt += ")o\n";
			qry_txt += "ON\n";

			//Armo join con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "= n.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}

			if (this.isHist)
			{
				// Condiciones de fecha.
				//AND	n.T_Start < o.T_Start
				//AND   o.T_Start <= n.T_End
				//AND	n.T_End <= o.T_End
				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " < ";
				qry_txt += "o.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += "\n";
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " <= ";
				qry_txt += "n.";
				qry_txt += this.EndDt_Column.ColName;
				qry_txt += "\n";
				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += this.EndDt_Column.ColName;
				qry_txt += " <= ";
				qry_txt += "o.";
				qry_txt += this.EndDt_Column.ColName;
				qry_txt += "\n";
			}

			qry_txt += "UNION\n";

			/*
			Caso 3
			El periodo del nuevo registro comienza despues que el periodo del registro existente,
			y finaliza despues que el fin del  registro existente.

			El registro existente debe actualizarse para que finalice antes que el registro nuevo.
			*/

			qry_txt += "SELECT --Caso 3\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in this.DbCols)
			{
				if (!(col.isTrkDt || col.isTrkEvent || col.isTrkTm || col.isTrkUser))
				{
					qry_txt += sep;
					qry_txt += "n.";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
				}
			}

			//Clausula from
			qry_txt += "FROM\n";
			qry_txt += stgDbStr;
			qry_txt += ".";
			qry_txt += stgTbStr;
			qry_txt += " AS n\n";
			qry_txt += "INNER JOIN\n";
			qry_txt += "(SELECT\n";

			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			if (this.isHist)
			{
				//Luego colummnas fecha
				// StartDt
				qry_txt += sep + this.StartDt_Column.ColName + "\n";
				sep = ",";

				// EndDt
				qry_txt += sep + this.EndDt_Column.ColName + "\n";
				sep = ",";
			}

			qry_txt += "FROM\n";
			qry_txt += this.DbName + "." + this.TbName + "\n";
			qry_txt += ")o\n";
			qry_txt += "ON\n";

			//Armo join con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "= n.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}

			if (this.isHist)
			{
				// Condiciones de fecha.
				//AND	o.T_Start < n.T_Start
				//AND   n.T_Start < o.T_End
				//AND	o.T_End < n.T_End
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " < ";
				qry_txt += "n.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += "\n";
				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " <= ";
				qry_txt += "o.";
				qry_txt += this.EndDt_Column.ColName;
				qry_txt += "\n";
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += this.EndDt_Column.ColName;
				qry_txt += " < ";
				qry_txt += "n.";
				qry_txt += this.EndDt_Column.ColName;
				qry_txt += "\n";
			}

			qry_txt += ")tmp1\n";
			qry_txt += "ON\n";

			//Armo join con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "tmp1.";
				qry_txt += col.ColName;
				qry_txt += "= t.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}

			if (this.isHist)
			{
				qry_txt += sep;
				qry_txt += "tmp1.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " = ";
				qry_txt += "t.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += "\n";
			}

			qry_txt += "WHERE\n";
			qry_txt += "tmp1." + this.PkColumns[0].ColName + " IS NULL\n"; 

			///////////////////
			return qry_txt;
		}

		public String collectStTableStatsQryTxt()
		{
			String qry_txt = "";
			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String sep = "";
			
			String upicolumn = "COLUMN ("; 

			qry_txt += "COLLECT STATISTICS";

			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep;
				upicolumn += sep;
				qry_txt += "\n";
				qry_txt += "COLUMN (" + col.ColName + ")";
				upicolumn += col.ColName;
				sep = ",";
			}

			qry_txt += sep;
			qry_txt += "\n";
			qry_txt += upicolumn;
			qry_txt += ")\n";
			qry_txt += "ON " + stgDbStr + "." + stgTbStr + ";";

			return qry_txt;
		}

		public String checkStTableDefaultValueQryTxt(CfDbColumn col)
		{
			String qry_txt = "";
			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.

			qry_txt += "INSERT INTO " + stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n";
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ")\n";

			qry_txt += "SELECT\n";
			qry_txt += "t.__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",CURRENT_DATE\n";
			qry_txt += ",CURRENT_TIME(6)\n";
			qry_txt += ",'Warn'\n";
			qry_txt += ",'" + col.ColLocalName + " no informado. Valor por defecto=" + col.DefaultValue + ".'";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " t\n";
			
			if (!(col.isEndDt && this.ExceptionTable_Ind.Equals("N", StringComparison.CurrentCultureIgnoreCase)))
			{
				qry_txt += "WHERE\n";
				qry_txt += col.ColName + " IS NULL\n";
			}

			return qry_txt;
		}

		public String updateStTableDefaultValueQryTxt(CfDbColumn col)
		{
			String qry_txt = "";
			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;

			if (col.isEndDt && this.ExceptionTable_Ind.Equals("N",StringComparison.CurrentCultureIgnoreCase))
			{
				qry_txt += "UPDATE " + stgDbStr + "." + stgTbStr + " t\n";
				qry_txt += "SET\n";
				qry_txt += col.ColName + "= '" + col.DefaultValue + "'\n";
				//qry_txt += "WHERE\n";
				//qry_txt += col.ColName + " IS NULL\n";
			}
			else if (col.DataType.Equals("DATE", StringComparison.CurrentCultureIgnoreCase) &&
				col.DefaultValue.Equals("CURRENT_DATE", StringComparison.CurrentCultureIgnoreCase)
			)
			{
				qry_txt += "UPDATE " + stgDbStr + "." + stgTbStr + " t\n";
				qry_txt += "SET\n";
				qry_txt += col.ColName + "= " + col.DefaultValue + "\n";
				qry_txt += "WHERE\n";
				qry_txt += col.ColName + " IS NULL\n";
			}
			else
			{
				qry_txt += "UPDATE " + stgDbStr + "." + stgTbStr + " t\n";
				qry_txt += "SET\n";
				qry_txt += col.ColName + "= '" + col.DefaultValue + "'\n";
				qry_txt += "WHERE\n";
				qry_txt += col.ColName + " IS NULL\n";
			}

			return qry_txt;
		}

		public String checkStgTableValidValueQryTxt(CfDbColumn col)
		{
			String qry_txt = "";
			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.

			qry_txt += "INSERT INTO " + stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n";
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ")\n";

			qry_txt += "SELECT\n";
			qry_txt += "t.__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",CURRENT_DATE\n";
			qry_txt += ",CURRENT_TIME(6)\n";
			qry_txt += ",'Error'\n";

			qry_txt += ",COALESCE(TRIM(CAST(" + col.ColName + " AS VARCHAR(500))),'null') || ' es un valor invalido para " + col.ColLocalName + ".'\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " t\n";
			qry_txt += "WHERE\n";

			if (col.isReq && (col.CheckTxt == null || col.CheckTxt.Equals("")))
			{
				qry_txt += col.ColName + " IS NULL\n";
			}
			else if (!col.isReq && col.CheckTxt != null && !col.CheckTxt.Equals(""))
			{
				qry_txt += col.ColName + " IS NOT NULL\n";
				qry_txt += "AND NOT (" + col.CheckTxt + ")\n";
			}
			else
			{
				qry_txt += col.ColName + " IS NULL\n";
				qry_txt += "OR NOT (" + col.CheckTxt + ")\n";
			}
			return qry_txt;
		}

		public String checkStgTableRejectedOverlap()
		{
			String qry_txt = "";

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.

			qry_txt += "LOCKING TABLE " + this.DbName + "." + this.TbName + " FOR ACCESS\n";
			qry_txt += "INSERT INTO " + stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n";
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ")\n";

			qry_txt += "SELECT\n";
			qry_txt += "rej.__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",CURRENT_DATE\n";
			qry_txt += ",CURRENT_TIME(6)\n";
			qry_txt += ",'Error'\n";
			qry_txt += ",'Registro Existente en " + this.TbLocalName + ", no se puede cargar.'\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " rej\n";
			qry_txt += "LEFT OUTER JOIN\n";
			qry_txt += this.DbName + "." + this.TbName + " orig\n";
			qry_txt += "ON\n";
			String sep = "";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += sep + "orig." + col.ColName + " = rej." + col.ColName + "\n";
				sep = "AND ";
			}
			qry_txt += sep + "CURRENT_DATE BETWEEN orig." + this.StartDt_Column.ColName + " AND orig." + this.EndDt_Column.ColName + "\n";
			qry_txt += "WHERE\n";
			qry_txt += "orig." + this.PkColumns[0].ColName + " IS NOT NULL";

			return qry_txt;
		}

		/// <summary>
		/// Limpia tabla donde se insertaran registros de novedad
		/// validados para ser aplicados a las tablas destino.
		/// </summary>
		/// <returns></returns>
		public String cleanUpdateStgTableQryTxt()
		{
			String qry_txt = "";

			String stgDbStr = this.DbName + "_STG";
			String stgUpdTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_U"; // Tabla para seleccionar registros a aplicar.

			qry_txt += "DELETE " + stgDbStr + "." + stgUpdTbStr + ";\n";
			return qry_txt;
		}
		/// <summary>
		/// Este metodo genera query para identificar filas a aplicar.
		/// Se deben filtrar todas las erroneas.
		/// Se presume que pueden informarse varias filas para la misma clave
		/// principal ( sin las fechas de vigencia en caso de tabla historica ),
		/// por lo que hay que ejecutarlo varias veces, hasta que no queden mas
		/// filas por aplicar. En cada ciclo se aplicara un registro por clave
		/// principal.
		/// </summary>
		/// <returns></returns>
		public String fillUpdateStgTableQryTxt(int cycleNum)
		{
			String qry_txt = "";

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.
			String stgUpdTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_U"; // Tabla para seleccionar registros a aplicar.


			qry_txt += "INSERT INTO " + stgDbStr + "." + stgUpdTbStr + "\n";
			qry_txt += "(\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			foreach (CfDbColumn col in this.DbCols)
			{
				if ((col.isTrkEvent || col.isTrkUser || col.isTrkDt || col.isTrkTm) && !col.isPk)
					continue;

				qry_txt += "\t," + col.ColName + "\n";
			}
			qry_txt += ")\n";
			qry_txt += "SELECT\n";
			qry_txt += "u.__IN_FILE_ROW_NUMBER\n";
			foreach (CfDbColumn col in this.DbCols)
			{
				if ((col.isTrkEvent || col.isTrkUser || col.isTrkDt || col.isTrkTm) && !col.isPk)
					continue;

				qry_txt += "\t," + "u." + col.ColName + "\n";
			}
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + " u\n";
			qry_txt += "WHERE\n";
			qry_txt += "u.__IN_FILE_ROW_NUMBER NOT IN (\n";
			qry_txt += "SELECT __IN_FILE_ROW_NUMBER FROM " + stgDbStr + "." + stgErrorTbStr + " WHERE MsgType = 'Error'\n";
			qry_txt += ")\n";
			qry_txt += "QUALIFY ROW_NUMBER() OVER (PARTITION BY \n";
			String sep = "";
			foreach (CfDbColumn col in this.PkColumns)
			{
				qry_txt += "\t" + sep + "u." + col.ColName + "\n";
				sep = ",";
			}
			if (isHist)
			{
				qry_txt += "ORDER BY u.__IN_FILE_ROW_NUMBER ASC, u." + this.StartDt_Column.ColName + " ASC\n";
			}
			else
			{
				qry_txt += "ORDER BY u.__IN_FILE_ROW_NUMBER ASC\n";
			}
			qry_txt += ") = " + cycleNum.ToString() + "\n";

			return qry_txt;
		}

		/// <summary>
		/// Este metodo aplica las novedades a la tabla del modelo.
		/// Genera un comando sql similar al que inserta una fila,
		/// pero en vez de recibir datos por parametros, los toma de
		/// otra tabla.
		/// </summary>
		/// <returns></returns>
		public String mergeUpdateStgTableQryTxt( String RepEvent_id, String RepUser_id)
		{
			//return merge_delete_HistStgTableQryText(true, RepEvent_id, RepUser_id);

			//CfDbTable tgtTbFmt;

			String srcDbName = this.DbName + "_STG";
			String srcTbName = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_U"; // Tabla para seleccionar registros a aplicar.

			if (this.isHist)
			{
				return merge_delete_Follower_Normal_HistStgTableQryText(
										this, this.DbName, this.TbName,
										this, srcDbName, srcTbName,           //El formato de origen es igual al destino. Gerardo
										true, RepEvent_id, RepUser_id);
			}
			else
			{
				return merge_Normal_StgTableQryText(
										this, this.DbName, this.TbName,
										this, srcDbName, srcTbName,
										RepEvent_id, RepUser_id);
			}
		}

		public String closeUpdateStgTableQryTxt(String RepEvent_id, String RepUser_id)
		{
			//return merge_delete_HistStgTableQryText(true, RepEvent_id, RepUser_id);

			//CfDbTable tgtTbFmt;

			String srcDbName = this.DbName + "_STG";
			String srcTbName = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_U"; // Tabla para seleccionar registros a aplicar.

				return close_Follower_Normal_HistStgTableQryText(
										this, this.DbName, this.TbName,
										this, srcDbName, srcTbName,           //El formato de origen es igual al destino. Gerardo
										true, RepEvent_id, RepUser_id);
		}

		public String deleteUpdateStgTableQryTxt(String RepEvent_id, String RepUser_id)
		{
			//return merge_delete_HistStgTableQryText(false, RepEvent_id, RepUser_id);

			//CfDbTable tgtTbFmt;

			String srcDbName = this.DbName + "_STG";
			String srcTbName = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_U"; // Tabla para seleccionar registros a aplicar.

			if (this.isHist)
			{
				return merge_delete_Follower_Normal_HistStgTableQryText(
									this, this.DbName, this.TbName,
									this, srcDbName, srcTbName,           //El formato de origen es igual al destino. Gerardo
									false, RepEvent_id, RepUser_id);
			}
			else
			{
				return delete_Normal_StgTableQryText(
										this, this.DbName, this.TbName,
										this, srcDbName, srcTbName);
			}
		}

		public String countDeleteRowsTableUQryTxt()
		{
			String qry_txt = "";
			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_U"; // Tabla para seleccionar registros a aplicar.

			qry_txt += "SELECT *\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgTbStr + "\n";

			return qry_txt;
		}

		public String msgStgQryTxt()
		{
			String qry_txt = "";

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = this.TbName;
			String stgErrorTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_E3"; // Esto tendria que ponerlo en CfDbTable y generarlo desde erwin.

			qry_txt += "SELECT\n";
			qry_txt += "__IN_FILE_ROW_NUMBER\n";
			qry_txt += ",MsgDt\n";
			qry_txt += ",MsgTm\n";
			qry_txt += ",MsgType\n";
			qry_txt += ",Msg\n";
			qry_txt += ",ColumnName\n";
			qry_txt += "FROM\n";
			qry_txt += stgDbStr + "." + stgErrorTbStr + "\n";
			qry_txt += "ORDER BY\n";
			qry_txt += "__IN_FILE_ROW_NUMBER,\n";
			qry_txt += "MsgDt,\n";
			qry_txt += "MsgTm\n";

			return qry_txt;
		}

		public String createFollowerStgTableQryText(int followerNum)
		{
			//Si no es un indice de follower valido.
			if (!(0 <= followerNum && followerNum <= this.paceFollowerTabs.Length))
			{
				return "";
			}

			CfDbTable followerTab = this.paceFollowerTabs[followerNum];

			return createUpdateFollowerStgTableQryText(followerNum, this, followerTab);
		}

		public String createExceptionFollowerStgTableQryText(int followerNum)
		{
			if (this.ExceptionTab == null)
				return "";

			if (this.ExceptionTab.paceFollowerTabs == null)
				return "";

			//Si no es un indice de follower valido.
			if (!(0 <= followerNum && followerNum <= this.ExceptionTab.paceFollowerTabs.Length))
			{
				return "";
			}

			CfDbTable followerTab = this.ExceptionTab.paceFollowerTabs[followerNum];

			return createUpdateFollowerStgTableQryText(followerNum, this.ExceptionTab, followerTab);
		}

		// Followers desde staging.
		public String createUpdateFollowerStgTableQryText(int followerNum, CfDbTable tabSrcInfo, CfDbTable followerTab)
		{
			String qry_txt = "";
			String sep = "";

			String stgDbStr = this.DbName + "_STG";
			String stgTbStr = tabSrcInfo.TbName;
			String stgUpdFollowerTbStr = Regex.Replace(stgTbStr, "(?<!^)[aeui](?!$)", "") + "_F" + followerNum.ToString(); // Tabla para seleccionar registros a aplicar del follower.

			qry_txt += "CREATE TABLE " + stgDbStr + "." + stgUpdFollowerTbStr + "\n";
			qry_txt += "(\n";

			sep = "";
			foreach (CfDbColumn col in followerTab.DbCols)
			{
				if (col.isTrkEvent || col.isTrkUser || col.isTrkDt || col.isTrkTm)
					continue;

				qry_txt += "\t" + sep + col.ColName + " " + col.DataType + "\n";
				sep = ",";
			}
			qry_txt += ")\n";
			qry_txt += "PRIMARY INDEX\n";
			qry_txt += "(\n";

			sep = "";
			foreach (CfDbColumn col in followerTab.PkColumns)
			{
				qry_txt += "\t" + sep + col.ColName + "\n";
				sep = ",";
			}
			qry_txt += ")\n";
			qry_txt += ";\n";

			return qry_txt;
		}

		public String cleanFollowerStgTableQryText(int followerNum)
		{
			//Si no es un indice de follower valido.
			if (!(0 <= followerNum && followerNum <= this.paceFollowerTabs.Length))
			{
				return "";
			}

			CfDbTable followerTab = this.paceFollowerTabs[followerNum];

			return cleanUpdateFollowerStgTableQryText(followerNum, this, followerTab);
		}

		public String cleanExceptionFollowerStgTableQryText(int followerNum)
		{
			if (this.ExceptionTab == null)
				return "";

			if (this.ExceptionTab.paceFollowerTabs == null)
				return "";

			//Si no es un indice de follower valido.
			if (!(0 <= followerNum && followerNum <= this.ExceptionTab.paceFollowerTabs.Length))
			{
				return "";
			}

			CfDbTable followerTab = this.ExceptionTab.paceFollowerTabs[followerNum];

			return cleanUpdateFollowerStgTableQryText(followerNum, this.ExceptionTab, followerTab);
		}

		public String cleanUpdateFollowerStgTableQryText(int followerNum, CfDbTable tabSrcInfo, CfDbTable followerTab)
		{
			String qry_txt = "";

			String stgDbStr = this.DbName + "_STG";
			String stgTbName = tabSrcInfo.TbName;
			String stgUpdFollowerTbStr = Regex.Replace(stgTbName, "(?<!^)[aeui](?!$)", "") + "_F" + followerNum.ToString(); // Tabla para seleccionar registros a aplicar del follower.

			qry_txt += "DELETE " + stgDbStr + "." + stgUpdFollowerTbStr + ";\n";
			return qry_txt;
		}

		public String fillFollowerStgTableQryText(int followerNum)
		{
			//Si no es un indice de follower valido.
			if (!(0 <= followerNum && followerNum <= this.paceFollowerTabs.Length))
			{
				return "";
			}

			CfDbTable followerTab = this.paceFollowerTabs[followerNum];

			return fillUpdateFollowerStgTableQryText(followerNum, this, followerTab);
		}

		public String fillExceptionFollowerStgTableQryText(int followerNum)
		{
			if (this.ExceptionTab == null)
				return "";

			if (this.ExceptionTab.paceFollowerTabs == null)
				return "";

			//Si no es un indice de follower valido.
			if (!(0 <= followerNum && followerNum <= this.ExceptionTab.paceFollowerTabs.Length))
			{
				return "";
			}

			CfDbTable followerTab = this.ExceptionTab.paceFollowerTabs[followerNum];

			return fillUpdateFollowerStgTableQryText(followerNum, this.ExceptionTab, followerTab);
		}

		public String fillUpdateFollowerStgTableQryText(int followerNum, CfDbTable tabSrcInfo, CfDbTable followerTab)
		{
			String qry_txt = "";
			String sep = "";

			String stgDbStr = this.DbName + "_STG";
			String stgUpdTbStr = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_U"; // Tabla para seleccionar registros a aplicar.
			String stgUpdFollowerTbStr = Regex.Replace(tabSrcInfo.TbName, "(?<!^)[aeui](?!$)", "") + "_F" + followerNum.ToString(); // Tabla para seleccionar registros a aplicar del follower.

			qry_txt += "INSERT INTO " + stgDbStr + "." + stgUpdFollowerTbStr + "\n";
			qry_txt += "(\n";

			//Columnas clave de follower.
			sep = "";
			foreach (CfDbColumn c in followerTab.PkColumns)
			{
				qry_txt += sep + c.ColName + "\n";
				sep = ",";
			}
			// StartDate. Van las fechas de la principal.
			qry_txt += sep + followerTab.StartDt_Column.ColName + "\n";

			// EndDate. Van las fechas de la principal.
			qry_txt += sep + followerTab.EndDt_Column.ColName + "\n";

			//Columnas no clave de follower.
			foreach (CfDbColumn c in followerTab.NoPkColumns)
			{
				if (c.isTrkEvent || c.isTrkUser || c.isTrkDt || c.isTrkTm)
					continue;

				qry_txt += sep + c.ColName + "\n";
				sep = ",";
			}

			qry_txt += ")\n";
			qry_txt += "SELECT\n";

			//Columnas clave de follower.
			sep = "";
		   foreach (CfDbColumn c in followerTab.PkColumns)
			{
				qry_txt += sep + "f." +  c.ColName + "\n";
				sep = ",";
			}

			// StartDate. Van las fechas de la principal.
		   qry_txt += sep + "p." + this.StartDt_Column.ColName + " " + followerTab.StartDt_Column.ColName + "\n";

			// EndDate. Van las fechas de la principal.
		   qry_txt += sep + "p." + this.EndDt_Column.ColName + " " + followerTab.EndDt_Column.ColName + "\n";

		   //Columnas no clave de follower.
		   foreach (CfDbColumn c in followerTab.NoPkColumns)
		   {
			   if (c.isTrkEvent || c.isTrkUser || c.isTrkDt || c.isTrkTm)
				   continue;

			   qry_txt += sep + "f." + c.ColName + "\n";
			   sep = ",";
		   }

			qry_txt += "FROM\n";
			qry_txt += followerTab.DbName + "." + followerTab.TbName + " f\n";
			qry_txt += "INNER JOIN\n";
			qry_txt += stgDbStr + "." + stgUpdTbStr + " p\n";
			qry_txt += "ON\n";

			//Armo condicion con claves en comun.
			String cond_sep = "    ";
			foreach (CfDbColumn k in followerTab.PkColumns)
			{
				//CfDbColumn km = this.PkColumns.First<CfDbColumn>( c => c.ColName.Equals(k.ColName,StringComparison.CurrentCultureIgnoreCase) );

				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < this.PkColumns.Length; idx++)
				{
					if (k.ColName.Equals(this.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = this.PkColumns[idx];
						break;
					}
				}

				if ( km != null)
				{
					qry_txt += cond_sep + "p." + km.ColName + " = f." + k.ColName + "\n";
					cond_sep = "AND ";
				}
			}

			// Y que el periodo se superponga con el solicitado
			qry_txt += cond_sep;
			qry_txt += "(f." + followerTab.StartDt_Column.ColName + ", f." + followerTab.EndDt_Column.ColName + ")\n";
			qry_txt += "OVERLAPS\n";
			qry_txt += "(p." + this.StartDt_Column.ColName + ", p." + this.EndDt_Column.ColName + ")\n";

			qry_txt += "QUALIFY\n";
			qry_txt += "    ROW_NUMBER() OVER (\n";
			qry_txt += "        PARTITION BY ";

			String lsep = "";
			foreach (CfDbColumn k in followerTab.PkColumns)
			{
				qry_txt += lsep + "f." + k.ColName;
				lsep = ", ";
			}
			qry_txt += "\n";
			qry_txt += "        ORDER BY ";
			qry_txt += "p." + this.StartDt_Column.ColName;
			qry_txt += "\n";
			qry_txt += "    ) = 1";

			return qry_txt;
		}


		public String mergeFollowerStgTableQryTxt(int followerNum, String RepEvent_id, String RepUser_id)
		{
			//Si no es un indice de follower valido.
			if (!(0 <= followerNum && followerNum <= this.paceFollowerTabs.Length))
			{
				return "";
			}

			CfDbTable tgtTbFmt = this.paceFollowerTabs[followerNum];

			String srcDbName = this.DbName + "_STG";
			String srcTbName = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_F" + followerNum.ToString(); // Tabla para seleccionar registros a aplicar del follower.

			return merge_delete_Follower_Normal_HistStgTableQryText(
									tgtTbFmt, tgtTbFmt.DbName, tgtTbFmt.TbName, 
									tgtTbFmt, srcDbName, srcTbName,           //El formato de origen es igual al destino. Gerardo
									true, RepEvent_id, RepUser_id);
		}

		public String deleteFollowerStgTableQryTxt(int followerNum, String RepEvent_id, String RepUser_id)
		{
			//Si no es un indice de follower valido.
			if (!(0 <= followerNum && followerNum <= this.paceFollowerTabs.Length))
			{
				return "";
			}

			CfDbTable tgtTbFmt = this.paceFollowerTabs[followerNum];

			String srcDbName = this.DbName + "_STG";
			String srcTbName = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_F" + followerNum.ToString(); // Tabla para seleccionar registros a aplicar del follower.

			return merge_delete_Follower_Normal_HistStgTableQryText(
									tgtTbFmt, tgtTbFmt.DbName, tgtTbFmt.TbName, 
									tgtTbFmt, srcDbName, srcTbName,           //El formato de origen es igual al destino. Gerardo
									false, RepEvent_id, RepUser_id);
		}

		/// <summary>
		/// Genera query para borrar registros de excepcion cuando se da de baja registro principal.
		/// </summary>
		/// <param name="RepEvent_id"></param>
		/// <param name="RepUser_id"></param>
		/// <returns></returns>
		public String deleteExceptionStgTableQryTxt(String RepEvent_id, String RepUser_id)
		{

			CfDbTable tgtTbFmt = this.ExceptionTab;

			String srcDbName = this.DbName + "_STG";
			String srcTbName = Regex.Replace(this.TbName, "(?<!^)[aeui](?!$)", "") + "_U"; // Tabla para seleccionar registros a aplicar.

			return merge_delete_Follower_Normal_HistStgTableQryText(
									tgtTbFmt, tgtTbFmt.DbName, tgtTbFmt.TbName,
									this, srcDbName, srcTbName, 
									false, RepEvent_id, RepUser_id);

		}

		/// <summary>
		/// Genera query para borrar registros de excepcion cuando se da de baja registro principal.
		/// </summary>
		/// <param name="RepEvent_id"></param>
		/// <param name="RepUser_id"></param>
		/// <returns></returns>
		public String deleteExceptionFollowerStgTableQryTxt(int followerNum, String RepEvent_id, String RepUser_id)
		{

			if (this.ExceptionTab == null)
				return "";

			if (this.ExceptionTab.paceFollowerTabs == null)
				return "";

			CfDbTable tgtTbFmt = this.ExceptionTab.paceFollowerTabs[followerNum];

			String srcDbName = this.DbName + "_STG";
			String srcTbName = Regex.Replace(this.ExceptionTab.TbName, "(?<!^)[aeui](?!$)", "") + "_F" + followerNum.ToString(); // Tabla para seleccionar registros a aplicar del follower.

			return merge_delete_Follower_Normal_HistStgTableQryText(
									tgtTbFmt, tgtTbFmt.DbName, tgtTbFmt.TbName,
									tgtTbFmt, srcDbName, srcTbName,
									false, RepEvent_id, RepUser_id);

		}


		public String delete_Normal_StgTableQryText (
								CfDbTable tgtTbFmt, String tgtDbName, String tgtTbName,
								CfDbTable srcTbFmt, String srcDbName, String srcTbName)
		{
			// Armo string de consulta.
			String qry_txt = "";
			String sep = "";

			qry_txt += "DELETE t--Caso 4\n";
			qry_txt += "FROM\n";
			qry_txt += tgtDbName + "." + tgtTbName + " t,\n";
			qry_txt += srcDbName + "." + srcTbName + " n\n";
			qry_txt += "WHERE\n";

			//Primary Key
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < srcTbFmt.PkColumns.Length; idx++)
				{
					if (col.ColName.Equals(srcTbFmt.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = srcTbFmt.PkColumns[idx];
						break;
					}
				}

				if (km != null)
				{
					qry_txt += sep;
					qry_txt += "t.";
					qry_txt += col.ColName;
					qry_txt += "=";
					qry_txt += "n.";
					qry_txt += km.ColName;
					qry_txt += "\n";
					sep = "AND ";
				}
			}

			return qry_txt;
		}


		private String merge_Normal_StgTableQryText(
								CfDbTable tgtTbFmt, String tgtDbName, String tgtTbName,
								CfDbTable srcTbFmt, String srcDbName, String srcTbName,
								String RepEvent_id, String RepUser_id)
		{
			String qry_txt = "";
			String sep = "";

			// Comienzo a armar el sql de actualizacion.

			qry_txt += "MERGE INTO ";
			qry_txt += tgtDbName;
			qry_txt += ".";
			qry_txt += tgtTbName;
			qry_txt += " AS __tbl\n";
			qry_txt += "USING(\n";
			qry_txt += "SELECT\n";


			//Todas las columnas
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.DbCols)
			{
				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Clausula from
			qry_txt += "FROM\n";
			qry_txt += "(SELECT\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep + col.ColName + "\n";
				sep = ",";
			}

			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in tgtTbFmt.NoPkColumns)
			{
				qry_txt += sep;

				if (col.isTrkDt) // Tracking date.
				{
					qry_txt += "CURRENT_DATE ";
				}
				else if (col.isTrkTm) // Tracking time.
				{
					qry_txt += "CURRENT_TIME(6) ";
				}
				else if (col.isTrkEvent) // Tracking event id
				{
					qry_txt += "'" + RepEvent_id + "' ";
				}
				else if (col.isTrkUser) // User id
				{
					qry_txt += "'" + RepUser_id + "' ";
				}

				qry_txt += col.ColName + "\n";
				sep = ",";
			}

			qry_txt += "FROM\n";
			qry_txt += srcDbName + "." + srcTbName + "\n";

			qry_txt += ")n\n";

			qry_txt += ")__u\n";
			qry_txt += "ON\n";

			/*
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "__tbl.";
				qry_txt += col.ColName;
				qry_txt += "=";
				qry_txt += "__u.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}
			*/
			//Armo condicion con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < srcTbFmt.PkColumns.Length; idx++)
				{
					if (col.ColName.Equals(srcTbFmt.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = srcTbFmt.PkColumns[idx];
						break;
					}
				}

				if (km != null)
				{
					qry_txt += sep;
					qry_txt += "__tbl.";
					qry_txt += col.ColName;
					qry_txt += "=";
					qry_txt += "__u.";
					qry_txt += km.ColName;
					qry_txt += "\n";
					sep = "AND ";
				}
			}

			qry_txt += "WHEN MATCHED THEN UPDATE\n";
			qry_txt += "SET\n";

			//Actualizo las columnas que no son pk
			sep = " ";
			foreach (CfDbColumn c in tgtTbFmt.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += c.ColName;
				qry_txt += " = ";
				qry_txt += "__u.";
				qry_txt += c.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			qry_txt += "WHEN NOT MATCHED THEN INSERT\n";
			qry_txt += "(\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.DbCols)
			{
				qry_txt += sep;
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			qry_txt += ")\n";
			qry_txt += "VALUES\n";
			qry_txt += "(\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.DbCols)
			{
				qry_txt += sep;
				qry_txt += "__u.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			qry_txt += ")\n";

			///////////////////
			return qry_txt;
		}


		public String merge_delete_Follower_Normal_HistStgTableQryText(
									CfDbTable tgtTbFmt, String tgtDbName, String tgtTbName, 
									CfDbTable srcTbFmt, String srcDbName, String srcTbName, 
									bool forMerge, String RepEvent_id, String RepUser_id)
		{
			String qry_txt = "";
			String sep = "";

			// Comienzo a armar el sql de actualizacion.

			/* 
				Caso 4
				El periodo del registro existente esta completamente contenido en el periodo del nuevo registro.
				El registro existente debe eliminarse.
			*/

			qry_txt += "DELETE t--Caso 4\n";
			qry_txt += "FROM\n";
			qry_txt += tgtDbName + "." + tgtTbName + " t,\n";
			qry_txt += srcDbName + "." + srcTbName + " n\n";
			qry_txt += "WHERE\n";

			//Armo condicion con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < srcTbFmt.PkColumns.Length; idx++)
				{
					if (col.ColName.Equals(srcTbFmt.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = srcTbFmt.PkColumns[idx];
						break;
					}
				}

				if (km != null)
				{
					qry_txt += sep + "t." + km.ColName + " = n." + col.ColName + "\n";
					sep = "AND ";
				}
			}

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep + "n." + tgtTbFmt.StartDt_Column.ColName + " <= t." + tgtTbFmt.StartDt_Column.ColName + "\n";
			sep = "AND ";

			// EndDt
			qry_txt += sep + "t." + tgtTbFmt.EndDt_Column.ColName + " <= n." + tgtTbFmt.EndDt_Column.ColName + "\n";
			sep = "AND ";

			qry_txt += ";";
			qry_txt += "MERGE INTO ";
			qry_txt += tgtDbName;
			qry_txt += ".";
			qry_txt += tgtTbName;
			qry_txt += " AS __tbl\n";
			qry_txt += "USING(\n";


			/*
			Caso 1
			El nuevo registro esta en medio del  periodo de un unico registro.
			El registro existente debe partirse en un primer periodo precedente al 
			nuevo ( nro 1), y otro posterior al nuevo ( nro 3). El nuevo, sera el 2 en 
			la nueva serie.
			*/

			qry_txt += "SELECT --Caso 1\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}
			/*-- Necesito una columna con StartDt original, para joinear,
			 * ya que el inicio es una columna que puede cambiar,
			 * en otros casos, y es pk.
			 * En el registro identificado 3, como es un registro nuevo
			 * debe ser igual a StartDt.
			*/
			qry_txt += sep;
			qry_txt += "(CASE\n";
			qry_txt += "WHEN Ord.Ord = 1 THEN o.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += "WHEN Ord.Ord = 3 THEN n.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " + 1\n";

			qry_txt += "END)";
			qry_txt += " AS _J_"; // Para armar el alias de esta columna uso el prefijo "_J_".
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";


			/* Armo la columna con StartDt, teniendo en cuenta si corresponde al periodo
			 * previo al nuevo registro o periodo posterior.
			 * Esto se hace mediante una columna que se genera dinamicamente, que puede ser
			 * '1' para el registro previo y '3' para el registro posterior. El '2' corresponderia
			 * al nuevo registro, pero no se usa.
			 * Si la nueva fecha de fin es '9999-12-31' no hace falta generar la fila posterior, ya
			 * que esa es la maxima fecha que se puede representar.
			*/
			qry_txt += sep;
			qry_txt += "(CASE\n";
			qry_txt += "WHEN Ord.Ord = 1 THEN o.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			/*
			 * Este caso no sucede por el filtro de esta subquery.
			qry_txt += "WHEN Ord.Ord = 3 AND n.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " = DATE '9999-12-31' THEN n.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " + 1\n";
			*/

			qry_txt += "WHEN Ord.Ord = 3 THEN n.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " + 1\n";

			qry_txt += "END)";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			/* Armo la columna con EndDt, teniendo en cuenta si corresponde al periodo
			 * previo al nuevo registro o periodo posterior.
			 * Esto se hace mediante una columna que se genera dinamicamente, que puede ser
			 * '1' para el registro previo y '3' para el registro posterior. El '2' corresponderia
			 * al nuevo registro, pero no se usa.
			*/

			qry_txt += sep;
			qry_txt += "(CASE Ord.Ord\n";
			qry_txt += "WHEN 1 THEN n.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " - 1\n ";
			qry_txt += "WHEN 3 THEN o.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";
			qry_txt += "END)";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";

			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in tgtTbFmt.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Clausula from
			qry_txt += "FROM\n";
			qry_txt += tgtDbName;
			qry_txt += ".";
			qry_txt += tgtTbName;
			qry_txt += " AS o\n";
			qry_txt += "INNER JOIN\n";
			qry_txt += "(SELECT\n";

			/*
			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep + col.ColName + "\n";
				sep = ",";
			}
			*/
			//Armo join con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < srcTbFmt.PkColumns.Length; idx++)
				{
					if (col.ColName.Equals(srcTbFmt.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = srcTbFmt.PkColumns[idx];
						break;
					}
				}

				if (km != null)
				{
					qry_txt += sep + km.ColName + "\n";
					sep = ",";
				}
			}

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep + tgtTbFmt.StartDt_Column.ColName + "\n";
			sep = ",";

			// EndDt
			qry_txt += sep + tgtTbFmt.EndDt_Column.ColName + "\n";
			sep = ",";

			qry_txt += "FROM\n";
			qry_txt += srcDbName + "." + srcTbName + "\n";

			qry_txt += ")n\n";
			qry_txt += "ON\n";

			/*
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "=n.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}
			*/
			//Armo join con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < srcTbFmt.PkColumns.Length; idx++)
				{
					if (col.ColName.Equals(srcTbFmt.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = srcTbFmt.PkColumns[idx];
						break;
					}
				}

				if (km != null)
				{
					qry_txt += sep;
					qry_txt += "o.";
					qry_txt += col.ColName;
					qry_txt += "= n.";
					qry_txt += km.ColName;
					qry_txt += "\n";
					sep = "AND ";
				}
			}

			// Condiciones de fecha.
			// AND	o.T_Start <= n.T_Start AND n.T_End <= o.T_End
			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " <= ";
			qry_txt += "n.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";
			sep = "AND ";

			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " <= ";
			qry_txt += "o.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";

			// La siguiente tabla permite generar dos registros de salida.
			qry_txt += "CROSS JOIN\n";
			qry_txt += "(\n";
			qry_txt += "SELECT Ord FROM  (SELECT 1 Ord ) t1\n"; //Para generar el registro previo al nuevo.
			qry_txt += "UNION\n";
			qry_txt += "SELECT Ord FROM  (SELECT 3 Ord ) t3\n"; //Para generar el registro posterior al nuevo.
			qry_txt += ") Ord\n";

			qry_txt += "WHERE\n";

			//Filtro por inicio de periodo. El registro 1 no sale si el nuevo empieza el mismo dia.
			qry_txt += "    NOT (  Ord.Ord = 1 AND o.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " = n.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += ")\n";

			//Filtro por fin de periodo. El registro 3 no sale si el nuevo termina el mismo dia.
			qry_txt += "AND NOT (  Ord.Ord = 3 AND n.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " = o.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " )\n";

			qry_txt += "UNION\n";

			/*
				Caso 2
				El periodo del nuevo registro comienza antes que el periodo del registro existente,
				y finaliza antes que el fin del  registro existente.
				El registro existente debe actualizarse para que cominenze luego del registro nuevo.
			*/

			qry_txt += "SELECT --Caso 2\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}
			/*-- Necesito una columna con StartDt original, para joinear,
			 * ya que el inicio es una columna que puede cambiar,
			 * en otros casos, y es pk.
			*/
			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " AS _J_"; // Para armar el alias de esta columna uso el prefijo "_J_".
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " + 1 ";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			// EndDt
			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " ";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";


			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in tgtTbFmt.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Clausula from
			qry_txt += "FROM\n";
			qry_txt += tgtDbName;
			qry_txt += ".";
			qry_txt += tgtTbName;
			qry_txt += " AS o\n";
			qry_txt += "INNER JOIN\n";
			qry_txt += "(SELECT\n";

			/*
			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep + col.ColName + "\n";
				sep = ",";
			}
			*/
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < srcTbFmt.PkColumns.Length; idx++)
				{
					if (col.ColName.Equals(srcTbFmt.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = srcTbFmt.PkColumns[idx];
						break;
					}
				}

				if (km != null)
				{
					qry_txt += sep + km.ColName + "\n";
					sep = ",";
				}
			}

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep + tgtTbFmt.StartDt_Column.ColName + "\n";
			sep = ",";

			// EndDt
			qry_txt += sep + tgtTbFmt.EndDt_Column.ColName + "\n";
			sep = ",";

			qry_txt += "FROM\n";
			qry_txt += srcDbName + "." + srcTbName + "\n";

			qry_txt += ")n\n";

			qry_txt += "ON\n";

			/*
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "=n.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}
			*/
			//Armo join con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < srcTbFmt.PkColumns.Length; idx++)
				{
					if (col.ColName.Equals(srcTbFmt.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = srcTbFmt.PkColumns[idx];
						break;
					}
				}

				if (km != null)
				{
					qry_txt += sep;
					qry_txt += "o.";
					qry_txt += col.ColName;
					qry_txt += "= n.";
					qry_txt += km.ColName;
					qry_txt += "\n";
					sep = "AND ";
				}
			}

			// Condiciones de fecha.
			//AND	n.T_Start < o.T_Start
			//AND   o.T_Start <= n.T_End
			//AND	n.T_End <= o.T_End

			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " < ";
			qry_txt += "o.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " <= ";
			qry_txt += "n.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " <= ";
			qry_txt += "o.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += "UNION\n";


			/*
			Caso 3
			El periodo del nuevo registro comienza despues que el periodo del registro existente,
			y finaliza despues que el fin del  registro existente.

			El registro existente debe actualizarse para que finalice antes que el registro nuevo.
			*/

			qry_txt += "SELECT --Caso 3\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}
			/*-- Necesito una columna con StartDt original, para joinear,
			 * ya que el inicio es una columna que puede cambiar,
			 * en otros casos, y es pk.
			*/
			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " AS _J_"; // Para armar el alias de esta columna uso el prefijo "_J_".
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " ";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			// EndDt
			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " - 1 ";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";


			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in tgtTbFmt.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Clausula from
			qry_txt += "FROM\n";
			qry_txt += tgtDbName;
			qry_txt += ".";
			qry_txt += tgtTbName;
			qry_txt += " AS o\n";
			qry_txt += "INNER JOIN\n";
			qry_txt += "(SELECT\n";

			/*
			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep + col.ColName + "\n";
				sep = ",";
			}
			*/
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < srcTbFmt.PkColumns.Length; idx++)
				{
					if (col.ColName.Equals(srcTbFmt.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = srcTbFmt.PkColumns[idx];
						break;
					}
				}

				if (km != null)
				{
					qry_txt += sep + km.ColName + "\n";
					sep = ",";
				}
			}

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep + tgtTbFmt.StartDt_Column.ColName + "\n";
			sep = ",";

			// EndDt
			qry_txt += sep + tgtTbFmt.EndDt_Column.ColName + "\n";
			sep = ",";

			qry_txt += "FROM\n";
			qry_txt += srcDbName+ "." + srcTbName + "\n";

			qry_txt += ")n\n";

			qry_txt += "ON\n";

			/*
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "o.";
				qry_txt += col.ColName;
				qry_txt += "=n.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}
			*/
			//Armo join con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < srcTbFmt.PkColumns.Length; idx++)
				{
					if (col.ColName.Equals(srcTbFmt.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = srcTbFmt.PkColumns[idx];
						break;
					}
				}

				if (km != null)
				{
					qry_txt += sep;
					qry_txt += "o.";
					qry_txt += col.ColName;
					qry_txt += "= n.";
					qry_txt += km.ColName;
					qry_txt += "\n";
					sep = "AND ";
				}
			}

			// Condiciones de fecha.
			//AND	o.T_Start < n.T_Start
			//AND   n.T_Start < o.T_End
			//AND	o.T_End < n.T_End

			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " < ";
			qry_txt += "n.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += sep;
			qry_txt += "n.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += " <= ";
			qry_txt += "o.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += sep;
			qry_txt += "o.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " < ";
			qry_txt += "n.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";

			if (forMerge) // Inicio seccion insert nuevo registro. Vale para merge, se omite para delete.
			{
				qry_txt += "UNION\n";

				/*
				 * Este es el registro nuevo completo, para la insercion.
				 * Se agrega columna StartDt para join.
				 * Se hace un join anidado para que la union lo vea como tabla.
				*/

				qry_txt += "SELECT\n";
				//Primero la pk, que no sea StartDt.
				sep = " ";
				foreach (CfDbColumn col in tgtTbFmt.PkColumns)
				{
					qry_txt += sep;
					qry_txt += "n.";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
				}
				/*-- Necesito una columna con StartDt original, para joinear,
				 * ya que el inicio es una columna que puede cambiar,
				 * en otros casos, y es pk.
				*/
				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += tgtTbFmt.StartDt_Column.ColName;
				qry_txt += " AS _J_"; // Para armar el alias de esta columna uso el prefijo "_J_".
				qry_txt += tgtTbFmt.StartDt_Column.ColName;
				qry_txt += "\n";

				//Luego colummnas fecha
				// StartDt
				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += tgtTbFmt.StartDt_Column.ColName;
				qry_txt += "\n";

				// EndDt
				qry_txt += sep;
				qry_txt += "n.";
				qry_txt += tgtTbFmt.EndDt_Column.ColName;
				qry_txt += "\n";


				//Al final, el resto de las columnas.
				foreach (CfDbColumn col in tgtTbFmt.NoPkColumns)
				{
					qry_txt += sep;
					qry_txt += "n.";
					qry_txt += col.ColName;
					qry_txt += "\n";
					sep = ",";
				}

				//Clausula from
				qry_txt += "FROM\n";
				qry_txt += "(SELECT\n";

				//Primero la pk, que no sea StartDt.
				sep = " ";
				foreach (CfDbColumn col in tgtTbFmt.PkColumns)
				{
					qry_txt += sep + col.ColName + "\n";
					sep = ",";
				}

				/*-- Necesito una columna con StartDt original, para joinear,
				 * ya que el inicio es una columna que puede cambiar,
				 * en otros casos, y es pk.
				*/
				qry_txt += sep + tgtTbFmt.StartDt_Column.ColName;
				qry_txt += " _J_";
				qry_txt += tgtTbFmt.StartDt_Column.ColName;
				qry_txt += "\n";
				sep = ",";

				//Luego colummnas fecha
				// StartDt
				qry_txt += sep + tgtTbFmt.StartDt_Column.ColName + "\n";
				sep = ",";

				// EndDt
				qry_txt += sep + tgtTbFmt.EndDt_Column.ColName + "\n";
				sep = ",";

				//Al final, el resto de las columnas.
				foreach (CfDbColumn col in tgtTbFmt.NoPkColumns)
				{
					qry_txt += sep;

					if (col.isTrkDt) // Tracking date.
					{
						qry_txt += "CURRENT_DATE ";
					}
					else if (col.isTrkTm) // Tracking time.
					{
						qry_txt += "CURRENT_TIME(6) ";
					}
					else if (col.isTrkEvent) // Tracking event id
					{
						qry_txt += "'" + RepEvent_id + "' ";
					}
					else if (col.isTrkUser) // User id
					{
						qry_txt += "'" + RepUser_id + "' ";
					}

					qry_txt += col.ColName + "\n";
					sep = ",";
				}

				qry_txt += "FROM\n";
				qry_txt += srcDbName + "." + srcTbName + "\n";

				qry_txt += ")n\n";
			} // Fin seccion insert nuevo registro.

			qry_txt += ")__u\n";
			qry_txt += "ON\n";

			/*
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "__tbl.";
				qry_txt += col.ColName;
				qry_txt += "=";
				qry_txt += "__u.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = "AND ";
			}
			*/
			//Armo join con claves en comun
			//Primero la pk, que no sea StartDt.
			sep = "    ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				CfDbColumn km = null;
				int idx;
				for (idx = 0; idx < srcTbFmt.PkColumns.Length; idx++)
				{
					if (col.ColName.Equals(srcTbFmt.PkColumns[idx].ColName, StringComparison.CurrentCultureIgnoreCase))
					{
						km = srcTbFmt.PkColumns[idx];
						break;
					}
				}

				if (km != null)
				{
					qry_txt += sep;
					qry_txt += "__tbl.";
					qry_txt += col.ColName;
					qry_txt += "=";
					qry_txt += "__u.";
					qry_txt += km.ColName;
					qry_txt += "\n";
					sep = "AND ";
				}
			}

			//Fecha de join, la original del registro.
			qry_txt += sep;
			qry_txt += "__tbl."; // En la tabla uso el nombre original.
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "=";
			qry_txt += "__u._J_"; // En el update uso la fecha reservada para el updata. con prefijo _J_
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += "WHEN MATCHED THEN UPDATE\n";
			qry_txt += "SET\n";

			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "=";
			qry_txt += "__u."; // Para armar el alias de esta columna uso el prefijo "_J_".
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			qry_txt += ",";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "=";
			qry_txt += "__u."; // Para armar el alias de esta columna uso el prefijo "_J_".
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";

			//Actualizo las columnas que no son pk
			sep = ",";
			foreach (CfDbColumn c in tgtTbFmt.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += c.ColName;
				qry_txt += " = ";
				qry_txt += "(CASE WHEN ";
				qry_txt += "__u._J_";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " = ";
				qry_txt += "__u.";
				qry_txt += this.StartDt_Column.ColName;
				qry_txt += " THEN ";
				qry_txt += "__u.";
				qry_txt += c.ColName;
				qry_txt += " ELSE ";
				qry_txt += "__tbl.";
				qry_txt += c.ColName;
				qry_txt += " END)";
				qry_txt += "\n";
				sep = ",";
			}

			qry_txt += "WHEN NOT MATCHED THEN INSERT\n";
			qry_txt += "(\n";


			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep;
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			// EndDt
			qry_txt += sep;
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";


			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in tgtTbFmt.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			qry_txt += ")\n";
			qry_txt += "VALUES\n";
			qry_txt += "(\n";

			//Primero la pk, que no sea StartDt.
			sep = " ";
			foreach (CfDbColumn col in tgtTbFmt.PkColumns)
			{
				qry_txt += sep;
				qry_txt += "__u.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			//Luego colummnas fecha
			// StartDt
			qry_txt += sep;
			qry_txt += "__u.";
			qry_txt += tgtTbFmt.StartDt_Column.ColName;
			qry_txt += "\n";

			// EndDt
			qry_txt += sep;
			qry_txt += "__u.";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += "\n";


			//Al final, el resto de las columnas.
			foreach (CfDbColumn col in tgtTbFmt.NoPkColumns)
			{
				qry_txt += sep;
				qry_txt += "__u.";
				qry_txt += col.ColName;
				qry_txt += "\n";
				sep = ",";
			}

			qry_txt += ")\n";

			///////////////////
			return qry_txt;
		}
		//////////////////////////////////////////////////////////////////////////////     
		public String CloseHistQryText(out String[] paramNames)
		{
			// Armo string de consulta.
			String sep = " ";
			int param_cnt = 0;
			String qry_txt = "";

			// Se crea array de salida con lista de parametros de query.
			// Para el tamaño se descuenta uno por el inicio de vigencia y se suma el evento y el usuario que modifico.
			int paramSize = (this.PkFldCnt - 1) * 2 + (this.TrkEvent_Column != null ? 1 : 0) + (this.TrkUser_Column != null ? 1 : 0);
			paramNames = new String[paramSize];

			CfDbTable followerTab = this.paceFollowingTab;

			qry_txt = "DELETE FROM\n";
			qry_txt += this.DbName + "." + this.TbName + "\n";
			qry_txt += "WHERE\n";

			foreach (CfDbColumn c in followerTab.DbCols)
			{
				if (c.isPk && c.isStartDt == false)
				{
					qry_txt += sep + c.ColName + " = CAST(? AS " + c.DataType + ")\n";

					paramNames[param_cnt++] = c.ColName; //Inserto en lista, y "luego" incremento indice.
				}

				sep = "AND ";
			}

			qry_txt += sep + this.StartDt_Column.ColName + " = CURRENT_DATE\n";
			qry_txt += ";\n";

			qry_txt += "UPDATE t \n";
			qry_txt += "FROM \n";
			qry_txt += this.DbName + "." + this.TbName + " t\n";
			qry_txt += ",(\n";
			qry_txt += "SELECT \n";

			sep = " ";
			foreach (CfDbColumn c in followerTab.DbCols)
			{
				if (c.isPk == true && c.isStartDt == false)
				{
					qry_txt += sep + c.ColName + "\n";
				}

				sep = ",";
			}

			qry_txt += sep + this.StartDt_Column.ColName + "\n";
			qry_txt += sep + this.EndDt_Column.ColName + "\n";

			sep = " ";
			qry_txt += "FROM\n";
			qry_txt += sep + followerTab.DbName + "." + followerTab.TbName + "\n";
			qry_txt += "WHERE\n";

			//NumCol = 0;
			foreach (CfDbColumn c in followerTab.DbCols)
			{
				if (c.isPk && c.isStartDt == false)
				{
					qry_txt += sep + c.ColName + " = CAST(? AS " + c.DataType + ")\n";

					paramNames[param_cnt++] = c.ColName; //Inserto en lista, y "luego" incremento indice.
				}

				sep = "AND ";
			}

			qry_txt += sep + "CURRENT_DATE BETWEEN " + followerTab.StartDt_Column.ColName + " AND " + followerTab.EndDt_Column.ColName + "\n";
			qry_txt += ")u\n";

			sep = " ";
			qry_txt += "SET\n";
			qry_txt += sep + this.EndDt_Column.ColName + " = CASE WHEN t." + this.EndDt_Column.ColName + "= '9999-12-31'" + " THEN CURRENT_DATE - 1" + " ELSE t." + this.StartDt_Column.ColName + " - 1 END \n";

			sep = ", ";
			qry_txt += sep + this.TrkDt_Column.ColName + " = CURRENT_DATE\n";
			qry_txt += sep + this.TrkTm_Column.ColName + " = CURRENT_TIME(6)\n";
			qry_txt += sep + this.TrkEvent_Column.ColName + " = CAST(? AS " + this.TrkEvent_Column.DataType + ")\n"; 
			paramNames[param_cnt++] = this.TrkEvent_Column.ColName;
			qry_txt += sep + this.TrkUser_Column.ColName + " = CAST(? AS " + this.TrkUser_Column.DataType + ")\n"; 
			paramNames[param_cnt++] = this.TrkUser_Column.ColName;

			qry_txt += "WHERE\n";

			sep = " ";
			foreach (CfDbColumn c in followerTab.DbCols)
			{
				if (c.isPk == true && c.isStartDt == false)
				{
					qry_txt += sep + "t." + c.ColName + " = u." + c.ColName + "\n";
				}

				sep = "AND ";
			}

			qry_txt += sep + "t." + this.StartDt_Column.ColName + " = u." + this.StartDt_Column.ColName + "\n";
			qry_txt += sep + "t." + this.EndDt_Column.ColName + " = u." + this.EndDt_Column.ColName + "\n";
			qry_txt += ";";

			return qry_txt;
		}
		//////////////////////////////////////////////////////////////////////////////    
		public String close_Follower_Normal_HistStgTableQryText(
									CfDbTable tgtTbFmt, String tgtDbName, String tgtTbName,
									CfDbTable srcTbFmt, String srcDbName, String srcTbName,
									bool forMerge, String RepEvent_id, String RepUser_id)
		{
			String sep = " ";
			String qry_txt = "";
			int numCol = 0;

			CfDbTable followerTab = tgtTbFmt.paceFollowingTab;

			qry_txt = "DELETE t FROM\n";
			qry_txt += tgtDbName + "." + tgtTbName + " t" + "\n";
			qry_txt += "," + srcDbName + "." + srcTbName + " u" + "\n";
			qry_txt += "WHERE\n";

			foreach (CfDbColumn c in followerTab.DbCols)
			{
				if (c.isPk && c.isStartDt == false)
				{
					//qry_txt += sep + c.ColName + " = CAST(? AS " + c.DataType + ")\n";
					qry_txt += sep + "t." + c.ColName + " = " + "u." + c.ColName + "\n";
				}

				sep = "AND ";
			}

			qry_txt += sep + "t." + this.StartDt_Column.ColName + " = CURRENT_DATE\n";
			qry_txt += ";\n";

			qry_txt += "UPDATE t \n";
			qry_txt += "FROM \n";
			qry_txt += tgtDbName + "." + tgtTbName + " t,\n";
			qry_txt += "(\n";
			qry_txt += "SELECT DISTINCT\n";
			foreach (CfDbColumn col in followerTab.PkColumns)
			{
				if (col.isPk == true)
				{
					if (numCol > 0)
					{
						qry_txt += ",";
					}
					qry_txt += col.ColName;
					qry_txt += "\n";
				}
				numCol++;
			}
			qry_txt += "," + tgtTbFmt.StartDt_Column.ColName + "\n";
			qry_txt += "FROM\n";
			qry_txt += srcDbName + "." + srcTbName + " u\n";
			qry_txt += ") u\n";
			qry_txt += "SET\n";
			qry_txt += tgtTbFmt.EndDt_Column.ColName;
			qry_txt += " = CASE WHEN " + "t." + tgtTbFmt.StartDt_Column.ColName + "= '9999-12-31'" + " THEN CURRENT_DATE - 1" + " ELSE " + "t." + tgtTbFmt.StartDt_Column.ColName + " - 1 END \n";
			qry_txt += "WHERE\n";
			foreach (CfDbColumn col in followerTab.PkColumns)
			{
				if (col.isPk == true && col.isStartDt == false)
				{
					qry_txt += "t." + col.ColName + " = " + "u." + col.ColName + "\n";
					qry_txt += "AND\n";
				}
			}
			qry_txt += "CURRENT_DATE <= t." + tgtTbFmt.EndDt_Column.ColName + "\n";
			qry_txt += "AND\n";
			qry_txt += "t." + tgtTbFmt.StartDt_Column.ColName + " = u." + srcTbFmt.StartDt_Column.ColName;
			qry_txt += ";\n";

			return qry_txt;
		}
		//////////////////////////////////////////////////////////////////////////////    
	}
}