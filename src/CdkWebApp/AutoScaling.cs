using System;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ElasticLoadBalancing;
using Amazon.CDK.AWS.AutoScaling;
using Amazon.CDK.AWS.IAM;

namespace CdkWebApp
{
    public class AutoScaling
    {
        public AutoScaling()
        {
           
        }

        public AutoScalingGroup Create(Construct scope, Vpc vpc, SecurityGroup sg, string name)
        {

            var role = new Security.IamRole().Create(scope);
            var selection = new SubnetSelection
            {
                SubnetType = SubnetType.PUBLIC
            };

            var healchCheck = Amazon.CDK.AWS.AutoScaling.HealthCheck.Elb(new ElbHealthCheckOptions {
                Grace = Duration.Minutes(5)
            });

            var asg = new AutoScalingGroup(scope, "ASG", new AutoScalingGroupProps
            {
                Vpc = vpc,
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
                // get the linux two type otherwise it defaults to the older image
                MachineImage = new AmazonLinuxImage(new AmazonLinuxImageProps { Generation = AmazonLinuxGeneration.AMAZON_LINUX_2 }),
                AllowAllOutbound = true,
                //DesiredCapacity = 1,
                MaxCapacity = 2,
                MinCapacity = 1,
                //KeyName = "geekcafe",  // get from config
                AssociatePublicIpAddress = true,
                VpcSubnets = selection,
                Role = role,
                UserData = GetUserData(),
                HealthCheck = healchCheck,
                SecurityGroup = sg
                 
            });

            Amazon.CDK.Tags.Of(asg).Add("Name", $"{name}"); 

            //asg.ScaleOnCpuUtilization()

            return asg;
        }

        private UserData GetUserData()
        {
            var ud = UserData.ForLinux();

            // todo pull this from s3 or an assets directory
            var commands = new string[]
            {
                "#!/bin/bash -ex",
                //"# add user-data logs",
                "exec > >(tee /var/log/user-data.log|logger -t user-data -s 2>/dev/console) 2>&1",

                "start_time=\"$(date -u +%s.%N)\"",

                "current_user=$(whoami)",
                "echo \"executing commands as user: ${current_user}\"",
                "whoami",

                "sudo yum update -y ",

                //"#####################################################################################################################################################",
                //"# install docker & configure",
                "sudo amazon-linux-extras install docker -y",
                //"#start docker",
                "sudo service docker start",
                //"# make sure it statys running",
                "sudo chkconfig docker on",
                //"#Add the ec2-user to the docker group so you can execute Docker commands without using sudo.",
                "sudo usermod -a -G docker ec2-user",
                //"#get the latest docker-compose program",
                "sudo curl -L https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m) -o /usr/local/bin/docker-compose",
                //"# fix permissions",
                "sudo chmod +x /usr/local/bin/docker-compose",
                //"# / docker configure",
                //"#####################################################################################################################################################",

                //# apache
                "sudo yum -y install httpd mod_ssl",
                "sudo systemctl start httpd",
                "udo systemctl enable httpd",
                "sudo usermod -a -G apache ec2-user",
                "hostname=$(curl http://169.254.169.254/latest/meta-data/hostname)",
                "sudo cat -s > \"/var/www/html/index.html\" << EOF",
                "<html>",
                "<body>Hello Internet <p>${hostname}</body>",
                "</html>",
                "EOF",
                "sudo systemctl restart httpd"
                //# / apache

            };

            ud.AddCommands(commands);

            return ud;
        }
    }
}
