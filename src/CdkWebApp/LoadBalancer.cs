using System;
using Amazon.CDK;
using Amazon.CDK.AWS.AutoScaling;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS;
using ELB = Amazon.CDK.AWS.ElasticLoadBalancing;

namespace CdkWebApp
{
    public class LoadBalancer
    {
        public LoadBalancer()
        {
        }

        public ELB.LoadBalancer Create(Construct construct, Vpc vpc, AutoScalingGroup asg)
        {
            var lb = new ELB.LoadBalancer(construct, "LB", new ELB.LoadBalancerProps
            {
                Vpc = vpc,
                InternetFacing = true,
                HealthCheck = new ELB.HealthCheck
                {
                    Port = 80
                }                
            });

            lb.AddTarget(asg);
            OpenPorts(lb, 80);
            // need an ssl cert to do this
            //OpenPorts(lb, 443);


            return lb;
        }

        private void OpenPorts(ELB.LoadBalancer lb, int port)
        {
            var listener = lb.AddListener(new ELB.LoadBalancerListener { ExternalPort = port });

            listener.Connections.AllowDefaultPortFromAnyIpv4("Open to the world");
        }

        

    }
}
