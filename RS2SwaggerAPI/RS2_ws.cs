using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RS2SwaggerAPI
{
    public class RS2_ws
    {
        #region Properties
        private string baseURL { get; set; }
        public HttpStatusCode LastStatus { get; set; }
        public string LastResponse { get; set; }
        public string LastRequest { get; set; }
        public string LastURL { get; private set; }
        private string AccessIT_URL { get; set; }
        private string AccessIT_UserName { get; set; }
        private string AccessIT_Password { get; set; }
        private string AccessIT_PublicKey { get; set; }
        #endregion


        #region Contsructors
        public RS2_ws(string in_AccessIT_URL)
        {
            AccessIT_URL = in_AccessIT_URL;

        }

        public RS2_ws(string AccessIT_URL, string AccessIT_UserName, string AccessIT_Password, string AccessIT_PublicKey)
        {
            this.AccessIT_URL = AccessIT_URL;
            this.AccessIT_UserName = AccessIT_UserName;
            this.AccessIT_Password = AccessIT_Password;
            this.AccessIT_PublicKey = AccessIT_PublicKey;
        }
        #endregion

        #region Enums
        public enum CardholderStatus
        {
            Disabled = 0,
            Active = 1,
            DateBased = 2
        }

        public enum CardStatus
        {
            Disabled = 0,
            Active = 1,
            DateBased = 2
        }

        #endregion

        #region Select

        public dynamic[] SelectAll(string tableName, bool ReturnAllData = false)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            string UtensilDataArray = "starting array";
            List<JsonApiObject> results = new List<JsonApiObject>();

            results.AddRange(PerformSelect(tableName, "", ReturnAllData, "", null, out UtensilDataArray));
            return results.ToArray();
        }

        public dynamic[] Select(string tableName, ICriteria criteria = null, bool ReturnAllData = false)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            List<JsonApiObject> results = new List<JsonApiObject>();
            string UtensilDataArray = "starting array";


            results.AddRange(PerformSelect(tableName, "", ReturnAllData, criteria.ToQueryString(), null, out UtensilDataArray));

            return results.ToArray();
        }

        public List<JsonApiObject> SelectList(string tableName, ICriteria criteria = null, bool ReturnAllData = false)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            List<JsonApiObject> results = new List<JsonApiObject>();
            string UtensilDataArray = "starting array";

            if (criteria != null)
                results.AddRange(PerformSelect(tableName, "", ReturnAllData, criteria.ToQueryString(), null, out UtensilDataArray));
            else
                results.AddRange(PerformSelect(tableName, "", ReturnAllData, "", null, out UtensilDataArray));

            return results;
        }

        //public dynamic[] Select(string tableName, string id, ICriteria criteria = null)
        //{
        //	if (tableName == null)
        //		throw new ArgumentNullException("tableName");

        //	List<JsonApiObject> results = new List<JsonApiObject>();
        //	string UtensilDataArray = "starting array";
        //	results.AddRange(PerformSelect(tableName, id, "", criteria.ToQueryString(), null, out UtensilDataArray));

        //	return results.ToArray();
        //}

        public dynamic Select(string tableName, string id)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            JsonApiObject result = new JsonApiObject();
            string AccessData = "starting array";

            result = PerformSelect(tableName, id, false, null, null, out AccessData)[0];

            return result;
        }

        public dynamic[] Select(string tableName, string id, ICriteria criteria, bool ReturnAllData = false)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            List<JsonApiObject> results = new List<JsonApiObject>();
            string DataArray = "starting array";

            results.AddRange(PerformSelect(tableName, id, ReturnAllData, criteria.ToQueryString(), null, out DataArray));

            return results.ToArray();
        }



        private List<JsonApiObject> PerformSelect(string tableName, string uuid, bool ReturnAllData, string filters, int? maxRows, out string dataReturn)
        {
            List<object> urlBits = new List<object> { tableName };

            if (uuid != null && uuid != "")
            {
                urlBits.Add(uuid);
            }

            string finalUrl = String.Join("/", urlBits);

            List<object> queryBits = new List<object>();
            if (filters != null && filters != "")
                queryBits.Add("filter=" + filters);

            if (maxRows != null && maxRows > 0)
                queryBits.Add("maxRows=" + maxRows.ToString());

            if (ReturnAllData)
            {
                queryBits.Add("includeAllData=true");
            }

            finalUrl += "?" + String.Join("&", queryBits);

            HttpStatusCode statusCode = PerformRequest(finalUrl, HttpMethod.Get, "", out string result);

            if (statusCode == HttpStatusCode.OK)
            {
                JArray jobject = JArray.Parse(result);

                dataReturn = result;
                return jobject.Select(x => new JsonApiObject(x, tableName)).ToList();
            }
            else
            {
                dataReturn = "";
                return null;
            }
        }

        #endregion

        #region Delete
        public bool Delete(string tableName, string id)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            string url = tableName + "/" + id;

            string result;
            HttpStatusCode status = PerformRequest(url, HttpMethod.Delete, "", out result);

            if (status == HttpStatusCode.OK)
                return true;
            else
                return false;
        }
        #endregion

        #region Update
        public bool Update(string tableName, string id, string json)
        {
            string url = tableName + "/" + id;
            string result;

            HttpStatusCode status = PerformRequest(url, HttpMethod.Put, json, out result);

            if (status == HttpStatusCode.OK)
                return true;
            else
                return false;
        }


        public bool Update(string tableName, string id, object Record)
        {
            string url = tableName + "/" + id;
            string result;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(Record, Formatting.Indented);

            HttpStatusCode status = PerformRequest(url, HttpMethod.Put, json, out result);

            if (status == HttpStatusCode.OK)
                return true;
            else
                return false;
        }
        #endregion


        #region ExecuteMacro

        public bool ExecuteMacro(string MacroID)
        {
            string result;

            List<string> urlBits = new List<string>() { "Macros", MacroID, "MacroControl" };

            string finalUrl = String.Join("/", urlBits);
            string MacroCommand = @"{""MacroCommand"": 1}";

            HttpStatusCode status = PerformRequest(finalUrl, HttpMethod.Put, MacroCommand, out result);

            if (status == HttpStatusCode.OK)
                return true;
            else
                return false;
        }

        #endregion

        #region Create
        public dynamic CreateDefault(string tableName)
        {
            string DataArray = "starting array";
            dynamic results = PerformSelect(tableName, null, true, null, 1, out DataArray);
            JsonApiObject result = results[0];

            result.SetDefaultValues();

            return result;
        }

        public dynamic Create(string tableName, string json)
        {
            string result;
            HttpStatusCode statusCode = PerformRequest(AccessIT_URL, HttpMethod.Post, json, out result);


            if (statusCode == HttpStatusCode.OK)
            {
                JArray jobject = JArray.Parse(result);

                //dataReturn = result;
                return jobject.Select(x => new JsonApiObject(x, tableName));
            }
            else
            {
                //dataReturn = "";
                return null;
            }

        }
        public dynamic Create(string tableName, object Record)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(Record, Formatting.Indented);

            string result;
            HttpStatusCode statusCode = PerformRequest(tableName, HttpMethod.Post, json, out result);


            if (statusCode == HttpStatusCode.OK)
            {
                JArray jobject = JArray.Parse(result);

                return jobject.Select(x => new JsonApiObject(x, tableName)).ToList()[0];
            }
            else
            {
                //dataReturn = "";
                return null;
            }

        }
        #endregion

        #region PrivateHelpers
        private void SetBasicAuthHeader(WebRequest request, String userName, String userPassword)
        {
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + userPassword));
            request.Headers.Add("Authorization", "Basic " + encoded);
        }

        private void SetPublicKeyHeader(WebRequest request, string PublicKey)
        {
            request.Headers.Add("PublicKey", PublicKey);
        }

        private HttpStatusCode PerformRequest(string url, HttpMethod method, string postJSON, out string result)
        {
            result = null;
            HttpStatusCode status = HttpStatusCode.BadRequest;

            Uri requestUri = new Uri(AccessIT_URL + url);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUri);
            SetBasicAuthHeader(req, AccessIT_UserName, AccessIT_Password);
            SetPublicKeyHeader(req, AccessIT_PublicKey);

            req.ContentType = "application/json";
            req.Accept = "application/json";
            req.Method = method.ToString();

            if (postJSON != "")
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(postJSON);
                req.ContentLength = byteArray.Length;
                Stream stream = req.GetRequestStream();
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Close();
            }

            try
            {
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    string RespStream = sr.ReadToEnd();
                    result = RespStream;
                    //result = JsonConvert.DeserializeObject<eMaintResult>(RespStream);
                    status = resp.StatusCode;
                }
            }
            catch (WebException ex)
            {
                using (var exStream = ex.Response.GetResponseStream())
                using (var exReader = new StreamReader(exStream))
                {
                    //result = exReader.ReadToEnd();
                    result = null;
                    status = HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                LastResponse = "Web service call failed with - " + ex;
            }

            this.LastURL = url;
            this.LastStatus = status;
            this.LastRequest = postJSON;
            this.LastResponse = result;

            return status;

        }


        #endregion

    }
}
