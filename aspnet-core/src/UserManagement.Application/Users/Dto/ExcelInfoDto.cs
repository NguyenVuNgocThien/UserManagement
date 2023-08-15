using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Users.Dto
{
    public class ExcelInfoDto
    {
        public string PathName { get; set; }
        public string StoreName { get; set; }
        public string TypeExport { get; set; }
        public bool ALL { get; set; }
        public List<ExcelParameter> Parameters { get; set; }
        
    }
    public class ReportInfoInput
    {
        public string PathName { get; set; }
        public string StoreName { get; set; }
    }
    public class ExcelParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
