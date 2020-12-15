using System;
using System.Collections.Generic;
using Amazon.CDK.AWS.IAM;

namespace CdkWebApp.Security.Roles.EC2
{
    internal class FullAccess
    {
        public FullAccess()
        {
        }

        public PolicyStatement[] Statements
        {
            get
            {
                return new PolicyStatement[] {
                    GenerateEC2(),
                    GenerateELB(),
                    GenerateCloudWatch(),
                    GenerateAutoScaling(),
                   // GenerateServiceLink(),
                };
            }
        }

        private PolicyStatement GenerateEC2()
        {
            var statementProps = new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new string[] { "ec2:*" },
                Resources = new string[] { "*" }
            };

            var statement = new PolicyStatement(statementProps);

            return statement;

        }

        private PolicyStatement GenerateELB()
        {
            var statementProps = new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new string[] { "elasticloadbalancing:*" },
                Resources = new string[] { "*" }
            };

            var statement = new PolicyStatement(statementProps);

            return statement;

        }

        private PolicyStatement GenerateCloudWatch()
        {
            var statementProps = new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new string[] { "cloudwatch:*" },
                Resources = new string[] { "*" }
            };

            var statement = new PolicyStatement(statementProps);

            return statement;

        }

        private PolicyStatement GenerateAutoScaling()
        {
            var statementProps = new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new string[] { "autoscaling:*" },
                Resources = new string[] { "*" }
            };

            var statement = new PolicyStatement(statementProps);

            return statement;

        }


        private PolicyStatement GenerateServiceLink()
        {
            var condtions = new Dictionary<string, object>();

            condtions.Add("StringEquals", "\"iam:AWSServiceName\": [ " 
                        + "\"autoscaling.amazonaws.com\", "
                        + "\"ec2scheduled.amazonaws.com\", "
                        + "\"elasticloadbalancing.amazonaws.com\", "
                        + "\"spot.amazonaws.com\", "
                        + "\"spotfleet.amazonaws.com\", "
                        + "\"transitgateway.amazonaws.com\" "
                        + "]"
                        );

            var statementProps = new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new string[] { "iam:CreateServiceLinkedRole:*" },
                Resources = new string[] { "*" },
                Conditions = condtions
            };

            var statement = new PolicyStatement(statementProps);

            return statement;

        }


    }
}
