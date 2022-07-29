using System.Collections.Generic;

namespace MultiplayerGameJam.Network
{
    public class SessionManager
    {
        private Dictionary<string, SessionData> _data = new();
        private Dictionary<ulong, string> _dataIdLookup = new();

        public void AddConnection(ulong clientId, string userId)
        {
            // Already connected
            if (_data.ContainsKey(userId))
            {
                if (!_data[userId].IsConnected) // Need to be reconnected
                {
                    _data[userId].IsConnected = true;
                    _data[userId].ClientId = clientId;
                }
            }
            else
            {
                var data = new SessionData()
                {
                    IsConnected = true,
                    ClientId = clientId
                };
                _data.Add(userId, data);
                _dataIdLookup.Add(clientId, userId);
            }
        }
    }
}
