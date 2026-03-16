using System;

namespace FogLightTask.Entity;

public class SqlKnittingView
{
    public string? OrderHash { get; set; }
    public int CostCode { get; set; }
    public string? PsCode { get; set; }
    public int? OrdNo { get; set; }
    public string? PairCode { get; set; }
    public string? Design { get; set; }
    public int? DelNo { get; set; }
    public int? McSize { get; set; }
    public string? BaseColor { get; set; }
    public string? BsCode { get; set; }
    public DateTime? LD { get; set; }
    public long? KnitQty { get; set; }
    public decimal? Balance { get; set; }
}