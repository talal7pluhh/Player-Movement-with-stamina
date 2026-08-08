using UnityEngine;
using UnityEngine.UI;
public class StaminaBar : MonoBehaviour
{
    [SerializeField] float stamina = 100f;
    [SerializeField] float maxStamina = 100f;
    [SerializeField] bool isSprinting;
    [SerializeField] float drainRate = 10f;
    [SerializeField] float rechargeRate = 10f;
    [SerializeField] bool isFatigued;
    [SerializeField] Image staminaBar;
    [SerializeField] CanvasGroup canvas_group;
    [SerializeField] float jumpCost = 20f;
    [SerializeField] private float FadeSpeed = 8f;
    public bool canSprint => stamina > 0f && !isFatigued;
    private void Update()
    {
        if (!isSprinting)
        {
            if (stamina < maxStamina)
            {
                stamina += rechargeRate * Time.deltaTime;
            }
        }
        if (isFatigued && stamina >= (maxStamina * 0.2f))
        {
            isFatigued = false;
        }
        stamina = Mathf.Clamp(stamina, 0f, maxStamina);
        FillStamina();
        isSprinting = false;
    }

    public void Sprinting()
    {
        if (isFatigued) return;

        
            isSprinting = true;
            stamina -= drainRate * Time.deltaTime;
            if (stamina <= 0f)
            {
                stamina = 0f;
                isFatigued = true;
            }
        
    }

    public bool JumpStamina()
    {
        if (stamina >= jumpCost)
        {
            stamina -= jumpCost;
            return true;
        } return false;
    }

    void FillStamina()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = stamina / maxStamina;
        }

        float targetAlpha = (stamina < maxStamina || isSprinting) ? 1f : 0f;
        if (canvas_group != null)
        {
            canvas_group.alpha = Mathf.MoveTowards(canvas_group.alpha, targetAlpha, FadeSpeed * Time.deltaTime);
        }

    }
}
