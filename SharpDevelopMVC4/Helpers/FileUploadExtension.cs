using System;
using System.IO;
using System.Web;

public static class FileUploadExtension
{
    public const string FOLDER = "UploadedFiles";

    /// <summary>
    /// Convert the file upload to byte array. [BernardGabon.com]
    /// </summary>
    /// <param name="file">HttpPostedFileBase</param>
    /// <returns>byte[] data</returns>
    public static byte[] ToFileByteArray(this HttpPostedFileBase file)
    {
        if (file == null) return null;

        try
        {
            byte[] fileBytes = new byte[file.ContentLength];
            file.InputStream.Read(fileBytes, 0, file.ContentLength);
            return fileBytes;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Convert the file upload to byte array. [BernardGabon.com]
    /// </summary>
    /// <param name="file">HttpPostedFile</param>
    /// <returns>byte[] data</returns>
    public static byte[] ToFileByteArray(this HttpPostedFile file)
    {
        return file != null ? new HttpPostedFileWrapper(file).ToFileByteArray() : null;
    }

    /// <summary>
    /// Save the uploaded file to a folder. [BernardGabon.com]
    /// </summary>
    /// <param name="file">HttpPostedFileBase</param>
    /// <param name="strFileName">Filename</param>
    /// <param name="strFolder">Folder</param>
    /// <returns>string Filename</returns>
    public static string SaveToFolder(this HttpPostedFileBase file, string strFileName = "", string strFolder = "")
    {
        if (file == null) return string.Empty;

        try
        {
            var fileBytes = file.ToFileByteArray();
            if (fileBytes == null) return string.Empty;

            string folder = string.IsNullOrEmpty(strFolder) ? HttpContext.Current.Server.MapPath("~/" + FOLDER) : HttpContext.Current.Server.MapPath("~/" + strFolder);
            string filename = string.IsNullOrEmpty(strFileName) ? Path.GetFileNameWithoutExtension(file.FileName) : strFileName;
            string filenameExt = filename + "_" + GenerateUniqueChars() + Path.GetExtension(file.FileName);
            string path = Path.Combine(folder, filenameExt);

            Directory.CreateDirectory(folder);
            File.WriteAllBytes(path, fileBytes);

            return filenameExt;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Save the uploaded file to a folder. [BernardGabon.com]
    /// </summary>
    /// <param name="file">HttpPostedFile</param>
    /// <param name="strFileName">Filename</param>
    /// <param name="strFolder">Folder</param>
    /// <returns>string Filename</returns>
    public static string SaveToFolder(this HttpPostedFile file, string strFileName = "", string strFolder = "")
    {
        return file != null ? new HttpPostedFileWrapper(file).SaveToFolder(strFileName, strFolder) : string.Empty;
    }

    private static string GenerateUniqueChars()
    {
        char[] padding = { '=' };
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
    }
}
