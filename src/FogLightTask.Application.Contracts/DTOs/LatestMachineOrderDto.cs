using System;

namespace FogLightTask.DTOs;

public class LatestMachineOrderDto
{
    public int? CostCode { get; set; }
    public int? Machine { get; set; }
    public string? OrderNo { get; set; }
    public string? PsCode { get; set; }
    public string? BsCode { get; set; }
    public string? PairCode { get; set; }
    public string? Size { get; set; }
    public string? Pattern { get; set; }
    public string? BaseColor { get; set; }
    public DateTime StartTime { get; set; }
}