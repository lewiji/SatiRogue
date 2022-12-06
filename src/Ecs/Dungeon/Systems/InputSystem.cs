using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public partial class InputSystem : ISystem {
   

   public static bool HandlingInput = true;
   public static bool Paused = false;
   public static bool PlayerInputted = false;
   public static bool PlayerShot = false;
   InputDirectionComponent? _playerInputDirectionComponent;

   World? _world;
   Player? _player;
   Entity? _playerEntity;
   public void Run(World world)
   {
      _world ??= world;
      _player ??= world.GetElement<Player>();
      _playerEntity ??= _player.GetEntity();
      if (Paused || !_player.Alive)
         return;
      _playerInputDirectionComponent ??= world.GetComponent<InputDirectionComponent>(_playerEntity);
      if (_playerInputDirectionComponent == null) return;
      
      var aim = Input.IsActionPressed("aim");
      var diagonalLock = Input.IsActionPressed("diagonal_lock");
      var shoot = Input.IsActionJustPressed("shoot");
      var direction = new Vector2(0, 0); //, "move_right", "move_up", "move_down"

      if (Input.IsActionPressed("move_left")) {
         direction.x = -1;
      } else if (Input.IsActionPressed("move_right")) {
         direction.x = 1;
      }

      if (Input.IsActionPressed("move_up")) {
         direction.y = -1;
      } else if (Input.IsActionPressed("move_down")) {
         direction.y = 1;
      }

      _playerInputDirectionComponent.Direction = direction.Round();

      if (diagonalLock)
      {
         HandleDiagonalLockedInput(_playerInputDirectionComponent);
      }
      else
      {
         HandleUnlockedInput(aim, _playerInputDirectionComponent, shoot);
      }
   }

   void HandleUnlockedInput(bool aim, InputDirectionComponent input, bool shoot) {
      if (Paused)
         return;

      if (_world!.HasComponent<DiagonalLock>(_playerEntity!)) {
         _world!.On(_playerEntity!).Remove<DiagonalLock>();
         _player!.DiagonalLockIndicator.Visible = false;
      }

      if (!aim && HandlingInput && input.Direction != Vector2.Zero) {
         /* It's sending a message to the `PlayerInputSystem` to tell it to run. */
         Logger.Info($"InputSystem: Detected player input: {input.Direction}");
         PlayerInputted = true;
         HandlingInput = false;
      } else if (shoot) {
         PlayerInputted = true;
         PlayerShot = true;
         HandlingInput = false;
      }

      if (aim && !_world.HasComponent<Aiming>(_playerEntity!)) {
	      _world!.On(_playerEntity!).Add<Aiming>();
	      _player!.DirectionIndicator.Visible = true;
      } else {
         RemoveAim();
      }
   }

   void HandleDiagonalLockedInput(InputDirectionComponent input) {
      RemoveAim();

      if (!_world!.HasComponent<DiagonalLock>(_playerEntity!)) {
         _world!.On(_playerEntity!).Add<DiagonalLock>();
         _player!.DiagonalLockIndicator.Visible = true;
      }

      if (HandlingInput && InputIsDiagonal(input.Direction)) {
         PlayerInputted = true;
         HandlingInput = false;
      }
   }

   bool InputIsDiagonal(Vector2 inputDirection) {
      if (inputDirection == Vector2.Zero)
         return false;
      return inputDirection.y != 0 && inputDirection.x != 0;
   }

   void RemoveAim()
   {
	   if (_player == null || _world == null) return;
	   if (!_world.HasComponent<Aiming>(_playerEntity!)) return;
	   _world.On(_playerEntity!).Remove<Aiming>();
	   _player.DirectionIndicator.Visible = false;
   }
}