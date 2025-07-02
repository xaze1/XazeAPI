using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XazeAPI.API.DiscordWebhook.Classes
{
    public class AuthorBuilder
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public object Build()
        {
            return new
            {
                name= Name,
                ulr= Url,
            };
        }
    }
}
