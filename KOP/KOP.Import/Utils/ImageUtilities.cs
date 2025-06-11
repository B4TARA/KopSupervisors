namespace KOP.Import.Utils
{
    public static class ImageUtilities
    {
        public static string SaveBase64Image(string? base64String, string fileName, string userImgDownloadPath)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String))
                {
                    return "../users_images/default_profile_icon.svg";
                }

                var file = Path.Combine(userImgDownloadPath, $"{fileName}.jpg");

                byte[] bytes = Convert.FromBase64String(base64String);

                using (FileStream fileStream = new FileStream(file, FileMode.Create))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Flush();
                }

                return Path.Combine("../users_images/", $"{fileName}.jpg");
            }
            catch (Exception ex)
            {
                return "../users_images/default_profile_icon.svg";
            }
        }
    }
}