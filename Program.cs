
using System;
using System.Linq;
using web.Http;

namespace web
{
    public class Program
    {
        static void Main(string[]args)
        {   
            int port = 80;
            if(args.Length>0 && args[0].All(char.IsNumber))
                port = int.Parse(args[0]);
            var server = new Server();
            server.AddMiddleware((req,res)=>{
                if(req.Query("err")=="500") 
                    throw new Exception(string.Format("middleware test : {0}", req.Path ));
            });
            //server.AddMiddleware((req,res)=>{
            //    if(req.Cookie("session_id") == "" && !req.Path.StartsWith("/assets"))
            //        req.Path = "/assets/html/user/login.html";
            //});
            server.AddRoute("GET:/test", (req,res) => {
                res.Write("test");
                return true;
            });
            server.Run( 
                address: "127.0.0.1", 
                port: port, 
                maxRetry: 10, 
                build: args.Length>0 && args[0].ToLower()=="build" );
        }
    }
}