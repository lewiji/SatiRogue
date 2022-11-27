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
            chest.BlocksCell = false;
            chest.Enabled = false;
            
            world.On(chestEntity).Remove<Closed>().Add<Open>();
            world.On(chestEntity).Remove<OpeningContainer>();
            
            if (chest.AnimatedSprite3D != null) chest.AnimatedSprite3D.Animation = "opening";
            if (chest.Particles != null) chest.Particles.Emitting = true;
            
            var goldAmount = 1; // TODO
            var playerStore = world.GetElement<PersistentPlayerData>();
            playerStore.Gold += goldAmount;
            world.GetElement<Loot>().NumLoots = playerStore.Gold;
            world.GetElement<MessageLog>().AddMessage($"Retrieved {goldAmount} gold from chest.");
        }
    }
}