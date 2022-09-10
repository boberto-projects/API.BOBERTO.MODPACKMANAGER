using System.Runtime.Serialization;

namespace MinecraftServer.Api.Models
{
    public enum SystemEnum
    {
        [EnumMember(Value = "windows")]
        WINDOWS,
        [EnumMember(Value = "linux")]
        LINUX,
        [EnumMember(Value = "mac")]
        MAC
    }

    public enum FolderType
    {
        [EnumMember(Value = "MOD")]
        MOD,
        //TODO: ALTERAR ESSA REFERÊNCIA NO LAUNCHER DEPOIS
        [EnumMember(Value = "VERIONSCUSTOM")]
        VERSIONCUSTOM,
        [EnumMember(Value = "FILE")]
        FILE,
        [EnumMember(Value = "LIBRARY")]
        LIBRARY
    }
}
