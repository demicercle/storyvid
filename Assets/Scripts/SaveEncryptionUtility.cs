// Copyright 2023 Hakim Bawa
// Licensed under the MIT License.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Summary: Simple local encryption that is based on the UDID of the device, which returns a base64 string that can be stored
// anywhere (e.g. PlayerPrefs). This can ensure that the encrypted data cannot be used on another device 
// without both the UDID of the original device, and the private key. It can be useful
// for storing, for example, a list of in-app purchases. 
// Note that this is far from a fool-proof method of encryption, as the keys are hardcoded here, and so on the device. 
// Any malicious user could reverse-engineer/decompile the code and get access to the keys. This is mostly intended as 
// a first level of encryption. Server-side encryption is always the secure way to go.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Device;

public static class EncryptionService {
		private static byte[] salt = Encoding.UTF8.GetBytes("your-salt");
		private static int iterations = 10;
		private static int keySize = 256;
		private static int blockSize = 128;

		private static readonly string _defaultDeviceId = SystemInfo.deviceUniqueIdentifier;
		
		public static string EncryptWithDeviceId<T>(T data, string deviceId = null)
		{
			if (string.IsNullOrEmpty(deviceId)) {
				deviceId = _defaultDeviceId;
			}
			var jsonData = JsonConvert.SerializeObject(data);

			using var ms = new MemoryStream();
			using (var aes = GetAes(deviceId))
			{
				// Generate a random IV
				using var rng = new RNGCryptoServiceProvider();
				var iv = aes.IV;
				rng.GetBytes(iv);
				aes.IV = iv;

				// Write the IV to the output stream
				ms.Write(iv, 0, iv.Length);

				using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
				{
					var dataBytes = Encoding.UTF8.GetBytes(jsonData);
					cs.Write(dataBytes, 0, dataBytes.Length);
					cs.FlushFinalBlock();
				}
			}

			var encryptedData = ms.ToArray();
			return Convert.ToBase64String(encryptedData);
		}

		public static T DecryptWithDeviceId<T>(string base64EncryptedData, string deviceId = null)
		{
			if (string.IsNullOrEmpty(deviceId)) {
				deviceId = _defaultDeviceId;
			}
			var encryptedData = Convert.FromBase64String(base64EncryptedData);
			using var ms = new MemoryStream(encryptedData);

			using var aes = GetAes(deviceId);

			// Read the IV from the input stream
			var iv = aes.IV;
			// ReSharper disable once MustUseReturnValue
			ms.Read(iv, 0, iv.Length);
			aes.IV = iv;

			using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
			using var sr = new StreamReader(cs);

			var jsonData = sr.ReadToEnd();
			T data = JsonConvert.DeserializeObject<T>(jsonData);
			return data;
		}

		private static Aes GetAes(string deviceId)
		{
			var aes = Aes.Create();
			aes.Mode = CipherMode.CBC;
			aes.KeySize = keySize;
			aes.BlockSize = blockSize;
			aes.Key = GenerateKey(deviceId);
			aes.IV = new byte[blockSize / 8];

			return aes;
		}

		private static byte[] GenerateKey(string deviceId)
		{
			using var rfc2898DeriveBytes = new Rfc2898DeriveBytes(deviceId, salt, iterations);
			return rfc2898DeriveBytes.GetBytes(keySize / 8);
		}
	}