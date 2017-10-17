using DarkMultiPlayer;
using System;
using UnityEngine;

namespace DMPSeedSync
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class SyncClient : MonoBehaviour
    {
        /// <summary>
        /// The savegame set set by the server admin
        /// </summary>
        public Int32 Seed;

        /// <summary>
        /// Whether the client already started
        /// </summary>
        public Boolean started;

        /// <summary>
        /// Register to the GameEvent that is fired on a scene change
        /// </summary>
        void Update()
        {
            // Update the game seed
			if (HighLogic.CurrentGame != null)
			{
				HighLogic.CurrentGame.Seed = Seed;
			}

            // Don't run the code if the client already started
            if (started)
                return;

            GameEvents.onGameSceneSwitchRequested.Add(HandleSceneSwitch);
            DontDestroyOnLoad(this);

            // Register the DMP Handler
            Client.dmpClient.dmpModInterface.RegisterRawModHandler("DMPSeedSync", HandleDMPMessageCallback);
            started = true;
        }

        /// <summary>
        /// This gets called when a new scene gets loaded.
        /// </summary>
        void HandleSceneSwitch(GameEvents.FromToAction<GameScenes, GameScenes> data)
        {
            // When switching from MainMenu to SpaceCenter, it is very likely that a
            // new scene got loaded
            if (data.from == GameScenes.MAINMENU && data.to == GameScenes.SPACECENTER)
            {
                if (HighLogic.CurrentGame == null) 
                {
                    Debug.Log("[DMPSeedSync] Current Game is null!");
                    return;
                }
				HighLogic.CurrentGame.Seed = Seed;
				Debug.Log("[DSS2] " + HighLogic.CurrentGame.Seed);
            }
        }

        /// <summary>
        /// This method gets called when the server sends us a new seed.
        /// </summary>
        void HandleDMPMessageCallback(Byte[] messageData)
        {
            try
            {
                Seed = BitConverter.ToInt32(messageData, 0);
                Debug.Log("[DSS] " + Seed);
            }
            catch (Exception)
            {
                Debug.Log("[DMPSeedSync] Failed to parse the Seed received by the server!");
            }
        }
    }
}
