namespace Techugo.POS.ECom.Model
{
    public class ApiSettings
    {
        public required string BaseUrl { get; set; }
        public required string InventoryEndpoint { get; set; }
        public required (string, string) StatusMapping { get; set; }
        public required string WeighingScalePortName { get; set; }
    }
}
