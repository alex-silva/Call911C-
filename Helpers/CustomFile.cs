using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Helpers
{
    public static class CustomFile
    {
        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open)) { }
            }
            catch (IOException e)
            {
                var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);

                return errorCode == 32 || errorCode == 33;
            }

            return false;
        }

        public static string ComputeHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                return Convert.ToBase64String(md5.ComputeHash(File.ReadAllBytes(filePath)));
            }
        }

        /// <summary>
        /// Verifies if the file exists, if so, adds the string "Copy" to the end of the file.
        /// </summary>
        /// <param name="pStrFilePath"></param>
        /// <returns></returns>
        public static string renameFileName(string pStrFilePath)
        {
            while (File.Exists(pStrFilePath))
            {
                string path = Path.GetDirectoryName(pStrFilePath);
                string fileName = Path.GetFileNameWithoutExtension(pStrFilePath);
                string fileExtension = Path.GetExtension(pStrFilePath);
                pStrFilePath = Path.Combine(path, fileName + " - Copy" + fileExtension);
            }
            return pStrFilePath;
        }

        /// <summary>
        /// Gets a byte[] from the complete path of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] getBytesFromFile(string filePath)
        {
            byte[] file;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    file = reader.ReadBytes((int)stream.Length);
                }
            }
            return file;
        }

        /// <summary>
        /// Saves a file in the destination dir/file from a byte[]
        /// </summary>
        /// <param name="pObjBytes"></param>
        /// <param name="destinationFilePath"></param>
        public static void getFileFromBytes(byte[] pObjBytes, string destinationFilePath)
        {
            var blob = pObjBytes.ToArray();
            using (var fs = new FileStream(destinationFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                fs.Write(blob, 0, blob.Length);
        }
    }
}
