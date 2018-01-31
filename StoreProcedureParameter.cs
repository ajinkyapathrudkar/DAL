
namespace Kale.Logistics.Framework.Common.DataAccessLayer
{
    public class StoreProcedureParameter
    {
        public string ParameterName { get; set; }
        public bool IsInParameter { get; set; }
        public string DbType { get; set; }
        public object Value { get; set; }
        public int Size { get; set; }
        public string SourceColumn { get; set; }
    }

}
