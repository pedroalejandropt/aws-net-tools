namespace aws.utils.dynamodb
{
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;
    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2.DataModel;
    using Amazon.Runtime;
    using Amazon.Util;
    using System;

    public class DynamoDBCommandBuilder 
    {
        public DynamoDBCommandBuilder(string tableName)
        {
            this.TableName = tableName;
        }

        private readonly string TableName;
        private DynamoDBContext? Context;
        private string IndexName = string.Empty;
        public Dictionary<string, Condition> KeyConditions = new Dictionary<string, Condition>();
        public Dictionary<string, AttributeValue> AttributeValues = new Dictionary<string, AttributeValue>();
        public Dictionary<string, string> AttributeNames = new Dictionary<string, string>();
        public string FilterExpression = string.Empty;

        public DynamoDBCommandBuilder MustEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("EQ", true, attribute, value);
        }

        public DynamoDBCommandBuilder MustNotEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("NE", true, attribute, value);
        }

        public DynamoDBCommandBuilder MustLessEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("LE", true, attribute, value);
        }

        public DynamoDBCommandBuilder MustLessFilter<T>(string attribute, T value) {
            return AddFilter<T>("LT", true, attribute, value);
        }

        public DynamoDBCommandBuilder MustGreaterEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("GE", true, attribute, value);
        }

        public DynamoDBCommandBuilder MustGreaterFilter<T>(string attribute, T value) {
            return AddFilter<T>("GT", true, attribute, value);
        }

        public DynamoDBCommandBuilder MustBetweenFilter<T>(string attribute, T value) {
            return AddFilter<T>("BETWEEN", true, attribute, value);
        }

        public DynamoDBCommandBuilder MustContainsFilter<T>(string attribute, T value) {
            return AddFilter<T>("CONTAINS", true, attribute, value);
        }

        public DynamoDBCommandBuilder MustNotContainsFilter<T>(string attribute, T value) {
            return AddFilter<T>("NOT_CONTAINS", true, attribute, value);
        }

        public DynamoDBCommandBuilder MustBeginWithFilter<T>(string attribute, T value) {
            return AddFilter<T>("BEGIN_WITH", true, attribute, value);
        }

        public DynamoDBCommandBuilder CouldEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("EQ", false, attribute, value);
        }

        public DynamoDBCommandBuilder CouldNotEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("NE", false, attribute, value);
        }

        public DynamoDBCommandBuilder CouldLessEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("LE", false, attribute, value);
        }

        public DynamoDBCommandBuilder CouldLessFilter<T>(string attribute, T value) {
            return AddFilter<T>("LT", false, attribute, value);
        }

        public DynamoDBCommandBuilder CouldGreaterEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("GE", false, attribute, value);
        }

        public DynamoDBCommandBuilder CouldGreaterFilter<T>(string attribute, T value) {
            return AddFilter<T>("GT", false, attribute, value);
        }

        public DynamoDBCommandBuilder CouldBetweenFilter<T>(string attribute, T value) {
            return AddFilter<T>("BETWEEN", false, attribute, value);
        }

        public DynamoDBCommandBuilder CouldContainsFilter<T>(string attribute, T value) {
            return AddFilter<T>("CONTAINS", false, attribute, value);
        }

        public DynamoDBCommandBuilder CouldNotContainsFilter<T>(string attribute, T value) {
            return AddFilter<T>("NOT_CONTAINS", false, attribute, value);
        }

        public DynamoDBCommandBuilder CouldBeginWithFilter<T>(string attribute, T value) {
            return AddFilter<T>("BEGIN_WITH", false, attribute, value);
        }

        public DynamoDBCommandBuilder AddKeyCondition(string attribute, string value) {
            Condition condition = new Condition
            {
                ComparisonOperator = "EQ",
                AttributeValueList = new List<AttributeValue>
                {
                    new AttributeValue { S = value }
                }
            };
            
            this.KeyConditions.Add(attribute, condition);
            
            return this;
        }

        public DynamoDBCommandBuilder SetIndexName(string indexName)
        {
            this.IndexName = indexName;
            return this;
        }

        public DynamoDBCommandBuilder AddFilter<T>(string operation, bool mandatory, string attribute, T value) {
            var attributeValue = DynamoDBHelpers.GetAttribute<T>(value);
            var filterExpression = DynamoDBHelpers.GetFilterExpression(operation, attribute);
            var conditionWrapper = mandatory ? "AND" : "OR";

            this.AttributeValues.Add($":{attribute}", attributeValue);
            this.AttributeNames.Add($"#{attribute}", attribute);
            this.FilterExpression = (!String.IsNullOrWhiteSpace(this.FilterExpression)) ? 
                $"{this.FilterExpression} {conditionWrapper} {filterExpression}" : $"{filterExpression}";

            return this;
        }

        public DynamoDBCommandBuilder AddAttribute<T>(string attribute, T value) {
            var attributeValue = DynamoDBHelpers.GetAttribute<T>(value);
            this.AttributeValues.Add($"{attribute}", attributeValue);
            return this;
        }

        public async Task<QueryResponse> ExecuteQuery()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            QueryRequest request = new QueryRequest 
            {
                TableName = this.TableName,
                KeyConditions = this.KeyConditions
            };
            
            if (!string.IsNullOrWhiteSpace(this.IndexName)) request.IndexName = this.IndexName;
            if (this.AttributeValues.Any()) request.ExpressionAttributeValues = this.AttributeValues;
            if (this.AttributeNames.Any()) request.ExpressionAttributeNames = this.AttributeNames;
            if (!String.IsNullOrWhiteSpace(this.FilterExpression)) request.FilterExpression = this.FilterExpression;

            return await client.QueryAsync(request);
        }

        public async Task<List<T>> ExecuteQuery<T>() where T : class, new()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            this.Context = new DynamoDBContext(client);

            QueryRequest request = new QueryRequest 
            {
                TableName = this.TableName,
                KeyConditions = this.KeyConditions
            };
            
            if (!string.IsNullOrWhiteSpace(this.IndexName)) request.IndexName = this.IndexName;
            if (this.AttributeValues.Any()) request.ExpressionAttributeValues = this.AttributeValues;
            if (this.AttributeNames.Any()) request.ExpressionAttributeNames = this.AttributeNames;
            if (!String.IsNullOrWhiteSpace(this.FilterExpression)) request.FilterExpression = this.FilterExpression;

            var response = await client.QueryAsync(request);
            return ParseItems<T>(response.Items);
        }

        public async Task<ScanResponse> ExecuteScan()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            ScanRequest request = new ScanRequest 
            {
                TableName = this.TableName,
            };
            
            if (this.AttributeValues.Any()) request.ExpressionAttributeValues = this.AttributeValues;
            if (this.AttributeNames.Any()) request.ExpressionAttributeNames = this.AttributeNames;
            if (!String.IsNullOrWhiteSpace(this.FilterExpression)) request.FilterExpression = this.FilterExpression;

            return await client.ScanAsync(request);
        }

        public async Task<List<T>> ExecuteScan<T>() where T : class, new()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            this.Context = new DynamoDBContext(client);

            ScanRequest request = new ScanRequest 
            {
                TableName = this.TableName,
            };
            
            if (this.AttributeValues.Any()) request.ExpressionAttributeValues = this.AttributeValues;
            if (this.AttributeNames.Any()) request.ExpressionAttributeNames = this.AttributeNames;
            if (!String.IsNullOrWhiteSpace(this.FilterExpression)) request.FilterExpression = this.FilterExpression;

            var response = await client.ScanAsync(request);
            return ParseItems<T>(response.Items);
        }

        public async Task AddUpdateItem()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            PutItemRequest request = new PutItemRequest 
            {
                TableName = this.TableName,
                Item = this.AttributeValues,
            };

            await client.PutItemAsync(request);  

        }

        public async void DeleteItem()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            DeleteItemRequest request = new DeleteItemRequest
            {
                TableName = this.TableName,
                Key = this.AttributeValues
            };

            await client.DeleteItemAsync(request);
        }

        private List<T> ParseItems<T>(IEnumerable<Dictionary<string, AttributeValue>> items) 
            where T : class, new()
        {
            var resultList = new List<T>();

            foreach (var item in items)
            {
                var instance = this.Context.FromDocument<T>(Document.FromAttributeMap(item));
                resultList.Add(instance);
            }

            return resultList;
        }
    }
}