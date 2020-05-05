using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchAPI
{
    public static class NOPIHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<List<List<string>>> ReadExcel(string fileName)
        {
            //打开Excel工作簿
            XSSFWorkbook hssfworkbook = null;
            try
            {
                using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new XSSFWorkbook(file);
                }
            }
            catch (Exception e)
            {
                //LogHandler.LogWrite(string.Format("文件{0}打开失败，错误：{1}", new string[] { fileName, e.ToString() }));
            }
            //循环Sheet页
            int sheetsCount = hssfworkbook.NumberOfSheets;
            List<List<List<string>>> workBookContent = new List<List<List<string>>>();
            for (int i = 0; i < sheetsCount; i++)
            {
                //Sheet索引从0开始
                ISheet sheet = hssfworkbook.GetSheetAt(i);
                //循环行
                List<List<string>> sheetContent = new List<List<string>>();
                int rowCount = sheet.PhysicalNumberOfRows;
                for (int j = 0; j < rowCount; j++)
                {
                    //Row（逻辑行）的索引从0开始
                    IRow row = sheet.GetRow(j);
                    //循环列（各行的列数可能不同）
                    List<string> rowContent = new List<string>();
                    int cellCount = row.PhysicalNumberOfCells;
                    for (int k = 0; k < cellCount; k++)
                    {
                        //ICell cell = row.GetCell(k);
                        NPOI.SS.UserModel.ICell cell = row.Cells[k];
                        if (cell == null)
                        {
                            rowContent.Add("NIL");
                        }
                        else
                        {
                            rowContent.Add(cell.ToString());
                            //rowContent.Add(cell.StringCellValue);
                        }
                    }
                    //添加行到集合中
                    sheetContent.Add(rowContent);
                }
                //添加Sheet到集合中
                workBookContent.Add(sheetContent);
            }

            return workBookContent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadExcelText(string fileName)
        {
            string ExcelCellSeparator = "";//ConfigurationManager.AppSettings["ExcelCellSeparator"];
            string ExcelRowSeparator = ""; //ConfigurationManager.AppSettings["ExcelRowSeparator"];
            string ExcelSheetSeparator = "";//ConfigurationManager.AppSettings["ExcelSheetSeparator"];
            //
            List<List<List<string>>> excelContent = ReadExcel(fileName);
            string fileText = string.Empty;
            StringBuilder sbFileText = new StringBuilder();
            //循环处理WorkBook中的各Sheet页
            List<List<List<string>>>.Enumerator enumeratorWorkBook = excelContent.GetEnumerator();
            while (enumeratorWorkBook.MoveNext())
            {

                //循环处理当期Sheet页中的各行
                List<List<string>>.Enumerator enumeratorSheet = enumeratorWorkBook.Current.GetEnumerator();
                while (enumeratorSheet.MoveNext())
                {

                    string[] rowContent = enumeratorSheet.Current.ToArray();
                    sbFileText.Append(string.Join(ExcelCellSeparator, rowContent));
                    sbFileText.Append(ExcelRowSeparator);
                }
                sbFileText.Append(ExcelSheetSeparator);
            }
            //
            fileText = sbFileText.ToString();
            return fileText;
        }

        /// <summary>
        /// 读取Word内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadWordText(string fileName)
        {
            string WordTableCellSeparator = "";// ConfigurationManager.AppSettings["WordTableCellSeparator"];
            string WordTableRowSeparator = "";// ConfigurationManager.AppSettings["WordTableRowSeparator"];
            string WordTableSeparator = "";//ConfigurationManager.AppSettings["WordTableSeparator"];
            //
            string CaptureWordHeader = "";//ConfigurationManager.AppSettings["CaptureWordHeader"];
            string CaptureWordFooter = "";//ConfigurationManager.AppSettings["CaptureWordFooter"];
            string CaptureWordTable = "";// ConfigurationManager.AppSettings["CaptureWordTable"];
            string CaptureWordImage = "true";// ConfigurationManager.AppSettings["CaptureWordImage"];
            //
            string CaptureWordImageFileName = @"D:\Data\test\"+"{0}";// ConfigurationManager.AppSettings["CaptureWordImageFileName"];
            //
            string fileText = string.Empty;
            StringBuilder sbFileText = new StringBuilder();

            #region 打开文档
            XWPFDocument document = null;
            try
            {
                using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    document = new XWPFDocument(file);
                }
            }
            catch (Exception e)
            {
                //LogHandler.LogWrite(string.Format("文件{0}打开失败，错误：{1}", new string[] { fileName, e.ToString() }));
            }
            #endregion

            #region 页眉、页脚
            //页眉
            if (CaptureWordHeader == "true")
            {
                sbFileText.AppendLine("Capture Header Begin");
                foreach (XWPFHeader xwpfHeader in document.HeaderList)
                {
                    sbFileText.AppendLine(string.Format("{0}", new string[] { xwpfHeader.Text }));
                }
                sbFileText.AppendLine("Capture Header End");
            }
            //页脚
            if (CaptureWordFooter == "true")
            {
                sbFileText.AppendLine("Capture Footer Begin");
                foreach (XWPFFooter xwpfFooter in document.FooterList)
                {
                    sbFileText.AppendLine(string.Format("{0}", new string[] { xwpfFooter.Text }));
                }
                sbFileText.AppendLine("Capture Footer End");
            }
            #endregion

            #region 表格
            if (CaptureWordTable == "true")
            {
                sbFileText.AppendLine("Capture Table Begin");
                foreach (XWPFTable table in document.Tables)
                {
                    //循环表格行
                    foreach (XWPFTableRow row in table.Rows)
                    {
                        foreach (XWPFTableCell cell in row.GetTableCells())
                        {
                            sbFileText.Append(cell.GetText());
                            //
                            sbFileText.Append(WordTableCellSeparator);
                        }

                        sbFileText.Append(WordTableRowSeparator);
                    }
                    sbFileText.Append(WordTableSeparator);
                }
                sbFileText.AppendLine("Capture Table End");
            }
            #endregion

            #region 图片
            if (CaptureWordImage == "true")
            {
                sbFileText.AppendLine("Capture Image Begin");
                foreach (XWPFPictureData pictureData in document.AllPictures)
                {
                    string picExtName = pictureData.SuggestFileExtension();
                    string picFileName = pictureData.FileName;
                    byte[] picFileContent = pictureData.Data;
                    //
                    string picTempName = string.Format(CaptureWordImageFileName, new string[] { Guid.NewGuid().ToString() + "_" + picFileName + "." + picExtName });
                    //
                    using (FileStream fs = new FileStream(picTempName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(picFileContent, 0, picFileContent.Length);
                        fs.Close();
                    }
                    //
                    sbFileText.AppendLine(picTempName);
                }
                sbFileText.AppendLine("Capture Image End");
            }
            #endregion

            //正文段落
            sbFileText.AppendLine("Capture Paragraph Begin");
            foreach (XWPFParagraph paragraph in document.Paragraphs)
            {
                sbFileText.AppendLine(paragraph.ParagraphText);

            }
            sbFileText.AppendLine("Capture Paragraph End");
            //

            //
            fileText = sbFileText.ToString();
            return fileText;
        }


    }
}
