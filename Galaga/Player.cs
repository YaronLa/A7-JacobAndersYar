using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Events;
using System.Collections.Generic;

namespace Galaga
{
    public class Player : IGameEventProcessor {
        private Entity entity;
        private DynamicShape shape;
        private float moveLeft = 0.0f;
        private float moveRight = 0.0f;
        private float moveUp = 0.0f;
        private float moveDown = 0.0f;
        private const float MOVEMENT_SPEED = 0.01f;


        public Player(DynamicShape shape, IBaseImage image) {
            entity = new Entity(shape, image);
            this.shape = shape;
           // eventBus = new GameEventBus();
            //eventBus.InitializeEventBus(new List<GameEventType> { GameEventType.MovementEvent });
            //eventBus.Subscribe(GameEventType.MovementEvent, this);
            GalagaBus.GetBus().Subscribe(GameEventType.MovementEvent, this);
        }

        public DynamicShape getShape() {
            return shape;
        }

        public void Render() {
            this.entity.RenderEntity();
        }

        public void Move() { 
            if (this.shape.Position.X < 0.0f) {
               this.shape.Position.X = 0.0f;
            } else if (this.shape.Position.X + this.shape.Extent.X > 1.0f) {
                this.shape.Position.X = 1.0f - this.shape.Extent.X;
            } else if (this.shape.Position.Y < 0.0f) {
                this.shape.Position.Y = 0.0f;
            } else if (this.shape.Position.Y + this.shape.Extent.Y > 1.0f) {
                this.shape.Position.Y = 1.0f - this.shape.Extent.Y;
            } else {
                this.shape.Move();
            }
        }

        private void SetMoveLeft(bool val) {
            if (val) {
                this.moveLeft -= MOVEMENT_SPEED;
            } else {
                this.moveLeft = 0.0f;
            }
            UpdateDirection();
        }

        private void SetMoveRight(bool val) {
            if (val) {
                this.moveRight += MOVEMENT_SPEED;
            } else {
                this.moveRight = 0.0f;
            }
            UpdateDirection();
        }

        public void SetMoveUp(bool val) {
            if (val) {
                this.moveUp += MOVEMENT_SPEED;
            } else {
                this.moveUp = 0.0f;
            }
            UpdateDirection();
        }

        public void SetMoveDown(bool val) {
            if (val) {
                this.moveDown -= MOVEMENT_SPEED;
            } else {
                this.moveDown = 0.0f;
            }
            UpdateDirection();
        }

        private void UpdateDirection() {
            Vec2F direction = new Vec2F(this.moveLeft + this.moveRight, this.moveUp + this.moveDown);
            this.shape.ChangeDirection(direction);
        }

        public Vec2F GetPosition() {
            Vec2F position = new Vec2F(this.shape.Position.X, this.shape.Position.Y);
            return position;
        }
        public void ProcessEvent(GameEvent gameEvent) {
            if (gameEvent.EventType == GameEventType.MovementEvent){
                switch (gameEvent.Message) {
                    case "MoveLeft":
                        SetMoveLeft(true);
                        break;
                    case "MoveRight":
                        SetMoveRight(true);
                        break;
                    case "MoveUp":
                        SetMoveUp(true);
                        break;
                    case "MoveDown":
                        SetMoveDown(true);
                        break;
                    case "StopLeft":
                        SetMoveLeft(false);
                        break;
                    case "StopRight":
                        SetMoveRight(false);
                        break;
                    case "StopUp":
                        SetMoveUp(false);
                        break;
                    case "StopDown":
                        SetMoveDown(false);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}