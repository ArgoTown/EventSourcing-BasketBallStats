using System.ComponentModel.DataAnnotations;

namespace BasketballStats.Api.Controllers.Request
{
    /// <summary>
    /// Game details request.
    /// </summary>
    public record GameRequestDto(Guid? TeamHomeId, Guid? TeamAwayId, [Required] DateTime GameTime);
}
