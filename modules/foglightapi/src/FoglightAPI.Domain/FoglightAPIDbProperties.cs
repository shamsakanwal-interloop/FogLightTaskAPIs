namespace FoglightAPI;

public static class FoglightAPIDbProperties
{
    public static string DbTablePrefix { get; set; } = "FoglightAPI";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "FoglightAPI";
}
