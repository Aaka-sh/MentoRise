using System;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{
    // Cloudinary instance for uploading and deleting photos
    private readonly Cloudinary _cloudinary;
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        // Initialize Cloudinary with the provided configuration settings
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        // Create a new Cloudinary instance with the account details
        _cloudinary = new Cloudinary(acc);
    }

    // Upload a photo to Cloudinary and return the upload result
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            //create a new FileDescription object with the file name and stream
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().
                    Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "da-net8"
            };
            //call the UploadAsync method of the Cloudinary instance to upload the photo
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }
        return uploadResult;
    }

    // Delete a photo from Cloudinary using its public ID and return the deletion result
    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId); //create a new DeletionParams object with the public ID of the photo to be deleted
        var result = await _cloudinary.DestroyAsync(deleteParams); //call the DestroyAsync method of the Cloudinary instance to delete the photo
        return result; //return the result of the deletion operation
    }
}
