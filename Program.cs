
namespace aws
{
    using aws.utils.dynamodb;
    using models;

    class Program
    {
        /// <summary>
        /// In the main program method, the application is calling all the functionalities 
        /// related with the aws dotnet tool service.
        /// </summary>
        static void Main(string[] args)
        {
            //If the dynamodb table does not exist this piece of code will create it
            //If the dynamodb table exists, the code will fail.
            // DynamoDBBuilder builder = new DynamoDBBuilder("Sample");
            // builder
            //     .AddSchema("pk", "HASH")
            //     .AddSchema("sk", "RANGE")
            //     .AddDefinition("pk", "S")
            //     .AddDefinition("sk", "S")
            //     .AddDefinition("Name", "S")
            //     .AddDefinition("LastName", "S")
            //     .AddDefinition("Age", "S")
            //     .SetProvisionedThroughput(1,1)
            //     .AddSecondaryIndex("SampleIndex", "Name", "RANGE", "ALL")
            //     .AddSecondaryIndex("TestIndex2", "LastName", "RANGE", "ALL")
            //     .CreateTable();

            // If the sk is not in the dynamodb table, this piece of code will create a new record
            // if it exists it will update the previous record
            Console.WriteLine("Start Add/Update Item!");
            
            DynamoDBCommandBuilder build = new DynamoDBCommandBuilder("Sample")
                .AddAttribute("pk", "Id")
                .AddAttribute("sk", "2")
                .AddAttribute("Name", "Pedro")
                .AddAttribute("LastName", "Pacheco")
                .AddAttribute("Age", "26");


            build.AddUpdateItem();
            
            Console.WriteLine("End Add/Update Item!");

            Console.WriteLine("Start Query!");
            DynamoDBCommandBuilder queryBuilder = new DynamoDBCommandBuilder("Sample")
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

            Console.WriteLine("Start Sample Query!");
            DynamoDBCommandBuilder querySampleBuilder = new DynamoDBCommandBuilder("Sample")
                .AddKeyCondition("pk", "Id")
                .AddKeyCondition("sk", "1");
            
            var querySampleRes = querySampleBuilder.ExecuteQuery<Sample>();
            
            querySampleRes.Result?.ForEach(item => {
                Console.WriteLine();
                Console.WriteLine($"Partition key: {item.Id}");
                Console.WriteLine($"Sort key: {item.IdValue}");
                Console.WriteLine($"Name: {item.Name}");
                Console.WriteLine($"LastName: {item.LastName}");
                Console.WriteLine();
            });
            Console.WriteLine("End Sample Query!"); 

            Console.WriteLine("Start Scan!");
            DynamoDBCommandBuilder scanBuilder = new DynamoDBCommandBuilder("Sample")
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

            Console.WriteLine("Start Sample Scan!");
            DynamoDBCommandBuilder scanSampleBuilder = new DynamoDBCommandBuilder("Sample")
                .MustEqualFilter("Name", "Pedro")
                .MustEqualFilter("LastName", "Pacheco");
            
            var scanSampleRes = scanSampleBuilder.ExecuteScan<Sample>();
            scanSampleRes.Result?.ForEach(item => {
                Console.WriteLine();
                Console.WriteLine($"Partition key: {item.Id}");
                Console.WriteLine($"Sort key: {item.IdValue}");
                Console.WriteLine($"Name: {item.Name}");
                Console.WriteLine($"LastName: {item.LastName}");
                Console.WriteLine();
            });
            Console.WriteLine("End Sample Scan!");
        }
    }
}

