using DeviceHub.Api.Data;
using DeviceHub.Api.Dtos;
using DeviceHub.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeviceHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HeadsetsController : ControllerBase
{
	private readonly AppDbContext _db;

	public HeadsetsController(AppDbContext db)
	{
		_db = db;
	}

	private static HeadsetDto ToDto(Headset headset) =>
	new(headset.Id, headset.Name, headset.SerialNumber,
		headset.Status, headset.BatteryLevel, headset.LastSeenAt);

	[HttpGet]
	public async Task<ActionResult<List<HeadsetDto>>> GetAll()

	{
		var headsets = await _db.Headsets.ToListAsync();
		return headsets.Select(ToDto).ToList();
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<HeadsetDto>> GetById(int id)
	{

		var headset = await _db.Headsets.FindAsync(id);
		if (headset == null)
		{
			return NotFound();
		}
		return ToDto(headset);
	}

	[HttpPost]
	public async Task<ActionResult<HeadsetDto>> Create(CreateHeadsetDto headsetDto)
	{
		var headset = new Headset
		{
			Name = headsetDto.Name,
			SerialNumber = headsetDto.SerialNumber,
		};
		_db.Headsets.Add(headset);
		await _db.SaveChangesAsync();
		return CreatedAtAction(nameof(GetById), new { id = headset.Id }, ToDto(headset));
	}

}
