using System;

namespace FogLightTask.DTOs;

public class KnittingReportDto
{
    public int? CostCode { get; set; }
    public string? BaseColor { get; set; }
    public int? OrdNo { get; set; }
    public int? DelNo { get; set; }
    public string? Article { get; set; }
    public string? PairCode { get; set; }
    public string? BsCode { get; set; }
    public string? PsCode { get; set; }
    public DateTime? LD { get; set; }
    public long? KnitQty { get; set; }
    public decimal? Balance { get; set; }
    public string? CustSize { get; set; }
    public string? Side { get; set; }
    public string? ArtFullDesc { get; set; }
    public string? ClrDesc { get; set; }
    public decimal? Qty { get; set; }
}