cd AzureAutomationFormGenerator.Persistence

//Migrate database
dotnet ef migrations add MyFirstMigration

//Update database
dotnet ef database update