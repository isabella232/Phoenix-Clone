using UnityEngine;

public static class CustomStyle {
	public static Font font = Resources.Load<Font>("astron boy");

	public static GUIStyle SetStyleData(GUIStyle style) {
		style.fontSize = 24;
		style.font = font;
		style.normal.textColor = Color.white;
		return style;
	}

	public static GUIStyle GetLabelStyle() {
		return SetStyleData(GUI.skin.GetStyle("label"));
	}
}