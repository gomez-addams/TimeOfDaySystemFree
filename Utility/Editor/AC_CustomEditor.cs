

//************************
// Custom editor methods.
//************************

//#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;


namespace AC.CustomEditor
{
	
	public abstract class AC_CustomEditor : Editor
	{


		protected Color WhiteColor{get{return Color.white;}}
		protected Color EnableButtonColor{get{return new Color(.75f, .75f, .75f, 1f);}}
		//___________________________________________________________________________________________________________


#region Color
		protected enum GUIColorType{ Background, Color }
		protected static GUIColorType ColorType;


		protected void ToggleColor(bool toggle, Color enableColor, Color disableColor, GUIColorType colorType) 
		{
			switch(ColorType)
			{
				case GUIColorType.Background :GUI.backgroundColor = toggle ? enableColor : disableColor; break;
				case GUIColorType.Color :GUI.color = toggle ? enableColor : disableColor; break;
			}
		}


		protected void ColorField(SerializedProperty color, string name, int width)
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (name, GUILayout.MinWidth(20));
			EditorGUILayout.PropertyField(color, new GUIContent(""), GUILayout.MaxWidth(width),GUILayout.MinWidth(width * .5f));
			EditorGUILayout.EndHorizontal ();
		}
#endregion


#region Curves
		protected void CurveField(string name,  SerializedProperty curve, Color color, Rect rect, int width)
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (name,GUILayout.MinWidth(20));
			curve.animationCurveValue = EditorGUILayout.CurveField ("", curve.animationCurveValue, color , rect, GUILayout.MaxWidth(width), GUILayout.MinWidth(width * .5f));
			EditorGUILayout.EndHorizontal ();
		}
#endregion


#region Text
		protected void Text(string text, GUIStyle textStyle, bool center)
		{

			if (center)
			{


				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(text, textStyle);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();

			} 
			else
			{


				GUILayout.Label (text, textStyle);
				GUI.backgroundColor = Color.white;

			}

			GUI.backgroundColor =  Color.white;
		}

		protected void Text(string text, GUIStyle textStyle, bool center, int width)
		{

			if (center)
			{


				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(text, textStyle, GUILayout.Width(width));
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();

			} 
			else
			{


				GUILayout.Label (text, textStyle, GUILayout.Width(width));
				GUI.backgroundColor = Color.white;

			}

			GUI.backgroundColor =  Color.white;
		}
#endregion


#region Separator
		protected void Separator(Color color, int height)
		{
			GUI.color = color;
			GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(height)});
			GUI.color = WhiteColor;
		}
#endregion


#region Progress Bar
		protected void ProgressBar(float value, string name)
		{

			EditorGUI.ProgressBar (GUILayoutUtility.GetRect(0,20),  value/100f, name);
		}

		protected void ProgressBar(Rect rect, float value, string name)
		{

			EditorGUI.ProgressBar (rect, value/100f, name);
		}
#endregion


#region Buttons
		protected void ToggleButton(SerializedProperty toggle, string name) 
		{

			ToggleColor(toggle.boolValue, EnableButtonColor, WhiteColor, GUIColorType.Background);

			if (GUILayout.Button (name, EditorStyles.miniButton, GUILayout.MaxWidth(20), GUILayout.MaxHeight(16)))
				toggle.boolValue = !toggle.boolValue;

			GUI.backgroundColor = Color.white;
		}
#endregion


	}

}
//#endif