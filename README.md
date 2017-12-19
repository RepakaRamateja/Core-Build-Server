This is Software modelling analysis course project 


####################     Core Build Server   ####################

Background Information:

In order to successfully implement big systems we need to partition code into relatively small parts and thoroughly test each of the parts before inserting them into the software baseline2. As new parts are added to the baseline and as we make changes to fix latent errors or performance problems we will re-run test sequences for those parts and, perhaps, for the entire baseline. Because there are so many packages the only way to make this intensive testing practical is to automate the process.


The process, described above, supports continuous integration. That is, when new code is created for a system, we build and test it in the context of other code which it calls, and which call it. As soon as all the tests pass, we check in the code and it becomes part of the current baseline. There are several services necessary to efficiently support continuous integration.


a Federation of servers, each providing a dedicated service for continuous integration.

The Federation consists of:


Repository:
Holds all code and documents for the current baseline, along with their dependency relationships. It also holds test results and may cache build images.


Build Server:
Based on build requests and code sent from the Repository, the Build Server builds test libraries for submission to the Test Harness. On completion, if successful, the build server submits test libraries and test requests to the Test Harness, and sends build logs to the Repository.



Test Harness:
Runs tests, concurrently for multiple users, based on test requests and libraries sent from the Build Server. Clients will checkin, to the Repository, code for testing, along with one or more test requests. The repository sends code and requests to the Build Server, where the code is built into libraries and the test requests and libraries are then sent to the Test Harness. The Test Harness executes tests, logs results, and submits results to the Repository. It also notifies the author of the tests of the results.



Client:
The user's primary interface into the Federation, serves to submit code and test requests to the Repository. Later, it will be used view test results, stored in the repository.





This project mainly focuses on building the core Build Server Functionality, and thoroughly testing to ensure that it functions as expected.



Requirements:



Shall be prepared using C#, the .Net Frameowrk, and Visual Studio 2017.

Shall include packages for an Executive, mock Repository, and mock Test Harness, as well as packages for the Core Project Builder.


The Executive shall construct a fixed sequence of operations of the mock Repository, mock Test Harness, and Core Project Builder, to demonstrate Builder operations.

The mock Repository shall, on command, copy a set of test source code files, test drivers, and a test request3 with a test for each test driver, to a path known to the Core Project Builder.


The Core Project Builder shall attempt to build each Visual Studio project delivered by the mock Repository, using the delivered code.



The Core Builder shall report, to the Console, success or failure of the build attempt, and any warnings emitted.



The Core Builder shall, on success, deliver the built library to a path known by the mock Test Harness.



The mock Test Harness shall attempt to load and execute each test library, catching any execeptions that may be emitted, and report sucess or failure and any exception messages, to the Console.



One interesting issue: it would be desireable to build sources from several often used languages, e.g., C#, C++, Java, etc. If you base your build process on the MSBuild library, that won't work for Java and won't work for anything on Linux platforms. Building for multiple platforms with multiple source languages may not be as difficult as it sounds. The idea is to create your own Build infrastruction that uses the compilers and their tool chains for the target platform. Essentially, you need to set up a configuration file for each platform and toolchain that identifies the paths to the tools you need, and translates your Builders commands into those needed by specific tool chains.