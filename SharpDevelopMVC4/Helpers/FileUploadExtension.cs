using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

public static class FileUploadExtension
{
    public const string FOLDER = "UploadedFiles";

    /// <summary>
    /// Convert the file upload to byte array. [BernardGabon.com]
    /// </summary>
    /// <param name="File">HttpPostedFileBase</param>
    /// <returns>byte[] data</returns>
    public static byte[] ToFileByteArray(this HttpPostedFile File)
    {
        try
        {
            byte[] file = new byte[File.ContentLength];
            File.InputStream.Read(file, 0, File.ContentLength);
            return file;
        }
        catch
        {
            return null;
        }

    }

    /// <summary>
    /// Save the uploaded file to a folder. [BernardGabon.com]
    /// </summary>
    /// <param name="File">Filename</param>
    /// <param name="strFileName">Filename</param>
    /// <param name="strFolder">Folder</param>
    /// <returns>string Filename</returns>
    public static string SaveToFolder(this HttpPostedFile File, string strFileName = "", string strFolder = "")
    {
        try
        {
            var file = ToFileByteArray(File);

            string folder = string.IsNullOrEmpty(strFolder) ? HttpContext.Current.Server.MapPath("~/" + FOLDER) : HttpContext.Current.Server.MapPath("~/" + strFolder);
            string filename = string.IsNullOrEmpty(strFileName) ? Path.GetFileNameWithoutExtension(File.FileName) : strFileName;
            string filenameExt = filename + "_" + GenerateUniqueChars() + Path.GetExtension(File.FileName);
            string path = Path.Combine(folder, filenameExt);

            System.IO.Directory.CreateDirectory(folder);
            System.IO.File.WriteAllBytes(path, file);

            return filenameExt;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string GenerateUniqueChars()
    {
        char[] padding = { '=' };
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd(padding).Replace('+', '-').Replace('/', '_'); ;
    }

}
