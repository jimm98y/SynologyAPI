using SynologyAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SynologyAPI
{
    /// <summary>
    /// Synology Surveillance Station client.
    /// </summary>
    /// <remarks>
    /// https://global.synologydownload.com/download/Document/Software/DeveloperGuide/Package/SurveillanceStation/All/enu/Surveillance_Station_Web_API.pdf
    /// </remarks>
    public class SurveillanceStationClient : SynologyClient
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="host">Host - e.g. https://host:5001.</param>
        public SurveillanceStationClient(string host) : base(host)
        { }

        /// <summary>
        /// Get a list of all cameras.
        /// </summary>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Max number of cameras to return.</param>
        /// <returns>A list of <see cref="Camera"/>.</returns>
        public async Task<IList<Camera>> GetCamerasAsync(int offset = 0, int limit = 100)
        {
            if (string.IsNullOrWhiteSpace(_sid))
                throw new InvalidOperationException("Not logged in.");

            const string SynoApiCamera = "SYNO.SurveillanceStation.Camera";
            const string SynoApiMethodList = "List";

            // first query the endpoint on which the API can be called
            string queryApiResponse = await QueryApiAsync(SynoApiCamera);
            JsonNode cameraApi = JsonNode.Parse(queryApiResponse)["data"][SynoApiCamera];

            int cameraApiVersion = cameraApi["maxVersion"].GetValue<int>();
            string cameraApiEndpoint = cameraApi["path"].GetValue<string>();

            // privCamType = 0x01 LiveStream & 0x02 Recording => 3
            string uri = $"{_host}/webapi/{cameraApiEndpoint}?privCamType=3&version={cameraApiVersion}&streamInfo=true&api={SynoApiCamera}&limit={limit}&offset={offset}&basic=true&blFromCamList=true&camStm=0&method={SynoApiMethodList}&_sid={_sid}";
            string listApiResponse = await GetAsync(uri);
            JsonNode listResponse = JsonNode.Parse(listApiResponse);

            if (listResponse["success"].GetValue<bool>())
            {
                return listResponse["data"]["cameras"].AsArray().Select(x => ParseCamera(x)).ToList();
            }
            else
            {
                throw new Exception("Unable to get cameras!");
            }
        }

        /// <summary>
        /// Get live stream descriptions.
        /// </summary>
        /// <param name="cameraIds">Camera IDs for which the info is requested.</param>
        /// <returns>A list of <see cref="LiveStream"/>.</returns>
        public async Task<IList<LiveStream>> GetLiveStreamsAsync(IEnumerable<int> cameraIds)
        {
            if (string.IsNullOrWhiteSpace(_sid))
                throw new InvalidOperationException("Not logged in.");

            if (cameraIds == null || cameraIds.Count() == 0)
                throw new ArgumentNullException("Camera IDs must be specified.");

            const string SynoApiCamera = "SYNO.SurveillanceStation.Camera";
            const string SynoApiMethodGetLiveView = "GetLiveViewPath";

            string cameraIdsString = string.Join(",", cameraIds);

            // first query the endpoint on which the API can be called
            string queryApiResponse = await QueryApiAsync(SynoApiCamera);
            JsonNode cameraApi = JsonNode.Parse(queryApiResponse)["data"][SynoApiCamera];

            int cameraApiVersion = cameraApi["maxVersion"].GetValue<int>();
            string cameraApiEndpoint = cameraApi["path"].GetValue<string>();

            string uri = $"{_host}/webapi/{cameraApiEndpoint}?version={cameraApiVersion}&api={SynoApiCamera}&method={SynoApiMethodGetLiveView}&idList={cameraIdsString}&_sid={_sid}";
            string getLiveViewApiResponse = await GetAsync(uri);
            JsonNode getLiveViewResponse = JsonNode.Parse(getLiveViewApiResponse);

            if (getLiveViewResponse["success"].GetValue<bool>())
            {
                return getLiveViewResponse["data"].AsArray().Select(x => ParseLiveStream(x)).ToList();
            }
            else
            {
                throw new Exception("Unable to get cameras!");
            }
        }

        /// <summary>
        /// Get event stream descriptions.
        /// </summary>
        /// <param name="cameraIds">Camera IDs for which the info is requested.</param>
        /// <param name="offset">Skip this number of recordings.</param>
        /// <param name="limit">Max number of recordings to return.</param>
        /// <returns>A list of <see cref="Recording"/>.</returns>
        public async Task<IList<Recording>> GetRecordingsAsync(IEnumerable<int> cameraIds, int offset = 0, int limit = 100)
        {
            if (string.IsNullOrWhiteSpace(_sid))
                throw new InvalidOperationException("Not logged in.");

            if (cameraIds == null || cameraIds.Count() == 0)
                throw new ArgumentNullException("Camera IDs must be specified.");

            const string SynoApiCamera = "SYNO.SurveillanceStation.Recording";
            const string SynoApiMethodList = "List";

            string cameraIdsString = string.Join(",", cameraIds);

            // first query the endpoint on which the API can be called
            string queryApiResponse = await QueryApiAsync(SynoApiCamera);
            JsonNode cameraApi = JsonNode.Parse(queryApiResponse)["data"][SynoApiCamera];

            int cameraApiVersion = cameraApi["maxVersion"].GetValue<int>();
            string cameraApiEndpoint = cameraApi["path"].GetValue<string>();

            string uri = $"{_host}/webapi/{cameraApiEndpoint}?version={cameraApiVersion}&api={SynoApiCamera}&method={SynoApiMethodList}&cameraIds={cameraIdsString}&offset={offset}&limit={limit}&_sid={_sid}";
            string getLiveViewApiResponse = await GetAsync(uri);
            JsonNode getLiveViewResponse = JsonNode.Parse(getLiveViewApiResponse);

            if (getLiveViewResponse["success"].GetValue<bool>())
            {
                return getLiveViewResponse["data"]["recordings"].AsArray().Select(x => ParseRecording(x)).ToList();
            }
            else
            {
                throw new Exception("Unable to get cameras!");
            }
        }

        /// <summary>
        /// Get the live streaming URI.
        /// </summary>
        /// <param name="recordingId">ID of the recording to be requested.</param>
        /// <returns>A list of <see cref="Recording"/>.</returns>
        public async Task<string> GetLiveStreamUriAsync(int recordingId)
        {
            if (string.IsNullOrWhiteSpace(_sid))
                throw new InvalidOperationException("Not logged in.");

            const string SynoApiCamera = "SYNO.SurveillanceStation.Recording";
            const string SynoApiMethodList = "Stream";

            // first query the endpoint on which the API can be called
            string queryApiResponse = await QueryApiAsync(SynoApiCamera);
            JsonNode cameraApi = JsonNode.Parse(queryApiResponse)["data"][SynoApiCamera];

            int cameraApiVersion = cameraApi["maxVersion"].GetValue<int>();
            string cameraApiEndpoint = cameraApi["path"].GetValue<string>();

            return $"{_host}/webapi/{cameraApiEndpoint}?version={cameraApiVersion}&api={SynoApiCamera}&method={SynoApiMethodList}&recordingId={recordingId}&_sid={_sid}";
        }

        private static Camera ParseCamera(JsonNode json)
        {
            // TODO: deserialize into model while not relying upon matching properties
            return JsonSerializer.Deserialize<Camera>(
                json,
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }

        private static LiveStream ParseLiveStream(JsonNode json)
        {
            // TODO: deserialize into model while not relying upon matching properties
            return JsonSerializer.Deserialize<LiveStream>(
                json,
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }

        private static Recording ParseRecording(JsonNode json)
        {
            // TODO: deserialize into model while not relying upon matching properties
            return JsonSerializer.Deserialize<Recording>(
                json,
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
    }
}
