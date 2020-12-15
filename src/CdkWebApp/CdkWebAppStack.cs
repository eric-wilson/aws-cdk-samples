using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CdkWebApp
{
    
    public class CdkWebAppStack : Stack
    {        
        internal CdkWebAppStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here

            // vpc
            var  tags = new Dictionary<string, string>();
            tags.Add("ManagedBy", "AWS-CDK");

            var config = new VpcProps {
                Cidr = "10.0.0.0/16",
                NatGateways = 0,           
            };

            // create the vpc
            var vpc = new Vpc(this, "VPC", config);

            

            // security groups
            var sg = BuildSG(vpc, "test-sg-name");

            // db allocation
            // -- do a separate stack
            

            // asg
            var auto = new AutoScaling(); // (, $"{id}-auto-scaling", props);
            var asg = auto.Create(this, vpc);


            // load balancer
            var lb = new LoadBalancer();
            lb.Create(this, vpc, asg);

        }

        private SecurityGroup BuildSG(Vpc vpc, string name = null)
        {
            var props = new SecurityGroupProps
            {
                AllowAllOutbound = true,
                Vpc = vpc,
                Description = "http/https",
                SecurityGroupName = "sgroup-web-access",                                
            };

            // create some security groups
            var sg = new SecurityGroup(this, "sgroup-http-https", props);
                     
            var peer = Peer.AnyIpv4();
            
            var http = new Port(GetPortProps(80, 80));
            var https = new Port(GetPortProps(443, 443));
            sg.AddIngressRule(peer, http, "ipv4-http");
            sg.AddIngressRule(peer, https, "ipv4-https");


            

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

        //private void AddTag(string resourceName, string key, string value)
        //{
        //    var manger = new TagManager(TagType.KEY_VALUE, resourceName);
        //    manger.SetTag(key, value);

        //}

        //private void AddTag(Reference reference, string key, string value)
        //{
        //    var manger = new TagManager();
        //    manger.SetTag(key, value);

        //}
    }

   

    //public class CustomTag: Tag
    //{
    //    public CustomTag(string key, string value, ITagProps? props): base(key, value, props)
    //    {

            
    //    }

    //    public void Appy (ITaggable resource)
    //    {
    //        base.ApplyTag(resource);
    //    }

    //}
}
