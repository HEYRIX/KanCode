using System;
using System.Net.Mail;
using MimeKit;
//using MimeKit;
//using MimeKit.Text;

namespace SharedOffice
{
	public enum BDMailOption
	{
		None,
		Sender,
		Receiver,
	}

	//public class BDMailSender
	//{
	//	public String title { get; set; }
	//	public String mail { get; set; }
	//	public String token { get; set; }
	//}

	public class BDMailItem
	{
		public BDMailOption option;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public String title { get; set; }
		public String mail { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	}

	public class BDMailContent
	{
		public String Sender { get; set; }
		public List<String> Receivers { get; set; }
		public String Subject { get; set; }
		public String Content { get; set; }
		// File Path list for attachments
		public List<String> Attachments { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public BDMailContent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			this.Receivers = new List<string>();
			this.Subject = "";
			this.Content = "";
			this.Attachments = new List<string>();
		}
	}

	public class BDMailUtils
	{
		// https://www.cnblogs.com/rocketRobin/p/8337055.html
		private BDMailUtils()
		{
		}

		public static void SendMail(List<BDMailItem> eMailSet, string eSubject, string eContent)
		{
			var eMailSender = "ai1699@qq.com";
			var message = new MimeKit.MimeMessage();
			message.From.Add(new MimeKit.MailboxAddress("DevMoss", eMailSender));
			foreach (var item in eMailSet) {
				switch (item.option) {
					case BDMailOption.Sender:
						//message.From.Add(new MimeKit.MailboxAddress(item.title, eMailSender));
						//eMailSender = item.mail;
						System.Diagnostics.Debug.Assert(item.option != BDMailOption.None);
						break;
					case BDMailOption.Receiver:
						message.To.Add(new MimeKit.MailboxAddress(item.title, item.mail));
						break;
					case BDMailOption.None:
					default:
						System.Diagnostics.Debug.Assert(item.option != BDMailOption.None);
						break;
				}
			}

			//eSubject = "测试报告";
			//eContent = "<h1>测试报告标题</h1><h2>测试报告内容</h2>";
			message.Subject = eSubject;
			var body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Html) {
				//Text = "<h1>标题部分</h1><h2>内容部分</h2>",
				Text = eContent,
			};

#pragma warning disable IDE0028 // Simplify collection initialization
			var multipart = new MimeKit.Multipart("mixed");
#pragma warning restore IDE0028 // Simplify collection initialization
			multipart.Add(body);
			message.Body = multipart;

			// https://service.mail.qq.com/cgi-bin/help?subtype=1&&id=28&&no=331
			using (var client = new MailKit.Net.Smtp.SmtpClient()) {
				client.Connect("smtp.qq.com", 587, false);
				// Note: only needed if the SMTP server requires authentication
				var eMailKey = "lifsdbgtzkyzcbca";
				client.Authenticate(eMailSender, eMailKey); // lifsdbgtzkyzcbca rgigiymyzmwlbjab
				client.Send(message);
				client.Disconnect(true);
			}
			SharedKit.BDSharedUtils.LogOut("Send Mail Successfully ...");
		}

		public static void SendMail(List<BDMailItem> eMailSet, string eSubject, string eContent, List<String> eAttachments)
		{
			if (null != eAttachments && eAttachments.Count > 0) {
				foreach (var fpath in eAttachments) {
					if (!File.Exists(fpath)) {
						throw new FileNotFoundException("Mail.Attachment.path NOT found.", fpath);
					}
				}
			}

			var eMailSender = "ai1699@qq.com";
			var message = new MimeKit.MimeMessage();
			message.From.Add(new MimeKit.MailboxAddress("DevMoss", eMailSender));
			foreach (var item in eMailSet) {
				switch (item.option) {
					case BDMailOption.Sender:
						//message.From.Add(new MimeKit.MailboxAddress(item.title, eMailSender));
						//eMailSender = item.mail;
						System.Diagnostics.Debug.Assert(item.option != BDMailOption.None);
						break;
					case BDMailOption.Receiver:
						message.To.Add(new MimeKit.MailboxAddress(item.title, item.mail));
						break;
					case BDMailOption.None:
					default:
						System.Diagnostics.Debug.Assert(item.option != BDMailOption.None);
						break;
				}
			}

			message.Subject = eSubject;
			var body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Html) {
				Text = eContent,
			};

#pragma warning disable IDE0028 // Simplify collection initialization
			var multipart = new MimeKit.Multipart("mixed");
#pragma warning restore IDE0028 // Simplify collection initialization
			multipart.Add(body);
			// http://www.mimekit.net/docs/html/Creating-Messages.htm
			if (null != eAttachments && eAttachments.Count > 0) {
				foreach (var fpath in eAttachments) {
					if (File.Exists(fpath)) {
						// TODO Memory Release
						// https://www.cnblogs.com/rocketRobin/p/8337055.html
						// create an image attachment for the file located at path
						var attachment = new MimePart("image", "gif") {
							Content = new MimeContent(File.OpenRead(fpath), ContentEncoding.Default),
							ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
							ContentTransferEncoding = ContentEncoding.Base64,
							FileName = Path.GetFileName(fpath)
						};
						multipart.Add(attachment);
					}
				}
			}
			message.Body = multipart;

			// https://service.mail.qq.com/cgi-bin/help?subtype=1&&id=28&&no=331
			using (var client = new MailKit.Net.Smtp.SmtpClient()) {
				client.Connect("smtp.qq.com", 587, false);
				// Note: only needed if the SMTP server requires authentication
				var eMailKey = "lifsdbgtzkyzcbca";
				client.Authenticate(eMailSender, eMailKey); // lifsdbgtzkyzcbca rgigiymyzmwlbjab
				client.Send(message);
				client.Disconnect(true);
			}
			SharedKit.BDSharedUtils.LogOut("Send Mail Successfully ...");
		}
	}
}
