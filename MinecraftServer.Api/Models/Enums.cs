using System.Runtime.Serialization;

namespace MinecraftServer.Api.Models
{
    public enum SystemEnum
    {
        WINDOWS,
        LINUX,
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
