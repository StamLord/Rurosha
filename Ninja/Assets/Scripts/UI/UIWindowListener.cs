using UnityEngine;
using UnityEngine.Events;

public class UIWindowListener : MonoBehaviour
{
    [SerializeField] private UIWindow window;

    [SerializeField] private UnityEvent[] onWindowOpen;
    [SerializeField] private UnityEvent[] onWindowClose;

    void Start()
    {
        window.OnWindowOpen += ExecuteOpen;
        window.OnWindowClose += ExecuteClose;
    }

    private void ExecuteOpen()
    {
        foreach(UnityEvent e in onWindowOpen)
            e.Invoke();
    }

    private void ExecuteClose()
    {
        foreach(UnityEvent e in onWindowClose)
            e.Invoke();
    }
}
