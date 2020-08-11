using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TimeManagementSystem.Startup))]

namespace TimeManagementSystem
{
    public static class AppAttributes
    {
        public static string Name => "Time Management System";
    }

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
