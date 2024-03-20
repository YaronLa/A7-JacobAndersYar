using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using System;
using System.Collections.Generic;

namespace Galaga.Squadron {
    public class RandomSquadron : ISquadron {
        public EntityContainer<Enemy> Enemies { get; }
        public int MaxEnemies { get; }
        
        public RandomSquadron(int maxEnemies) {
            Enemies = new EntityContainer<Enemy>(maxEnemies);
            MaxEnemies = maxEnemies;
        }

        private bool OverlapPrevention(Vec2F posA, Vec2F posB) {
            // Calculate the distance by euclidean formula.
            float distance = (float)Math.Sqrt(Math.Pow(posA.X - posB.X, 2) + Math.Pow(posA.Y - posB.Y, 2));

            // Set the threshold of overlap to 0.1f
            return distance < 0.1f;
        }

        /* Create enemies randomly on the top half of the screen using rand.nextdouble(). 
        Ensuring no overlap and respect to the game boarders
        */
        public void CreateEnemies(List<Image> enemyStride, List<Image> alternativeEnemyStride) {
            Random rand = new Random();
            int enemiesAdded = 0; 
            float boundaryEnemyMax = 0.9f;
            float boundaryEnemyMinX = 0.1f;
            float boundaryEnemyMinY = 0.5f;

            while (enemiesAdded < MaxEnemies) {
                float posX = (float)rand.NextDouble();
                float posY = (float)rand.NextDouble();
                // Check if the new enemy would go out of bounds
                if (posY >= boundaryEnemyMinY && posY <= boundaryEnemyMax &&
                    posX >= boundaryEnemyMinX && posX <= boundaryEnemyMax) {
                    bool canAddEnemy = true;
                    foreach (Enemy enemy in Enemies) {
                        if (OverlapPrevention(enemy.Shape.Position, new Vec2F(posX, posY))) {
                            canAddEnemy = false;
                            break;
                        }
                    }
                    if (canAddEnemy) {
                        // Add new enemy
                        Enemies.AddEntity(new Enemy(
                            new DynamicShape(new Vec2F(posX, posY), new Vec2F(0.1f, 0.1f)),
                            new ImageStride(80, enemyStride)
                        ));

                        enemiesAdded++; 
                    }
                }
            }
        }
    }
}
