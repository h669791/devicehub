namespace DeviceHub.Api.Models;

public enum HeadsetStatus { Offline, Online, InUse, Error }

public class Headset
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public required string SerialNumber { get; set; }
	public HeadsetStatus Status { get; set; } = HeadsetStatus.Offline;
	public int BatteryLevel { get; set; }
	public DateTime? LastSeenAt { get; set; }
}