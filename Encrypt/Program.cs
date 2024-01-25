using System;
using System.IO;
using System.Security.Cryptography;

namespace AesEncryption
{
    class Program
    {
        private static string EncryptDataWithAes(string plainText, string keyBase64, out string vectorBase64)
        {
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                aesAlgorithm.GenerateIV();
                Console.WriteLine($"Aes Cipher Mode : {aesAlgorithm.Mode}");
                Console.WriteLine($"Aes Padding Mode: {aesAlgorithm.Padding}");
                Console.WriteLine($"Aes Key Size : {aesAlgorithm.KeySize}");

                vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);

                ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();

                byte[] encryptedData;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encryptedData = ms.ToArray();
                    }
                }

                return Convert.ToBase64String(encryptedData);
            }
        }
        private static string DecryptDataWithAes(string cipherText, string keyBase64, string vectorBase64)
        {
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                aesAlgorithm.IV = Convert.FromBase64String(vectorBase64);

                Console.WriteLine($"Aes Cipher Mode : {aesAlgorithm.Mode}");
                Console.WriteLine($"Aes Padding Mode: {aesAlgorithm.Padding}");
                Console.WriteLine($"Aes Key Size : {aesAlgorithm.KeySize}");
                Console.WriteLine($"Aes Block Size : {aesAlgorithm.BlockSize}");

                ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

                byte[] cipher = Convert.FromBase64String(cipherText);

                using (MemoryStream ms = new MemoryStream(cipher))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
        public static string ReadFileData(string filePath)
        {
            try
            {
                string data = File.ReadAllText(filePath);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                return null;
            }
        }
        public static void SaveToFile(string content, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save file: " + ex.Message);
            }
        }
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please provide a file path.");
                Console.WriteLine("Encrypt.exe <ShellcodeFilePath> <PathToSaveFiles>");
                return;
            }

            string filePath = args[0];
            string directoryPath = args[1];
            string payload = ReadFileData(filePath);

            if (payload != null)
            {
                Console.WriteLine("Data read.");
            }
            else
            {
                Console.WriteLine("Failed to read the file.");
            }
            // CHANGE KEY IF NEEDED
            var key = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Console.WriteLine("Welcome to the Aes Encryption Test tool");
            Console.WriteLine("Provide the Aes Key in base64 format :");
            string keyBase64 = Convert.ToBase64String(key);
            Console.WriteLine("--------------------------------------------------------------");
            string cipherText = EncryptDataWithAes(payload, keyBase64, out string vectorBase64);

            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("Payload:");
            Console.WriteLine(cipherText);
            string cipherTextPath = Path.Combine(directoryPath, "cipherText.txt");
            SaveToFile(cipherText, cipherTextPath);

            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("Here is the Aes IV in Base64:");
            Console.WriteLine(vectorBase64);
            string vectorBase64Path = Path.Combine(directoryPath, "vectorBase64.txt");
            SaveToFile(vectorBase64, vectorBase64Path);

            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("Here is the Aes key in Base64:");
            Console.WriteLine(keyBase64);
            string keyBase64Path = Path.Combine(directoryPath, "keyBase64.txt");
            SaveToFile(keyBase64, keyBase64Path);

            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("Files saved successfully:");
            Console.WriteLine($"- Cipher Text: {cipherTextPath}");
            Console.WriteLine($"- Vector Base64: {vectorBase64Path}");
            Console.WriteLine($"- Key Base64: {keyBase64Path}");
        }
    }
}
