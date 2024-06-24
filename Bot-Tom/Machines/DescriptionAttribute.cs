using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotTom.Machines;

[AttributeUsage(AttributeTargets.All)]
sealed class DescriptionAttribute : Attribute
{
    // See the attribute guidelines at
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    readonly string description;
    
    // This is a positional argument
    public DescriptionAttribute(string description)
    {
        this.description = description;
        
        // TODO: Implement code here
        /* throw new System.NotImplementedException(); */
    }
    
    public string Description
    {
        get { return Description; }
    }
    
    // This is a named argument
    /* public int NamedInt { get; set; } */
}