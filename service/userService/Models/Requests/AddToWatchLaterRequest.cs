using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Requests;

public class AddToWatchLaterRequest
{
    [Required]
    public Guid MovieId { get; set; }
}
