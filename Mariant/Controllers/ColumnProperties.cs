using System;

namespace WebAsistida.lib
{
    public class ColumnProperties
    {
        public String ColName { get; set; }
        public String ColAlias { get; set; }
        public bool ColPk { get; set; }
        public String TableAlias { get; set; }
        public String TableName { get; set; }
    }

    public class TableProperties
    {
        public String TableAlias { get; set; }
        public String TableName { get; set; }
        public String TableDbName {get; set;}
        public CfDbColumn[] PkColumns { get; set; }
        public CfDbRelAttribute[] RelAtts { get; set; }
        public String JointType { get; set; }
        public String JoinTableAlias { get; set; }
    }
}