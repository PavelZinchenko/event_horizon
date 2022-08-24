using System;
using UnityEngine;
using Zenject;

namespace Services
{
    public class DiscordController : IInitializable, IDisposable, ITickable
    {
        public void Initialize()
        {
            try
            {
                _discord = new Discord.Discord(599315405261111316, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
                Debug.LogWarning("Discord initialized");

                var activityManager = _discord.GetActivityManager();
                var activity = new Discord.Activity
                {
                    State = string.Empty,
                    Details = string.Empty
                };

                activityManager.UpdateActivity(activity, res => { });
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to initialize Discord: " + e.Message);
                return;
            }
        }

        public void Dispose()
        {
            _discord?.Dispose();
            _discord = null;
        }

        public void Tick()
        {
            try
            {
                _discord?.RunCallbacks();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Dispose();
            }
        }

        private Discord.Discord _discord;
    }
}
