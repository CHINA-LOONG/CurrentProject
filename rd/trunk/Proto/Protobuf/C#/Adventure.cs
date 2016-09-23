//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Protocol/Adventure.proto
// Note: requires additional types generated from: Protocol/Const.proto
// Note: requires additional types generated from: Protocol/Reward.proto
// Note: requires additional types generated from: Protocol/Alliance.proto
namespace PB
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureTeam")]
  public partial class HSAdventureTeam : global::ProtoBuf.IExtensible
  {
    public HSAdventureTeam() {}
    
    private int _teamId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"teamId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int teamId
    {
      get { return _teamId; }
      set { _teamId = value; }
    }
    private int _adventureId;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"adventureId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int adventureId
    {
      get { return _adventureId; }
      set { _adventureId = value; }
    }
    private int _endTime;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"endTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int endTime
    {
      get { return _endTime; }
      set { _endTime = value; }
    }
    private readonly global::System.Collections.Generic.List<int> _selfMonsterId = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(4, Name=@"selfMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> selfMonsterId
    {
      get { return _selfMonsterId; }
    }
  
    private AllianceBaseMonster _hireMonster = null;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"hireMonster", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public AllianceBaseMonster hireMonster
    {
      get { return _hireMonster; }
      set { _hireMonster = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureCondition")]
  public partial class HSAdventureCondition : global::ProtoBuf.IExtensible
  {
    public HSAdventureCondition() {}
    
    private int _monsterCount;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"monsterCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int monsterCount
    {
      get { return _monsterCount; }
      set { _monsterCount = value; }
    }
    private int _conditionTypeCfgId;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"conditionTypeCfgId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int conditionTypeCfgId
    {
      get { return _conditionTypeCfgId; }
      set { _conditionTypeCfgId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventure")]
  public partial class HSAdventure : global::ProtoBuf.IExtensible
  {
    public HSAdventure() {}
    
    private int _adventureId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"adventureId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int adventureId
    {
      get { return _adventureId; }
      set { _adventureId = value; }
    }
    private readonly global::System.Collections.Generic.List<HSAdventureCondition> _condition = new global::System.Collections.Generic.List<HSAdventureCondition>();
    [global::ProtoBuf.ProtoMember(2, Name=@"condition", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<HSAdventureCondition> condition
    {
      get { return _condition; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureInfoSync")]
  public partial class HSAdventureInfoSync : global::ProtoBuf.IExtensible
  {
    public HSAdventureInfoSync() {}
    
    private int _teamCount;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"teamCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int teamCount
    {
      get { return _teamCount; }
      set { _teamCount = value; }
    }
    private readonly global::System.Collections.Generic.List<HSAdventureTeam> _busyTeam = new global::System.Collections.Generic.List<HSAdventureTeam>();
    [global::ProtoBuf.ProtoMember(2, Name=@"busyTeam", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<HSAdventureTeam> busyTeam
    {
      get { return _busyTeam; }
    }
  
    private readonly global::System.Collections.Generic.List<HSAdventure> _idleAdventure = new global::System.Collections.Generic.List<HSAdventure>();
    [global::ProtoBuf.ProtoMember(3, Name=@"idleAdventure", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<HSAdventure> idleAdventure
    {
      get { return _idleAdventure; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureEnter")]
  public partial class HSAdventureEnter : global::ProtoBuf.IExtensible
  {
    public HSAdventureEnter() {}
    
    private int _teamId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"teamId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int teamId
    {
      get { return _teamId; }
      set { _teamId = value; }
    }
    private int _type;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int type
    {
      get { return _type; }
      set { _type = value; }
    }
    private int _gear;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"gear", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int gear
    {
      get { return _gear; }
      set { _gear = value; }
    }
    private readonly global::System.Collections.Generic.List<int> _selfMonsterId = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(4, Name=@"selfMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> selfMonsterId
    {
      get { return _selfMonsterId; }
    }
  
    private AllianceBaseMonster _hireMonster = null;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"hireMonster", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public AllianceBaseMonster hireMonster
    {
      get { return _hireMonster; }
      set { _hireMonster = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureEnterRet")]
  public partial class HSAdventureEnterRet : global::ProtoBuf.IExtensible
  {
    public HSAdventureEnterRet() {}
    
    private int _teamId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"teamId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int teamId
    {
      get { return _teamId; }
      set { _teamId = value; }
    }
    private int _endTime;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"endTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int endTime
    {
      get { return _endTime; }
      set { _endTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureSettle")]
  public partial class HSAdventureSettle : global::ProtoBuf.IExtensible
  {
    public HSAdventureSettle() {}
    
    private int _teamId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"teamId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int teamId
    {
      get { return _teamId; }
      set { _teamId = value; }
    }
    private bool _pay;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"pay", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public bool pay
    {
      get { return _pay; }
      set { _pay = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureSettleRet")]
  public partial class HSAdventureSettleRet : global::ProtoBuf.IExtensible
  {
    public HSAdventureSettleRet() {}
    
    private int _teamId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"teamId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int teamId
    {
      get { return _teamId; }
      set { _teamId = value; }
    }
    private readonly global::System.Collections.Generic.List<RewardItem> _basicReward = new global::System.Collections.Generic.List<RewardItem>();
    [global::ProtoBuf.ProtoMember(2, Name=@"basicReward", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<RewardItem> basicReward
    {
      get { return _basicReward; }
    }
  
    private readonly global::System.Collections.Generic.List<RewardItem> _extraReward = new global::System.Collections.Generic.List<RewardItem>();
    [global::ProtoBuf.ProtoMember(3, Name=@"extraReward", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<RewardItem> extraReward
    {
      get { return _extraReward; }
    }
  
    private HSAdventure _adventure;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"adventure", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public HSAdventure adventure
    {
      get { return _adventure; }
      set { _adventure = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureNewCondition")]
  public partial class HSAdventureNewCondition : global::ProtoBuf.IExtensible
  {
    public HSAdventureNewCondition() {}
    
    private int _type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int type
    {
      get { return _type; }
      set { _type = value; }
    }
    private int _gear;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"gear", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int gear
    {
      get { return _gear; }
      set { _gear = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureNewConditionRet")]
  public partial class HSAdventureNewConditionRet : global::ProtoBuf.IExtensible
  {
    public HSAdventureNewConditionRet() {}
    
    private HSAdventure _adventure;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"adventure", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public HSAdventure adventure
    {
      get { return _adventure; }
      set { _adventure = value; }
    }
    private int _changeCount;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"changeCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int changeCount
    {
      get { return _changeCount; }
      set { _changeCount = value; }
    }
    private int _changeCountBeginTime;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"changeCountBeginTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int changeCountBeginTime
    {
      get { return _changeCountBeginTime; }
      set { _changeCountBeginTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureBuyCondition")]
  public partial class HSAdventureBuyCondition : global::ProtoBuf.IExtensible
  {
    public HSAdventureBuyCondition() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureBuyConditionRet")]
  public partial class HSAdventureBuyConditionRet : global::ProtoBuf.IExtensible
  {
    public HSAdventureBuyConditionRet() {}
    
    private int _changeCount;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"changeCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int changeCount
    {
      get { return _changeCount; }
      set { _changeCount = value; }
    }
    private int _changeCountBeginTime;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"changeCountBeginTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int changeCountBeginTime
    {
      get { return _changeCountBeginTime; }
      set { _changeCountBeginTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureBuyTeam")]
  public partial class HSAdventureBuyTeam : global::ProtoBuf.IExtensible
  {
    public HSAdventureBuyTeam() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureBuyTeamRet")]
  public partial class HSAdventureBuyTeamRet : global::ProtoBuf.IExtensible
  {
    public HSAdventureBuyTeamRet() {}
    
    private int _teamId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"teamId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int teamId
    {
      get { return _teamId; }
      set { _teamId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSAdventureConditionPush")]
  public partial class HSAdventureConditionPush : global::ProtoBuf.IExtensible
  {
    public HSAdventureConditionPush() {}
    
    private readonly global::System.Collections.Generic.List<HSAdventure> _idleAdventure = new global::System.Collections.Generic.List<HSAdventure>();
    [global::ProtoBuf.ProtoMember(1, Name=@"idleAdventure", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<HSAdventure> idleAdventure
    {
      get { return _idleAdventure; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}