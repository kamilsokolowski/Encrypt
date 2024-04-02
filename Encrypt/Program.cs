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
        public static byte[] GenerateRandomBytes(int numberOfBytes)
        {
            byte[] randomBytes = new byte[numberOfBytes];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        public static string ReadFileData(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);

            string base64String = Convert.ToBase64String(fileBytes);

            return base64String;
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
                Console.WriteLine($"Insufficient arguments provided.{Environment.NewLine}Usage: Encrypt.exe <ShellcodeFilePath> <PathToSaveFiles>");
                return;
            }

            string filePath = args[0];
            string directoryPath = args[1];
            string payload = ReadFileData(filePath);

            if (!string.IsNullOrEmpty(payload))
            {
                Console.WriteLine("Data read successfully.");
            }
            else
            {
                Console.WriteLine("Failed to read the file. Please check the file path and permissions.");
            }
            // Generate a 16-byte random key
            byte[] key = GenerateRandomBytes(16);
            Console.WriteLine("Welcome to the AES Encryption Test Tool");

            // Convert the key to a Base64 string for display and use in encryption
            string keyBase64 = Convert.ToBase64String(key);
            Console.WriteLine("AES Key in Base64 format:");
            Console.WriteLine(keyBase64);

            // Divider for better readability
            Console.WriteLine(new string('-', 60));

            // Encrypt the payload using AES encryption
            string cipherText = EncryptDataWithAes(payload, keyBase64, out string vectorBase64);

            // Display the encrypted payload
            Console.WriteLine("Encrypted Payload:");
            Console.WriteLine(cipherText);

            // Save the encrypted payload to a file
            string cipherTextPath = Path.Combine(directoryPath, "cipherText.txt");
            SaveToFile(cipherText, cipherTextPath);

            // Divider for better readability
            Console.WriteLine(new string('-', 60));

            // Display the AES initialization vector (IV) in Base64 format
            Console.WriteLine("AES IV in Base64:");
            Console.WriteLine(vectorBase64);

            // Save the IV to a file
            string vectorBase64Path = Path.Combine(directoryPath, "vectorBase64.txt");
            SaveToFile(vectorBase64, vectorBase64Path);

            // Divider for better readability
            Console.WriteLine(new string('-', 60));

            // Re-display the AES key in Base64 format for confirmation
            Console.WriteLine("AES Key in Base64 (repeated for convenience):");
            Console.WriteLine(keyBase64);

            // Save the key to a file
            string keyBase64Path = Path.Combine(directoryPath, "keyBase64.txt");
            SaveToFile(keyBase64, keyBase64Path);

            // Divider for better readability
            Console.WriteLine(new string('-', 60));

            // Confirm the successful saving of all files
            Console.WriteLine("All files saved successfully:");
            Console.WriteLine($"- Cipher Text: {cipherTextPath}");
            Console.WriteLine($"- AES IV Base64: {vectorBase64Path}");
            Console.WriteLine($"- AES Key Base64: {keyBase64Path}");
        }
    }
}
