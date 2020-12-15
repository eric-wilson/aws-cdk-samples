using System;
using Amazon.CDK.AWS.IAM;

namespace CdkWebApp.Security.Roles.SSM
{
    internal class TrustedCommunication
    {
        public TrustedCommunication()
        {
        }

        public PolicyStatement[] Statements
        {
            get
            {
                return new PolicyStatement[] {
                    AssumeRole(),
                    
                };
            }
        }

        private PolicyStatement AssumeRole()
        {
           
            var statementProps = new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new string[] { "sts:AssumeRole" },
                Principals = new ServicePrincipal[] { new ServicePrincipal("ec2.amazonaws.com") }
                
            };

            var statement = new PolicyStatement(statementProps);

            return statement;

        }

        

    }
}
