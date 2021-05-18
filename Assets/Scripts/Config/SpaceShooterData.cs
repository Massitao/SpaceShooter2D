public class SpaceShooterData
{
    public static float WrapX = 21f;
    public static float SpawnX = 20f;

    public static UnityEngine.Vector2 PlayerBoundLimitsY = new UnityEngine.Vector2(-9f, 8f);
    public static float EnemyBoundLimitsY = 12f;
    public static float LaserBoundLimitsY = 11f;
    public static float MaxEnemyRotation = 30f;


    public enum Enemies { Rookie, Bomber, Toxic, Reckless, Watcher, Avoider, Collector, Asteroid }
    public enum Levels { MainMenuScene, GameScene }
}