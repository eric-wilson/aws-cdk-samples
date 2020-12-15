using System;
using Amazon.CDK.AWS.IAM;

namespace CdkWebApp.Security.Roles.SSM
{
    internal class ManagedInstanceCore
    {
        public ManagedInstanceCore()
        {
        }

        public PolicyStatement[] Statements
        {
            get
            {
                return new PolicyStatement[] {
                    GenerateEC2MessageStatement(),
                    GenerateSSMMessageStatement(),
                    GenerateSSMStatement()
                };
            }
        }

        private PolicyStatement GenerateSSMStatement()
        {
            var statementProps = new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new string[] { "ssm:DescribeAssociation",
                "ssm:GetDeployablePatchSnapshotForInstance",
                "ssm:GetDocument",
                "ssm:DescribeDocument",
                "ssm:GetManifest",
                "ssm:GetParameter",
                "ssm:GetParameters",
                "ssm:ListAssociations",
                "ssm:ListInstanceAssociations",
                "ssm:PutInventory",
                "ssm:PutComplianceItems",
                "ssm:PutConfigurePackageResult",
                "ssm:UpdateAssociationStatus",
                "ssm:UpdateInstanceAssociationStatus",
                "ssm:UpdateInstanceInformation" },
                Resources = new string[] { "*" }
            };

            var statement = new PolicyStatement(statementProps);

            return statement;

        }

        private PolicyStatement GenerateSSMMessageStatement()
        {
            var statementProps = new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new string[]
                {
                    "ssmmessages:CreateControlChannel",
                    "ssmmessages:CreateDataChannel",
                    "ssmmessages:OpenControlChannel",
                    "ssmmessages:OpenDataChannel"
                },
                Resources = new string[] { "*" }
            };

            var statement = new PolicyStatement(statementProps);

            return statement;

        }

        private PolicyStatement GenerateEC2MessageStatement()
        {
            var statementProps = new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new string[]
                {
                    "ec2messages:AcknowledgeMessage",
                    "ec2messages:DeleteMessage",
                    "ec2messages:FailMessage",
                    "ec2messages:GetEndpoint",
                    "ec2messages:GetMessages",
                    "ec2messages:SendReply"
                },
                Resources = new string[] { "*" }
            };

            var statement = new PolicyStatement(statementProps);

            return statement;

        }

    }
}
