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

        public AutoScalingGroup Create(Construct scope, Vpc vpc)
        {

            var role = new Security.IamRole().Create(scope);
            var selection = new SubnetSelection
            {
                SubnetType = SubnetType.PUBLIC
            };

            var asg = new AutoScalingGroup(scope, "ASG", new AutoScalingGroupProps
            {
                Vpc = vpc,
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
                // get the linux two type otherwise it defaults to the older image
                MachineImage = new AmazonLinuxImage(new AmazonLinuxImageProps { Generation = AmazonLinuxGeneration.AMAZON_LINUX_2 }),
                AllowAllOutbound = true,
                DesiredCapacity = 1,
                MaxCapacity = 2,
                MinCapacity = 1,
                KeyName = "a4l-key-pair",
                AssociatePublicIpAddress = true,
                VpcSubnets = selection,
                Role = role,
                UserData = GetUserData()
            });

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


            };

            ud.AddCommands(commands);

            return ud;
        }
    }
}
