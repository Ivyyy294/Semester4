using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
	Editor shapeEditor;
	Editor colorEditor;
	Editor grassEditor;
	Editor waterEditor;
	Editor rockEditor;

	public override void OnInspectorGUI()
	{
		using (var check = new EditorGUI.ChangeCheckScope())
		{
			base.OnInspectorGUI();

			if (check.changed)
				planet.GeneratePlanet();
		}
		
		DrawSettingsEditor (planet.m_shapeSetting, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);

		//Shader
		DrawSettingsEditor (planet.m_colorSetting, planet.OnShaderSettingsUpdated, ref planet.colorSettingsFoldout, ref colorEditor);
		DrawSettingsEditor (planet.m_grassSetting, planet.OnShaderSettingsUpdated, ref planet.grassSettingsFoldout, ref grassEditor);
		DrawSettingsEditor (planet.m_waterSetting, planet.OnShaderSettingsUpdated, ref planet.waterSettingsFoldout, ref waterEditor);

		//Terrain Objects
		DrawSettingsEditor (planet.m_rockSetting, planet.OnTerrainObjectSettingsUpdated, ref planet.rockSettingsFoldout, ref rockEditor);
	}

	void DrawSettingsEditor (Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
	{
		if (settings == null)
			return;

		foldout = EditorGUILayout.InspectorTitlebar (foldout, settings);
		
		using (var check = new EditorGUI.ChangeCheckScope())
		{
			if (foldout)
			{
				CreateCachedEditor (settings, null, ref editor);
				editor.OnInspectorGUI();

				if (check.changed)
				{
					if (onSettingsUpdated != null)
						onSettingsUpdated();
				}
			}
		}
	}

	private void OnEnable()
	{
		planet = (Planet)target;
		planet.GeneratePlanet();
	}
}
