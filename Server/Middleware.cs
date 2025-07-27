using System;
using System.Collections.Generic;

namespace web.Http
{
    public class Middleware
    {
        public List<Action<HttpRequest,HttpResponse>> Actions { get; private set; }

        public Middleware()
        {
            Actions = new List<Action<HttpRequest, HttpResponse>>();
        }
    }
}