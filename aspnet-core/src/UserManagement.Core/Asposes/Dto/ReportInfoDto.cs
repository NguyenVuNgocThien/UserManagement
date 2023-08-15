using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Helpper;

namespace UserManagement.Asposes.Dto
{
    public class ReportInfoDto
    {
        public ReportInfoDto()
        {
            TypeExport = "";
            StoreName = "";
            PathName = null;
            Parameters = new List<ReportParameter>();
            Values = new List<ReportParameter>();
            Header = new List<string>();
            Start_Column = 0;
            Start_Row = 0;
            addHeader = false;
            orientation_page = "portrait";
        }
        public string StoreName { get; set; }
        public string TypeExport { get; set; }
        public string PathName { get; set; }
        public string FileName { get; set; }
        public List<ReportParameter> Parameters { get; set; }
        public List<ReportParameter> Values { get; set; }
        public List<string> Header { get; set; }
        public int Start_Column { get; set; }
        public int Start_Row { get; set; }
        public bool addHeader { get; set; }
        public string orientation_page { get; set; }
    }
}
