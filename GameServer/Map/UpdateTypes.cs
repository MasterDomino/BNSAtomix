namespace SagaBNS.GameServer.Map
{
    public enum RelationType
    {
        FaceTo = 0x24,
        Hold = 0x2B,
    }

    public enum StatusType
    {
        Combat=0x38,
    }

    public enum UpdateTypes
    {
        Movement = 1,
        Skill = 3,
        Effect = 4,
        Actor = 5,
        ActorExtension = 6,
        MapObjectOperate = 7,
        MapObjectVisibilityChange = 0xA,
        NPCTalk = 0xC,
        ItemAppear = 0xF,
        ItemShow = 0x11,
        ItemHide = 0x15,
        ItemDisappear = 0x16,
        ItemPick = 0x17,
        ItemPickCorpse = 0x18,
        ItemDrop = 0x19,
        ShowCorpse = 0x1A,
        DeleteCorpse = 0x1C,
        CorpseDoQuest,
        MapObjectDoQuest = 0x1F,
        CorpseInteraction = 0x22,
        PlayerRecover = 0x23,
        MapObjectInteraction = 0x24,
        Repair = 0x25,
        Teleport = 0x26,
        NPCDash = 0x27,
        DragonStream = 0x28,
        Appear,
        Disappear,
        Debug,
        /*Movement                    =   0x011B,
        ActorStatus                 =   0x0423,
        PlayerTurn                  =   0x0424,
        PlayerEquipChange           =   0x0426,
        ActorRelation               =   0x042A,
        Unknown50E                  =   0x050E,
        Unknown518                  =   0x0518,
        MapObjectOperate            =   0x0613,
        MapObjectVisibilityChange   =   0x0810,
        NPCTalk                     =   0x0C10,
        PlayerHoldItem              =   0x0E22,
        PlayerHoldItemCancel        =   0x1518,
        NPCDash                     =   0x251A,
        Debug,*/
    }
}
