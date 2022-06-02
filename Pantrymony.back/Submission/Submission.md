# Pantrymony

I have implemented an application to manage ones Pantry. The application website is in [this](https://d1z0cnh1fsmyo1.cloudfront.net) url.

## Technologies employed

The Backend (deliverable) was developed in .NET6 and C# v.10. For IaaS I am using AWS Serverless Application Model (SAM).
The [Frontend](https://github.com/gantonopoulos/Pantrymony) was developed in Blazor WASM using .NET6 and C# v.10.

## Frontend

The URL of the application is: https://d3v0si9mbf84mx.cloudfront.net

### Authentication

The user gets authenticated by logging to the application via an OpenId Provider (OAuth0).
The application requests the email of the user to use as unique identifier. Any subsequent action from the user gets authenticated using the ID_Token returned from OAuth.

The user can logout.

The application has 2 pages. The Dashboard and the Editing page.

### Dashboard

Here the user sees the Victuals that they have so far stored, along with selected properties.

- The user can multi-select victuals with the purpose of deletion.
- The user can create a new victual.
- The user can edit the displayed properties of the victual. Those changes are persisted immediately.
- The user can select a single victual with the purpose of Editing. In this case they will be redirected to the Editing form.

### Editing form

- The user can select to upload an image from the file system. Only .jpeg and .png images are supported.
- The user can clear the selected image.
- Any changes the user makes are not persisted until the user presses **Save**
- The user can cancel the Editing and return to the Dashboard either by pressing **Cancel** or by pressing the **Dashboard** button on the side bar.

### Remarks

The resulting website is not very fast but it works as expected. This is partly because Blazor must download the framework to the browser and because I am an inexperienced frontend developer. But since neither the frontend nor optimization was the focus of the project and since it is fully functional, I chose not to optimize it. Just be a bit patient with the loading times, some times they are as long as 5 seconds. But it gets faster as you use it.

## Backend

As mentioned above, the backend uses AWS SAM. The template for the creation of the infrastructure can be found in the [template.yaml](../template.yaml) file in the project directory.

It creates an API-Gateway with Proxy-Lambda integrations.

Each lambda corresponds to one endpoint of our API.

A custom authorizer has been implemented. It validates the ID_Token passed by the Frontend against the JWKS Keys in the OpenId provider. Unauthenticated requests are of course rejected.

A DynamoDb table which holds the Victual data except the image attachment has also been created.

An S3 bucket is being created with no public access. For fetching, saving and deleting elements from the S3-Bucket, the application uses Pre-Signed Urls.

All Permissions are defined per Lambda function on a minimum access basis.

All requests are validated through a custom validator resource operating on the schema of the the Victual. If a request query parameter is omitted, or a Body entry has wrong type, the appropriate error is emitted.

CORS requests are handled by a PreFlightRequest lambda. Also all Responses contain the required headers.

Distributed Tracing through X-Ray and Logging is enabled and accessible in Cloudwatch.

## Acknowledgements

All the resources that I consulted are being documented in Markdown files found under **Doc/Knowledge**

## Deliverables

- [Frontend](https://github.com/gantonopoulos/Pantrymony)
- [Backend](https://github.com/gantonopoulos/Pantrymony.back)
- [template.yaml](../template.yaml) (Serverless file)
- [Image](CloudFormationStacks.png) of the CloudFormation Stacks for both frontend and backend (backend in focus).
- [Image](ApiGatewayWithLambdaIntegrationSample.png) of the resulting API Gateway with all its endpoints and a sample lambda proxy integration.
- [Image](ApiGatewayWithLoggingAndTracingEnabled.png) of the API-Gateway's configuration with both execution logging and X-Ray tracing enabled on it.
- [Image](PutLambdaWithParameterAndBodyVerificationEnabled.png) of the update lambda configuration, where I had to activate verification for both the parameters and the body.
- [Image](UpdateLambdaWIthXrayTracing.png) of the update lambda with XRay tracing of its calls.
- [Image](S3ImageBucketPermissions.png) of the image's S3 bucket with the permissions configuration.
- [Image](CloudwatchLogGroups.png) from Cloudwatch with all resulting log groups, one for each Lambda and the ApiGateway.
- [Image](CloudwatchXRayAnalysis.png) from Cloudwatch with an overview of the X-Ray analysis.
