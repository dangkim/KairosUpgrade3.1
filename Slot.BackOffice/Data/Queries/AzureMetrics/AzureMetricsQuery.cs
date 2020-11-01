namespace Slot.BackOffice.Data.Queries.AzureMetrics
{
    public class AzureMetricsQuery : IOperatorQuery
    {
        public string Period { get; set; }

        public string Region { get; set; }

        public string Operator { get; set; }

        public string Currency { get; set; }

        public int? OperatorId { get; set; }

        public int?[] OperatorIds { get; set; }

        public string OperatorTag { get => Operator; set => Operator = value; }
    }
}
