using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CdkWebApp
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            var environment = "dev";
            var project = $"cdk-proving-ground";
            var id = $"{environment}-{project}";
            // get the stack name from the args or a setting
            //var name = $"cdk-proving-ground";
            //name = "WebAppStack";

            var stack = new CdkWebAppStack(app, id, project, environment);

            Tags.Of(stack).Add("Stack-Author", "Eric Wilson");            
            Tags.Of(stack).Add("Stack-Version", "0.0.1-beta");
            Tags.Of(stack).Add("Stack-Tool", "GeekCafe-AWS-CDK");
            Tags.Of(stack).Add("Stack-Environment", environment);
            Tags.Of(stack).Add("Stack-Project", project);

            var cloudAssembly = app.Synth();
        }
    }
}
