using Amazon.DynamoDBv2.DataModel;
namespace models
{
    [DynamoDBTable("TestTable2")]
    class Sample {
        [DynamoDBHashKey("pk")]
        public string? Id { get; set; }

        [DynamoDBRangeKey("sk")]
        public string? IdValue { get; set; }

        [DynamoDBProperty]
        public string? Name { get; set; }

        [DynamoDBProperty]
        public string? LastName { get; set; }
    }
}