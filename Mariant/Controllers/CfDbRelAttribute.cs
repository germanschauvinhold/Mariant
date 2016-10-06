using System;

namespace WebAsistida.lib
{
    public class CfDbRelAttribute
    {
        public CfDbRelation Relation;
        public CfDbColumn ParentColumn;
        public CfDbColumn ChildColumn;

        public CfDbRelAttribute(CfDbRelation _Relation, CfDbColumn _ParentColumn, CfDbColumn _ChildColumn)
        {
            Relation = _Relation;
            ParentColumn = _ParentColumn;
            ChildColumn = _ChildColumn;
        }
        
        public CfDbRelAttribute(CfDbRelation _Relation, String _ParentColName, String _ChildColName)
        {
            Relation = _Relation;
            ParentColumn = _Relation.ParentTable.getColumn(_ParentColName);
            ChildColumn = _Relation.ChildTable.getColumn(_ChildColName);
        }

    
    }
}