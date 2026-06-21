using UnityEngine;

public class Ak47 : BaseWeapon
{
    protected override void Start()
    {
        base.Start();

        // Forzamos a que empiece exactamente en el centro de la cámara
        hipPosition = Vector3.zero; 
        adsPosition = new Vector3(0f, -0.1f, 0.2f); // Un ajuste leve para el ADS

        hipSpread = 0.06f;
        adsSpread = 0.008f;
        recoilForce = 3.5f;
    }
}