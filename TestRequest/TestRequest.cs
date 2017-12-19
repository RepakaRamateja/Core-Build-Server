/////////////////////////////////////////////////////////////////////
// TestRequest.cs - build and parse test requests                  //
//                                                                 //
// Author: Jim Fawcett, CST 4-187, jfawcett@twcny.rr.com           //
// Application: CSE681-Software Modeling and Analysis Demo         //
// Environment: C# console                                         //
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * Creates and parses TestRequest XML messages using XDocument
 * 
 * Public Interface
 * ---------------------------------------------
 *TestRequest tr = new TestRequest(); //create test request
 * tr.makeRequest()  //for XML document that represents a test request
 * tr.loadXml(string path)  //load TestRequest from XML file
 * tr.parse(string propertyName)   //parse document for property value 
 * tr.parseList(string propertyName)  //parse document for property list
 * 
 * Required Files:
 * ---------------
 * TestRequest.cs
 * 
 *Build Process
 *-------------
 * Required files:TestRequest.cs
 * 
 * Compiler command: csc  TestRequest.cs 
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 07 Sep 2017
 * - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HelpSession
{
  ///////////////////////////////////////////////////////////////////
  // TestRequest class

  class TestRequest
  {
    public string author { get; set; } = "";
    public string dateTime { get; set; } = "";
    public string testDriver { get; set; } = "";
    public List<string> testedFiles { get; set; } = new List<string>();
    public XDocument doc { get; set; } = new XDocument();

    /*----< build XML document that represents a test request >----*/

    public void makeRequest()
    {
      XElement testRequestElem = new XElement("testRequest");
      doc.Add(testRequestElem);

      XElement authorElem = new XElement("author");
      authorElem.Add(author);
      testRequestElem.Add(authorElem);

      XElement dateTimeElem = new XElement("dateTime");
      dateTimeElem.Add(DateTime.Now.ToString());
      testRequestElem.Add(dateTimeElem);

      XElement testElem = new XElement("test");
      testRequestElem.Add(testElem);

      XElement driverElem = new XElement("testDriver");
      driverElem.Add(testDriver);
      testElem.Add(driverElem);

      foreach (string file in testedFiles)
      {
        XElement testedElem = new XElement("tested");
        testedElem.Add(file);
        testElem.Add(testedElem);
      }
    }
    /*----< load TestRequest from XML file >-----------------------*/

    public bool loadXml(string path)
    {
      try
      {
        doc = XDocument.Load(path);
        return true;
      }
      catch(Exception ex)
      {
        Console.Write("\n--{0}--\n", ex.Message);
        return false;
      }
    }
    /*----< save TestRequest to XML file >-------------------------*/

    public bool saveXml(string path)
    {
      try
      {
        doc.Save(path);
        return true;
      }
      catch(Exception ex)
      {
        Console.Write("\n--{0}--\n", ex.Message);
        return false;
      }
    }
    /*----< parse document for property value >--------------------*/

    public string parse(string propertyName)
    {

      string parseStr = doc.Descendants(propertyName).First().Value;
      if (parseStr.Length > 0)
      {
        switch (propertyName)
        {
          case "author":
            author = parseStr;
            break;
          case "dateTime":
            dateTime = parseStr;
            break;
          case "testDriver":
            testDriver = parseStr;
            break;
          default:
            break;
        }
        return parseStr;
      }
      return "";
    }
    /*----< parse document for property list >---------------------*/
    /*
    * - now, there is only one property list for tested files
    */
    public List<string> parseList(string propertyName)
    {
      List<string> values = new List<string>();

      IEnumerable<XElement> parseElems = doc.Descendants(propertyName);

      if (parseElems.Count() > 0)
      {
        switch (propertyName)
        {
          case "tested":
            foreach (XElement elem in parseElems)
            {
              values.Add(elem.Value);
            }
            testedFiles = values;
            break;
          default:
            break;
        }
      }
      return values;
    }
  }
  ///////////////////////////////////////////////////////////////////
  // test_TestRequest class

  class Test_TestRequest
  {
#if (TEST_X)
    static void Main(string[] args)
    {
      Console.Write("\n  Testing TestRequest");
      Console.Write("\n =====================");

      string savePath = "../../test/";
      string fileName = "TestRequest1.xml";

      if (!System.IO.Directory.Exists(savePath))
        System.IO.Directory.CreateDirectory(savePath);
      string fileSpec = System.IO.Path.Combine(savePath, fileName);
      fileSpec = System.IO.Path.GetFullPath(fileSpec);

      TestRequest tr = new TestRequest();
      tr.author = "Jim Fawcett";
      tr.testDriver = "td1.cs";
      tr.testedFiles.Add("tf1.cs");
      tr.testedFiles.Add("tf2.cs");
      tr.testedFiles.Add("tf3.cs");
      tr.makeRequest();
      Console.Write("\n{0}", tr.doc.ToString());

      Console.Write("\n  saving to \"{0}\"", fileSpec);
      tr.saveXml(fileSpec);

      Console.Write("\n  reading from \"{0}\"", fileSpec);

      TestRequest tr2 = new TestRequest();
      tr2.loadXml(fileSpec);
      Console.Write("\n{0}", tr2.doc.ToString());
      Console.Write("\n");

      tr2.parse("author");
      Console.Write("\n  author is \"{0}\"", tr2.author);

      tr2.parse("dateTime");
      Console.Write("\n  dateTime is \"{0}\"", tr2.dateTime);

      tr2.parse("testDriver");
      Console.Write("\n  testDriver is \"{0}\"", tr2.testDriver);

      tr2.parseList("tested");
      Console.Write("\n  testedFiles are:");
      foreach(string file in tr2.testedFiles)
      {
        Console.Write("\n    \"{0}\"", file);
      }
      Console.Write("\n\n");
    }
  }
#endif
}

