///////////////////////////////////////////////////////////////////////////////////////////////////
// Executive.cs - Demonstrate Executive  operations                                               //
//                                                                                               //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 2-Core Build Server                                               //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * Demonstrates fixed sequence of operations of the mock Repository, mock Test Harness, and Core Project Builder, to demonstrate Builder operations.
 * 
 * Public Interface
 * ----------------
 *  Executive exe = new Executive(); //for executive operations
 *  exe.processdircontents();// get files names from the complete file path
 *  exe.getbuilderfiles();//get the builder files complete list
 *  
 * Required Files:
 * ---------------
 * RepoMock.cs,BuildRequest.cs,TestMockHarness.cs,Buildserver.cs,client.cs,TestRequest.cs
 * 
 * Build Process
 * -------------
 * Required files: RepoMock.cs,BuildRequest.cs,TestMockHarness.cs,Buildserver.cs,client.cs,TestRequest.cs
 * 
 * Compiler command: csc RepoMock.cs  BuildRequest.cs TestMockHarness.cs  Buildserver.cs  client.cs  TestRequest.cs
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 05 Oct 2017
 * - first release
 * 
 */
using Build_Request;
using Federation;
using MockClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildServer;
using System.IO;
using MockTestHarness;
namespace TestExecutive
{
    //Demonstrates executive operations
    class Executive
    {
        //---------------<get the files names from the complete file path>--------------------------- 
        public   List<string> processdircontents(List<string> direc)
        {
            List<string> onlynames = new List<string>();

            foreach (string f in direc)
            {
                string fileName = Path.GetFileName(f);
                onlynames.Add(fileName);
            }
            return onlynames;
        }

        //-----------------<get the builder files complete list>----------------------------------------------
        public  List<string> getbuilderfiles()
        {

            List<string> files = new List<string>();

            string[] tempFiles = Directory.GetFiles("../../../BuildServer/BuilderStorage/", "*.cs");

            for (int i = 0; i < tempFiles.Length; ++i)
            {
                tempFiles[i] = Path.GetFullPath(tempFiles[i]);
            }
            files.AddRange(tempFiles);

            return files;

        }
        
        //------------------<used for demonstrating first few requirements>---------------------------------------
        static void printrequirementsdemo()
        { 
            Console.WriteLine("--------------------------------Test Executive started-------------------------------------");
            Console.WriteLine("\n"); Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Demonstration of Requirement 1"); Console.WriteLine("--------------------------------------------------------------------------------------------------------"); Console.WriteLine("\n");
            Console.WriteLine("Implemented using C# langauge as you can see all .cs files in the SMAPROJECT2 folder");
            Console.WriteLine("\n");
            Console.WriteLine("Using C# Framework Version" + Environment.Version.ToString());
            Console.WriteLine("\n");
            Console.WriteLine("Implemented code using visual studio as you can see the solution file SMAPROJECT2.sln in the SMAPROJECT2 folder");
            Console.WriteLine("\n"); Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Demonstration of Requirement 2"); Console.WriteLine("--------------------------------------------------------------------------------------------------------"); Console.WriteLine("\n");
            Console.WriteLine("Included seperate packages:\n\n Executive \n \n RepoMock\n\n MocktestHarness \n\n BuildServer \n\n some other additional packages \n\n All of them could be found in the SMAPROJECT2 folder"); Console.WriteLine("\n");
            Console.WriteLine("\n"); Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Demonstration of Requirement 3"); Console.WriteLine("--------------------------------------------------------------------------------------------------------"); Console.WriteLine("\n");
            Console.WriteLine(" All the operations of mockclient  mockrepo  buildserver  mocktestharness are displayed below in Sequence Order");
        }
      
        //<-------------------driver logic-------------------------------->
        static void Main(string[] args)
        {
            printrequirementsdemo();
            Executive exe = new Executive();
            Client client = new Client();
            BuildRequest request =client.makerequest();
            RepoMock repomock = new RepoMock();
            Buildserver buildserver = new Buildserver();
            TestMockHarness harness = new TestMockHarness();
            string xmlcontent = client.getXmlRequest();
            repomock.storagePath = "../../../RepoMock/RepoStorage";
            repomock.receivePath = "../../../BuildServer/BuilderStorage";
            repomock.getFiles("*.*");
            Console.WriteLine("\n");
            Console.WriteLine("Client sent build request to MOCK REPOSITORY");
            Boolean validation = repomock.parsebuildmessage(request, repomock.files);
            string buildrequestlocation = "../../../RepoMock/RepoStorage/BuildRequests/BuildRequest.xml";
            if (validation == true)
            {
                repomock.savecontent(xmlcontent, buildrequestlocation);
            }
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Demonstration of Requirement 4 command sent to mock repo");Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
            repomock.processcommand("buildrequesttobuildsever", buildrequestlocation, repomock.receivePath);
          Dictionary<string,string> dict=buildserver.processbuildrequest("../../../BuildServer/BuilderStorage/BuildRequest.xml",repomock);
            List<string> files = exe.getbuilderfiles();
            files= exe.processdircontents(files);
            List<string> outputfilelist=buildserver.generatedllfiles(files,dict, "../../../BuildServer/BuilderStorage");
            buildserver.generatetestrequest("../../../BuildServer/BuilderStorage/BuildRequest.xml", outputfilelist);
            harness.processtestrequest("../../../MockTestHarness/DLLRepository/TestRequest.xml",buildserver);
            TestMockHarness.testersLocation = "../../../MockTestHarness/DLLRepository";
            TestMockHarness.testersLocation = Path.GetFullPath(TestMockHarness.testersLocation);
            string result = harness.loadAndExerciseTesters();
            Console.Write("\n\n  {0}", result);
            Console.Write("\n\n");
        }
    }
}
