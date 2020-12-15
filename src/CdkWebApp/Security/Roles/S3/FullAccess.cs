using System;
using Amazon.CDK.AWS.IAM;

namespace CdkWebApp.Security.Roles.S3
{
    public class FullAccess
    {
        public FullAccess()
        {
        }

        public PolicyStatement[] Statements
        {
            get
            {
                return new PolicyStatement[] {
                    GenerateS3()
                    
                };
            }
        }

        private PolicyStatement GenerateS3()
        {
            var statementProps = new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new string[] { "s3:*" },
                Resources = new string[] { "*" }
            };

            var statement = new PolicyStatement(statementProps);

            return statement;

        }
    }
}
