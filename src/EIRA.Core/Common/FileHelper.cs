using EIRA.Dto;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EIRA.Common
{
    public class FileHelper
    {
        #region http上傳文件

        /// <summary>
        /// http上傳文件
        /// </summary>
        /// <param name="File">文件流</param>
        /// <param name="folder">文件夹</param>
        /// <returns></returns>
        public static string UploadFile(HttpPostedFile File, string folder)
        {
            try
            {
                //隨機生成文件夾（由於同一個檔案下如果名字相同的文件會覆蓋）
                string newFolder = DateTime.Now.GetCode();

                var dPath = System.AppDomain.CurrentDomain.BaseDirectory + folder + "/" + newFolder;
                if (!Directory.Exists(dPath))
                {
                    Directory.CreateDirectory(dPath);
                }

                //文件存放路径
                string Path = dPath + "/" + File.FileName;

                File.SaveAs(Path);
                return folder + "/" + newFolder + "/" + File.FileName;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #endregion

        #region 導出表格

        public string Export(List<ExportQuestionnairesDto> list)
        {
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Control Risk Questionnaire");
            int iNum = 1;//去除兩行標題行

            //創建標題單元格樣式
            ICellStyle fTCStyle = workbook.CreateCellStyle();
            IFont tfont = workbook.CreateFont();
            tfont.IsBold = true;
            fTCStyle.SetFont(tfont);
            fTCStyle.Alignment = HorizontalAlignment.Center;//居中;

            #region 定義列
            //第一行
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("Entity");
            row.CreateCell(1).SetCellValue("BU");
            row.CreateCell(2).SetCellValue("Question type ID");
            row.CreateCell(3).SetCellValue("Question type");
            row.CreateCell(4).SetCellValue("Qesition ID");
            row.CreateCell(5).SetCellValue("Question");
            row.CreateCell(6).SetCellValue("Highest rating");
            row.CreateCell(7).SetCellValue("MC answer");
            row.CreateCell(8).SetCellValue("Recommended Score");
            row.CreateCell(9).SetCellValue("Free text answer");
            row.CreateCell(10).SetCellValue("No. of supporting document(s)");

            List<ICell> clist = row.Cells;
            foreach (var cItem in clist)
            {
                cItem.CellStyle = fTCStyle;
                sheet.SetColumnWidth(cItem.ColumnIndex, 30 * 165);
            }
            sheet.SetColumnWidth(clist.Count - 1, 30 * 240);
            #endregion

            #region 填充數據
            foreach (var item in list)
            {
                IRow r = sheet.CreateRow(iNum);
                r.CreateCell(0).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(0).SetCellValue(item.Entity);
                r.CreateCell(1).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(1).SetCellValue(item.BU);
                r.CreateCell(2).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(2).SetCellValue(item.QuestionTypeId);
                r.CreateCell(3).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(3).SetCellValue(item.QuestionType);
                r.CreateCell(4).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(4).SetCellValue(item.QuestionId);
                r.CreateCell(5).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(5).SetCellValue(item.Question);
                r.CreateCell(6).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(6).SetCellValue(item.HighestRating);
                r.CreateCell(7).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(7).SetCellValue(item.MCAnswer);
                r.CreateCell(8).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(8).SetCellValue(item.RecommendedScore);
                r.CreateCell(9).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(9).SetCellValue(item.FreeTextAnswer);
                r.CreateCell(10).CellStyle.Alignment = HorizontalAlignment.Center;
                r.CreateCell(10).SetCellValue(item.NomberOfSupportingDocument);

                iNum++;
            }
            #endregion

            var savePath = AppDomain.CurrentDomain.BaseDirectory + "/ExportReport";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            var fileName = "/Carbon_Report_" + "_" + DateTime.Now.GetCode() + ".xls";
            var file = savePath + fileName;
            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (FileStream f = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                f.Write(buf, 0, buf.Length);
                f.Flush();
                f.Close();
            }
            stream.Close();
            stream.Dispose();
            workbook = null;
            return "ExportReport" + fileName;
        }

        #endregion

        /// <summary>
        /// 生成文字頭像
        /// </summary>
        /// <param name="str">文字內容</param>
        /// <returns></returns>
        public static string GenerateImage(string str)
        {
            try
            {
                Bitmap bmp = new Bitmap(1600, 1600);      //定义画布大小
                Graphics g = Graphics.FromImage(bmp);      //封装一个GDI+绘图图面
                g.Clear(ColorTranslator.FromHtml("#f5f5f5"));  //背景色
                g.SmoothingMode = SmoothingMode.AntiAlias; //使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                var format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;  // 更正： 垂直居中
                format.Alignment = StringAlignment.Center;      // 水平居中

                Rectangle lbRect = new Rectangle(0, 0, 1600, 1600);
                g.DrawString(str, new Font("微软雅黑", 400, FontStyle.Bold), Brushes.DeepSkyBlue, lbRect, format);//画图

                var dPath = System.AppDomain.CurrentDomain.BaseDirectory + "HeadPortrait/";
                if (!Directory.Exists(dPath))
                {
                    Directory.CreateDirectory(dPath);
                }
                var fileName = DateTime.Now.ToFileTime() + ".jpg";
                var strFullName = dPath + fileName;  //存储位置+图片名
                bmp.Save(strFullName, ImageFormat.Jpeg);    //以指定的格式保存图片文件
                return "HeadPortrait/" + fileName;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}