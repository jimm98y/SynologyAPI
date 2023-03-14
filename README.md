# SynologyAPI
An unofficial client to communicate with Synology devices. It can query all connected devices from the Synology Router and also enumerate all cameras from the Surveillance Station on Synology NAS.

## RouterClient
Create the `RouterClient` and sign in:
```
var routerClient = new RouterClient("https://host:5001");
await routerClient.SignInAsync("userName", "mySecretPassword");
```
Get all currently connected devices:
```
var devices = await routerClient.GetConnectedDevicesAsync();
```
## SurveillanceStationClient
Create the `SurveillanceStationClient` and sign in:
```
var surveillanceClient = new SurveillanceStationClient("https://host:5001");
await surveillanceClient.SignInAsync("userName", "mySecretPassword");
```
Get all currently connected cameras:
```
var cameras = await surveillanceClient.GetCamerasAsync();
```
Get all live streams:
```
int[] ids = cameras.Select(x => x.Id).ToArray();
var liveStreams = await surveillanceClient.GetLiveStreamsAsync(ids);
```
Get all recordings:
```
var recordings = await surveillanceClient.GetRecordingsAsync(ids);
```
Get the URI of the recording to play it:
```
string videoUri = await surveillanceClient.GetLiveStreamUriAsync(recordings.FirstOrDefault().Id);
```