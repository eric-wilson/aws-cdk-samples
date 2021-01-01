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

        public ELB.ApplicationLoadBalancer Create(Construct construct, Vpc vpc, AutoScalingGroup asg, SecurityGroup sg, string name)
        {
            var lb = new ELB.ApplicationLoadBalancer(construct, "LB", new ELB.ApplicationLoadBalancerProps
            {
                Vpc = vpc,
                InternetFacing = true,
                LoadBalancerName = "MyAppLoadBalancer",
                SecurityGroup = sg
                        
            });

            Amazon.CDK.Tags.Of(lb).Add("Name", $"{name}"); 

            // add a listener
            var listener = AddListener(lb, 80, null);
            var appPort = 80;
            var group = listener.AddTargets($"AppFleet", new ELB.AddApplicationTargetsProps {
                Port = appPort,
                Targets = new [] {asg} 
            });

            Amazon.CDK.Tags.Of(listener).Add("Name", $"{name}-listner"); 
            Amazon.CDK.Tags.Of(group).Add("Name", $"{name}-fleet"); 
            

            listener.AddAction($"FixedOkMessage", new ELB.AddApplicationActionProps {
                Priority = 10,
                Conditions = new [] { ELB.ListenerCondition.PathPatterns(new [] { "/ok" }) },
                Action = ELB.ListenerAction.FixedResponse(200, new ELB.FixedResponseOptions {
                    ContentType = "text/html",
                    MessageBody = "OK"
                })
            });

            listener.AddAction($"LBHealthInfo", new ELB.AddApplicationActionProps {
                Priority = 15,
                Conditions = new [] { ELB.ListenerCondition.PathPatterns(new [] { "/lb-status" }) },
                Action = ELB.ListenerAction.FixedResponse(200, new ELB.FixedResponseOptions {
                    ContentType = "application/json",
                    MessageBody = "{ \"lb\": { \"type\": \"application-load-balancer\", \"launchDateUtc\": \"{" + DateTime.UtcNow + "}\", \"status\": \"ok\" } }"
                })
            });

            // this id was obtained from the certificate manager
            // TODO, get the certificate (if it exists based on tags?)
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
