using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManip;

namespace Charsheet.ForgedInTheDark;

public static class ClockCabinet
{
    public const string FilePath = "sessiondata";
    public const string FileName = "ClockCabinet";

    private static readonly Dictionary<ulong,Dictionary<string,Clock>> _clocks;

    public static Dictionary<ulong,Dictionary<string,Clock>> Clocks => _clocks;

    static ClockCabinet()
    {
        _clocks = [];
        Deserialize();
    }

    public static void Deserialize()
    {
        string serialPath = Path.Combine(Environment.CurrentDirectory,FilePath,FileName);
        if (File.Exists(serialPath))
        {
            var stream = File.Open(serialPath,FileMode.Open);
            BinaryReader reader = new (stream,Encoding.UTF8,false);

            uint cabinetSize = reader.ReadUInt32();
            if (cabinetSize > 0)
                for(uint i=cabinetSize;i>0;i--)
                {
                    ulong newUser = reader.ReadUInt64();
                    Dictionary<string,Clock> newUserClocks = [];
                    for(uint j=reader.ReadUInt32();j>0;j--)
                    {
                        Clock newClock = new();
                        newClock.Deserialize(reader);
                        newUserClocks.Add(newClock.Label,newClock);
                    }
                    _clocks.Add(newUser,newUserClocks);
                }
            stream.Dispose();
        }
    }

    public static void Serialize()
    {
		string serialPath = Path.Combine(Environment.CurrentDirectory,FilePath);

		if (!Directory.Exists(serialPath))
			Directory.CreateDirectory(serialPath);
		
        serialPath = Path.Combine(serialPath,FileName);
		
        var stream = File.Open(serialPath,FileMode.Create);
        BinaryWriter writer = new (stream,Encoding.UTF8,false);
        
        writer.Write((UInt32)_clocks.Count);
        foreach(var userClocks in _clocks)
        {
            writer.Write((UInt64)userClocks.Key);
            writer.Write((UInt32)userClocks.Value.Count);
            foreach(Clock clock in userClocks.Value.Values)
            {
                clock.Serialize(writer);
            }
        }
        stream.Dispose();
    }
}