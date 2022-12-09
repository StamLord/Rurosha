using UnityEngine;
using TMPro;

public class NameWindow : UIWindow
{
    [Header("References")]
    [SerializeField] private CharacterStats stats;
    [SerializeField] private TMP_InputField nameInput;

    private void Start() 
    {
        DebugCommandDatabase.AddCommand(new DebugCommand(
            "shownamemenu", 
            "Shows character name menu", 
            "shownamemenu",
            (string[] parameters) => 
            {
                this.Open();
                return "";
            }));    
    }

    public void Done()
    {
        stats.charName = nameInput.text;
        Close();
    }
}
