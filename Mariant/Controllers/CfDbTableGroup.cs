using System;

namespace WebAsistida.lib
{
    public class CfDbTableGroup
    {
        public CfDbTable[] Tables;
        public bool[] TableFilterFlag;
        public String[] TableColFilter;

        ///////////////////////////////////////////////////////////////////////////////
        //Constructor
        public CfDbTableGroup(CfDbTable[] _Tables, bool[] _TableFilterFlag, String[] _TableColFilter)
        {
            Tables = _Tables;
            TableFilterFlag = _TableFilterFlag;
            TableColFilter = _TableColFilter;
        }
        ///////////////////////////////////////////////////////////////////////////////
    }
}