using System;
namespace SharedKit
{
	// 支持继承的单例类
	/*
	 *	 子类要泛型
		 public class Two : SingletonExample<Two> {
			public void Show() {
				Console.WriteLine("Two Sub class.......");
			}
		 }
		 调用 Two.getInstance().Show();
	 */
	public class BDSharedSingletonClass<T> where T : class, new()
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		private static T _instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE0090 // Use 'new(...)'
		private static readonly object syslock = new object();
#pragma warning restore IDE0090 // Use 'new(...)'

		public static T Instance()
		{
			//线程安全锁
			if (_instance == null) {
				lock (syslock) {
					if (_instance == null) {
						_instance = new T();
					}
				}
			}
			return _instance;
		}
	}

	internal class BDChildSingletonClass : BDSharedSingletonClass<BDChildSingletonClass>
	{
#pragma warning disable CA1822 // Mark members as static
		public void Show()
		{
			Console.WriteLine("This is a Sub Singleton Class.");
		}
#pragma warning restore CA1822 // Mark members as static
	}
}
