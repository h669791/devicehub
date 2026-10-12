using DeviceHub.Api.Data;
using DeviceHub.Api.Dtos;
using DeviceHub.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

	private static bool IsUniqueViolation(DbUpdateException ex) =>
	ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

	private ObjectResult SerialNumberConflict(string serialNumber) =>
		Problem(
			statusCode: StatusCodes.Status409Conflict,
			title: "Serienummeret er allerede i bruk",
			detail: $"Et headset med serienummer {serialNumber} finnes allerede.");

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
		var exists = await _db.Headsets.AnyAsync(h => h.SerialNumber == headsetDto.SerialNumber);
		if (exists)
		{
			return SerialNumberConflict(headsetDto.SerialNumber);
		}

		var headset = new Headset
		{
			Name = headsetDto.Name,
			SerialNumber = headsetDto.SerialNumber,
		};
		_db.Headsets.Add(headset);

		try
		{
			await _db.SaveChangesAsync();
		}
		catch (DbUpdateException ex) when (IsUniqueViolation(ex))
		{
			return SerialNumberConflict(headsetDto.SerialNumber);
		}

		return CreatedAtAction(nameof(GetById), new { id = headset.Id }, ToDto(headset));
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(int id, UpdateHeadsetDto updateHeadsetDto)
	{
		var headset = await _db.Headsets.FindAsync(id);
		if (headset == null)
		{
			return NotFound();
		}

		var exists = await _db.Headsets.AnyAsync(h =>
			h.SerialNumber == updateHeadsetDto.SerialNumber && h.Id != id);
		if (exists)
		{
			return SerialNumberConflict(updateHeadsetDto.SerialNumber);
		}

		headset.Name = updateHeadsetDto.Name;
		headset.SerialNumber = updateHeadsetDto.SerialNumber;

		try
		{
			await _db.SaveChangesAsync();
		}
		catch (DbUpdateException ex) when (IsUniqueViolation(ex))
		{
			return SerialNumberConflict(updateHeadsetDto.SerialNumber);
		}

		return NoContent();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var headset = await _db.Headsets.FindAsync(id);
		if (headset == null)
		{
			return NotFound();
		}
		_db.Headsets.Remove(headset);
		await _db.SaveChangesAsync();
		return NoContent();
	}

}
