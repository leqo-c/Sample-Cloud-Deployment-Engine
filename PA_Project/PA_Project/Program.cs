using System;
using System.IO;
using System.Threading;
using PA_Project.CDE;
using PA_Project.Parsing;
using PA_Project.Provider;

namespace PA_Project
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Test1();
            //Test2();
            Console.ReadKey();
        }

        private static void Test1()
        {
            var spec = File.ReadAllText("test_specification.yaml");
            var t = new SpecificationTransformer(spec);
            var mod = t.Transform();
            var specObj = new Parser().Parse(new CharacterStream(mod));

            var cde = new CloudDeploymentEngine(specObj);
            var provider = new CloudProvider();
            cde.CreateAgents(provider);

            var th = new Thread(() => cde.CheckConditionsAndRaiseEvents());
            th.Start();
        }

        private static void Test2()
        {
            var spec = File.ReadAllText("test_specification.yaml");
            var t = new SpecificationTransformer(spec);
            var mod = t.Transform();
            var specObj = new Parser().Parse(new CharacterStream(mod));
            var cde = new ExtendedCloudDeploymentEngine(specObj);
            var provider = new CloudProvider();
            cde.CreateAgents(provider);

            var th = new Thread(() => cde.CheckConditionsAndRaiseEvents());
            th.Start();

            Thread.Sleep(3000);
            var newspec = File.ReadAllText("new_specification.yaml");
            var newt = new SpecificationTransformer(newspec);
            var newmod = newt.Transform();
            var newspecObj = new Parser().Parse(new CharacterStream(newmod));
            cde.UpdateSpecification(newspecObj, provider);
        }
    }
}