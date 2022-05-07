# DynamoDB

## Credentials

I did not have to provide credentials since I am issuing my DynamoDB access commands from within the lambdas. The lambda must follow the correct policy and this is defined in the template.yaml for each lambda. 

## Declaring Attributes (Columns)

Since DynamoDB can dynamically expand its column space, it only cares about those attributes that participate in keys. 

## KeySchema

The primary key of the table. Must consist of 2 attributes at most.  

    The Hash key, using which entries are grouped and  

    the Range key, using which the grouped entries are sorted. 

All indexes must have at most  2 attributes  with the role of Hash and Range key  respectively 

## GlobalSecondaryIndexes

This key sort of defines another hash key and thus results in the creation and maintenance (in the background) of a duplicate table.  

This incurs additional cost to  all operations (x2).

## LocalSecondaryIndexes

This key defines another sort key, which is why during definition the Hash  key must be present. No extra costs for that. One can have up to 5 such keys.


## High level Programming interface

Go [here](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBContext.QueryScan.html) for code examples for querying DynamoDB tables.

Also a very good article about high level interface [here](https://www.rahulpnath.com/blog/aws-dynamodb-net-core/)