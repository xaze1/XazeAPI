using PlayerRoles;
using LabApi.Features.Wrappers;
using RueI.Extensions.HintBuilding;
using System.Drawing;
using System.Text;
using XazeAPI.API.Enums;
using XazeAPI.API.Helpers;

namespace XazeAPI.API.Structures
{
    public struct MVPInfoStruct
    {
        public MVPInfoType InfoType { get; private set; }
        public float Data { get; private set; }
        public string Text { get; private set; }
        public string Nickname { get; private set; }
        public Color Color { get; private set; }
        public StringBuilder Builder { get; private set; }
        public string Plaintext { get; private set; }

        public MVPInfoStruct(string nickname, float data, string mvpMessage, Color teamColor, MVPInfoType infoType)
        {
            InfoType = infoType;
            Nickname = nickname;
            Data = data;
            Text = mvpMessage;
            Color = teamColor;

            Plaintext = Nickname + Text + Data;
            Builder = new StringBuilder()
                .SetColor(Color)
                .Append(Nickname)
                .CloseColor()
                .Append(Text)
                .Append(InfoType == MVPInfoType.FirstToEscape ? MainHelper.getMinutes(Data) : Data);
        }
        
        public MVPInfoStruct(CustomPlayer plr, float data, string mvpMessage, MVPInfoType infoType)
        {
            InfoType = infoType;
            Nickname = plr.Username;
            Data = data;
            Text = mvpMessage;
            Color = MainHelper.getColorFromTeam(plr.LastRole.GetTeam());

            Plaintext = Nickname + Text + Data;
            Builder = new StringBuilder()
                .SetColor(Color)
                .Append(Nickname)
                .CloseColor()
                .Append(Text)
                .Append(InfoType == MVPInfoType.FirstToEscape ? MainHelper.getMinutes(Data) : Data);
        }
        
        public MVPInfoStruct(Player plr, float data, string mvpMessage, MVPInfoType infoType)
        {
            InfoType = infoType;
            Nickname = plr.DisplayName;
            Data = data;
            Text = mvpMessage;
            Color = MainHelper.getColorFromTeam(plr.Team);

            Plaintext = Nickname + Text + Data;
            Builder = new StringBuilder()
                .SetColor(Color)
                .Append(Nickname)
                .CloseColor()
                .Append(Text)
                .Append(InfoType == MVPInfoType.FirstToEscape ? MainHelper.getMinutes(Data) : Data);
        }
        
        public MVPInfoStruct(ReferenceHub plr, float data, string mvpMessage, MVPInfoType infoType)
        {
            InfoType = infoType;
            Nickname = plr.nicknameSync.DisplayName;
            Data = data;
            Text = mvpMessage;
            Color = MainHelper.getColorFromTeam(plr.GetTeam());

            Plaintext = Nickname + Text + Data;
            Builder = new StringBuilder()
                .SetColor(Color)
                .Append(Nickname)
                .CloseColor()
                .Append(Text)
                .Append(InfoType == MVPInfoType.FirstToEscape ? MainHelper.getMinutes(Data) : Data);
        }

        public override string ToString()
        {
            return Builder?.ToString()?? Plaintext;
        }
    }
}
