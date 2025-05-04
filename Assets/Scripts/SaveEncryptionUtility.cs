using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveEncryptionUtility
{ 
    private static readonly string EncryptionKey = "b9w8T3yLf0q3Xv9sA6M1D2c7Kz4Nh2Qr";
    // 32 caractères pour AES-256

    public static void SaveEncrypted(string path, string json)
    {
        try
        {
            byte[] encryptedData = EncryptStringToBytes(json, EncryptionKey);
            File.WriteAllBytes(path, encryptedData);
            Debug.Log($"Encrypted save written to: {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save encrypted data: {ex.Message}");
        }
    }

    public static string LoadDecrypted(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning("Encrypted save file not found.");
                return null;
            }

            byte[] encryptedData = File.ReadAllBytes(path);
            string decryptedJson = DecryptStringFromBytes(encryptedData, EncryptionKey);
            return decryptedJson;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load or decrypt data: {ex.Message}");
            return null;
        }
    }

    public static byte[] EncryptStringToBytes(string plainText, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.GenerateIV();
        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using MemoryStream ms = new();
        ms.Write(aes.IV, 0, aes.IV.Length); // prepend IV
        using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
        using StreamWriter sw = new(cs);
        sw.Write(plainText);
        return ms.ToArray();
    }

    public static string DecryptStringFromBytes(byte[] cipherData, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);

        byte[] iv = new byte[aes.BlockSize / 8];
        Array.Copy(cipherData, iv, iv.Length);

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);

        using MemoryStream ms = new(cipherData, iv.Length, cipherData.Length - iv.Length);
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using StreamReader sr = new(cs);
        return sr.ReadToEnd();
    }
}
