using UnityEngine;

public class TimeSkipper : MonoBehaviour
{
    public bool TimeSkip(int hours, int minutes, int seconds)
    {
        if(ValidateSkip() == false)
            return false;

        DayNightManager.instance.ProgressTime(hours, minutes, seconds);
        return true;
    }

    private bool ValidateSkip()
    {
        // TODO: Add validations like enemies in area
        return true;
    }
}
