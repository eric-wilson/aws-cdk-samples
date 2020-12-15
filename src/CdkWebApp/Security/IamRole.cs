using System;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;

namespace CdkWebApp.Security
{
    public class IamRole
    {
        public IamRole()
        {
        }

        public Role Create(Construct scope)
        {
            
            
            var props = new RoleProps
            {
                RoleName = "CDK-Role",
                ManagedPolicies = GetManagedPolicies(scope),                
                AssumedBy = new ServicePrincipal("ec2.amazonaws.com")
                
            };
            var role = new Role(scope, "iam-role", props);

            return role;
        }

        public ManagedPolicy[] GetManagedPolicies(Construct scope)
        {

            var ssmStatements = new Security.Roles.SSM.ManagedInstanceCore().Statements;
            var ssmAssume = new Security.Roles.SSM.TrustedCommunication().Statements;
            var ec2Statements = new Security.Roles.EC2.FullAccess().Statements;
            var s3Statments = new Security.Roles.S3.FullAccess().Statements;
            var policies = new List<ManagedPolicy>();

            policies.Add(Policy(scope, "ssm-statements", null, ssmStatements));
            //policies.Add(Policy(scope, "ssm-assume", null, ssmAssume));
            policies.Add(Policy(scope, "ec2-full-access", null, ec2Statements));
            policies.Add(Policy(scope, "s3-full-access", null, s3Statments));



            return policies.ToArray();
        }

        public ManagedPolicy Policy(Construct scope, string id, string policyName, PolicyStatement[] statements)
        {

            if (policyName == null) policyName = id;
            
            var props = new ManagedPolicyProps
            {
                ManagedPolicyName = policyName,
                Statements = statements
                

            };
            var policy = new ManagedPolicy(scope, id, props);

            return policy;
        }
    }
}
