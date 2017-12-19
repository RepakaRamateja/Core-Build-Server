///////////////////////////////////////////////////////////////////////////////////////////////////
// Buildserver.cs - Demonstrate Build Server  operations                                         //
//                                                                                               //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 2-Core Build Server                                               //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * Demonstrates Build Server  operations like parsing build request ,generating dll files, 
 * generating test request,send files to Test Harness
 * 
 * Public Interface
 * ----------------
 * Buildserver buildserver = new Buildserver();// Builder server instantiation
 * buildserver.getbuilderfiles()//get the builder file from the builder repo location
 * buildserver.processbuildrequest(string path, RepoMock mockobj)//core logic to process build request
 * buildserver.processdircontents(List<string> direc)// get the names of files list from the absolute path file list
 * buildserver.deletetempfiles(string path)//logic to delete temparary files
 * buildserver.deleteallfiles(string path)//logic to   delete all files
 * buildserver.generatedllfiles(List<string> Files, Dictionary<string,string> dict,string dir)//core logic to generate dll files using process class
 * buildserver.sendFile(string fileSpec,string receivePath)//copy file to RepoMock.receivePath
 * buildserver.generatetestrequest(string buildrequest,List<string> outputfilelist)//used for generating the test request
 * buildserver.processdllrequest(string name)//processing the dllrequest for the given file name
 * 
 * Required Files:
 * ---------------
 * Buildserver.cs, RepoMock.cs, BuildRequest.cs, Serialization.cs 
 * 
 * Build Process
 * -------------
 * Required files: Buildserver.cs, RepoMock.cs, BuildRequest.cs, Serialization.cs 
 * Compiler command: csc Buildserver.cs RepoMock.cs BuildRequest.cs Serialization.cs 
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
using Build_Request;
using Utilities;
using Federation;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TestHarnessMessages;

namespace BuildServer
{
    // class which contains  operations related to build server
    public class Buildserver
    {
        RepoMock repomock = null;
        Dictionary<string, string> dict = new Dictionary<string, string>();
        //<-----------Contains logic to get the builder file from the builder repo location----------------> 
        public List<string> getbuilderfiles()
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
        //<------------------ Contains core logic to process build request-----------------------------------> 
        public Dictionary<string,string> processbuildrequest(string path, RepoMock mockobj)
        {
            Console.WriteLine("---------------------------------Build Server processing build request-----------------------------------");
            Console.WriteLine("\n");
            string xmlstring = File.ReadAllText(path);

            BuildRequest newRequest = xmlstring.FromXml<BuildRequest>();

            repomock = mockobj;

            foreach (BuildItem item in newRequest.Builds)
            {
                string child = ""; 
                List<string> parent = new List<string>();
                foreach (file f in item.driver)
                {
                    parent.Add(f.name);
                    Console.WriteLine("requesting  " + f.name);Console.WriteLine("\n");
                    repomock.processfilerequest(f.name);
                }

                foreach (file f in item.sourcefiles)
                {
                    child =child+f.name+ "  ";
                    Console.WriteLine("requesting  " + f.name);Console.WriteLine("\n");
                    repomock.processfilerequest(f.name);
                }

                foreach(string str in parent)
                {
                    dict.Add(str, child);
                }

            }

            return dict;

        }

        //<---------used to get the names of files list from the absolute path file list-------------->
        public List<string> processdircontents(List<string> direc)
        {
            List<string> onlynames = new List<string>();

            foreach (string f in direc)
            {
                string fileName = Path.GetFileName(f);
                onlynames.Add(fileName);
            }
            return onlynames;
        }


        //<----------contains logic to delete temparary files--------------------------------->
        public void deletetempfiles(string path)
        {
            string[] picList = Directory.GetFiles(path, "*.dll");
            foreach (string f in picList)
            {
                File.Delete(f);
            }
        }

        //<----------contains logic to delete all files in a directory--------------------------------->
        public void deleteallfiles(string path)
        {
            string[] picList = Directory.GetFiles(path, "*.*");
            foreach (string f in picList)
            {
                File.Delete(f);
            }
        }


