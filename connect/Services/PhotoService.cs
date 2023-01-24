using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace connect.Services;

public class PhotoService
{
    private readonly Cloudinary _cloudinary;
    public PhotoService()
    {
        var CLOUDINARY_URL = Environment.GetEnvironmentVariable("CLOUDINARY_URL");

        var data = CLOUDINARY_URL.Split("://")[1];
        var apiKey = data.Split(":")[0];
        var apiSecret = (data.Split(':')[1]).Split('@')[0];
        var cloudName = data.Split('@')[1];
        var acc = new Account(apiKey: apiKey, apiSecret: apiSecret, cloud: cloudName);
        _cloudinary = new Cloudinary(account: acc);
    }
    public async Task<ImageUploadResult> AddAvatarAsync(IFormFile file)
    {
        int quality = 30;
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation()
                .Quality(quality)
                .Gravity("face")
                .Height(200)
                .Width(200)
                .Crop("thumb")
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult;
    }
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        int quality = 50;
        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Quality(quality)
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }
        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result;
    }
}
