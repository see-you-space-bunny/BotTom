using System.Diagnostics.Tracing;
using FileManip;

namespace Charsheet.ForgedInTheDark;

public class Clock : IBinarySerializable
{
  #region F(-)
  private string _label;
  private string _description;
  private string _group;
  private ushort _faceSize;
  private ushort _progressFilled;
  #endregion

  #region P(+)
  public string Label => _label;
  public string Description => _description;
  public string Group => _group;
  public ushort FaceSize => _faceSize;
  public ushort ProgressFilled => _progressFilled;
  public ushort ProgressEmpty => (ushort)(_faceSize - _progressFilled);
  public bool IsComplete => _faceSize == _progressFilled;
  #endregion

  public Clock() : this(string.Empty,2,0) {}

  public Clock(string label, string group, int faceSize, int progressFilled = 0)
    : this(label,group,(ushort)faceSize,(ushort)progressFilled)
  { }

  public Clock(string label, string group, ushort faceSize, ushort progressFilled = 0)
    : this(label,faceSize,progressFilled)
  { _group = group; }

  public Clock(string label, int faceSize, int progressFilled = 0)
    : this(label,(ushort)faceSize,(ushort)progressFilled)
  { }

  public Clock(string label, ushort faceSize, ushort progressFilled = 0)
  {
    _label = label;
    _description = string.Empty;
    _faceSize = faceSize;
    _progressFilled = progressFilled;
    _group = string.Empty;
  }

  public void AddDescription(string description) => _description = $"> {description.Replace("\\n","\n> ")}";
  public void AddProgress(int progress) => AddProgress((ushort)progress);
  public void AddProgress(ushort progress) => _progressFilled = (ushort)Math.Min(_progressFilled+progress,_faceSize);

  public void AddFaces(int faces) => AddFaces((ushort)faces);
  public void AddFaces(ushort faces) => _faceSize = (ushort)(_faceSize+faces);

  public void Deserialize(BinaryReader reader)
  {
    _label = reader.ReadString();
    _description = reader.ReadString();
    _group = reader.ReadString();
    _faceSize = reader.ReadUInt16();
    _progressFilled = reader.ReadUInt16();
  }

  public void Serialize(BinaryWriter writer)
  {
    writer.Write(_label);
    writer.Write(_description);
    writer.Write(_group);
    writer.Write(_faceSize);
    writer.Write(_progressFilled);
  }

  public override string ToString() => 
    $"{(IsComplete?"**":string.Empty)}` {_progressFilled.ToString().PadLeft(2,' ')} `​`/`​` {_faceSize.ToString().PadLeft(2,' ')} ` {_label}{(Group!=string.Empty?$" ({Group})":string.Empty)}{(IsComplete?"**":string.Empty)}";

  public string ToStringWithDescription() =>
    ToString()+$"{(Description!=string.Empty?Description:string.Empty)}" ;
}
