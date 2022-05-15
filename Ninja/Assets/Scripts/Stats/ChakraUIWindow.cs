using UnityEngine;

public class ChakraUIWindow : MultipleChoiceWindow
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private UIWindow actionWindow;
    [SerializeField] private ChakraConvertWindow convertWindow;

    [SerializeField] private ChakraUIButton[] chakraImages;

    private ChakraUIButton selected;
    private ChakraUIButton lastFocus;

    private void Start() 
    {
         foreach(ChakraUIButton i in chakraImages)
            i.SetContext(this);
    }

    private void Update() 
    {
        // Update chakra button values
        foreach(ChakraUIButton i in chakraImages)    
            i.UpdateValue(characterStats.GetChakraAmount(i.Type));
    }

    public void Select(Vector3 position, ChakraUIButton button)
    {
        if(convertWindow.IsOpen)
        {
            convertWindow.Initialize(this, selected.Type, button.Type, characterStats.GetChakraAmount(selected.Type), characterStats.GetChakraAmount(button.Type), 1f);
        }
        else
        {
            selected = button;    
            OpenActionWindow(position);
        }
    }

    public void OpenActionWindow(Vector3 position)
    {
        actionWindow.Open();
        actionWindow.transform.position = position;
    }

    public void StartConvert()
    {
        convertWindow.Open();
        convertWindow.Initialize(this, selected.Type, selected.Type, characterStats.GetChakraAmount(selected.Type), 0, 1f);
    }

    public void ApplyConvert(ChakraType from, ChakraType to, float amount)
    {
        characterStats.Convert(from, to, amount);
    }

    public void Focus()
    {
        characterStats.Focus(selected.Type);
        UpdateFocus(selected);
    }

    private void UpdateFocus(ChakraUIButton button)
    {
        if(lastFocus) lastFocus.ShowFocus(false);
        button.ShowFocus(true);
        lastFocus = button;
    }
}
