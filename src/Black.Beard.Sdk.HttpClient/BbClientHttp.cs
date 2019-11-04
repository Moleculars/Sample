﻿using Bb.Http.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Bb.Option
{

    public class BbClientHttp
    {

        public BbClientHttp(Uri server)
        {
            _uri = server;
        }


        public async Task<T> Post<T>(string path, object model, params (string, object)[] headers)
        {
            Dictionary<string, object> _headers = new Dictionary<string, object>();
            if (headers != null)
                foreach (var item in headers)
                    _headers.Add(item.Item1, item.Item2);
            return await Post<T>(path, model, _headers);
        }

        public async Task<T> Post<T>(string path, object model, Dictionary<string, object> headers = null)
        {

            var client = new HttpClient
            {
                BaseAddress = _uri,
            };

            ResolverRequestHeaders(headers, client.DefaultRequestHeaders);

            var msg = model.Serialize();

            HttpResponseMessage response = await client.PostAsync(path, msg);

            if (response.IsSuccessStatusCode)
            {
                var payloadResponse = await response.Content.ReadAsStringAsync();
                if (typeof(T) == typeof(string))
                    return (T)(object)payloadResponse;

                T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(payloadResponse);

                StoreResponseHeaders(headers, response);

                return result;
            }
            else
            {
                var u = ThrowWebRequest(response);
                if (u != null)
                    throw u;
            }

            return default(T);

        }



        public async Task<T> Get<T>(string path, params (string, object)[] headers)
        {
            Dictionary<string, object> _headers = new Dictionary<string, object>();
            if (headers != null)
                foreach (var item in headers)
                    _headers.Add(item.Item1, item.Item2);
            return await Get<T>(path, _headers);
        }

        public async Task<T> Get<T>(string path, Dictionary<string, object> headers = null)
        {

            using (var client = new HttpClient
            {
                BaseAddress = _uri,
            })
            {

                ResolverRequestHeaders(headers, client.DefaultRequestHeaders);

                HttpResponseMessage response = await client.GetAsync(path);

                if (response.IsSuccessStatusCode)
                {
                    var payloadResponse = await response.Content.ReadAsStringAsync();
                    if (typeof(T) == typeof(string))
                        return (T)(object)payloadResponse;

                    T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(payloadResponse);
                    StoreResponseHeaders(headers, response);

                    return result;
                }
                else
                {
                    var u = ThrowWebRequest(response);
                    if (u != null)
                        throw u;
                }

            }

            return default(T);

        }




        public async Task<T> Put<T>(string path, object model, params (string, object)[] headers)
        {
            Dictionary<string, object> _headers = new Dictionary<string, object>();
            if (headers != null)
                foreach (var item in headers)
                    _headers.Add(item.Item1, item.Item2);
            return await Put<T>(path, model, _headers);
        }

        public async Task<T> Put<T>(string path, object model, Dictionary<string, object> headers = null)
        {

            var client = new HttpClient
            {
                BaseAddress = _uri,
            };

            ResolverRequestHeaders(headers, client.DefaultRequestHeaders);

            var msg = model.Serialize();

            HttpResponseMessage response = await client.PutAsync(path, msg);

            if (response.IsSuccessStatusCode)
            {
                var payloadResponse = await response.Content.ReadAsStringAsync();
                if (typeof(T) == typeof(string))
                    return (T)(object)payloadResponse;

                T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(payloadResponse);

                StoreResponseHeaders(headers, response);

                return result;
            }
            else
            {
                var u = ThrowWebRequest(response);
                if (u != null)
                    throw u;
            }

            return default(T);

        }



        protected virtual Exception ThrowWebRequest(HttpResponseMessage response)
        {

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new AuthenticationException();

            var _response1 = response.StatusCode.ToString();
            return new System.Net.WebException($"Request failed {response.StatusCode}");

        }

        private static void StoreResponseHeaders(Dictionary<string, object> headers, HttpResponseMessage response)
        {
            if (headers != null)
            {
                foreach (var item in response.Headers)
                {
                    if (headers.ContainsKey(item.Key))
                        headers[item.Key] = item.Value;
                    else
                        headers.Add(item.Key, item.Value);
                }
            }
        }

        private static void ResolverRequestHeaders(Dictionary<string, object> headers, HttpRequestHeaders request)
        {

            Dictionary<string, object> _h;

            if (headers != null)
                _h = headers;
            else
                _h = new Dictionary<string, object>();

            if (!_h.ContainsKey("User-Agent"))
                _h.Add("User-Agent", Assembly.GetEntryAssembly().GetName().Name + ".NET client");

            request.Accept.Clear();
            request.AddHeader(_h);

            //    _h = TranslateObjectToDictionnarySerializerExtension.GetDictionnaryProperties(headers, true);

        }



        private Uri _uri;

    }


}
