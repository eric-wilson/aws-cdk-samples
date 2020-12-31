using System;
using Amazon.CDK;
using Amazon.CDK.AWS.AutoScaling;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS;
using ELB = Amazon.CDK.AWS.ElasticLoadBalancing;

namespace CdkWebApp.LoadBalancers
{
    public class ClassicLoadBalancer
    {
        public ClassicLoadBalancer()
        {
        }

        public ELB.LoadBalancer Create(Construct construct, Vpc vpc, AutoScalingGroup asg, SecurityGroup sg = null)
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

            OpenPort(lb, 80);
            // this id was obtained from the certificate manager
            var certId = "arn:aws:acm:us-east-1:867915409343:certificate/eb2b584c-421d-4134-b679-1746642b5e3f";
            OpenPort(lb, 443, certId);


            return lb;
        }

        private void OpenPort(ELB.LoadBalancer lb, int port, string certId = null)
        {
            var listener = lb.AddListener(new ELB.LoadBalancerListener { ExternalPort = port, SslCertificateId = certId });

            listener.Connections.AllowDefaultPortFromAnyIpv4("Open to the world");
        }

       

        

    }
}
