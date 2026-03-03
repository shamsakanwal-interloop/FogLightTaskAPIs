using System;

namespace FogLightTask;

public class ToeClosingHourlyDto
{
    public DateTime StitchDate { get; set; }
    public int TcCostCenter { get; set; }
    public int MachineId { get; set; }
    public string? BatchNo { get; set; }
    public int YearCode { get; set; }
    public string? EmpCode { get; set; }
    public string? EmpName { get; set; }
    public int BatchId { get; set; }
    public decimal ShiftProd { get; set; }
    public decimal H1 { get; set; }
    public decimal H2 { get; set; }
    public decimal H3 { get; set; }
    public decimal H4 { get; set; }
    public decimal H5 { get; set; }
    public decimal H6 { get; set; }
    public decimal H7 { get; set; }
    public decimal H8 { get; set; }
}