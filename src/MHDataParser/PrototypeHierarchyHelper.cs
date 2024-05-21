namespace MHDataParser
{
    public static class PrototypeHierarchyHelper
    {
        private static readonly string[] EntityPrototypes = new string[]
        {
            "AgentPrototype",
            "AgentTeamUpPrototype",
            "ArmorPrototype",
            "ArtifactPrototype",
            "AvatarPrototype",
            "BagItemPrototype",
            "CharacterTokenPrototype",
            "CostumeCorePrototype",
            "CostumePrototype",
            "CraftingIngredientPrototype",
            "CraftingRecipePrototype",
            "DestructiblePropPrototype",
            "DestructibleSmartPropPrototype",
            "EmoteTokenPrototype",
            "EntityPrototype",
            "HotspotPrototype",
            "InventoryExtraSlotsGrantPrototype",
            "InventoryStashTokenPrototype",
            "ItemPrototype",
            "KismetSequenceEntityPrototype",
            "LegendaryPrototype",
            "MatchMetaGamePrototype",
            "MedalPrototype",
            "MetaGamePrototype",
            "MissilePrototype",
            "MissionMetaGamePrototype",
            "OmegaPrestigeUnlockPrototype",
            "OrbPrototype",
            "PlayerPrototype",
            "PropPrototype",
            "PvPPrototype",
            "RelicPrototype",
            "SmartPropPrototype",
            "SpawnerPrototype",
            "TeamUpGearPrototype",
            "TransitionPrototype",
            "WorldEntityPrototype"
        };

        private static readonly string[] InventoryPrototypes = new string[]
        {
            "InventoryPrototype",
            "PlayerStashInventoryPrototype"
        };

        private static readonly string[] PowerPrototypes = new string[]
        {
            "MissilePowerPrototype",
            "MovementPowerPrototype",
            "PowerPrototype",
            "SpecializationPowerPrototype",
            "SummonPowerPrototype"
        };

        public static bool IsEntityPrototype(string prototypeClassName)
        {
            return EntityPrototypes.Contains(prototypeClassName);
        }

        public static bool IsInventoryPrototype(string prototypeClassName)
        {
            return InventoryPrototypes.Contains(prototypeClassName);
        }

        public static bool IsPowerPrototype(string prototypeClassName)
        {
            return PowerPrototypes.Contains(prototypeClassName);
        }
    }
}
