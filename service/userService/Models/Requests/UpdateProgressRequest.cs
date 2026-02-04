using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Requests;

public class UpdateProgressRequest
{
    [Range(0, 100)]
    public double Progress { get; set; }
}
