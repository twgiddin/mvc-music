﻿using Microsoft.Owin;
using Owin;


[assembly: OwinStartupAttribute(typeof(MvcMusicStore_Wed_F2017.Startup))]
namespace MvcMusicStore_Wed_F2017
{
   
    public partial class Startup
    {
       
        public void Configuration(IAppBuilder app)
        {
        ConfigureAuth(app);
        }
    }
}
