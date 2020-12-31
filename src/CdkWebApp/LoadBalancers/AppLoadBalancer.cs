using System;
using Amazon.CDK;
using Amazon.CDK.AWS.AutoScaling;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS;
using ELB = Amazon.CDK.AWS.ElasticLoadBalancingV2;

namespace CdkWebApp.LoadBalancers
{
    public class ApplicationLoadBalancer
    {
        public ApplicationLoadBalancer()
        {
        }

        public ELB.ApplicationLoadBalancer Create(Construct construct, Vpc vpc, AutoScalingGroup asg, SecurityGroup sg)
        {
            var lb = new ELB.ApplicationLoadBalancer(construct, "LB", new ELB.ApplicationLoadBalancerProps
            {
                Vpc = vpc,
                InternetFacing = true,
                LoadBalancerName = "MyAppLoadBalancer",
                SecurityGroup = sg
                        
            });

            // add a listener
            var listener = AddListener(lb, 80, null);
            var appPort = 80;
            var group = listener.AddTargets($"AppFleet", new ELB.AddApplicationTargetsProps {
                Port = appPort,
                Targets = new [] {asg} 
            });

            listener.AddAction($"Fixed_{80}", new ELB.AddApplicationActionProps {
                Priority = 10,
                Conditions = new [] { ELB.ListenerCondition.PathPatterns(new [] { "/ok" }) },
                Action = ELB.ListenerAction.FixedResponse(200, new ELB.FixedResponseOptions {
                    ContentType = "text/html",
                    MessageBody = "OK"
                })
            });

            listener.AddAction($"LBHealthCheck", new ELB.AddApplicationActionProps {
                Priority = 15,
                Conditions = new [] { ELB.ListenerCondition.PathPatterns(new [] { "/lbhealth" }) },
                Action = ELB.ListenerAction.FixedResponse(200, new ELB.FixedResponseOptions {
                    ContentType = "text/html",
                    MessageBody = "LB Health=OK"
                })
            });

            // this id was obtained from the certificate manager
            var certArn = "arn:aws:acm:us-east-1:867915409343:certificate/eb2b584c-421d-4134-b679-1746642b5e3f";
            listener = AddListener(lb, 443, certArn);            

            // forward any ssl requests to the target group
            listener.AddAction("SSLForward", new ELB.AddApplicationActionProps {
                
                Action = ELB.ListenerAction.Forward(new [] {group}),


            });           

            
            return lb;
        }

        private ELB.ApplicationListener AddListener(ELB.ApplicationLoadBalancer lb, int port, string certArn = null)
        {

            var certs = (certArn == null) ? null : new ELB.ListenerCertificate[] { new ELB.ListenerCertificate(certArn)} ;

            
            var listener = lb.AddListener($"App2BalancerListener_{port}", new ELB.BaseApplicationListenerProps { 
                Open = true, 
                Certificates = certs,
                Port = port,                                                              
                });

            //listener.Connections.AllowDefaultPortFromAnyIpv4("Open to the world");
            
            return listener;
        
              
        }

       

        

    }
}
