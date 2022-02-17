using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JustAssets.TerrainUtility
{
    /// <summary>
    ///     Splits terrain in nxn pieces.
    /// </summary>
    public sealed class SplitTerrain : IValidate<Terrain>
    {
        public enum PowerOf2Squared
        {
            _2x2 = 1,

            _4x4,

            _8x8,

            _16x16,

            _32x32,

            _64x64,

            _128x128
        }

        private const int MINIMAL_HEIGHTMAP_RESOLUTION = 33;

        private const int MINIMAL_CONTROL_TEXTURE_RESOLUTION = 16;

        private const int MINIMAL_BASE_TEXTURE_RESOLUTION = 16;

        private const int MINIMAL_DETAIL_RESOLUTION = 8;

        private bool _doSplitTrees;

        private PowerOf2Squared _targetPiecesPerAxis;

        public Terrain[] Selection { get; set; }

        public bool IsValid(out string reason)
        {
            if (Selection == null)
            {
                reason = "Select some terrain.";
                return false;
            }

            var setHelpString = "";
            if (Selection.Count(x => x != null) == 0)
            {
                setHelpString =
                    $"Currently no source terrain is selected. Please select at least one terrain. Each terrain will be split into {_targetPiecesPerAxis} pieces.";
            }

            reason = setHelpString;
            return string.IsNullOrEmpty(setHelpString);
        }

        public void Split(TerrainUtility.SaveInfo saveInfo, bool doSplitTrees = true, PowerOf2Squared targetPiecesPerAxis = PowerOf2Squared._2x2)
        {
            _doSplitTrees = doSplitTrees;
            _targetPiecesPerAxis = targetPiecesPerAxis;

            string reason;

            if (!IsValid(out reason))
                throw new Exception(reason);

            if (Selection.Length == 0)
                return;

            var piecesPerAxis = 1 << (int) _targetPiecesPerAxis;

            for (var parentTerrainIndex = 0; parentTerrainIndex < Selection.Length; parentTerrainIndex++)
            {
                Terrain sourceTerrain = Selection[parentTerrainIndex];
                EditorUtility.DisplayProgressBar("Split terrain", "Process " + parentTerrainIndex, (float) parentTerrainIndex / Selection.Length);
                SplitTerrainTile(piecesPerAxis, sourceTerrain,saveInfo);
            }

            EditorUtility.ClearProgressBar();
        }

        private static void CopyPrototypes(TerrainData targetTerrainData, TerrainData sourceTerrainData)
        {
            targetTerrainData.terrainLayers = sourceTerrainData.terrainLayers;
            targetTerrainData.detailPrototypes = sourceTerrainData.detailPrototypes;
            targetTerrainData.treePrototypes = sourceTerrainData.treePrototypes;
        }

        private void CopyTerrainProperties(Terrain sourceTerrain, Terrain targetTerrain, int piecesPerAxis)
        {
            targetTerrain.basemapDistance = sourceTerrain.basemapDistance;
            targetTerrain.shadowCastingMode = sourceTerrain.shadowCastingMode;
            targetTerrain.detailObjectDensity = sourceTerrain.detailObjectDensity;
            targetTerrain.detailObjectDistance = sourceTerrain.detailObjectDistance;
            targetTerrain.heightmapMaximumLOD = sourceTerrain.heightmapMaximumLOD;
            targetTerrain.heightmapPixelError = sourceTerrain.heightmapPixelError;
            targetTerrain.treeBillboardDistance = sourceTerrain.treeBillboardDistance;
            targetTerrain.treeCrossFadeLength = sourceTerrain.treeCrossFadeLength;
            targetTerrain.treeDistance = sourceTerrain.treeDistance;
            targetTerrain.treeMaximumFullLODCount = sourceTerrain.treeMaximumFullLODCount;
            targetTerrain.bakeLightProbesForTrees = sourceTerrain.bakeLightProbesForTrees;
            targetTerrain.drawInstanced = sourceTerrain.drawInstanced;
            targetTerrain.reflectionProbeUsage = sourceTerrain.reflectionProbeUsage;
            targetTerrain.realtimeLightmapScaleOffset = sourceTerrain.realtimeLightmapScaleOffset;
            targetTerrain.lightmapScaleOffset = sourceTerrain.lightmapScaleOffset;

            SetLightmapScale(sourceTerrain, targetTerrain, piecesPerAxis);

            targetTerrain.terrainData.wavingGrassAmount = sourceTerrain.terrainData.wavingGrassAmount;
            targetTerrain.terrainData.wavingGrassSpeed = sourceTerrain.terrainData.wavingGrassSpeed;
            targetTerrain.terrainData.wavingGrassStrength = sourceTerrain.terrainData.wavingGrassStrength;
            targetTerrain.terrainData.wavingGrassTint = sourceTerrain.terrainData.wavingGrassTint;
        }

        private void SetLightmapScale(Terrain sourceTerrain, Terrain targetTerrain, int piecesPerAxis)
        {
            var sos = new SerializedObject(sourceTerrain);
            var sourceValue = sos.FindProperty("m_ScaleInLightmap").floatValue;
            sos.ApplyModifiedProperties();

            var so = new SerializedObject(targetTerrain);
            so.FindProperty("m_ScaleInLightmap").floatValue = sourceValue / piecesPerAxis;
            so.ApplyModifiedProperties();
        }

        private static void SetTerrainSlicePosition(Terrain sourceTerrain, int piecesPerAxis, int sliceIndex, Transform terrainTransform)
        {
            Vector3 parentPosition = sourceTerrain.GetPosition();

            var spaceShiftX = sourceTerrain.terrainData.size.z / piecesPerAxis;
            var spaceShiftY = sourceTerrain.terrainData.size.x / piecesPerAxis;

            var xWShift = sliceIndex % piecesPerAxis * spaceShiftX;
            var zWShift = sliceIndex / piecesPerAxis * spaceShiftY;

            terrainTransform.position = new Vector3(terrainTransform.position.x + zWShift, terrainTransform.position.y, terrainTransform.position.z + xWShift);

            terrainTransform.position = new Vector3(terrainTransform.position.x + parentPosition.x, terrainTransform.position.y + parentPosition.y,
                terrainTransform.position.z + parentPosition.z);
        }

        private static void SplitControlTexture(TerrainData targetTerrainData, int piecesPerAxis, int sliceIndex, TerrainData sourceTerrainData)
        {
            var sourceControlTextureResolution = sourceTerrainData.alphamapResolution;
            var sourceControlTextureResolutionMinus1 = sourceControlTextureResolution - 1;
            var sourceBaseMapResolution = sourceTerrainData.baseMapResolution;
            var targetControlTextureResolution = Math.Max(MINIMAL_CONTROL_TEXTURE_RESOLUTION, sourceControlTextureResolution / piecesPerAxis);
            var targetBaseMapResolution = Math.Max(MINIMAL_BASE_TEXTURE_RESOLUTION, sourceBaseMapResolution / piecesPerAxis);

            targetTerrainData.alphamapResolution = targetControlTextureResolution;
            targetTerrainData.baseMapResolution = targetBaseMapResolution;

            var sourceControlTextures = sourceTerrainData.GetAlphamaps(0, 0, sourceControlTextureResolution, sourceControlTextureResolution);
            var targetControlTextures = new float[targetControlTextureResolution, targetControlTextureResolution, sourceTerrainData.alphamapLayers];

            var xShift = sliceIndex % piecesPerAxis * sourceControlTextureResolution / piecesPerAxis;
            var yShift = sliceIndex / piecesPerAxis * sourceControlTextureResolution / piecesPerAxis;
            var sampleRatio = targetControlTextureResolution * piecesPerAxis / (float) sourceControlTextureResolution;

            for (var s = 0; s < sourceTerrainData.alphamapLayers; s++)
            {
                for (var x = 0; x < targetControlTextureResolution; x++)
                {
                    if (x % 100 == 0)
                        EditorUtility.DisplayProgressBar("Split terrain", "Split splat", (float) x / targetControlTextureResolution);

                    var xPos = xShift + x / sampleRatio;
                    for (var y = 0; y < targetControlTextureResolution; y++)
                    {
                        var yPos = yShift + y / sampleRatio;
                        var layerIndex = s;
                        var ph = BilinearInterpolator.Interpolate(sourceControlTextures, layerIndex, xPos, yPos, sourceControlTextureResolutionMinus1);

                        targetControlTextures[x, y, s] = ph;
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            targetTerrainData.SetAlphamaps(0, 0, targetControlTextures);
        }

        private static void SplitDetailMap(TerrainData targetTerrainData, int piecesPerAxis, int sliceIndex, TerrainData sourceTerrainData)
        {
            var sourceResolution = sourceTerrainData.detailResolution;
            var targetResolution = Math.Max(MINIMAL_DETAIL_RESOLUTION, sourceResolution / piecesPerAxis);
            var detailResolutionPerPatch = Math.Min(targetResolution, Math.Max(MINIMAL_DETAIL_RESOLUTION, sourceTerrainData.detailResolutionPerPatch));

            targetTerrainData.SetDetailResolution(targetResolution, detailResolutionPerPatch);

            var xShift = sliceIndex % piecesPerAxis * sourceResolution / piecesPerAxis;
            var yShift = sliceIndex / piecesPerAxis * sourceResolution / piecesPerAxis;
            var sampleRatio = targetResolution * piecesPerAxis / (float) sourceResolution;

            var sourceResolutionMinus1 = sourceResolution - 1;

            for (var detLay = 0; detLay < sourceTerrainData.detailPrototypes.Length; detLay++)
            {
                var parentDetail = sourceTerrainData.GetDetailLayer(0, 0, sourceResolution, sourceResolution, detLay);

                var detailResolution = targetResolution;
                var pieceDetail = new int[detailResolution, detailResolution];

                for (var x = 0; x < targetResolution; x++)
                {
                    if (x % 100 == 0)
                        EditorUtility.DisplayProgressBar("Split terrain", "Split details", (float) x / targetResolution);

                    var xPos = xShift + x / sampleRatio;
                    for (var y = 0; y < targetResolution; y++)
                    {
                        var yPos = yShift + y / sampleRatio;
                        var ph = BilinearInterpolator.Interpolate(parentDetail, xPos, yPos, sourceResolutionMinus1);

                        pieceDetail[x, y] = (int) (ph / sampleRatio);
                    }
                }

                EditorUtility.ClearProgressBar();

                targetTerrainData.SetDetailLayer(0, 0, detLay, pieceDetail);
            }
        }

        private static void SplitHeightMap(TerrainData targetTerrainData, int piecesPerAxis, int sliceIndex, TerrainData sourceTerrainData)
        {
            var sourceHeightmapResolutionPlusOne = sourceTerrainData.heightmapResolution;
            var sourceHeightmapResolution = sourceHeightmapResolutionPlusOne - 1;
            var targetHeightmapResolution = sourceHeightmapResolution / piecesPerAxis;
            targetHeightmapResolution = Math.Max(MINIMAL_HEIGHTMAP_RESOLUTION - 1, targetHeightmapResolution);

            targetTerrainData.heightmapResolution = targetHeightmapResolution;
            targetTerrainData.size = new Vector3(sourceTerrainData.size.x / piecesPerAxis, sourceTerrainData.size.y, sourceTerrainData.size.z / piecesPerAxis);

            var pieceHeight = new float[targetHeightmapResolution + 1, targetHeightmapResolution + 1];

            var sampleRatio = targetHeightmapResolution * piecesPerAxis / (float) sourceHeightmapResolution;
            var xShift = sliceIndex % piecesPerAxis * sourceHeightmapResolution / piecesPerAxis;
            var yShift = sliceIndex / piecesPerAxis * sourceHeightmapResolution / piecesPerAxis;

            var parentHeights = sourceTerrainData.GetHeights(0, 0, sourceHeightmapResolutionPlusOne, sourceHeightmapResolutionPlusOne);
            var parentWidth = parentHeights.GetLength(0);
            var parentWidthMinus1 = parentWidth - 1;

            for (var x = 0; x <= targetHeightmapResolution; x++)
            {
                if (x % 100 == 0)
                    EditorUtility.DisplayProgressBar("Split terrain", "Split height", (float) x / targetHeightmapResolution);

                var xPos = xShift + x / sampleRatio;
                for (var y = 0; y <= targetHeightmapResolution; y++)
                {
                    var yPos = yShift + y / sampleRatio;
                    var ph = BilinearInterpolator.Interpolate(parentHeights, xPos, yPos, parentWidthMinus1);

                    pieceHeight[x, y] = ph;
                }
            }

            EditorUtility.ClearProgressBar();

            targetTerrainData.SetHeights(0, 0, pieceHeight);
        }

        void SplitGrid(GameObject terrainGameObject)
        {
            GameObject newObject = MyFramework.MeshUtil.CreateGridObject(MyFramework.CellGroup.GetSize());
            newObject.transform.SetParent(terrainGameObject.transform);
            newObject.transform.localPosition = new Vector3(0, 0.01f, 0f);
        }

        private void SplitTerrainTile(int piecesPerAxis, Terrain sourceTerrain, TerrainUtility.SaveInfo saveInfo)
        {
            string prefabFolderPath = AssetDatabase.GetAssetPath(saveInfo.PrefabFolder) + "/";

            TerrainData sourceTerrainData = sourceTerrain.terrainData;
            var terrains = new Terrain[piecesPerAxis * piecesPerAxis];

            //Split terrain 
            for (var sliceIndex = 0; sliceIndex < terrains.Length; sliceIndex++)
            {
                var targetTerrainData = new TerrainData();
                GameObject terrainGameObject = Terrain.CreateTerrainGameObject(targetTerrainData);

                terrainGameObject.name = $"{saveInfo.TerrainName}_{sliceIndex}";

                Terrain targetTerrain = terrains[sliceIndex] = terrainGameObject.GetComponent<Terrain>();
                targetTerrain.terrainData = targetTerrainData;

                AssetDatabase.CreateAsset(targetTerrainData, prefabFolderPath + terrainGameObject.name + ".asset");
                //AssetDatabase.CreateAsset(terrainGameObject, prefabFolderPath + terrainGameObject.name + ".prefab");

                CopyPrototypes(targetTerrainData, sourceTerrainData);
                CopyTerrainProperties(sourceTerrain, targetTerrain, piecesPerAxis);
                SetTerrainSlicePosition(sourceTerrain, piecesPerAxis, sliceIndex, terrainGameObject.transform);
                SplitHeightMap(targetTerrainData, piecesPerAxis, sliceIndex, sourceTerrainData);
                SplitControlTexture(targetTerrainData, piecesPerAxis, sliceIndex, sourceTerrainData);
                SplitDetailMap(targetTerrainData, piecesPerAxis, sliceIndex, sourceTerrainData);
                SplitGrid(targetTerrain.gameObject);

                PrefabUtility.SaveAsPrefabAsset(terrainGameObject, prefabFolderPath + terrainGameObject.name + ".prefab");
                AssetDatabase.SaveAssets();
            }

            SplitTrees(piecesPerAxis, terrains, sourceTerrain);
            AssetDatabase.SaveAssets();

            //foreach(var terrain in terrains)
            //{
            //    GameObject.DestroyImmediate(terrain.gameObject);
            //}
        }

        private void SplitTrees(int terraPieces, Terrain[] tiles, Terrain sourceTerrain)
        {
            if (!_doSplitTrees)
                return;

            var terrainDataTreeInstances = sourceTerrain.terrainData.treeInstances;

            var stepSize = 1f / terraPieces;

            for (var t = 0; t < terrainDataTreeInstances.Length; t++)
            {
                if (t % 100 == 0)
                    EditorUtility.DisplayProgressBar("Split terrain", "Split trees ", (float) t / terrainDataTreeInstances.Length);

                // Get tree instance					
                TreeInstance ti = terrainDataTreeInstances[t];

                for (var i = 0; i < tiles.Length; i++)
                {
                    var splitRect = new Rect(i / terraPieces * stepSize, i % terraPieces * stepSize, stepSize, stepSize);

                    if (!splitRect.Contains(new Vector2(ti.position.x, ti.position.z)))
                        continue;

                    // Recalculate new tree position	
                    ti.position = new Vector3((ti.position.x - splitRect.x) * terraPieces, ti.position.y, (ti.position.z - splitRect.y) * terraPieces);

                    // Add tree instance						
                    tiles[i]?.AddTreeInstance(ti);
                }
            }
        }
    }
}