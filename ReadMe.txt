
  Mock client sends Build request to RepoMock as string 

    if it is valid then it is saved in the RepoMock/RepoStorage folder

  On command RepoMock sends the request to BuildServer
   
    BuildServer process request and gets all the source files

    BuildServer generate dll files in the  BuildServer/BuilderStorage

    BuildServer generate testRequest and sent it to the MockTestHarness/DLLRepository

     MockTestHarness process the testrequest and gets all the dll files 

    Then it starts loading all the files