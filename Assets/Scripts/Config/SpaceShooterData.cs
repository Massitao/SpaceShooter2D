public class SpaceShooterData
{
    public static float WrapX = 21f;
    public static float SpawnX = 20f;

    public static UnityEngine.Vector2 PlayerBoundLimitsY = new UnityEngine.Vector2(-9f, 8f);
    public static UnityEngine.Vector2 EnemyBoundLimitsY = new UnityEngine.Vector2(-12f, 12f);
    public static UnityEngine.Vector2 LaserBoundLimitsY = new UnityEngine.Vector2(-11f, 11f);

    public enum Enemies { Rookie }
    public enum Levels { MainMenuScene, GameScene }
}