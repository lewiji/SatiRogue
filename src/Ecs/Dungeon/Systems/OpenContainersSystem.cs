using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Nodes.Items;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class OpenContainersSystem : ISystem
{
    public void Run(World world)
    {
        foreach (var (chestEntity, chest, _) in world.Query<Entity, Chest, OpeningContainer>().Build())
        {
            chest.Open = true;
            chest.BlocksCell = false;
            world.On(chestEntity).Remove<Closed>().Add<Open>();
            world.On(chestEntity).Remove<OpeningContainer>();
            chest.Enabled = false;
            var goldAmount = 1;
            var playerStore = world.GetElement<PersistentPlayerData>();
            playerStore.Gold += goldAmount;
            world.GetElement<Loot>().NumLoots = playerStore.Gold;
            world.GetElement<MessageLog>().AddMessage($"Retrieved {goldAmount} gold from chest.");
        }
    }
}