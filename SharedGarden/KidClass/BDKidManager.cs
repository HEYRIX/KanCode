using System;
using SharedKit;

namespace SharedGarden
{
	public class BDKidItem
	{
		public String Name { get; set; }
		public String Gender { get; set; }

		public String? School { get; set; }
		public String GradeClass { get; set; }		
		public String ReClassCode { get; set; }
		// Registration Number
		public String ReStateCode { get; set; }  
	}

	public class BDKidManager
	{
		private BDKidManager()
		{
		}

		public static void RunDiffDetail(string totalPath, string findPath)
		{
			// lhsPath IS All Kids Info set, read-only.
			// rhsPath IS In-hand Info set.
			var dirPath = "/Users/KOKOAR/HEYRIX/hkContext/KidContext/KidClass";
			var filePath0 = System.IO.Path.Join(dirPath, "KidClass.xlsx");
			if (!File.Exists(filePath0)) {
				BDSharedUtils.LogOut($"File {filePath0} NOT Found.");
				return;
			}

			var filePath1 = System.IO.Path.Join(dirPath, "HandInSet.xlsx");
			if (!File.Exists(filePath0)) {
				BDSharedUtils.LogOut($"File {filePath1} NOT Found.");
				return;
			}

			var totalKidArray = GetKidArray(filePath0);
			var datArray = DiffItems(filePath0, filePath1);
		}

		internal static List<String> DiffItems(string lhsPath, string rhsPath)
		{
			var datArray = new List<String>();
			{
				//var lhsBook = BDOfficeUtils.GetBook(lhsPath, FileAccess.Read);
				//var lhsSheetArray = SharedKit.BDOfficeUtils.GetSheets(lhsBook);				
				//var lhsSheet = lhsSheetArray[0]; // zero-based sheet index
				//var lhsRowCount = lhsSheet.LastRowNum;
				var lhsKidArray = GetKidArray(lhsPath);

				var rhsBook = BDOfficeUtils.GetBook(rhsPath, FileAccess.Read);
				var rhsSheetArray = SharedKit.BDOfficeUtils.GetSheets(rhsBook);
				// Sheet Index
				var rhsSheet = rhsSheetArray[1]; // zero-based sheet index
				var rhsKidArray = new List<String>();
				for (int i = 0; i <= rhsSheet.LastRowNum; i++) {
					var row = rhsSheet.GetRow(i); // Column Index
					if (row != null) {
						var nameColumn = 1;
						var valCell1 = (null == row.GetCell(nameColumn)) ? "" : row.GetCell(nameColumn).ToString().Trim();
						rhsKidArray.Add(valCell1);
					}
				}

				foreach (var item in lhsKidArray) {
					if (rhsKidArray.Contains(item.Name)) {
					} else {
						datArray.Add(item.Name);
					}
				}
			}
			return datArray;
		}

		internal static List<BDKidItem> GetKidArray(string lhsPath)
		{
			var datArray = new List<BDKidItem>();
			{
				var lhsBook = BDOfficeUtils.GetBook(lhsPath, FileAccess.Read);
				var lhsSheetArray = SharedKit.BDOfficeUtils.GetSheets(lhsBook);
				var lhsSheet = lhsSheetArray[0]; // zero-based sheet index
				var lhsRowCount = lhsSheet.LastRowNum;
				var lhsKidArray = new List<String>();
				for (int i = 1/*0*/; i <= lhsSheet.LastRowNum; i++) {
					var row = lhsSheet.GetRow(i);
					if (row != null) {
						
						var valCell0 = (null == row.GetCell(0)) ? "" : row.GetCell(0).ToString().Trim();
						var valCell1 = (null == row.GetCell(1)) ? "" : row.GetCell(1).ToString().Trim(); // Name
						var valCell2 = (null == row.GetCell(2)) ? "" : row.GetCell(2).ToString().Trim();
						var valCell3 = (null == row.GetCell(3)) ? "" : row.GetCell(3).ToString().Trim();
						var valCell4 = (null == row.GetCell(4)) ? "" : row.GetCell(4).ToString().Trim();
						var kidInfo = new BDKidItem() {
							GradeClass = valCell0,
							Name = valCell1,							
							Gender = valCell2,
							ReClassCode = valCell3,
							ReStateCode= valCell4,
						};
						lhsKidArray.Add(kidInfo.Name);
						datArray.Add(kidInfo);
					}
				}
			}
			return datArray;
		}
	}
}

