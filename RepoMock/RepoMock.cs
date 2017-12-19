///////////////////////////////////////////////////////////////////////////////////////////////////
// RepoMock.cs - Demonstrate RepoMock  operations                                                //
//                                                                                               //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 2-Core Build Server                                               //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * Demonstrates Mock Repo  operations like parsingbuildmessage,processingfilerequest,sending files to buildserver
 * processing given commands, processing given directory contents
 * 
 *  Public Interface
 * ----------------
 * RepoMock repo = new RepoMock();// for doing mock repo operations 
 * repo.processfilerequest(string filename)//to handle the file request
 * repo.processcommand(string command,string sourcepath,string recievepath)//to process the  command input
 * repo.savecontent(string xmlcontent,string dirpath)//for saving the content to given directory path
 * repo.parsebuildmessage(BuildRequest request,List<string> dircontents)//for parsing buildmessage
 * repo.processdircontents(List<string> direc)//for processing directory file names
 * repo.sendFile(string fileSpec) //copy file to RepoMock.receivePath
 * repo.getFiles(string pattern)  // for getting files from the given pattern
 * 
 * Required Files:
 * ---------------
 * BuildRequest.cs,RepoMock.cs
 * 
 *  Build Process
 * --------------
 * Required files: BuildRequest.cs,RepoMock.cs
 * 
 * Compiler command: csc  BuildRequest.cs RepoMock.cs
 * 
 * Maintenance History:
 * --------------------
 *ver 1.0 : 08 Oct 2017
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Build_Request;
using Utilities;

namespace Federation
{
    ///////////////////////////////////////////////////////////////////
    // RepoMock class
    // - begins to simulate basic Repo operations
    public class RepoMock
    {
        public string storagePath { get; set; } = "../../RepoStorage";
        public string receivePath { get; set; } = "../../../BuildServer/BuilderStorage";
        public List<string> files { get; set; } = new List<string>();

        /*----< initialize RepoMock Storage>---------------------------*/
        public RepoMock()
        {
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);
            if (!Directory.Exists(receivePath))
                Directory.CreateDirectory(receivePath);
        }
        /*----< private helper function for RepoMock.getFiles >--------*/

        private void getFilesHelper(string path, string pattern)
        {
            string[] tempFiles = Directory.GetFiles(path, pattern);
            for (int i = 0; i < tempFiles.Length; ++i)
            {
                tempFiles[i] = Path.GetFullPath(tempFiles[i]);
            }
            files.AddRange(tempFiles);

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                getFilesHelper(dir, pattern);
            }
        }
        /*----< find all the files in RepoMock.storagePath >-----------*/
        /*
        *  Finds all the files, matching pattern, in the entire 
        *  directory tree rooted at repo.storagePath.
        */

        //<-----------contains logic for getting files from the given pattern-------------------->
        public void getFiles(string pattern)
        {
            files.Clear();
            getFilesHelper(storagePath, pattern);
        }
        /*---< copy file to RepoMock.receivePath >---------------------*/
        /*
        *  Will overwrite file if it exists. 
        */
        public bool sendFile(string fileSpec)
        {
            try
            {
                string fileName = Path.GetFileName(fileSpec);
                string destSpec = Path.Combine(receivePath, fileName);
                File.Copy(fileSpec, destSpec, true);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write("\n--{0}--", ex.Message);
                return false;
            }
        }

        //<-----------contains logic for processing directory file names---------------------------->
        public List<string> processdircontents(List<string> direc)
        {
            List<string> onlynames=new List<string>();
            foreach (string f in direc)
            {
                string fileName = Path.GetFileName(f);
                onlynames.Add(fileName);
            }
            return onlynames;
        }
  
        //<--------------contains logic for parsing buildmessage----------------------------------->
        public Boolean parsebuildmessage(BuildRequest request,List<string> dircontents)
        {
            Boolean found = true;
            dircontents = processdircontents(dircontents);
            Console.WriteLine("\n");
            Console.WriteLine("-------------------------------------------Mock Repo parsing build message----------------------------------------"); Console.WriteLine("\n");
            Console.WriteLine("Mockrepo repositorystorage relative path is  " + "../../../RepoMock/RepoStorage"); Console.WriteLine("\n");
            foreach (BuildItem item in request.Builds)
            {
                foreach (file f in item.driver)
                {
                    if(dircontents.Contains(f.name))
                    {
                        Console.WriteLine("test driver: "+ f.name+ "  found"); Console.WriteLine("\n");
                    }
                    else
                    {
                        found = false;
                    }
                }
                    foreach (file f in item.sourcefiles)
                    {
                        if (dircontents.Contains(f.name))
                        {
                        Console.WriteLine("source file: " + f.name+ "  found"); Console.WriteLine("\n");
                    }

                    else
                    {
                        found = false;
                    }
                    }
            }

            if(found==false)
            {
                Console.WriteLine("Build request validation failed"); Console.WriteLine("\n");
                return false;
            }

            else
            {
                Console.WriteLine("Build request validation passed"); Console.WriteLine("\n");
                return true;
            }
        }

        //<---------contains logic for saving the content to given directory path------------------>
        public void savecontent(string xmlcontent,string dirpath)
        {
            File.WriteAllText(dirpath, xmlcontent);
            Console.WriteLine("Build Request saved in the mockrepo  repositorystorage");
            Console.WriteLine("\n");
        }
        //<---------contains logic to process the  command input------------------------------------>
        public void processcommand(string command,string sourcepath,string recievepath)
        {
            if (command == "buildrequesttobuildsever")
            {
                Console.WriteLine("\n");
                Console.WriteLine("Mockrepo process command  " + command); Console.WriteLine("\n");
                receivePath = recievepath;
                Boolean result=sendFile(sourcepath);
                if(result==true)
                {
                    Console.WriteLine("Build request sent  to the buildserver builderstorage whose relative path is ../../../BuildServer/BuilderStorage"); Console.WriteLine("\n");
                    Console.WriteLine("\n");
                }
            }
        }
        //<----------contains logic to handle the file request-------------------------------------->
        public void processfilerequest(string filename)
        {
            string destSpec = Path.Combine(storagePath, filename);
            sendFile(destSpec);
            Console.WriteLine("File sent to build server builder storage ../../../BuildServer/BuilderStorage");Console.WriteLine("\n");
        }

    }

    //<----------------------------------------TEST STUB--------------------------------------->

#if (TEST_REPOMOCK  )
  ///////////////////////////////////////////////////////////////////
  // TestRepoMock class
  class TestRepoMock
  {
        //<------------------Driver Logic-------------------------------->
    static void Main(string[] args)
    {
      Console.Write("\n  Demonstration of Mock Repo");
      Console.Write("\n ============================");
      RepoMock repo = new RepoMock();
      repo.getFiles("*.*");
      foreach(string file in repo.files)
        Console.Write("\n  \"{0}\"", file);
            Console.Write("before printing the files\n");
      string fileSpec = repo.files[1];
      string fileName = Path.GetFileName(fileSpec);
      Console.Write("\n  sending \"{0}\" to \"{1}\"", fileName, repo.receivePath);
      repo.sendFile(repo.files[1]);
      Console.Write("\n\n");
    }
  }
#endif
}
