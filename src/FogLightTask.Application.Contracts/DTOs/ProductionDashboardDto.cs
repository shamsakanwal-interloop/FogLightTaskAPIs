using System;
using System.Collections.Generic;
using System.Text;

namespace FogLightTask.DTOs;

public class ProductionDashboardDto
{
    public string OrderHash { get; set; }
    public int SNo { get; set; }
    public DateTime KnitDate { get; set; }
    public string Shift { get; set; }
    public int YarnTransId { get; set; }

    public decimal Prod { get; set; }
    public decimal CGrade { get; set; }
    public string KnitReq { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; }
    public string OrderDel { get; set; }
    public string PairCode { get; set; }
    public string Pattern { get; set; }
    public string Size { get; set; }
    public string Color { get; set; }
    public int ActiveMachines { get; set; }
}