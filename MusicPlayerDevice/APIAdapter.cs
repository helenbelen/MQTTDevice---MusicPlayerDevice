using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MusicPlayerDevice
{
    public enum URLType {
        DEVICEDATA = 0,
        DEVICELIST = 1,
        DEVICEBYID = 3,
        DEVICELOC = 4,
        DEVICEREG = 5,
        CONNECTIONINFO = 6,
        DATABYID = 7,
        ADDDATA = 8
    }
    public static class APIAdapter
    {
        static private readonly string baseURL = MQTTCommon.Resources.webApiUrl;
        static HttpClient httpClient;
      
        static string result;

        public static string GetURL(URLType type, int id = -1, string info = null)
        {
            string url = "";
            switch (type)
            {
                case URLType.DEVICEDATA:
                    url = baseURL + "DeviceData";
                    break;

                case URLType.DEVICELIST:
                    url = baseURL + "DeviceList";
                    break;

                case URLType.DEVICEBYID:
                    if (id >= 0)
                    {
                        url = baseURL + "DeviceList/GetDevice/" + id.ToString();
                    }
                    break;
                case URLType.DEVICELOC:
                    if (id >= 0 && info != null)
                    {
                        url = baseURL + "DeviceList/UpdateLocation/" + id.ToString() + "/" + info;
                    }
                    break;
                case URLType.DEVICEREG:
                    if (info != null)
                    {
                        url = baseURL + "DeviceList/RegisterDevice/" + info;
                    }
                    break;
                case URLType.CONNECTIONINFO:
                    url = baseURL + "DeviceData/GetConnectionInfo";
                    break;
                case URLType.DATABYID:
                    if (id >= 0)
                    {
                        url = baseURL + "DeviceData/GetDeviceData" + id.ToString();
                    }
                    break;
                case URLType.ADDDATA:
                    if (id >= 0 && info != null)
                    {
                        url = baseURL + "DeviceData/AddData/" + id.ToString() + "/" + info;
                    }
                    break;
                default:

                    break;

            }
            return url;
        }


        public static string APIResponse {get{return result;} }


        public static async Task UpdateDeviceLocation(int id, string info)
        {
            httpClient = new HttpClient();
            HttpContent content = null;
            var response = httpClient.PutAsync(new Uri(GetURL(URLType.DEVICELOC,id,info)),content).Result;
            result = await response.Content.ReadAsStringAsync();
        }

        public static async Task RegisterDevice(string info)
        {
            httpClient = new HttpClient();
            
            var response = httpClient.PostAsync(new Uri(GetURL(URLType.DEVICEREG,-1,info)), new StringContent(info)).Result;
            result = await response.Content.ReadAsStringAsync();
            
        }
    }
}
