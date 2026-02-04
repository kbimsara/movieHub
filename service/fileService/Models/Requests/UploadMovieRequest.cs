using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FileService.Models.Requests;

public class UploadMovieRequest
{
    [Required]
    public IFormFile? File { get; set; }

    public IFormFile? Poster { get; set; }

    [Required]
    public string Metadata { get; set; } = string.Empty;
}
