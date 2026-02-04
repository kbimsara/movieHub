using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Requests;

public class AddToWatchHistoryRequest
{
    [Required]
    public Guid MovieId { get; set; }

    [Range(0, 100)]
    public double Progress { get; set; }
}
