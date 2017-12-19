///////////////////////////////////////////////////////////////////////////////////////////////////
// Client.cs - Demonstrate Mock Client  operations                                               //
//                                                                                               //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 2-Core Build Server                                               //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * Demonstrates Mock Client  operations like makebuildrequest
 * 
 * Public Interface
 * ----------------
 * Client client = new Client();//used for all client operations
 * client.setXmlRequest(request); //set the xml request
 * client.getXmlRequest();//get the xml request
 * BuildRequest request=client.makerequest();//used for creating build request
 * 
 * Required Files:
 * ---------------
 * RepoMock.cs, BuildRequest.cs, Serialization.cs 
 * 
 * Build Process
 * -------------
 * Required files:   RepoMock.cs, BuildRequest.cs, Serialization.cs 
 * 
 * Compiler command: csc RepoMock.cs BuildRequest.cs Serialization.cs 
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 05 Oct 2017
 * - first release
 * 
 */
using Build_Request;
using Federation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace MockClient
{
    // Class which contains operations related to the client 
    public class Client
    {
        BuildRequest tr;

         string XmlRequest = null;

        //<--------------------------constructor initializing variable BuildRequest---------------------->
        public Client()
        {
            tr = new BuildRequest();
        }
        //<--------------------------method to set the xml request member variable--------------------------------->
        public void setXmlRequest(string request)
        {
            XmlRequest = request;
        }
        //<--------------------------method to get the xml request member variable---------------------------------->
        public string getXmlRequest()
        {
            return XmlRequest;
        }

        //<-------------------------------used for creating build request---------------------------------->
        public BuildRequest makerequest()
        {
            Console.WriteLine("\n");
            Console.WriteLine("---------------- MockClient generated build request which is displayed below-----------------");
            BuildItem te1 = new BuildItem();
            te1.builddesc = "firstproject";
            file driver = new file();
            driver.name="TestDriver.cs";
            te1.addDriver(driver);
            file one = new file();
            one.name="TestedOne.cs";
            file two = new file();
            two.name="TestedTwo.cs";
            te1.addCode(one);
            te1.addCode(two);
            tr.author = "Rama Teja Repaka";
            BuildItem te3 = new BuildItem();
            te3.builddesc = "Secondproject";
            file driver3 = new file();
            driver3.name = "SecondTestDriver.cs";
            te3.addDriver(driver3);
            file five = new file();
            five.name = "TestedLibDependency.cs";
            file six = new file();
            six.name = "TestedLib.cs";
            file seven = new file();
            seven.name = "Interfaces.cs";
            te3.addCode(five);te3.addCode(six);te3.addCode(seven);
            tr.Builds.Add(te1);
            tr.Builds.Add(te3);
            XmlRequest = tr.ToXml();
            Console.WriteLine("\n");
            Console.WriteLine(XmlRequest);
            return tr;
        }

    }

    //<------------------------------------------------TEST STUB------------------------------------------->
#if (TEST_Client)
    class TestClient
    {
   //<-----------------------------------------Driver Logic------------------------------------------->
        static void Main(string[] args)
        {
            Client client = new Client();
            BuildRequest request = client.makerequest();
        }
    }
#endif
}
