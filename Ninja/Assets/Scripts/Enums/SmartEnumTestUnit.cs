using UnityEngine;
using UnityEngine.Assertions;

public class SmartEnumTestUnit : MonoBehaviour
{
    enum Color {RED, GREEN, BLUE}
    enum Car {SUZUKI, HONDA}

    private void Start()
    {
        SmartEnum<Color> green = new SmartEnum<Color>(Color.GREEN);
        SmartEnum<Color> red = new SmartEnum<Color>(Color.RED);
        SmartEnum<Color> red2 = new SmartEnum<Color>(Color.RED);
        SmartEnum<Car> car = new SmartEnum<Car>(Car.SUZUKI);

        Assert.IsTrue(green.Equals(green), "SmartEnumTestUnit Failed: green is not equal to green");
        Assert.IsFalse(green.Equals(red), "SmartEnumTestUnit Failed: green is equal to red");
        Assert.IsTrue(red.Equals(red2), "SmartEnumTestUnit Failed: red not equal to red2");
        Assert.IsTrue(car.Equals(car), "SmartEnumTestUnit Failed: car is equal to car");
        Assert.IsFalse(red.Equals<Car>(car), "SmartEnumTestUnit Failed: red is equal to car");
    }

    // public void SendMessage(SmartEnum smartEnum)
    // {
    //     Debug.Log(smartEnum == new SmartEnum<Color>(Color.RED));
    // }
}
