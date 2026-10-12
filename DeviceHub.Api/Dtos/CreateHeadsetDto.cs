using System.ComponentModel.DataAnnotations;

namespace DeviceHub.Api.Dtos;



	public record CreateHeadsetDto([Required, StringLength(100)] string Name, [Required, StringLength(50)] string SerialNumber);


