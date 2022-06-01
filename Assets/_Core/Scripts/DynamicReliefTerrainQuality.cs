//using UnityEngine;
//
//public class DynamicReliefTerrainQuality : MonoBehaviour
//{
//    public RTP_LODmanager RTP;
//    private const float _updateCheckFrequency = .25f;
//
//    void Start()
//    {
//        InvokeRepeating("UpdateCheck", Random.Range(0, _updateCheckFrequency), _updateCheckFrequency);
//    }
//
//    void UpdateCheck()
//    {
//        var currentQualityLevel = QualitySettings.GetQualityLevel();
//
//        if (currentQualityLevel > 4)
//        {
//            RTP.RTP_LODlevel = TerrainShaderLod.POM;
//        }
//        else if (currentQualityLevel > 2)
//        {
//            RTP.RTP_LODlevel = TerrainShaderLod.PM;
//        }
//        else
//        {
//            RTP.RTP_LODlevel = TerrainShaderLod.SIMPLE;
//        }
//    }
//}