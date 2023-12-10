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
        public Dictionary<string, Condition> KeyConditions = new Dictionary<string, Condition>();
        public Dictionary<string, AttributeValue> ExpressionAttributeValues = new Dictionary<string, AttributeValue>();
        public Dictionary<string, string> ExpressionAttributeNames = new Dictionary<string, string>();
        public string FilterExpression = string.Empty;

        public DynamoDBBuilder MustEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("EQ", true, attribute, value);
        }

        public DynamoDBBuilder MustNotEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("NE", true, attribute, value);
        }

        public DynamoDBBuilder MustLessEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("LE", true, attribute, value);
        }

        public DynamoDBBuilder MustLessFilter<T>(string attribute, T value) {
            return AddFilter<T>("LT", true, attribute, value);
        }

        public DynamoDBBuilder MustGreaterEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("GE", true, attribute, value);
        }

        public DynamoDBBuilder MustGreaterFilter<T>(string attribute, T value) {
            return AddFilter<T>("GT", true, attribute, value);
        }

        public DynamoDBBuilder MustBetweenFilter<T>(string attribute, T value) {
            return AddFilter<T>("BETWEEN", true, attribute, value);
        }

        public DynamoDBBuilder MustContainsFilter<T>(string attribute, T value) {
            return AddFilter<T>("CONTAINS", true, attribute, value);
        }

        public DynamoDBBuilder MustNotContainsFilter<T>(string attribute, T value) {
            return AddFilter<T>("NOT_CONTAINS", true, attribute, value);
        }

        public DynamoDBBuilder MustBeginWithFilter<T>(string attribute, T value) {
            return AddFilter<T>("BEGIN_WITH", true, attribute, value);
        }

        public DynamoDBBuilder CouldEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("EQ", false, attribute, value);
        }

        public DynamoDBBuilder CouldNotEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("NE", false, attribute, value);
        }

        public DynamoDBBuilder CouldLessEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("LE", false, attribute, value);
        }

        public DynamoDBBuilder CouldLessFilter<T>(string attribute, T value) {
            return AddFilter<T>("LT", false, attribute, value);
        }

        public DynamoDBBuilder CouldGreaterEqualFilter<T>(string attribute, T value) {
            return AddFilter<T>("GE", false, attribute, value);
        }

        public DynamoDBBuilder CouldGreaterFilter<T>(string attribute, T value) {
            return AddFilter<T>("GT", false, attribute, value);
        }

        public DynamoDBBuilder CouldBetweenFilter<T>(string attribute, T value) {
            return AddFilter<T>("BETWEEN", false, attribute, value);
        }

        public DynamoDBBuilder CouldContainsFilter<T>(string attribute, T value) {
            return AddFilter<T>("CONTAINS", false, attribute, value);
        }

        public DynamoDBBuilder CouldNotContainsFilter<T>(string attribute, T value) {
            return AddFilter<T>("NOT_CONTAINS", false, attribute, value);
        }

        public DynamoDBBuilder CouldBeginWithFilter<T>(string attribute, T value) {
            return AddFilter<T>("BEGIN_WITH", false, attribute, value);
        }

        public DynamoDBBuilder AddKeyCondition(string attribute, string value) {
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

        public DynamoDBBuilder AddFilter<T>(string operation, bool mandatory, string attribute, T value) {
            var attributeValue = GetAttribute<T>(value);
            var filterExpression = GetFilterExpression(operation, attribute);
            var conditionWrapper = mandatory ? "AND" : "OR";

            this.ExpressionAttributeValues.Add($":{attribute}", attributeValue);
            this.ExpressionAttributeNames.Add($"#{attribute}", attribute);
            this.FilterExpression = (!String.IsNullOrWhiteSpace(this.FilterExpression)) ? 
                $"{this.FilterExpression} {conditionWrapper} {filterExpression}" : $"{filterExpression}";

            return this;
        }

        public static AttributeValue GetAttribute<T>(T value)
        {
            if (value is null) throw new Exception("Value is null.");
            switch (typeof(T).ToString())
            {
                case "System.String":
                    return new AttributeValue { S = value.ToString() };
                case "System.Int32":
                    return new AttributeValue { N = value.ToString() };
                case "System.Double":
                    return new AttributeValue { N = value.ToString() };
                case "System.Boolean":
                    return new AttributeValue { BOOL = Convert.ToBoolean(value) };
                default:
                    throw new Exception("Type is not valid.");
            }
        }

        public static string GetFilterExpression(string operation, string attribute)
        {
            switch (operation)
            {
                case "EQ":
                    return $"#{attribute} = :{attribute}";
                case "NE":
                    return $"#{attribute} <> :{attribute}";
                case "LT":
                    return $"#{attribute} < :{attribute}";
                case "LE":
                    return $"#{attribute} <= :{attribute}";
                case "GT":
                    return $"#{attribute} > :{attribute}";
                case "GE":
                    return $"#{attribute} >= :{attribute}";
                case "BETWEEN":
                    return $"#{attribute} = :{attribute}";
                case "CONTAINS":
                    return $"contains(#{attribute}, :{attribute})";
                case "NOT_CONTAINS":
                    return $"NOT contains(#{attribute}, :{attribute})";
                case "BEGIN_WITH":
                    return $"begins_with(#{attribute}, :{attribute})";
                default:
                    throw new Exception("Operation is not valid.");
            }
        }

        public async Task<QueryResponse> ExecuteQuery()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            QueryRequest request = new QueryRequest 
            {
                TableName = this.TableName,
                KeyConditions = this.KeyConditions
            };

            // Console.WriteLine($"CONDITIONS => {this.FilterExpression}");

            if (this.ExpressionAttributeValues.Any()) request.ExpressionAttributeValues = this.ExpressionAttributeValues;
            if (this.ExpressionAttributeNames.Any()) request.ExpressionAttributeNames = this.ExpressionAttributeNames;
            if (!String.IsNullOrWhiteSpace(this.FilterExpression)) request.FilterExpression = this.FilterExpression;

            return await client.QueryAsync(request);
        }

        public async Task<ScanResponse> ExecuteScan()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            ScanRequest request = new ScanRequest 
            {
                TableName = this.TableName,
            };
            
            // Console.WriteLine($"CONDITIONS => {this.FilterExpression}");
            
            if (this.ExpressionAttributeValues.Any()) request.ExpressionAttributeValues = this.ExpressionAttributeValues;
            if (this.ExpressionAttributeNames.Any()) request.ExpressionAttributeNames = this.ExpressionAttributeNames;
            if (!String.IsNullOrWhiteSpace(this.FilterExpression)) request.FilterExpression = this.FilterExpression;

            return await client.ScanAsync(request);
        }

    }
}