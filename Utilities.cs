using DotNetEnv;
using System.Text;
using System.Text.Json;

namespace AquireBearerToken
{
    public class Utilities
    {
        public static void ReadEnvFile()
        {
            var envFile = Env.Load();
            if (!envFile.Any())
            {
                envFile = Env.TraversePath().Load();
            }
            return;
        }

        public static void ParseJWTToken(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                Console.WriteLine("JWT Token is null or empty.");
                return;
            }
            var parts = jwtToken.Split('.');
            if (parts.Length != 3)
            {
                Console.WriteLine("Invalid JWT Token format.");
                return;
            }
            var header = parts[0];
            var payload = parts[1];
            var signature = parts[2];

            Console.WriteLine($"Header: {header}");
            Console.WriteLine($"Payload: {payload}");
            Console.WriteLine($"Signature: {signature}");

            // Decode the payload
            string decodedPayload = DecodeBase64Url(payload);
            Console.WriteLine($"Decoded Payload: {decodedPayload}");

            // Parse JSON and extract "exp"
            try
            {
                using var doc = JsonDocument.Parse(decodedPayload);
                if (doc.RootElement.TryGetProperty("exp", out var expElement))
                {
                    long exp = expElement.GetInt64();
                    DateTimeOffset expiration = DateTimeOffset.FromUnixTimeSeconds(exp);
                    Console.WriteLine($"Expiration (exp): {exp} ({expiration.UtcDateTime} UTC)");
                    Console.WriteLine($"Expiration (exp) in local time: {expiration.LocalDateTime}");
                    if(expiration.UtcDateTime < DateTimeOffset.UtcNow)
                    {
                        Console.WriteLine("The token has expired.");
                    }
                    else
                    {
                        Console.WriteLine("The token is still valid.");
                    }
                }
                else
                {
                    Console.WriteLine("No 'exp' claim found in payload.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse payload: {ex.Message}");
            }
        }

        private static string DecodeBase64Url(string base64Url)
        {
            string padded = base64Url.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            var bytes = Convert.FromBase64String(padded);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
