using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OMSharp
{
    /// <summary>
    /// The main class used to connect to an Olympus camera
    /// </summary>
    public class Camera
    {
        private HttpClient client;
        const string camapi = "http://192.168.0.10:80/";
        public Camera()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "OI.Share v2");
        }
        public struct CamInfo
        {
            public string model;
            public string HighResolutionSoundPlay;
            public SerialNumberList SerialNumberList;
            public string GetImageScreennailSupport;
            public string GetRawImageSupport;
            public BleFunctionDetailList BleFunctionDetailList;
        }
        public struct SerialNumberList
        {
            public string Serial1;
            public string Serial2;
            public string Serial3;
            public string Serial4;
        }
        public struct BleFunctionDetailList
        {
            public string Func1;
            public string Func2;
            public string Func3;
            public string Func4;
        }
        public async Task<CamInfo?> GetCamInfoAsync()
        {
            var response = await client.GetAsync(camapi+"get_caminfo.cgi");
            if (response.IsSuccessStatusCode)
            {
                XmlSerializer xml = new XmlSerializer(typeof(CamInfo));
                return xml.Deserialize(response.Content.ReadAsStream()) as CamInfo?;
            }
            else
            {
                return null;
            }
        }
        public enum CamMode { play, rec }
        public enum RecResolution { r0320x0240, r0640x0480, r0800x0600, r1024x0768, r1280x0960, none }
        public async Task<bool> SetCameraMode(CamMode mode, RecResolution lvqty = RecResolution.none)
        {
            HttpResponseMessage response = null;
            switch (mode)
            {
                case CamMode.play:
                    response = await client.GetAsync(camapi+"switch_cammode.cgi?mode=play");
                    break;
                case CamMode.rec:
                    response = await client.GetAsync(camapi+"switch_cammode.cgi?mode=rec&lvqty="+lvqty.ToString().Substring(1));
                    break;
            }
            return response?.IsSuccessStatusCode ?? false;
        }
        public async Task<bool> PowerOff()
        {
            var response = await client.GetAsync(camapi+"exec_pwoff.cgi?mode=withble");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> SetTimeout(int timeoutsec)
        {
            var response = await client.GetAsync(camapi+"set_timeout.cgi?timeoutsec="+timeoutsec.ToString());
            return response.IsSuccessStatusCode;
        }
        public enum RSize { r1024, r1600, r1920, r2048 }
        public async Task<bool> Get_ResiseImg(string DIR, RSize size, Stream writeTo)
        {
            var response = await client.GetAsync(camapi+$"get_resizeimg.cgi?DIR={DIR}&size={size.ToString().Substring(1)}");
            if (response.IsSuccessStatusCode)
            {
                await response.Content.CopyToAsync(writeTo);
            }
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> StartLiveView(int port)
        {
            var response = await client.GetAsync(camapi+"exec_takemisc.cgi?com=startliveview&port="+port.ToString());
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> StopLiveView()
        {
            var response = await client.GetAsync(camapi+"exec_takemisc.cgi?com=stopliveview");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> GetRecView(Stream writeTo)
        {
            var response = await client.GetAsync(camapi+$"exec_takemisc.cgi?com=getrecview");
            if (response.IsSuccessStatusCode)
            {
                await response.Content.CopyToAsync(writeTo);
            }
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> GetLastJpg(Stream writeTo)
        {
            var response = await client.GetAsync(camapi+$"exec_takemisc.cgi?com=getlastjpg");
            if (response.IsSuccessStatusCode)
            {
                await response.Content.CopyToAsync(writeTo);
            }
            return response.IsSuccessStatusCode;
        }
        //the next two are not tested, I can't test them, if you have a problem with them, fix it yourself and make a pull request - Mukunya
        public enum CtrlzoomMove { widemove, telemove, off, wideterm, teleterm }
        public async Task<bool> CtrlZoom(CtrlzoomMove move)
        {
            var response = await client.GetAsync(camapi+"exec_takemisc.cgi?com=ctrlzoom&move="+move.ToString());
            return response.IsSuccessStatusCode;
        }
        public enum SuperMacromFlinaFlockMove { nearstep, farstep, near, far, stop }
        public async Task<bool> SuperMacromFlinaFlock(SuperMacromFlinaFlockMove move, int movement)
        {
            var response = await client.GetAsync(camapi+$"exec_takemisc.cgi?com=ctrlzoom&move={move}&movement={movement}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> MovieThroughStart()
        {
            var response = await client.GetAsync(camapi+"exec_takemisc.cgi?com=MovieThroughStart");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> MovieThroughStop()
        {
            var response = await client.GetAsync(camapi+"exec_takemisc.cgi?com=MovieThroughStop");
            return response.IsSuccessStatusCode;
        }
        //TODO: test the next two, figure out what they do
        public async Task<bool> GetShortMoviesAlbumInfo()
        {
            var response = await client.GetAsync(camapi+"exec_takemisc.cgi?com=GetShortMoviesAlbumInfo");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> GetMovieSetting()
        {
            var response = await client.GetAsync(camapi+"exec_takemisc.cgi?com=GetMovieSetting");
            return response.IsSuccessStatusCode;
        }
        public enum PropName { touchactiveframe , takemode, drivemode, focalvalue, expcomp, shutspeedvalue, isospeedvalue, wbvalue, noisereduction, lowvibtime, bulbtimelimit, artfilter,
         digitaltelecon, exposemovie, cameradrivemode, colorphase, SceneSub, SilentNoiseReduction, SilentTime, ArtEffectTypePopart, ArtEffectTypeRoughMonochrome, ArtEffectTypeToyPhoto,
         ArtEffectTypeDaydream, ArtEffectTypeCrossProcess, ArtEffectTypeDramaticTone, ArtEffectTypeLigneClair, ArtEffectTypePastel,
         ArtEffectTypeMiniature, ArtEffectTypeVintage, ArtEffectTypePartcolor, ArtEffectTypeBleachBypass, NoiseReductionExposureTime
        }

        public async Task<string> GetProperty(PropName name)
        {
            var response = await client.GetAsync(camapi+"get_camprop.cgi?com=get&propname="+name.ToString());
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<bool> SetProperty(PropName name, string value)
        {
            var response = await client.PostAsync(camapi+"set_camprop.cgi?com=set&propname="+name.ToString(),new StringContent(value));
            return response.IsSuccessStatusCode;
        }
        public async Task<long?> GetUnusedCapacity()
        {
            var response = await client.GetAsync(camapi+"get_unusedcapacity.cgi");
            string s = await response.Content.ReadAsStringAsync();
            return long.Parse(s.Substring(31, s.Length-31-11));
        }
    }
}
