
namespace aws
{
    using aws.utils.dynamodb;
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Query!");
            DynamoDBBuilder queryBuilder = new DynamoDBBuilder("TestTable")
                .AddKeyCondition("pk", "User")
                .MustEqualFilter("Name", "Pedro")
                .MustEqualFilter("LastName", "Pacheco")
                .CouldEqualFilter("Age", "26");
            
            var queryRes = queryBuilder.ExecuteQuery();
            queryRes.Result?.Items?.ToList().ForEach(item => {
                Console.WriteLine();
                Console.WriteLine($"Partition key: {item["pk"].S}");
                Console.WriteLine($"Sort key: {item["sk"].S}");
                Console.WriteLine($"Name: {item["Name"].S}");
                Console.WriteLine($"LastName: {item["LastName"].S}");
                Console.WriteLine();
            });
            Console.WriteLine("End Query!");

            Console.WriteLine("Start Scan!");
            DynamoDBBuilder scanBuilder = new DynamoDBBuilder("TestTable")
                .MustEqualFilter("Name", "Pedro")
                .MustEqualFilter("LastName", "Pacheco")
                .CouldEqualFilter("Age", "26");
            
            var scanRes = scanBuilder.ExecuteScan();
            scanRes.Result?.Items?.ToList().ForEach(item => {
                Console.WriteLine();
                Console.WriteLine($"Partition key: {item["pk"].S}");
                Console.WriteLine($"Sort key: {item["sk"].S}");
                Console.WriteLine($"Name: {item["Name"].S}");
                Console.WriteLine($"LastName: {item["LastName"].S}");
                Console.WriteLine();
            });
            Console.WriteLine("End Scan!");
        }
    }
}