        //<-------------------contains the core logic to generate dll files using process class---------------------->
        public List<string> generatedllfiles(List<string> Files, Dictionary<string,string> dict,string dir)
        {
            List<string> outputfilelist = new List<string>();deletetempfiles(dir);deleteallfiles("../../../MockTestHarness/DLLRepository"); Console.WriteLine("\n");Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Requirement 5&6  Build Server started processing Source code files and displays sucess,error,warning messages if any"); Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");Console.WriteLine("\n"); Console.WriteLine("\n");
            foreach (string file in Files)
            { 
                Process first = new Process();string dllfile = file;dllfile = dllfile.Replace(".cs", ".dll");
                try
                {
                    string temp = null;
                    if (file.Contains("Driver"))
                    temp = file + "   " + dict[file];
                    else
                    temp = file;
                        var frameworkPath = RuntimeEnvironment.GetRuntimeDirectory();
                        var cscPath = Path.Combine(frameworkPath, "csc.exe"); first.StartInfo.FileName = cscPath;
                        first.StartInfo.Arguments = "/warn:4  /target:library " + temp; first.StartInfo.UseShellExecute = false;
                        first.StartInfo.WorkingDirectory = "../../../BuildServer/BuilderStorage"; first.StartInfo.RedirectStandardOutput = true;
                        first.StartInfo.RedirectStandardError = true; first.Start();
                        string output = first.StandardOutput.ReadToEnd(); string error = first.StandardError.ReadToEnd();
                        if (output != null)
                        {
                            string[] tempFiles = Directory.GetFiles("../../../BuildServer/BuilderStorage/", "*.dll");
                            for (int i = 0; i < tempFiles.Length; ++i)
                            {
                                tempFiles[i] = Path.GetFileName(tempFiles[i]);
                            }
                            if (tempFiles.Contains(dllfile))
                            {
                                outputfilelist.Add(dllfile); Console.WriteLine("Build Successful For : " + file); Console.WriteLine("\n");
                                Console.WriteLine(" Generated-------->" + dllfile + "  in ../../../BuildServer/BuilderStorage"); Console.WriteLine("\n");
                            }
                            else
                            {
                                Console.WriteLine("BUILDNOTSUCCESSFULL:" + temp); Console.WriteLine("\n"); Console.WriteLine(output);
                            }
                        }
                        if (error != null)
                        {
                            Console.WriteLine(error);
                        }
                        first.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("The Exception caught is "+e.Message);
                } 
            }
            return outputfilelist;
        }


        /*---< copy file to RepoMock.receivePath >---------------------*/
        /*
        *  Will overwrite file if it exists. 
        */
        public bool sendFile(string fileSpec,string receivePath)
        {
            try
            {
                string fileName = Path.GetFileName(fileSpec);
                string destSpec = Path.Combine(receivePath, fileName);
                File.Copy(fileSpec, destSpec, true);
                Console.WriteLine("file sent to mock test harness dllrepository storage ../../../MockTestHarness/DLLRepository");
                return true;
            }
            catch (Exception ex)
            {
                Console.Write("\n--{0}--", ex.Message);
                return false;
            }
        }

        //<----------------------------------used for generating the test request------------------------------------>
        public void generatetestrequest(string buildrequest,List<string> outputfilelist)
        {
            Console.WriteLine("--------------------------------------Build Server Creating Test Request---------------------------------------");
            Console.WriteLine("\n");
            string xmlstring = File.ReadAllText(buildrequest);
            BuildRequest newRequest = xmlstring.FromXml<BuildRequest>();
            TestRequest testRequest = new TestRequest();
            testRequest.author = "Ramteja Repaka";
            foreach (BuildItem item in newRequest.Builds)
            {
                TestElement element = new TestElement();
                element.testName = item.builddesc;
                string testdrivername = null;
                foreach (file f in item.driver)
                {
                    String s = f.name;
                   s=s.Replace(".cs", ".dll");
                    element.addDriver(s);
                    testdrivername = s;

                }

                foreach (file f in item.sourcefiles)
                {
                    String s = f.name;
                    s = s.Replace(".cs", ".dll");
                    element.addCode(s);
                }
                if (outputfilelist.Contains(testdrivername))
                {
                    testRequest.tests.Add(element);
                }
            }

            string testxml = testRequest.ToXml();
            File.WriteAllText("../../../BuildServer/BuilderStorage/TestRequest.xml",testxml);
            Console.WriteLine("TestRequest saved to BuilderStorage\n");
            Console.WriteLine("Sending test Request to mock test harness\n");
            sendFile("../../../BuildServer/BuilderStorage/TestRequest.xml", "../../../MockTestHarness/DLLRepository");
            Console.WriteLine("\n");
        }

        //<----------------used for processing the dllrequest for the given file name-------------------------------->
        public void processdllrequest(string name)
        {
            string destSpec = Path.Combine("../../../BuildServer/BuilderStorage", name);
            sendFile(destSpec, "../../../MockTestHarness/DLLRepository");  
        }

       
     //<--------------------------------------------TEST STUB-------------------------------------------------->
#if (TEST_BuildServer)
        class testbuildserver
        {
            //<-----------------------------------------Driver Logic------------------------------------------->
            static void Main(string[] args)
            {
                RepoMock repomock = new RepoMock();
                Buildserver buildserver = new Buildserver();
                repomock.storagePath = "../../../RepoMock/RepoStorage";
                repomock.receivePath = "../../../BuildServer/BuilderStorage";
                repomock.getFiles("*.*");
                Dictionary<string, string> dict = buildserver.processbuildrequest("../../../BuildServer/BuilderStorage/BuildRequest.xml", repomock);
                List<string> files = buildserver.getbuilderfiles();
                files = buildserver.processdircontents(files);
               List<string> outputlist=buildserver.generatedllfiles(files, dict,"../../../BuildServer/BuilderStorage");
                buildserver.generatetestrequest("../../../BuildServer/BuilderStorage/BuildRequest.xml",outputlist);
            }
        }
#endif
    }
}
