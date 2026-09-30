using DeviceHub.Api.Models;

namespace DeviceHub.Api.Dtos;

public record HeadsetDto(int Id, string Name, string SerialNumber, HeadsetStatus Status, int BatteryLevel, DateTime? LastSeenAt);
