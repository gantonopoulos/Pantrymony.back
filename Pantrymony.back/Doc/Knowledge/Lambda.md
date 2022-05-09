# Lambda

## Resources

[Lambda@github](https://github.com/aws/aws-lambda-dotnet)

[AWS Lambda@ .net6](https://aws.amazon.com/blogs/compute/introducing-the-net-6-runtime-for-aws-lambda/)

## Defining path parameters for lambda in SAM .yaml:

To define a path parameter userId for the endpoint /victuals (I.e. /victuals/{userId}):

```yaml
Events: 
GetVictualsApi: 
Type: Api 
Properties: 
Path: /victuals 
Method: delete 
RequestParameters: 
- method.request.path.userId: 
    Required: true 
 ```

For query parameters use method.request.querystring.NAME_OF_PARAMETER 

See [here](https://docs.aws.amazon.com/apigateway/latest/developerguide/api-gateway-method-settings-method-request.html#setup-method-request-parameters)

## Executing a Lambda and passing a Path parameter as argument (for direct execution, either locally or on the AWS web cli)

Pass a json entry:

``` json
{ 
"pathParameters": { 
"PATH_PARAMETER_NAME":"PATH_PARAMETER_VALUE" 
} 
} 
```

The rest of the required (from the framework) data of the APIGatewayProxyRequest that we do not provide, appear to be filled in automatically.

For querystrings:

``` json
{ 
"queryStringParameters": { 
"QUERY_PARAMETER_NAME":"QUERY_PARAMETER_VALUE" 
} 
} 
```

See [here](https://newbedev.com/getting-json-body-in-aws-lambda-via-api-gateway/)