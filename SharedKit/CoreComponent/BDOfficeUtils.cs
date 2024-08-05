using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace SharedKit
{
	// NPOI操作Excel
	// https://www.cnblogs.com/hao-1234-1234/p/14283241.html
	// https://www.cnblogs.com/Can-daydayup/p/12501400.html
	public partial class BDOfficeUtils
	{
		private BDOfficeUtils() { }

		/// <summary>
		/// 读取IWorkbook
		/// </summary>
		//public NPOI.SS.UserModel.IWorkbook ReadWorkbook = null;

		/// <summary>
		/// 获取读取 WorkBook
		/// </summary>
		public static NPOI.SS.UserModel.IWorkbook GetBook(String filePath, FileAccess fileOption)
		{
			NPOI.SS.UserModel.IWorkbook book = null;
			string fileExt = System.IO.Path.GetExtension(filePath);
			var fileStream = new FileStream(filePath, FileMode.Open, /*FileAccess.ReadWrite*/fileOption);
			// 把xls写入workbook中 2003版本
			if (fileExt.Equals(".xls")) {
				book = new HSSFWorkbook(fileStream);
			} else if (fileExt.Equals(".xlsx")) {
				// 把xlsx 写入workbook中 2007版本
				book = new XSSFWorkbook(fileStream);
			} else {
				book = null;
			}
			fileStream.Close();
			return book;
		}

		/// <summary>
		/// 获取表中的Sheet名称
		/// </summary>
		//public List<ISheet> Sheets = null;

		/// <summary>
		/// 获取所有 Sheet表
		/// </summary>
		public static List<ISheet> GetSheets(NPOI.SS.UserModel.IWorkbook book)
		{
			var sheets = new List<ISheet>();
			var sheetCount = book.NumberOfSheets;
			for (int i = 0; i < sheetCount; i++) {
				sheets.Add(book.GetSheetAt(i));
			}
			return sheets;
		}

		public static ISheet GetSheet(NPOI.SS.UserModel.IWorkbook book, String name)
		{
			// 1. 通过Sheet 名获取 Sheet
			//int sheetIndex = book.GetSheet(name);
			// 2.a通过Sheet 名获取 Sheet数组对应的下标；
			var sheetIndex = book.GetSheetIndex(name);
			// 2.b 通过 Sheet下标获取 对应的 Sheet 数据
			var sheet = book.GetSheetAt(sheetIndex);
			return sheet;
		}

		/// <summary>
		/// 获取 Sheet 表数据
		/// </summary>
		/// <param name="sheet"></param>
		public static DataTable GetSheetContent(ISheet sheet)
		{
			// https://www.cnblogs.com/hao-1234-1234/p/14283241.html
			//if (Sheet == default) {
			//	return null;
			//}
			IRow row;
			// 1. 获取行数
			var rowCount = sheet.LastRowNum;

			// 从第四行(下标为3)开始获取数据，前三行是表头
			// 如果从第一行开始，则i=0就可以了
			var dataTable = new System.Data.DataTable();
			for (int i = 3; i <= rowCount; i++) {
				// 获取具体行
				row = sheet.GetRow(i);
				if (row != null) {
					// 2. 获取行对应的列数
					var column = row.LastCellNum;
					for (int j = 0; j < column; j++) {
						// 3. 获取某行某列对应的单元格数据
						var cellValue = row.GetCell(j).ToString();
						// 4. 输出单元格数据        
						Console.WriteLine(cellValue + " ");
					}
					// 换行
					//Console.WriteLine();
				}
			}
			return dataTable;
		}

		/// <summary>
		/// 修改 Field Sheet
		/// </summary>
		/// <param name="SheetName"></param>
		/// <returns></returns>
		public static void UpdateSheet(String filePath, String SheetName)
		{
			var book = GetBook(filePath, FileAccess.ReadWrite);
			var fsWrite = new FileStream(filePath, FileMode.Open, FileAccess.Write);
			try {
				// 1. 通过Sheet名 获取对应的ISeet--其中 ReadWorkbook 为读取Excel文档时获取
				var sheet = book.GetSheet(SheetName);
				// 2. 获取行数
				int rowCount = sheet.LastRowNum;
				for (int i = 0; i < rowCount; i++) {
					// 3. 获取行对应的列数
					int columnount = sheet.GetRow(i).LastCellNum;
					for (int j = 0; j < columnount; j++) {
						// 4. 获取某行某列对应的单元格数据
						// 其中前三行设计为表头，故i+3，这里可以自己定义
						var sheetCellValue = sheet.GetRow(i + 3).GetCell(j);
						// 5. 向单元格传值，以覆盖对应的单元格数据
						sheetCellValue.SetCellValue(sheetCellValue + "Update");
					}
				}
                // 6. 对 Workbook 的修改写入文件流，对文件进行相应操作
                book.Write(fsWrite, false);
			} catch (Exception ex) {
				throw ex;
			} finally {
				// 7. 关闭文件流：报不报错都关闭
				fsWrite.Close();
			}
		}

		/// <summary>
		/// 修改 Field Sheet
		/// </summary>
		/// <param name="SheetName"></param>
		/// <returns></returns>
		//public static void UpdateSheet(string SheetName, string FilePath, int RowCount)
		//{
		//	// 创建文件流
		//	var fsWrite = new FileStream(FilePath, FileMode.Open, FileAccess.Write);
		//	try {
		//		// 1. 通过Sheet名 获取对应的ISeet--其中 ReadWorkbook 为读取Excel文档时获取
		//		var sheet = ReadWorkbook.GetSheet(SheetName);
		//		// 2. 获取行对应的列数
		//		int column = sheet.GetRow(RowCount).LastCellNum;
		//		for (int j = 0; j < column; j++) {
		//			// 3. 获取某行某列对应的单元格数据
		//			// 其中前三行设计为表头，故i+3，这里可以自己定义
		//			var sheetCellValue = sheet.GetRow(RowCount).GetCell(j);
		//			// 4. 向单元格传值，以覆盖对应的单元格数据
		//			sheetCellValue.SetCellValue(sheetCellValue + "Update");
		//		}
		//		// 5. 对 Workbook 的修改写入文件流，对文件进行相应操作
		//		ReadWorkbook.Write(fsWrite);
		//	} catch (Exception ex) {
		//		throw ex;
		//	} finally {
		//		// 7. 关闭文件流：报不报错都关闭
		//		fsWrite.Close();
		//	}
		//}

		/// <summary>
		/// 删除其中一个Sheet
		/// Bug:删除后，无Sheet表存在BUG
		/// </summary>
		/// <param name="SheetName"></param>
		/// <param name="FilePath"></param>
		/// <returns></returns>
		public static bool RemoveSheet(NPOI.SS.UserModel.IWorkbook book, string SheetName, String filePath)
		{
			var ret = false;
			var fsWrite = new FileStream(filePath, FileMode.Open, FileAccess.Write);
			try {
				// 1. 通过Sheet名字查找Sheet下标
				var sheetIndex = book.GetSheetIndex(SheetName);
				if (sheetIndex >= 0) {
					// 2. 通过Sheet下标移除 Sheet
					book.RemoveSheetAt(sheetIndex);
					// 3. 对 Workbook 的修改写入文件流，对文件进行相应操作
					book.Write(fsWrite, false);
					ret = true;
				}
				return ret;
			} catch (Exception ex) {
				throw ex;
			} finally {
				// 4. 关闭文件流：报不报错都关闭
				fsWrite.Close();
			}
		}
	}

	public partial class BDOfficeUtils
	{
		/// <summary>
		/// Stream读取.csv文件
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public static DataTable OpenCSV(string filePath)
		{
			CoreComponent.BDPathUtils.EnsurePathReadyIfNeed(filePath);
			var dt = new DataTable();
			var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read); // default utf8
			var sr = new StreamReader(fs, System.Text.Encoding.Default);

			string strLine = ""; // 记录每次读取的一行记录
			string[] lineSplitArray; // 记录每行记录中的各字段内容
			int headColumnCount = 0; // 标示列数
			bool isHeadReadable = true; // 标示是否是读取的第一行

			// 逐行读取CSV中的数据
			while ((strLine = sr.ReadLine()) != null) {
				lineSplitArray = strLine.Split(',');
				//if (lineSplitArray.Length != 4) {
				//	Console.WriteLine("");
				//}
				if (isHeadReadable) {
					isHeadReadable = false;
					headColumnCount = lineSplitArray.Length;
					for (int i = 0; i < lineSplitArray.Length; i++) {
						var dc = new DataColumn(lineSplitArray[i]);
						dt.Columns.Add(dc);
					}
				} else {
					DataRow dr = dt.NewRow();
					for (int j = 0; j < headColumnCount; j++) {
						var cell = (headColumnCount > lineSplitArray.Length) ? "" : lineSplitArray[j];
						dr[j] = cell;
					}
					dt.Rows.Add(dr);
				}
			}
			sr.Close();
			fs.Close();
			return dt;
		}
	}

	public class NpoiExcelExportHelper
	{
		private static NpoiExcelExportHelper _exportHelper;
		public static NpoiExcelExportHelper _ {
			get => _exportHelper ?? (_exportHelper = new NpoiExcelExportHelper());
			set => _exportHelper = value;
		}

		/// <summary>
		/// TODO 先创建行，然后创建对应的列
		/// 创建Excel中指定的行
		/// </summary>
		/// <param name="sheet">Excel工作表对象</param>
		/// <param name="rowNum">创建第几行(从0开始)</param>
		/// <param name="rowHeight">行高</param>
		public static HSSFRow CreateRow(ISheet sheet, int rowNum, float rowHeight)
		{
			HSSFRow row = (HSSFRow)sheet.CreateRow(rowNum); //创建行
			row.HeightInPoints = rowHeight; //设置列头行高
			return row;
		}

		/// <summary>
		/// 创建行内指定的单元格
		/// </summary>
		/// <param name="row">需要创建单元格的行</param>
		/// <param name="cellStyle">单元格样式</param>
		/// <param name="cellNum">创建第几个单元格(从0开始)</param>
		/// <param name="cellValue">给单元格赋值</param>
		/// <returns></returns>
		public static HSSFCell CreateCells(HSSFRow row, HSSFCellStyle cellStyle, int cellNum, string cellValue)
		{
			HSSFCell cell = (HSSFCell)row.CreateCell(cellNum); //创建单元格
			cell.CellStyle = cellStyle; //将样式绑定到单元格
			if (!string.IsNullOrWhiteSpace(cellValue)) {
				//单元格赋值
				cell.SetCellValue(cellValue);
			}
			return cell;
		}

        /// <summary>
        /// 行内单元格常用样式设置
        /// </summary>
        /// <param name="workbook">Excel文件对象</param>
        /// <param name="hAlignment">水平布局方式</param>
        /// <param name="vAlignment">垂直布局方式</param>
        /// <param name="fontHeightInPoints">字体大小</param>
        /// <param name="isAddBorder">是否需要边框</param>
        /// <param name="boldWeight">字体加粗 (None = 0,Normal = 400，Bold = 700</param>
        /// <param name="fontName">字体（仿宋，楷体，宋体，微软雅黑...与Excel主题字体相对应）</param>
        /// <param name="isAddBorderColor">是否增加边框颜色</param>
        /// <param name="isItalic">是否将文字变为斜体</param>
        /// <param name="isLineFeed">是否自动换行</param>
        /// <param name="isAddCellBackground">是否增加单元格背景颜色</param>
        /// <param name="fillPattern">填充图案样式(FineDots 细点，SolidForeground立体前景，isAddFillPattern=true时存在)</param>
        /// <param name="cellBackgroundColor">单元格背景颜色（当isAddCellBackground=true时存在）</param>
        /// <param name="fontColor">字体颜色</param>
        /// <param name="underlineStyle">下划线样式（无下划线[None],单下划线[Single],双下划线[Double],会计用单下划线[SingleAccounting],会计用双下划线[DoubleAccounting]）</param>
        /// <param name="typeOffset">字体上标下标(普通默认值[None],上标[Sub],下标[Super]),即字体在单元格内的上下偏移量</param>
        /// <param name="isStrikeout">是否显示删除线</param>
        /// <returns></returns>
        [Obsolete]
        public HSSFCellStyle CreateStyle(HSSFWorkbook workbook, HorizontalAlignment hAlignment, VerticalAlignment vAlignment, short fontHeightInPoints, bool isAddBorder, short boldWeight, string fontName = "宋体", bool isAddBorderColor = true, bool isItalic = false, bool isLineFeed = false, bool isAddCellBackground = false, FillPattern fillPattern = FillPattern.NoFill, short cellBackgroundColor = HSSFColor.Yellow.Index, short fontColor = HSSFColor.Black.Index, FontUnderlineType underlineStyle =
			FontUnderlineType.None, FontSuperScript typeOffset = FontSuperScript.None, bool isStrikeout = false)
		{
			HSSFCellStyle cellStyle = (HSSFCellStyle)workbook.CreateCellStyle(); //创建列头单元格实例样式
			cellStyle.Alignment = hAlignment; //水平居中
			cellStyle.VerticalAlignment = vAlignment; //垂直居中
			cellStyle.WrapText = isLineFeed;//自动换行

			//背景颜色，边框颜色，字体颜色都是使用 HSSFColor属性中的对应调色板索引，关于 HSSFColor 颜色索引对照表，详情参考：https://www.cnblogs.com/Brainpan/p/5804167.html
			//TODO 引用了NPOI后可通过ICellStyle 接口的 FillForegroundColor 属性实现 Excel 单元格的背景色设置，FillPattern 为单元格背景色的填充样式
			//TODO 注意 要设置单元格背景色必须是FillForegroundColor和FillPattern两个属性同时设置，否则是不会显示背景颜色
			if (isAddCellBackground) {
				cellStyle.FillForegroundColor = cellBackgroundColor;//单元格背景颜色
				cellStyle.FillPattern = fillPattern;//填充图案样式(FineDots 细点，SolidForeground立体前景)
			}

			//是否增加边框
			if (isAddBorder) {
				//常用的边框样式 None(没有),Thin(细边框，瘦的),Medium(中等),Dashed(虚线),Dotted(星罗棋布的),Thick(厚的),Double(双倍),Hair(头发)[上右下左顺序设置]
				cellStyle.BorderBottom = BorderStyle.Thin;
				cellStyle.BorderRight = BorderStyle.Thin;
				cellStyle.BorderTop = BorderStyle.Thin;
				cellStyle.BorderLeft = BorderStyle.Thin;
			}

			//是否设置边框颜色
			if (isAddBorderColor) {
				//边框颜色[上右下左顺序设置]
				cellStyle.TopBorderColor = HSSFColor.DarkGreen.Index;//DarkGreen(黑绿色)
				cellStyle.RightBorderColor = HSSFColor.DarkGreen.Index;
				cellStyle.BottomBorderColor = HSSFColor.DarkGreen.Index;
				cellStyle.LeftBorderColor = HSSFColor.DarkGreen.Index;
			}

			/**
             * 设置相关字体样式
             */
			var cellStyleFont = (HSSFFont)workbook.CreateFont(); //创建字体

			//假如字体大小只需要是粗体的话直接使用下面该属性即可
			//cellStyleFont.IsBold = true;
			cellStyleFont.Boldweight = boldWeight; //字体加粗
			cellStyleFont.FontHeightInPoints = fontHeightInPoints; //字体大小
			cellStyleFont.FontName = fontName;//字体（仿宋，楷体，宋体 ）
			cellStyleFont.Color = fontColor;//设置字体颜色
			cellStyleFont.IsItalic = isItalic;//是否将文字变为斜体
			cellStyleFont.Underline = underlineStyle;//字体下划线
			cellStyleFont.TypeOffset = typeOffset;//字体上标下标
			cellStyleFont.IsStrikeout = isStrikeout;//是否有删除线

			cellStyle.SetFont(cellStyleFont); //将字体绑定到样式
			return cellStyle;
		}
	}
}