using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Attached onto enemies and player
public class KnockbackFeedback : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    [SerializeField] float strength, delay;

    public UnityEvent onBegin, onDone;

    public void playFeedback(GameObject sender)
    {
        StopAllCoroutines();
        onBegin?.Invoke();
        Vector2 dir = (transform.position - sender.transform.position).normalized;
        body.AddForce(dir * strength, ForceMode2D.Impulse);
        StartCoroutine("reset");
    }

    IEnumerator reset()
    {
        yield return new WaitForSeconds(delay);
        onDone?.Invoke();
    }
}
