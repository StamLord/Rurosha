using System.Collections;
using UnityEngine;
using TMPro;

public class WorldDialogue : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Animator animator;

    public void NewMessage(string message, float duration)
    {
        StartCoroutine(DisplayMessage(message, duration));
    }

    private IEnumerator DisplayMessage(string message, float duration)
    {
        text.text = message;
        animator.Play("start");

        yield return new WaitForSeconds(duration);

        animator.Play("end");
        text.text = "";
    }
}
