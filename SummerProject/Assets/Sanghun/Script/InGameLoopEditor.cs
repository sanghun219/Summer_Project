using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InGameLoop))]
public class InGameLoopEditor : Editor
{
    public InGameLoop selected;
    public bool SpawnerFoldOut = true;

    private void OnEnable()
    {
        if (AssetDatabase.Contains(target))
        {
            selected = null;
        }
        else
        {
            selected = (InGameLoop)target;
        }
    }

    public override void OnInspectorGUI()
    {
        if (selected == null) return;

        EditorGUILayout.Space();

        SpawnerFoldOut = EditorGUILayout.Foldout(SpawnerFoldOut, "Spawner");
        if (SpawnerFoldOut)
        {
            selected.StartSpawnTime = EditorGUILayout.FloatField("스폰 시작 시간", selected.StartSpawnTime);
            selected.distSpawnerToPlayer = EditorGUILayout.FloatField("스포너와 플레이어간 거리", selected.distSpawnerToPlayer);
            selected.numOfSpawnedObj = EditorGUILayout.IntField("미리 생성할 오브젝트 개수", selected.numOfSpawnedObj);
            selected.elaspedSpawn = EditorGUILayout.FloatField("스폰 간격", selected.elaspedSpawn);
            selected.numOfSpawnPoint = EditorGUILayout.IntField("스폰 될 지점 개수", selected.numOfSpawnPoint);
            selected.lengOfSpawner = EditorGUILayout.FloatField("스포너 길이", selected.lengOfSpawner);
        }
    }
}