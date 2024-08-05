using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using NPOI.OpenXmlFormats.Wordprocessing;
using SharedKit.CoreComponent;

namespace SharedKit
{
	// TODO
	// https://blog.poychang.net/note-sqlite/
	// https://learn.microsoft.com/en-us/ef/core/providers/sqlite/?tabs=dotnet-core-cli
	// https://stackoverflow.com/questions/53473747/check-if-a-table-exists-using-ef-core-2-1
	// https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.entityframeworkcore?view=efcore-6.0
	// 
	// [Keyless]
	// The entity type 'BDGardenCandidateItem' requires a primary key to be defined. If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating'. For more information on keyless entity types, see https://go.microsoft.com/fwlink/?linkid=2141943.
	// https://docs.microsoft.com/en-us/ef/core/modeling/keyless-entity-types?tabs=data-annotations
	// https://stackoverflow.com/questions/15381233/can-we-have-table-without-primary-key-in-entity-framework
	public class BDSharedStorageContext : DbContext
	{
#pragma warning disable IDE1006 // Naming Styles
		// In SharedContext
		// DatKey as Text, KeyID as Integer. 
		public string dbTitle = "storage.db";
        public string dbPath { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public BDSharedStorageContext() {
			this.dbPath = "";
		}

		public void InitAll()
		{
			//var folder = Environment.SpecialFolder.LocalApplicationData;
			//var path = Environment.GetFolderPath(folder);
			var path = Directory.GetCurrentDirectory();
			path = BDSharedUtils.SharedDirPath();
			dbPath = InitPath(path, dbTitle);
			System.Diagnostics.Debug.Assert(null != dbPath && dbPath.Trim().Length > 0);

			// TODO SQLite Error 1: 'table "DBSet" already exists'.
			// How to get Table Name of mapped entity in Entity Framework Core
			// https://stackoverflow.com/questions/45667126/how-to-get-table-name-of-mapped-entity-in-entity-framework-core/45671666#45671666
			// https://stackoverflow.com/questions/33911316/entity-framework-core-how-to-check-if-database-exists
			// (context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()
			var dbChecked = ((RelationalDatabaseCreator)this.Database.GetService<IDatabaseCreator>()).Exists();
			// NetCore 3.1, To check if a database exists and can be contacted. 
			// dbContext.Database.CanConnect(); 
			_ = this.Database.EnsureCreated();
			this.Database.Migrate();

			// https://stackoverflow.com/questions/38532764/create-tables-on-runtime-in-ef-core
			try {
				RelationalDatabaseCreator dbCreator = (RelationalDatabaseCreator)this.Database.GetService<IDatabaseCreator>();
				dbCreator.EnsureCreated();
				var isChecked = dbCreator.HasTables();
				dbCreator.CreateTables();
			} catch (Exception ex) {
				System.Console.WriteLine(ex.Message);
			}
		}

		//public void ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
		//{
		//	services.AddDbContext<BloggingContext>(options =>
		//		   //options.UseSqlite($"Data Source={_hostEnvironment.ContentRootPath}/data.db"));
		//		   options.UseSqlite($"Data Source={this.dbPath}"));
		//}

		//public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		//{
		//    using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
		//    {
		//        var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		//        context.Database.EnsureCreated();
		//    }
		//}

		// The following configures EF to create a Sqlite database file in the
		// special "local" folder for your platform.
		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			base.OnConfiguring(options);
			options.UseSqlite($"Data Source={this.dbPath}");
		}

		//https://stackoverflow.com/questions/62449078/what-is-the-alternate-for-addorupdate-method-in-ef-core
		//public virtual void AddOrUpdate(T entity)
		//{
		//    if (entity == null)
		//        throw new ArgumentNullException("entity");

		//    this.DbContext.Update(entity);
		//    this.DbContext.SaveChanges();
		//}

		public static String InitPath(String dbDirPath, String dbTitle)
        {
			System.Diagnostics.Debug.Assert(null != dbTitle && dbTitle.Trim().Length > 0);
            System.Diagnostics.Debug.Assert(null != dbDirPath && dbDirPath.Trim().Length > 0);
            var ret = System.IO.Path.Join(dbDirPath, dbTitle);
			return ret;
		}

		public string GetTable(Type clrEntityType)
		{
			// https://stackoverflow.com/questions/42455279/how-to-get-column-name-and-corresponding-database-type-from-dbcontext-in-entity
			//var dbContext = new BDSharedStorageContext();
			//var entityType = dbContext.Model.FindEntityType(typeof(BDChineseCultureItem));
			var entityType = this.Model.FindEntityType(clrEntityType);
			// Table info 
#pragma warning disable CS8604 // Possible null reference argument.
			var tableName = entityType.GetTableName();
#pragma warning restore CS8604 // Possible null reference argument.

			//var tableSchema = entityType.GetSchema();

			// Column info 
			//foreach (var property in entityType.GetProperties()) {
			//	var columnName = property.GetColumnName();
			//	var columnType = property.GetColumnType();
			//};
			return tableName;
		}
	}
}

