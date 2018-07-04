using HangfireData;
using Microsoft.Extensions.Configuration;
using System;

namespace HangfireDashboard
{
    public class ConnectionStringManager : IConnectionStringManager
    {
        public string AppDatabase { get; private set; }
        public ConnectionStringManager(IConfiguration config)
        {
            AppDatabase = DecryptUserPass(config.GetConnectionString("AppDatabase"));
        }

        private string DecryptUserPass(string str)
        {
            var pieces = str.Split(';');
            for (var i = 0; i < pieces.Length; i++)
            {
                if (pieces[i].StartsWith("user", StringComparison.OrdinalIgnoreCase) || pieces[i].StartsWith("password", StringComparison.OrdinalIgnoreCase))
                {
                    var userPieces = pieces[i].Split('=');
                    if (userPieces.Length == 2)
                    {
                        pieces[i] = $"{userPieces[0]}={ConnectionStringCrypto.Decrypt(userPieces[1])}";
                    }
                }
            }
            return string.Join(';', pieces);
        }
    }
}
