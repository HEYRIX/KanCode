using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace SharedKit
{
	public class BDProtectDataUtils
	{
		public BDProtectDataUtils() {
		}
	}
}

// Ref ProtectData in Chrome
// https://catgirl.is/posts/how-to-read-encrypted-google-chrome-cookies-in-c
//namespace Integrative.Encryption
namespace SharedKit
{
	interface IProtector
	{
		byte[] Protect(byte[] userData, byte[] optionalEntrypy, DataProtectionScope scope);
		byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope);
	}

	public static class CrossProtect
	{
		readonly static IProtector _protector = CreateProtector();

		private static IProtector CreateProtector() {
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				return new DpapiWrapper();
			} else {
				return new AspNetWrapper();
			}
		}

		public static byte[] Protect(byte[] userData, byte[] optionalEntropy, DataProtectionScope scope) {
			return _protector.Protect(userData, optionalEntropy, scope);
		}

		public static byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope) {
			return _protector.Unprotect(encryptedData, optionalEntropy, scope);
		}
	}

	class AspNetWrapper : IProtector
	{
		private const string AppName = "CrossProtect";
		private const string BaseName = "CrossProtected_";
		private static readonly byte[] _emptyBytes = new byte[0];

		public byte[] Protect(byte[] userData, byte[] optionalEntropy, DataProtectionScope scope) {
			optionalEntropy = optionalEntropy ?? _emptyBytes;
			var protector = GetProtector(scope, optionalEntropy);
			return protector.Protect(userData);
		}

		public byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope) {
			optionalEntropy ??= _emptyBytes;
			var protector = GetProtector(scope, optionalEntropy);
			return protector.Unprotect(encryptedData);
		}

		private /*IDataProtector*/static IDataProtector GetProtector(DataProtectionScope scope, byte[] optionalEntropy) {
			return (scope == DataProtectionScope.CurrentUser) ? GetUserProtector(optionalEntropy) : GetMachineProtector(optionalEntropy);
		}

		private /*IDataProtector*/static IDataProtector GetUserProtector(byte[] optionalEntropy) {
			var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var path = Path.Combine(appData, AppName);
			var info = new DirectoryInfo(path);
			var provider = DataProtectionProvider.Create(info);
			var purpose = CreatePurpose(optionalEntropy);
			return provider.CreateProtector(purpose);
		}

		private /*IDataProtector*/static IDataProtector GetMachineProtector(byte[] optionalEntropy) {
			var provider = DataProtectionProvider.Create(AppName);
			var purpose = CreatePurpose(optionalEntropy);
			return provider.CreateProtector(purpose);
		}

		private static string CreatePurpose(byte[] optionalEntropy) { 
			var result = BaseName + Convert.ToBase64String(optionalEntropy);
			return Uri.EscapeDataString(result);
		}
	}

	sealed class DpapiWrapper : IProtector
	{
		public byte[] Protect(byte[] userData, byte[] optionalEntropy, DataProtectionScope scope) {
			return ProtectedData.Protect(userData, optionalEntropy, scope);
		}

		public byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope) {
			return ProtectedData.Unprotect(encryptedData, optionalEntropy, scope);
		}
	}
}
