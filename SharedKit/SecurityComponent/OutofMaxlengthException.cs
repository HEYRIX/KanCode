using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SharedKit.SecurityComponent
{
	/// <summary>
	/// The encrypt string out of max length exception
	/// </summary>
	public class OutofMaxlengthException : Exception
	{
		/// <summary>
		/// The max length of ecnrypt data
		/// </summary>
		public int MaxLength { get; private set; }

		/// <summary>
		/// Error message
		/// </summary>

		public string ErrorMessage { get; private set; }

		/// <summary>
		/// Rsa key size
		/// </summary>
		public int KeySize { get; private set; }

		/// <summary>
		/// Rsa padding
		/// </summary>
		public RSAEncryptionPadding RSAEncryptionPadding { get; private set; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="maxLength"></param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public OutofMaxlengthException(int maxLength, int keySize, RSAEncryptionPadding rsaEncryptionPadding)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			MaxLength = maxLength;
			KeySize = keySize;
			RSAEncryptionPadding = rsaEncryptionPadding;
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="maxLength"></param>
		public OutofMaxlengthException(string message, int maxLength, int keySize, RSAEncryptionPadding rsaEncryptionPadding) : this(maxLength, keySize, rsaEncryptionPadding)
		{
			ErrorMessage = message;
		}
	}
}
