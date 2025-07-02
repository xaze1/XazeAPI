using System;
using System.Collections.Generic;
using System.Drawing;
using XazeAPI.API.Extensions;

namespace XazeAPI.API.DiscordWebhook.Classes
{
    public class EmbedBuilder
    {
        public EmbedBuilder()
        {
            Title = "";
            TitleUrl = "";
            Description = "";
            Color = Color.White;
            Timestamp = DateTimeOffset.Now;
            Fields = new();
        }

        public EmbedBuilder(string title, string titleUrl = "", string description = "", Color color = default(Color), DateTimeOffset timestamp = default, List<FieldBuilder> fields = null)
        {
            Title = title;
            TitleUrl = titleUrl;
            Description = description;
            Color = color == Color.Empty? Color.White : color;
            Timestamp = timestamp == default? DateTimeOffset.Now : timestamp;
            Fields = fields?? new();
        }

        public string Title { get; set; }
        public string TitleUrl { get; set; }

        public string Description { get; set; }

        public Color Color { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public object Footer { get; set; }
        public string ThumbnailUrl { get; set; }
        public object Author { get; set; }

        public List<FieldBuilder> Fields { get; set; }

        public object Build()
        {
            List<object> fieldList = new();

            Fields.ForEach(x => fieldList.Add(x.Build()));

            return new
            {
                title= Title,
                url= TitleUrl,
                description= Description,
                color= int.Parse(Color.ToHex(), System.Globalization.NumberStyles.HexNumber),
                timestamp= Timestamp.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                fields= fieldList
            };
        }

        public void AddField(string Name, string Value = "", bool Inline = false)
        {
            Fields.Add(new FieldBuilder(Name, Value, Inline));
        }
    }
}
