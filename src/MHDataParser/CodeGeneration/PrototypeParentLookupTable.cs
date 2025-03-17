namespace MHDataParser.CodeGeneration
{
    public static class PrototypeParentLookupTable
    {
        // Hardcoded parents for classes that probably had the same hierarchy for the entire lifespan of the game
        private static readonly Dictionary<string, string> ParentLookup = new()
        {
            { "WorldEntityPrototype",   "EntityPrototype" },
            { "AgentPrototype",         "WorldEntityPrototype" },
            { "AvatarPrototype",        "AgentPrototype" },
            { "AgentTeamUpPrototype",   "AgentPrototype" },
            { "HotspotPrototype",       "WorldEntityPrototype" },
            { "ItemPrototype",          "WorldEntityPrototype" },
            { "CostumePrototype",       "ItemPrototype" },
            { "SpawnerPrototype",       "WorldEntityPrototype" },
            { "TransitionPrototype",    "WorldEntityPrototype" },
            // Not including PropPrototype because it seems to differ based on version (WorldEntity vs Agent?)

            { "PlayerStashInventoryPrototype",  "InventoryPrototype" },

            { "PowerPrototype",                 "Prototype" },
            { "MissilePowerPrototype",          "PowerPrototype" },
            { "SummonPowerPrototype",           "PowerPrototype" },
            { "MovementPowerPrototype",         "PowerPrototype" },
            { "PowerReplacementPowerPrototype", "PowerPrototype" },
            { "SpecializationPowerPrototype",   "PowerPrototype" },

            { "MissionActionEntityDestroyPrototype",        "MissionActionEntityTargetPrototype" },
            { "MissionActionEntityKillPrototype",           "MissionActionEntityTargetPrototype" },
            { "MissionActionEntityPerformPowerPrototype",   "MissionActionEntityTargetPrototype" },
            { "MissionActionEntitySetStatePrototype",       "MissionActionEntityTargetPrototype" },
            { "MissionActionSpawnerTriggerPrototype",       "MissionActionEntityTargetPrototype" },
            { "MissionActionShowOverheadTextPrototype",     "MissionActionEntityTargetPrototype" },
            { "MissionActionEntSelEvtBroadcastPrototype",   "MissionActionEntityTargetPrototype" },
            { "MissionActionAllianceSetPrototype",          "MissionActionEntityTargetPrototype" },

            { "PopulationClusterMixedPrototype",    "PopulationObjectPrototype" },
            { "PopulationClusterFixedPrototype",    "PopulationObjectPrototype" },
            { "PopulationClusterPrototype",         "PopulationObjectPrototype" },
            { "PopulationEncounterPrototype",       "PopulationObjectPrototype" },
            { "PopulationEntityPrototype",          "PopulationObjectPrototype" },
            { "PopulationLeaderPrototype",          "PopulationObjectPrototype" },
            { "PopulationFormationPrototype",       "PopulationObjectPrototype" },
            { "PopulationObjectListPrototype",      "Prototype" },

            { "MobKeywordPrototype",                "EntityKeywordPrototype" },
        };

        public static bool TryGetParentForPrototype(string prototypeClassName, out string parentName)
        {
            return ParentLookup.TryGetValue(prototypeClassName, out parentName);
        }
    }
}
