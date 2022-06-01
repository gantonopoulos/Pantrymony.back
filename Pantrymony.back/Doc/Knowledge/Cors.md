# CORS

## Enabling CORS in AWS Lambda integration

We need:

- Every lambda must return the cors request headers, even when an error occurs. Otherwise the browser will not be able to understand the request.
- API Gateway must have the OPTIONS verb implemented and returning the **Access-Control-Allow-Credentials:false** CORS headers, even if our custom authorizer denies the access to the function. 