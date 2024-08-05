using System;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using SharedKit;

namespace SharedGarden
{
	internal class BDGardenStorageContext : BDSharedStorageContext
	{
		private Microsoft.EntityFrameworkCore.DbSet<BDGreenGardenInviteItem>? GreenGardenBidInviteSet { get; set; }
		private Microsoft.EntityFrameworkCore.DbSet<BDGreenGardenCandidateItem>? GreenGardenBidCandidateSet { get; set; }
		private Microsoft.EntityFrameworkCore.DbSet<BDGreenGardenResultItem>? GreenGardenBidResultSet { get; set; }

		private Microsoft.EntityFrameworkCore.DbSet<BDGreenCityInviteItem>? GreenCityBidInviteSet { get; set; }
		private Microsoft.EntityFrameworkCore.DbSet<BDGreenCityCandidateItem>? GreenCityBidCandidateSet { get; set; }
		private Microsoft.EntityFrameworkCore.DbSet<BDGreenCityResultItem>? GreenCityBidResultSet { get; set; }

		private Microsoft.EntityFrameworkCore.DbSet<BDGreenKeepInviteItem>? GreenKeepBidInviteSet { get; set; }
		private Microsoft.EntityFrameworkCore.DbSet<BDGreenKeepCandidateItem>? GreenKeepBidCandidateSet { get; set; }
		private Microsoft.EntityFrameworkCore.DbSet<BDGreenKeepResultItem>? GreenKeepBidResultSet { get; set; }

		private Microsoft.EntityFrameworkCore.DbSet<BDBidGardenDataItem>? BidDataSet { get; set; }
		private Microsoft.EntityFrameworkCore.DbSet<BDBidGardenSideItem>? BidSideSet { get; set; }
		//private Microsoft.EntityFrameworkCore.DbSet<BDBidGardenMarkItem>? BidMarkSet { get; set; }

		public BDGardenStorageContext()
		{
			//this.dbTitle = "HiveStorage.db";
			var path = BDSharedUtils.SharedDirPath();
			this.dbPath = BDSharedStorageContext.InitPath(path, dbTitle);
			this.InitAll();
		}

        #region Required
        protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<BDGardenCandidateItem>().HasNoKey();
            base.OnModelCreating(modelBuilder);

			//modelBuilder.Entity<BDGardenCandidateItem>().HasKey(
			//	t => new { t.DatKey, t.strGongShiRiQiStart, t.title, t.strZhaoBiaoOption, t.datUrl, /*t.zbKey, t.fileTitle,*/ t.datCategory, }
			//);
			//modelBuilder.Entity<BDGardenCandidateItem>().Ignore(t => t.bakup);

			// https://learn.microsoft.com/zh-cn/ef/core/modeling/value-conversions?tabs=data-annotations
			modelBuilder.Entity<BDBidGardenDataItem>().Property(e => e.BidCategory).HasConversion<int>();
			//modelBuilder.Entity<BDBidGardenDataItem>().Property(e => e.BidCategory).HasConversion(
			//	v => v.ToString(),
			//	v => (BDBidCategory)Enum.Parse(typeof(BDBidCategory), v));
			modelBuilder.Entity<BDBidGardenDataItem>().Property(e => e.BidStep).HasConversion<int>();
			//modelBuilder.Entity<BDBidGardenDataItem>().Property(e => e.BidStep).HasConversion(
			//	v => v.ToString(),
			//	v => (BDBidStep)Enum.Parse(typeof(BDBidStep), v));
		}
        #endregion

