namespace XazeAPI.API.DiscordWebhook.Classes
{
    public class FieldBuilder
    {
        public FieldBuilder()
        {
            Name = "";
            Value = "";
            Inline = false;
        }
        
        public FieldBuilder(string name, string value = "", bool inline = false)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public bool Inline { get; set; }

        public object Build()
        {
            return new
            {
                name= Name,
                value= Value,
                inline= Inline,
            };
        }
    }
}
