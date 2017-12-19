///////////////////////////////////////////////////////////////////////////////////////////////////
// TestMockHarness.cs - Demonstrate MocktestHarness  operations                                               //
//                                                                                               //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 2-Core Build Server                                               //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * Demonstrates MockTestHarness  operations like processingtestrequest,LoadingFromComponentLibFolder
 * loadingAndExercisingTesters,runningSimulatedTest
 * 
 * Public Interface
 * ----------------
 * TestMockHarness harness = new TestMockHarness();//used for TestHarness operations
 * harness.processtestrequest(string path, Buildserver serv) //contains logic to process the test request
 * harness.loadAndExerciseTesters();//load assemblies from testersLocation and run their tests
 *
 * 
 * Required Files:
 * ---------------
 * BuildRequest.cs, Serialization.cs,THMessages.cs, TestMockHarness.cs
 * 
 * Build Process
 * -------------
 * Required files: TestMockHarness.cs, BuildRequest.cs, Serialization.cs,THMessages.cs
 * 
 * Compiler command: csc TestMockHarness.cs BuildRequest.cs  Serialization.cs THMessages.cs
 * 
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 05 Oct 2017
 * - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHarnessMessages;
using Utilities;
using BuildServer;
using System.Reflection;

namespace MockTestHarness
{
    //Class contains operations related to MockTestHarness
    public class TestMockHarness
    {
        Buildserver server = null;

        public static  string testersLocation { get; set; } = ".";

        //<--------contains logic to process the test request------------------------------>
        public void processtestrequest(string path, Buildserver serv)
        {
            server = serv;
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Requirement 7 MockTestHarness processing test request and gets successful built library from build server");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            string xmlstring = File.ReadAllText(path);
            Console.WriteLine("\n");
            Console.WriteLine("Test Request:");
            Console.WriteLine(xmlstring);
            TestRequest newRequest = xmlstring.FromXml<TestRequest>();
            foreach (TestElement te in newRequest.tests)
            {
                Console.WriteLine("\n");
                Console.WriteLine("requesting  " + te.testDriver);
                Console.WriteLine("\n");
                server.processdllrequest(te.testDriver);

                foreach (string testCode in te.testCodes)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("requesting  " + testCode);
                    Console.WriteLine("\n");
                    server.processdllrequest(testCode);

                }
            }

            Console.WriteLine("\n");


        }

        /*----< library binding error event handler >------------------*/
        /*
         *  This function is an event handler for binding errors when
         *  loading libraries.  These occur when a loaded library has
         *  dependent libraries that are not located in the directory
         *  where the Executable is running.
         */
        static Assembly LoadFromComponentLibFolder(object sender, ResolveEventArgs args)
        {
            Console.Write("\n  called binding error event handler");
            string folderPath = testersLocation;
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        //----< load assemblies from testersLocation and run their tests >-----

        public string loadAndExerciseTesters()
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Requirement 8 MockTestHarness  loading dll files and displays test status success,failure,exceptions if any"); Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromComponentLibFolder);

            try
            {
                TestMockHarness loader = new TestMockHarness();

                // load each assembly found in testersLocation

                string[] files = Directory.GetFiles(testersLocation, "*.dll");
                foreach (string file in files)
                {

                    //Assembly asm = Assembly.LoadFrom(file);
                    Assembly asm = Assembly.LoadFile(file);
                    string fileName = Path.GetFileName(file);
                    Console.Write("\n  loaded {0}", fileName);

                    // exercise each tester found in assembly

                    Type[] types = asm.GetTypes();
                    foreach (Type t in types)
                    {
                        // if type supports ITest interface then run test

                        if (t.GetInterface("TestBuild.ITest", true) != null)
                            if (!loader.runSimulatedTest(t, asm))
                                Console.Write("\n  test {0} failed to run", t.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Simulated Testing completed";
        }
        //
        //----< run tester t from assembly asm >-------------------------------

        bool runSimulatedTest(Type t, Assembly asm)
        {
            try
            {
                Console.WriteLine("\n");
                Console.Write(
                  "\n  attempting to create instance of {0}", t.ToString()
                  );
                object obj = asm.CreateInstance(t.ToString());
                MethodInfo method1 = t.GetMethod("say"); 
                if (method1 != null)
                    method1.Invoke(obj, new object[0]);
                bool status = false;
                MethodInfo method = t.GetMethod("test");
                if (method != null)
                    status = (bool)method.Invoke(obj, new object[0]);

                Func<bool, string> act = (bool pass) =>
                {
                    if (pass)
                        return "passed";
                    return "failed";
                };
                Console.Write("\n  test {0}", act(status)); Console.WriteLine("\n");
            }
            catch (Exception ex)
            {
                Console.Write("\n  test failed with message \"{0}\"", ex.Message);
                return false;
            }

            
            return true;
        }
        //
    }

    #if (TEST_Harness)
    //<---------------------------------------TEST STUB------------------------------------------------>
    class test_TestHarness
    {
        //<-----------------------------------------Driver Logic------------------------------------------->
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstrating Robust Test Loader");
            Console.Write("\n ==================================\n");
            TestMockHarness harness = new TestMockHarness();
            TestMockHarness.testersLocation = "../../DLLRepository";
            TestMockHarness.testersLocation = Path.GetFullPath(TestMockHarness.testersLocation);
            string result = harness.loadAndExerciseTesters();
            Console.Write("\n\n  {0}", result);
            Console.Write("\n\n");
        }
    }
#endif
}
