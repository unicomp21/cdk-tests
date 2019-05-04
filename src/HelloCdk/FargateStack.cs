using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using System.Collections.Generic;

namespace HelloCdk
{
    class FargateStack : Stack
    {
        public FargateStack(Construct parent, string id, IStackProps props) : base(parent, id, props)
        {
            var vpc = new VpcNetwork(this, "MyVpc", new VpcNetworkProps() { MaxAZs = 2 });
            var cluster = new Cluster(this, "Cluster", new ClusterProps() { Vpc = VpcNetwork.Import(this, "vpc2", vpc.Export()) });
            var env = new Dictionary<string, string>();

            // Instantiate Fargate Service with just cluster and image
            var fargateService = new LoadBalancedFargateService(this, "FargateService",
                new LoadBalancedFargateServiceProps()
                {
                    Cluster = cluster,
                    //Image = ContainerImage.FromRegistry("amazon/amazon-ecs-sample", null),
                    Image = ContainerImage.FromRegistry("karthequian/helloworld", null),
                    Environment = env,
                    PublicLoadBalancer = true,
                    CreateLogs = true
                });

            // Output the DNS where you can access your service
            new CfnOutput(this, "LoadBalancerDNS", new CfnOutputProps()
            {
                Value = fargateService.LoadBalancer.DnsName
            });
        }
    }
}
