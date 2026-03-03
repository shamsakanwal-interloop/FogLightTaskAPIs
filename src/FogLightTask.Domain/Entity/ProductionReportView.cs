using Volo.Abp.Domain.Entities;

namespace FogLightTask.Entity;

public class ProductionReportView: Entity<int>
{  
    public string? Id { get; set; }
    public string? OrderNoDelNo { get; set; }
    public string? LoadDate { get; set; }
    public string? PairCode { get; set; }
    public string? BatchNo { get; set; }              
    public string? Required { get; set; }
    public string? StitchedPieces { get; set; }
    public string? StitchedHMS { get; set; }
    public string? WastedInvisible { get; set; }
    public string? StitchPendingFL { get; set; }
    public string? UnstitchedFL { get; set; }             
    public string? EmpCode { get; set; }
    public string? EmpName { get; set; }
    public string? Shift { get; set; }
    public string? ProductionType { get; set; }
    public string? EmployeeStitchQty { get; set; }
    public string? Machines { get; set; }
}