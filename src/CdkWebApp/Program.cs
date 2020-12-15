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

            var env = "dev";
            // get the stack name from the args or a setting
            var name = $"{env}-project-cdk";
            //name = "WebAppStack";

            var stack = new CdkWebAppStack(app, name);

            Tags.Of(stack).Add("Stack-Author", "Eric Wilson");            
            Tags.Of(stack).Add("Stack-Version", "0.0.1-beta");
            Tags.Of(stack).Add("Stack-Tool", "GeekCafe-AWS-CDK");

            var cloudAssembly = app.Synth();
        }
    }
}
