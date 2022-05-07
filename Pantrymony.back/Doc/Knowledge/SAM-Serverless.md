# SAM Serverless

## Specification

[AWS Serverless Application Model (AWS SAM) specification](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/sam-specification.html)

## Assigning policies to Lambdas 

In the template file we can assign individual access right policies to lambdas. This policies will be attached to the role that automatically gets created, in the case we haven't specified one as it is in our case.

See : [How do I grant IAM permissions to a Lambda function using an AWS SAM template?](https://aws.amazon.com/premiumsupport/knowledge-center/lambda-sam-template-permissions/)

## Environment Variables

I can define environment variables in the *template.yaml*, which will then be accessible both from withing the lambda code

``` c#
Environment.GetEnvironmentVariable())
```

and the template.yaml ($self:Globals....) 

See [Globals section of the AWS SAM template](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/sam-specification-template-anatomy-globals.html) for globals section.

## Authorization

Check [Controlling access to API Gateway APIs](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-controlling-access-to-apis.html)

## Build and publish 

``` bash
sam build -t template.yaml && sam deploy -t .aws-sam/build/template.yaml --s3-bucket pantrymony-serverless-code --stack-name Pantrymony --capabilities CAPABILITY_IAM 
```

## Debug Lambda locally

Resharper issues the following command. Apparently creates a temporary file with the event-input and calls its debuggers.

``` bash
/usr/bin/sam local invoke --template /home/pontifikas/Courses/CloudDeveloperUdacity/Code/cloud-developer-capstone/backend/Pantrymony.back/.aws-sam/build/template.yaml --event "/tmp/[Local] GetVictualsOfUser-event3.json" --debugger-path /usr/share/rider/lib/ReSharperHost --debug-port 45123 --debug-port 38063
```