namespace aws.utils.dynamodb
{
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;
    using Amazon.Runtime;
    using Amazon.Util;
    public class DynamoDBBuilder 
    {

        public DynamoDBBuilder(string tableName)
        {
            this.TableName = tableName;
        }

        private readonly string TableName;
        public List<KeySchemaElement> Schema = new List<KeySchemaElement>();
        public List<GlobalSecondaryIndex> SecondaryIndexes = new List<GlobalSecondaryIndex>();
        public List<AttributeDefinition> Definitions = new List<AttributeDefinition>();
        public ProvisionedThroughput ProvisionedThroughput = new ProvisionedThroughput();


        public DynamoDBBuilder AddSchema(string attributeName, string type) {
            if (attributeName is null) throw new ArgumentNullException();
            if (type is null) throw new ArgumentNullException();
            // if (type != "HASH" || type != "RANGE") throw new ArgumentException();

            this.Schema.Add(
                new KeySchemaElement
                {
                    AttributeName = attributeName, KeyType = type
                }
            );

            return this;
        }

        public DynamoDBBuilder AddDefinition(string attributeName, string type) {
            if (attributeName is null) throw new ArgumentNullException();
            if (type is null) throw new ArgumentNullException();
            
            this.Definitions.Add(
                new AttributeDefinition
                {
                    AttributeName = attributeName, AttributeType = type
                }
            );

            return this;
        }

        public DynamoDBBuilder AddSecondaryIndex(string indexName, string attributeName, string keyType, string projectionType) {
            if (attributeName is null) throw new ArgumentNullException();
            if (keyType is null) throw new ArgumentNullException();

            var KeyType = this.Schema.Find(keySchema => keySchema.KeyType == "HASH");
            if (KeyType is null) throw new ArgumentNullException();

            
            this.SecondaryIndexes.Add(
                new GlobalSecondaryIndex
                {
                    IndexName = indexName,
                    KeySchema = new List<KeySchemaElement>
                    {
                        KeyType,
                        new KeySchemaElement { AttributeName = attributeName, KeyType = keyType }
                    },
                    Projection = new Projection
                    {
                        ProjectionType = projectionType
                    },
                    ProvisionedThroughput = this.ProvisionedThroughput
                }
            );

            return this;
        }

        public DynamoDBBuilder SetProvisionedThroughput(int readCapacityUnits, int writeCapacityUnits) {            
            this.ProvisionedThroughput.ReadCapacityUnits = readCapacityUnits;
            this.ProvisionedThroughput.WriteCapacityUnits = writeCapacityUnits;

            return this;
        }

        public async void CreateTable()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            CreateTableRequest request = new CreateTableRequest
            {
                TableName = this.TableName,
                KeySchema = this.Schema,
                ProvisionedThroughput = this.ProvisionedThroughput,
                AttributeDefinitions = this.Definitions,
                GlobalSecondaryIndexes = this.SecondaryIndexes
            };
            
            var tableDescription = await client.CreateTableAsync(request);//.CreateTableResult.TableDescription;
            
            // Console.WriteLine("Table name: {0}", tableDescription.TableName);
            // Console.WriteLine("Creation time: {0}", tableDescription.CreationDateTime);
            // Console.WriteLine("Item count: {0}", tableDescription.ItemCount);
            // Console.WriteLine("Table size (bytes): {0}", tableDescription.TableSizeBytes);
            // Console.WriteLine("Table status: {0}", tableDescription.TableStatus);
        }

        public async void UpdateTable()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            UpdateTableRequest request = new UpdateTableRequest
            {
                TableName = this.TableName,
                ProvisionedThroughput = this.ProvisionedThroughput,
                AttributeDefinitions = this.Definitions,
            };
            
            var tableDescription = await client.UpdateTableAsync(request); //.CreateTableResult.TableDescription;
            
            // Console.WriteLine("Table name: {0}", tableDescription.TableName);
            // Console.WriteLine("Creation time: {0}", tableDescription.CreationDateTime);
            // Console.WriteLine("Item count: {0}", tableDescription.ItemCount);
            // Console.WriteLine("Table size (bytes): {0}", tableDescription.TableSizeBytes);
            // Console.WriteLine("Table status: {0}", tableDescription.TableStatus);
        }

        public async void DeleteTable()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            DeleteTableRequest request = new DeleteTableRequest
            {
                TableName = this.TableName
            };

            var tableDescription = await client.DeleteTableAsync(request);//.DeleteTableResult.TableDescription;

            //Console.WriteLine("Table name: {0}", tableDescription.TableName);
            //Console.WriteLine("Table status: {0}", tableDescription.TableStatus);
        }

    }
}