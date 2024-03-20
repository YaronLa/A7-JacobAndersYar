using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using System.Collections.Generic;

namespace Galaga.Squadron {
    public class DiagonalSquadron : ISquadron {
        public EntityContainer<Enemy> Enemies { get; }
        public int MaxEnemies { get; }
        
        public DiagonalSquadron(int maxEnemies) {
            Enemies = new EntityContainer<Enemy>(maxEnemies);
            MaxEnemies = maxEnemies;
        }
        
        /*Create enemies in a diagonal formation. If they near the bottom half of the screen (y<0.05f) a new diagonal is created next to the initial
        diagonal until there is no available space. 
        */
        public void CreateEnemies(List<Image> enemyStride, List<Image> alternativeEnemyStride) {
            float boundaryEnemyMax = 0.9f;
            float boundaryEnemyMinY = 0.5f;
            float startX = 0.1f;
            float startY = 0.9f;
            float spacingX = 0.1f; 
            float spacingY = 0.1f; 
            int enemiesAdded = 0; 
            float currentX = startX;
            float currentY = startY;
            while (enemiesAdded < MaxEnemies) {
                // Check if the new enemy would go out of bounds
                if (currentY < boundaryEnemyMinY || currentX > boundaryEnemyMax) {
                    break; 
                }
                // Add new enemy
                Enemies.AddEntity(new Enemy(
                    new DynamicShape(new Vec2F(currentX, currentY), new Vec2F(0.1f, 0.1f)),
                    new ImageStride(80, enemyStride)
                ));
                // Increment counter
                enemiesAdded++;
                // Update current position for next enemy 
                currentX += spacingX;
                currentY -= spacingY;
                // If we've reached the bottom half of the screen, begin a new diagonal
                if (currentY < boundaryEnemyMinY) {
                    currentX += spacingX;
                    currentY = startY; 
                }
            }
        }
    }
}
