namespace MovieService.Models.Requests;

public class UpdateMovieRequest : CreateMovieRequest
{
    public long? Views { get; set; }
}
