//Setup
dotnet new

//Database

//Testing

dotnet new xunit -n AppName.Tests
cd Appname.Tests
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package FluentAssertions

cd ../

dotnet sln add AppName.API/
dotnet sln add AppName.Test/

cd App.Api/
dotnet add reference ../App.Api/
cd ../App.Test/
dotnet add reference ../App.Test/

//Logging

//Code Coverage
dotnet add package coverlet.collector 
or 
dotnet add package coverlet.msbuild