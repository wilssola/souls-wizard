using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AchievementEditor : MonoBehaviour {

    [MenuItem("TecWolf/Atualizar Tabela De Conquistas")]
	public static void LoadTable()
    {
        FindObjectOfType<AchievementManager>().LoadAchievementsTable();
    }
}
