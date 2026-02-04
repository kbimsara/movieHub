using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Requests;

public class AddToLibraryRequest
{
    [Required]
    public Guid MovieId { get; set; }
}
