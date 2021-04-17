public class SpaceShooterData
{
    public static float PlayerStartWrapX = 21f;

    public static UnityEngine.Vector2 PlayerBoundLimitsY = new UnityEngine.Vector2(-9f, 8f);
    public static UnityEngine.Vector2 EnemyBoundLimitsY = new UnityEngine.Vector2(-12f, 12f);
    public static UnityEngine.Vector2 LaserBoundLimitsY = new UnityEngine.Vector2(-11f, 11f);

    public static float EnemySpawnX = 20f;

    public enum Levels { MainMenuScene, GameScene }
}