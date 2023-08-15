using Aspose.Cells;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserManagement.Asposes.Dto;
using UserManagement.CustomRepository;

namespace UserManagement.Asposes
{
    public class ReportExporter : IReportExporter
    {
        private readonly string directionReport;
        private readonly ISqlCustomRepository _storeProcedureProvider;
        private readonly IConfigurationRoot appConfiguration;

        public ReportExporter(ISqlCustomRepository storeProcedureProvider,IHostingEnvironment hostingEnvironment)
        {
            _storeProcedureProvider = storeProcedureProvider;
            directionReport = hostingEnvironment.ContentRootPath;
        }

        public async Task<dynamic> CreateExcelFileAndDesignUser(ReportInfoDto info)
        {
            DataSet data = await GetDataFromStoreToUser(info);
            Workbook designer = new Workbook(directionReport + info.PathName);
            int tableCount = data.Tables.Count;
            WorkbookDesigner designerWord = new WorkbookDesigner(designer);
            designerWord.SetDataSource("A1", DateTime.Now.Month+"/"+DateTime.Now.Year);
            for(int i = 0; i < tableCount; i++)
            {
                int rows = data.Tables[i].Rows.Count;
                int cols = data.Tables[i].Columns.Count;

                for(int row = 0; row < rows; row++)
                {
                    for(int col = 0; col < cols; col++)
                    {
                        var obj = data.Tables[i].Rows[row][col];
                        if (obj.GetType() == typeof(string))
                        {
                            string _value = obj.ToString();
                            bool check = _value.StartsWith('=') ||
                                _value.StartsWith('+') ||
                                _value.StartsWith('-') ||
                                _value.StartsWith('@');
                            if (check)
                            {
                                data.Tables[i].Rows[row][col] = _value.Insert(0, "'");
                            }
                        }
                    }
                }
            }
            designerWord.SetDataSource(data);
            designerWord.Process(false);
            designerWord.Workbook.FileName = info.FileName;
            if (info.TypeExport == "FileTypeConst.Pdf")
            {
                designerWord.Workbook.FileFormat = FileFormatType.Pdf;
            }
            else
            {
                designerWord.Workbook.FileFormat = FileFormatType.Xlsx;
            }
            designerWord.Workbook.Settings.CalcMode = CalcModeType.Automatic;
            designerWord.Workbook.Settings.RecalculateBeforeSave = true;
            designerWord.Workbook.Settings.ReCalculateOnOpen = true;
            designerWord.Workbook.Settings.CheckCustomNumberFormat = true;
            return new
            {
                designerWord = designerWord,
                data = data
            };
        }

