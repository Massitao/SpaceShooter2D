public class TripleLaserPowerUp : PowerUp
{
    protected override void PickUp(Ship player)
    {
        player.ActivateTripleShot();
    }
}