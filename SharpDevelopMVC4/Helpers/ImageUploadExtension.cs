using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

public static class ImageUploadExtension
{
    private const int MAX_HEIGHT = 960; // default height in pixel
    private const bool QUALITY = true; // true = high quality, false = fast performance
    /// <summary>
    /// The folder name of where the uploaded images are stored. http://SERVER/FOLDER
    /// </summary>
    public const string FOLDER = "UploadedImages";
    /// <summary>
    /// The folder name of where the thumbnails are stored. http://SERVER/FOLDER/THUMBNAIL
    /// </summary>
    public const string THUMBNAIL = "thumb";
    private const int THUMBNAIL_WIDTH = 200; // Assign 0 to disable
    private const int THUMBNAIL_HEIGHT = 150; // Assign 0 to disable

    public static string ToBase64String(this byte[] imageByte)
    {
        return imageByte != null ? Convert.ToBase64String(imageByte) : string.Empty;
    }

    public static string ToBase64StringHTMLImgJpgSrc(this byte[] imageByte)
    {
        return imageByte != null ? "data:image/jpg;base64," + Convert.ToBase64String(imageByte) : string.Empty;
    }

    public static byte[] ToImageByteArray(this HttpPostedFileBase file, int maxHeight = MAX_HEIGHT, bool highQuality = QUALITY)
    {
        if (file == null || !file.ContentType.Contains("image")) return null;

        try
        {
            byte[] imageBytes = new byte[file.ContentLength];
            file.InputStream.Read(imageBytes, 0, file.ContentLength);
            return Resize(imageBytes, Path.GetExtension(file.FileName), maxHeight, highQuality);
        }
        catch
        {
            return null;
        }
    }

    public static string SaveAsImageFile(this HttpPostedFileBase file, string strFileName = "", string strFolder = FOLDER, int maxHeight = MAX_HEIGHT, bool highQuality = QUALITY)
    {
        if (file == null || !file.ContentType.Contains("image")) return string.Empty;

        try
        {
            var arrImageBytes = ToImageByteArray(file, maxHeight, highQuality);
            if (arrImageBytes == null) return string.Empty;

            string folder = HttpContext.Current.Server.MapPath("~/" + (string.IsNullOrEmpty(strFolder) ? FOLDER : strFolder));
            string filename = string.IsNullOrEmpty(strFileName) ? Path.GetFileNameWithoutExtension(file.FileName) : strFileName;
            string fileExtension = Path.GetExtension(file.FileName);
            
            Directory.CreateDirectory(folder);

            bool fileExist = File.Exists(Path.Combine(folder, filename + fileExtension));
            string filenameWithExt = filename + (fileExist ? GenerateUniqueChars(true) : "") + fileExtension;
            string path = Path.Combine(folder, filenameWithExt);

            File.WriteAllBytes(path, arrImageBytes);

            // Thumbnail generation
            if (THUMBNAIL_HEIGHT > 0 && THUMBNAIL_WIDTH > 0)
            {
                using (var image = Image.FromFile(path))
                using (var thumb = FixedSize(image, THUMBNAIL_WIDTH, THUMBNAIL_HEIGHT, true))
                {
                    string thumbFolder = Path.Combine(folder, THUMBNAIL);
                    Directory.CreateDirectory(thumbFolder);
                    thumb.Save(Path.Combine(thumbFolder, filenameWithExt), GetImageFormat(filenameWithExt));
                }
            }

            return filenameWithExt;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static byte[] Resize(this byte[] image, string fileFormat, int maxHeight, bool highQuality = QUALITY)
    {
        if (image == null) return null;

        using (var stream = new MemoryStream(image))
        using (var img = Image.FromStream(stream))
        {
            Image processedImg = img;

            // Correctly handle orientation (EXIF 274)
            if (Array.IndexOf(processedImg.PropertyIdList, 274) > -1)
            {
                var orientation = (int)processedImg.GetPropertyItem(274).Value[0];
                processedImg = OrientImage(processedImg, orientation);
            }

            using (var scaledImg = ScaleImage(processedImg, maxHeight, highQuality))
            using (var ms = new MemoryStream())
            {
                scaledImg.Save(ms, GetImageFormat(fileFormat));
                return ms.ToArray();
            }
        }
    }

    private static Image OrientImage(Image img, int orientation)
    {
        switch (orientation)
        {
            case 2: img.RotateFlip(RotateFlipType.RotateNoneFlipX); break;
            case 3: img.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
            case 4: img.RotateFlip(RotateFlipType.Rotate180FlipX); break;
            case 5: img.RotateFlip(RotateFlipType.Rotate90FlipX); break;
            case 6: img.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
            case 7: img.RotateFlip(RotateFlipType.Rotate270FlipX); break;
            case 8: img.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
        }
        try { img.RemovePropertyItem(274); } catch { }
        return img;
    }

    private static Image ScaleImage(Image image, int maxHeight, bool highQuality)
    {
        var ratio = (double)maxHeight / image.Height;
        var newWidth = (int)(image.Width * ratio);
        var newHeight = (int)(image.Height * ratio);
        var newImage = new Bitmap(newWidth, newHeight);

        using (var g = Graphics.FromImage(newImage))
        {
            if (highQuality)
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            }
            g.DrawImage(image, 0, 0, newWidth, newHeight);
        }

        return newImage;
    }

    private static Image FixedSize(Image image, int Width, int Height, bool needToFill)
    {
        int sourceWidth = image.Width;
        int sourceHeight = image.Height;
        double destX = 0, destY = 0, nScale = 0;

        double nScaleW = (double)Width / sourceWidth;
        double nScaleH = (double)Height / sourceHeight;

        if (!needToFill)
        {
            nScale = Math.Min(nScaleH, nScaleW);
        }
        else
        {
            nScale = Math.Max(nScaleH, nScaleW);
            destY = (Height - sourceHeight * nScale) / 2;
            destX = (Width - sourceWidth * nScale) / 2;
        }

        if (nScale > 1) nScale = 1;

        int destWidth = (int)Math.Round(sourceWidth * nScale);
        int destHeight = (int)Math.Round(sourceHeight * nScale);

        Bitmap bmPhoto = new Bitmap(destWidth + (int)Math.Round(2 * destX), destHeight + (int)Math.Round(2 * destY));
        using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
        {
            if (QUALITY)
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grPhoto.CompositingQuality = CompositingQuality.HighQuality;
                grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            }

            Rectangle to = new Rectangle((int)Math.Round(destX), (int)Math.Round(destY), destWidth, destHeight);
            Rectangle from = new Rectangle(0, 0, sourceWidth, sourceHeight);
            grPhoto.DrawImage(image, to, from, GraphicsUnit.Pixel);
            return bmPhoto;
        }
    }

    private static ImageFormat GetImageFormat(string fileFormat)
    {
        string ext = fileFormat.ToLower();
        if (ext.Contains("png")) return ImageFormat.Png;
        if (ext.Contains("gif")) return ImageFormat.Gif;
        return ImageFormat.Jpeg;
    }

    private static string GenerateUniqueChars(bool fileExist = true)
    {
        if (fileExist)
            return "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        else
            return string.Empty;
    }
}
