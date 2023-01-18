using UnityEngine;

public static class ExtensionMethods {
    public static int ToInt(this float self) {
        return (int)Mathf.Floor(self);
    }
}