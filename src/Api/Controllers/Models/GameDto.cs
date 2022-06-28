using System.ComponentModel.DataAnnotations;

namespace BasketballStats.Api.Controllers.Models
{
    public record GameDto(Guid? TeamHomeId, Guid? TeamAwayId, [Required] DateTime GameTime);
}
