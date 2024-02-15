namespace aws.utils.dynamodb
{
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;
    using Amazon.Runtime;
    using Amazon.Util;
    public class DynamoDBHelpers
    {
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

    }
}