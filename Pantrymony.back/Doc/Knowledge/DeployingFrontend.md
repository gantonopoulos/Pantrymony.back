# Deploying in Cloudfront

I use the template I got from [this](https://aws.amazon.com/blogs/developer/run-blazor-based-net-web-applications-on-aws-serverless/) example publish to upload my 
frontend to a stack in CloudFormation which in turn creates a Distribution in Cloudfront.

It is very important do define some error page redirects in the Distribution for reasons explained [here](https://stackoverflow.com/questions/44318922/receive-accessdenied-when-trying-to-access-a-page-via-the-full-url-on-my-website)
so as no to get an error during login.

## Publishing

Right click on the project, and select the project directory as a target for publishing to a folder. 

If you need to publish new changes, delete the _framework and _content directories first, as well as the following files:
- Pantrymony.styles.css
- appsettings.json.br
- appsettings.json.gz

After publishing call:

``` bash
aws s3 sync . s3://pantrymony-frontend-web
```

to upload the changes to the S3 bucket of the frontend.

## Creating the Stack

Follow the instructions in the [howto](https://aws.amazon.com/blogs/developer/run-blazor-based-net-web-applications-on-aws-serverless/).

The template is in the project directory.


## Updating the stack

After pushing the changes to the S3 bucket (see [Publishing](#publishing)), wait a bit until the distribution is updated.

## OAuth

Everytime a new distribution is created (not updated), the expected urls in the [OAuth application](https://manage.auth0.com/dashboard/eu/gantonopoulos/applications/0JAJqL9SQopGrtHSSQaEgsHAYUTZQlvR/settings) have to be updated to the new address.