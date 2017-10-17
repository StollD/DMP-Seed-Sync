using DarkMultiPlayerServer;
using SettingsParser;
using System;
using System.ComponentModel;
using System.IO;

namespace DMPSeedSync
{
    /// <summary>
    /// Reads the seed from the server settings and sends it to the clients
    /// </summary>
    public class SyncServer : DMPPlugin
    {
        /// <summary>
        /// The seed for the savegame
        /// </summary>
        public Int32 Seed;

        /// <summary>
        /// Load the seed when the server starts
        /// </summary>
        public override void OnServerStart()
        {
            String path = Path.Combine(Server.configDirectory, "Seed.txt");
            ConfigParser<SeedSettings> seedSettings = new ConfigParser<SeedSettings>(new SeedSettings(), path);
            if (!File.Exists(path))
                seedSettings.SaveSettings();
            seedSettings.LoadSettings();
            String seed = seedSettings.Settings.seed?.Trim();
            if (String.IsNullOrEmpty(seed))
                throw new InvalidOperationException();
            if (Int32.TryParse(seed, out Int32 iSeed))
                Seed = iSeed;
            else
                Seed = seed.GetHashCode();
        }

        /// <summary>
        /// Send the seed to every client that just connected.
        /// </summary>
        public override void OnClientAuthenticated(ClientObject client)
        {
            DMPModInterface.SendDMPModMessageToClient(client, "DMPSeedSync", BitConverter.GetBytes(Seed), true);
        }

        /// <summary>
        /// Contains the settings for loading the seed
        /// </summary>
        public class SeedSettings
        {
            [Description("The seed that is used to generate the save game on the client instances.")]
            public String seed = new Random().Next().ToString();
        }
    }
}
