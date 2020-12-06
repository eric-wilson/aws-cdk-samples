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
            new CdkWebAppStack(app, "CdkWebAppStack");
            app.Synth();
        }
    }
}
