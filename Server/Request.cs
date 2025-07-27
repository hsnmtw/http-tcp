using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace web.Http
{
    public enum HttpRequestMethod {_,GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS, TRACE, CONNECT}
    
    public class StringKeyValue
    {
        public StringKeyValue(string text, char sep = ':')
        {
            text = (""+text).Trim();
            if(text.Length>0)
            {
                int eq = text.IndexOf(sep);
                if(eq>-1)
                {
                    Key = text.Substring(0,eq).Trim().Trim('\r').Trim('\t').Trim('\n').Trim('\0').Trim().ToLower();
                    Value = text.Substring(eq+1).Trim();
                }
            }
        }
        public string Key { get; private set; }
        public string Value { get; private set; }
    }


    public class HttpRequest
    {
        private const string TOKEN = "9j21iox7";
        private HttpRequestMethod _method;
        private string _path = "";
        private string _body = "";
        private readonly Dictionary<string,string> _query = new Dictionary<string,string>();
        private readonly Dictionary<string,string> _form = new Dictionary<string,string>();
        private readonly Dictionary<string,string> _cookies = new Dictionary<string,string>();
        private readonly Dictionary<string,string> _headers = new Dictionary<string,string>();

        public string SessionId { get { return Cookie("session_id"); } }

        public HttpRequest(string requestText)
        {
            string[] lines = (""+requestText).Trim().Split('\n');
            //line 0 contains method and path and query
            for(int i=0;i<lines.Length;i++)
            {
                string line = lines[i].Trim();
                if(i==0) ParseFirstLine(line);
                else ParseOtherLines(line);
                if(line.Length==0)
                {
                    for(int j=i;j<lines.Length;j++)
                        _body += lines[j];
                    _body = _body.Trim().Trim('\r').Trim('\n').Trim(' ').Trim('\0').Trim('\t').Trim();
                    break;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("METHOD: '{0}'\nPATH: '{1}'\nQUERY: '{2}'\nHEADERS: '{3}'\nCOOKIES: '{4}'\nBODY: '{5}'",
            Method,
            Path,
            string.Join("&", _query.Keys.Select(k => string.Format("{0}={1}",k,_query[k]))),
            string.Join("&", _headers.Keys.Select(k => string.Format("{0}={1}",k,_headers[k]))),
            string.Join("&", _cookies.Keys.Select(k => string.Format("{0}={1}",k,_cookies[k]))),
            Body.Trim()
            );
        }

        private void ParseCookies(string cookies)
        {
            string[] parts = cookies.Split(new[]{"; "}, StringSplitOptions.RemoveEmptyEntries);
            for(int i=0;i<parts.Length;i++)
            {
                var kv = new StringKeyValue(parts[i],'=');
                if(string.IsNullOrEmpty(kv.Key) || string.IsNullOrEmpty(kv.Value)) continue;
                if(!kv.Value.EndsWith(TOKEN)) continue;
                _cookies[kv.Key] = kv.Value.Substring(0,kv.Value.Length - (1+TOKEN.Length));
            }
        }

        private void ParseOtherLines(string line)
        {
            var kv = new StringKeyValue(line);
            if(string.IsNullOrEmpty(kv.Key)) return;
            if(Equals(kv.Key,"cookie"))
            {
                ParseCookies(kv.Value);
                return;
            }
            _headers[kv.Key] = kv.Value;
        }

        private void ParseFirstLine(string line)
        {
            string[]parts = line.Split(' ');
            if(parts.Length==0) return;

            switch(parts[0].ToUpper())
            {
                case "GET":     _method = HttpRequestMethod.GET;     break;
                case "PUT":     _method = HttpRequestMethod.PUT;     break;
                case "POST":    _method = HttpRequestMethod.POST;    break;
                case "PATCH":   _method = HttpRequestMethod.PATCH;   break;
                case "DELETE":  _method = HttpRequestMethod.DELETE;  break;
                case "HEAD":    _method = HttpRequestMethod.HEAD;    break;
                case "OPTIONS": _method = HttpRequestMethod.OPTIONS; break;
                case "TRACE":   _method = HttpRequestMethod.TRACE;   break;
                case "CONNECT": _method = HttpRequestMethod.CONNECT; break;
                default:        _method = HttpRequestMethod._;       break;
            }

            if(parts.Length>1)
            {
                string[] ps = parts[1].Split('?');
                _path = ps[0].ToLower();
                if(ps.Length>1)
                {
                    string[] qs = ps[1].Split(new[]{"&"},StringSplitOptions.RemoveEmptyEntries);
                    for(int j=0;j<qs.Length;j++)
                    {
                        var kv = new StringKeyValue(qs[j],'=');
                        if(!string.IsNullOrEmpty(kv.Key)) _query[kv.Key] = kv.Value;
                    }
                }
            }
        }

        public HttpRequestMethod Method { get { return _method; } }
        public string Path { get { return _path; } set { _path = (""+value).Trim().ToLower(); } }
        public string Body { get { return _body; } }

        public string Header(string name) { return string.IsNullOrEmpty(name) || !_headers.ContainsKey(name) ? "" : _headers[(""+name).ToLower()]; }
        public string Form(string name)   { return string.IsNullOrEmpty(name) || !_form.ContainsKey(name)    ? "" : _form[(""+name).ToLower()]; }
        public string Query(string name)  { return string.IsNullOrEmpty(name) || !_query.ContainsKey(name)   ? "" : _query[(""+name).ToLower()]; }
        public string Cookie(string name) { return string.IsNullOrEmpty(name) || !_cookies.ContainsKey(name) ? "" : _cookies[(""+name).ToLower()]; }
    }
}