using System;

namespace SharedKit.SecurityComponent
{
	public enum MD5Length
	{
		L16 = 16,
		L32 = 32
	}

	public enum RsaSize
	{
		R2048 = 2048,
		R3072 = 3072,
		R4096 = 4096,
	}

	public enum RsaKeyType
	{
		XML,
		JSON,
	}

	internal class RSAParametersJson
	{
		//Public key Modulus
		public string Modulus { get; set; }
		//Public key Exponent
		public string Exponent { get; set; }
		public string P { get; set; }
		public string Q { get; set; }
		public string DP { get; set; }
		public string DQ { get; set; }
		public string InverseQ { get; set; }
		public string D { get; set; }
	}

	public class RSAKey
	{
		/// <summary>
		/// Rsa public key
		/// </summary>
		public string PublicKey { get; set; }

		/// <summary>
		/// Rsa private key
		/// </summary>
		public string PrivateKey { get; set; }

		/// <summary>
		/// Rsa public key Exponent
		/// </summary>
		public string Exponent { get; set; }

		/// <summary>
		/// Rsa public key Modulus
		/// </summary>
		public string Modulus { get; set; }
	}

	public class AESKey
	{
		/// <summary>
		/// ase key
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// ase IV
		/// </summary>
		public string IV { get; set; }
	}

	public class BDSecurityConstant
	{
		public BDSecurityConstant()
		{
		}
	}
}

