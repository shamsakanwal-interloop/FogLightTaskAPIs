namespace FogLightTask.DTOs;

public class OrderBatchSummaryDto
{
    public string OrderHash { get; set; }
    public int OrdNo { get; set; }
    public int DelNo { get; set; }

    public string PairCode { get; set; }
    public string Pattern { get; set; }
    public string Size { get; set; }
    public string Color { get; set; }
    public string BsCode { get; set; }
    public string Article { get; set; }

    public int BatchNo { get; set; }
    public string PSCode { get; set; }
    public decimal TotalReqQty { get; set; }
    public decimal Knitting { get; set; }
    public decimal Defects { get; set; }
    public decimal Stitching { get; set; }
    public decimal SyncStitching { get; set; }
}