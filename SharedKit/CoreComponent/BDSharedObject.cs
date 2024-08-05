using System;

namespace SharedKit
{
	public interface IBDSharedSerializedItem
	{
	}

	public interface IBDSharedObject
	{
	}

	public class BDSharedItem : IBDSharedObject
	{
		// 自行定义的唯一识别逻辑Key。
		//[System.ComponentModel.DataAnnotations.Schema.Column("DatKey")]
		//public String DatKey { get; set; }
		//[System.ComponentModel.DataAnnotations.Schema.NotMapped]
		//public Dictionary<string, string>? bakup { get; set; }

		public BDSharedItem()
		{
			//this.datKey = SharedKit.BDSharedUtils.DateCodeKey();
		}
	}

	public class BDSharedObject : IBDSharedObject
	{
		public BDSharedObject()
		{
		}
	}
}

