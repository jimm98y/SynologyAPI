using SynologyAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SynologyAPI
{
    /// <summary>
    /// Synology Router client.
    /// </summary>
    public class RouterClient : SynologyClient
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="host">Host - e.g. https://host:8001.</param>
        public RouterClient(string host) : base(host)
        { }

        /// <summary>
        /// Returns a list of connected devices.
        /// </summary>
        /// <returns>A list of <see cref="Device"/>.</returns>
        public async Task<IList<Device>> GetConnectedDevicesAsync()
        {
            if (string.IsNullOrWhiteSpace(_sid))
                throw new InvalidOperationException("Not logged in.");

            const string SynoApiNsmDevice = "SYNO.Core.Network.NSM.Device";

            // first query the endpoint on which the API can be called
            string queryApiResponse = await QueryApiAsync(SynoApiNsmDevice);
            JsonNode deviceApi = JsonNode.Parse(queryApiResponse)["data"][SynoApiNsmDevice];

            int deviceApiVersion = deviceApi["maxVersion"].GetValue<int>();
            string deviceApiEndpoint = deviceApi["path"].GetValue<string>();

            string uri = $"{_host}/webapi/{deviceApiEndpoint}";
            Dictionary<string, string> content = new Dictionary<string, string>
            {
                { "conntype", "all" },
                { "api", SynoApiNsmDevice },
                { "method", "get" },
                { "version", deviceApiVersion.ToString() }
            };

            string deviceApiResponse = await PostAsync(uri, content);

            JsonNode deviceResponse = JsonNode.Parse(deviceApiResponse);
            if (deviceResponse["success"].GetValue<bool>())
            {
                return deviceResponse["data"]["devices"].AsArray().Select(x => ParseDevice(x)).ToList();
            }

            throw new Exception("Unable to retrieve connected devices");
        }

        private static Device ParseDevice(JsonNode json)
        {
            var device = new Device();
            device.Band = json["band"]?.GetValue<string>();
            device.Connection = json["connection"]?.GetValue<string>();
            device.CurrentRate = json["current_rate"]?.GetValue<int>();
            device.DeviceType = json["dev_type"]?.GetValue<string>();
            device.HostName = json["hostname"]?.GetValue<string>();
            device.IP6 = json["ip6_addr"]?.GetValue<string>();
            device.IP4 = json["ip_addr"]?.GetValue<string>();
            device.IsBanned = json["is_baned"]?.GetValue<bool>();
            device.IsBeamformingOn = json["is_beamforming_on"]?.GetValue<bool>();
            device.IsGuest = json["is_guest"]?.GetValue<bool>();
            device.IsHighQOS = json["is_high_qos"]?.GetValue<bool>();
            device.IsLowQOS = json["is_low_qos"]?.GetValue<bool>();
            device.IsManualDeviceType = json["is_manual_dev_type"]?.GetValue<bool>();
            device.IsManualHostName = json["is_manual_hostname"]?.GetValue<bool>();
            device.IsOnline = json["is_online"]?.GetValue<bool>();
            device.IsWireless = json["is_wireless"]?.GetValue<bool>();
            device.MAC = json["mac"]?.GetValue<string>();
            device.MaxRate = json["max_rate"]?.GetValue<int>();
            device.MeshNodeID = json["mesh_node_id"]?.GetValue<int>();
            device.RateQuality = json["rate_quality"]?.GetValue<string>();
            device.SignalStrength = json["signalstrength"]?.GetValue<int>();
            device.TransferRXRate = json["transferRXRate"]?.GetValue<int>();
            device.TransferTXRate = json["transferTXRate"]?.GetValue<int>();
            return device;
        }
    }
}
