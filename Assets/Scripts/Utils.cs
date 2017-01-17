using UnityEngine;

static class Utils
{
    public static bool Clicked()
    {
        bool clicked;
        if (Application.platform == RuntimePlatform.Android)
        {
            clicked = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        }
        else
        {
            clicked = Input.GetMouseButtonDown(0);
        }

        return clicked;
    }

    public static Vector3 GetClickedPosition()
    {
        Vector3 clickedPosition = new Vector3();
        if (Application.platform == RuntimePlatform.Android)
            clickedPosition = Input.GetTouch(0).position;
        else
            clickedPosition = Input.mousePosition;

        return clickedPosition;
    }
}
