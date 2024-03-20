using DIKUArcade.Input;
using DIKUArcade.Events;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade;
using DIKUArcade.GUI;
using System.Collections.Generic;
using DIKUArcade.Physics;
using System;
using Galaga.Squadron;

namespace Galaga
{
    public class Game : DIKUGame , IGameEventProcessor {
        private Player player;
        private GameEventBus eventBus;
        private EntityContainer<Enemy> enemies;
        private EntityContainer<PlayerShot> playerShots;
        private IBaseImage playerShotImage;
        private AnimationContainer enemyExplosions;
        private List<Image> explosionStrides;
        private const int EXPLOSION_LENGTH_MS = 500;

        public Game(WindowArgs windowArgs) : base(windowArgs) {
            // Player
            player = new Player(
                new DynamicShape(new Vec2F(0.45f, 0.1f), new Vec2F(0.1f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "Player.png")));
            //Eventbus
            eventBus = new GameEventBus();
            eventBus.InitializeEventBus(new List<GameEventType> { GameEventType.InputEvent, GameEventType.WindowEvent, GameEventType.MovementEvent});
            window.SetKeyEventHandler(KeyHandler);
            eventBus.Subscribe(GameEventType.InputEvent, this);
            eventBus.Subscribe(GameEventType.WindowEvent, this);
            eventBus.Subscribe(GameEventType.MovementEvent, this);
            // Enemies 
            List<Image> images = ImageStride.CreateStrides(4, Path.Combine("Assets", "Images", "BlueMonster.png"));
            const int numEnemies = 10;
            enemies = new EntityContainer<Enemy>(numEnemies);
            ISquadron squadron = new VerticalSquadron(numEnemies);
            squadron.CreateEnemies(images, null);
            enemies = squadron.Enemies;
           /* for (int i = 0; i < numEnemies; i++) {
                enemies.AddEntity(new Enemy(
                    new DynamicShape(new Vec2F(0.1f + (float)i * 0.1f, 0.9f), new Vec2F(0.1f, 0.1f)),
                    new ImageStride(80, images)));
                    
            }
            */
            // Playershots
            playerShots = new EntityContainer<PlayerShot>();
            playerShotImage = new Image(Path.Combine("Assets", "Images", "BulletRed2.png"));
            // Enemy explosion
            enemyExplosions = new AnimationContainer(numEnemies);
            explosionStrides = ImageStride.CreateStrides(8, Path.Combine("Assets", "Images", "Explosion.png"));
        }
     
        public override void Render() {
            player.Render();
            enemies.RenderEntities();
            playerShots.RenderEntities();
            enemyExplosions.RenderAnimations();
        }

        public override void Update() {
            eventBus.ProcessEventsSequentially();
            player.Move();
            IterateShots();
        }

        private void KeyPress(KeyboardKey key) {
            switch (key) {
                case KeyboardKey.Escape:
                    var closeWindowEvent = new GameEvent {EventType = GameEventType.WindowEvent, Message="CloseWindow"};
                    eventBus.RegisterEvent(closeWindowEvent);
                    break;
                case KeyboardKey.Left:
                    eventBus.RegisterEvent(new GameEvent {EventType = GameEventType.MovementEvent, To = player, Message="MoveLeft"});
                    break;
                case KeyboardKey.Right:
                    eventBus.RegisterEvent(new GameEvent {EventType = GameEventType.MovementEvent, To = player, Message="MoveRight"});
                    break;
                case KeyboardKey.Up:
                    eventBus.RegisterEvent(new GameEvent {EventType = GameEventType.MovementEvent, To = player, Message="MoveUp"});
                    break;
                case KeyboardKey.Down:
                    eventBus.RegisterEvent(new GameEvent {EventType = GameEventType.MovementEvent, To = player, Message="MoveDown"});
                    break;
        }
    }

        private void KeyRelease(KeyboardKey key) {
            switch (key) {
                case KeyboardKey.Left:
                    eventBus.RegisterEvent(new GameEvent {EventType = GameEventType.MovementEvent, To = player, Message="StopLeft"});
                    break;
                case KeyboardKey.Right:
                    eventBus.RegisterEvent(new GameEvent {EventType = GameEventType.MovementEvent, To = player, Message="StopRight"});
                    break;
                case KeyboardKey.Up:
                    eventBus.RegisterEvent(new GameEvent {EventType = GameEventType.MovementEvent, To = player, Message="StopUp"});
                    break;
                case KeyboardKey.Down:
                    eventBus.RegisterEvent(new GameEvent {EventType = GameEventType.MovementEvent, To = player, Message="StopDown"});
                    break;
                case KeyboardKey.Space:
                    Vec2F shotPosition = player.GetPosition() + new Vec2F(player.getShape().Extent.X / 2, player.getShape().Extent.Y / 2);
                    PlayerShot shot = new PlayerShot(shotPosition, playerShotImage);
                    playerShots.AddEntity(shot); 
                    break;
            }
        }

        private void KeyHandler(KeyboardAction action, KeyboardKey key) {
            switch (action) {
                case KeyboardAction.KeyPress:
                    KeyPress(key);
                    break; 
                case KeyboardAction.KeyRelease:
                    KeyRelease(key);
                    break;
                default:
                    break;
            }
        }

        public void ProcessEvent(GameEvent gameEvent) {
            switch (gameEvent.Message) {
                case "CloseWindow":
                    window.CloseWindow();
                    break;
                default:
                    break;
            }
        }

        private void IterateShots() {
            playerShots.Iterate(shot => {
                shot.Shape.Move(shot.Direction);
                if (shot.Shape.Position.Y > 1.0f) {
                    shot.DeleteEntity();
                } else {
                    enemies.Iterate(enemy => {
                        if (CollisionDetection.Aabb(shot.Shape.AsDynamicShape(), enemy.Shape).Collision) {
                            shot.DeleteEntity();
                            enemy.DeleteEntity();
                            AddExplosion(enemy.Shape.Position, enemy.Shape.Extent);
                        } 
                    }); 
                }
            });
        }   

        public void AddExplosion(Vec2F position, Vec2F extent) { 
            StationaryShape explosion = new(position, extent);  
            ImageStride explosion_image = new(EXPLOSION_LENGTH_MS, explosionStrides);
            enemyExplosions.AddAnimation(explosion, EXPLOSION_LENGTH_MS/8, explosion_image);
        }
    }
}