        #region GreenGardenBidRegion
        public static int AddOrUpdate(List<BDGreenGardenInviteItem> items)
		{
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenGardenBidInviteSet;
			//var list = dbSet.ToList();
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					dbContext.Add(items[i]);
					ret++;
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				dbContext.SaveChanges();
			}
			dbContext.Dispose();
			return ret;
		}

		public static List<BDGreenGardenInviteItem> DiffSet(List<BDGreenGardenInviteItem> items)
		{
			// Find the new items and return
			var ret = new List<BDGreenGardenInviteItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenGardenBidInviteSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}

		// Invoke BDGardenStorageContext.Delete(datArrayChanged.ConvertAll(x => (BDGreenGardenCandidateItem)x));
		public static void DelItem(List<BDGreenGardenCandidateItem> items)
        {
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenGardenBidCandidateSet;
			//var list = dbSet.ToList();
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				var target = dbSet.Find(new object[] { items[i].DatKey, });
				if (null == /*dbSet.Find(new object[] { items[i].DatKey, })*/target) {
					//dbContext.Add(items[i]);
					//ret++;
				} else {
					var vv = items[i];					
					if (target.DatKey == items[i].DatKey) {
						dbContext.Entry(target).State = EntityState.Detached;
						dbContext.Remove(vv);
						ret++;
					}
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				dbContext.SaveChanges();
			}
			dbContext.Dispose();
		}

		public static int AddOrUpdate(List<BDGreenGardenCandidateItem> items, bool isUpdate = false)
		{
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenGardenBidCandidateSet;
			//var list = dbSet.ToList();
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				var target = dbSet.Find(new object[] { items[i].DatKey, });
				if (null == /*dbSet.Find(new object[] { items[i].DatKey, })*/target) {
					dbContext.Add(items[i]);
					ret++;
				} else {
					var vv = items[i];
					if (target.DatKey == items[i].DatKey && isUpdate && items[i].IsChanged) {
						// https://stackoverflow.com/questions/46657813/how-to-update-record-using-entity-framework-core
						// https://stackoverflow.com/questions/72351977/ef-core-update-an-existing-entity
						target.Update((BDGreenGardenCandidateItem)items[i]);
						dbContext.Entry(target).State = EntityState.Detached;
						dbContext.Update(items[i]);
						//dbContext.Update(items[i]);
						ret++;
					}
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				dbContext.SaveChanges();
			}
			dbContext.Dispose();
			return ret;
		}

		public static List<BDGreenGardenCandidateItem> DiffSet(List<BDGreenGardenCandidateItem> items)
		{
			// Find the new items and return
			var ret = new List<BDGreenGardenCandidateItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenGardenBidCandidateSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}

		public static int AddOrUpdate(List<BDGreenGardenResultItem> items)
		{
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenGardenBidResultSet;
			//var list = dbSet.ToList();
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					dbContext.Add(items[i]);
					ret++;
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				dbContext.SaveChanges();
			}
			dbContext.Dispose();
			return ret;
		}

		public static List<BDGreenGardenSharedItem> DiffSet(List<BDGreenGardenResultItem> items)
		{
			// Find the new items and return
			var ret = new List<BDGreenGardenSharedItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenGardenBidResultSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}
		#endregion

		#region GreenCityBidRegion
		public static int AddOrUpdate(List<BDGreenCityInviteItem> items)
		{
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenCityBidInviteSet;
			//var list = dbSet.ToList();
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					dbContext.Add(items[i]);
					ret++;
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				dbContext.SaveChanges();
			}
			dbContext.Dispose();
			return ret;
		}

		public static List<BDGreenCityInviteItem> DiffSet(List<BDGreenCityInviteItem> items)
		{
			// Find the new items and return
			var ret = new List<BDGreenCityInviteItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenCityBidInviteSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}

		public static int AddOrUpdate(List<BDGreenCityCandidateItem> items)
		{
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenCityBidCandidateSet;
			//var list = dbSet.ToList();
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					dbContext.Add(items[i]);
					ret++;
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				dbContext.SaveChanges();
			}
			dbContext.Dispose();
			return ret;
		}

		public static List<BDGreenCityCandidateItem> DiffSet(List<BDGreenCityCandidateItem> items)
		{
			// Find the new items and return
			var ret = new List<BDGreenCityCandidateItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenCityBidCandidateSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}

		public static int AddOrUpdate(List<BDGreenCityResultItem> items)
		{
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenCityBidResultSet;
			//var list = dbSet.ToList();
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					dbContext.Add(items[i]);
					ret++;
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				dbContext.SaveChanges();
			}
			dbContext.Dispose();
			return ret;
		}

		public static List<BDGreenCityResultItem> DiffSet(List<BDGreenCityResultItem> items)
		{
			// Find the new items and return
			var ret = new List<BDGreenCityResultItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenCityBidResultSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}
		#endregion

		#region GreenKeepBidRegion
		public static int AddOrUpdate(List<BDGreenKeepInviteItem> items)
		{
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenKeepBidInviteSet;
			//var list = dbSet.ToList();
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					dbContext.Add(items[i]);
					ret++;
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				dbContext.SaveChanges();
			}
			dbContext.Dispose();
			return ret;
		}

		public static List<BDGreenKeepInviteItem> DiffSet(List<BDGreenKeepInviteItem> items)
		{
			// Find the new items and return
			var ret = new List<BDGreenKeepInviteItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenKeepBidInviteSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}

		public static int AddOrUpdate(List<BDGreenKeepCandidateItem> items)
		{
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenKeepBidCandidateSet;
			//var list = dbSet.ToList();
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					dbContext.Add(items[i]);
					ret++;
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				dbContext.SaveChanges();
			}
			dbContext.Dispose();
			return ret;
		}

		public static List<BDGreenKeepCandidateItem> DiffSet(List<BDGreenKeepCandidateItem> items)
		{
			// Find the new items and return
			var ret = new List<BDGreenKeepCandidateItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenKeepBidCandidateSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}

		public static int AddOrUpdate(List<BDGreenKeepResultItem> items)
		{
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenKeepBidResultSet;
			//var list = dbSet.ToList();
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					dbContext.Add(items[i]);
					ret++;
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				dbContext.SaveChanges();
			}
			dbContext.Dispose();
			return ret;
		}

		public static List<BDGreenKeepResultItem> DiffSet(List<BDGreenKeepResultItem> items)
		{
			// Find the new items and return
			var ret = new List<BDGreenKeepResultItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenKeepBidResultSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}
		#endregion

		#region BidDataInfoRegion
		public static int AddOrUpdate(List<BDBidGardenDataItem> items)
		{
			var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.BidDataSet;
			// "The database operation was expected to affect 1 row(s), but actually affected 0 row(s);
			// data may have been modified or deleted since entities were loaded.
			// See http://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions."
			// https://learn.microsoft.com/zh-cn/ef/core/saving/transactions
			// https://learn.microsoft.com/zh-cn/ef/core/saving/concurrency?tabs=data-annotations
			// https://learn.microsoft.com/zh-tw/dotnet/api/microsoft.entityframeworkcore.entitystate?view=efcore-6.0	
			// https://stackoverflow.com/questions/70095949/the-instance-of-entity-type-cannot-be-tracked-because-another-instance-with-the
			// https://learn.microsoft.com/en-us/answers/questions/181032/having-trouble-with-foreign-key-in-entity-cant-tra						
			// https://github.com/dotnet/efcore/issues/28822
			// https://stackoverflow.com/questions/48202403/instance-of-entity-type-cannot-be-tracked-because-another-instance-with-same-key
			// https://stackoverflow.com/questions/30987806/dbset-attachentity-vs-dbcontext-entryentity-state-entitystate-modified
			// https://www.cnblogs.com/cdaniu/p/16746672.html TODO Read
			// using var transaction = dbContext.Database.BeginTransaction();           
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				// TODO Tracking() or AsNoTracking()
				// https://learn.microsoft.com/zh-tw/ef/core/querying/tracking?source=recommendations
				// https://learn.microsoft.com/zh-cn/ef/core/change-tracking/
				//var target = dbSet.AsNoTracking().FirstOrDefault(e => e.DatKey == items[i].DatKey);
				var target = dbSet.Find(new object[] { items[i].DatKey, });
				if (null == target) {
					dbContext.Add(items[i]);
					//dbSet.Add(items[i]);
					ret++;
				} else {
                    if (target.DatKey == items[i].DatKey && items[i].IsChanged) {
						// State.Modified or State.Detached or dbContext.Attach()
						// 经测试，State改变时，应该修改其内容，而不是对象整体赋值。
						// 不可直接将target = items[i];对象整体赋值，这会导致dbContext中的内容Tracking出现问题。						
						//target.BidName = items[i].BidName + $"{i}";
						//dbContext.Entry(target).State = EntityState.Modified;
						//dbContext.Update(target);

						// Case 1 Modified Mode
						//target.Update(items[i]); TODO
						//dbContext.Entry(target).State = EntityState.Modified; 						
						// Case 2 Detached Mode		
						dbContext.Entry(target).State = EntityState.Detached;
						//dbContext.Entry(target).DetectChanges();
						dbContext.Update(items[i]);
						//dbContext.Entry(target).DetectChanges();
						//dbSet.Update(items[i]);
						ret++;
                    }
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			if (ret > 0) {
				var modCount = dbContext.SaveChanges();
                if (modCount != ret) {
					Console.WriteLine($"dbContext save Real[{modCount}]/WANT[{ret}] items ...");
				}
				ret = modCount;
			}
			//transaction.Commit();
			dbContext.Dispose();

			Console.WriteLine($"dbContext save [{ret}] items ...");
			return ret;
		}

		public static List<BDBidGardenDataItem> DiffSet(List<BDBidGardenDataItem> items)
		{
			// Find the new items and return
			var ret = new List<BDBidGardenDataItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.BidDataSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
				//var tt = dbSet.Where(x => x.DatKey == items[i].DatKey);//.AsNoTracking(); //TODO
				//if (null == tt) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}		

		internal static List<BDBidGardenDataItem> GetNewItems(BDBidCategory category, BDBidStep step, int deltaDays, /*int tIndex,*/int tCount)
		{
			System.Diagnostics.Debug.Assert(/*tIndex >= 0 && */tCount >= 0 && deltaDays >= 0);
			
			var ret = new List<BDBidGardenDataItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.BidDataSet;
			// TODO 参数逻辑设计问题
			var dbArray = dbSet.Where(x => (/*TODO(null == x.BidBuiltKey) &&*/ (x.BidCategory == category || category == BDBidCategory.None) && (x.BidStep == step || step == BDBidStep.None))).ToArray();
			
			var loopCount = (tCount >= dbSet.Count()) ? dbSet.Count() : tCount;
			//dbArray.Reverse();
			foreach (var item in dbArray) {
				if (ret.Count() <= loopCount) {
					//if ((item.BidCategory == category || category == BDBidCategory.None) && (item.BidStep == step || step == BDBidStep.None)) {
					var isDaysIncluded = ((DateTime.Now - BDDateUtils.ToDate(item.BidRiQi, false)).Days <= deltaDays) ? true : false;
					if (isDaysIncluded) {
						ret.Add(item);
					}
					//}
				} else {
					break;
				}
			}
			dbContext.Dispose();
			ret.Reverse();
			return ret;
		}
		#endregion

		internal static List<BDBidGardenDataItem> GetReportItems(BDBidCategory category, BDBidStep step, int deltaDays, /*int tIndex,*/int tCount, bool isSidable)
		{
			Console.WriteLine($"dbContext is processing items include side data. Please wait for a moment ...");
			var ret = GetNewItems(category, step, deltaDays, tCount);			
			if (isSidable) {
				var datSideArray = GetSideArray(BDBidCategory.kGreenGarden, BDBidStep.BCandidate);
				foreach (var item in ret) {
					//var dItem = BDBidGardenDataItem.Init(item);
					var dItem = item;
					dItem.UpdateSideArray(datSideArray); //TODO TOO SLOWLY !!!
					dItem.IsChanged = false; // Copy Only
				}
			}
			return ret;
		}

		public static List<BDGreenGardenSharedItem> DiffSet(List<BDGreenGardenSharedItem> items)
		{
			// Find the new items and return
			var ret = new List<BDGreenGardenSharedItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.GreenCityBidResultSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}

		public static int AddOrUpdate(List<BDGreenGardenSharedItem> items, BDBidCategory category, BDBidStep step)
		{
			var ret = 0;
			var dArray = new List<BDGreenGardenSharedItem>();
            foreach (var tt in items) {
                if (tt.BidCategory == category && tt.BidStep == step) {
					dArray.Add(tt);
                }
            }

			var dbContext = new BDGardenStorageContext();		
			var diffArray = BDGardenStorageContext.DiffSet(dArray);
            if (diffArray.Count() > 0) {
				dbContext.AddRange(diffArray);
				ret = dbContext.SaveChanges();
			}
			dbContext.Dispose();
			//ret = diffArray.Count();
			return ret;
		}
		
		internal static List<BDGardenSharedItem> GetOldItems(BDBidCategory category, BDBidStep step)
		{
			// Find the new items and return
			var ret = new List<BDGardenSharedItem>();
			var dbContext = new BDGardenStorageContext();
			if (category == BDBidCategory.kGreenGarden) {
				if (step == BDBidStep.AInvite)    { ret.AddRange(dbContext.GreenGardenBidInviteSet.ToArray()); }
				if (step == BDBidStep.BCandidate) { ret.AddRange(dbContext.GreenGardenBidCandidateSet.ToArray()); }
				if (step == BDBidStep.CResult)    { ret.AddRange(dbContext.GreenGardenBidResultSet.ToArray()); }
			}
			if (category == BDBidCategory.kGreenCity) {
				if (step == BDBidStep.AInvite)    { ret.AddRange(dbContext.GreenCityBidInviteSet.ToArray()); }
				if (step == BDBidStep.BCandidate) { ret.AddRange(dbContext.GreenCityBidCandidateSet.ToArray()); }
				if (step == BDBidStep.CResult)    { ret.AddRange(dbContext.GreenCityBidResultSet.ToArray()); }
			}
			if (category == BDBidCategory.kGreenYangHu) {
				if (step == BDBidStep.AInvite)    { ret.AddRange(dbContext.GreenKeepBidInviteSet.ToArray()); }
				if (step == BDBidStep.BCandidate) { ret.AddRange(dbContext.GreenKeepBidCandidateSet.ToArray()); }
				if (step == BDBidStep.CResult)    { ret.AddRange(dbContext.GreenKeepBidResultSet.ToArray()); }
			}
			dbContext.Dispose();

			foreach (var item in ret) {
				item.BidCategory = category;
				item.BidStep = step;
			}			
			return ret;
		}

		internal static List<BDBidGardenDataItem> GetMirgrateItems(BDBidCategory category, BDBidStep step)
		{
			System.Diagnostics.Debug.Assert(category != BDBidCategory.None && step != BDBidStep.None);
			var ext1 = "";
            if (category == BDBidCategory.kGreenGarden) { ext1 = "A"; }
			if (category == BDBidCategory.kGreenCity) { ext1 = "B"; }
			if (category == BDBidCategory.kGreenYangHu) { ext1 = "C"; }
			var ext2 = "";
            if (step == BDBidStep.AInvite) { ext2 = "01"; }
			if (step == BDBidStep.BCandidate) { ext2 = "02"; }
			if (step == BDBidStep.CResult) { ext2 = "03"; }

			// Find the new items and return			
			var dbArray = new List<BDGardenSharedItem>();
			var dbContext = new BDGardenStorageContext(); 
			if (category == BDBidCategory.kGreenGarden) {
				if (step == BDBidStep.AInvite) { dbArray.AddRange(dbContext.GreenGardenBidInviteSet.ToArray()); }
				if (step == BDBidStep.BCandidate) { dbArray.AddRange(dbContext.GreenGardenBidCandidateSet.ToArray()); }
				if (step == BDBidStep.CResult) { dbArray.AddRange(dbContext.GreenGardenBidResultSet.ToArray()); }
			}
			if (category == BDBidCategory.kGreenCity) {
				if (step == BDBidStep.AInvite) { dbArray.AddRange(dbContext.GreenCityBidInviteSet.ToArray()); }
				if (step == BDBidStep.BCandidate) { dbArray.AddRange(dbContext.GreenCityBidCandidateSet.ToArray()); }
				if (step == BDBidStep.CResult) { dbArray.AddRange(dbContext.GreenCityBidResultSet.ToArray()); }
			}
			if (category == BDBidCategory.kGreenYangHu) {
				if (step == BDBidStep.AInvite) { dbArray.AddRange(dbContext.GreenKeepBidInviteSet.ToArray()); }
				if (step == BDBidStep.BCandidate) { dbArray.AddRange(dbContext.GreenKeepBidCandidateSet.ToArray()); }
				if (step == BDBidStep.CResult) { dbArray.AddRange(dbContext.GreenKeepBidResultSet.ToArray()); }
			}
			dbContext.Dispose();

			var ret = new List<BDBidGardenDataItem>();
			foreach (var item in dbArray) {
				item.DatKey = BDBidGardenDataItem.UpdateDatKey(item);// $"{item.DatKey}.{ext1}{ext2}";
				item.BidCategory = category;
				item.BidStep = step;
			}
			var datSideArray = GetSideArray(BDBidCategory.kGreenGarden, BDBidStep.BCandidate);
			foreach (var item in dbArray) {
				var dItem = BDBidGardenDataItem.Init(item);
				dItem.UpdateSideArray(datSideArray);
				dItem.IsChanged = false; // Copy Only
				ret.Add(dItem);
			}
			return ret;
		}

		internal static List<BDGardenSharedItem> HandleBidSummaryOut(List<BDGardenSharedItem> datArray, BDBidGardenObject bidObject)
		{
			var diffSet = new List<BDGardenSharedItem>();
			switch (bidObject.Category) {
				case BDBidCategory.kGreenGarden:
					switch (bidObject.Step) {
						case BDBidStep.AInvite:
							var datArray11 = datArray.ConvertAll<BDGreenGardenInviteItem>(tt => (BDGreenGardenInviteItem)tt);
							var diffSet11 = BDGardenStorageContext.DiffSet(datArray11);
							diffSet.AddRange(diffSet11);
							_ = BDGardenStorageContext.AddOrUpdate(datArray11);
							break;
						case BDBidStep.BCandidate:
							var datArray12 = datArray.ConvertAll<BDGreenGardenCandidateItem>(tt => (BDGreenGardenCandidateItem)tt );
							var diffSet12 = BDGardenStorageContext.DiffSet(datArray12);
							diffSet.AddRange(diffSet12);
							_ = BDGardenStorageContext.AddOrUpdate(datArray12);
							break;
						case BDBidStep.CResult:
							var datArray13 = datArray.ConvertAll<BDGreenGardenResultItem>(tt => (BDGreenGardenResultItem)tt);
							var diffSet13 = BDGardenStorageContext.DiffSet(datArray13);
							diffSet.AddRange(diffSet13);
							_ = BDGardenStorageContext.AddOrUpdate(datArray13);
							break;
						case BDBidStep.None:
						default:
							break;
					}
					break;
				case BDBidCategory.kGreenCity:
					switch (bidObject.Step) {
						case BDBidStep.AInvite:
							var datArray21 = datArray.ConvertAll<BDGreenCityInviteItem>(tt => (BDGreenCityInviteItem)tt);
							var diffSet21 = BDGardenStorageContext.DiffSet(datArray21);
							diffSet.AddRange(diffSet21);
							_ = BDGardenStorageContext.AddOrUpdate(datArray21);
							break;
						case BDBidStep.BCandidate:
							var datArray22 = datArray.ConvertAll<BDGreenCityCandidateItem>(tt => (BDGreenCityCandidateItem)tt);
							var diffSet22 = BDGardenStorageContext.DiffSet(datArray22);
							diffSet.AddRange(diffSet22);
							_ = BDGardenStorageContext.AddOrUpdate(datArray22);
							break;
						case BDBidStep.CResult:
							var datArray23 = datArray.ConvertAll<BDGreenCityResultItem>(tt => (BDGreenCityResultItem)tt);
							var diffSet23 = BDGardenStorageContext.DiffSet(datArray23);
							diffSet.AddRange(diffSet23);
							_ = BDGardenStorageContext.AddOrUpdate(datArray23);
							break;
						case BDBidStep.None:
						default:
							break;
					}
					break;
				case BDBidCategory.kGreenYangHu:
					switch (bidObject.Step) {
						case BDBidStep.AInvite:
							var datArray31 = datArray.ConvertAll<BDGreenKeepInviteItem>(tt => (BDGreenKeepInviteItem)tt);
							var diffSet31 = BDGardenStorageContext.DiffSet(datArray31);
							diffSet.AddRange(diffSet31);
							_ = BDGardenStorageContext.AddOrUpdate(datArray31);
							break;
						case BDBidStep.BCandidate:
							var datArray32 = datArray.ConvertAll<BDGreenKeepCandidateItem>(tt => (BDGreenKeepCandidateItem)tt);
							var diffSet32 = BDGardenStorageContext.DiffSet(datArray32);
							diffSet.AddRange(diffSet32);
							_ = BDGardenStorageContext.AddOrUpdate(datArray32);
							break;
						case BDBidStep.CResult:
							var datArray33 = datArray.ConvertAll<BDGreenKeepResultItem>(tt => (BDGreenKeepResultItem)tt);
							var diffSet33 = BDGardenStorageContext.DiffSet(datArray33);
							diffSet.AddRange(diffSet33);
							_ = BDGardenStorageContext.AddOrUpdate(datArray33);
							break;
						case BDBidStep.None:
						default:
							break;
					}
					break;
				case BDBidCategory.None:
				default:
					break;
			}
			SharedKit.BDSharedUtils.LogOut($"{BDGardenUtils.ShowText(bidObject)} DiffCount {diffSet.Count} of TotalCount {datArray.Count} ... ");
			return diffSet;
		}

		internal static List<BDBidGardenSideItem> GetSideArray(BDBidCategory category, BDBidStep step)
		{
			System.Diagnostics.Debug.Assert(category != BDBidCategory.None && step != BDBidStep.None);
			var ext1 = "";
			if (category == BDBidCategory.kGreenGarden) { ext1 = "A"; }
			if (category == BDBidCategory.kGreenCity) { ext1 = "B"; }
			if (category == BDBidCategory.kGreenYangHu) { ext1 = "C"; }
			var ext2 = "";
			if (step == BDBidStep.AInvite) { ext2 = "01"; }
			if (step == BDBidStep.BCandidate) { ext2 = "02"; }
			if (step == BDBidStep.CResult) { ext2 = "03"; }
			var mid = $"{ext1}{ext2}";
			// Find the new items and return
			var ret = new List<BDBidGardenSideItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.BidSideSet; var dbArray = dbSet.ToArray();		
			for (int i = 0; i < dbArray.Count(); i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				var tt = dbArray[i];
				if (tt.DatKey.Contains($".{mid}.")) {
					//var tt = dbSet.Where(x => x.DatKey == items[i].DatKey);//.AsNoTracking(); //TODO
					tt.UpdateSideKind(tt.BidSideText ?? "");
					ret.Add(tt);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}

		internal static List<BDBidGardenSideItem> DiffSet(List<BDBidGardenSideItem> items)
		{
			// Find the new items and return
			var ret = new List<BDBidGardenSideItem>();
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.BidSideSet;
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				if (null == dbSet.Find(new object[] { items[i].DatKey, })) {
					//var tt = dbSet.Where(x => x.DatKey == items[i].DatKey);//.AsNoTracking(); //TODO
					ret.Add(items[i]);
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
			dbContext.Dispose();
			return ret;
		}

		public static int AddOrUpdate(List<BDBidGardenSideItem> items)
		{
            //if (items.Count > 0) {
            //    items[0].DatKey = "20230230.135821.A02.SB01";
            //    items[0].IsChanged = true;
            //}

            var ret = 0;
			var dbContext = new BDGardenStorageContext();
			var dbSet = dbContext.BidSideSet;        
			for (int i = 0; i < items.Count; i++) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				//var target = dbSet.AsNoTracking().FirstOrDefault(e => e.DatKey == items[i].DatKey);				
				//var target = dbSet.FirstOrDefault(e => e.DatKey == items[i].DatKey );
				var target = dbSet.Find(new object[] { items[i].DatKey, });
#pragma warning restore CS8602 // Dereference of a possibly null reference.
				if (null == target) {
					dbContext.Add(items[i]);
					ret++;
				} else {
					if (target.DatKey == items[i].DatKey && items[i].IsChanged) {
                        //dbContext.Entry(target).State = EntityState.Detached;
						//dbContext.Entry(target).DetectChanges();
						target.UpdateContent(items[i]);
                        //dbContext.Update(target);
                        //dbSet.Update(items[i]);
                        //dbContext.Entry(target).DetectChanges();
                        ret++;
					}
				}
			}

			if (ret > 0) {
				ret = dbContext.SaveChanges();
            }
            //transaction.Commit();
            dbContext.Dispose();
			Console.WriteLine($"dbContext.{dbSet} save [{ret}] items ...");
			return ret;
		}
	}
}
