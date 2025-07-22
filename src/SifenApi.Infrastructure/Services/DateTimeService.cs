// src/SifenApi.Infrastructure/Services/DateTimeService.cs
using SifenApi.Application.Common.Interfaces;

namespace SifenApi.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}