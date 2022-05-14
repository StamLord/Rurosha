using UnityEngine;
using UnityEngine.UI;

public class ChakraUIWindow : MultipleChoiceWindow
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private UIWindow actionWindow;
    [SerializeField] private ChakraConvertWindow convertWindow;

    [SerializeField] private ChakraUIButton[] chakraImages;

    private ChakraType selected;

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

    public void Select(Vector3 position, ChakraType type)
    {
        if(convertWindow.IsOpen)
        {
            convertWindow.Initialize(this, selected, type, characterStats.GetChakraAmount(selected), characterStats.GetChakraAmount(type), 1f);
        }
        else
        {
            selected = type;    
            OpenActionWindow(position, type);
        }
    }

    public void OpenActionWindow(Vector3 position, ChakraType type)
    {
        actionWindow.Open();
        actionWindow.transform.position = position;
    }

    public void StartConvert()
    {
        convertWindow.Open();
        convertWindow.Initialize(this, selected, selected, characterStats.GetChakraAmount(selected), 0, 1f);
    }

    public void ApplyConvert(ChakraType from, ChakraType to, float amount)
    {
        characterStats.Convert(from, to, amount);
    }

    public void Focus()
    {
        characterStats.Focus(selected);
    }
}
