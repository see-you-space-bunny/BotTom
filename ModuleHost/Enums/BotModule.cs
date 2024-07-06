using System.ComponentModel;

namespace ModuleHost
{
    public enum BotModule
    {
        [Description("")]
        None = 0x00,

        [Description("")]
        Clock,
        
        [Description("")]
        XCG,
        
        [Description("")]
        System,
    }
}