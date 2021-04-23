using System;
using UnityEngine;

public class SimplePauseMenu : MonoBehaviour
{
	[SerializeField] private int baseFontSize = 24;
	[SerializeField] private Color headingColor = Color.white;
	[SerializeField] private Color buttonsColor = Color.white;

	private bool menuEnabled = false;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			menuEnabled = !menuEnabled;
		}
	}

	void OnGUI()
	{
		if (menuEnabled) ShowPauseMenu();
	}

	private void ShowPauseMenu()
	{
		ShowBackground();
		ShowHeading("Pause Menu", baseFontSize * 1.8f);
		ShowButton("Resume", 0f, () => menuEnabled = !menuEnabled);
		ShowButton("Exit", -baseFontSize * 1.6f, Application.Quit);
	}

	private void ShowBackground()
	{
		int w = Screen.width, h = Screen.height;
		GUIStyle style = new GUIStyle(GUI.skin.box);

		GUI.Box(new Rect(0, 0, w, h), "",style);
	}

	private void ShowHeading(string text, float yOffset = 0f)
	{
		GUIStyle headingStyle = new GUIStyle(GUI.skin.label)
		{
			alignment = TextAnchor.UpperCenter,
			fontSize = (int) (baseFontSize * 1.5f),
			normal = {textColor = headingColor},
			fontStyle = FontStyle.Bold
		};

		Rect labelRect = GetCenteredRect(yOffset, headingStyle.fontSize);
		GUI.Label(labelRect, text, headingStyle);
	}

	private void ShowButton(string text, float yOffset, Action onClick)
	{
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
		{
			alignment = TextAnchor.MiddleCenter,
			fontSize = baseFontSize,
			normal = {textColor = buttonsColor}
		};

		Rect buttonRect = GetCenteredRect(yOffset, baseFontSize);
		bool clicked = GUI.Button(buttonRect, text, buttonStyle);

		if (clicked) onClick();
	}

	private Rect GetCenteredRect(float yOffset, float fontSize)
	{
		int w = Screen.width, h = Screen.height;

		float rectWidth = (float)w / 2;
		float rectHeight = fontSize * 1.4f;

		float x = w / 2f - rectWidth / 2;
		float y = h / 2f - rectHeight / 2 - yOffset;

		return new Rect(x, y, rectWidth, rectHeight);
	}
}