using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CdkWebApp
{
    
    public class CdkWebAppStack : Stack
    {        
        internal CdkWebAppStack(Construct scope, string id, string project, string environment, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here            

            var config = new VpcProps {
                Cidr = "10.0.0.0/16",
                NatGateways = 0,
                
            };

            // create the vpc
            var vpc = new Vpc(this, "VPC", config);
            
            Amazon.CDK.Tags.Of(vpc).Add("Name", $"{id}-vpc");             

            // db allocation
            // -- do a separate stack
            var webAppSG = BuildWebAppSG(vpc, $"{id}-web-app-sg");
            var db = new DataStore.RDSMySQLDatabase();
            var dbInstance = db.Create(this, vpc, webAppSG, $"{id}-db");

            
            // asg
            var auto = new AutoScaling(); // (, $"{id}-auto-scaling", props);
            var asg = auto.Create(this, vpc, webAppSG, $"{id}-asg");

            // allow the connections from the autoscaling group to the db instance
            // alternative to setting up the secruity groups directly
            //asg.Connections.AllowDefaultPortTo(dbInstance);

            // load balancer
            //var lb = new LoadBalancers.ClassicLoadBalancer();
            var lb = new LoadBalancers.ApplicationLoadBalancer();

            // security groups
            var webSG = BuildWebSG(vpc, "http-https");
            lb.Create(this, vpc, asg, webSG ,$"{id}-lb");

        }

        private SecurityGroup BuildWebSG(Vpc vpc, string name = null)
        {
            var props = new SecurityGroupProps
            {
                AllowAllOutbound = true,
                Vpc = vpc,
                Description = name,
                SecurityGroupName = $"{name}",                                
            };

            // create some security groups
            var sg = new SecurityGroup(this, $"{name}", props);
                     
            var peer = Peer.AnyIpv4();
            
            var http = new Port(GetPortProps(80, 80));
            var https = new Port(GetPortProps(443, 443));
            sg.AddIngressRule(peer, http, "ipv4-http");
            sg.AddIngressRule(peer, https, "ipv4-https");            

            Amazon.CDK.Tags.Of(sg).Add("Name", $"{name}"); 

            return sg;
        }

        private SecurityGroup BuildWebAppSG(Vpc vpc, string name = null)
        {
            var props = new SecurityGroupProps
            {
                AllowAllOutbound = true,
                Vpc = vpc,
                Description = name,
                SecurityGroupName = $"{name}",                                
            };

            // create some security groups
            var sg = new SecurityGroup(this, name, props);
                     
            Amazon.CDK.Tags.Of(sg).Add("Name", $"{name}"); 

            return sg;
        }

        

        private PortProps GetPortProps(int from, int to)
        {
            var portProps = new PortProps
            {
                FromPort = from,
                ToPort = to,
                Protocol = Protocol.TCP,
                StringRepresentation = "??"
            };

            return portProps;
        }

        
    }

   

    
}
