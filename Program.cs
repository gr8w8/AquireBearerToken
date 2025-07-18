﻿using AquireBearerToken;
using Microsoft.Identity.Client;

#region Class Variables
// Load the ENV file
Utilities.ReadEnvFile();

string TenantId = Environment.GetEnvironmentVariable("TENANT_ID") ?? "";
string ClientId = Environment.GetEnvironmentVariable("CLIENT_ID") ?? "";
string Scope = "api://" + ClientId + "/.default"; // The scope you want to access
string FilePath = Environment.GetEnvironmentVariable("FILE_PATH") ??"";
string[] Scopes = [Scope];
#endregion

#region Getting the Bearer Token using Device Code Flow
IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
    .Create(ClientId)
    .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
    .WithDefaultRedirectUri()
    .Build();

// Use the device code flow to acquire a token
var bearerToken = await publicClientApplication.AcquireTokenWithDeviceCode(Scopes, deviceCodeResult =>
{
    // Display the message to the user
    Console.WriteLine(deviceCodeResult.Message);

    return Task.CompletedTask;

}).ExecuteAsync();

// Write the access token to the console
Console.WriteLine($"Access Token: {bearerToken.AccessToken}");
Console.WriteLine($"Expires On: {bearerToken.ExpiresOn}");
Console.WriteLine($"Press any key to write the access token to {FilePath}");
Console.ReadLine();
// Write to a file
File.WriteAllText(FilePath, bearerToken.AccessToken);
Console.WriteLine($"Access token written to {FilePath}");
Console.WriteLine("Press any key to exit the application.");
// Wait for user input before closing the console window
Console.ReadLine();
#endregion