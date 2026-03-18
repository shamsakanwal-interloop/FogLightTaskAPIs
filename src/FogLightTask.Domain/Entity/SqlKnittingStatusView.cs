using System;

namespace FogLightTask.Entity;

public class SqlKnittingStatusView
{
    public string? OrderNo { get; set; }
    public int? D { get; set; }              
    public DateTime? LD { get; set; }       
    public long? ReqKnit { get; set; }       
    public long? Knitted { get; set; }
    public long? BalKnit { get; set; }
    public string? KnitStatus { get; set; }
}