        public async Task<MemoryStream> CreateExcelFileUser(ReportInfoDto info)
        {
            var fileAndDesign = await CreateExcelFileAndDesignUser(info);
            DynamicColumnFormat(fileAndDesign.designerWord.Workbook, fileAndDesign.data);
            MemoryStream str = new MemoryStream();
            switch (info.TypeExport.ToLower())
            {
                case FileTypeConst.Pdf:
                    fileAndDesign.designerWord.Workbook.Save(str, Aspose.Cells.SaveFormat.Pdf);
                    break;
                case FileTypeConst.Excel:
                    fileAndDesign.designerWord.Workbook.Save(str, Aspose.Cells.SaveFormat.Xlsx);
                    break;
            }
            return str;
        }
        void DynamicColumnFormat(Workbook wb,DataSet data)
        {
            var _dataSheetSource = data.Tables[0];
            //var _dynamicSource = data.Tables[0];
            //var _dataSource = data.Tables[0];
            foreach(var ws in wb.Worksheets)
            {
                int rowBegin, rowEnd, colBegin, colEnd;
                if (ws.Cells.FirstCell == null)
                {
                    continue;
                }
                rowBegin = ws.Cells.MinRow;
                rowEnd = ws.Cells.MaxRow;
                colBegin = ws.Cells.MinColumn;
                colEnd = ws.Cells.MaxColumn;
                int dynamicCollOffset = -1, dynamicRowOffset = -1;
                string dynamicColName = "";
                for(int i = rowBegin; i< rowEnd; i++)
                {
                    for(int j = colBegin; j < colEnd; j++)
                    {
                        if (ws.Cells.Rows[i][j].Value != null)
                        {
                            string regex = @"<dynamic>(.*)<\/dynamic>";
                            Match match = Regex.Match(ws.Cells.Rows[i][j].Value.ToString(), regex);
                            if (match.Success)
                            {
                                dynamicRowOffset = i;
                                dynamicCollOffset = j;
                                dynamicColName = match.Groups[1].Value;
                                goto findDynamicPositionDone;
                            }
                        }
                    }
                }
            findDynamicPositionDone:;
                if (dynamicRowOffset == -1 || dynamicCollOffset == -1)
                {
                    return;
                }
                int maxDynamicCollCount = 0;
                //int maxRowInDynamicSouce = _dynamicSource.Rows.Count;
                //for(int i = 0; i < maxRowInDynamicSouce; i++)
                //{
                //    int x = 0;
                //    maxDynamicCollCount = _dynamicSource.Rows.Count;
                //}
                //for(int i = 0; i < _dataSource.Rows.Count; i++)
                //{
                //    ws.Cells.InsertColumn(dynamicCollOffset + 1, true);
                //    ws.Cells.Rows[dynamicRowOffset][dynamicCollOffset + 1].Value = _dataSource.Rows[i][0].ToString() + "%";
                //}
                //ws.Cells.DeleteColumn(dynamicCollOffset + _dataSource.Rows.Count);
                int stt = 0;
                for (stt = 0; stt < _dataSheetSource.Rows.Count; stt++)
                {
                    int countNumber = 7;
                    //for(int i = dynamicRowOffset; i < rowEnd; i++)
                    //{
                    //    int phase = 0;
                    //    for(int j = dynamicCollOffset; j < dynamicCollOffset + _dataSource.Rows.Count; j++)
                    //    {
                    //        phase++;
                    //        if (_dataSheetSource.Rows.Count > 0)
                    //        {
                    //            if (i == 7)
                    //            {
                    //                ws.Cells.Rows[i][j].Value = "(" + countNumber.ToString() + ")";
                    //                countNumber++;
                    //            }
                    //        }
                    //    }
                    //}
                    //for(int i = dynamicRowOffset; i <= rowEnd; i++)
                    //{
                    //    int phase = 0;
                    //    for(int j = dynamicCollOffset; j < dynamicCollOffset + _dataSource.Rows.Count; j++)
                    //    {
                    //        phase++;
                    //        for(int k = 0; k < _dynamicSource.Rows.Count; k++)
                    //        {
                    //            if (_dataSheetSource.Rows.Count > 0)
                    //            {
                    //                if(
                    //                    _dynamicSource.Rows[k][7].ToString() == ws.Cells.Rows[i][1].Value.ToString()&&
                    //                    _dynamicSource.Rows[k][6].ToString() == ws.Cells.Rows[i][4].Value.ToString() &&
                    //                    _dynamicSource.Rows[k][5].ToString() == ws.Cells.Rows[i][3].Value.ToString() &&
                    //                    _dynamicSource.Rows[k][3].ToString()+"%" == ws.Cells.Rows[i][6].Value.ToString())
                    //                {
                    //                    ws.Cells.Rows[i][j].Value = _dynamicSource.Rows[k][1];
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
        }
        private async Task<DataSet> GetDataFromStoreToUser(ReportInfoDto info)
        {
            try
            {
                DataSet data = await _storeProcedureProvider.GetDataUser(info);
                for(int i = 0; i < data.Tables.Count; i++)
                {
                    data.Tables[i].TableName = "table" + i;
                }
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
