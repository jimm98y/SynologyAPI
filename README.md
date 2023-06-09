# SynologyAPI
An unofficial client to communicate with Synology devices. It can query all connected devices from the Synology Router and also enumerate all cameras from the Surveillance Station on Synology NAS.

## RouterClient
Create the `RouterClient` and sign in:
```cs
var routerClient = new RouterClient("https://host:5001");
await routerClient.SignInAsync("userName", "mySecretPassword");
```
Get all currently connected devices:
```cs
var devices = await routerClient.GetConnectedDevicesAsync();
```
## SurveillanceStationClient
Create the `SurveillanceStationClient` and sign in:
```cs
var surveillanceClient = new SurveillanceStationClient("https://host:5001");
await surveillanceClient.SignInAsync("userName", "mySecretPassword");
```
Get all currently connected cameras:
```cs
var cameras = await surveillanceClient.GetCamerasAsync();
```
Get all live streams:
```cs
int[] ids = cameras.Select(x => x.Id).ToArray();
var liveStreams = await surveillanceClient.GetLiveStreamsAsync(ids);
```
Get all recordings:
```cs
var recordings = await surveillanceClient.GetRecordingsAsync(ids);
```
Get the URI of the recording to play it:
```cs
string videoUri = await surveillanceClient.GetLiveStreamUriAsync(recordings.FirstOrDefault().Id);
```