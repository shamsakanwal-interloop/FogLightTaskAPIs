using System;

namespace FogLightTask.DTOs;

public class ProductionReportDto
{
    public string? OrderNoDelNo { get; set; }
    public DateTime? LoadDate { get; set; }
    public string? PairCode { get; set; }
    public string? BatchNo { get; set; }
    public int? Required { get; set; }
    public int? StitchedPieces { get; set; }
    public int? StitchedHMS { get; set; }
    public int? WastedInvisible { get; set; }
    public int? StitchPendingFL { get; set; }
    public int? UnstitchedFL { get; set; }
    public string? EmpCode { get; set; }
    public string? EmpName { get; set; }
    public string? Shift { get; set; }
    public string? ProductionType { get; set; }
    public int? EmployeeStitchQty { get; set; }
    public string? Machines { get; set; }
}