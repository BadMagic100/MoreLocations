using ItemChangerTesting;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoreLocationsICTesting
{
    public class MoreLocationsICTests : Mod
    {
        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public override int LoadPriority() => 99999;

        public override void Initialize()
        {
            Log("Initializing");

            ItemChangerTestingMenu.TestInjectors += InjectTests;

            Log("Initialized");
        }

        private IEnumerable<Test> InjectTests()
        {
            IEnumerable<Type> types = typeof(MoreLocationsICTests).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(Test).IsAssignableFrom(t));
            foreach (Type t in types)
            {
                if (t.GetConstructor(Array.Empty<Type>())?.Invoke(Array.Empty<object>()) is Test test)
                {
                    yield return test;
                }
            }
        }
    }
}
