namespace WebAsistida.lib
{
    public class CfDbRelation
    {
        public CfDbTable ParentTable;
        public CfDbTable ChildTable;
        public CfDbRelAttribute[] RelAtts;

        public CfDbRelation(CfDbTable _ParentTable,
            CfDbTable _ChildTable, CfDbRelAttribute[] _RelAtts
            )
        {
            ParentTable = _ParentTable;
            ChildTable = _ChildTable;
            RelAtts = _RelAtts;
        }

        public CfDbRelation(CfDbTable _ParentTable, CfDbTable _ChildTable
            )
        {
            ParentTable = _ParentTable;
            ChildTable = _ChildTable;
            RelAtts = null;
        }

        public void setRelAtts(CfDbRelAttribute[] _RelAtts)
        {
            RelAtts = _RelAtts;               
        }
    }
}