
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg
{
public partial struct ComboPoint
{
    public ComboPoint(JSONNode _buf) 
    {
        { if(!_buf["combo"].IsNumber) { throw new SerializationException(); }  Combo = _buf["combo"]; }
        { if(!_buf["perfect_point"].IsNumber) { throw new SerializationException(); }  PerfectPoint = _buf["perfect_point"]; }
        { if(!_buf["great_point"].IsNumber) { throw new SerializationException(); }  GreatPoint = _buf["great_point"]; }
        { if(!_buf["good_point"].IsNumber) { throw new SerializationException(); }  GoodPoint = _buf["good_point"]; }
        { if(!_buf["miss_point"].IsNumber) { throw new SerializationException(); }  MissPoint = _buf["miss_point"]; }
    }

    public static ComboPoint DeserializeComboPoint(JSONNode _buf)
    {
        return new ComboPoint(_buf);
    }

    public readonly int Combo;
    public readonly int PerfectPoint;
    public readonly int GreatPoint;
    public readonly int GoodPoint;
    public readonly int MissPoint;
   

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "combo:" + Combo + ","
        + "perfectPoint:" + PerfectPoint + ","
        + "greatPoint:" + GreatPoint + ","
        + "goodPoint:" + GoodPoint + ","
        + "missPoint:" + MissPoint + ","
        + "}";
    }
}

}

