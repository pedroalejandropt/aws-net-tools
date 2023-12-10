# AWS Utils for .NET DynamoDB Tool

## Overview

This is a .NET tool designed to simplify interactions with AWS. At the moment this tool offer a solution to interact with DynamoDB. 

The tool provides a flexible and fluent API for building queries and scans, making it easy to construct complex DynamoDB queries with various filtering conditions.

## Getting Started

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) (version 6 or later)

### Installation

Clone the repository and build the project:

```bash
git clone https://github.com/pedroalejandropt/aws-net-tools
cd aws-net-tools
dotnet build
```

## Usage

To use the DynamoDB tool, follow these steps:

1. Create an instance of `DynamoDBBuilder` by providing the DynamoDB table name.

```csharp
var dynamoDBBuilder = new DynamoDBBuilder("YourTableName");
```

2. Chain together various filter conditions using the fluent API provided by the tool.

```csharp
dynamoDBBuilder.MustEqualFilter("AttributeName", "SomeValue")
               .MustLessFilter("NumericAttribute", 100)
               .CouldGreaterEqualFilter("AnotherAttribute", DateTime.Now.AddDays(-7));
```

3. Execute a query or scan by calling the appropriate method (`ExecuteQuery` or `ExecuteScan`).

```csharp
var queryResponse = await dynamoDBBuilder.ExecuteQuery();
var scanResponse = await dynamoDBBuilder.ExecuteScan();
```

4. Handle the query or scan responses as needed.

## Filter Operations

The tool supports various filter operations, including:

- Equal (`MustEqualFilter`, `CouldEqualFilter`)
- Not Equal (`MustNotEqualFilter`, `CouldNotEqualFilter`)
- Less Than (`MustLessFilter`, `CouldLessFilter`)
- Less Than or Equal To (`MustLessEqualFilter`, `CouldLessEqualFilter`)
- Greater Than (`MustGreaterFilter`, `CouldGreaterFilter`)
- Greater Than or Equal To (`MustGreaterEqualFilter`, `CouldGreaterEqualFilter`)
- Between (`MustBetweenFilter`, `CouldBetweenFilter`)
- Contains (`MustContainsFilter`, `CouldContainsFilter`)
- Not Contains (`MustNotContainsFilter`, `CouldNotContainsFilter`)
- Begins With (`MustBeginWithFilter`, `CouldBeginWithFilter`)

## Example

```csharp
var dynamoDBBuilder = new DynamoDBBuilder("YourTableName");

dynamoDBBuilder.MustEqualFilter("Category", "Electronics")
               .CouldGreaterEqualFilter("Price", 100);

var queryResponse = await dynamoDBBuilder.ExecuteQuery();
```

This example creates a query for items in the "YourTableName" table where the category is "Electronics" and the price is greater than or equal to 100.

## Notes

- Make sure to replace placeholders such as "YourTableName," "AttributeName," and "SomeValue" with your actual values.
- This tool currently supports DynamoDB queries and scans. Additional features and services may be added in future releases.

## Contributors

- [Pedro Alejandro Pacheco Tripi](https://github.com/pedroalejandropt)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
