using UnityEngine;

public class KatanaOrbitShield : Weapon 
{
    protected override void SpawnBullet() {
        // Thằng này dị biệt: Mỗi lần vung kiếm, nó đẻ ra kiếm khí 
        // rồi ép cái kiếm khí đó làm con của Player, cho nó tự xoay vòng vòng quanh người!
        GameObject orbitSword = Instantiate(bulletPrefab, transform.parent.position, Quaternion.identity);
        orbitSword.transform.SetParent(transform.parent); // Bám theo Player
        // Logic tự xoay viết trong script của viên đạn orbit đó
    }
    public override void UpgradeStats(int level) { /* Tăng số lượng kiếm xoay từ 2 cây lên 4 cây... */ }
}