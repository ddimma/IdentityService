# README # 

  

Read the instructions below in order to have the Identity Server configured and running properly 

  

### Steps to perform: ### 

  

* 1 Configure your ConnectionString 

  

* 2 Run the migrations commands to build the database (one by one) 

  

``` 

dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext  

dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext  

dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c AspNetIdentityDbContext  

  

dotnet ef database update -c PersistedGrantDbContext  

dotnet ef database update -c ConfigurationDbContext  

dotnet ef database update -c AspNetIdentityDbContext  

``` 

  

* 3 Add the Client-Side libraries 

Find the libman.json file under the IdentityService project root directory

Right-click on it and select Restore Client-Side Libraries option



* 4 Once the server successfully started, go to the https://localhost:7059/Seed url to seed the data from the Config.cs file in the root directory into your database. This will add client, resources, scopes ...



* 5 Enjoy using the Identity Server for the main Book Sharing project



* To register a user on the Identity Server just send the JSON request to the /Account/Register endpoint.