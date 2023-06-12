using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StageLightManeuver;
using UnityEditor;
using UnityEngine;

public class GetAssetPathTest : MonoBehaviour
{

    public List<StageLightProfile> stageLightProfiles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Find")]
    public void Find()
    {
        var guids = AssetDatabase.FindAssets("t:StageLightProfile");

// GUIDからAssetPathに変換する
        var assetPaths = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();

// AssetPathから対象のScriptableObjectを取得する
        stageLightProfiles = assetPaths.Select(AssetDatabase.LoadAssetAtPath<StageLightProfile>).ToList();
    }
}
